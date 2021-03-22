package com.donews.android;

/**
 * 创建日期： 2020/9/1
 * 创建时间： 17:26
 * author:yaoyaozhong
 **/
public interface RewardVideoCallBack {
    void onAdError(String msg);

    void onAdShow();

    void onAdClick();

    void onAdClose();

    void onVideoComplete();

    void onRewardVerify(boolean rewardVerify);

    void onSkippedVideo();
}
