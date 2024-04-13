using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviour
{
    public int Damage;

    private void Update()
    {
        
    }

    private void OnCollisionEnter(Collision hit)
    {
        if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
        {
            hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, Damage);
            Debug.Log("projectile hit!");
            Destroy(gameObject);
            
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("projectile hit something else!");
        }
    }

    public void SetDamage(int damage)
    {
        Damage = damage;
    }
}
