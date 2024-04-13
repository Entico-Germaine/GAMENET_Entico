using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public TMP_Text playerNameText;

    public GameObject fpsModel;
    public GameObject nonFpsModel;

    [SerializeField]
    Camera fpsCamera;

    public GameObject playerUIprefab;

    public PlayerMovementfps playerMovementfps;
    public Shooting shooting;
    // Start is called before the first frame update
    void Start()
    {
        playerUIprefab = this.transform.Find("PlayerName").GetComponent<GameObject>();
        playerMovementfps = this.GetComponent<PlayerMovementfps>();
        shooting = this.GetComponent<Shooting>();

        fpsModel.SetActive(photonView.IsMine);
        nonFpsModel.SetActive(!photonView.IsMine);

        if (photonView.IsMine)
        {
            playerMovementfps.enabled = true;
            fpsCamera.enabled = true;
            shooting.enabled = true;
        }
        else
        {
            playerMovementfps.enabled = false;
            fpsCamera.enabled = false;
            shooting.enabled = false;
        }

        playerNameText.text = photonView.Owner.NickName;
    }
}
