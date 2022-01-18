using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public GameObject StartTile;
    public GameObject NormalTile;

    public List<GameObject> TilePrefabList = new List<GameObject>();

    public static ResourceManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
