using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class MainMenuHud : MonoBehaviour {

    public InputField password;
    public InputField server_ip;
    public InputField port;

    public void CreateServer()
    {
        ConnectionHandler(CustomNetworkManager.instance.CreateServer(4, Int32.Parse(port.text), password.text));
    }

    public void ConnectClient()
    {
        ConnectionHandler(CustomNetworkManager.instance.ConnectToSever(server_ip.text, Int32.Parse(port.text), password.text));
    }

    private void ConnectionHandler(NetworkConnectionError answer)
    {
        switch (answer)
        {
            case NetworkConnectionError.NoError:
                SceneManager.LoadScene(1);
                break;
            default: Debug.Log(answer); break;
        }
    }

}
