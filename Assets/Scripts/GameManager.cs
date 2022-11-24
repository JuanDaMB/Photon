using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    
    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("{0} Se ha unido a la partida", other.NickName); // not seen if you're the player connecting

        if (PhotonNetwork.IsMasterClient)
        {
          
        }
    }
    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.LogFormat("{0} Ha abandonado la partida", other.NickName);

        if (PhotonNetwork.IsMasterClient)
        {

        }
    }
    
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }
    void Start()
    {
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if (player.IsLocal)
            {
                GameObject p = PhotonNetwork.Instantiate("Player", new Vector3(2f, Random.Range(-3.25f, -4.25f), 0), Quaternion.identity, 0);
                Color color = new Color
                {
                    r = (float)player.CustomProperties["r"],
                    g = (float)player.CustomProperties["g"],
                    b = (float)player.CustomProperties["b"],
                    a = 1
                };
                p.GetComponent<SpriteRenderer>().color = color;
                p.GetComponent<PlayerObservable>().SetName(player.NickName);
            }
        }
    }
}
