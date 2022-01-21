using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StaticTile;

public class Board : MonoBehaviour
{
    public Transform parent;

    public List<StaticTile> BoardTiles = new List<StaticTile>();

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
        SetTile(6, 6, TileType.StartTile, 1);
        SetTile(7, 6, TileType.StartTile, 2);
        SetTile(6, 7, TileType.StartTile, 3);
        SetTile(7, 7, TileType.StartTile, 4);


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
    public bool CheckValid(StaticTile aTile,string aStringNumber)
    {
        int aNumber = int.Parse(aStringNumber);
        try
        {

        
        //Up
        if(BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue()>0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue()>0)
            if(BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y -2 )*14].GetValue() +
            BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y -1)*14].GetValue() ==
            aNumber)
            return true;
        //Down
        if(BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue()>0&& BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue()>0)
        if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() +
            BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() ==
            aNumber)
            return true;
        //Left
        if(BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue()>0&& BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue()>0)
        if (BoardTiles[(int)(aTile.BoardPosition.x-1) + (int)(aTile.BoardPosition.y ) * 14].GetValue() +
            BoardTiles[(int)(aTile.BoardPosition.x-2) + (int)(aTile.BoardPosition.y ) * 14].GetValue() ==
            aNumber)
            return true;
        //Right
        if(BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue()>0&& BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue()>0)
        if (BoardTiles[(int)(aTile.BoardPosition.x+1) + (int)(aTile.BoardPosition.y ) * 14].GetValue() +
            BoardTiles[(int)(aTile.BoardPosition.x+2) + (int)(aTile.BoardPosition.y ) * 14].GetValue() ==
            aNumber)
            return true;

        //Up
        if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() > 0)
            if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() *
            BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() ==
            aNumber)
            return true;
        //Down
        if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() > 0)
            if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() *
            BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() ==
            aNumber)
            return true;
        //Left
        if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
            if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() *
            BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
            aNumber)
            return true;
        //Right
        if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
            if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() *
            BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
            aNumber)
            return true;

        //Up
        if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() > 0)
            if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() -
            BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() ==
            aNumber)
            return true;
        //Down
        if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() > 0)
            if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() -
            BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() ==
            aNumber)
            return true;
        //Left
        if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
            if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() -
            BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
            aNumber)
            return true;
        //Right
        if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
            if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() -
            BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
            aNumber)
            return true;

        //Up
        if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() > 0)
            if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() -
            BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() ==
            aNumber)
            return true;
        //Down
        if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() > 0)
            if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() -
            BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() ==
            aNumber)
            return true;
        //Left
        if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
            if (BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() -
            BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
            aNumber)
            return true;
        //Right
        if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
            if (BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() -
            BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
            aNumber)
            return true;

        //Up
        if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() > 0)
            if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() /
            BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() ==
            aNumber)
            return true;
        //Down
        if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() > 0)
            if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() /
            BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() ==
            aNumber)
            return true;
        //Left
        if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
            if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() /
            BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
            aNumber)
            return true;
        //Right
        if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
            if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() /
            BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
            aNumber)
            return true;

        //Up
        if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 1) * 14].GetValue() > 0)
            if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 2) * 14].GetValue() /
            BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y - 3) * 14].GetValue() ==
            aNumber)
            return true;
        //Down
        if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() > 0 && BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() > 0)
            if (BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 1) * 14].GetValue() /
            BoardTiles[(int)aTile.BoardPosition.x + (int)(aTile.BoardPosition.y + 2) * 14].GetValue() ==
            aNumber)
            return true;
        //Left
        if (BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
            if (BoardTiles[(int)(aTile.BoardPosition.x - 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() /
            BoardTiles[(int)(aTile.BoardPosition.x - 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
            aNumber)
            return true;
        //Right
        if (BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0 && BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() > 0)
            if (BoardTiles[(int)(aTile.BoardPosition.x + 2) + (int)(aTile.BoardPosition.y) * 14].GetValue() /
            BoardTiles[(int)(aTile.BoardPosition.x + 1) + (int)(aTile.BoardPosition.y) * 14].GetValue() ==
            aNumber)
            return true;

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
    // Update is called once per frame
    void Update()
    {
        
    }
}
