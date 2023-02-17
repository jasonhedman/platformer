using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    public float speed = 8;

    public float destroyTime = 2;

    public bool isLeft;

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(destroyTime);
        this.GetComponent<PhotonView>().RPC("Destroy", RpcTarget.AllBuffered);
    }

    private void Update()
    {
        if(isLeft)
        {
            transform.Translate(speed * Time.deltaTime * Vector2.left);
        }
        else
        {
            transform.Translate(speed * Time.deltaTime * Vector2.right);
        }
    }

    [PunRPC]
    public void SetIsLeft()
    {
        isLeft = true;
    }
    [PunRPC]
    void Destroy()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.IsMine) return;

        PhotonView target = collision.gameObject.GetComponent<PhotonView>();

        if (target != null && (!target.IsMine || target.IsRoomView))
        {
            this.GetComponent<PhotonView>().RPC("Destroy", RpcTarget.AllBuffered);
        }
    }
}
