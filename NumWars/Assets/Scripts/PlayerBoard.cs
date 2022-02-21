using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBoard : MonoBehaviour
{
    public GameObject TileObject;
    public GameObject panel;
    public GameObject panelAi;
    public GameObject MaskedParent;
    public static PlayerBoard instance;




    public Player myPlayer;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;



        //AddNewPlayerTiles();
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
    public void Init(Player aPlayer)
    {
        myPlayer = aPlayer;


  

        GameManager.instance.UpdateUI();
        RefreshLayout();

        // AddNewPlayerTiles();
    }
    public void RefreshLayout()
    {
        panel.GetComponent<HorizontalLayoutGroup>().enabled = true;

        myPlayer.myTiles.Sort(HelperFunctions.SortByScore);
        for (int i = 0; i < myPlayer.myTiles.Count; i++)
        {
            myPlayer.myTiles[i].transform.SetAsFirstSibling();
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(PlayerBoard.instance.panel.GetComponent<RectTransform>());
        for (int i = 0; i < myPlayer.myTiles.Count; i++)
        {
            myPlayer.myTiles[i].ResetStartPosition();
        }
    }
    //public void AddNewPlayerTiles()
    //{
    //    panel.GetComponent<HorizontalLayoutGroup>().enabled = true;
    //    for (int i = myPlayer.myTiles.Count; i< 6;i++)
    //    {
    //        GameObject go = GameObject.Instantiate(TileObject, panel.transform);
    //        myPlayer.myTiles.Add(go.GetComponent<Tile>());
    //        go.GetComponent<Tile>().Init(Board.instance.AllTilesNumbers[0]);
    //        Board.instance.AllTilesNumbers.RemoveAt(0);
    //    }
    //    myPlayer.myTiles.Sort(HelperFunctions.SortByScore);
    //    for (int i = 0; i < myPlayer.myTiles.Count; i++)
    //    {
    //        myPlayer.myTiles[i].transform.SetAsFirstSibling();
    //    }
    //    LayoutRebuilder.ForceRebuildLayoutImmediate(panel.GetComponent<RectTransform>());
    //    for (int i =0; i < myPlayer.myTiles.Count; i++)
    //    {
    //        myPlayer.myTiles[i].ResetStartPosition();
    //    }

    //    GameManager.instance.UpdateUI();


    //}
    // Update is called once per frame
    void Update()
    {
        panel.GetComponent<HorizontalLayoutGroup>().enabled = false;
    }

    public void UpdateAllTiles()
    {
        for (int i = 0; i < myPlayer.myTiles.Count; i++)
        {

            myPlayer.myTiles[i].UpdateTile();

        }
    }
}
