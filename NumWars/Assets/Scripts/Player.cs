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

      //  AddNewPlayerTiles();


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
    public List<string> GetMyTiles()
    {
        List<string> tileListString = new List<string>();
        for (int i = 0; i < myTiles.Count; i++)
        {
            tileListString.Add(myTiles[i].GetValue());
        }
        return tileListString;
    }
    public void LoadPlayerTiles(List<string> aList)
    {
        for (int i = 0; i < aList.Count; i++)
        {
            GameObject go = GameObject.Instantiate(PlayerBoard.instance.TileObject, tileParent);
            myTiles.Add(go.GetComponent<Tile>());
            go.GetComponent<Tile>().Init(int.Parse(aList[i]));
            go.GetComponent<Tile>().TileParent = tileParent;
        }
    }

    public void DoAI()
    {
        Debug.Log("Do AI!");
        StartCoroutine(AiSequence());


     



    }
    bool swapedLastTurn = false;
    IEnumerator AiSequence()
    {
        if(Board.instance.GetTilesLeft().Count<=0)
        {
            GameManager.instance.NextTurn(true);
            yield break;
        }

        // Use this for making empty turns
        //yield return new WaitForSeconds(1.75f);
        //yield return new WaitForSeconds(4.5f);
        //GameManager.instance.NextTurn(true);
        //yield break;

        yield return new WaitForSeconds(3.0f);

        int placedTiles = 0;

        for (int i = 0; i < myTiles.Count; i++)
        {
            for (int j = 0; j < Board.instance.BoardTiles.Count; j++)
            {
                if (Board.instance.BoardTiles[j].GetValue() == 0)
                {
                    if (Board.instance.CheckValid(Board.instance.BoardTiles[j], myTiles[i].GetValue()) == true)
                    {
                        placedTiles++;
                        Debug.Log("Valid:" + j + " Value: " + myTiles[i].GetValue());
                        Board.instance.Selection.transform.position = Board.instance.BoardTiles[j].transform.position;
                        myTiles[i].PlacedOnTile = Board.instance.BoardTiles[j];
                        myTiles[i].PlaceTileOnSelection();
                        break;
                    }
                }
            }
        }

        GameManager.instance.HideThinkingOverlay();
        yield return new WaitForSeconds(1.5f);
        if(placedTiles == 0 )
        {
            bool isEmptyTurn = false;

            if (swapedLastTurn == false)
            {
                if (Board.instance.AllTilesNumbers.Count == 0)
                    isEmptyTurn = true;
                // Swap
                for (int i = myTiles.Count - 1; i >= 0; i--)
                {
                    Tile currentT = myTiles[i];
                    Board.instance.AllTilesNumbers.Add(int.Parse(currentT.textLabel.text));
                    myTiles.Remove(currentT);
                    Destroy(currentT.gameObject);
                    swapedLastTurn = true;
                }
                AddNewPlayerTiles();
                Board.instance.History.Add("#SWAP#");
                AlertText.instance.ShowAlert("Ai swapped!", 0.5f);
                GameManager.instance.MakeLastPlayedTilesColored();


            }
            else
            {
                isEmptyTurn = true;
            }

            GameManager.instance.NextTurn(isEmptyTurn);

        }
        else
        {
            swapedLastTurn = false;
            List<Tile> scoreTiles = new List<Tile>();
            for (int i = 0; i < myTiles.Count; i++)
            {
                if (myTiles[i].isValidPlaced())
                    scoreTiles.Add(myTiles[i]);
            }

            ScoreScreen.instance.ShowScore(scoreTiles, this);
        }


        yield return new WaitForSeconds(1.0f);




    }

    public void AddNewPlayerTiles()
    {

        //Board.instance.AllTilesNumbers[0] = (2);
        //Board.instance.AllTilesNumbers[1]=(4);
        //Board.instance.AllTilesNumbers[2]=(6);
        //Board.instance.AllTilesNumbers[3]=(10);
        //Board.instance.AllTilesNumbers[4] =(4);
        //Board.instance.AllTilesNumbers[5]=(14);




        for (int i = myTiles.Count; i < 6; i++)
            {
                if (Board.instance.AllTilesNumbers.Count > 0)
                {
                    GameObject go = GameObject.Instantiate(PlayerBoard.instance.TileObject, tileParent);
                    myTiles.Add(go.GetComponent<Tile>());
                    go.GetComponent<Tile>().Init(Board.instance.AllTilesNumbers[0]);
                go.GetComponent<Tile>().TileParent = tileParent;
                Board.instance.AllTilesNumbers.RemoveAt(0);
                }

            }
            GameManager.instance.tileLeft.text = Board.instance.AllTilesNumbers.Count.ToString();
        
    }

}
