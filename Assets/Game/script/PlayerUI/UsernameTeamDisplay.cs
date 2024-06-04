using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class UsernameTeamDisplay : MonoBehaviour
{
    public Text usernameText;
   /* public Text teamText;*/

    public PhotonView view;

    private void Start()
    {
        if(view.IsMine)
        {
            gameObject.SetActive(false);
            
        }

        usernameText.text = view.Owner.NickName;


        if (view.Owner.CustomProperties.ContainsKey("Team"))
        {
            int team = (int)view.Owner.CustomProperties["Team"];
           /* teamText.text = "T" + team;*/

            if (team == 1)
            {
                usernameText.color = Color.blue;
               /* teamText.color = Color.blue;*/
            }
            else if (team == 2)
            {
                usernameText.color = Color.red;
              /*  teamText.color = Color.red;*/
            }
        }

    }
}
