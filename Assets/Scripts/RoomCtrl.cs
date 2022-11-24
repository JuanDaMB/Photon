using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RoomCtrl : MonoBehaviourPunCallbacks
{
    public const string MAP_PROP_KEY = "map";

    public int map;
    [SerializeField]
    private Transform playersContainer;
    [SerializeField]
    private GameObject playerListItemPrefab;
    [SerializeField]
    private GameObject MasterPanel;
    [SerializeField]
    private Text RoomName;
    [SerializeField]
    private Dropdown DropdownMap;

    #region Photon Callbacks

    public override void OnJoinedRoom()
    {
        map = (int)PhotonNetwork.CurrentRoom.CustomProperties[MAP_PROP_KEY];
        Debug.Log("Map : " + map);
        DropdownMap.value = map;
        RoomName.text = PhotonNetwork.CurrentRoom.Name;
        if (PhotonNetwork.IsMasterClient)
        {
            MasterPanel.SetActive(true);
        }
        else
        {
            MasterPanel.SetActive(false);
        }
        RemovePlayerFromList();
        FillPlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RemovePlayerFromList();
        FillPlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemovePlayerFromList();
        FillPlayerList();
        if (PhotonNetwork.IsMasterClient)
        {
            MasterPanel.SetActive(true);
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("El Master Cliente Se fue \nEl nuevo Master Client es :" + newMasterClient.NickName);     
;    }

    #endregion

    private Color color;
    private Image colorImage;
    
    public void SetRed(float value)
    {
        color.r = value;
        color.a = 1;
        colorImage.color = color;
    }
    public void SetGreen(float value)
    {
        color.g = value;
        color.a = 1;
        colorImage.color = color;
    }
    public void SetBlue(float value)
    {
        color.b = value;
        color.a = 1;
        colorImage.color = color;
    }

    public void SetColor()
    {
        SetColor(color);
    }

    private void SetColor(Color color)
    {
        var red = new ExitGames.Client.Photon.Hashtable() { { "r", color.r } };
        var green = new ExitGames.Client.Photon.Hashtable() { { "g", color.g } };
        var blue = new ExitGames.Client.Photon.Hashtable() { { "b", color.b } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(red);
        PhotonNetwork.LocalPlayer.SetCustomProperties(green);
        PhotonNetwork.LocalPlayer.SetCustomProperties(blue);
        var propsToSet = new ExitGames.Client.Photon.Hashtable() { { "color", true } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(propsToSet);
    }
    
    
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Refresh();
    }

    public List<GameObject> Objects = new List<GameObject>();
    #region private methods
    void FillPlayerList()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject tempListing = Instantiate(playerListItemPrefab, playersContainer);
            Objects.Add(tempListing);
            Text tempText = tempListing.transform.GetChild(0).GetComponent<Text>();
            tempText.text = player.NickName;
            if (player.IsLocal)
            {
                colorImage = tempListing.GetComponentsInChildren<Image>()[1];
            }
            
            
            if (!player.CustomProperties.ContainsKey("color"))
            {
                continue;
            }
            bool colorReady = (bool)player.CustomProperties["color"];
            if (!colorReady) continue;
            Color newColor = new Color(1,1,1,1);
            newColor.r =((float)player.CustomProperties["r"]);
            newColor.g =((float)player.CustomProperties["g"]);
            newColor.b =((float)player.CustomProperties["b"]);
            color = newColor;
            tempListing.GetComponentsInChildren<Image>()[1].color = newColor;
        }
    }

    void Refresh()
    {
        foreach (GameObject o in Objects)
        {
            Destroy(o);
        }
        Objects.Clear();
        FillPlayerList();
    }

    void RemovePlayerFromList()
    {
        // Borra cada entrada de la lista de jugadores
        for (int i = playersContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(playersContainer.GetChild(i).gameObject);
        }
    }
    #endregion

    #region public Methods
    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(map+1);
        }
    }

    public void LeaveRoom() // Retorna al lobby
    {
        UIManager.Instance.GoToLobby();
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        StartCoroutine(rejoinLobby());
    }
    #endregion

    IEnumerator rejoinLobby()
    {
        yield return new WaitForSeconds(2);
        // para forzar la actualización de la lista de salas 
        PhotonNetwork.JoinLobby();
    }

}
