using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MainPlayer : MonoBehaviour, IOnEventCallback
{
    private PhotonView PV;
    private PlayerObservable Observable;
    private float cooldown;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        Observable = GetComponent<PlayerObservable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine) return;
        Move();
        Cure();
        Bomb();
    }

    private void Move()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(new Vector2(0, 2f) * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector2(0, -2f) * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector2(-2f, 0f) * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector2(2f, 0) * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    #region RPC

    private void Bomb()
    {
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PV.RPC("SetBomb", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    private void SetBomb()
    {
        cooldown = 3f;
        PhotonNetwork.Instantiate("Bomb", transform.position, Quaternion.identity).GetComponent<Bomb>().Init();
    }

    #endregion

    #region RaiseEvent


    private const byte CureEventCode = 1;

    private void Cure()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            GenarateCure();
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void GenarateCure()
    {
        RaiseEventOptions eventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache,
        };

        PhotonNetwork.RaiseEvent(CureEventCode, null, eventOptions, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == CureEventCode)
        {
            Observable.Heal();
        }
    }

    #endregion

}
