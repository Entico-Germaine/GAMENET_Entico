using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;

public class Shooting : MonoBehaviourPunCallbacks
{
    public Camera camera;

    //public Canvas whoKilledWhoImage;
    //public TMP_Text whoKilledWhoKiller;
    //public TMP_Text whoKilledWhoKilled;

    public int kills;
    public TMP_Text playerisDeadText;
    private OnLastManStanding lastManStanding;
    public object killedPlayer;

    [Header("HP Related Stuff")]
    public float startHealth;
    public float currHealth;
    public Image healthBar;

    public bool isControlEnabled;

    [SerializeField]
    public GameObject projectileFirePoint;
    [SerializeField]
    public GameObject projectilePrefab;
    public int projectileDamage;
    public int raycastDamage;

    // Start is called before the first frame update
    void Start()
    {
        currHealth = startHealth;
        healthBar.fillAmount = currHealth / startHealth;

        lastManStanding = GetComponent<OnLastManStanding>();
        playerisDeadText = DeathRaceGameManager.instance.deadText;
        playerisDeadText.text = "";

        projectileDamage = 50;
        raycastDamage = 25;
        isControlEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isControlEnabled && photonView.IsMine)
        {
            if(Input.GetMouseButtonDown(0))
            {
                FireProjectile();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                FireRaycast();
            }
        }
    }

    public void FireRaycast()
    {
        RaycastHit hit;
        Ray ray = new Ray(projectileFirePoint.transform.position, projectileFirePoint.transform.forward);

        if (Physics.Raycast(ray, out hit, 200))
        {

            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, raycastDamage);

                if (hit.collider.gameObject.GetComponent<Shooting>().currHealth <= 0)
                {
                    //
                }

            }
        }
    }

    public void FireProjectile()
    {
        GameObject bullet = Instantiate(projectilePrefab, projectileFirePoint.transform.position, projectileFirePoint.transform.rotation);
        bullet.GetComponent<Projectile>().SetDamage(projectileDamage);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(projectileFirePoint.transform.forward * 40, ForceMode.Impulse);
        Destroy(bullet, 3f);
    }

    [PunRPC]
    public void TakeDamage(int damage, PhotonMessageInfo info)
    {
        this.currHealth -= damage;
        this.healthBar.fillAmount = currHealth / startHealth;

        if (currHealth <= 0)
        {
            currHealth = 0;
            Die();
        }
    }

    public void Die()
    {

        if (photonView.IsMine)
        {
            lastManStanding.GetComponent<OnLastManStanding>().PlayerDied();

            GetComponent<Shooting>().enabled = false;
            GetComponent<VehicleMovementScript>().enabled = false;
            playerisDeadText.text = "You Died";
        }
    }
}