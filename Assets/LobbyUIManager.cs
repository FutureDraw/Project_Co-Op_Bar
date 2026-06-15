using TMPro;
using UnityEngine;
using Unity.Netcode;
using XRMultiplayer;

public class LobbyUIManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject lobbyPanel;

    public TMP_Text player1Text;
    public TMP_Text player2Text;
    public TMP_Text statusText;

    public GameObject spinner;

    private void Start()
    {
        lobbyPanel.SetActive(false);

        if (XRINetworkGameManager.Instance != null)
        {
            XRINetworkGameManager.Instance.OnPlayerStateChanged += OnPlayerStateChanged;
        }
    }

    private void OnDestroy()
    {
        if (XRINetworkGameManager.Instance != null)
        {
            XRINetworkGameManager.Instance.OnPlayerStateChanged -= OnPlayerStateChanged;
        }
    }

    /// Вызывается кнопкой открытия лобби
    public void OpenLobby()
    {
        lobbyPanel.SetActive(true);

        UpdateUI();
    }

    private void OnPlayerStateChanged(ulong playerId, bool joined)
    {
        UpdateUI();
    }

    private void Update()
    {
        if (lobbyPanel.activeSelf)
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (NetworkManager.Singleton == null)
            return;

        int players = NetworkManager.Singleton.ConnectedClients.Count;

        switch (players)
        {
            case 0:
                player1Text.text = "Клиент 1 : Ожидание...";
                player2Text.text = "Клиент 2 : Ожидание...";
                statusText.text = "Подключение...";
                spinner.SetActive(true);
                break;

            case 1:
                player1Text.text = "Клиент 1 : Подключен";
                player2Text.text = "Клиент 2 : Ожидание...";
                statusText.text = "Ожидание второго клиента...";
                spinner.SetActive(true);
                break;

            default:
                player1Text.text = "Клиент 1 : Подключен";
                player2Text.text = "Клиент 2 : Подключен";
                statusText.text = "Оба клиента подключены";
                spinner.SetActive(false);
                break;
        }
    }
}