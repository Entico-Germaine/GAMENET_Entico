using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LaunchManager : MonoBehaviourPunCallbacks
{
    public GameObject EnterGamePanel;
    public GameObject ConnectStatusPanel;
    public GameObject LobbyPanel;

    // Start is called before the first frame update
    void Start()
    {
        EnterGamePanel.SetActive(true);
        ConnectStatusPanel.SetActive(false);
        LobbyPanel.SetActive(false);
    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    void Update()
    {
        
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.NickName.ToString() + " has connected to photon servers");
        ConnectStatusPanel.SetActive(false);
        LobbyPanel.SetActive(true);
    }
    public override void OnConnected()
    {
        Debug.Log("Connected to the net");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateAndJoinRoom();
    }

    public void ConnectToPhotonServer()
    {
        if(!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            ConnectStatusPanel.SetActive(true);
            EnterGamePanel.SetActive(false);
        }
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    
    public void CreateAndJoinRoom()
    {
        string randomRoomName = "Room" + Random.Range(0, 1000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 20;

        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.NickName.ToString() + " has entered" + PhotonNetwork.CurrentRoom.Name );
        PhotonNetwork.LoadLevel("GameScene");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " has entered room " + PhotonNetwork.CurrentRoom.Name + ". Room has now: " + PhotonNetwork.CurrentRoom.PlayerCount + " players");
    }
}
