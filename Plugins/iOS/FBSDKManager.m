//
//  FBSDKManager.m
//  UnityFramework
//
//  Created by 晴天网络 on 2023/3/6.
//

#import <Foundation/Foundation.h>
#import "FBSDKManager.h"
#import "FBSDKCoreKit/FBSDKCoreKit.h"
#import "FBSDKCoreKit/FBSDKAppEvents.h"
#import "FBSDKShareKit/FBSDKShareKit.h"
#import "FBSDKLoginKit/FBSDKLoginKit.h"
#import "FBUtility.h"
#import "UnityAppController.h"

@implementation FBSDKManager

static FBSDKManager* _instance;

+ (FBSDKManager *) instance {
    static dispatch_once_t token;
    dispatch_once(&token, ^{
        if(_instance == nil)
        {
            _instance = [[self alloc] init];
        }
    });
    return _instance;
}

-(BOOL) initSdk:(UIApplication *)application
didFinishLaunchingWithOptions:(nullable NSDictionary<UIApplicationLaunchOptionsKey, id> *)launchOptions{
    [FBSDKApplicationDelegate.sharedInstance application:application didFinishLaunchingWithOptions:launchOptions];
    [FBSDKAppEvents.shared setIsUnityInitialized:true];
    [FBSDKAppEvents.shared sendEventBindingsToUnity];
    [FBSDKAppEvents.shared activateApp];
    self.initialized = YES;
    return YES;
}

-(BOOL)application:(UIApplication *)app
            openURL:(NSURL *)url
           options:(NSDictionary<UIApplicationOpenURLOptionsKey, id> *)options{
    NSLog(@"application===");
    [FBSDKApplicationDelegate.sharedInstance application:app openURL:url options:options];
    return YES;
}

-(void) setAdvertiserTrackingEnabled:(BOOL) enable{
    FBSDKSettings.sharedSettings.isAdvertiserTrackingEnabled = enable;
}

-(void) logInWithReadPermissions{
    [self startlogin:NO];
}

-(void) loginWithPublishPermissions{
    [self startlogin:YES];
}

-(void) logOut{
    FBSDKLoginManager * login = [[FBSDKLoginManager alloc] init];
    [login logOut];
}

-(BOOL) isLogged{
    FBSDKAccessToken * token = [FBSDKAccessToken currentAccessToken];
    if (token && !token.isExpired) {
        return YES;
    }
    FBSDKAuthenticationToken * token2 = [FBSDKAuthenticationToken currentAuthenticationToken];
    if (token2) {
        return YES;
    }
    
    return NO;
}

-(void) logPurchase:(double) price
           currency:(const char*) currency
               pars:(const char*) pars{
    
    NSString * ns_currency = [NSString stringWithUTF8String:currency];
    NSDictionary * parsMap = [FBUtility chatPars2Map:pars];
    [FBSDKAppEvents.shared logPurchase:price currency:ns_currency parameters:parsMap];
}

-(void) logEvent:(const char *) eventName
      valueToSum:(double) valueToSum
            pars:(const char*) pars{
    NSString * ns_eventName = [NSString stringWithUTF8String:eventName];
    NSDictionary * parsMap = [FBUtility chatPars2Map:pars];
    [FBSDKAppEvents.shared logEvent:ns_eventName valueToSum:valueToSum parameters:parsMap];
}

-(void) logEvent:(const char *) eventName
            pars:(const char*) pars{
    NSString * ns_eventName = [NSString stringWithUTF8String:eventName];
    NSDictionary * parsMap = [FBUtility chatPars2Map:pars];
    [FBSDKAppEvents.shared logEvent:ns_eventName parameters:parsMap];
}

-(void) shareLink:(NSURL*) linkUri{
    if (linkUri) {
        FBSDKShareLinkContent* content = [[FBSDKShareLinkContent alloc] init];
        content.contentURL = linkUri;
        [self startShare:content];
    }
}

-(void) shareImage:(NSArray<UIImage*>*) imageList{
    if (imageList) {
        // FB图片Object集合
        NSMutableArray<FBSDKSharePhoto*>* photoList = [NSMutableArray array];
        for (int i = 0; i < imageList.count; i++) {
            FBSDKSharePhoto* photo = [[FBSDKSharePhoto alloc] initWithImage:imageList[i] isUserGenerated:NO];
            [photoList addObject:photo];
        }
        FBSDKSharePhotoContent* content = [[FBSDKSharePhotoContent alloc] init];
        content.photos = photoList;
        [self startShare:content];
    }
}

-(void) shareVideo:(NSURL*) videoUri{
    if (videoUri) {
        FBSDKShareVideo* video = [[FBSDKShareVideo alloc] initWithVideoURL:videoUri previewPhoto:nil];
        FBSDKShareVideoContent* content = [[FBSDKShareVideoContent alloc] init];
        content.video = video;
        [self startShare:content];
    }
}

-(void) shareMedia:(NSArray<UIImage*>*) imageList
         videoList:(NSArray<NSURL*>*) videoList{
    NSLog(@"未接入");
    NSMutableArray* mediaArray = [NSMutableArray array];
    for (int i = 0; i < imageList.count; i++) {
        FBSDKSharePhoto* photo = [[FBSDKSharePhoto alloc] initWithImage:imageList[i] isUserGenerated:NO];
        [mediaArray addObject:photo];
    }
    
    for (int i = 0; i < videoList.count; i++) {
        FBSDKShareVideo* video = [[FBSDKShareVideo alloc] initWithVideoURL:videoList[i] previewPhoto:nil];
        [mediaArray addObject:video];
    }
    
    FBSDKShareMediaContent* content = [[FBSDKShareMediaContent alloc] init];
    [self startShare:content];
}

-(void) startShare:(id<FBSDKSharingContent>)content{
    [FBSDKShareDialog showFromViewController:GetAppController().rootViewController
                                 withContent:content delegate:self];
}

-(void) startlogin:(BOOL) isPublish{
    NSString* fbPermissions = @"public_profile,email";
    //NSString* fbPermissions = @"public_profile,email,gaming_profile,gaming_user_picture";
    NSArray * permissions = [fbPermissions componentsSeparatedByString:@","];
    NSLog(@"startlogin===");
    void(^ loginHandler)(FBSDKLoginManagerLoginResult *, NSError *) = ^(FBSDKLoginManagerLoginResult * result, NSError * error){
        if (error) {
            NSLog(@"===login error: %@", error.domain);
            if (self.onCompleteLogin) {
                self.onCompleteLogin(false, [error.domain UTF8String]);
            }
            return;
        }else if (result.isCancelled){
            NSLog(@"===user cancel");
            if (self.onCompleteLogin) {
                self.onCompleteLogin(false, "user cancel");
            }
            return;
        }
        
        if ([self tryCompleteLogin]) {
            if (self.onCompleteLogin) {
                self.onCompleteLogin(true, "success");
            }
        }else{
            NSLog(@"===unknow login error");
            if (self.onCompleteLogin) {
                self.onCompleteLogin(false, "unknow login error");
            }
        }
    };
    
    FBSDKLoginManager * login = [[FBSDKLoginManager alloc] init];
    [login logInWithPermissions:permissions fromViewController:nil handler:loginHandler];
}

-(BOOL) tryCompleteLogin{
    NSLog(@"tryCompleteLogin===");
    NSMutableDictionary * userData = [[NSMutableDictionary alloc] init];
    
    NSDictionary * accessTokenUserData = [self getAccessTokenUserData];
    NSDictionary * authenticationTokenUserData = [self getAuthenticationTokenUserData];
    if (accessTokenUserData) {
        [userData addEntriesFromDictionary:accessTokenUserData];
    }
    if (authenticationTokenUserData) {
        [userData addEntriesFromDictionary:authenticationTokenUserData];
    }
    if (userData) {
        return YES;
    }
    else{
        return NO;
    }
}

-(NSDictionary *) getAccessTokenUserData{
    FBSDKAccessToken * token = [FBSDKAccessToken currentAccessToken];
    if (token) {
        NSDictionary * userData = [FBUtility getUserDataFromAccessToken:token];
        if (userData) {
            return userData;
        }
        else{
            [[[FBSDKLoginManager alloc] init] logOut];
        }
    }
    
    return nil;
}

-(NSDictionary *) getAuthenticationTokenUserData{
    FBSDKAuthenticationToken * token = [FBSDKAuthenticationToken currentAuthenticationToken];
    if (token.tokenString && token.nonce) {
        return @{
            @"auth_token_string":token.tokenString,
            @"auth_nonce":token.nonce
        };
    }
    
    return nil;
}

#pragma mark - 分享回调实现

/**
  Sent to the delegate when the share completes without error or cancellation.
 @param sharer The FBSDKSharing that completed.
 @param results The results from the sharer.  This may be nil or empty.
 */
- (void)sharer:(id<FBSDKSharing>)sharer didCompleteWithResults:(NSDictionary<NSString *, id> *)results{
    NSLog(@"分享成功");
    if (self.onCompleteShare) {
        self.onCompleteShare(true, "");
    }
}

/**
  Sent to the delegate when the sharer encounters an error.
 @param sharer The FBSDKSharing that completed.
 @param error The error.
 */
- (void)sharer:(id<FBSDKSharing>)sharer didFailWithError:(NSError *)error{
    NSLog(@"分享失败：%@", error.domain);
    if (self.onCompleteShare) {
        self.onCompleteShare(false, [error.domain UTF8String]);
    }
}

/**
  Sent to the delegate when the sharer is cancelled.
 @param sharer The FBSDKSharing that completed.
 */
- (void)sharerDidCancel:(id<FBSDKSharing>)sharer{
    NSLog(@"用户取消分享");
    if (self.onCompleteShare) {
        self.onCompleteShare(false, "cancel");
    }
}

#pragma mark - Unity FacebookSDKManager 脚本中的接口

#if __cplusplus
extern "C" {
#endif
    /**
     初始化FBSDK
     */
    void fb_sdkInitialize(){
        if ([FBSDKManager instance].initialized){
            NSLog(@"facebook===已初始化");
        }else{
            NSLog(@"facebook===未初始化");
        }
    }
    
    /**
     检测FB登陆情况
     */
    void fb_logInWithReadPermissions(UnityOnCompleteLogin onCompleteLogin){
        [FBSDKManager instance].onCompleteLogin = onCompleteLogin;
        [[FBSDKManager instance] logInWithReadPermissions];
    }
    
    bool fb_isLogged(){
        return [[FBSDKManager instance] isLogged];
    }
    
    void fb_logout(){
        [[FBSDKManager instance] logOut];
    }
    
    /**
     分享图片
     @param imagePath 图片本地路径
     */
    void fb_shareImage(const char* imagePath, UnityOnCompleteShare onCompleteShare){
        NSString *path = [NSString stringWithUTF8String:imagePath];
        UIImage* image = [UIImage imageWithContentsOfFile:path];
        NSMutableArray* imageList = [NSMutableArray array];
        [imageList addObject:image];
        [FBSDKManager instance].onCompleteShare = onCompleteShare;
        [[FBSDKManager instance] shareImage:imageList];
    }
    
    /**
     分享图片
     @param imagePath 图片本地路径
     */
    void fb_shareImageWithDatas(const Byte* datas, int length, UnityOnCompleteShare onCompleteShare){
        NSLog(@"fb_shareImageWithDatas");
        NSData * imageData = [NSData dataWithBytes:datas length:length];
        NSLog(@"imageData");
        UIImage* image = [UIImage imageWithData:imageData];
        NSLog(@"image");
        NSMutableArray* imageList = [NSMutableArray array];
        NSLog(@"imageList");
        [imageList addObject:image];
        NSLog(@"addObject");
        [FBSDKManager instance].onCompleteShare = onCompleteShare;
        NSLog(@"onCompleteShare");
        [[FBSDKManager instance] shareImage:imageList];
    }
    
    /**
     分享链接
     @param way Unity内部分享途径的字符串
     @param uri 链接地址
     */
    void fb_shareLink(const char* uri, UnityOnCompleteShare onCompleteShare){
        NSString *path = [NSString stringWithUTF8String:uri];
        NSURL* url = [NSURL URLWithString:path];
        [FBSDKManager instance].onCompleteShare = onCompleteShare;
        [[FBSDKManager instance] shareLink:url];
    }
    
    /**
     分享视频
     @param way Unity内部分享途径的字符串
     @param uri 视频本地路径
     */
    void fb_shareVideo(const char* uri, UnityOnCompleteShare onCompleteShare){
        NSString *path = [NSString stringWithUTF8String:uri];
        NSURL* url = [NSURL URLWithString:path];
        [FBSDKManager instance].onCompleteShare = onCompleteShare;
        [[FBSDKManager instance] shareVideo:url];
    }
    
    void fb_logPurchase(const char* logName, double logPurchase, const char* currency, const char* pars){
        [[FBSDKManager instance] logPurchase:logPurchase
                                    currency:currency
                                        pars:pars];
    }
    
    void fb_logEvent(const char* logName, double valueToNum, const char* pars){
        [[FBSDKManager instance] logEvent:logName valueToSum:valueToNum pars:pars];
    }
    
    void fb_logEvent(const char* logName, const char* pars){
        [[FBSDKManager instance] logEvent:logName pars:pars];
    }
    
    void fb_setAdvertiserTrackingEnabled(bool enabled){
        [[FBSDKManager  instance] setAdvertiserTrackingEnabled:enabled];
    }
#if __cplusplus
}
#endif

@end
