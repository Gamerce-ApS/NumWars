using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaticTile : MonoBehaviour
{
    public enum TileType
    {
        EmptyTile,
        StartTile,
        NormalTile,
        MultiplierX2,
        MultiplierX3,
        MultiplierX4,
        AdditionTile,
        DivisionTile,
        MultiplicationTile,
        SubtractionTile
    };

    public TileType myTileType = TileType.EmptyTile;
    public Vector2 BoardPosition;
    public GameObject _child=null;
    public int Number = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public float GetValue()
    {
        return Number;
    }
    public int GetScoreMultiplier(int aScore)
    {
        if(myTileType == TileType.MultiplierX2)
            return aScore*2;
        if (myTileType == TileType.MultiplierX3)
            return aScore*3;
        if (myTileType == TileType.MultiplierX4)
            return aScore*4;
        if (myTileType == TileType.MultiplicationTile || myTileType == TileType.SubtractionTile || myTileType == TileType.AdditionTile|| myTileType == TileType.DivisionTile)
            return aScore+10;



        return aScore;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetValue(int aNumber)
    {
        Debug.Log("SetNumber" + aNumber);
        Number = aNumber;
    }
    public void SetTile(TileType aType, int aNumber)
    {
        if (_child != null)
            Destroy(_child);

        Number = aNumber;

        myTileType = aType;

        _child = (GameObject)GameObject.Instantiate(ResourceManager.instance.TilePrefabList[(int)aType], transform);
        _child.transform.position = transform.position;
        _child.SetActive(true);

        if(aType == TileType.StartTile || aType == TileType.NormalTile)
        _child.transform.GetChild(0).GetComponent<Text>().text = aNumber.ToString();

    }
}
