using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;




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

    public int CurrentTurn = -1;

    public GameObject WaitingOverlay;

    // Start is called before the first frame update
    void Start()
    {
        Board.instance.Init();
        thePlayers.Add(new GameObject("Player1").AddComponent<Player>().Init("Pax: ", 0));
        thePlayers.Add(new GameObject("Player2").AddComponent<Player>().Init("AI: ", 1,true));
        PlayerBoard.instance.Init(thePlayers[0]);

        CurrentTurn = 0;


        p1_name.text = thePlayers[0].Username;
        p2_name.text = thePlayers[1].Username;
        p1_score.text = "0";
        p2_score.text = "0";
        p1_lastScore.text = "";
        p2_lastScore.text = "";
        tileLeft.text = Board.instance.AllTilesNumbers.Count.ToString() + " / 105";

    }
    public void AddScore(Player aPlayer, int aScore)
    {
        aPlayer.LastScore = aScore;

        aPlayer.Score += aScore;

        UpdateUI();

        scoreOverview.text = aScore.ToString();
    }
    public bool CheckIfMyTurn()
    {
        if (CurrentTurn == 0)
        {

            return true;
        }
        else
        {

            AlertText.instance.ShowAlert("Not your turn!");

            return false;
        }

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
        tileLeft.text = Board.instance.AllTilesNumbers.Count.ToString() + " / 105";

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void NextTurn()
    {

        if (CurrentTurn == 0)
            CurrentTurn = 1;
        else if (CurrentTurn == 1)
            CurrentTurn = 0;

        Debug.Log("Next Turn: " + CurrentTurn);
        if (CurrentTurn == 1)
        {

            if (thePlayers[1].isAI)
                thePlayers[1].DoAI();

            WaitingOverlay.SetActive(true);
            WaitingOverlay.GetComponent<CanvasGroup>().alpha = 0;
            WaitingOverlay.GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.InOutQuart);
        }
        else
        {

            WaitingOverlay.GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetEase(Ease.InOutQuart).OnComplete(() => { WaitingOverlay.SetActive(false); });
           
        }
    }
}
