using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player
{
    public Player(string aName,int aID)
    {
        Username = aName;
        ID = aID;
    }
    public string Username = "";
    public int Score = 0;
    public int ID = 0;
    public int LastScore = 0;
}


public class GameManager : MonoBehaviour
{
    public static GameManager _instance=null;
    public static GameManager instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.Find("GameManager").GetComponent<GameManager>();
            return _instance;
        }
    }
    public Text p1_name;
    public Text p1_score;
    public Text p1_lastScore;
    public Text p2_name;
    public Text p2_score;
    public Text p2_lastScore;
    public Text scoreOverview;
    public Text tileLeft;

    public List<Player> thePlayers = new List<Player>();

    // Start is called before the first frame update
    void Start()
    {
        thePlayers.Add(new Player("Pax: ",0));
        thePlayers.Add(new Player("AI: ",1));

        p1_name.text = thePlayers[0].Username;
        p2_name.text = thePlayers[1].Username;
        p1_score.text = "0";
        p2_score.text = "0";
        p1_lastScore.text = "";
        p2_lastScore.text = "";
        tileLeft.text = PlayerBoard.instance.AllTilesNumbers.Count.ToString() + " / 105";

    }
    public void AddScore(int aPlayerId, int aScore)
    {
        thePlayers[aPlayerId].LastScore = aScore;

        thePlayers[aPlayerId].Score += aScore;

        UpdateUI();

        scoreOverview.text = aScore.ToString();
    }

    public void UpdateUI()
    {
        if (thePlayers.Count == 0)
            return;

        p1_name.text = thePlayers[0].Username;
        p2_name.text = thePlayers[1].Username;
        p1_score.text = thePlayers[0].Score.ToString();
        p2_score.text = thePlayers[1].Score.ToString();
        p1_lastScore.text = "+"+thePlayers[0].LastScore.ToString();
        p2_lastScore.text = "+"+thePlayers[1].LastScore.ToString();
        tileLeft.text = PlayerBoard.instance.AllTilesNumbers.Count.ToString() + " / 105";

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
