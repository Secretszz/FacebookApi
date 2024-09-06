package com.bridge.facebook.callback;

import androidx.annotation.NonNull;

import com.bridge.common.listener.ILoginListener;
import com.facebook.FacebookCallback;
import com.facebook.FacebookException;
import com.facebook.login.LoginResult;

public class LoginCallBack implements FacebookCallback<LoginResult> {
    ILoginListener listener;
    public LoginCallBack(ILoginListener listener){
        this.listener = listener;
    }

    @Override
    public void onCancel() {
        this.listener.onCancel();
    }

    @Override
    public void onError(@NonNull FacebookException e) {
        this.listener.onError(-1, e.toString());
    }

    @Override
    public void onSuccess(LoginResult loginResult) {
        this.listener.onSuccess(loginResult.getAccessToken().toString());
    }
}
