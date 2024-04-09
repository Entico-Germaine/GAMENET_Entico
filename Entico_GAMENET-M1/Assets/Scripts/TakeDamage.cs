using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class TakeDamage : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Image healthbar;

    private float startHealth = 100;
    private float currHealth;
    // Start is called before the first frame update
    void Start()
    {
        currHealth = startHealth;
        healthbar.fillAmount = currHealth / startHealth;
    }

    [PunRPC]
    public void TakingDamage(int damage)
    {
        currHealth -= damage;

        healthbar.fillAmount = currHealth / startHealth;

        if (currHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if(photonView.IsMine)
        {
            GameManager.instance.LeaveRoom();
        }
    }
}
