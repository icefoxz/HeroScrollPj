package com.donews.android;

import android.app.Activity;
import android.content.Context;
import android.os.Build;
import android.util.DisplayMetrics;
import android.util.Log;
import android.view.Display;
import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup;
import android.view.WindowManager;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;
import android.widget.Toast;

import com.donews.b.main.DoNewsAdNative;
import com.donews.b.main.info.DoNewsAD;
import com.donews.b.start.DoNewsAdManagerHolder;

import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.util.HashMap;

/**
 * 创建日期： 2020/8/28
 * 创建时间： 16:04
 * author:yaoyaozhong
 **/
public class DnUnitySDK {

    private static HashMap<String, DoNewsAdNative> hashMap = new HashMap<>();//缓存不同的广告位id

    private DnUnitySDK() {
    }

    public static DnUnitySDK getInstance() {
        //第一次调用getInstance方法时才加载SingletonHolder并初始化sInstance
        return DnUnitySDK.SingletonHolder.sInstance;
    }

    //静态内部类
    private static class SingletonHolder {
        private static final DnUnitySDK sInstance = new DnUnitySDK();
    }

    /**
     * 获取unity项目的上下文
     *
     * @return
     */
    public Activity getActivity() {
        Activity activity = null;
        try {
            Class<?> classtype = Class.forName("com.unity3d.player.UnityPlayer");
            activity = (Activity) classtype.getDeclaredField("currentActivity").get(classtype);
        } catch (Throwable e) {
            e.printStackTrace();
        }
        return activity;
    }

    private RelativeLayout splashLayout;
    private RelativeLayout.LayoutParams rootParam;

    public void showSplashAd(final String positionId, final SplashCallBack splashCallBack) {//显示广告
        final Activity activity = getActivity();
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (rootParam == null) {
                    Log.d("DnLogMsg", "rootParam is not null：");
                    rootParam = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT);
                }
                if (splashLayout == null) {
                    splashLayout = new RelativeLayout(activity);
                    splashLayout.setBackgroundColor(0x00000000);
                    splashLayout.setLayoutParams(rootParam);
                    splashLayout.addOnAttachStateChangeListener(new View.OnAttachStateChangeListener() {
                        @Override
                        public void onViewAttachedToWindow(View v) {
                            Log.d("DnLogMsg", "onViewAttachedToWindow：");
                            showSplashAD(splashLayout,positionId,activity,splashCallBack);
                        }

                        @Override
                        public void onViewDetachedFromWindow(View v) {
                            Log.d("DnLogMsg", "onViewDetachedFromWindow：");
                        }
                    });
                    activity.addContentView(splashLayout, rootParam);
                }else{
                    Log.d("DnLogMsg", "splashLayout is not null");
                    showSplashAD(splashLayout,positionId,activity,splashCallBack);
                }
            }
        });
    }

    private void showSplashAD(final RelativeLayout splashLayout, String positionId, Activity activity, final SplashCallBack splashCallBack) {
        DoNewsAD doNewsAD = new DoNewsAD.Builder()
                .setPositionid(positionId)//广告位id 5234
                .setView(splashLayout)
                .build();
        DoNewsAdNative doNewsAdNative = DoNewsAdManagerHolder.get().createDoNewsAdNative();
        doNewsAdNative.onCreateAdSplash(activity, doNewsAD, new DoNewsAdNative.SplashListener() {
            @Override
            public void onNoAD(String s) {//未获取到填充广告 s代表错误信息 可以通过sendMsg回调给游戏
                splashCallBack.onNoAD(s);
                splashLayout.removeAllViews();
            }

            @Override
            public void onClicked() {//点击广告
                splashCallBack.onAdClick();
            }

            @Override
            public void onShow() {//开始展示广告
                splashCallBack.onShow();
            }

            @Override
            public void extendExtra(String s) {//回调的透传字段，App可以暂时不用
                splashCallBack.extendExtra(s);
            }

            @Override
            public void onPresent() {//广告曝光
                splashCallBack.onPresent();
            }

            @Override
            public void onADDismissed() {//广告消失，跳转界面
                splashCallBack.onADDismissed();
                splashLayout.removeAllViews();
            }
        });

    }

    /**
     * 初始化方法
     *
     * @param isDebug
     */
    public void init(boolean isDebug) {
        final Activity unityActivity = getActivity();
        DoNewsAdManagerHolder.init(unityActivity, isDebug);//是否是测试环境 false代表正式环境 true代表测试环境，接入一律写false
    }

    private RelativeLayout bannerLayout;
    private RelativeLayout.LayoutParams bannerRootParam;
    private RelativeLayout.LayoutParams bannerLayoutParam;

    /**
     * banner广告
     *
     * @param positionId     广告位id
     * @param width          宽
     * @param height         高
     * @param marginL        距离屏幕左的距离
     * @param marginR        距离屏幕右的距离
     * @param marginT        距离屏幕上的距离
     * @param marginB        距离屏幕下的距离
     * @param bannerCallBack
     */
    public void requestBannerAd(final String positionId, final int width, final int height, final int marginL, final int marginR, final int marginT, final int marginB, final BannerCallBack bannerCallBack) {//显示广告
        final Activity unityActivity = getActivity();
        unityActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {

                int widthDP = 0;
                if (width <= 0) {
                    if (marginL > 0 || marginR > 0) {//判断居左或者居右是否都大于0
                        int screenWidth = getScreenWidthpx(getActivity());
                        int widthS = 0;
                        if (marginL > 0 && marginR > 0) {//如果都大于0
                            widthS = screenWidth - marginL - marginR;
                            widthDP = px2dip(unityActivity, (float) widthS);
                        } else {//其中一个大于0
                            if (marginR > 0) {
                                widthS = screenWidth - marginR;
                                widthDP = px2dip(unityActivity, (float) widthS);
                            }
                            if (marginL > 0) {
                                widthS = screenWidth - marginL;
                                widthDP = px2dip(unityActivity, (float) widthS);
                            }
                        }

                    } else {
                        bannerCallBack.onAdError("广告宽度不能小于屏幕宽度的百分之75");
                    }
                } else {
                    widthDP = px2dip(unityActivity, (float) width);
                }
                int heightDP = 0;

                if (height <= 0) {
                    bannerCallBack.onAdError("广告宽度不能小于0");
                    return;
                } else {
                    heightDP = px2dip(unityActivity, (float) height);//插屏的高度
                }
                if(bannerLayoutParam==null){
                    bannerLayoutParam = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT);
                }

                if(bannerLayout==null){
                    //添加banner布局 通过addContentView()添加到UnityPlayerActivity里面
                    bannerLayout = new RelativeLayout(unityActivity);
                    bannerLayout.setBackgroundColor(0x00000000);
                    bannerLayout.setGravity(Gravity.CENTER);
                    bannerLayout.setLayoutParams(bannerLayoutParam);
                    final int finalWidthDP = widthDP;
                    final int finalHeightDP = heightDP;
                    bannerLayout.addOnAttachStateChangeListener(new View.OnAttachStateChangeListener() {
                        @Override
                        public void onViewAttachedToWindow(View v) {
                            Log.d("DnLogMsg", "banner onViewAttachedToWindow：");
                            showBannerAD(unityActivity,bannerLayout,positionId, finalWidthDP, finalHeightDP,bannerCallBack);
                        }

                        @Override
                        public void onViewDetachedFromWindow(View v) {
                            Log.d("DnLogMsg", "banner onViewDetachedFromWindow：");
                        }
                    });
                    int screenHeight = (int) getHeight(getActivity());
                    Log.d("DnLogMsg", "屏幕高：" + screenHeight);
                    Log.d("DnLogMsg", "屏幕宽：" + getScreenWidthpx(getActivity()));
                    int marginLP = 0;
                    if (marginL < 0) {
                        marginLP = 0;
                    } else {
                        marginLP = marginL;
                    }

                    int marginRP = 0;
                    if (marginR < 0) {
                        marginRP = 0;
                    } else {
                        marginRP = marginR;
                    }
                    int marginTP = 0;
                    if (marginT < 0) {
                        marginTP = 0;
                        if (marginB > 0) {
                            marginTP = screenHeight - height - marginB;
                            Log.d("DnLogMsg", "banner 在屏幕底部:");
                        }
                    } else {
                        Log.d("DnLogMsg", "banner 在屏幕顶部:");
                        marginTP = marginT;
                    }
                    int marginBP = 0;
                    if (marginB < 0) {
                        marginBP = 0;
                    } else {
                        marginBP = marginB;
                    }
                    if(bannerRootParam ==null){
                        bannerRootParam = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT);
                    }
                    bannerRootParam.setMargins(marginLP, marginTP, marginRP, marginBP);
                    unityActivity.addContentView(bannerLayout, bannerRootParam);
                }else{
                    Log.d("DnLogMsg", "bannerLayout is not null：");
                    showBannerAD(unityActivity,bannerLayout,positionId,widthDP,heightDP,bannerCallBack);
                }
            }
        });

    }

    private void showBannerAD(Activity activity,final RelativeLayout bannerLayout, String positionId, int widthDP, int heightDP, final BannerCallBack bannerCallBack) {
        DoNewsAD doNewsAD = new DoNewsAD.Builder()
                .setPositionid(positionId)//广告位ID
                .setExpressViewWidth(widthDP)//插屏宽度 dp
                .setExpressViewHeight(heightDP)//高度 dp
                .setView(bannerLayout)
                .build();
        DoNewsAdNative doNewsAdNative = DoNewsAdManagerHolder.get().createDoNewsAdNative();
        doNewsAdNative.onCreateBanner(activity, doNewsAD, new DoNewsAdNative.DoNewsBannerADListener() {

            @Override
            public void onAdError(String s) {
                bannerLayout.removeAllViews();
                bannerCallBack.onAdError(s);
            }

            @Override
            public void showAd() {
                bannerCallBack.onAdShow();
            }

            @Override
            public void onRenderSuccess(View view) {

            }

            @Override
            public void onADExposure() {
                bannerCallBack.onAdExposure();
            }

            @Override
            public void onADClosed() {
                bannerLayout.removeAllViews();
                bannerCallBack.onAdClose();
            }

            @Override
            public void onADClicked() {
                bannerCallBack.onAdClick();
            }
        });
    }


    /**
     * 请求激励视频的方法
     * 这个方法名已经被我改了，因为原名[requestRewardVideo]获取不到。
     * @param positionId
     * @param rewardVideoCallBack
     * @return
     */
    public void requestDirectAd(final String positionId, final RewardVideoCallBack rewardVideoCallBack) {
        final Activity unityActivity = getActivity();
        unityActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                DoNewsAD doNewsAD;//多牛Bean
                DoNewsAdNative doNewsAdNative;
                doNewsAD = new DoNewsAD.Builder()
                        .setPositionid(positionId)//广告位ID
                        .build();
                doNewsAdNative = DoNewsAdManagerHolder.get().createDoNewsAdNative();
                doNewsAdNative.onCreateRewardAd(unityActivity, doNewsAD, new DoNewsAdNative.RewardVideoAdListener() {
                    @Override
                    public void onError(int code, String msg) {//请求激励视频出错
                        rewardVideoCallBack.onAdError(msg);
                    }

                    @Override
                    public void onAdShow() {//视频显示
                        rewardVideoCallBack.onAdShow();
                    }

                    @Override
                    public void onAdVideoBarClick() {//点击激励视频
                        rewardVideoCallBack.onAdClick();
                    }

                    @Override
                    public void onAdClose() {//视频关闭
                        rewardVideoCallBack.onAdClose();
                    }

                    @Override
                    public void onVideoComplete() {//视频播放完成
                        rewardVideoCallBack.onVideoComplete();
                    }

                    //视频播放完成后，奖励验证回调，rewardVerify：是否有效，发奖励比例要以此为接口为准。
                    @Override
                    public void onRewardVerify(boolean rewardVerify) {//获取奖励回调
                        if (rewardVerify) {
                            rewardVideoCallBack.onRewardVerify(true);
                        } else {
                            rewardVideoCallBack.onRewardVerify(false);
                        }
                    }

                    @Override
                    public void onSkippedVideo() {//跳过回调
                        rewardVideoCallBack.onSkippedVideo();
                    }
                });
            }
        });

    }

    /**
     * 请求激励视频的方法
     *
     * @param positionId
     * @param rewardVideoPloadCallBack
     * @return
     */
    public void requestRVPload(final String positionId, final RewardVideoPloadCallBack rewardVideoPloadCallBack) {
        final Activity unityActivity = getActivity();
        unityActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                DoNewsAD doNewsAD;//多牛Bean
                DoNewsAdNative doNewsAdNative;
                doNewsAD = new DoNewsAD.Builder()
                        .setPositionid(positionId)//广告位ID
                        .build();
                doNewsAdNative = DoNewsAdManagerHolder.get().createDoNewsAdNative();
                doNewsAdNative.preLoadRewardAd(unityActivity, doNewsAD, new DoNewsAdNative.RewardVideoAdCacheListener() {
                    @Override
                    public void onError(int code, String msg) {//请求激励视频出错
                        rewardVideoPloadCallBack.onAdError(msg);
                    }

                    @Override
                    public void onADLoad() {
                        rewardVideoPloadCallBack.onADLoad();
                    }

                    @Override
                    public void onVideoCached() {
                        rewardVideoPloadCallBack.onVideoCached();
                    }

                    @Override
                    public void onAdShow() {//视频显示
                        rewardVideoPloadCallBack.onAdShow();
                    }

                    @Override
                    public void onAdVideoBarClick() {//点击激励视频
                        rewardVideoPloadCallBack.onAdClick();
                    }

                    @Override
                    public void onAdClose() {//视频关闭
                        rewardVideoPloadCallBack.onAdClose();
                    }

                    @Override
                    public void onVideoComplete() {//视频播放完成
                        rewardVideoPloadCallBack.onVideoComplete();
                    }

                    //视频播放完成后，奖励验证回调，rewardVerify：是否有效，发奖励比例要以此为接口为准。
                    @Override
                    public void onRewardVerify(boolean rewardVerify) {//获取奖励回调
                        if (rewardVerify) {
                            rewardVideoPloadCallBack.onRewardVerify(true);
                        } else {
                            rewardVideoPloadCallBack.onRewardVerify(false);
                        }
                    }
                });
                hashMap.put(positionId, doNewsAdNative);
            }
        });
    }

    /**
     * 预加载激励视频显示方法
     *
     * @param positionId
     */
    public void requestRewardVideo(final String positionId) {
        final Activity unityActivity = getActivity();
        unityActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (hashMap.size() > 0) {
                    if (hashMap.get(positionId) != null) {
                        if (hashMap.get(positionId).isLoadReady()) {
                            hashMap.get(positionId).showRewardAd();//播放
                            hashMap.remove(positionId);
                        }
                    }
                } else {
                    Toast.makeText(unityActivity, "暂时没有预加载的激励视频，请稍后再试！", Toast.LENGTH_SHORT).show();
                }
            }
        });

    }

    /**
     * 请求插屏的方法
     *
     * @param positionId
     * @param width
     * @param height
     * @param insterStialCallBack
     * @return
     */
    public void requestInstertialAd(final String positionId, final int width, final int height, final InsterStialCallBack insterStialCallBack) {
        Log.d("requestInstertialAd", "insterstial width:" + width);
        final Activity unityActivity = getActivity();
        unityActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                //模块高（屏幕宽度-左右种间距）  容器的布局文件建议为宽高都为wrap_content
                int widthDP = 0;
                if (width == 0) {
                    widthDP = getScreenWidthDp(unityActivity);
                    if (widthDP == 0) {
                        widthDP = 360;
                    }
                } else {
                    widthDP = px2dip(getActivity(), (float) width);
                }
                int heightDP = px2dip(unityActivity, (float) height);//插屏的高度，强烈建议设置0,，0是自适应，头条广点通的广告都建议传0，自适应
                DoNewsAD doNewsAD = new DoNewsAD.Builder()
                        .setPositionid(positionId)//广告位ID
                        .setExpressViewWidth(widthDP)//插屏宽度 dp
                        .setExpressViewHeight(heightDP)//高度 dp
                        .build();
                DoNewsAdNative doNewsAdNative = DoNewsAdManagerHolder.get().createDoNewsAdNative();
                doNewsAdNative.onCreateInterstitial(getActivity(), doNewsAD, new DoNewsAdNative.DonewsInterstitialADListener() {
                    @Override
                    public void onAdError(final String s) {//没有获取到广告
                        insterStialCallBack.onAdError(s);
                    }

                    @Override
                    public void showAd() {//显示广告
                        insterStialCallBack.onAdShow();
                    }

                    @Override
                    public void onADExposure() {//广告曝光
                        insterStialCallBack.onADExposure();
                    }

                    @Override
                    public void onADClosed() {//广告关闭
                        insterStialCallBack.onAdClose();
                    }

                    @Override
                    public void onADClicked() {//广告点击
                        insterStialCallBack.onAdClick();
                    }

                });
            }
        });

    }

    /**
     * 根据手机的分辨率从 dp 的单位 转成为 px(像素)
     */
    public static int dip2px(Context context, float dpValue) {
        final float scale = context.getResources().getDisplayMetrics().density;
        return (int) (dpValue * scale + 0.5f);
    }

    /**
     * 根据手机的分辨率从 px(像素) 的单位 转成为 dp
     */
    public static int px2dip(Context context, float pxValue) {
        final float scale = context.getResources().getDisplayMetrics().density;
        return (int) (pxValue / scale + 0.5f);
    }

    public int getScreenWidthDp(Context context) {
        final float scale = context.getResources().getDisplayMetrics().density;
        float width = context.getResources().getDisplayMetrics().widthPixels;
        return (int) (width / (scale <= 0 ? 1 : scale) + 0.5f);
    }

    public int getScreenWidthpx(Context context) {
        float width = context.getResources().getDisplayMetrics().widthPixels;
        return (int) width;
    }

    //全面屏、刘海屏适配
    public float getHeight(Activity activity) {
        hideBottomUIMenu(activity);
        float height;
        int realHeight = getRealHeight(activity);
        if (hasNotchScreen(activity)) {
            height = realHeight - getStatusBarHeight(activity);
        } else {
            height = realHeight;
        }
        return height;
    }

    /**
     * 获取状态栏高度
     *
     * @param context
     * @return
     */
    public static int getStatusBarHeight(Context context) {
        int result = 0;
        int resourceId = context.getResources().getIdentifier("status_bar_height", "dimen", "android");
        if (resourceId > 0) {
            result = context.getResources().getDimensionPixelSize(resourceId);
        }
        return result;
    }

    public void hideBottomUIMenu(Activity activity) {
        if (activity == null) {
            return;
        }
        try {
            //隐藏虚拟按键，并且全屏
            if (Build.VERSION.SDK_INT > 11 && Build.VERSION.SDK_INT < 19) { // lower api
                View v = activity.getWindow().getDecorView();
                v.setSystemUiVisibility(View.GONE);
            } else if (Build.VERSION.SDK_INT >= 19) {
                //for new api versions.
                View decorView = activity.getWindow().getDecorView();
                int uiOptions = View.SYSTEM_UI_FLAG_LAYOUT_STABLE
                        | View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                        | View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
                        | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION // hide nav bar
                        //                    | View.SYSTEM_UI_FLAG_FULLSCREEN // hide status bar
                        | View.SYSTEM_UI_FLAG_IMMERSIVE;
                decorView.setSystemUiVisibility(uiOptions);
                activity.getWindow().addFlags(WindowManager.LayoutParams.FLAG_TRANSLUCENT_NAVIGATION);
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    //获取屏幕真实高度，不包含下方虚拟导航栏
    public int getRealHeight(Context context) {
        WindowManager windowManager = (WindowManager) context.getSystemService(Context.WINDOW_SERVICE);
        Display display = windowManager.getDefaultDisplay();
        DisplayMetrics dm = new DisplayMetrics();
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1) {
            display.getRealMetrics(dm);
        } else {
            display.getMetrics(dm);
        }
        int realHeight = dm.heightPixels;
        return realHeight;
    }

    /**
     * 判断是否是刘海屏
     *
     * @return
     */
    public boolean hasNotchScreen(Activity activity) {
        if (getInt("ro.miui.notch", activity) == 1 || hasNotchAtHuawei(activity) || hasNotchAtOPPO(activity)
                || hasNotchAtVivo(activity) || isAndroidPHasNotch(activity)) { //TODO 各种品牌
            return true;
        }

        return false;
    }

    /**
     * Android P 刘海屏判断
     *
     * @param activity
     * @return
     */
    public static boolean isAndroidPHasNotch(Activity activity) {
        boolean ret = false;
        if (Build.VERSION.SDK_INT >= 28) {
            try {
                Class windowInsets = Class.forName("android.view.WindowInsets");
                Method method = windowInsets.getMethod("getDisplayCutout");
                Object displayCutout = method.invoke(windowInsets);
                if (displayCutout != null) {
                    ret = true;
                }
            } catch (Exception e) {
                e.printStackTrace();
            }
        }
        return ret;
    }

    /**
     * 小米刘海屏判断.
     *
     * @return 0 if it is not notch ; return 1 means notch
     * @throws IllegalArgumentException if the key exceeds 32 characters
     */
    public static int getInt(String key, Activity activity) {
        int result = 0;
        if (isMiui()) {
            try {
                ClassLoader classLoader = activity.getClassLoader();
                @SuppressWarnings("rawtypes")
                Class SystemProperties = classLoader.loadClass("android.os.SystemProperties");
                //参数类型
                @SuppressWarnings("rawtypes")
                Class[] paramTypes = new Class[2];
                paramTypes[0] = String.class;
                paramTypes[1] = int.class;
                Method getInt = SystemProperties.getMethod("getInt", paramTypes);
                //参数
                Object[] params = new Object[2];
                params[0] = new String(key);
                params[1] = new Integer(0);
                result = (Integer) getInt.invoke(SystemProperties, params);

            } catch (ClassNotFoundException e) {
                e.printStackTrace();
            } catch (NoSuchMethodException e) {
                e.printStackTrace();
            } catch (IllegalAccessException e) {
                e.printStackTrace();
            } catch (IllegalArgumentException e) {
                e.printStackTrace();
            } catch (InvocationTargetException e) {
                e.printStackTrace();
            }
        }
        return result;
    }

    public static boolean isMiui() {
        boolean sIsMiui = false;
        try {
            Class<?> clz = Class.forName("miui.os.Build");
            if (clz != null) {
                sIsMiui = true;
                //noinspection ConstantConditions
                return sIsMiui;
            }
        } catch (Exception e) {
            // ignore
        }
        return sIsMiui;
    }

    /**
     * 华为刘海屏判断
     *
     * @return
     */
    public static boolean hasNotchAtHuawei(Context context) {
        boolean ret = false;
        try {
            ClassLoader classLoader = context.getClassLoader();
            Class HwNotchSizeUtil = classLoader.loadClass("com.huawei.android.util.HwNotchSizeUtil");
            Method get = HwNotchSizeUtil.getMethod("hasNotchInScreen");
            ret = (boolean) get.invoke(HwNotchSizeUtil);
        } catch (ClassNotFoundException e) {
        } catch (NoSuchMethodException e) {
        } catch (Exception e) {
        } finally {
            return ret;
        }
    }

    /**
     * OPPO刘海屏判断
     *
     * @return
     */
    public static boolean hasNotchAtOPPO(Context context) {
        return context.getPackageManager().hasSystemFeature("com.oppo.feature.screen.heteromorphism");
    }

    public static final int VIVO_NOTCH = 0x00000020;//是否有刘海

    /**
     * VIVO刘海屏判断
     *
     * @return
     */
    public static boolean hasNotchAtVivo(Context context) {
        boolean ret = false;
        try {
            ClassLoader classLoader = context.getClassLoader();
            Class FtFeature = classLoader.loadClass("android.util.FtFeature");
            Method method = FtFeature.getMethod("isFeatureSupport", int.class);
            ret = (boolean) method.invoke(FtFeature, VIVO_NOTCH);
        } catch (ClassNotFoundException e) {
        } catch (NoSuchMethodException e) {
        } catch (Exception e) {
        } finally {
            return ret;
        }
    }

}


