package com.bridge.facebook.listener;

public interface IBridgeListener {

    /**
     * 成功桥接回调
     */
    void onSuccess();

    /**
     * 取消桥接回调
     */
    void onCancel();

    /**
     * 错误桥接回调
     * @param errCode 错误代码
     * @param errMsg 错误信息
     */
    void onError(int errCode, String errMsg);
}
