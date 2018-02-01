using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsInitializer : MonoBehaviour
{
    [SerializeField]
    private string
        androidGameId = "1039992",
        iosGameId = "1039993";

    [SerializeField]
    private bool testMode;

    void Start()
    {
        string gameId = null;

#if UNITY_ANDROID
        gameId = androidGameId;
#elif UNITY_IOS
        gameId = iosGameId;
#endif

        if (!Advertisement.isSupported)
        {
            Debug.LogWarning("Unable to initialize Unity Ads. Platform not supported.");
        }
        else if (Advertisement.isInitialized)
        {
            Debug.Log("Unity Ads is already initialized.");
        }
        else
        {
            Debug.Log("Initialize Unity Ads");
            Advertisement.Initialize(gameId, true);
        }
    }
}