using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class RacingGameManager : MonoBehaviour
{
    public GameObject[] vehiclePrefabs;
    public Transform[] startingPositions;
    public GameObject[] finisherTextui;

    public static RacingGameManager instance = null;

    public TMP_Text timeText;

    public List<GameObject> lapTriggers = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsConnectedAndReady)
        {
            object playerSelectionNumber;

            if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                Debug.Log((int)playerSelectionNumber);

                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                Vector3 initStartPos = startingPositions[actorNumber-1].position;
                Quaternion initStartRot = startingPositions[actorNumber - 1].rotation;
                PhotonNetwork.Instantiate(vehiclePrefabs[(int)playerSelectionNumber].name, initStartPos, initStartRot);
            }
        }

        foreach(GameObject go in finisherTextui)
        {
            go.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
