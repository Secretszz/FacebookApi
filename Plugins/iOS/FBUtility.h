//
//  FBUtility.h
//  UnityFramework
//
//  Created by 晴天网络 on 2023/3/6.
//

#import "FBSDKCoreKit/FBSDKAccessToken.h"

@interface FBUtility : NSObject
+(NSDictionary *)getUserDataFromAccessToken:(FBSDKAccessToken *)token;

+(NSDictionary *) nsPars2Map:(NSString*) pars;

+(NSDictionary *) chatPars2Map:(const char *) pars;
@end
