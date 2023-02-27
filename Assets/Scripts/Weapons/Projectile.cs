using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviourPun
{
    [SerializeReference]
    private float speed;

    [SerializeReference]
    private string targetTag;

    [SerializeField]
    private float destroyTime;

    [SerializeField]
    private float damage;

    [SerializeField]
    private float knockbackForce;

    private Vector2 direction;

    private string killerName;

    public GameObject localPlayerObj;

    private void Start()
    {
        if (photonView.IsMine)
        {
            killerName = localPlayerObj.GetComponent<PlayerScript>().PlayerStats.PlayerName;
        }
        StartCoroutine(DestroyBullet());
    }


    // Update is called once per frame
    private void Update()
    {
        transform.Translate(speed * Time.deltaTime * direction);
    }

    [PunRPC]
    public void SetDirection(Vector2 direction)
    {
        this.direction = direction;
    }

    [PunRPC]
    void Destroy()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(destroyTime);
        GetComponent<PhotonView>().RPC("Destroy", RpcTarget.AllBuffered);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.IsMine) return;

        PlayerScript playerScript = collision.gameObject.GetComponent<PlayerScript>();
        if (playerScript != null)
        {
            Vector2 collisionDirection = playerScript.transform.position - transform.position;
            playerScript.photonView.RPC(
                "OnStrike",
                RpcTarget.AllBuffered,
                collisionDirection,
                knockbackForce,
                damage
            );
            photonView.RPC("Destroy", RpcTarget.AllBuffered);
        }
    }
}
