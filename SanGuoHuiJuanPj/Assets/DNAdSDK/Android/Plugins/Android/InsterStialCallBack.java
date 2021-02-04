package com.donews.android;
/**
 * 创建日期： 2020/9/1
 * 创建时间： 17:26
 * author:yaoyaozhong
 **/
public interface InsterStialCallBack {

    void onAdError(String msg);

    void onAdShow();

    void onADExposure();

    void onAdClick();

    void onAdClose();
}
