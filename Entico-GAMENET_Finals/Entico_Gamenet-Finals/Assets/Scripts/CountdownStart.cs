using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class CountdownStart : MonoBehaviourPunCallbacks
{
    public TMP_Text timerText;
    public TMP_Text label;

    public float timeToStartMatch = 5.0f;

    void Start()
    {
        timerText = GameManager.instance.timer;
        label = GameManager.instance.startLabel;
        label.enabled = true;
        timerText.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (timeToStartMatch > 0)
            {
                timeToStartMatch -= Time.deltaTime;
                photonView.RPC("SetTime", RpcTarget.AllBuffered, timeToStartMatch);
            }
            else if (timeToStartMatch < 0)
            {
                photonView.RPC("StartMatch", RpcTarget.AllBuffered);
            }
        }

    }

    [PunRPC]
    public void SetTime(float time)
    {
        if (time > 0)
        {
            timerText.text = time.ToString("F1");
        }
        else
        {
            timerText.text = "";
        }
    }

    [PunRPC]
    public void StartMatch()
    {
        label.enabled = false;
        this.enabled = false;
        this.GetComponent<Shooting>().enabled = true;
        this.GetComponent<PlayerMovementfps>().enabled = true;
    }
}
