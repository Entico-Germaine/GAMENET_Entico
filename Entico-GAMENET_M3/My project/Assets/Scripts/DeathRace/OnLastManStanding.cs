using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

public class OnLastManStanding : MonoBehaviourPunCallbacks
{
    public enum RaiseEventsCode
    {
        LastManStandingBoard = 0
    }

    private int deathOrder = 0;

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
        if (photonEvent.Code == (byte)RaiseEventsCode.LastManStandingBoard)
        {
            //retrieve data passed for our event
            object[] data = (object[])photonEvent.CustomData;

            string deadPlayerName = (string)data[0];
            deathOrder = (int)data[1];
            int viewId = (int)data[2];

            GameObject orderUiText = DeathRaceGameManager.instance.lastManStandingUI[deathOrder - 1];
            orderUiText.SetActive(true);

            if (viewId == photonView.ViewID) // is you
            {
                orderUiText.GetComponent<TMP_Text>().text = deathOrder + " " + deadPlayerName + "( YOU )";
                orderUiText.GetComponent<TMP_Text>().color = Color.red;
            }
            else
            {
                orderUiText.GetComponent<TMP_Text>().text = deathOrder + " " + deadPlayerName;
            }
        }
    }

    public void PlayerDied()
    {
        deathOrder++;

        string nickName = photonView.Owner.NickName;
        int viewId = photonView.ViewID;

        //event data 
        object[] data = new object[] { nickName, deathOrder, viewId };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOption = new SendOptions
        {
            Reliability = false
        };

        PhotonNetwork.RaiseEvent((byte)RaiseEventsCode.LastManStandingBoard, data, raiseEventOptions, sendOption);
    }
}
