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

    public GameObject hitEffectPrefab;

    public Canvas whoKilledWhoImage;
    public TMP_Text whoKilledWhoKiller;
    public TMP_Text whoKilledWhoKilled;

    public int kills;
    
    [Header("HP Related Stuff")]
    public float startHealth;
    public float currHealth;
    public Image healthBar;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        currHealth = startHealth;
        healthBar.fillAmount = currHealth / startHealth;

        animator = this.GetComponent<Animator>();
        whoKilledWhoImage.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire()
    {
        RaycastHit hit;
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if(Physics.Raycast(ray, out hit, 200))
        {
            photonView.RPC("CreateHitEffects", RpcTarget.All, hit.point);

            if(hit.collider.gameObject.CompareTag("Player")
                && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                // NOTE: All buffered means current and future players will get broadcast function
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 25);

                if(hit.collider.gameObject.GetComponent<Shooting>().currHealth <= 0)
                {
                    this.gameObject.GetComponent<PhotonView>().RPC("KillCount", RpcTarget.AllBuffered);
                }

            }
        }
    }

    [PunRPC]
    public void TakeDamage(int damage, PhotonMessageInfo info)
    {
        this.currHealth -= damage;
        this.healthBar.fillAmount = currHealth / startHealth;

        if(currHealth <= 0)
        {
            currHealth = 0;
            Die();
            StartCoroutine(FadeWhoKilledWhoCounter((string)info.Sender.NickName,(string)info.photonView.Owner.NickName)); 
        }
    }

    [PunRPC]
    public void CreateHitEffects(Vector3 position)
    {
        GameObject hitEffectGameObject = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        Destroy(hitEffectGameObject, 0.2f);
    }

    public void Die()
    {
        if(photonView.IsMine)
        {
            animator.SetBool("isDead", true);
            StartCoroutine(RespawnCountdown());
        }
        
    }

    IEnumerator RespawnCountdown()
    {
        GameObject respawnText = GameObject.Find("RespawnText");
        float respawnTime = 5.0f;

        while(respawnTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime--;

            transform.GetComponent<PlayerMovementController>().enabled = false;
            respawnText.GetComponent<TMP_Text>().text = "You were killed. Respawning in " + respawnTime.ToString(".00");
        }

        animator.SetBool("isDead", false);
        respawnText.GetComponent<TMP_Text>().text = "";

        int randomPointX = Random.Range(-20, 20);
        int randomPointZ = Random.Range(-20, 20);

        this.transform.position = new Vector3(randomPointX, 0, randomPointZ);
        transform.GetComponent<PlayerMovementController>().enabled = true;

        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
    }

    IEnumerator FadeWhoKilledWhoCounter(string killer, string killed)
    {
        float fadeTime = 3.0f;

        // downside to this is that it won't take multiple kills unless I use a cache
        whoKilledWhoImage.enabled = true;
        whoKilledWhoKiller.text = killer;
        whoKilledWhoKilled.text = killed;

        while (fadeTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            fadeTime--;
        }

        whoKilledWhoKiller.text = "";
        whoKilledWhoKilled.text = "";
        whoKilledWhoImage.enabled = false;
    }
    [PunRPC]
    public void RegainHealth()
    {
        currHealth = 100;
        healthBar.fillAmount = currHealth / startHealth;
    }

    [PunRPC]
    public void KillCount()
    {
        if(photonView.IsMine)
        {
            // could display kill count using this
            kills++;

            if(kills >= 10)
            {
                PhotonNetwork.LeaveRoom();
                SceneManager.LoadScene("LobbyScene");
            }
        }
    }
}
