//
//  FBSDKManager.h
//  UnityFramework
//
//  Created by 晴天网络 on 2023/3/6.
//

#import "FBSDKCoreKit/FBSDKCoreKit.h"
#import "FBSDKShareKit/FBSDKShareKit.h"
#import "CommonApi.h"

/**
 完成登陆回调事件
 */
@interface FBSDKManager : NSObject<FBSDKSharingDelegate>

@property (nonatomic, assign) BOOL initialized;
@property (nonatomic, assign) U3DBridgeCallback_Success onSuccess;
@property (nonatomic, assign) U3DBridgeCallback_Cancel onCancel;
@property (nonatomic, assign) U3DBridgeCallback_Error onError;

+ (FBSDKManager *) instance;

-(BOOL) initSdk:(UIApplication *)application
didFinishLaunchingWithOptions:(nullable NSDictionary<UIApplicationLaunchOptionsKey, id> *)launchOptions;

-(BOOL)application:(UIApplication *)app
            openURL:(NSURL *)url
            options:(NSDictionary<UIApplicationOpenURLOptionsKey, id> *)options;

-(void) setAdvertiserTrackingEnabled:(BOOL) enable;

-(void) logInWithReadPermissions;

-(void) loginWithPublishPermissions;

-(void) logOut;

-(BOOL) isLogged;

-(void) logPurchase:(double) price
           currency:(const char*) currency
               pars:(const char*) pars;

-(void) logEvent:(const char *) eventName
      valueToSum:(double) valueToSum
            pars:(const char*) pars;

-(void) logEvent:(const char *) eventName
            pars:(const char*) pars;

-(void) shareLink:(NSURL*) linkUri;

-(void) shareImage:(NSArray<UIImage*>*) imageList;

-(void) shareVideo:(NSURL*) videoUri;

-(void) shareMedia:(NSArray<UIImage*>*) imageList
         videoList:(NSArray<NSURL*>*) videoList;

@end
