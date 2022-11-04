using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerObservable : MonoBehaviourPunCallbacks, IPunObservable
{
    public SpriteRenderer SR;

    public Image healthBar;

    public int life;
    public TextMeshProUGUI text;
    private string nickName;
    private Color C;
    
    // Start is called before the first frame update
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        C = SR.color;
    }

    public void SetName(string nickName)
    {
        this.nickName = nickName;
        text.text = this.nickName;
    }

    public void TakeDamage()
    {
        if (life <= 0)return;
        life--;
        healthBar.fillAmount = life / 3f;
    }

    public void Heal()
    {
        if (life >= 3) return;
        life++;
        healthBar.fillAmount = life / 3f;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(SR.color.r);
            stream.SendNext(SR.color.g);
            stream.SendNext(SR.color.b);
            stream.SendNext(life);
            stream.SendNext(nickName);
        }
        else
        {
            C.r = (float)stream.ReceiveNext();
            C.g = (float)stream.ReceiveNext();
            C.b = (float)stream.ReceiveNext();
            C.a = 1f;
            SR.color = C;
            life = (int)stream.ReceiveNext();
            healthBar.fillAmount = life / 3f;
            nickName = (string)stream.ReceiveNext();;
            text.text = nickName;
        }
    }
}
