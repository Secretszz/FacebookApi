package com.bridge.facebook.callback;

import androidx.annotation.NonNull;

import com.bridge.common.listener.IShareListener;
import com.facebook.FacebookCallback;
import com.facebook.FacebookException;
import com.facebook.share.Sharer;

public class ShareCallback implements FacebookCallback<Sharer.Result> {
    IShareListener listener;
    public ShareCallback(IShareListener listener){
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
    public void onSuccess(Sharer.Result result) {
        this.listener.onSuccess();
    }
}
