using UnityEngine;
using Unity.Netcode;

public class ConnectionUI : MonoBehaviour
{
    public GameObject loadingScreen;

    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnected;
    }

    public void Connect()
    {
        loadingScreen.SetActive(true);
        NetworkManager.Singleton.StartClient();
    }

    void OnConnected(ulong id)
    {
        loadingScreen.SetActive(false);
    }

    void OnDisconnected(ulong id)
    {
        loadingScreen.SetActive(false);
    }
}