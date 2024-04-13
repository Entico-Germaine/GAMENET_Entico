using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetp : MonoBehaviourPunCallbacks
{
    public Camera camera;

    public GameObject playerHeader;

    [SerializeField]
    public TMP_Text playerName;

    void Start()
    {
        this.camera = transform.Find("Camera").GetComponent<Camera>();
        
        if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
        {
            playerHeader.SetActive(false);
            GetComponent<VehicleMovementScript>().enabled = photonView.IsMine;
            GetComponent<LapController>().enabled = photonView.IsMine;
            camera.enabled = photonView.IsMine;
        }
        else if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            playerHeader.SetActive(true);
            GetComponent<LapController>().enabled = false;
            playerName.text = photonView.Owner.NickName;

            if (photonView.IsMine)
            {
                GetComponent<VehicleMovementScript>().enabled = true;
                GetComponent<Shooting>().enabled = true;
                camera.enabled = true;
            }
            else
            {
                GetComponent<VehicleMovementScript>().enabled = false;
                //GetComponent<Shooting>().enabled = false;
                camera.enabled = false;
            }
        } 
    }
}
