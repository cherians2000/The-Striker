using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
   public static GameManager instance;
  public bool IsMenuOpened =false;
    public GameObject menuUI;
    public GameObject gameOverPanel;
    public Text winnerText;
    public Text blueTeamScore;
    public Text redTeamScore;

    ScoreBoard scoreBoard;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        scoreBoard = ScoreBoard.Instance;
    }
    
    public void LeaveGame()
    {
        Debug.Log("Gameleaved");
        Application.Quit();
    }
    public void ShowGameOverPanel()
    {
       
            gameOverPanel.SetActive(true);
            Time.timeScale = 0;
            string winningTeam = DetermineWinningTeam();
            winnerText.text = winningTeam.ToString();
            blueTeamScore.text = scoreBoard.blueTeamScore.ToString(); // Update blue team score text
            redTeamScore.text = scoreBoard.redTeamScore.ToString(); // Update red team score text
        

    }
    private string DetermineWinningTeam()
    {
        if (scoreBoard.blueTeamScore > scoreBoard.redTeamScore)
        {
            return "Blue Wins";
        }
        else if (scoreBoard.redTeamScore > scoreBoard.blueTeamScore)
        {
            return "Red Wins";
        }
        else
        {
            return "Tie"; // Handle tie condition if needed
        }
    }
    public void RestarGame()
    {
        //restart
    }
}
