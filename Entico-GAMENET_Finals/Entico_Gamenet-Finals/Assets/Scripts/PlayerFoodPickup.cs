using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerFoodPickup : MonoBehaviourPunCallbacks
{
    public int numberOfFood;

    // Start is called before the first frame update
    void Start()
    {
        numberOfFood = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    public void FruitGet(PhotonMessageInfo info)
    {
        Debug.Log("Fruit!");

        numberOfFood++;

        if(numberOfFood >= 3)
        {
            GameManager.instance.EndFruitSpawn();
        }
    }
}
