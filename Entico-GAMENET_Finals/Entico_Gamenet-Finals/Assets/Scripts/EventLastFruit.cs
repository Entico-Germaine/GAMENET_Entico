using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

public class EventLastFruit : MonoBehaviourPunCallbacks
{
    public enum RaiseEventsCode
    {
        TouchedEndFruit = 0
    }

    int finishOrder;

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
        if (photonEvent.Code == (byte)RaiseEventsCode.TouchedEndFruit)
        {
            object[] data = (object[])photonEvent.CustomData;

            string nickName = (string)data[0];
            int food = (int)data[1];

            GameObject leaderBoard = GameManager.instance.leaderBoardUI[finishOrder - 1];
            leaderBoard.SetActive(true);

            leaderBoard.transform.Find("PlayerName").GetComponent<TMP_Text>().text = nickName;
            leaderBoard.transform.Find("PlayerNumberofFruit").GetComponent<TMP_Text>().text = food.ToString();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finish"))
        {
            EndFruitTouched();
        }
    }

    public void EndFruitTouched()
    {
        finishOrder++;

        GameObject endUI = GameManager.instance.endGameUI;
        endUI.SetActive(true);

        GetComponent<Shooting>().enabled = false;
        GetComponent<PlayerMovementfps>().enabled = false;
        Debug.Log("Peach Touched");

        int food = this.GetComponent<PlayerFoodPickup>().numberOfFood;
        string nickName = photonView.Owner.NickName;

        // event data
        object[] data = new object[] { nickName, food };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = false
        };

        PhotonNetwork.RaiseEvent((byte)RaiseEventsCode.TouchedEndFruit, data, raiseEventOptions, sendOptions);
    }
}