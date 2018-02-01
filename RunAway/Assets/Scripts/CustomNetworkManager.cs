using UnityEngine;

[RequireComponent(typeof(NetworkView))]
public class CustomNetworkManager : MonoBehaviour {

    public static CustomNetworkManager instance;
    private int playerCount = 0;
    public int PlayersCount { get { return playerCount; } }

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public NetworkConnectionError CreateServer(int con_count, int port, string pass = "")
    {
        Network.incomingPassword = pass;
        bool useNat = !Network.HavePublicAddress();
        return Network.InitializeServer(con_count, port, useNat);
    }

	public NetworkConnectionError ConnectToSever(string ip, int port, string pass = "")
    {
        return Network.Connect(ip, port, pass);
    }

    private void OnPlayerConnected(NetworkPlayer player)
    {
        playerCount++;
    }

    private void OnPlayerDisconnected(NetworkPlayer player)
    {
        playerCount--; // уменьшаем количество игроков
        Network.RemoveRPCs(player); // очищаем список процедур игрока
        Network.DestroyPlayerObjects(player); // уничтожаем все объекты игрока
    }
}
