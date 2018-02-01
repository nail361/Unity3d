using UnityEngine;

public class InternetChecker : MonoBehaviour
{
    public static bool isConnected;
    public static Ping ping;

    public static void CheckInternetConnect()
    {
        isConnected = false;
        if (ping != null) ping.DestroyPing();

        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            isConnected = true;
            ping = new Ping("8.8.8.8");
        }
        
    }
}