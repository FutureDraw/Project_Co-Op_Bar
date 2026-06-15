using TMPro;
using UnityEngine;
using Unity.Netcode;
using XRMultiplayer;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject lobbyPanel;

    public TMP_Text player1Text;
    public TMP_Text player2Text;
    public TMP_Text statusText;

    public GameObject smallSpinner;
    public GameObject bigSpinner;

    public GameObject client1Panel;
    public GameObject client2Panel;

    [Header("Start Button")]
    public Button startGameButton;
    public Image startGameButtonImage;

    private bool lobbyClosedByUser = false;

    private void Start()
    {
        lobbyPanel.SetActive(false);
        bigSpinner.SetActive(false);
        smallSpinner.SetActive(false);

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientChanged;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientChanged;
        }

        if (XRINetworkGameManager.Instance != null)
        {
            XRINetworkGameManager.Instance.OnPlayerStateChanged += OnPlayerStateChanged;
        }

        UpdateStartButtonState(0);
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientChanged;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientChanged;
        }

        if (XRINetworkGameManager.Instance != null)
        {
            XRINetworkGameManager.Instance.OnPlayerStateChanged -= OnPlayerStateChanged;
        }
    }

    // =========================
    // BUTTONS
    // =========================

    public void OpenLobby()
    {
        lobbyClosedByUser = false;

        lobbyPanel.SetActive(true);
        bigSpinner.SetActive(false);

        UpdateUI();
    }

    public void ExitLobby()
    {
        lobbyClosedByUser = true;

        lobbyPanel.SetActive(false);
    }

    public void StartGame()
    {
        if (NetworkManager.Singleton == null)
            return;

        int players = NetworkManager.Singleton.ConnectedClientsList.Count;

        if (players < 2)
            return;

        Debug.Log("Start Game!");
    }

    // =========================
    // EVENTS
    // =========================

    private void OnClientChanged(ulong clientId)
    {
        UpdateUI();
    }

    private void OnPlayerStateChanged(ulong playerId, bool joined)
    {
        UpdateUI();
    }

    // =========================
    // UI LOGIC
    // =========================

    private void UpdateUI()
    {
        if (NetworkManager.Singleton == null)
            return;

        if (lobbyClosedByUser)
            return;

        int players = NetworkManager.Singleton.ConnectedClientsList.Count;

        UpdateStartButtonState(players);

        if (players == 0)
        {
            bigSpinner.SetActive(true);
            lobbyPanel.SetActive(false);
            return;
        }

        lobbyPanel.SetActive(true);
        bigSpinner.SetActive(false);

        if (players == 1)
        {
            player1Text.text = "Клиент 1 : Подключен";
            player2Text.text = "Клиент 2 : Ожидание...";
            statusText.text = "Ожидание второго клиента...";

            smallSpinner.SetActive(true);

            SetPanelColor(client1Panel, true);
            SetPanelColor(client2Panel, false);
        }
        else
        {
            player1Text.text = "Клиент 1 : Подключен";
            player2Text.text = "Клиент 2 : Подключен";
            statusText.text = "Оба клиента подключены";

            smallSpinner.SetActive(false);

            SetPanelColor(client1Panel, true);
            SetPanelColor(client2Panel, true);
        }
    }

    private void UpdateStartButtonState(int players)
    {
        if (startGameButton == null || startGameButtonImage == null)
            return;

        bool canStart = players >= 2;

        startGameButton.interactable = canStart;
        startGameButtonImage.color = canStart ? Color.white : Color.gray;
    }

    private void SetPanelColor(GameObject panel, bool connected)
    {
        if (panel == null) return;

        var img = panel.GetComponent<Image>();
        if (img == null) return;

        img.color = connected ? Color.green : Color.white;
    }
}