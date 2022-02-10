using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingOverlay : MonoBehaviour
{
    private static LoadingOverlay _instance=null;
    public static LoadingOverlay instance
    {
        get
        {
            
            return FindObjectOfType<LoadingOverlay>();
           
        }
    }

    public List<string> LoadingCall = new List<string>();
    public Text log;
    float timer = -1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(timer != -1)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                timer = -1;
                transform.GetChild(0).gameObject.SetActive(false);

            }
        }
    
    }

    public void ShowLoading(string function)
    {
        timer = -1;
        transform.GetChild(0).gameObject.SetActive(true);
        LoadingCall.Add(function);
        log.text = "Start:" + function;
    }
    public void DoneLoading(string aFunction)
    {
        LoadingCall.Remove(aFunction);
        if(LoadingCall.Count<=0)
        {
            timer = 0.50f;
        }
        log.text = "Done:" + aFunction;
    }

}
