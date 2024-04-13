using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class LapController : MonoBehaviourPunCallbacks
{
    public List<GameObject> lapTriggers = new List<GameObject>();

    public enum RaiseEventsCode
    {
        WhoFinishedEventCode = 0
    }

    private int FinishOrder;

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == (byte)RaiseEventsCode.WhoFinishedEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            string nickNameOfFinishedPlayer = (string)data[0];
            FinishOrder = (int)data[1];
            int viewId = (int)data[2];

            Debug.Log(nickNameOfFinishedPlayer + " " + FinishOrder);

            GameObject orderUiText = RacingGameManager.instance.finisherTextui[FinishOrder - 1];
            orderUiText.SetActive(true);

            if(viewId == photonView.ViewID) // is you
            {
                orderUiText.GetComponent<TMP_Text>().text = FinishOrder + " " + nickNameOfFinishedPlayer + "( YOU )";
                orderUiText.GetComponent<TMP_Text>().color = Color.red;
            }
            else
            {
                orderUiText.GetComponent<TMP_Text>().text = FinishOrder + " " + nickNameOfFinishedPlayer;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject go in RacingGameManager.instance.lapTriggers)
        {
            lapTriggers.Add(go);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if(lapTriggers.Contains(col.gameObject))
        {
            int indexOfTrigger = lapTriggers.IndexOf(col.gameObject);

            lapTriggers[indexOfTrigger].SetActive(false);

            Debug.Log("LAP");
        }

        if(col.gameObject.tag == "FinishTrigger")
        {
            GameFinish();
        }
    }

    public void GameFinish()
    {
        GetComponent<PlayerSetp>().camera.transform.parent = null;
        GetComponent<VehicleMovementScript>().enabled = false;

        FinishOrder++;

        string nickName = photonView.Owner.NickName;
        int viewId = photonView.ViewID;

        // event data
        object[] data = new object[] { nickName, FinishOrder, viewId };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = false
        };
        PhotonNetwork.RaiseEvent((byte)RaiseEventsCode.WhoFinishedEventCode, data, raiseEventOptions, sendOptions);
    }
}
