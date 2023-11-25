using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Connection Status Panel")]
    public TextMeshProUGUI connectionStatusText;

    [Header("Login Panel")]
    public GameObject LoginUIPanel;
    public TMP_InputField PlayerNameInput;

    [Header("Connecting... Panel")]
    public GameObject ConnectingUIPanel;

    [Header("Game Options Panel")]
    public GameObject GameOptionsUIPanel;

    [Header("Create Room Panel")]
    public GameObject CreateRoomUIPanel;
    public TMP_InputField RoomNameInputField;
    public TMP_InputField MaxPlayersInputField;
    public string PlatformSize;
    public string TileVanishRate;

    [Header("Creating... Panel")]
    public GameObject CreatingUIPanel;

    [Header("Room List Panel")]
    public GameObject RoomListUIPanel;
    public GameObject roomItemPrefab;
    public GameObject roomListParent;

    [Header("Joining... Panel")]
    public GameObject JoiningUIPanel;

    [Header("Inside Room Panel")]
    public GameObject InsideRoomUIPanel;
    public TextMeshProUGUI RoomNameText;
    public TextMeshProUGUI CurrentPlayersText;
    public TextMeshProUGUI PlatformSizeText;
    public TextMeshProUGUI TileVanishRateText;
    public GameObject playerListPrefab;
    public GameObject playerListParent;
    public GameObject StartGameButton;

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListGameObjects;
    private Dictionary<int, GameObject> playerListGameObjects;

    #region Unity Functions
    // Start is called before the first frame update
    void Start()
    {
        // Holds all existing rooms
        cachedRoomList = new Dictionary<string, RoomInfo>();

        // Holds all game objects in rooms
        roomListGameObjects = new Dictionary<string, GameObject>();

        ActivatePanel(LoginUIPanel.name);

        // players who will be joining your room will also be loading the same level
        // when host loads a scene, all other players will load that same scene
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Provides information on connection to Photon Servers.
        connectionStatusText.text = "Connection Status: " + PhotonNetwork.NetworkClientState;
    }

    #endregion

    #region UI Callbacks
    public void OnLoginButtonClicked()
    {
        string playerName = PlayerNameInput.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            ActivatePanel(ConnectingUIPanel.name);

            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();

                // --> public override void OnConnectedToMaster()
            }
        }
        else
        {
            Debug.Log("PlayerName is invalid!");
        }
    }

    public void OnCreateRoomButtonClicked()
    {
        ActivatePanel(CreatingUIPanel.name);

        if (PlatformSize != null && TileVanishRate != null && int.Parse(MaxPlayersInputField.text) > 1)
        {
            string roomName = RoomNameInputField.text;

            if (string.IsNullOrEmpty(roomName))
            {
                roomName = "Room " + Random.Range(1000, 10000);
            }

            RoomOptions roomOptions = new RoomOptions();

            // Typcast input field text to byte <- Convert input field text to integer by parsing it.
            roomOptions.MaxPlayers = (byte)int.Parse(MaxPlayersInputField.text);

            if (string.IsNullOrEmpty(MaxPlayersInputField.text))
            {
                roomOptions.MaxPlayers = 3;
            }

            // Initialize Platform Size and Tile Vanish Rate in Hashtable
            string[] roomPropertiesInLobby = { "ps" };
            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "ps", PlatformSize }, { "tvr", TileVanishRate }, { "players", 0 } };

            roomOptions.CustomRoomPropertiesForLobby = roomPropertiesInLobby;
            roomOptions.CustomRoomProperties = customRoomProperties;

            // Only using Default Lobby Type
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
    }

    // Go to Room List Panel
    public void OnShowRoomListButtonClicked()
    {
        if (!PhotonNetwork.InLobby) // If not yet in lobby, join lobby when checking room list
        {
            PhotonNetwork.JoinLobby();
        }

        ActivatePanel(RoomListUIPanel.name);
    }

    // Leave Room List Panel
    public void OnBackButtonClicked()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        ActivatePanel(GameOptionsUIPanel.name);
    }

    // Leave Inside Room Panel
    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnStartGameButtonClicked()
    {
        Debug.Log("room opened");

        /*
        // NOTE: Won't update, find out why
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "players", PhotonNetwork.CurrentRoom.PlayerCount } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(customRoomProperties);

        object type;

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("players", out type))
        {
            Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
            Debug.Log("Players: " + (int)type);
        }
        */

        // There must be at least two people in the room to start the game.
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("ps"))
            {
                if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("Small"))
                {
                    PhotonNetwork.LoadLevel("PlatformSceneSmall");
                }
                else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("Medium"))
                {
                    PhotonNetwork.LoadLevel("PlatformSceneMedium");
                }
                else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("Large"))
                {
                    PhotonNetwork.LoadLevel("PlatformSceneLarge");
                }
            }
        }
        else
        {
            Debug.Log("needs more people to start!");
        }
    }

    #endregion

    #region Photon PUN Callbacks

    // Check if connected to internet (will be called first).
    public override void OnConnected()
    {
        Debug.Log("Connected to the Internet!");
    }

    // Check if connected to Photon server.
    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has connected to photon servers");
        ActivatePanel(GameOptionsUIPanel.name);
    }

    // Check if a new room has been created.
    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " created!");
    }

    // Check if the person who created the room has now joined the created room.
    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has joined the " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Player count: " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers);

        ActivatePanel(InsideRoomUIPanel.name);

        RoomNameText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
        CurrentPlayersText.text = "Current Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        object type;

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("ps", out type))
        {
            PlatformSizeText.text = "Platform Size: " + (string)type;
        }
        
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("tvr", out type))
        {
            TileVanishRateText.text = "Tile Vanish Rate: " + (string)type;
        }

        if (playerListGameObjects == null)
        {
            playerListGameObjects = new Dictionary<int, GameObject>();
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerItem = Instantiate(playerListPrefab);
            playerItem.transform.SetParent(playerListParent.transform);
            playerItem.transform.localScale = Vector3.one;

            playerItem.GetComponent<PlayerListManager>().Initialize(player.ActorNumber, player.NickName);

            object isPlayerReady;
            if (player.CustomProperties.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                playerItem.GetComponent<PlayerListManager>().SetPlayerReady((bool)isPlayerReady);
            }

            playerListGameObjects.Add(player.ActorNumber, playerItem);
        }

        // Default State of Start Game Button
        StartGameButton.SetActive(false);
    }

    // Whenever a room is updated in a lobby, it will return a list of all room information
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // Clear previous items in order to refresh room list
        ClearRoomListGameObjects();
        Debug.Log("OnRoomListUpdate called");

        StartGameButton.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient);
        
        foreach (RoomInfo info in roomList)
        {
            Debug.Log(info.Name);

            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList) // room is: not open, not visible, or just removed from list
            {
                if (cachedRoomList.ContainsKey(info.Name)) // check to make sure there are no duplicates
                {
                    cachedRoomList.Remove(info.Name);
                }
            }
            else
            {
                // update existing rooms info
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList[info.Name] = info;
                }
                else
                {
                    cachedRoomList.Add(info.Name, info);
                }
            }
        }

        foreach (RoomInfo info in cachedRoomList.Values)
        {
            GameObject listItem = Instantiate(roomItemPrefab);
            listItem.transform.SetParent(roomListParent.transform);
            listItem.transform.localScale = Vector3.one; // update scale

            listItem.transform.Find("Room Name (TMP)").GetComponent<TextMeshProUGUI>().text = info.Name;
            listItem.transform.Find("Room Players (TMP)").GetComponent<TextMeshProUGUI>().text = info.PlayerCount + "/" + info.MaxPlayers;

            object platformType;

            if (info.CustomProperties.TryGetValue("ps", out platformType))
            {
                listItem.transform.Find("Platform Size (TMP)").GetComponent<TextMeshProUGUI>().text = (string)platformType;
            }

            object tileVanishType;

            if (info.CustomProperties.TryGetValue("tvr", out tileVanishType))
            {
                listItem.transform.Find("Tile Vanish Rate (TMP)").GetComponent<TextMeshProUGUI>().text = (string)tileVanishType;
            }

            listItem.transform.Find("JoinRoomButton").GetComponent<Button>().onClick.AddListener(() => OnJoinRoomClicked(info.Name));

            roomListGameObjects.Add(info.Name, listItem);
        }
    }

    public override void OnLeftLobby()
    {
        ClearRoomListGameObjects();
        cachedRoomList.Clear();
    }

    // Used to update the Inside Room Panel on all players
    // OnPlayerEnteredRoom and OnPlayerLeftRoom are callbacks for other players joining and leaving the room that you're currently in
    public override void OnPlayerEnteredRoom(Player newPlayer) // First player will see the names of newer players appear on their end
    {
        GameObject playerListItem = Instantiate(playerListPrefab);
        playerListItem.transform.SetParent(playerListParent.transform);
        playerListItem.transform.localScale = Vector3.one;

        playerListItem.GetComponent<PlayerListManager>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

        playerListGameObjects.Add(newPlayer.ActorNumber, playerListItem);

        // To update player count
        CurrentPlayersText.text = "Current Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        // Check status of Start Game Button whenever a new player enters
        StartGameButton.SetActive(CheckAllPlayerReady());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // Destroy PlayerListPrefab Game Object
        Destroy(playerListGameObjects[otherPlayer.ActorNumber].gameObject);

        // Remove from Dictionary
        playerListGameObjects.Remove(otherPlayer.ActorNumber);

        // To update player count
        CurrentPlayersText.text = "Current Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    public override void OnLeftRoom()
    {
        ActivatePanel(GameOptionsUIPanel.name);

        foreach (GameObject playerlistGameObject in playerListGameObjects.Values)
        {
            Destroy(playerlistGameObject);
        }

        playerListGameObjects.Clear();
        playerListGameObjects = null;
    }

    /*
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
    }
    */

    // Used to update properties (ie. check mark bool) for all players
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        GameObject playerlistGameObject;

        if (playerListGameObjects.TryGetValue(targetPlayer.ActorNumber, out playerlistGameObject))
        {
            object isPlayerReady;

            if (changedProps.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                playerlistGameObject.GetComponent<PlayerListManager>().SetPlayerReady((bool)isPlayerReady);
            }
        }

        // Update Start Game Button in case another player changes his/her status.
        StartGameButton.SetActive(CheckAllPlayerReady());
    }

    // In case the Master Client switches
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // base.OnMasterClientSwitched(newMasterClient);

        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            StartGameButton.SetActive(CheckAllPlayerReady());
        }
    }
    #endregion

    #region Public Methods
    public void ActivatePanel(string panelNameToBeActivated)
    {
        LoginUIPanel.SetActive(LoginUIPanel.name.Equals(panelNameToBeActivated));
        ConnectingUIPanel.SetActive(ConnectingUIPanel.name.Equals(panelNameToBeActivated));
        GameOptionsUIPanel.SetActive(GameOptionsUIPanel.name.Equals(panelNameToBeActivated));
        CreateRoomUIPanel.SetActive(CreateRoomUIPanel.name.Equals(panelNameToBeActivated));
        CreatingUIPanel.SetActive(CreatingUIPanel.name.Equals(panelNameToBeActivated));
        RoomListUIPanel.SetActive(RoomListUIPanel.name.Equals(panelNameToBeActivated));
        JoiningUIPanel.SetActive(JoiningUIPanel.name.Equals(panelNameToBeActivated));
        InsideRoomUIPanel.SetActive(InsideRoomUIPanel.name.Equals(panelNameToBeActivated));
    }

    // For toggles
    public void SetPlatformSize(string platformSize)
    {
        PlatformSize = platformSize;
    }

    public void SetTileVanishRate(string tileVanishRate)
    {
        TileVanishRate = tileVanishRate;
    }

    #endregion

    #region Private Methods
    private void OnJoinRoomClicked(string roomName)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        PhotonNetwork.JoinRoom(roomName);
    }

    private void ClearRoomListGameObjects()
    {
        foreach (var item in roomListGameObjects.Values)
        {
            Destroy(item);
        }

        roomListGameObjects.Clear();
    }
    private bool CheckAllPlayerReady()
    {
        // Only activates for the Master Client
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;

            // if at least one isPlayerReady is false
            if (p.CustomProperties.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        // only occurs if no other end statement resulted in false, which ends the function prematurely.
        return true;
    }

    #endregion
}
