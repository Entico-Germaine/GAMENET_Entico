using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class CountdownManager : MonoBehaviourPunCallbacks
{
    public TMP_Text timerText;
    public TMP_Text label;

    public float timeToStartRace = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
        {
            timerText = RacingGameManager.instance.timeText;
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            timerText = DeathRaceGameManager.instance.timeText;
            label = DeathRaceGameManager.instance.label;
            label.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            if (timeToStartRace > 0)
            {
                timeToStartRace -= Time.deltaTime;
                photonView.RPC("SetTime", RpcTarget.AllBuffered, timeToStartRace);
            }
            else if (timeToStartRace < 0)
            {
                photonView.RPC("StartRace", RpcTarget.AllBuffered);
            }
        }

    }

    [PunRPC]
    public void SetTime(float time)
    {
        if(time > 0)
        {
            timerText.text = time.ToString("F1");
        }
        else
        {
            timerText.text = "";
        }
    }

    [PunRPC]
    public void StartRace()
    {
        label.enabled = false;
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            GetComponent<Shooting>().isControlEnabled = true;
        }
        GetComponent<VehicleMovementScript>().isControlEnabled = true;
        this.enabled = false;
    }
}
