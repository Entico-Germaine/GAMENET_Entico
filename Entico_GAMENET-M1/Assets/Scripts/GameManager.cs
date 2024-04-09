using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    [SerializeField]
    private GameObject playerPrefab;
    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsConnected)
        {
            if(playerPrefab != null)
            {
                int xRandPoint = Random.Range(-20, 20);
                int zRandomPoint = Random.Range(-20, 20);
                int yPoint = 1;

                PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(xRandPoint, yPoint, zRandomPoint), Quaternion.identity);
            }
        }
    }

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.NickName + " has joined room");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " has entered room " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Room has now " + PhotonNetwork.CurrentRoom.PlayerCount + "/20 players");
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("GameLauncherScene");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
