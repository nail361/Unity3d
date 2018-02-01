using UnityEngine;

#if UNITY_ANDROID
using GooglePlayGames;
#elif UNITY_IOS

#endif

public static class SocialsPlatform {

    public static void Init () {
#if UNITY_ANDROID

        if (!Social.localUser.authenticated)
        {
            ConnectToGoogleService();
        }


#elif UNITY_IOS

#endif
    }

    private static void ConnectToGoogleService()
    {
        PlayGamesPlatform.Activate();
        // authenticate user:
        Social.localUser.Authenticate((bool success) => {});
    }

    public static void ReportScore(long score)
    {
#if UNITY_ANDROID
        Social.ReportScore(score, GameServicesClass.LEAD_LEADERBOARD, (bool success) => {});
#elif UNITY_IOS

#endif
    }

    public static void AddAchievement(string name)
    {
#if UNITY_ANDROID
        Social.ReportProgress(name, 100.0f, (bool success) => {});
#elif UNITY_IOS

#endif
    }

    public static void IncreaseAchievements(string name, int steps)
    {
#if UNITY_ANDROID
        PlayGamesPlatform.Instance.IncrementAchievement(name, steps, (bool success) => {});
#elif UNITY_IOS

#endif
    }

    public static void ShowLeaderboard()
    {
#if UNITY_ANDROID

        if (Social.localUser.authenticated)
            PlayGamesPlatform.Instance.ShowLeaderboardUI(GameServicesClass.LEAD_LEADERBOARD);
        else
        {
            AndroidNativeUtils.ShowMsg(LanguageManager.GetText("NoWeb"));
        }        

#elif UNITY_IOS

#endif
    }

    public static void ShowAchievements()
    {
#if UNITY_ANDROID
        if (Social.localUser.authenticated)
            Social.Active.ShowAchievementsUI();
        else
        {
            AndroidNativeUtils.ShowMsg(LanguageManager.GetText("NoWeb"));
        }
#elif UNITY_IOS

#endif
    }

}