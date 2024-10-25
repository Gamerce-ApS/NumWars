﻿using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static StaticTile;

[Serializable]
public  class BoardData
{
    public BoardData(string p1, string p2, string turn, List<StaticTile> tileData,string aRoomName,List<string> aHistory, List<string> tilesLeft,string aEmptyTurns, List<string> ap1_tiles, List<string> ap2_tiles, string aLastMoveTimeStamp)
    {
        player1_PlayfabId = p1;
        player2_PlayfabId = p2;
        playerTurn = turn;
        RoomName = aRoomName;
        EmptyTurns = aEmptyTurns;
        LastMoveTimeStamp = aLastMoveTimeStamp;
        for (int i = 0; i < tileData.Count;i++)
        {
            BoardTiles.Add(tileData[i].GetJsonObject());
        }
        for (int i = 0; i < tilesLeft.Count; i++)
        {
            TilesLeft.Add(tilesLeft[i]);
        }
        for (int i = 0; i < aHistory.Count; i++)
        {
            History.Add(aHistory[i]);
        }
        for (int i = 0; i < ap1_tiles.Count; i++)
        {
            p1_tiles.Add(ap1_tiles[i]);
        }
        for (int i = 0; i < ap2_tiles.Count; i++)
        {
            p2_tiles.Add(ap2_tiles[i]);
        }
    } 

    public string player1_PlayfabId = "";
    public string player2_PlayfabId = "";
    public string playerTurn = "";
    public List<string> BoardTiles = new List<string>();
    public List<string> History = new List<string>();
    public List<string> p1_tiles = new List<string>();
    public List<string> p2_tiles = new List<string>();
    public string RoomName = "";
    public string player1_score = "";
    public string player2_score = "";
    public string player1_displayName = "";
    public string player2_displayName = "";
    public string player1_abandon = "";
    public string player2_abandon = "";
    public string player1_avatarURL = "";
    public string player2_avatarURL = "";
    

    public string EmptyTurns = "";
    public List<string> TilesLeft = new List<string>();

    public string LastMoveTimeStamp = "";

    [NonSerialized]
    public bool hasFinished = false;

    public string GetJson()
    {
        return JsonUtility.ToJson(this);
    }
    public BoardData(string aJson)
    {
        BoardData bd = JsonUtility.FromJson<BoardData>(aJson);

        player1_PlayfabId = bd.player1_PlayfabId;
        player2_PlayfabId = bd.player2_PlayfabId;
        playerTurn = bd.playerTurn;
        BoardTiles = bd.BoardTiles;
        History = bd.History;
        RoomName = bd.RoomName;
        player1_displayName = bd.player1_displayName;
        player2_displayName = bd.player2_displayName;
        player1_score = bd.player1_score;
        player2_score = bd.player2_score;
        player1_abandon = bd.player1_abandon;
        player2_abandon = bd.player2_abandon;
        player1_avatarURL = bd.player1_avatarURL;
        player2_avatarURL = bd.player2_avatarURL;
        EmptyTurns = bd.EmptyTurns;
        TilesLeft = bd.TilesLeft;
        p1_tiles = bd.p1_tiles;
        p2_tiles = bd.p2_tiles;
        LastMoveTimeStamp = bd.LastMoveTimeStamp;
    }

    public bool GetHasTimeout()
    {
        long timeToDeadline = 0;
        if (LastMoveTimeStamp != null)
        {
            long a = System.DateTimeOffset.Now.ToUnixTimeSeconds();

            long b = long.Parse(LastMoveTimeStamp);
            //  long Future = b + 60 * 60 * 24 * 2;
            long Future = b + Startup.TIMEOUT;
            timeToDeadline = Future - a;

            if (timeToDeadline < 0)
                return true;
            else
                return false;
        }

        return false;


    }
    public string GetOtherPlayer()
    {
        if (Startup._instance.MyPlayfabID == player1_PlayfabId)
            return player2_displayName;
        else
            return player1_displayName;

    }

    public string GetWinner()
    {
        int score1 = 0;
        int score2 = 0;
        if(player1_score.Length>0)
            score1 = int.Parse(player1_score);
        if (player2_score.Length > 0)
            score2 = int.Parse(player2_score);

        if(player2_displayName != "AI")
        {
            if (GetHasAbboned() == player1_PlayfabId)
                return player2_PlayfabId;
            else if (GetHasAbboned() == player2_PlayfabId)
                return player1_PlayfabId;
        }


        if (score1 > score2)
            return player1_PlayfabId;
        else if (score2 > score1)
            return player2_PlayfabId;
        else
            return "";
    }
    public string GetHasAbboned()
    {
        if (player1_abandon == "1")
            return player1_PlayfabId;
        if (player2_abandon == "1")
            return player2_PlayfabId;

        return "";
    }
    public bool WasTimout()
    {
        int score1 = 0;
        int score2 = 0;
        if (player1_score.Length > 0)
            score1 = int.Parse(player1_score);
        if (player2_score.Length > 0)
            score2 = int.Parse(player2_score);

        if(score1 == 0 || score2 == 0)
        {
            return true;
        }

        return false;
    }
    public string GetOtherPlayerPlayfab()
    {
        if (Startup._instance.MyPlayfabID == player1_PlayfabId)
            return player2_PlayfabId;
        else
            return player1_PlayfabId;

    }
    public int GetPlayerTurn()
    {
        // we need to check who owns the game, as "turn 0" is always the host
        // So if the other players owns the game we need to flip the turn order as localy the current users is seen as player 1.
        if (Startup._instance.MyPlayfabID == player1_PlayfabId)
        {
            return int.Parse(playerTurn);
        }
        else
        {
            if (playerTurn == "0")
                return 1;
            else
                return 0;
        }
    }
    public int GetPlayerTurn(int aLocalTurn)
    {
     // This is used to convert the "local turn" to the backend turn when sending the new board data to the backend
        if (Startup._instance.MyPlayfabID == player1_PlayfabId)
        {
            return (aLocalTurn);
        }
        else
        {
            if (aLocalTurn == 0)
                return 1;
            else
                return 0;
        }
    }
    public void SetPlayerAbandome()
    {
        if(player1_PlayfabId == Startup._instance.MyPlayfabID)
        {
            player1_abandon = "1";
        }else
        {
            player1_abandon = "1";
        }
    }


    public bool CheckBoard()
    {
        //return true;
        Dictionary<int, int> d = new Dictionary<int, int>();
        for (int i = 0; i < BoardTiles.Count; i++)
        {
            StaticTile tl = new StaticTile();
            tl.LoadFromJson(BoardTiles[i]);
            {
                if (tl.myTileType == StaticTile.TileType.NormalTile)
                {
                    if (d.ContainsKey((int)tl.GetValue()))
                    {
                        d[(int)tl.GetValue()]++;
                    }
                    else
                        d.Add((int)tl.GetValue(), 1);

                }
            }
        }

        for (int i = 0; i < p2_tiles.Count; i++)
        {
            int val = int.Parse(p2_tiles[i]);
            if (d.ContainsKey(val))
            {
                d[val]++;
            }
            else
                d.Add(val, 1);
        }
        for (int i = 0; i < p1_tiles.Count; i++)
        {
            int val = int.Parse(p1_tiles[i]);
            if (d.ContainsKey(val))
            {
                d[val]++;
            }
            else
                d.Add(val, 1);
        }
        for (int i = 0; i < TilesLeft.Count; i++)
        {
            int val = int.Parse(TilesLeft[i]);
            if (d.ContainsKey(val))
            {
                d[val]++;
            }
            else
                d.Add(val, 1);
        }

        int tot = 0;
        bool hasFailed = false;
        foreach (KeyValuePair<int, int> v in d)
        {
            if (v.Key <= 10 && v.Value != 7)
                hasFailed = true;

            if (v.Key > 10)
            {
                if (v.Value != 1)
                    hasFailed = true;
            }

            tot += v.Value;

        }

        if (hasFailed)
            return false;










        return true;
    }


};



public class Board : MonoBehaviour
{

    public Transform parent;

    public List<StaticTile> BoardTiles = new List<StaticTile>();
    public List<string> History = new List<string>();
    public string LastMoveTimeStamp = "";
    public static Board _instance=null;
    public static Board instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = GameManager.FindObjectOfType<Board>();
            }
            return _instance;
        }
    }

    public GameObject Selection;
    public GameObject ScoreEffect;

    public List<int> AllTilesNumbers = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        // if(Startup._instance.isFake)
        //{
        //    ShowExtraTiles(false);
        //    ShowExtraTiles2(false);
        //}
    }
    public void Init()
    {
        // Add tiles to pool
        //for (int i = 1; i < 11; i++)
        //{
        //    for (int j = 0; j < 7; j++)
        //        AllTilesNumbers.Add(i);
        //}
        //for (int i = 11; i < 22; i++)
        //{
        //    AllTilesNumbers.Add(i);
        //}
        //AllTilesNumbers.Add(24); AllTilesNumbers.Add(25); AllTilesNumbers.Add(27); AllTilesNumbers.Add(28); AllTilesNumbers.Add(30); AllTilesNumbers.Add(32); AllTilesNumbers.Add(35); AllTilesNumbers.Add(36);
        //AllTilesNumbers.Add(40); AllTilesNumbers.Add(42); AllTilesNumbers.Add(45); AllTilesNumbers.Add(48); AllTilesNumbers.Add(49); AllTilesNumbers.Add(50); AllTilesNumbers.Add(54); AllTilesNumbers.Add(56); AllTilesNumbers.Add(60);
        //AllTilesNumbers.Add(63); AllTilesNumbers.Add(64); AllTilesNumbers.Add(70); AllTilesNumbers.Add(72); AllTilesNumbers.Add(80); AllTilesNumbers.Add(81); AllTilesNumbers.Add(90);

        //AllTilesNumbers.Shuffle();



        //for (int i = 1; i < 15; i++)
        //{
        //     AllTilesNumbers.Add(i);
        //}
        //AllTilesNumbers.Shuffle();

        int amount = -1;
        if (Startup._instance != null && Startup._instance.StaticServerData != null)
            amount = int.Parse(Startup._instance.StaticServerData["TilesAmount"]);





        // Create board layout

        for (int i = 0; i < parent.childCount; i++)
        {
            StaticTile st = parent.GetChild(i).GetComponent<StaticTile>();
            st.BoardPosition = new Vector2(i - ((int)i / 14) * 14, (int)i / 14);
            BoardTiles.Add(st);
        }

        GenerateStartBoard(amount, PlayerPrefs.GetInt("BoardLayout", 0).ToString());




        //List<int> startNumbers = new List<int>();
        //startNumbers.Add(1);
        //startNumbers.Add(2);
        //startNumbers.Add(3);
        //startNumbers.Add(4);
        //startNumbers.Shuffle();

        if (TutorialController.instance != null && TutorialController.instance.IsTutorial)
        {
            SetTile(6, 6, TileType.StartTile, 3);
            SetTile(7, 6, TileType.StartTile, 4);
            SetTile(6, 7, TileType.StartTile, 1);
            SetTile(7, 7, TileType.StartTile, 2);
        }
        else
        {
       
                SetTile(6, 6, TileType.StartTile, 1);
                SetTile(7, 6, TileType.StartTile, 2);
                SetTile(6, 7, TileType.StartTile, 3);
                SetTile(7, 7, TileType.StartTile, 4);
       
        }



    







     //   PlayerPrefs.SetString("PlayerBoard", new BoardData("id1","id2","0",BoardTiles).GetJson());

    }
    void SetStandardLayout()
    {
        SetTile(5, 0, TileType.MultiplierX3, 0);
        SetTile(8, 0, TileType.MultiplierX3, 0);
        SetTile(6, 1, TileType.MultiplierX2, 0);
        SetTile(7, 1, TileType.MultiplierX2, 0);

        SetTile(5, 13, TileType.MultiplierX3, 0);
        SetTile(8, 13, TileType.MultiplierX3, 0);
        SetTile(6, 12, TileType.MultiplierX2, 0);
        SetTile(7, 12, TileType.MultiplierX2, 0);

        SetTile(13, 5, TileType.MultiplierX3, 0);
        SetTile(13, 8, TileType.MultiplierX3, 0);
        SetTile(12, 6, TileType.MultiplierX2, 0);
        SetTile(12, 7, TileType.MultiplierX2, 0);

        SetTile(0, 5, TileType.MultiplierX3, 0);
        SetTile(0, 8, TileType.MultiplierX3, 0);
        SetTile(1, 6, TileType.MultiplierX2, 0);
        SetTile(1, 7, TileType.MultiplierX2, 0);

        SetTile(0, 0, TileType.MultiplierX4, 0);
        SetTile(2, 2, TileType.MultiplierX4, 0);
        SetTile(13, 0, TileType.MultiplierX4, 0);
        SetTile(11, 2, TileType.MultiplierX4, 0);

        SetTile(0, 13, TileType.MultiplierX4, 0);
        SetTile(2, 11, TileType.MultiplierX4, 0);
        SetTile(13, 13, TileType.MultiplierX4, 0);
        SetTile(11, 11, TileType.MultiplierX4, 0);

        SetTile(1, 1, TileType.MultiplierX2, 0);
        SetTile(3, 3, TileType.MultiplierX2, 0);
        SetTile(12, 1, TileType.MultiplierX2, 0);
        SetTile(10, 3, TileType.MultiplierX2, 0);
        SetTile(1, 12, TileType.MultiplierX2, 0);
        SetTile(3, 10, TileType.MultiplierX2, 0);
        SetTile(12, 12, TileType.MultiplierX2, 0);
        SetTile(10, 10, TileType.MultiplierX2, 0);

        // -
        SetTile(5, 2, TileType.SubtractionTile, 0);
        SetTile(8, 2, TileType.SubtractionTile, 0);
        SetTile(8, 11, TileType.SubtractionTile, 0);
        SetTile(5, 11, TileType.SubtractionTile, 0);

        // /
        SetTile(9, 1, TileType.DivisionTile, 0);
        SetTile(4, 1, TileType.DivisionTile, 0);
        SetTile(9, 12, TileType.DivisionTile, 0);
        SetTile(4, 12, TileType.DivisionTile, 0);

        SetTile(1, 4, TileType.DivisionTile, 0);
        SetTile(1, 9, TileType.DivisionTile, 0);
        SetTile(2, 5, TileType.SubtractionTile, 0);
        SetTile(2, 8, TileType.SubtractionTile, 0);

        SetTile(12, 4, TileType.DivisionTile, 0);
        SetTile(12, 9, TileType.DivisionTile, 0);
        SetTile(11, 5, TileType.SubtractionTile, 0);
        SetTile(11, 8, TileType.SubtractionTile, 0);


        SetTile(6, 9, TileType.MultiplicationTile, 0);
        SetTile(7, 9, TileType.AdditionTile, 0);
        SetTile(6, 4, TileType.AdditionTile, 0);
        SetTile(7, 4, TileType.MultiplicationTile, 0);

        SetTile(4, 6, TileType.MultiplicationTile, 0);
        SetTile(4, 7, TileType.AdditionTile, 0);
        SetTile(9, 6, TileType.AdditionTile, 0);
        SetTile(9, 7, TileType.MultiplicationTile, 0);

        SetTile(5, 5, TileType.MultiplierX2, 0);
        SetTile(8, 5, TileType.MultiplierX2, 0);
        SetTile(5, 8, TileType.MultiplierX2, 0);
        SetTile(8, 8, TileType.MultiplierX2, 0);
    }
    void SetRandomLayout()
    {
        List<TileType> allTypes = new List<TileType>();
        allTypes.Add(TileType.MultiplierX3);
        allTypes.Add(TileType.MultiplierX3);
        allTypes.Add(TileType.MultiplierX2);
        allTypes.Add(TileType.MultiplierX2);
        allTypes.Add(TileType.MultiplierX3);
        allTypes.Add(TileType.MultiplierX3);
        allTypes.Add(TileType.MultiplierX2);
        allTypes.Add(TileType.MultiplierX2);
        allTypes.Add(TileType.MultiplierX3);
        allTypes.Add(TileType.MultiplierX3);
        allTypes.Add(TileType.MultiplierX2);
        allTypes.Add(TileType.MultiplierX2);
        allTypes.Add(TileType.MultiplierX3);
        allTypes.Add(TileType.MultiplierX3);
        allTypes.Add(TileType.MultiplierX2);
        allTypes.Add(TileType.MultiplierX2);
        allTypes.Add(TileType.MultiplierX4);
        allTypes.Add(TileType.MultiplierX4);
        allTypes.Add(TileType.MultiplierX4);
        allTypes.Add(TileType.MultiplierX4);
        allTypes.Add(TileType.MultiplierX4);
        allTypes.Add(TileType.MultiplierX4);
        allTypes.Add(TileType.MultiplierX4);
        allTypes.Add(TileType.MultiplierX4);
        allTypes.Add(TileType.MultiplierX2);
        allTypes.Add(TileType.MultiplierX2);
        allTypes.Add(TileType.MultiplierX2);
        allTypes.Add(TileType.MultiplierX2);
        allTypes.Add(TileType.MultiplierX2);
        allTypes.Add(TileType.MultiplierX2);
        allTypes.Add(TileType.MultiplierX2);
        allTypes.Add(TileType.MultiplierX2);
        allTypes.Add(TileType.SubtractionTile);
        allTypes.Add(TileType.SubtractionTile);
        allTypes.Add(TileType.SubtractionTile);
        allTypes.Add(TileType.SubtractionTile);
        allTypes.Add(TileType.DivisionTile);
        allTypes.Add(TileType.DivisionTile);
        allTypes.Add(TileType.DivisionTile);
        allTypes.Add(TileType.DivisionTile);
        allTypes.Add(TileType.DivisionTile);
        allTypes.Add(TileType.DivisionTile);
        allTypes.Add(TileType.SubtractionTile);
        allTypes.Add(TileType.SubtractionTile);
        allTypes.Add(TileType.DivisionTile);
        allTypes.Add(TileType.DivisionTile);
        allTypes.Add(TileType.SubtractionTile);
        allTypes.Add(TileType.SubtractionTile);
        allTypes.Add(TileType.MultiplicationTile);
        allTypes.Add(TileType.AdditionTile);
        allTypes.Add(TileType.AdditionTile);
        allTypes.Add(TileType.MultiplicationTile);
        allTypes.Add(TileType.MultiplicationTile);
        allTypes.Add(TileType.AdditionTile);
        allTypes.Add(TileType.AdditionTile);
        allTypes.Add(TileType.MultiplicationTile);
        allTypes.Add(TileType.MultiplierX2);
        allTypes.Add(TileType.MultiplierX2);
        allTypes.Add(TileType.MultiplierX2);
        allTypes.Add(TileType.MultiplierX2);


        List<Vector2> posList = new List<Vector2>();
        for (int i = 0; i < 14; i++)
        {
            for (int j = 0; j < 14; j++)
            {
                posList.Add(new Vector2(i, j));
            }
        }
        posList.Shuffle();

        for (int i = 0; i < allTypes.Count;i++)
        {
            if(posList[i].x != 6 && posList[i].x != 6 &&
                posList[i].x != 6 && posList[i].x != 7 &&
                posList[i].x != 7 && posList[i].x != 6 &&
                posList[i].x != 7 && posList[i].x != 7)
            SetTile((int)posList[i].x, (int)posList[i].y, allTypes[i], 0); 
        }


    }
    // Temp stuff for generating new game
    public List<string> p1_tiles = new List<string>();
    public List<string> p2_tiles = new List<string>();
    public void GenerateStartBoard(int amount, string layout)
    {
        AllTilesNumbers.Clear();
        p1_tiles.Clear();
        p2_tiles.Clear();

        if(SceneManager.GetActiveScene().name != "MenuScene")
        {
            for (int i = 0; i < BoardTiles.Count; i++)
            {
                BoardTiles[i].SetTile(TileType.EmptyTile, 0);
            }
            SetTile(6, 6, TileType.StartTile, 1);
            SetTile(7, 6, TileType.StartTile, 2);
            SetTile(6, 7, TileType.StartTile, 3);
            SetTile(7, 7, TileType.StartTile, 4);
        }


        for (int i = 1; i < 11; i++)
        {
            for (int j = 0; j < 7; j++)
                AllTilesNumbers.Add(i);
        }
        for (int i = 11; i < 22; i++)
        {
            AllTilesNumbers.Add(i);
        }
        AllTilesNumbers.Add(24); AllTilesNumbers.Add(25); AllTilesNumbers.Add(27); AllTilesNumbers.Add(28); AllTilesNumbers.Add(30); AllTilesNumbers.Add(32); AllTilesNumbers.Add(35); AllTilesNumbers.Add(36);
        AllTilesNumbers.Add(40); AllTilesNumbers.Add(42); AllTilesNumbers.Add(45); AllTilesNumbers.Add(48); AllTilesNumbers.Add(49); AllTilesNumbers.Add(50); AllTilesNumbers.Add(54); AllTilesNumbers.Add(56); AllTilesNumbers.Add(60);
        AllTilesNumbers.Add(63); AllTilesNumbers.Add(64); AllTilesNumbers.Add(70); AllTilesNumbers.Add(72); AllTilesNumbers.Add(80); AllTilesNumbers.Add(81); AllTilesNumbers.Add(90);



        


        AllTilesNumbers.Shuffle();

        if (amount != -1)
        {
            AllTilesNumbers.RemoveRange(amount, AllTilesNumbers.Count - amount);

        }

        for (int i = 0; i < 6; i++)
        {
            p1_tiles.Add(AllTilesNumbers[0].ToString());
            AllTilesNumbers.RemoveAt(0);
            p2_tiles.Add(AllTilesNumbers[0].ToString());
            AllTilesNumbers.RemoveAt(0);
        }

        if(TutorialController.instance != null && TutorialController.instance.IsTutorial)
        {
            AllTilesNumbers.Clear();
            AllTilesNumbers.Add(3);
            AllTilesNumbers.Add(2);
            AllTilesNumbers.Add(6);
            AllTilesNumbers.Add(12);
            AllTilesNumbers.Add(36);

            AllTilesNumbers.Add(36);
            AllTilesNumbers.Add(36);
            AllTilesNumbers.Add(36);
            AllTilesNumbers.Add(36);
            AllTilesNumbers.Add(36);
            AllTilesNumbers.Add(36);
            AllTilesNumbers.Add(36);
            AllTilesNumbers.Add(36);
            AllTilesNumbers.Add(36);
            AllTilesNumbers.Add(36);
            AllTilesNumbers.Add(36);
            AllTilesNumbers.Add(36);
            AllTilesNumbers.Add(36);
            AllTilesNumbers.Add(36);
            AllTilesNumbers.Add(36);
            AllTilesNumbers.Add(36);

        }


        //AllTilesNumbers.RemoveRange(int.Parse(Startup._instance.StaticServerData["TilesAmount"]), AllTilesNumbers.Count- int.Parse(Startup._instance.StaticServerData["TilesAmount"]));


  if(GetSpecialTiles() <=5 && BoardTiles.Count>5)
        {
            if (layout == "0" || (TutorialController.instance != null && TutorialController.instance.IsTutorial))
            {
                SetStandardLayout();
            }
            else
            {
                SetRandomLayout();
            }
        }
      

    }


    public List<String> GetTilesLeft()
    {
        List<String> allT = new List<string>();
        for (int i = 0; i < AllTilesNumbers.Count; i++)
        {
            allT.Add(AllTilesNumbers[i].ToString());
        }
        return allT;

    }
    public BoardData boardData;
    public void LoadBoardData(BoardData aBD)
    {
        boardData = aBD;
        for (int i = 0; i < aBD.BoardTiles.Count; i++)
        {
            BoardTiles[i].LoadFromJson(aBD.BoardTiles[i]);
            BoardTiles[i].Refresh();
        }

        History = aBD.History;
        LastMoveTimeStamp = aBD.LastMoveTimeStamp;
        AllTilesNumbers.Clear();
        for (int i = 0; i < aBD.TilesLeft.Count; i++)
        {
            AllTilesNumbers.Add(int.Parse( aBD.TilesLeft[i] ));
        }

      


    }

    public List<FakeTileData> GetLastMoves()
    {

        List<string> moveHistory = History;
        List<FakeTileData> lastMoves = new List<FakeTileData>();

        int myBackednTurn = Board.instance.boardData.GetPlayerTurn(GameManager.instance.CurrentTurn);

        int LastMoveIndex = 0;
        for (int i = moveHistory.Count - 1; i >= 0; i--)
        {
            if (moveHistory[i].Contains("#SWAP#") || moveHistory[i] == "#EMPTY#" || moveHistory[i].Contains("#TILESONHAND"))
            {
                break;
            }
            string[] moveInfo = moveHistory[i].Split('#');
            FakeTileData ftd = new FakeTileData();
            ftd.Position = ScoreScreen.StringToVector2(moveInfo[0]);
            ftd.Number = int.Parse(moveInfo[1]);
            ftd.ScoreValue = int.Parse(moveInfo[2]);
            ftd.Player = int.Parse(moveInfo[3]);
            ftd.SpecialTypeTile = 0;
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
            LastMoveIndex = i;
        }


        if (GameManager.instance.CurrentTurn == 1)
            return lastMoves;



        lastMoves.Clear();
        if (moveHistory != null)
            if (moveHistory.Count > 0)
                if (moveHistory[moveHistory.Count - 1].Contains("#SWAP#"))
                {

                    LastMoveIndex = moveHistory.Count - 0;

                }
        if (moveHistory != null)
            if (moveHistory.Count > 0)
                if (moveHistory[moveHistory.Count - 1] == "#EMPTY#")
                {

                    LastMoveIndex = moveHistory.Count - 1;

                }

        for (int i = LastMoveIndex - 2; i >= 0; i--)
        {
            if (moveHistory[i].Contains("#SWAP#") || moveHistory[i] == "#EMPTY#" || moveHistory[i].Contains("#TILESONHAND"))
            {
                break;
            }
            string[] moveInfo = moveHistory[i].Split('#');
            FakeTileData ftd = new FakeTileData();
            ftd.Position = ScoreScreen.StringToVector2(moveInfo[0]);
            ftd.Number = int.Parse(moveInfo[1]);
            ftd.ScoreValue = int.Parse(moveInfo[2]);
            ftd.Player = int.Parse(moveInfo[3]);
            ftd.SpecialTypeTile = 0;
            if (moveInfo.Length > 4)
                ftd.SpecialTypeTile = int.Parse(moveInfo[4]);



            if (myBackednTurn == ftd.Player)
            {
                lastMoves.Add(ftd);
            }
            else
            {
                break;
            }
        }

        return lastMoves;



    }



    public void LoadLastUsedTiles(List<Tile> myTiles)
    {

        // Last placed tiles
        // Tiles on hand before
        // Tiles on hand after




        List<FakeTileData>  lastMoves = GetLastMoves();
        // List<string> tiles = PlayerBoard.instance.myPlayer.GetMyTiles();


        //for (int i = 0; i < lastMoves.Count; i++)
        //{
        //    for (int j = 0; j < myTiles.Count; j++)
        //    {


        //    }
        //}



        //if (lastMoves[i].Number == int.Parse(myTiles[j].textLabel.text))
        //{

        //}

        

      


     //   return;


        /////// OLD APPROACH

        List<List<string>> tilesOnHandHistory = new List<List<string>>();
        for (int i = History.Count - 1; i >= 0; i--)
        {
            if (History[i].Contains("#TILESONHAND_" + Startup._instance.MyPlayfabID + "#"))
            {
                ListContainer tileList = JsonUtility.FromJson<ListContainer>(History[i].Replace("#TILESONHAND_" + Startup._instance.MyPlayfabID + "#", ""));
                tilesOnHandHistory.Add(tileList.Data);
            }
            if (History[i].Contains("#SWAP#" + Startup.instance.MyPlayfabID + "#"))
            {
                string val = History[i].Replace("#SWAP#" + Startup.instance.MyPlayfabID + "#", "");
                string[] numberA = val.Split(',');
                List<string> listOfNumbers = new List<string>();
                //for (int j = 0; j < numberA.Length; j++)
                //{
                //    if (numberA[j].Length > 0 && numberA[j] != ",")
                //    {
                //        listOfNumbers.Add(numberA[j]);
                //    }
                //}
                //tilesOnHandHistory.Add(listOfNumbers);
                List<Tile> myTiles2 = new List<Tile>(myTiles);

                if (tilesOnHandHistory.Count==0)
                {
                    for (int j = 0; j < numberA.Length; j++)
                    {
                        if (numberA[j].Length > 0 && numberA[j] != ",")
                        {
                            for (int k = 0; k < myTiles2.Count; k++)
                            {
                                string a = numberA[j];
                                string b = myTiles2[k].GetValue();
                                if (a == b)
                                {
                                    Color col;
                                    ColorUtility.TryParseHtmlString("#6EFFB0", out col);
                                    Sequence mySequence = DOTween.Sequence();
                                    mySequence
                                      .Append(myTiles2[k].GetComponent<Image>().DOColor(col, 0.5f))
                                      .PrependInterval(1)
                                      .Append(myTiles2[k].GetComponent<Image>().DOColor(Color.white, 0.5f));

                                    myTiles2.RemoveAt(k);
                                    break;
                                }
                            }


                        }
                    }
                    return;
                }


            }

        }

        if (tilesOnHandHistory.Count > 0)
        {
            List<string> tiles = PlayerBoard.instance.myPlayer.GetMyTiles();
            List<string> newTiles = ComparDif(tiles, tilesOnHandHistory[0]);


            List<Tile> myTiles2 = new List<Tile>(myTiles);
            for (int i = 0; i < newTiles.Count; i++)
            {
                for (int j = 0; j < myTiles2.Count; j++)
                {
                    string a = newTiles[i];
                    string b = myTiles2[j].GetValue();
                    if (a == b)
                    {
                        Color col;
                        ColorUtility.TryParseHtmlString("#6EFFB0", out col);
                        Sequence mySequence = DOTween.Sequence();
                        mySequence
                          .Append(myTiles2[j].GetComponent<Image>().DOColor(col, 0.5f))
                          .PrependInterval(1)
                          .Append(myTiles2[j].GetComponent<Image>().DOColor(Color.white, 0.5f));

                        myTiles2.RemoveAt(j);
                        break;
                    }
                }
            }

            // this is to check if you go the same tile so we check if we placed it and it is new
            for(int i = 0; i < lastMoves.Count;i++)
            {
                for (int j = 0; j < myTiles2.Count; j++)
                {
                    string a = lastMoves[i].Number.ToString();
                    string b = myTiles2[j].GetValue();
                    if (a == b)
                    {
                        Color col;
                        ColorUtility.TryParseHtmlString("#6EFFB0", out col);
                        Sequence mySequence = DOTween.Sequence();
                        mySequence
                          .Append(myTiles2[j].GetComponent<Image>().DOColor(col, 0.5f))
                          .PrependInterval(1)
                          .Append(myTiles2[j].GetComponent<Image>().DOColor(Color.white, 0.5f));

                        myTiles2.RemoveAt(j);
                        break;
                    }
                }  
            }


        }






    }
    List<string> ComparDif (List<string> newL, List<string> oldl )
    {
        List<string> dif = new List<string>();

        List<string> workingOldList = oldl;

        for (int i = 0; i < newL.Count;i++)
        {
            if(oldl.Contains( newL[i] ) == false)
            {
                dif.Add(newL[i]);
            }
            else
            {
                workingOldList.Remove(newL[i]);
            }
        }


        return dif;
    }
    public bool CheckAmountValidWithoutCurrent(StaticTile aTileCurrent,StaticTile aTile1, StaticTile aTile2)
    {

        float storedV = aTileCurrent.GetValue();
        if ((aTileCurrent.myTileType != TileType.NormalTile && aTileCurrent.myTileType != TileType.StartTile) || aTileCurrent.preDestroy == true)
            aTileCurrent.SetValue(0);

        int valid1 = 0;
        if ((aTile1.myTileType == TileType.NormalTile || aTile1.myTileType == TileType.StartTile) && aTile1.preDestroy == false )
            valid1 = 1;
        else
            valid1 = CheckAmountValid(aTile1, aTile1.GetValue().ToString());

        int valid2 = 0;
        if ((aTile2.myTileType == TileType.NormalTile || aTile2.myTileType == TileType.StartTile) && aTile2.preDestroy == false)
            valid2 = 1;
        else
            valid2 = CheckAmountValid(aTile2, aTile2.GetValue().ToString());

        if ((aTileCurrent.myTileType != TileType.NormalTile && aTileCurrent.myTileType != TileType.StartTile) || aTileCurrent.preDestroy == true)
            aTileCurrent.SetValue( (int)storedV );

        if (valid1 > 0 && valid2 > 0)
            return true;
        else
            return false;
    }
    public int CheckAmountValid(StaticTile aTile, string aStringNumber )
    {
        int aNumber = int.Parse(aStringNumber);
   
        int up = 0;
        int down = 0;
        int right = 0;
        int left = 0;

        try
        {

            if (aTile.myTileType != TileType.MultiplicationTile && aTile.myTileType != TileType.SubtractionTile && aTile.myTileType != TileType.DivisionTile)
            {
                //Up
                if (aTile.BoardPosition.y - 2 >= 0 && aTile.BoardPosition.y - 1 >= 0)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() > 0)
                        if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() +
                              BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() ==
                              aNumber)
                            if (CheckAmountValidWithoutCurrent(aTile, BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14], BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14]))
                                up++;
                //Down
                if (aTile.BoardPosition.y + 2 < 14 && aTile.BoardPosition.y + 1 < 14)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() > 0)
                        if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() +
                            BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() ==
                            aNumber)
                            if (CheckAmountValidWithoutCurrent(aTile, BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14], BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14]))
                                down++;
                //Left
                if (aTile.BoardPosition.x - 2 >= 0 && aTile.BoardPosition.x - 1 >= 0)
                    if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
                        if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() +
                            BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
                            aNumber)
                            if (CheckAmountValidWithoutCurrent(aTile, BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14], BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14]))
                                left++;
                //Right
                if (aTile.BoardPosition.x + 2 < 14 && aTile.BoardPosition.x + 1 < 14)
                    if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
                        if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() +
                            BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
                            aNumber)
                            if (CheckAmountValidWithoutCurrent(aTile, BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14], BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14]))
                                right++;
            }


            if (aTile.myTileType != TileType.AdditionTile && aTile.myTileType != TileType.SubtractionTile && aTile.myTileType != TileType.DivisionTile)
            {
                //Up
                if (aTile.BoardPosition.y - 2 >= 0 && aTile.BoardPosition.y - 1 >= 0)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() > 0)
                        if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() *
                        BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() ==
                        aNumber)
                            if (CheckAmountValidWithoutCurrent(aTile, BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14], BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14]))
                                up++;
                //Down
                if (aTile.BoardPosition.y + 2 < 14 && aTile.BoardPosition.y + 1 < 14)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() > 0)
                        if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() *
                        BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() ==
                        aNumber)
                            if (CheckAmountValidWithoutCurrent(aTile, BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14], BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14]))
                                down++;
                //Left
                if (aTile.BoardPosition.x - 2 >= 0 && aTile.BoardPosition.x - 1 >= 0)
                    if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
                        if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() *
                        BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
                        aNumber)
                            if (CheckAmountValidWithoutCurrent(aTile, BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14], BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14]))
                                left++;
                //Right
                if (aTile.BoardPosition.x + 2 < 14 && aTile.BoardPosition.x + 1 < 14)
                    if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
                        if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() *
                        BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
                        aNumber)
                            if (CheckAmountValidWithoutCurrent(aTile, BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14], BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14]))
                                right++;
            }

            if (aTile.myTileType != TileType.AdditionTile && aTile.myTileType != TileType.MultiplicationTile && aTile.myTileType != TileType.DivisionTile)
            {
                //Up
                if (aTile.BoardPosition.y - 2 >= 0 && aTile.BoardPosition.y - 1 >= 0)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() > 0)
                        if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() -
                        BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() ==
                        aNumber)
                            if (CheckAmountValidWithoutCurrent(aTile, BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14], BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14]))
                                up++;
                //Down
                if (aTile.BoardPosition.y + 2 < 14 && aTile.BoardPosition.y + 1 < 14)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() > 0)
                        if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() -
                        BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() ==
                        aNumber)
                            if (CheckAmountValidWithoutCurrent(aTile, BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14], BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14]))
                                down++;
                //Left
                if (aTile.BoardPosition.x - 2 >= 0 && aTile.BoardPosition.x - 1 >= 0)
                    if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
                        if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() -
                        BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
                        aNumber)
                            if (CheckAmountValidWithoutCurrent(aTile, BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14], BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14]))
                                left++;
                //Right
                if (aTile.BoardPosition.x + 2 < 14 && aTile.BoardPosition.x + 1 < 14)
                    if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
                        if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() -
                        BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
                        aNumber)
                            if (CheckAmountValidWithoutCurrent(aTile, BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14], BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14]))
                                right++;

                //Up
                if (aTile.BoardPosition.y - 2 >= 0 && aTile.BoardPosition.y - 1 >= 0)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() > 0)
                        if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() -
                        BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() ==
                        aNumber)
                            if (CheckAmountValidWithoutCurrent(aTile, BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14], BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14]))
                                up++;
                //Down
                if (aTile.BoardPosition.y + 2 < 14 && aTile.BoardPosition.y + 1 < 14)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() > 0)
                        if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() -
                        BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() ==
                        aNumber)
                            if (CheckAmountValidWithoutCurrent(aTile, BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14], BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14]))
                                down++;
                //Left
                if (aTile.BoardPosition.x - 2 >= 0 && aTile.BoardPosition.x - 1 >= 0)
                    if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
                        if (BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() -
                        BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
                        aNumber)
                            if (CheckAmountValidWithoutCurrent(aTile, BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14], BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14]))
                                left++;
                //Right
                if (aTile.BoardPosition.x + 2 < 14 && aTile.BoardPosition.x + 1 < 14)
                    if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
                        if (BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() -
                        BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
                        aNumber)
                            if (CheckAmountValidWithoutCurrent(aTile, BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14], BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14]))
                                right++;
            }

            if (aTile.myTileType != TileType.AdditionTile && aTile.myTileType != TileType.MultiplicationTile && aTile.myTileType != TileType.SubtractionTile)
            {
                //Up
                if (aTile.BoardPosition.y - 2 >= 0 && aTile.BoardPosition.y - 1 >= 0)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() > 0)
                        if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() /
                        BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() ==
                        aNumber)
                            if (CheckAmountValidWithoutCurrent(aTile, BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14], BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14]))
                                up++;
                //Down
                if (aTile.BoardPosition.y + 2 < 14 && aTile.BoardPosition.y + 1 < 14)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() > 0)
                        if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() /
                        BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() ==
                        aNumber)
                            if (CheckAmountValidWithoutCurrent(aTile, BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14], BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14]))
                                down++;
                //Left
                if (aTile.BoardPosition.x - 2 >= 0 && aTile.BoardPosition.x - 1 >= 0)
                    if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
                        if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() /
                        BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
                        aNumber)
                            if (CheckAmountValidWithoutCurrent(aTile, BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14], BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14]))
                                left++;
                //Right
                if (aTile.BoardPosition.x + 2 < 14 && aTile.BoardPosition.x + 1 < 14)
                    if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
                        if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() /
                        BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
                        aNumber)
                            if (CheckAmountValidWithoutCurrent(aTile, BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14], BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14]))
                                right++;

                //Up
                if (aTile.BoardPosition.y - 2 >= 0 && aTile.BoardPosition.y - 1 >= 0)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() > 0)
                        if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() /
                        BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() ==
                        aNumber)
                            if (CheckAmountValidWithoutCurrent(aTile, BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14], BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14]))
                                up++;
                //Down
                if (aTile.BoardPosition.y + 2 < 14 && aTile.BoardPosition.y + 1 < 14)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() > 0)
                        if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() /
                        BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() ==
                        aNumber)
                            if (CheckAmountValidWithoutCurrent(aTile, BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14], BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14]))
                                down++;
                //Left
                if (aTile.BoardPosition.x - 2 >= 0 && aTile.BoardPosition.x - 1 >= 0)
                    if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
                        if (BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() /
                        BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
                        aNumber)
                            if (CheckAmountValidWithoutCurrent(aTile, BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14], BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14]))
                                left++;
                //Right
                if (aTile.BoardPosition.x + 2 < 14 && aTile.BoardPosition.x + 1 < 14)
                    if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
                        if (BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() /
                        BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
                        aNumber)
                            if (CheckAmountValidWithoutCurrent(aTile, BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14], BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14]))
                                right++;
            }
            // BoardTiles[(int)aTile.BoardPosition.x + (int)aTile.BoardPosition.y * 14].GetValue()

        }
        catch
        {

        }
        int tot = 0;
        if (down > 0)
            tot++;
        if (up > 0)
            tot++;
        if (left > 0)
            tot++;
        if (right > 0)
            tot++;

        return tot;
    }
    public bool CheckValid(StaticTile aTile,string aStringNumber)
    {
        

        int aNumber = int.Parse(aStringNumber);
        try
        {

            if (aTile.myTileType != TileType.MultiplicationTile && aTile.myTileType != TileType.SubtractionTile && aTile.myTileType != TileType.DivisionTile )
            {
                //Up
                if(aTile.BoardPosition.y-2>=0 && aTile.BoardPosition.y - 1 >= 0)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() > 0)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() +
                          BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() ==
                          aNumber)
                        return true;
                //Down
                if (aTile.BoardPosition.y + 2 < 14 && aTile.BoardPosition.y + 1 < 14)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() > 0)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() +
                        BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() ==
                        aNumber)
                        return true;
                //Left
                if (aTile.BoardPosition.x - 2 >= 0 && aTile.BoardPosition.x - 1 >= 0)
                    if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
                    if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() +
                        BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
                        aNumber)
                        return true;
                //Right
                if (aTile.BoardPosition.x + 2 < 14 && aTile.BoardPosition.x + 1 < 14)
                    if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
                    if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() +
                        BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
                        aNumber)
                        return true;
            }


            if (aTile.myTileType != TileType.AdditionTile && aTile.myTileType != TileType.SubtractionTile && aTile.myTileType != TileType.DivisionTile)
            {
                //Up
                if (aTile.BoardPosition.y - 2 >= 0 && aTile.BoardPosition.y - 1 >= 0)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() > 0)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() *
                    BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() ==
                    aNumber)
                        return true;
                //Down
                if (aTile.BoardPosition.y + 2 < 14 && aTile.BoardPosition.y + 1 < 14)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() > 0)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() *
                    BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() ==
                    aNumber)
                        return true;
                //Left
                if (aTile.BoardPosition.x - 2 >= 0 && aTile.BoardPosition.x - 1 >= 0)
                    if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
                    if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() *
                    BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
                    aNumber)
                        return true;
                //Right
                if (aTile.BoardPosition.x + 2 < 14 && aTile.BoardPosition.x + 1 < 14)
                    if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
                    if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() *
                    BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
                    aNumber)
                        return true;
            }

            if (aTile.myTileType != TileType.AdditionTile && aTile.myTileType != TileType.MultiplicationTile && aTile.myTileType != TileType.DivisionTile)
            {
                //Up
                if (aTile.BoardPosition.y - 2 >= 0 && aTile.BoardPosition.y - 1 >= 0)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() > 0)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() -
                    BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() ==
                    aNumber)
                        return true;
                //Down
                if (aTile.BoardPosition.y + 2 < 14 && aTile.BoardPosition.y + 1 < 14)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() > 0)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() -
                    BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() ==
                    aNumber)
                        return true;
                //Left
                if (aTile.BoardPosition.x - 2 >= 0 && aTile.BoardPosition.x - 1 >= 0)
                    if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
                    if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() -
                    BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
                    aNumber)
                        return true;
                //Right
                if (aTile.BoardPosition.x + 2 < 14 && aTile.BoardPosition.x + 1 < 14)
                    if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
                    if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() -
                    BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
                    aNumber)
                        return true;

                //Up
                if (aTile.BoardPosition.y - 2 >= 0 && aTile.BoardPosition.y - 1 >= 0)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() > 0)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() -
                    BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() ==
                    aNumber)
                        return true;
                //Down
                if (aTile.BoardPosition.y + 2 < 14 && aTile.BoardPosition.y + 1 < 14)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() > 0)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() -
                    BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() ==
                    aNumber)
                        return true;
                //Left
                if (aTile.BoardPosition.x - 2 >= 0 && aTile.BoardPosition.x - 1 >= 0)
                    if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
                    if (BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() -
                    BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
                    aNumber)
                        return true;
                //Right
                if (aTile.BoardPosition.x + 2 < 14 && aTile.BoardPosition.x + 1 < 14)
                    if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
                    if (BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() -
                    BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
                    aNumber)
                        return true;
            }

            if (aTile.myTileType != TileType.AdditionTile && aTile.myTileType != TileType.MultiplicationTile && aTile.myTileType != TileType.SubtractionTile)
            {
                //Up
                if (aTile.BoardPosition.y - 2 >= 0 && aTile.BoardPosition.y - 1 >= 0)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() > 0)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() /
                    BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() ==
                    aNumber)
                        return true;
                //Down
                if (aTile.BoardPosition.y + 2 < 14 && aTile.BoardPosition.y + 1 < 14)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() > 0)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() /
                    BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() ==
                    aNumber)
                        return true;
                //Left
                if (aTile.BoardPosition.x - 2 >= 0 && aTile.BoardPosition.x - 1 >= 0)
                    if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
                    if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() /
                    BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
                    aNumber)
                        return true;
                //Right
                if (aTile.BoardPosition.x + 2 < 14 && aTile.BoardPosition.x + 1 < 14)
                    if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
                    if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() /
                    BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
                    aNumber)
                        return true;

                //Up
                if (aTile.BoardPosition.y - 2 >= 0 && aTile.BoardPosition.y - 1 >= 0)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() > 0)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() /
                    BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() ==
                    aNumber)
                        return true;
                //Down
                if (aTile.BoardPosition.y + 2 < 14 && aTile.BoardPosition.y + 1 < 14)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() > 0)
                    if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() /
                    BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() ==
                    aNumber)
                        return true;
                //Left
                if (aTile.BoardPosition.x - 2 >= 0 && aTile.BoardPosition.x - 1 >= 0)
                    if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
                    if (BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() /
                    BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
                    aNumber)
                        return true;
                //Right
                if (aTile.BoardPosition.x + 2 < 14 && aTile.BoardPosition.x + 1 < 14)
                    if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
                    if (BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() /
                    BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
                    aNumber)
                        return true;
            }
            // BoardTiles[(int)aTile.BoardPosition.x + (int)aTile.BoardPosition.y * 14].GetValue()

        }
        catch
        {
            
        }


        return false;
    }
    public int GetSpecialTiles()
    {
        int amount = 0;
        for(int i = 0; i < BoardTiles.Count;i++)
        {
            if( BoardTiles[i].myTileType != TileType.EmptyTile && BoardTiles[i].myTileType != TileType.NormalTile && BoardTiles[i].myTileType != TileType.StartTile)
            {
                amount++;
            }
        }
        return amount;

    }
    public void ShowExtraTiles(bool activate)
    {
        for (int i = 0; i < BoardTiles.Count; i++)
        {
            if (BoardTiles[i].myTileType == TileType.MultiplierX2 || BoardTiles[i].myTileType == TileType.MultiplierX3 || BoardTiles[i].myTileType == TileType.MultiplierX4)
            {
                BoardTiles[i].transform.GetChild(0).gameObject.SetActive(activate);
                BoardTiles[i].transform.GetChild(0).gameObject.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                BoardTiles[i].transform.GetChild(0).DOScale(new Vector3(1f, 1f, 1f), 0.4f).SetEase(Ease.OutBounce);

            }
        }
    }

    public void ShowExtraTiles2(bool activate)
    {
        for (int i = 0; i < BoardTiles.Count; i++)
        {
            if (BoardTiles[i].myTileType == TileType.AdditionTile || BoardTiles[i].myTileType == TileType.DivisionTile || BoardTiles[i].myTileType == TileType.SubtractionTile || BoardTiles[i].myTileType == TileType.MultiplicationTile)
            {
                BoardTiles[i].transform.GetChild(0).gameObject.SetActive(activate);
                BoardTiles[i].transform.GetChild(0).gameObject.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                BoardTiles[i].transform.GetChild(0).DOScale(new Vector3(1f, 1f, 1f), 0.4f).SetEase(Ease.OutBounce);
            }
        }
    }
    public bool TilesAndMoveValid()
    {
        //return true;
        if (GameManager.instance.thePlayers[1].isAI)
            return true;

            Dictionary<int, int> d = new Dictionary<int, int>();
        for (int i = 0; i < Board.instance.BoardTiles.Count; i++)
        {
            if (Board.instance.BoardTiles[i]._child != null)
            {
                if (Board.instance.BoardTiles[i].myTileType == StaticTile.TileType.NormalTile)
                {
                    if (d.ContainsKey((int)Board.instance.BoardTiles[i].GetValue()))
                    {
                        d[(int)Board.instance.BoardTiles[i].GetValue()]++;
                    }
                    else
                        d.Add((int)Board.instance.BoardTiles[i].GetValue(), 1);

                }
            }
        }

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
        bool hasFailed = false;
        foreach (KeyValuePair<int, int> v in d)
        {
            if (v.Key <= 10 && v.Value != 7)
                hasFailed = true;

            if (v.Key > 10)
            {
                if (v.Value != 1)
                    hasFailed = true;
            }

            tot += v.Value;

        }

        if (hasFailed)
            return false;




        for (int i = 0; i < PlayerBoard.instance.myPlayer.myTiles.Count; i++)
        {
            if(Startup.instance.GameToLoad.player2_PlayfabId == Startup.instance.MyPlayfabID)
            {
                if (Startup.instance.GameToLoad.p2_tiles.Contains(PlayerBoard.instance.myPlayer.myTiles[i].GetTileNumber()) == false)
                {
                    return false;
                }
            }
            else
            {
                if (Startup.instance.GameToLoad.p1_tiles.Contains(PlayerBoard.instance.myPlayer.myTiles[i].GetTileNumber()) == false)
                {
                    return false;
                }
            }

        
        }





        return true;
    }
    public void PressContinue()
    {
        GameAnalytics.NewDesignEvent("MadeMove");


        if (Startup.instance.GameToLoad != null)
        {
            if (Startup.instance.GameToLoad.RoomName != boardData.RoomName ||
                 Startup.instance.GameToLoad.player1_PlayfabId != boardData.player1_PlayfabId ||
                 Startup.instance.GameToLoad.player2_PlayfabId != boardData.player2_PlayfabId)
            {
                return;
            }
        }
 


        if (GameManager.instance.CheckIfMyTurn() == false)
            return;
        int notPlaced = 0;
        bool allValid = true;
        for (int i = 0; i < PlayerBoard.instance.myPlayer.myTiles.Count; i++)
        {
            if (PlayerBoard.instance.myPlayer.myTiles[i].PlacedOnTile != null &&  PlayerBoard.instance.myPlayer.myTiles[i].isValidPlaced() == false)
                allValid = false;

            if (PlayerBoard.instance.myPlayer.myTiles[i].PlacedOnTile == null)
                notPlaced++;
        }
        if(notPlaced == 6 || notPlaced == PlayerBoard.instance.myPlayer.myTiles.Count)
        {
            ValidationScreen.instance.OpenWindow();
            return;
        }
        if(allValid)
        {
            if (TilesAndMoveValid())
            {

            }
            else
            {
                SceneManager.LoadScene(0);
                if (LoadingOverlay.instance != null)
                    LoadingOverlay.instance.ShowLoadingFullscreen("Updating..");
                Startup._instance.Refresh(0.1f);

                return;
            }


            List<Tile> scoreTiles = new List<Tile>();
            for (int i = 0; i < PlayerBoard.instance.myPlayer.myTiles.Count; i++)
            {
                if (PlayerBoard.instance.myPlayer.myTiles[i].isValidPlaced())
                    scoreTiles.Add(PlayerBoard.instance.myPlayer.myTiles[i]);
            }
            GameManager.instance.IsSendingData = true;
            GameManager.instance.SendingDataDelay = 0;
            ScoreScreen.instance.ShowScore(scoreTiles, PlayerBoard.instance.myPlayer);


            if (TutorialController.instance != null)
                if (TutorialController.instance.myActions[TutorialController.instance.CurrentIndex].ID == 8)
            {
                    TutorialController.instance.TapToContinue();
            }



                    //for (int i = 0; i < PlayerBoard.instance.myTiles.Count; i++)
                    //{
                    //    PlayerBoard.instance.myTiles[i].Flip();
                    //}
                    //StartCoroutine(UpdateTilesAfterTime());
                }

        

    }
    //void UpdateTilesAfterTime()
    //{
    //    yield return new WaitForSeconds(2f);
    //    PlayerBoard.instance.AddNewPlayerTiles();

    //}
    public void SetTile(int aX,int aY, TileType aType, int aNumber)
    {
        BoardTiles[aX +  aY*14].SetTile(aType, aNumber);

    
    }
    public void SetTileColor(int aX, int aY, Color aCol)
    {
        if(BoardTiles[aX + aY * 14].transform.childCount==1)
        {
                BoardTiles[aX + aY * 14].transform.GetChild(0).GetComponent<Image>().color = aCol;
        }
        else if (BoardTiles[aX + aY * 14].transform.childCount >1)
        {
            if(BoardTiles[aX + aY * 14].transform.GetChild(0).name.Contains( "StartTile" ))
                BoardTiles[aX + aY * 14].transform.GetChild(0).GetComponent<Image>().color = aCol;
            else 
                BoardTiles[aX + aY * 14].transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }
        //BoardTiles[aX + aY * 14].transform.GetChild(0).gameObject.SetActive(false);
        //Destroy(BoardTiles[aX + aY * 14].transform.GetChild(0).gameObject);
    }
    public float GetScaleDif()
    {
        return 1+transform.localScale.x- 1.017f;
    }
    private float initialDistance;
    private Vector3 initialScale;
    private Vector3 initialPosition;

    Vector3 LastFrameSelectionPosition;
    float _timerSoudn = 0;

    // Update is called once per frame
    void Update()
    {

        //if (Input.GetKeyUp(KeyCode.Q))
        //    PressContinue();
        //if (Input.GetKeyUp(KeyCode.W))
        //    GameManager.instance.CurrentTurn = 0;

        //if (Input.GetKeyUp(KeyCode.E))
        //    ShowExtraTiles(true);
        //if (Input.GetKeyUp(KeyCode.R))
        //    ShowExtraTiles2(true);

        //if (Input.GetKeyUp(KeyCode.T))
        //{
        //    GameFinishedScreen.instance.Show();
        //}
        //if (Input.GetKeyUp(KeyCode.A))
        //{
        //    Startup._instance.ConfetiPart.SetActive(false);
        //    Startup._instance.ConfetiPart.SetActive(true);

        //}


        //if (Input.GetKeyUp(KeyCode.T))
        //    ShowExtraTiles2(false);
        //if (Input.GetKeyUp(KeyCode.Y))
        //    ShowExtraTiles2(true);


        if ( SceneManager.GetActiveScene().name == "MenuScene" || SceneManager.GetActiveScene().name == "testBasic")
        {
            return;
        }


        if (Board.instance.Selection.GetComponent<Image>().enabled || GetIsDraginTile())
        {


            //if(_timerSoudn>0.2f)
            //{
            //    if (LastFrameSelectionPosition != Board.instance.Selection.transform.position)
            //        Startup._instance.PlaySoundEffect(0);
            //    LastFrameSelectionPosition = Board.instance.Selection.transform.position;
            //    _timerSoudn = 0;
                
            //}
            //_timerSoudn += Time.deltaTime;



            dragCooldownTimer = 0.1f;
            return;
        }



        if (dragCooldownTimer > 0)
        {
            dragCooldownTimer -= Time.deltaTime;
            return;
        }
        if (GameManager.instance.aspect >= 1.4f)
        {

        
        if (Input.touchCount == 2)
        {
            var touchZero = Input.GetTouch(0);
            var touchOne = Input.GetTouch(1);

            // if one of the touches Ended or Canceled do nothing
            if (touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled
               || touchOne.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Canceled)
            {
                return;
            }

            // It is enough to check whether one of them began since we
            // already excluded the Ended and Canceled phase in the line before
            if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
            {
                // track the initial values
                initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
                initialScale = transform.transform.localScale;
                initialPosition = transform.localPosition;
            }
            // else now is any other case where touchZero and/or touchOne are in one of the states
            // of Stationary or Moved
            else
            {
                // otherwise get the current distance
                var currentDistance = Vector2.Distance(touchZero.position, touchOne.position);

                // A little emergency brake ;)
                if (Mathf.Approximately(initialDistance, 0)) return;

                // get the scale factor of the current distance relative to the inital one
                var factor = currentDistance / initialDistance;

                if((initialScale * factor).x < 1.017f)
                {
                    initialScale = new Vector3(1.017f, 1.017f, 1.017f);
                    factor = 1;
                }
                if((initialScale * factor).x > 2.30338f)
                {
                    initialScale = new Vector3(2.30338f, 2.30338f, 2.30338f);
                    factor = 1;
                }

                // apply the scale
                // instead of a continuous addition rather always base the 
                // calculation on the initial and current value only
                bool shouldScale = true;
                if (factor<0)
                    transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 7);

                transform.localPosition += new Vector3(touchZero.deltaPosition.x, touchZero.deltaPosition.y,0);

                Vector3[] v = new Vector3[4];
                transform.GetComponent<RectTransform>().GetWorldCorners(v);

                Vector3[] canvasC = new Vector3[4];
                GameObject.Find("Canvas").GetComponent<RectTransform>().GetWorldCorners(canvasC);


                Vector3[] canvasMask = new Vector3[4];
                transform.parent.GetComponent<RectTransform>().GetWorldCorners(canvasMask);


                if (v[0].x > canvasC[0].x)
                {
                    transform.localPosition -= new Vector3(touchZero.deltaPosition.x, 0, 0);
                    shouldScale = false;
                }

                else if (v[2].x < canvasC[2].x)
                {
                    transform.localPosition -= new Vector3(touchZero.deltaPosition.x, 0, 0);
                    shouldScale = false;

                }
                if (v[0].y > canvasMask[0].y)
                {
                    transform.localPosition -= new Vector3(0, touchZero.deltaPosition.y, 0);
                    shouldScale = false;

                }
                else if (v[2].y < canvasMask[2].y)
                {
                    transform.localPosition -= new Vector3(0, touchZero.deltaPosition.y, 0);
                    shouldScale = false;

                }

                if (transform.localScale.x < 1.02f)
                    transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 7);


                if(shouldScale )
                    transform.transform.localScale = initialScale * factor;
                else
                {
                    transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 7);

                }


            }

      






        }
        else if (Input.touchCount == 1)
        {
            if(transform.localScale.x > 1.08f)
            {
                var touchZero = Input.GetTouch(0);


                transform.localPosition += new Vector3(touchZero.deltaPosition.x, touchZero.deltaPosition.y, 0);

                Vector3[] v = new Vector3[4];
                transform.GetComponent<RectTransform>().GetWorldCorners(v);

                Vector3[] canvasC = new Vector3[4];
                GameObject.Find("Canvas").GetComponent<RectTransform>().GetWorldCorners(canvasC);

                Vector3[] canvasMask = new Vector3[4];
                transform.parent.GetComponent<RectTransform>().GetWorldCorners(canvasMask);


                if (v[0].x > canvasC[0].x)
                {
                    transform.localPosition -= new Vector3(touchZero.deltaPosition.x, 0, 0);
                }
                else if (v[2].x < canvasC[2].x)
                {
                    transform.localPosition -= new Vector3(touchZero.deltaPosition.x, 0, 0);
                }

                if (v[0].y > canvasMask[0].y)
                {
                    transform.localPosition -= new Vector3(0, touchZero.deltaPosition.y, 0);
                }
                else if (v[2].y < canvasMask[2].y)
                {
                    transform.localPosition -= new Vector3(0, touchZero.deltaPosition.y, 0);
                }
            }
  



        }
        else
        {
            if (transform.localScale.x < 1.02f)
                transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 7);
        }
        


        if (ScoreScreen.instance.bg.activeSelf )
        {

            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 11);

            initialScale = new Vector3(1.017f, 1.017f, 1.017f);
            transform.transform.localScale = Vector3.Lerp(transform.transform.localScale, initialScale, Time.deltaTime * 5);

        }
        }

    }
    float dragCooldownTimer = 0;
    bool GetIsDraginTile ()
    {
        for(int i = 0; i < GameManager.instance.thePlayers[0].myTiles.Count;i++)
        {
            if( GameManager.instance.thePlayers[0].myTiles[i].myTileStatus == Tile.TileStatus.Dragging)
            {
                dragCooldownTimer = 0.1f;
                return true;
            }

        }

        return false;
    }
}
