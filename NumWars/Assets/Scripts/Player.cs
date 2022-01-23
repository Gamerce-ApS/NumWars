using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

  
    public Player Init(string aName, int aID,GameObject aScoreObject, bool aIsAI = false)
    {
        Username = aName;
        ID = aID;
        isAI = aIsAI;
        scoreObject = aScoreObject;
        if (ID == 0)
        {
            tileParent = PlayerBoard.instance.panel.transform;
        }
        else
        {
            tileParent = PlayerBoard.instance.panelAi.transform; ;
        }

        AddNewPlayerTiles();


        return this;
    }
    public string Username = "";
    public int Score = 0;
    public int ID = 0;
    public int LastScore = 0;
    public bool isAI = false;

    public List<Tile> myTiles = new List<Tile>();

    Transform tileParent;

    public GameObject scoreObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoAI()
    {
        Debug.Log("Do AI!");
        StartCoroutine(AiSequence());


    }

    IEnumerator AiSequence()
    {


        yield return new WaitForSeconds(4.5f);

        int placedTiles = 0;
        for(int i = 0; i< myTiles.Count;i++ )
        {
            for(int j = 0; j < Board.instance.BoardTiles.Count;j++)
            {
                if( Board.instance.BoardTiles[j].GetValue()==0)
                {
                    if(Board.instance.CheckValid(Board.instance.BoardTiles[j], myTiles[i].GetValue()) == true )
                    {
                        placedTiles++;
                        Debug.Log("Valid:" + j + " Value: " + myTiles[i].GetValue());
                        Board.instance.Selection.transform.position = Board.instance.BoardTiles[j].transform.position;
                        myTiles[i].PlacedOnTile = Board.instance.BoardTiles[j];
                        myTiles[i].PlaceTileOnSelection();
                        break ;
                    }
                }
            }
        }

        yield return new WaitForSeconds(1.75f);
        if(placedTiles == 0)
        {
            // Swap
            for (int i = myTiles.Count - 1; i >= 0; i--)
            {
                Tile currentT = myTiles[i];
                Board.instance.AllTilesNumbers.Add(int.Parse(currentT.textLabel.text));
                myTiles.Remove(currentT);
                Destroy(currentT.gameObject);
            }
            AddNewPlayerTiles();
            GameManager.instance.NextTurn();


        }
        else
        {
            List<Tile> scoreTiles = new List<Tile>();
            for (int i = 0; i < myTiles.Count; i++)
            {
                if (myTiles[i].isValidPlaced())
                    scoreTiles.Add(myTiles[i]);
            }

            ScoreScreen.instance.ShowScore(scoreTiles, this);
        }

   




    }

    public void AddNewPlayerTiles()
    {
       
        for (int i = myTiles.Count; i < 6; i++)
        {
            GameObject go = GameObject.Instantiate(PlayerBoard.instance.TileObject, tileParent);
            myTiles.Add(go.GetComponent<Tile>());
            go.GetComponent<Tile>().Init(Board.instance.AllTilesNumbers[0]);
            Board.instance.AllTilesNumbers.RemoveAt(0);
        }

        

    }

}
