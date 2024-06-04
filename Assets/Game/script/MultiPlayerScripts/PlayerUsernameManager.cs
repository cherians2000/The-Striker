using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUsernameManager : MonoBehaviour
{
    [SerializeField] private InputField usernameInput;
    [SerializeField] private Text errorMessageText;

    private void Start()
    {
        if (PlayerPrefs.HasKey("username")) 
        {
            usernameInput.text = PlayerPrefs.GetString("username");
            PhotonNetwork.NickName =PlayerPrefs.GetString("username");
        }
    }
    public void PlayerUsernameInputValueChanger()
    {
        string username = usernameInput.text;
        if(!string.IsNullOrEmpty(username)&& username.Length <= 20)
        {
            PhotonNetwork.NickName = username;
            PlayerPrefs.SetString("username",username);
            errorMessageText.text = "";
            MenuManager.instance.OpenMenu("TitleMenu");
        }
        else
        {
            errorMessageText.text = "Username must not be empty and should be 8 characters or less.";
        }
    }
}
