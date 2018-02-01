using UnityEngine;

public class AndroidNativeUtils
{

#if UNITY_ANDROID

    private static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    private static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    private static AndroidJavaClass pluginClass = new AndroidJavaClass("com.utils.smileapps.androidnative.AndroidUtils");

    private static AndroidJavaObject toastExample = pluginClass.CallStatic<AndroidJavaObject>("instance");

    public static long WorkingTime()
    {
        return pluginClass.CallStatic<long>("GetWorkTime");
    }

    public static void ShowMsg(string msg)
    {
        toastExample.Call("setContext", currentActivity);
        currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
                    toastExample.Call("showMessage", msg);
        }));
    }

#endif
}