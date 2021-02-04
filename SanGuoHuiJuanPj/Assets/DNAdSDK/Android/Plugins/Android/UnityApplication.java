package com.donews.android;

import android.app.Application;

import com.donews.b.start.DoNewsAdManagerHolder;

/**
 * 创建日期： 2020/8/27
 * 创建时间： 10:51
 * author:yaoyaozhong
 **/
public class UnityApplication extends Application {

    @Override
    public void onCreate() {
        super.onCreate();
        DoNewsAdManagerHolder.init(this,false);//是否是测试环境 false代表正式环境 true代表测试环境，接入一律写false
    }
}
