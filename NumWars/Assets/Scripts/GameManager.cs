using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        
        thePlayers.Add(new GameObject("Player1").AddComponent<Player>().Init(Startup._instance.displayName + ": ", 0, p1_score.gameObject));


            if(Startup._instance != null && Startup._instance.GameToLoad != null)
            {
                Board.instance.LoadBoardData(Startup._instance.GameToLoad);
                thePlayers.Add(new GameObject("Player2").AddComponent<Player>().Init(Startup._instance.GameToLoad.GetOtherPlayer()+": ", 1, p2_score.gameObject, false));
                CurrentTurn = Startup._instance.GameToLoad.GetPlayerTurn();

                if(Startup._instance.GameToLoad.player1_PlayfabId == Startup._instance.MyPlayfabID)
                {
                    if (Startup._instance.GameToLoad.player1_score.Length > 0)
                        thePlayers[0].Score = int.Parse(Startup._instance.GameToLoad.player1_score);
                    if (Startup._instance.GameToLoad.player2_score.Length > 0)
                        thePlayers[1].Score = int.Parse(Startup._instance.GameToLoad.player2_score);
                }
                else
                {
                    if(Startup._instance.GameToLoad.player2_score.Length>0)
                    thePlayers[0].Score = int.Parse(Startup._instance.GameToLoad.player2_score);
                    if (Startup._instance.GameToLoad.player1_score.Length > 0)
                        thePlayers[1].Score = int.Parse(Startup._instance.GameToLoad.player1_score);
                }

                

        }
        else
            {
                thePlayers.Add(new GameObject("Player2").AddComponent<Player>().Init("AI: ", 1, p2_score.gameObject, true));
                CurrentTurn = 0;
            }
        PlayerBoard.instance.Init(thePlayers[0]);




        p1_name.text = thePlayers[0].Username;
        p2_name.text = thePlayers[1].Username;
        p1_score.text = thePlayers[0].Score.ToString();
        p2_score.text = thePlayers[1].Score.ToString();
        p1_lastScore.text = "";
        p2_lastScore.text = "";
        tileLeft.text = Board.instance.AllTilesNumbers.Count.ToString();


        if(CurrentTurn == 1)
        {
            WaitingOverlay.SetActive(true);
            WaitingOverlay.GetComponent<CanvasGroup>().alpha = 0;
            WaitingOverlay.GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.InOutQuart);
        }
        else
        {
            if(thePlayers[1].isAI == false)
                ScoreScreen.instance.ShowScoreLastPlay();
        }

    }
    public void AddScore(Player aPlayer, int aScore, bool updateLast=true)
    {
        if(updateLast)
        aPlayer.LastScore = aScore;

        aPlayer.Score += aScore;

        UpdateUI();

        scoreOverview.text = aScore.ToString();
    }
    public bool CheckIfMyTurn(bool showAlert = true)
    {
        if (CurrentTurn == 0)
        {

            return true;
        }
        else
        {
            if(showAlert)
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
        tileLeft.text = Board.instance.AllTilesNumbers.Count.ToString();

    }

    private float _refreshTimer = 0;
    public bool updateInProgress = false;
    // Update is called once per frame
    void Update()
    {

        if(thePlayers[1].isAI == false)
        {
            if (CheckIfMyTurn(false) == false)
            {
                _refreshTimer += Time.deltaTime;
                if (_refreshTimer > 5)
                {
                    if (updateInProgress == false)
                    {
                        PlayfabHelperFunctions.instance.UpdateTargetGame(Startup._instance.GameToLoad.RoomName);
                        Debug.Log("Refreshing game");
                        updateInProgress = true;
                    }
                    _refreshTimer = 0;

                }
            }
        }

  


    }
    public void RefreshBackendCallback()
    {
        if (CheckIfMyTurn(false) == false)
        {
            if( Startup._instance.GameToLoad.GetPlayerTurn() == 0)
            {
                // we got a refresh that is not synced, there was a turn made so we need to "play" the turn on screen and change turn
                Board.instance.LoadBoardData(Startup._instance.GameToLoad);
                CurrentTurn = 0;
                WaitingOverlay.GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetEase(Ease.InOutQuart).OnComplete(() => { WaitingOverlay.SetActive(false); });

                ScoreScreen.instance.ShowScoreLastPlay();

            }
        }
        updateInProgress = false;
        _refreshTimer = 0;
    }
    public void NextTurn()
    {
        _refreshTimer = 0;


        if (CurrentTurn == 0)
            CurrentTurn = 1;
        else if (CurrentTurn == 1)
            CurrentTurn = 0;

        Debug.Log("Next Turn: " + CurrentTurn);
        if (CurrentTurn == 1)
        {
            if (thePlayers[1].isAI)
                thePlayers[1].DoAI();
            else
            {
                BoardData updatedBoard = new BoardData(Startup._instance.GameToLoad.player1_PlayfabId, Startup._instance.GameToLoad.player2_PlayfabId, Startup._instance.GameToLoad.GetPlayerTurn(CurrentTurn).ToString(), Board.instance.BoardTiles, Startup._instance.GameToLoad.RoomName, Startup._instance.GameToLoad.History);
                updatedBoard.player1_displayName = Startup._instance.GameToLoad.player1_displayName;
                updatedBoard.player2_displayName = Startup._instance.GameToLoad.player2_displayName;
                updatedBoard.player1_score = Startup._instance.GameToLoad.player1_score;
                updatedBoard.player2_score = Startup._instance.GameToLoad.player2_score;
                updatedBoard.player1_abandon = Startup._instance.GameToLoad.player1_abandon;
                updatedBoard.player2_abandon = Startup._instance.GameToLoad.player2_abandon;

                if (updatedBoard.GetPlayerTurn(CurrentTurn) == 1)
                    updatedBoard.player1_score = thePlayers[0].Score.ToString();
                else
                    updatedBoard.player2_score = thePlayers[0].Score.ToString();



                updateInProgress = true;
                PlayfabHelperFunctions.instance.SendNextTurn(updatedBoard);
                _refreshTimer = 0;
            }

            WaitingOverlay.SetActive(true);
            WaitingOverlay.GetComponent<CanvasGroup>().alpha = 0;
            WaitingOverlay.GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.InOutQuart);
        }
        else
        {

            WaitingOverlay.GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetEase(Ease.InOutQuart).OnComplete(() => { WaitingOverlay.SetActive(false); });
           
        }
    }

    public void ClickBack()
    {
        SceneManager.LoadScene(0);
        Startup._instance.Refresh(0.1f);
    }
}
