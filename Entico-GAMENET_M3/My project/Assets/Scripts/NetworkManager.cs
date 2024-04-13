using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    [Header("Login UI")]
    public GameObject LoginUIPanel;
    public InputField PlayerNameInput;

    [Header("Connecting Info Panel")]
    public GameObject ConnectingInfoUIPanel;

    [Header("Creating Room Info Panel")]
    public GameObject CreatingRoomInfoUIPanel;

    [Header("GameOptions  Panel")]
    public GameObject GameOptionsUIPanel;

    [Header("Create Room Panel")]
    public GameObject CreateRoomUIPanel;
    public InputField RoomNameInputField;
    public string GameMode;

    [Header("Inside Room Panel")]
    public GameObject InsideRoomUIPanel;
    public Text roomInfoText;
    public GameObject playerListPrefab;
    public GameObject playerListPrefabParent;
    public GameObject startGameButton;
    public Text gameModeText;
   
    [Header("Join Random Room Panel")]
    public GameObject JoinRandomRoomUIPanel;

    private Dictionary<int, GameObject> playerListGameObjects;

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        ActivatePanel(LoginUIPanel.name);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region UI Callback Methods
    public void OnLoginButtonClicked()
    {
        string playerName = PlayerNameInput.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            ActivatePanel(ConnectingInfoUIPanel.name);

            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            Debug.Log("PlayerName is invalid!");
        }
    }

    public void OnCancelButtonClicked()
    {
        ActivatePanel(GameOptionsUIPanel.name);
    }
    
    public void OnCreateRoomButtonClicked()
    {
        ActivatePanel(CreatingRoomInfoUIPanel.name);

        if(GameMode != null)
        {
            string roomName = RoomNameInputField.text;

            if (string.IsNullOrEmpty(roomName))
            {
                roomName = "Room " + Random.Range(1000, 10000);
            }

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 3;
            //custom properties
            string[] roomPropertiesInLobby = { "gm" }; // gm = game mode

            //gamemodes
            // rac - racing
            // dr - death race
            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", GameMode } };

            roomOptions.CustomRoomPropertiesForLobby = roomPropertiesInLobby;
            roomOptions.CustomRoomProperties = customRoomProperties;
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }  
    }

    public void OnJoinRandomRoomClicked(string gameMode)
    {
        GameMode = gameMode;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", gameMode } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    public void OnBackButtonClick()
    {
        ActivatePanel(GameOptionsUIPanel.name);
    }

    public void OnLeaveGameButtonClick()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnStartGameButtonClick()
    {
        if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gm"))
        {
            if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
            {
                // racing game mode load
                PhotonNetwork.LoadLevel("RacingScene");
            }
            else if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
            {
                // death race game mode load
                PhotonNetwork.LoadLevel("DeathRaceScene");
            }
        }
    }

    #endregion

    #region Photon Callbacks
    public override void OnConnected()
    {
        Debug.Log("Connected to Internet");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName+ " is connected to Photon");
        ActivatePanel(GameOptionsUIPanel.name);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom + " has been created");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has joined room " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Player count: " + PhotonNetwork.CurrentRoom.PlayerCount);
        ActivatePanel(InsideRoomUIPanel.name);

        object gameModeName;
        if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gm", out gameModeName))
        {
            Debug.Log(gameModeName.ToString());
            roomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + 
                " / " + PhotonNetwork.CurrentRoom.MaxPlayers;

            if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
            {
                gameModeText.text = "Racing Mode";
            }
            else if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
            {
                gameModeText.text = "Death Race Mode";
            }
        }

        if(playerListGameObjects == null)
        {
            playerListGameObjects = new Dictionary<int, GameObject>();
        }

        foreach(Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerListItem = Instantiate(playerListPrefab);
            playerListItem.transform.SetParent(playerListPrefabParent.transform);
            playerListItem.transform.localScale = Vector3.one;

            playerListItem.GetComponent<PlayerListItemInit>().Initialize(player.ActorNumber, player.NickName);

            object isPlayerReady;
            if(player.CustomProperties.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                playerListItem.GetComponent<PlayerListItemInit>().SetPlayerReady((bool)isPlayerReady);
            }
            playerListGameObjects.Add(player.ActorNumber, playerListItem);
        }

        if(PhotonNetwork.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
        else
        {
            startGameButton.SetActive(false);
        }
        
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if (GameMode != null)
        {
            string roomName = RoomNameInputField.text;

            if (string.IsNullOrEmpty(roomName))
            {
                roomName = "Room " + Random.Range(1000, 10000);
            }

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 3;
            //custom properties
            string[] roomPropertiesInLobby = { "gm" }; // gm = game mode

            //gamemodes
            // rac - racing
            // dr - death race
            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", GameMode } };

            roomOptions.CustomRoomPropertiesForLobby = roomPropertiesInLobby;
            roomOptions.CustomRoomProperties = customRoomProperties;
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        GameObject playerListItem = Instantiate(playerListPrefab);
        playerListItem.transform.SetParent(playerListPrefabParent.transform);
        playerListItem.transform.localScale = Vector3.one;

        playerListItem.GetComponent<PlayerListItemInit>().Initialize(player.ActorNumber, player.NickName);

        playerListGameObjects.Add(player.ActorNumber, playerListItem);

        roomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount +
                " / " + PhotonNetwork.CurrentRoom.MaxPlayers;

        startGameButton.SetActive(CheckAllPlayerReady());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(playerListGameObjects[otherPlayer.ActorNumber].gameObject);
        playerListGameObjects.Remove(otherPlayer.ActorNumber);

        roomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount +
                " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    public override void OnLeftRoom()
    {
        ActivatePanel(GameOptionsUIPanel.name);

        foreach(GameObject playerListGameObject in playerListGameObjects.Values)
        {
            Destroy(playerListGameObject);
        }

        playerListGameObjects.Clear();
        playerListGameObjects = null;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        GameObject playerListGameObject;
        if(playerListGameObjects.TryGetValue(targetPlayer.ActorNumber, out playerListGameObject))
        {
            object isPlayerReady;
            if (changedProps.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                playerListGameObject.GetComponent<PlayerListItemInit>().SetPlayerReady((bool)isPlayerReady);
            }
        }
        startGameButton.SetActive(CheckAllPlayerReady());
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if(PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            startGameButton.SetActive(CheckAllPlayerReady());
        }
    }
    #endregion

    #region Public Methods
    public void ActivatePanel(string panelNameToBeActivated)
    {
        LoginUIPanel.SetActive(LoginUIPanel.name.Equals(panelNameToBeActivated));
        ConnectingInfoUIPanel.SetActive(ConnectingInfoUIPanel.name.Equals(panelNameToBeActivated));
        CreatingRoomInfoUIPanel.SetActive(CreatingRoomInfoUIPanel.name.Equals(panelNameToBeActivated));
        CreateRoomUIPanel.SetActive(CreateRoomUIPanel.name.Equals(panelNameToBeActivated));
        GameOptionsUIPanel.SetActive(GameOptionsUIPanel.name.Equals(panelNameToBeActivated));
        JoinRandomRoomUIPanel.SetActive(JoinRandomRoomUIPanel.name.Equals(panelNameToBeActivated));
        InsideRoomUIPanel.SetActive(InsideRoomUIPanel.name.Equals(panelNameToBeActivated));
    }

    public void SetGameMode(string gameMode)
    {
        GameMode = gameMode;
    }
    #endregion

    #region Private Methods

    private bool CheckAllPlayerReady()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        foreach(Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if(p.CustomProperties.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                if(!(bool) isPlayerReady)
                {
                    return false;
                }
            }
        }

        return true;
    }

    #endregion
}
