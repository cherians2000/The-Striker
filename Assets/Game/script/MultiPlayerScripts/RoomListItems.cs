using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
public class RoomListItems : MonoBehaviour
{
    [SerializeField] Text roomNameText;

    public RoomInfo info;
    

    public void SetUp(RoomInfo _info)
    {
        info =_info;
        roomNameText.text = _info.Name;
    }

    public void onClick()
    {
        Launcher.Instance.JoinRoom(info);
    }
}
