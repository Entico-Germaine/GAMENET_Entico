using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance = null;

    public GameObject playerPrefab;
    public GameObject fruitsPrefab;
    public GameObject endFruitPrefab;

    [Header("Spawners")]
    public GameObject[] spawnPointsArray;
    public GameObject[] spawnFruitsArray;
    public GameObject spawnEndFruit;

    [Header("Countdown")]
    public TMP_Text startLabel;
    public TMP_Text endLabel;
    public TMP_Text timer;

    [Header("LeaderBoard")]
    public GameObject[] leaderBoardUI;
    public GameObject endGameUI;

    //public GameObject leaderboard;

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
        PhotonNetwork.AutomaticallySyncScene = true;

        endLabel.enabled = false;
        //leaderboard.SetActive(false);

        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, SetBeginPlayerSpawnPointTransform(), Quaternion.identity);
        }

        StartCoroutine(spawnFruit());
    }

    public Vector3 SetBeginPlayerSpawnPointTransform()
    {
        return spawnPointsArray[Random.Range(0, spawnPointsArray.Length - 1)].transform.position;
    }

    IEnumerator spawnFruit()
    {
        while(true)
        {
            for(int i = 0; i < spawnFruitsArray.Length-1; i++)
            {
                PhotonNetwork.Instantiate(fruitsPrefab.name, spawnFruitsArray[i].transform.position, Quaternion.identity);
            }
            yield return new WaitForSeconds(10f);
        }
    }

    [PunRPC]
    public void EndFruitSpawn()
    {
        PhotonNetwork.Instantiate(endFruitPrefab.name, spawnEndFruit.transform.position, Quaternion.identity);
    }
}
