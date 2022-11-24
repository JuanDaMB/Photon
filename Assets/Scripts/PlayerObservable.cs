using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerObservable : MonoBehaviourPunCallbacks, IPunObservable
{
    public SpriteRenderer SR;

    public int laps;
    private Color C;
    private string nickname;
    
    // Start is called before the first frame update
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        C = SR.color;
    }

    private void Update()
    {
        if (laps > 10)
        {
            Winner.Instance.SetWinner(nickname);
        }
    }

    public void SetName(string newNickname)
    {
        nickname = newNickname;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(SR.color.r);
            stream.SendNext(SR.color.g);
            stream.SendNext(SR.color.b);
            stream.SendNext(laps);
        }
        else
        {
            C.r = (float)stream.ReceiveNext();
            C.g = (float)stream.ReceiveNext();
            C.b = (float)stream.ReceiveNext();
            C.a = 1f;
            SR.color = C;
            laps = (int)stream.ReceiveNext();
        }
    }
}
