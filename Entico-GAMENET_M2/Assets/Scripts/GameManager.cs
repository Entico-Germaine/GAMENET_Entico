using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;

    public GameObject[] spawnPointsArray;

    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsConnectedAndReady)
        {
            int randXpos = Random.Range(-10, 10);
            int randZpos = Random.Range(-10, 10);
            PhotonNetwork.Instantiate(playerPrefab.name, SetSpawnPointTransform(), Quaternion.identity);
        }

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public Vector3 SetSpawnPointTransform()
    {
        return spawnPointsArray[Random.Range(0, spawnPointsArray.Length - 1)].transform.position;
    }
}
