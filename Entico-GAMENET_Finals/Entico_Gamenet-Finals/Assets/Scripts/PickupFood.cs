using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PickupFood : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        StartCoroutine(DestroyAfterSeconds());
    }

    private void OnCollisionEnter(Collision hit)
    {
        if (hit.collider.gameObject.CompareTag("Player"))
        {
            hit.collider.gameObject.GetComponent<PhotonView>().RPC("FruitGet", RpcTarget.AllBuffered);
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyAfterSeconds()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
