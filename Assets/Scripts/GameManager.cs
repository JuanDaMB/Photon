using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player1;
    
    void Start()
    {
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if (player.IsLocal)
            {
                GameObject p = PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-3f, 3f), 0, 0), Quaternion.identity, 0);
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
