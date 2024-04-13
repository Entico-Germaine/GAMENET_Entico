using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class CountdownEndGame : MonoBehaviourPunCallbacks
{
    public TMP_Text timerText;
    public TMP_Text label;

    public float timeToEndMatch = 5.0f;

    void Start()
    {
        timerText = GameManager.instance.timer;
        label = GameManager.instance.endLabel;
        label.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (timeToEndMatch > 0)
            {
                timeToEndMatch -= Time.deltaTime;
                photonView.RPC("SetTime", RpcTarget.AllBuffered, timeToEndMatch);
            }
            else if (timeToEndMatch < 0)
            {
                photonView.RPC("EndMatch", RpcTarget.AllBuffered);
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
    public void EndMatch()
    {
        label.enabled = false;
        this.enabled = false;
        this.GetComponent<Shooting>().enabled = false;
        this.GetComponent<PlayerMovementfps>().enabled = false;

        // call raised event for leaderboard
    }
}
