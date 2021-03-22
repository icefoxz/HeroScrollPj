package com.donews.android;

/**
 * 创建日期： 2020/9/3
 * 创建时间： 17:46
 * author:yaoyaozhong
 **/
public interface SplashCallBack {
    void onNoAD(String s);
    void onShow();
    void onPresent();
    void onAdClick();
    void onADDismissed();
    void extendExtra(String s);
}
