using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public TMP_Text playerNameText;

    public GameObject fpsModel;
    public GameObject nonFpsModel;

    public GameObject playerUIprefab;

    public PlayerMovementController playerMovementController;
    public Camera fpsCamera;

    private Animator animator;
    public Avatar fpsAvatar;
    public Avatar nonFPSavatar;

    public Shooting shooting;
    // Start is called before the first frame update
    void Start()
    {
        playerMovementController = this.GetComponent<PlayerMovementController>();
        animator = this.GetComponent<Animator>();
        shooting = this.GetComponent<Shooting>();

        fpsModel.SetActive(photonView.IsMine);
        nonFpsModel.SetActive(!photonView.IsMine);
        
        animator.avatar = photonView.IsMine ? fpsAvatar : nonFPSavatar;

        // order matters - animations did not work when I put this ahead
        animator.SetBool("isLocalPlayer", photonView.IsMine);

        if (photonView.IsMine)
        {
            GameObject playerUI = Instantiate(playerUIprefab);
            playerMovementController.fixedTouchField = playerUI.transform.Find("RotationTouchField").GetComponent<FixedTouchField>();
            playerMovementController.joystick = playerUI.transform.Find("Fixed Joystick").GetComponent<Joystick>();

            Canvas whoKilledWhoImage = playerUI.transform.Find("WhoKilledWho").GetComponent<Canvas>();
            TMP_Text whoKilledWhoKiller = playerUI.transform.Find("WhoKilledWho/Killer").GetComponent<TMP_Text>();
            TMP_Text whoKilledWhoKilled = playerUI.transform.Find("WhoKilledWho/Killed").GetComponent<TMP_Text>();

            shooting.whoKilledWhoImage = whoKilledWhoImage;
            shooting.whoKilledWhoKilled = whoKilledWhoKilled;
            shooting.whoKilledWhoKiller = whoKilledWhoKiller;

            fpsCamera.enabled = true;

            playerUI.transform.Find("FireButton").GetComponent<Button>().onClick.AddListener(()=> shooting.Fire());
        }
        else
        {
            playerMovementController.enabled = false;
            GetComponent<RigidbodyFirstPersonController>().enabled = false;
            fpsCamera.enabled = false;
        }

        playerNameText.text = photonView.Owner.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
