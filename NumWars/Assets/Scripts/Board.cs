using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static StaticTile;

[Serializable]
public  class BoardData
{
    public BoardData(string p1, string p2, string turn, List<StaticTile> tileData,string aRoomName,List<string> aHistory, List<string> tilesLeft,string aEmptyTurns, List<string> ap1_tiles, List<string> ap2_tiles)
    {
        player1_PlayfabId = p1;
        player2_PlayfabId = p2;
        playerTurn = turn;
        RoomName = aRoomName;
        EmptyTurns = aEmptyTurns;
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

    public string EmptyTurns = "";
    public List<string> TilesLeft = new List<string>();

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

        EmptyTurns = bd.EmptyTurns;
        TilesLeft = bd.TilesLeft;
        p1_tiles = bd.p1_tiles;
        p2_tiles = bd.p2_tiles;

    }


    public string GetOtherPlayer()
    {
        if (Startup._instance.MyPlayfabID == player1_PlayfabId)
            return player2_displayName;
        else
            return player1_displayName;

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



};



public class Board : MonoBehaviour
{

    public Transform parent;

    public List<StaticTile> BoardTiles = new List<StaticTile>();
    public List<string> History = new List<string>();
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

    }
    public void Init()
    {
        // Add tiles to pool
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


        // Create board layout

        for (int i = 0; i < parent.childCount; i++)
        {
            StaticTile st = parent.GetChild(i).GetComponent<StaticTile>();
            st.BoardPosition = new Vector2(i - ((int)i / 14) * 14, (int)i / 14);
            BoardTiles.Add(st);
        }


        List<int> startNumbers = new List<int>();
        startNumbers.Add(1);
        startNumbers.Add(2);
        startNumbers.Add(3);
        startNumbers.Add(4);
        startNumbers.Shuffle();
        SetTile(6, 6, TileType.StartTile, startNumbers[0]);
        SetTile(7, 6, TileType.StartTile, startNumbers[1]);
        SetTile(6, 7, TileType.StartTile, startNumbers[2]);
        SetTile(7, 7, TileType.StartTile, startNumbers[3]);




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


     //   PlayerPrefs.SetString("PlayerBoard", new BoardData("id1","id2","0",BoardTiles).GetJson());

    }

    // Temp stuff for generating new game
    public List<string> p1_tiles = new List<string>();
    public List<string> p2_tiles = new List<string>();
    public void GenerateStartBoard()
    {
        AllTilesNumbers.Clear();
        p1_tiles.Clear();
        p2_tiles.Clear();
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

        for(int i = 0; i < 6; i++)
        {
            p1_tiles.Add(AllTilesNumbers[0].ToString());
            AllTilesNumbers.RemoveAt(0);
            p2_tiles.Add(AllTilesNumbers[0].ToString());
            AllTilesNumbers.RemoveAt(0);

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
    public void LoadBoardData(BoardData aBD)
    {

        for (int i = 0; i < aBD.BoardTiles.Count; i++)
        {
            BoardTiles[i].LoadFromJson(aBD.BoardTiles[i]);
            BoardTiles[i].Refresh();
        }

        History = aBD.History;
        AllTilesNumbers.Clear();
        for (int i = 0; i < aBD.TilesLeft.Count; i++)
        {
            AllTilesNumbers.Add(int.Parse( aBD.TilesLeft[i] ));
        }

        


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

    public void PressContinue()
    {
        if (GameManager.instance.CheckIfMyTurn() == false)
            return;

        bool allValid = true;
        for (int i = 0; i < PlayerBoard.instance.myPlayer.myTiles.Count; i++)
        {
            if (PlayerBoard.instance.myPlayer.myTiles[i].PlacedOnTile != null &&  PlayerBoard.instance.myPlayer.myTiles[i].isValidPlaced() == false)
                allValid = false;
        }

        if(allValid)
        {
            List<Tile> scoreTiles = new List<Tile>();
            for (int i = 0; i < PlayerBoard.instance.myPlayer.myTiles.Count; i++)
            {
                if (PlayerBoard.instance.myPlayer.myTiles[i].isValidPlaced())
                    scoreTiles.Add(PlayerBoard.instance.myPlayer.myTiles[i]);
            }

            ScoreScreen.instance.ShowScore(scoreTiles, PlayerBoard.instance.myPlayer);







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
    public float GetScaleDif()
    {
        return 1+transform.localScale.x- 1.017f;
    }
    private float initialDistance;
    private Vector3 initialScale;
    private Vector3 initialPosition;
    // Update is called once per frame
    void Update()
    {
        if( SceneManager.GetActiveScene().name == "MenuScene")
        {
            return;
        }


        if (Board.instance.Selection.GetComponent<Image>().enabled)
            return;

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
            else if(v[2].x < canvasC[2].x)
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
        else
        {
            if (transform.localScale.x < 1.02f)
                transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 7);
        }





    }
}
