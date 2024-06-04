using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI playerUsername;
    public Text teamText;

    Player player;
    int team;
    public void SetUp(Player _player ,int _team)
    {
        player = _player;
        team = _team;
        playerUsername.text = _player.NickName;
        teamText.text = "T" + _team;
        if (_team == 1)
        {
            teamText.color = Color.blue;
        }
        else if (_team == 2)
        {
            teamText.color = Color.red;
        }
        ExitGames.Client.Photon.Hashtable customProps =new ExitGames.Client.Photon.Hashtable();
        customProps["Team"] = _team;
        _player.SetCustomProperties(customProps);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject) ;
    }
}
