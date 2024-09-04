//
//  FBUtility.m
//  UnityFramework
//
//  Created by 晴天网络 on 2023/3/6.
//

#import <Foundation/Foundation.h>
#import "FBSDKLoginKit/FBSDKLoginKit.h"
#import "FBUtility.h"
#import "FBSDKShareKit/FBSDKShareKit.h"
#import "FBSDKCoreKit/FBSDKAccessToken.h"

@implementation FBUtility

+ (NSDictionary *)getUserDataFromAccessToken:(FBSDKAccessToken *)token
{
  if (token) {
    if (token.tokenString &&
        token.expirationDate &&
        token.userID &&
        token.permissions &&
        token.declinedPermissions) {
      NSInteger expiration = token.expirationDate.timeIntervalSince1970;
      NSInteger lastRefreshDate = token.refreshDate ? token.refreshDate.timeIntervalSince1970 : 0;
      return @{
               @"opened" : @"true",
               @"access_token" : token.tokenString,
               @"expiration_timestamp" : [@(expiration) stringValue],
               @"user_id" : token.userID,
               @"permissions" : [token.permissions allObjects],
               @"granted_permissions" : [token.permissions allObjects],
               @"declined_permissions" : [token.declinedPermissions allObjects],
               @"last_refresh" : [@(lastRefreshDate) stringValue],
               @"graph_domain" : @"facebook",
               };
    }
  }

  return nil;
}

+(NSDictionary *) nsPars2Map:(NSString*) pars{
    NSDictionary * parsMap = nil;
    if (pars) {
        NSData *jsonData = [pars dataUsingEncoding:NSUTF8StringEncoding];
        NSError *error = nil;
        parsMap = [NSJSONSerialization JSONObjectWithData:jsonData options:NSJSONReadingMutableContainers error:&error];
        return parsMap;
    }
    return nil;
}

+(NSDictionary *) chatPars2Map:(const char *) pars{
    if (pars) {
        return [self nsPars2Map:[NSString stringWithUTF8String:pars]];
    }
    return nil;
}

@end
