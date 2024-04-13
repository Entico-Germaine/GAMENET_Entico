using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Shooting : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Camera fpsCamera;

    [SerializeField]
    public float fireRate = 0.1f;
    private float fireTimer = 0;

    private Shooting shooting;
    private PlayerMovementfps movement;

    // Start is called before the first frame update
    void Start()
    {
        shooting = this.GetComponent<Shooting>();
        movement = this.GetComponent<PlayerMovementfps>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fireTimer < fireRate)
        {
            fireTimer += Time.deltaTime;
        }

        if (Input.GetButton("Fire1") && fireTimer > fireRate)
        {
            fireTimer = 0.0f;
            Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                Debug.Log(hit.collider.gameObject.name);
                if (hit.collider.gameObject.CompareTag("Player")
                    && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    hit.collider.gameObject.GetComponent<PhotonView>().RPC("Stunned", RpcTarget.AllBuffered, 5);
                }
            }
        }
    }

    [PunRPC]
    public void Stunned(int secondsToWait)
    {
        movement.enabled = false;
        shooting.enabled = false;

        StartCoroutine(StunDuration(secondsToWait));
    }

    IEnumerator StunDuration(int secondsToWait)
    {
        Debug.Log("Stunned");
        yield return new WaitForSeconds(secondsToWait);

        movement.enabled = true;
        shooting.enabled = true;
    }
}
