package com.bridge.facebook;

import android.app.Activity;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.Bundle;
import android.util.Log;

import com.bridge.facebook.callback.LoginCallBack;
import com.bridge.facebook.callback.ShareCallback;
import com.bridge.facebook.listener.IBridgeListener;
import com.facebook.AccessToken;
import com.facebook.CallbackManager;
import com.facebook.FacebookSdk;
import com.facebook.LoginStatusCallback;
import com.facebook.appevents.AppEventsLogger;
import com.facebook.login.LoginManager;
import com.facebook.share.model.ShareLinkContent;
import com.facebook.share.model.ShareMediaContent;
import com.facebook.share.model.SharePhoto;
import com.facebook.share.model.SharePhotoContent;
import com.facebook.share.model.ShareVideo;
import com.facebook.share.model.ShareVideoContent;
import com.facebook.share.widget.ShareDialog;

import org.jetbrains.annotations.NotNull;
import org.json.JSONObject;

import java.math.BigDecimal;
import java.util.Arrays;
import java.util.Currency;
import java.util.Iterator;
import java.util.List;

public class FacebookSdkManager {
    private interface Permissions{
        String EMAIL = "email";
        String PUBLIC_PROFILE = "public_profile";
        String GAMING_PROFILE = "gaming_profile";
        String GAMING_USER_PICTURE = "gaming_user_picture";
    }

    private static class Holder{
        public static FacebookSdkManager INSTANCE = new FacebookSdkManager();
    }

    private final static String TAG = FacebookSdkManager.class.getName();
    private CallbackManager callbackManager;
    private List<String> permissions;
    private static AppEventsLogger appEventsLogger;
    private final static String packageName = "com.facebook.katana";

    /**
     * 获取单例
     * @return
     */
    public static FacebookSdkManager getInstance(){
        return Holder.INSTANCE;
    }

    /**
     * 初始化sdk
     * @param activity
     * @param listener
     */
    public void initSDK(Activity activity, IBridgeListener listener){
        permissions = Arrays.asList(Permissions.PUBLIC_PROFILE, Permissions.GAMING_PROFILE, Permissions.EMAIL, Permissions.GAMING_USER_PICTURE);
        appEventsLogger = AppEventsLogger.newLogger(activity);
        if (isInitialized()){
            listener.onSuccess();
        } else {
            listener.onError(-1, "");
        }
    }

    /**
     * 从facebook客户端回来的回调事件
     * @param requestCode
     * @param resultCode
     * @param data
     */
    public void onActivityResult(final int requestCode, final int resultCode, final Intent data) {
        if (callbackManager != null){
            callbackManager.onActivityResult(requestCode, resultCode, data);
        }
    }

    /**
     * 是否安装了facebook
     * @param activity
     * @return
     */
    public boolean isInstalled(Activity activity){
        try {
            activity.getPackageManager().getApplicationInfo(packageName, 0);
            return true;
        } catch (Exception var2) {
            return false;
        }
    }

    /**
     * 是否已经初始化
     * @return 是否已经初始化
     */
    public boolean isInitialized(){
        return FacebookSdk.isInitialized();
    }

    /**
     * facebook登陆
     * @param activity
     * @param callBack
     */
    public void login(Activity activity, IBridgeListener callBack){
        AccessToken accessToken = AccessToken.getCurrentAccessToken();
        if (accessToken != null && !accessToken.isExpired()){
            // 已登陆
            callBack.onSuccess();
            return;
        }
        logInWithReadPermissions(activity, callBack);
    }

    /**
     * facebook快捷登陆
     * @param activity
     * @param callBack
     */
    public void retrieveLoginStatus(Activity activity, IBridgeListener callBack){
        AccessToken accessToken = AccessToken.getCurrentAccessToken();
        if (accessToken != null && !accessToken.isExpired()){
            // 已登陆
            callBack.onSuccess();
            return;
        }
        LoginManager.getInstance().retrieveLoginStatus(activity, new LoginStatusCallback() {
            @Override
            public void onCompleted(@NotNull AccessToken accessToken) {
                callBack.onSuccess();
            }

            @Override
            public void onFailure() {
                logInWithReadPermissions(activity, callBack);
            }

            @Override
            public void onError(@NotNull Exception e) {
                callBack.onError(-1, e.getMessage());
            }
        });
    }

    /**
     * 登陆伴随读取权限
     * @param activity
     * @param callBack
     */
    private void logInWithReadPermissions(Activity activity, IBridgeListener callBack){
        callbackManager = CallbackManager.Factory.create();
        LoginManager.getInstance().registerCallback(callbackManager, new LoginCallBack(callBack));
        LoginManager.getInstance().logInWithReadPermissions(activity, permissions);
    }

    /**
     * 分享链接
     * @param activity
     * @param linkUrl 链接地址
     * @param callback 分享回调
     */
    public void shareLink(Activity activity, String linkUrl, IBridgeListener callback){
        if (ShareDialog.canShow(ShareLinkContent.class)){
            ShareLinkContent content = new ShareLinkContent.Builder()
                    .setContentUrl(Uri.parse(linkUrl))
                    .build();
            ShareDialog dialog = new ShareDialog(activity);
            callbackManager = CallbackManager.Factory.create();
            dialog.registerCallback(callbackManager, new ShareCallback(callback));
            dialog.show(content);
        } else {
            callback.onError(-1, "not support");
        }
    }

    /**
     * 分享图片
     * @param activity
     * @param imagePath 图片本地地址
     * @param callback
     */
    public void shareImage(Activity activity, String imagePath, IBridgeListener callback){
        if (ShareDialog.canShow(SharePhotoContent.class)){
            SharePhoto photo = new SharePhoto.Builder().setImageUrl(Uri.parse(imagePath)).build();
            SharePhotoContent content = new SharePhotoContent.Builder().addPhoto(photo).build();
            ShareDialog dialog = new ShareDialog(activity);
            callbackManager = CallbackManager.Factory.create();
            dialog.registerCallback(callbackManager, new ShareCallback(callback));
            dialog.show(content);
        } else {
            callback.onError(-1, "not support");
        }
    }

    /**
     * 分享图片
     * @param activity
     * @param data 图片数据
     * @param callback
     */
    public void shareImage(Activity activity, byte[] data, IBridgeListener callback){
        if (ShareDialog.canShow(SharePhotoContent.class)){
            Bitmap bmp = BitmapFactory.decodeByteArray(data, 0 , data.length);
            SharePhoto photo = new SharePhoto.Builder().setBitmap(bmp).build();
            SharePhotoContent content = new SharePhotoContent.Builder().addPhoto(photo).build();
            ShareDialog dialog = new ShareDialog(activity);
            callbackManager = CallbackManager.Factory.create();
            dialog.registerCallback(callbackManager, new ShareCallback(callback));
            dialog.show(content);
        } else {
            callback.onError(-1, "not support");
        }
    }

    /**
     * 分享视频
     * @param activity
     * @param videoUrl 视频本地地址
     * @param callback
     */
    public void shareVideo(Activity activity, String videoUrl, IBridgeListener callback) {
        if (ShareDialog.canShow(ShareVideoContent.class)){
            ShareVideo shareVideo = new ShareVideo.Builder().setLocalUrl(Uri.parse(videoUrl)).build();
            ShareVideoContent content = new ShareVideoContent.Builder().setVideo(shareVideo).build();
            ShareDialog dialog = new ShareDialog(activity);
            callbackManager = CallbackManager.Factory.create();
            dialog.registerCallback(callbackManager, new ShareCallback(callback));
            dialog.show(content);
        } else {
            callback.onError(-1, "not support");
        }
    }

    /**
     * 多媒体分享
     * @param activity
     * @param videoList
     * @param imageList
     * @param imageDataList
     * @param callback
     */
    public void shareMedia(Activity activity, String[] videoList, String[] imageList, byte[][] imageDataList, final IBridgeListener callback) {
        if (videoList.length + imageList.length + imageDataList.length > 6){
            callback.onError(-1, "There are more than 6 files");
            return;
        }
        if (ShareDialog.canShow(ShareMediaContent.class)){
            ShareMediaContent.Builder build = new ShareMediaContent.Builder();
            ShareVideo video;
            for (String url : videoList){
                video = new ShareVideo.Builder().setLocalUrl(Uri.parse(url)).build();
                build.addMedium(video);
            }
            SharePhoto photo;
            for (String url : imageList){
                photo = new SharePhoto.Builder().setImageUrl(Uri.parse(url)).build();
                build.addMedium(photo);
            }
            for (byte[] data : imageDataList){
                photo = new SharePhoto.Builder().setBitmap(BitmapFactory.decodeByteArray(data, 0 , data.length)).build();
                build.addMedium(photo);
            }
            ShareDialog dialog = new ShareDialog(activity);
            callbackManager = CallbackManager.Factory.create();
            dialog.registerCallback(callbackManager, new ShareCallback(callback));
            dialog.show(build.build());
        } else {
            callback.onError(-1, "not support");
        }
    }

    /**
     * 记录支付
     * @param price
     * @param currency
     * @param pars
     */
    public void logPurchase(double price, String currency, String pars){
        Bundle parameters = json2Bundle(pars);
        if (parameters != null){
            appEventsLogger.logPurchase(new BigDecimal(price), Currency.getInstance(currency), parameters);
        }
    }

    /**
     * 自定义日志
     * @param logEventName
     * @param valueToSum
     * @param pars
     */
    public void logEvent(String logEventName, double valueToSum, String pars){
        Bundle parameters = json2Bundle(pars);
        if (parameters != null){
            appEventsLogger.logEvent(logEventName, valueToSum, parameters);
        }
    }

    /**
     * 自定义日志
     * @param logEventName
     * @param pars
     */
    public void logEvent(String logEventName, String pars){
        Bundle parameters = json2Bundle(pars);
        if (parameters != null){
            appEventsLogger.logEvent(logEventName, parameters);
        }
    }

    /**
     * 将json字符串转成Bundle
     * @param json 记录事件的参数
     * @return 参数Bundle
     */
    private Bundle json2Bundle(String json){
        try {
            JSONObject jsonObject = new JSONObject(json);
            Iterator<String> keys = jsonObject.keys();
            Bundle parameters = new Bundle();
            while (keys.hasNext()){
                String key = keys.next();
                String value = (String) jsonObject.get(key);
                if (!value.isEmpty()){
                    parameters.putString(key, value);
                }
            }
            return parameters;
        } catch (Exception ex) {
            Log.e(TAG, "getParameters: ", ex);
            return null;
        }
    }

}
