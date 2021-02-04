package com.donews.android;

import android.Manifest;
import android.annotation.TargetApi;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import java.util.ArrayList;
import java.util.List;

public class MainActivity extends UnityPlayerActivity {

    private static boolean isExit = false;
    //动态权限可以不申请，但是建议申请，为了精准投放，提高ecmp,本demo申请了相应的权限。。，
    private String[] permissions = new String[]{
            Manifest.permission.READ_PHONE_STATE,//获取手机状态，为了精准投放
            Manifest.permission.ACCESS_COARSE_LOCATION,//获取位置，为了精准投放，提高ecmp
            Manifest.permission.WRITE_EXTERNAL_STORAGE,//获取手机存储权限 为了更好的展示广告
            Manifest.permission.READ_EXTERNAL_STORAGE,
    };
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        //动态申请权限
        if(Build.VERSION.SDK_INT > Build.VERSION_CODES.O){
            //权限可以不申请，强烈建议申请，申请会提高ecmp以及广告填充率，提高贵公司的收益
            checkAndRequestPermission();
        }
    }

    @TargetApi(Build.VERSION_CODES.M)
    private void checkAndRequestPermission() {
        final List<String> lackedPermission = new ArrayList<String>();
        //获取手机状态 为了精准投放，提高ecmp
        if (!(checkSelfPermission(Manifest.permission.READ_PHONE_STATE) == PackageManager.PERMISSION_GRANTED)) {
            lackedPermission.add(Manifest.permission.READ_PHONE_STATE);
        }
        //获取位置，为了精准投放，提高ecmp
        if (!(checkSelfPermission(Manifest.permission.ACCESS_FINE_LOCATION) == PackageManager.PERMISSION_GRANTED)) {
            lackedPermission.add(Manifest.permission.ACCESS_FINE_LOCATION);
        }
        // //获取手机存储权限 为了更好的优化
        if (!(checkSelfPermission(Manifest.permission.READ_EXTERNAL_STORAGE) == PackageManager.PERMISSION_GRANTED )){
            lackedPermission.add(Manifest.permission.READ_EXTERNAL_STORAGE);
        }
        if (!(checkSelfPermission( Manifest.permission.WRITE_EXTERNAL_STORAGE) == PackageManager.PERMISSION_GRANTED)) {
            lackedPermission.add(Manifest.permission.WRITE_EXTERNAL_STORAGE);
        }
        // 如果需要的权限都已经有了，那么直接调用SDK
        if (lackedPermission.size() == 0) {
        } else {
            new Handler().postDelayed(new Runnable() {
                @Override
                public void run() {
                    // 否则，建议请求所缺少的权限，在onRequestPermissionsResult中再看是否获得权限
                    String[] requestPermissions = new String[lackedPermission.size()];
                    lackedPermission.toArray(requestPermissions);
                    requestPermissions(requestPermissions, 1024);
                }
            },1000);
        }
    }

}
