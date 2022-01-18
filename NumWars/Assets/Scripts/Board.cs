using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StaticTile;

public class Board : MonoBehaviour
{
    public Transform parent;

    public List<StaticTile> BoardTiles = new List<StaticTile>();
    public static Board instance;
    public GameObject Selection;
    public GameObject ScoreEffect;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        for (int i = 0; i < parent.childCount;i++)
        {
            StaticTile st = parent.GetChild(i).GetComponent<StaticTile>();
            st.BoardPosition = new Vector2(i - ((int)i / 14)*14, (int)i / 14);
            BoardTiles.Add(st);
        }
        SetTile(6, 6, TileType.StartTile, 1);
        SetTile(7, 6, TileType.StartTile, 2);
        SetTile(6, 7, TileType.StartTile, 3);
        SetTile(7, 7, TileType.StartTile, 4);


        SetTile(5, 0, TileType.MultiplierX3, 1);
        SetTile(8, 0, TileType.MultiplierX3, 2);
        SetTile(6, 1, TileType.MultiplierX2, 3);
        SetTile(7, 1, TileType.MultiplierX2, 4);

        SetTile(5, 13, TileType.MultiplierX3, 1);
        SetTile(8, 13, TileType.MultiplierX3, 2);
        SetTile(6, 12, TileType.MultiplierX2, 3);
        SetTile(7, 12, TileType.MultiplierX2, 4);

        SetTile(13, 5, TileType.MultiplierX3, 1);
        SetTile(13, 8, TileType.MultiplierX3, 2);
        SetTile(12, 6, TileType.MultiplierX2, 3);
        SetTile(12, 7, TileType.MultiplierX2, 4);

        SetTile(0, 5, TileType.MultiplierX3, 1);
        SetTile(0, 8, TileType.MultiplierX3, 2);
        SetTile(1, 6, TileType.MultiplierX2, 3);
        SetTile(1, 7, TileType.MultiplierX2, 4);

        SetTile(0, 0, TileType.MultiplierX4, 4);
        SetTile(2, 2, TileType.MultiplierX4, 4);
        SetTile(13, 0, TileType.MultiplierX4, 4);
        SetTile(11, 2, TileType.MultiplierX4, 4);

        SetTile(0, 13, TileType.MultiplierX4, 4);
        SetTile(2, 11, TileType.MultiplierX4, 4);
        SetTile(13, 13, TileType.MultiplierX4, 4);
        SetTile(11, 11, TileType.MultiplierX4, 4);

        SetTile(1, 1, TileType.MultiplierX2, 3);
        SetTile(3, 3, TileType.MultiplierX2, 4);
        SetTile(12, 1, TileType.MultiplierX2, 3);
        SetTile(10, 3, TileType.MultiplierX2, 4);
        SetTile(1, 12, TileType.MultiplierX2, 3);
        SetTile(3, 10, TileType.MultiplierX2, 4);
        SetTile(12, 12, TileType.MultiplierX2, 3);
        SetTile(10, 10, TileType.MultiplierX2, 4);

        // -
        SetTile(5,2 , TileType.SubtractionTile, 3);
        SetTile(8,2 , TileType.SubtractionTile, 4);
        SetTile(8, 11, TileType.SubtractionTile, 4);
        SetTile(5, 11, TileType.SubtractionTile, 4);

        // /
        SetTile(9, 1, TileType.DivisionTile, 4);
        SetTile(4, 1, TileType.DivisionTile, 4);
        SetTile(9, 12, TileType.DivisionTile, 4);
        SetTile(4, 12, TileType.DivisionTile, 4);

        SetTile(1, 4, TileType.DivisionTile, 4);
        SetTile(1, 9, TileType.DivisionTile, 4);
        SetTile(2, 5, TileType.SubtractionTile, 4);
        SetTile(2, 8, TileType.SubtractionTile, 4);

        SetTile(12, 4, TileType.DivisionTile, 4);
        SetTile(12, 9, TileType.DivisionTile, 4);
        SetTile(11, 5, TileType.SubtractionTile, 4);
        SetTile(11, 8, TileType.SubtractionTile, 4);


        SetTile(6, 9, TileType.MultiplicationTile, 4);
        SetTile(7, 9, TileType.AdditionTile, 4);
        SetTile(6, 4, TileType.AdditionTile, 4);
        SetTile(7, 4, TileType.MultiplicationTile , 4);

        SetTile(4, 6, TileType.MultiplicationTile, 4);
        SetTile(4, 7, TileType.AdditionTile, 4);
        SetTile(9, 6, TileType.AdditionTile, 4);
        SetTile(9, 7, TileType.MultiplicationTile, 4);

        SetTile(5, 5, TileType.MultiplierX2, 3);
        SetTile(8, 5, TileType.MultiplierX2, 3);
        SetTile(5, 8, TileType.MultiplierX2, 3);
        SetTile(8, 8, TileType.MultiplierX2, 3);

    }
    public bool CheckValid(StaticTile aTile,string aStringNumber)
    {
        int aNumber = int.Parse(aStringNumber);

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




        return false;
    }

    public void PressContinue()
    {
        bool allValid = true;
        for (int i = 0; i < PlayerBoard.instance.myTiles.Count; i++)
        {
            if (PlayerBoard.instance.myTiles[i].PlacedOnTile != null &&  PlayerBoard.instance.myTiles[i].isValidPlaced() == false)
                allValid = false;
        }

        if(allValid)
        {
            List<Tile> scoreTiles = new List<Tile>();
            for (int i = 0; i < PlayerBoard.instance.myTiles.Count; i++)
            {
                if (PlayerBoard.instance.myTiles[i].isValidPlaced())
                    scoreTiles.Add(PlayerBoard.instance.myTiles[i]);
            }

            ScoreScreen.instance.ShowScore(scoreTiles);







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
