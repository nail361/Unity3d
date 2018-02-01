using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(NetworkView))]

public class GameManager : MonoBehaviour {

    public Text status_label;
    public Transform[] spawns;
    public GameObject playerPrefab;

	void Start () {
        if (Network.isServer)
        {
            status_label.text = "ServerCreated";
        }
        else if (Network.isClient)
        {
            status_label.text = "ClientConnected";
        }

        SpawnPlayer();
    }
    
    private void SpawnPlayer()
    {
        GameObject newPlayer = Network.Instantiate(playerPrefab, spawns[Random.Range(0,spawns.Length)].position, Quaternion.identity, 0) as GameObject;
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        Network.DestroyPlayerObjects(Network.player); // удаляемся из игры
    }

}
