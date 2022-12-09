using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoubleEventSystemCheck : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    bool firstFrame = true;
    // Update is called once per frame
    void Update()
    {
        if(firstFrame)
        {
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());

            firstFrame = false;
        }

    }
}
