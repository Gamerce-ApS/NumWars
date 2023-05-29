using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameAnalyticsSDK;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
//using AppodealAds.Unity.Api;
//using AppodealAds.Unity.Common;



public class GameManager : MonoBehaviour
{
    public static GameManager _instance=null;
    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                if (GameObject.Find("GameManager") == null)
                    return null;

                _instance = GameObject.Find("GameManager").GetComponent<GameManager>();

            }
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

    public RectTransform TimerGO;
    public RectTransform SwapButtonGO;
    public RectTransform DoneButtonGO;
    public RectTransform SettingsButtonGO;

    public List<Player> thePlayers = new List<Player>();

    public int CurrentTurn = -1;

    public GameObject WaitingOverlay;
    public GameObject GameEndedOverlay;

    public int AIGAME_EMPTY_TURNS = 0;

    public GameObject ChatNotificationIcon;
    public GameObject TutorailGO;
    public GameObject TutorailGO2;


    public Text p1_level;
    public Text p1_thropies;

    public Text p2_level;
    public Text p2_thropies;


    public bool isFakeGame;

    public bool IsSendingData = false;
    public Image BackButton;
    public float aspect;

    public Text otherPlayerTurnText;

    // Start is called before the first frame update
    void Start()
    {

        _TextFlyInBoxoriginalPos = _userInfoWindow.transform.GetChild(1).transform.position;

        Startup.instance.limitFpsTimer = 10;
       // PhotonNetwork.Disconnect();
        if (isFakeGame)
        {
            PlayerPrefs.SetInt("BoardLayout", 0);
            Board.instance.Init();

            thePlayers.Add(new GameObject("Player1").AddComponent<Player>().Init("Patrik", 0, p1_score.gameObject));
            thePlayers[0].AddNewPlayerTiles();
            PlayerBoard.instance.Init(thePlayers[0]);
            thePlayers.Add(new GameObject("Player2").AddComponent<Player>().Init("AI: ", 1, p2_score.gameObject, true));

            CurrentTurn = 0;
            return;
        }



        //GameAnalytics.NewDesignEvent("GameStarted", 0);
        // TinySauce.OnGameStarted();


        if (Startup._instance.isTutorialGame)
        {
            TutorailGO.SetActive(true);
            TutorailGO2.SetActive(true);
            Startup._instance.isTutorialGame = false;
            GameAnalytics.NewDesignEvent("StartedTutorialGame");
        }
        else
        {
            TutorailGO.SetActive(false);
            TutorailGO2.SetActive(false);

        }


        aspect = (float)Screen.height / (float)Screen.width;
        Debug.Log("aspect:"+aspect);
        if (aspect<1.4f)
        SetIpadScreen();

        Board.instance.Init();


        string displayName = "";
        if(Startup._instance !=null)
            displayName = Startup._instance.displayName;

        thePlayers.Add(new GameObject("Player1").AddComponent<Player>().Init(displayName + "", 0, p1_score.gameObject));


        if(Startup._instance != null && Startup._instance.GameToLoad != null && Startup._instance.GameToLoad.BoardTiles != null)
        {
                Board.instance.LoadBoardData(Startup._instance.GameToLoad);
                thePlayers.Add(new GameObject("Player2").AddComponent<Player>().Init(Startup._instance.GameToLoad.GetOtherPlayer()+"", 1, p2_score.gameObject, false));
                CurrentTurn = Startup._instance.GameToLoad.GetPlayerTurn();

                if(Startup._instance.GameToLoad.player1_PlayfabId == Startup._instance.MyPlayfabID)
                {
                    if (Startup._instance.GameToLoad.player1_score.Length > 0)
                        thePlayers[0].Score = int.Parse(Startup._instance.GameToLoad.player1_score);
                    if (Startup._instance.GameToLoad.player2_score.Length > 0)
                        thePlayers[1].Score = int.Parse(Startup._instance.GameToLoad.player2_score);

                thePlayers[0].LoadPlayerTiles(Startup._instance.GameToLoad.p1_tiles);
                thePlayers[1].LoadPlayerTiles(Startup._instance.GameToLoad.p2_tiles);
            }
                else
                {
                    if(Startup._instance.GameToLoad.player2_score.Length>0)
                    thePlayers[0].Score = int.Parse(Startup._instance.GameToLoad.player2_score);
                    if (Startup._instance.GameToLoad.player1_score.Length > 0)
                        thePlayers[1].Score = int.Parse(Startup._instance.GameToLoad.player1_score);

                thePlayers[1].LoadPlayerTiles(Startup._instance.GameToLoad.p1_tiles);
                thePlayers[0].LoadPlayerTiles(Startup._instance.GameToLoad.p2_tiles);
            }




            if(Startup._instance.GameToLoad.GetHasTimeout() )
            {
                GameEndedOverlay.SetActive(true);
                GameEndedOverlay.GetComponent<CanvasGroup>().alpha = 0;
                GameEndedOverlay.GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.InOutQuart);
                WaitingOverlay.SetActive(false);
            }

            for(int i = 0; i < Startup.instance.myOldGameList.Count;i++)
            {
                if(Startup.instance.myOldGameList[i].RoomName == Startup._instance.GameToLoad.RoomName)
                {
                    GameEndedOverlay.SetActive(true);
                    GameEndedOverlay.GetComponent<CanvasGroup>().alpha = 0;
                    GameEndedOverlay.GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.InOutQuart);
                    WaitingOverlay.SetActive(false);
                }
            }




        


        }
        else
        {

            thePlayers.Add(new GameObject("Player2").AddComponent<Player>().Init("AI: ", 1, p2_score.gameObject, true));
            CurrentTurn = 0;

            string localAIgame = PlayerPrefs.GetString("AIGame", "");
            if (localAIgame != "" && TutorailGO.activeSelf == false)
            {
                BoardData ai_bd = new BoardData(localAIgame);

                Board.instance.LoadBoardData(ai_bd);
                

                thePlayers[0].Username = ai_bd.player1_displayName+"";
                thePlayers[0].Score = int.Parse(ai_bd.player1_score);

                thePlayers[1].Username = ai_bd.player2_displayName+"";
                thePlayers[1].Score = int.Parse(ai_bd.player2_score);

              
                thePlayers[0].LoadPlayerTiles(ai_bd.p1_tiles);
                thePlayers[1].LoadPlayerTiles(ai_bd.p2_tiles);

                



            }
            else
            {
                thePlayers[0].AddNewPlayerTiles();
                thePlayers[1].AddNewPlayerTiles();

            }

        }
        PlayerBoard.instance.Init(thePlayers[0]);

  


        p1_name.text = thePlayers[0].Username;
        p2_name.text = thePlayers[1].Username;
        p1_score.text = thePlayers[0].Score.ToString();
        p2_score.text = thePlayers[1].Score.ToString();
        p1_lastScore.text = "";
        p2_lastScore.text = "";

        p1_thropies.text = Startup._instance.myData["Ranking"].Value;
        p1_level.text = HelperFunctions.XPtoLevel( Startup._instance.myData["XP"].Value).ToString();


        tileLeft.text = Board.instance.AllTilesNumbers.Count.ToString();


        if(CurrentTurn == 1)
        {
            if(GameEndedOverlay.activeSelf == false)
            {
                WaitingOverlay.SetActive(true);
                otherPlayerTurnText.text = "Waiting for " + thePlayers[1].Username + "..";
                WaitingOverlay.GetComponent<CanvasGroup>().alpha = 0;
                WaitingOverlay.GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.InOutQuart);
            }

            
            if (int.Parse(Startup._instance.GameToLoad.EmptyTurns) >= 4)
                ScoreScreen.instance.ShowScoreLastPlay(true,Startup._instance.GameToLoad);

            if(Startup._instance.GameToLoad != null)
            {
                List<string> moveHistory = Startup._instance.GameToLoad.History;
                if (moveHistory != null)
                    if (moveHistory.Count > 0)
                        {
                            ScoreScreen.instance.SetLastScoreNotYournTurn(Startup._instance.GameToLoad);
                        }
            }
    

                        

        }
        else
        {
            if(thePlayers[1].isAI == false)
            {
                ScoreScreen.instance.ShowScoreLastPlay(true, Startup._instance.GameToLoad);
            }
            else
            {
                ScoreScreen.instance.ShowScoreLastPlay(true, Board.instance.boardData);
            }
        }



        Board.instance.LoadLastUsedTiles(PlayerBoard.instance.myPlayer.myTiles);


        _refreshTimer = 110;







        if(thePlayers[1].isAI)
        {
            if(PlayerPrefs.HasKey("AIGame") == false)
            {

                

                BoardData updatedBoard = new BoardData(Startup._instance.MyPlayfabID, "", "1", Board.instance.BoardTiles, "AI_GAME", Board.instance.History, Board.instance.GetTilesLeft(), "0", thePlayers[0].GetMyTiles(), thePlayers[1].GetMyTiles(), System.DateTimeOffset.Now.ToUnixTimeSeconds().ToString());

                updatedBoard.player1_displayName = Startup._instance.displayName;
                updatedBoard.player2_displayName = "AI";
                updatedBoard.player1_score = GameManager.instance.p1_score.text;
                updatedBoard.player2_score = GameManager.instance.p2_score.text;
                updatedBoard.player1_abandon = "0";
                updatedBoard.player2_abandon = "0";
                updatedBoard.EmptyTurns = AIGAME_EMPTY_TURNS.ToString();

                if (TutorialController.instance == null)
                    PlayerPrefs.SetString("AIGame", updatedBoard.GetJson());

                int turn = Random.Range(0, 2);
                if(turn>0)
                {
                    GameManager.instance.NextTurn(false);
                }

            }
      
        }

        bool gameEnded = false;
        if (thePlayers[1].isAI == false)
            if (Startup._instance != null && Startup._instance.GameToLoad != null && Startup._instance.GameToLoad.BoardTiles != null)
                if (int.Parse(Startup._instance.GameToLoad.EmptyTurns) >= 4)
                {
                    //Board.instance.PressContinue();
                    GameManager.instance.updateInProgress = false;
                    GameManager.instance.EndGameAfterPasses(Startup._instance.GameToLoad);
                    gameEnded = true;
                }


        if(gameEnded == false)
        {
            if (thePlayers[1].isAI == false)
                _userInfoWindow.PreLoadData();

            if (Startup._instance != null && Startup._instance.GameToLoad != null && Startup._instance.GameToLoad.BoardTiles != null)
                PlayfabHelperFunctions.instance.GetOtherUserData(Startup._instance.GameToLoad.GetOtherPlayerPlayfab());

        }




    }
    public void SetOpponentData(string xp, string rank)
    {
        p2_thropies.text = rank;
        p2_level.text = HelperFunctions.XPtoLevel(xp).ToString();
    }
    public void SetIpadScreen()
    {
        Board.instance.transform.GetComponent<RectTransform>().transform.localScale = new Vector3(0.82f, 0.82f, 0.82f);
        PlayerBoard.instance.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 105, 0);

        TimerGO.anchoredPosition = new Vector3(-15, -833, 0);
        TimerGO.transform.localScale = new Vector3(0.4612228f, 0.4612228f, 0.4612228f);





        SwapButtonGO.anchoredPosition = new Vector3(-288f, 211f, 0);
        DoneButtonGO.anchoredPosition = new Vector3(282.9f, 210.5f,0);
        SettingsButtonGO.anchoredPosition = new Vector3(11.182f, -89f, 0);


        WaitingOverlay.transform.localScale = new Vector3(0.81f, 0.81f, 0.81f);
       // GameObject.Find("Canvas").GetComponent<CanvasScaler>().referenceResolution = new Vector2(1100, 600);

    }

    public void AddScore(Player aPlayer, int aScore, int amountOfTiles , bool updateLast=true,bool isReplay = false,bool updateUI = true)
    {
        if (isFakeGame)
            return;

        if(updateLast)
        aPlayer.LastScore = aScore;

        aPlayer.Score += aScore;

        if(aPlayer.ID == 0)
        AchivmentController.instance.Scored(aScore, amountOfTiles);

        if (aPlayer.ID == 1 && aPlayer.isAI && isReplay== false)
            AchivmentController.instance.Scored(aScore, amountOfTiles, true);

        if(updateUI)
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
        if (isFakeGame)
            return;

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
    float runSetupAfterTime = 0.2f;
    public float SendingDataDelay = 0;
    public GameObject connectionissues;
    // Update is called once per frame
void Update()
    {
        if (isFakeGame)
            return;


        if(IsSendingData)
        {
            if(BackButton.gameObject.activeSelf)
            BackButton.gameObject.SetActive(false);

            SendingDataDelay += Time.deltaTime;

            if(SendingDataDelay>15)
            {
                if (!BackButton.gameObject.activeSelf)
                    BackButton.gameObject.SetActive(true);

                connectionissues.SetActive(true);
            }
        }
        else
        {
            if (!BackButton.gameObject.activeSelf)
                BackButton.gameObject.SetActive(true);

            connectionissues.SetActive(false);
            SendingDataDelay = 0;
        }

        if (runSetupAfterTime != -1)
        {
            runSetupAfterTime -= Time.deltaTime;
            if(runSetupAfterTime<0)
            {
                runSetupAfterTime = -1;
                StartGameFirstFrame();
            }

    
        }


        if (thePlayers[1].isAI == false && GameFinishedScreen.instance.isShowing == false)
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
            else
            {
                _refreshTimer += Time.deltaTime;
                if (_refreshTimer > 120)
                {
             
                    PlayfabHelperFunctions.instance.UpdateChatMessages(Startup._instance.GameToLoad.RoomName);
                    Debug.Log("Refreshing chat");
        
                    _refreshTimer = 0;

                }
            }
        }

  


    }

    private void OnGUI()
    {
       // if (Startup.DEBUG_TOOLS == false)
            return;

        GUI.skin.textField.fontSize = 24;
        GUILayout.BeginVertical(GUILayout.Height(Screen.height));
        GUILayout.FlexibleSpace();


 


        Dictionary<int, int> d = new Dictionary<int, int>();
        for (int i = 0;i < Board.instance.BoardTiles.Count;i++)
        {
            if(Board.instance.BoardTiles[i]._child != null)
            {
                if(Board.instance.BoardTiles[i].myTileType == StaticTile.TileType.NormalTile)
                {

                    if( d.ContainsKey((int)Board.instance.BoardTiles[i].GetValue()) )
                    {
                        d[(int)Board.instance.BoardTiles[i].GetValue()]++;
                    }
                    else
                    d.Add((int)Board.instance.BoardTiles[i].GetValue(), 1);
                    
                }
            }
        }
        //for(int i = 0; i < thePlayers[0].myTiles.Count;i++)
        //{
        //    int val = int.Parse(thePlayers[0].myTiles[i].GetTileNumber());
        //    if (d.ContainsKey(val))
        //    {
        //        d[val]++;
        //    }
        //    else
        //        d.Add(val, 1);
        //}
        for (int i = 0; i < Startup.instance.GameToLoad.p2_tiles.Count; i++)
        {
            int val = int.Parse(Startup.instance.GameToLoad.p2_tiles[i]);
            if (d.ContainsKey(val))
            {
                d[val]++;
            }
            else
                d.Add(val, 1);
        }
        for (int i = 0; i < Startup.instance.GameToLoad.p1_tiles.Count; i++)
        {
            int val = int.Parse(Startup.instance.GameToLoad.p1_tiles[i]);
            if (d.ContainsKey(val))
            {
                d[val]++;
            }
            else
                d.Add(val, 1);
        }
        for (int i = 0; i < Board.instance.AllTilesNumbers.Count; i++)
        {
            int val = Board.instance.AllTilesNumbers[i];
            if (d.ContainsKey(val))
            {
                d[val]++;
            }
            else
                d.Add(val, 1);
        }


  

        int tot = 0;
        foreach (KeyValuePair<int, int> v in d.OrderBy(key => key.Key))
        {
            if(v.Key<=10 && v.Value != 7)
                GUI.color = Color.red;
            else
                GUI.color = Color.white;



            if (v.Key > 10)
            {
                if( v.Value != 1)
                GUI.color = Color.red;
                else
                GUI.color = Color.white;
            } 

            tot += v.Value;
            GUILayout.TextField(v.Key+" : "+ v.Value);
  
        }

        GUILayout.TextField("Total:" + tot);
        GUILayout.EndVertical();
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

                ScoreScreen.instance.ShowScoreLastPlay(false, Startup._instance.GameToLoad);

            }
            Board.instance.boardData.LastMoveTimeStamp = Startup._instance.GameToLoad.LastMoveTimeStamp;
        }
        updateInProgress = false;
        _refreshTimer = 0;
    }
    public void NextTurn(bool isEmptyTurn = false,System.Action callback=null)
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
            {
                //Startup._instance.AddXP(5);
                GameManager.instance.SendingDataDelay = 0;
                thePlayers[1].DoAI();

                if (isEmptyTurn) // you ended your turn with an empty move, it will not be stored until AI makes his move
                {
                    Board.instance.History.Add("#EMPTY#");
                    AIGAME_EMPTY_TURNS = ((AIGAME_EMPTY_TURNS) + 1);
                    GameManager.instance.MakeLastPlayedTilesColored();
                }
                else
                    AIGAME_EMPTY_TURNS = 0;
            }
            else
            {
                
                
                List<string> p1_tiles = thePlayers[0].GetMyTiles();
                List<string> p2_tiles = Startup._instance.GameToLoad.p2_tiles;

                if(Startup._instance.GameToLoad.player2_PlayfabId == Startup._instance.MyPlayfabID)
                {
                   p1_tiles = Startup._instance.GameToLoad.p1_tiles;
                    p2_tiles = thePlayers[0].GetMyTiles();
                }

                BoardData updatedBoard = new BoardData(Startup._instance.GameToLoad.player1_PlayfabId, Startup._instance.GameToLoad.player2_PlayfabId, Startup._instance.GameToLoad.GetPlayerTurn(CurrentTurn).ToString(), Board.instance.BoardTiles, Startup._instance.GameToLoad.RoomName, Startup._instance.GameToLoad.History, Board.instance.GetTilesLeft(), Startup._instance.GameToLoad.EmptyTurns, p1_tiles, p2_tiles, System.DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                updatedBoard.player1_displayName = Startup._instance.GameToLoad.player1_displayName;
                updatedBoard.player2_displayName = Startup._instance.GameToLoad.player2_displayName;
                updatedBoard.player1_avatarURL = Startup._instance.GameToLoad.player1_avatarURL;
                updatedBoard.player2_avatarURL = Startup._instance.GameToLoad.player2_avatarURL;
                
                updatedBoard.player1_score = Startup._instance.GameToLoad.player1_score;
                updatedBoard.player2_score = Startup._instance.GameToLoad.player2_score;
                updatedBoard.player1_abandon = Startup._instance.GameToLoad.player1_abandon;
                updatedBoard.player2_abandon = Startup._instance.GameToLoad.player2_abandon;

                if (isEmptyTurn)
                {
                    updatedBoard.EmptyTurns = (int.Parse(updatedBoard.EmptyTurns) + 1).ToString();
                    updatedBoard.History.Add("#EMPTY#");
                }
                else
                {
                    updatedBoard.EmptyTurns = "0";

                }

                if (updatedBoard.GetPlayerTurn(CurrentTurn) == 1)
                    updatedBoard.player1_score = thePlayers[0].Score.ToString();
                else
                    updatedBoard.player2_score = thePlayers[0].Score.ToString();

                //if (Board.instance.GetTilesLeft().Count<=0)
                //{
                //    updatedBoard.EmptyTurns = "4";
                //} 
                if (thePlayers[0].GetMyTiles().Count == 0 && isEmptyTurn)
                {
                    updatedBoard.EmptyTurns = "4";
                }


                updateInProgress = true;
                if( updatedBoard.CheckBoard() == true)
                {
                    PlayfabHelperFunctions.instance.SendNextTurn(updatedBoard, callback);
                }
                else
                {
                    SceneManager.LoadScene(0);
                    if (LoadingOverlay.instance != null)
                        LoadingOverlay.instance.ShowLoadingFullscreen("Updating..");
                    Startup._instance.Refresh(0.1f);

                    return;
                }

                _refreshTimer = 0;


                //if (int.Parse(updatedBoard.EmptyTurns) >= 4)
                //{
                //    updateInProgress = false;
                //    EndGameAfterPasses(updatedBoard);
                //}
                //else
                //{
                //    WaitingOverlay.SetActive(true);
                //    WaitingOverlay.GetComponent<CanvasGroup>().alpha = 0;
                //    WaitingOverlay.GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.InOutQuart);
                //    //if (TutorialController.instance == null && Random.Range(0,100) <50)
                //    //    Appodeal.show(Appodeal.INTERSTITIAL);
                //}
            }

            if (thePlayers[1].isAI)
            {
                WaitingOverlay.SetActive(true);
                otherPlayerTurnText.text = "Waiting for " + thePlayers[1].Username + "..";
                WaitingOverlay.GetComponent<CanvasGroup>().alpha = 0;
                WaitingOverlay.GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.InOutQuart);
                //if (TutorialController.instance == null && Random.Range(0, 100) < 50)
                //    Appodeal.show(Appodeal.INTERSTITIAL);
            }
                

        }
        else
        {
            if(TutorialController.instance != null)
            if (TutorialController.instance.myActions[TutorialController.instance.CurrentIndex].ID == 9)
            {
                TutorialController.instance.TapToContinue();
            }



            //Use this for testing win scenario
            // AIGAME_EMPTY_TURNS = ((AIGAME_EMPTY_TURNS) + 1);
            if (isEmptyTurn)
            {
                AIGAME_EMPTY_TURNS = ((AIGAME_EMPTY_TURNS) + 1);

                Board.instance.History.Add("#EMPTY#");
            }
            else
            {
                AIGAME_EMPTY_TURNS = 0;

            }





            BoardData updatedBoard = new BoardData(Startup._instance.MyPlayfabID, "", "1", Board.instance.BoardTiles, "AI_GAME", Board.instance.History, Board.instance.GetTilesLeft(), "0",thePlayers[0].GetMyTiles(), thePlayers[1].GetMyTiles(), System.DateTimeOffset.Now.ToUnixTimeSeconds().ToString());

            updatedBoard.player1_displayName = Startup._instance.displayName;
            updatedBoard.player2_displayName = "AI";
            updatedBoard.player1_score = GameManager.instance.p1_score.text;
            updatedBoard.player2_score = GameManager.instance.p2_score.text;
            updatedBoard.player1_abandon = "0";
            updatedBoard.player2_abandon = "0";
            updatedBoard.EmptyTurns = AIGAME_EMPTY_TURNS.ToString();


            //if (Board.instance.GetTilesLeft().Count <= 0)
            //{
            //    updatedBoard.EmptyTurns = "4";
            //}
            if (thePlayers[1].GetMyTiles().Count == 0)
            {
                updatedBoard.EmptyTurns = "4";
                AIGAME_EMPTY_TURNS = 4;
            }
            if (thePlayers[0].GetMyTiles().Count == 0)
            {
                updatedBoard.EmptyTurns = "4";
                AIGAME_EMPTY_TURNS = 4;
            }

            bool shouldAddToAI = true;
            if (TutorialController.instance != null)
                shouldAddToAI = false;

            if (shouldAddToAI)
            {
                if (TutorialController.instance == null)
                    PlayerPrefs.SetString("AIGame", updatedBoard.GetJson());
                Board.instance.boardData = updatedBoard;
            }

            WaitingOverlay.GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetEase(Ease.InOutQuart).OnComplete(() => { WaitingOverlay.SetActive(false); });

        


            if (int.Parse(updatedBoard.EmptyTurns) >= 4 && AIGAME_EMPTY_TURNS >= 4)
            {
                EndGameAfterPasses(updatedBoard);
            }
            GameManager.instance.IsSendingData = false;
            GameManager.instance.SendingDataDelay = 0;
        }
    }
    public void HideThinkingOverlay()
    {
        WaitingOverlay.GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetEase(Ease.InOutQuart).OnComplete(() => { WaitingOverlay.SetActive(false); });

    }
    public UserInfoWindow _userInfoWindow;
    Vector3 _TextFlyInBoxoriginalPos;
    public void ClickProfile(int aUserId)
    {
        _userInfoWindow.gameObject.SetActive(true);

        _userInfoWindow.InitUser(aUserId);


        
        Startup._instance.PlaySoundEffect(0);
        _userInfoWindow.transform.GetChild(0).GetComponent<Image>().DOFade(157f / 255f, 0).SetEase(Ease.InOutQuart);
        _userInfoWindow.transform.GetChild(1).transform.position += new Vector3(10, 0, 0);
        _userInfoWindow.transform.GetChild(1).transform.DOMoveX(_TextFlyInBoxoriginalPos.x, 0.3f).SetEase(Ease.InOutQuart);



    }
    public void MakeLastPlayedTilesColored()
    {
        Color col;
        ColorUtility.TryParseHtmlString("#FFFE67", out col);

        List<string> moveHistory = Board.instance.History;
        List<FakeTileData> lastMoves = new List<FakeTileData>();

        int myBackednTurn = GameManager.instance.CurrentTurn;

        if(Startup._instance != null && Startup._instance.GameToLoad != null && Startup._instance.GameToLoad.BoardTiles != null)
        {
             myBackednTurn = Startup._instance.GameToLoad.GetPlayerTurn(GameManager.instance.CurrentTurn);
             moveHistory = Startup._instance.GameToLoad.History;

        }
        if(moveHistory != null)
        for (int i = moveHistory.Count - 1; i >= 0; i--)
        {
            string[] moveInfo = moveHistory[i].Split('#');

            if( !moveHistory[i].Contains( "#SWAP#") && moveHistory[i] != "#EMPTY#" && !moveHistory[i].Contains("#TILESONHAND"))
            {
                Vector2 v2 = ScoreScreen.StringToVector2(moveInfo[0]);
                Board.instance.SetTileColor((int)(v2.x), (int)(v2.y), Color.white);
            }



        }
        if (moveHistory != null)
            for (int i = moveHistory.Count - 1; i >= 0; i--)
        {
            if( moveHistory[i].Contains( "#SWAP#") || moveHistory[i] == "#EMPTY#"  || moveHistory[i].Contains("#TILESONHAND"))
            {
                break;
            }
            string[] moveInfo = moveHistory[i].Split('#');
            FakeTileData ftd = new FakeTileData();
            ftd.Position = ScoreScreen.StringToVector2(moveInfo[0]);
            ftd.Number = int.Parse(moveInfo[1]);
            ftd.ScoreValue = int.Parse(moveInfo[2]);
            ftd.Player = int.Parse(moveInfo[3]);
            if (moveInfo.Length > 4)
                ftd.SpecialTypeTile = int.Parse(moveInfo[4]);

                if (myBackednTurn != ftd.Player)
            {
                lastMoves.Add(ftd);
            }
            else
            {
                break;
            }
        }


        for(int i = 0; i < lastMoves.Count;i++)
        {
                Board.instance.SetTileColor((int)(lastMoves[i].Position.x), (int)(lastMoves[i].Position.y), col);


        }

        

    }
    public void EndGameAfterPasses(BoardData bd)
    {
        GameFinishedScreen.instance.Show(bd);
    }
    public void ClickBack()
    {
       // TinySauce.OnGameFinished(0);

        if (thePlayers[1].isAI)
        {
            if( CurrentTurn == 1 )
            {
                AlertText.instance.ShowAlert("Wait until end of AI turn!");

                return;
            }
        }



        SceneManager.LoadScene(0);
        Startup._instance.Refresh(0.1f);
        Startup._instance.LoadGameList(0.1f);
        if (Startup._instance.avatarURL != null && Startup._instance.avatarURL.Length > 0)
        {
            PlayfabHelperFunctions.instance.LoadAvatarURL(Startup._instance.avatarURL);
        }
    }
    public void StartGameFirstFrame()
    {
        MakeLastPlayedTilesColored();
    }
}
