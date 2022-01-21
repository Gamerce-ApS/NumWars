using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public GameObject StartTile;
    public GameObject NormalTile;

    public List<GameObject> TilePrefabList = new List<GameObject>();

    public static ResourceManager _instance = null;
    public static ResourceManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameManager.FindObjectOfType<ResourceManager>();
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
