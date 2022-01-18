using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBoard : MonoBehaviour
{
    public GameObject TileObject;
    public GameObject panel;
    public List<Tile> myTiles;

    public static PlayerBoard instance;

    public List<int> AllTilesNumbers = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        for(int i = 1; i < 11;i++)
        {
            for (int j = 0; j < 7; j++)
                AllTilesNumbers.Add(i);
        }
        for (int i = 11; i < 22; i++)
        {
                AllTilesNumbers.Add(i);
        }
        AllTilesNumbers.Add(24);AllTilesNumbers.Add(25);AllTilesNumbers.Add(27);AllTilesNumbers.Add(28);AllTilesNumbers.Add(30);AllTilesNumbers.Add(32);AllTilesNumbers.Add(35);AllTilesNumbers.Add(36);
        AllTilesNumbers.Add(40);AllTilesNumbers.Add(42);AllTilesNumbers.Add(45);AllTilesNumbers.Add(48);AllTilesNumbers.Add(49);AllTilesNumbers.Add(50);AllTilesNumbers.Add(54);AllTilesNumbers.Add(56);AllTilesNumbers.Add(60);
        AllTilesNumbers.Add(63);AllTilesNumbers.Add(64);AllTilesNumbers.Add(70);AllTilesNumbers.Add(72);AllTilesNumbers.Add(80);AllTilesNumbers.Add(81);AllTilesNumbers.Add(90);

        AllTilesNumbers.Shuffle();

        AddNewPlayerTiles();
        //for (int i = 0; i < 6;i++)
        //{
        //    GameObject go = GameObject.Instantiate(TileObject, panel.transform);
        //    myTiles.Add(go.GetComponent<Tile>());
        //}
        //LayoutRebuilder.ForceRebuildLayoutImmediate(panel.GetComponent<RectTransform>());
        //for (int i = 0; i < myTiles.Count; i++)
        //{

        //    myTiles[i].Init(AllTilesNumbers[0]);
        //    AllTilesNumbers.RemoveAt(0);


        //}
        //panel.GetComponent<HorizontalLayoutGroup>().enabled = false;
    }

    public void AddNewPlayerTiles()
    {
        panel.GetComponent<HorizontalLayoutGroup>().enabled = true;
        for (int i = myTiles.Count; i< 6;i++)
        {
            GameObject go = GameObject.Instantiate(TileObject, panel.transform);
            myTiles.Add(go.GetComponent<Tile>());
            go.GetComponent<Tile>().Init(AllTilesNumbers[0]);
            AllTilesNumbers.RemoveAt(0);
        }
        myTiles.Sort(HelperFunctions.SortByScore);
        for (int i = 0; i < myTiles.Count; i++)
        {
            myTiles[i].transform.SetAsFirstSibling();
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel.GetComponent<RectTransform>());
        for (int i =0; i < myTiles.Count; i++)
        {
            myTiles[i].ResetStartPosition();
        }

        GameManager.instance.UpdateUI();


    }
    // Update is called once per frame
    void Update()
    {
        panel.GetComponent<HorizontalLayoutGroup>().enabled = false;
    }

    public void UpdateAllTiles()
    {
        for (int i = 0; i < myTiles.Count; i++)
        {

            myTiles[i].UpdateTile();

        }
    }
}
