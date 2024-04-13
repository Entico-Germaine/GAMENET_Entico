using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class DeathRaceGameManager : MonoBehaviour
{
    public GameObject[] vehiclePrefabs;
    public static DeathRaceGameManager instance = null;

    public GameObject[] spawnPointsArray;
    public TMP_Text timeText;
    public TMP_Text label;
    public TMP_Text deadText;
    public GameObject whoKilledWhoUI;
    public GameObject[] lastManStandingUI;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        object playerSelectionNumber;
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                PhotonNetwork.Instantiate(vehiclePrefabs[(int)playerSelectionNumber].name, SetSpawnPointTransform(), Quaternion.identity);
            }
                
        }

        PhotonNetwork.AutomaticallySyncScene = true;
        whoKilledWhoUI.SetActive(false);

        foreach (GameObject go in lastManStandingUI)
        {
            go.SetActive(false);
        }
    }

    public Vector3 SetSpawnPointTransform()
    {
        return spawnPointsArray[Random.Range(0, spawnPointsArray.Length - 1)].transform.position;
    }
}
