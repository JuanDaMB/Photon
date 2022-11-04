using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public enum RegionCode
{
    AUTO = 0,
    CAE = 1,
    EU = 2,
    US = 3,
    USW = 4,
    SA = 5
}
public class ConnectCrtl : MonoBehaviourPunCallbacks
{

    [SerializeField]
    string gameVersion = "1";
    [SerializeField]
    string regionCode = null;
    [SerializeField]
    private GameObject PanelConnect;
    [SerializeField]
    private GameObject PanelRoom;

    public TMP_Dropdown dropdownRegion;
    public Color color;
    public Image colorImage;
    public Button button;
    public Button readyButton;
    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        color = Color.white;
        PopulateDropdown();
    }

    private void PopulateDropdown()
    {
        dropdownRegion.ClearOptions();
        List<TMP_Dropdown.OptionData> datas = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < 5; i++)
        {
            TMP_Dropdown.OptionData item = new TMP_Dropdown.OptionData
            {
                text = ((RegionCode)i).ToString()
            };
            datas.Add(item);
        }
        dropdownRegion.AddOptions(datas);
    }

    public void SetRegion(int index)
    {
        RegionCode region = (RegionCode)index;
        regionCode = region == RegionCode.AUTO ? null : region.ToString();
        Debug.Log("Region seleccionada: " + regionCode);
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = regionCode;
    }

    public void SetUsername(string value)
    {
        PhotonNetwork.NickName = value;
    }

    public void SetRed(float value)
    {
        color.r = value;
        colorImage.color = color;
    }
    public void SetGreen(float value)
    {
        color.g = value;
        colorImage.color = color;
    }
    public void SetBlue(float value)
    {
        color.b = value;
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
        readyButton.interactable = true;
    }

    public void SetReady()
    {
        var propsToSet = new ExitGames.Client.Photon.Hashtable() { { "ready", true } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(propsToSet);
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    void SetButton(bool state, string msg)
    {
        button.GetComponentInChildren<TextMeshProUGUI>().text = msg;
        button.GetComponent<Button>().interactable = state;
    }

    void ShowRoomPanel()
    {
        readyButton.interactable = false;
        PanelConnect.SetActive(false);
        PanelRoom.SetActive(true);
    }

    #region MonoBehaviourPunCallbacks Callbacks


    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN");
        SetButton(true, "LETS BATTLE");
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions());
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        SetButton(false, "WATING PLAYERS");

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            Debug.Log("Room is Ready");     
            ShowRoomPanel();
        }

    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " Se Ha unido al cuarto, Players: " + PhotonNetwork.CurrentRoom.PlayerCount);
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
            //PhotonNetwork.LoadLevel("Game");
            ShowRoomPanel();
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("color"))
        {
            foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                if (!player.CustomProperties.ContainsKey("color"))
                {
                    continue;
                }
                bool colorReady = (bool)player.CustomProperties["color"];
                if (!colorReady) continue;
                Color newColor = Color.white;
                newColor.r =((float)player.CustomProperties["r"]);
                newColor.g =((float)player.CustomProperties["g"]);
                newColor.b =((float)player.CustomProperties["b"]);
            }
        }

        if (!changedProps.ContainsKey("ready")) return;
        int playersReady = 0;
        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values) 
        {
            if (!player.CustomProperties.ContainsKey("ready"))
            {
                continue;
            }
            bool ready = (bool)player.CustomProperties["ready"];
            if (ready)
            {
                playersReady++;
            }
            if(playersReady == 2)
            {
                PhotonNetwork.LoadLevel("Game");
            }
        }
    }

    #endregion

}
