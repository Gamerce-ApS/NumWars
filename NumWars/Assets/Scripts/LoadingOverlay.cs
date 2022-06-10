using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public GameObject LoadingGOList;
    public GameObject badConnection;
    public float laodingTimer = 0;

    float targetAlpha = 0;
    
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
                //transform.GetChild(0).gameObject.SetActive(false);
                LoadingGOList.SetActive(false);
                targetAlpha = 0;

            }
        }


        if (LoadingCall.Count > 0)
            laodingTimer += Time.deltaTime;
        else
            laodingTimer = 0;

        if(laodingTimer > 7)
        {
            badConnection.SetActive(true);
        }
        else
        {
            if(badConnection.activeSelf)
                badConnection.SetActive(false);
        }


        transform.GetChild(0).GetComponent<CanvasGroup>().alpha = Mathf.Lerp(transform.GetChild(0).GetComponent<CanvasGroup>().alpha, targetAlpha, Time.deltaTime * 3);
        if(transform.GetChild(0).GetComponent<CanvasGroup>().alpha <= 0.05)
        {
            transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0;
            transform.GetChild(0).gameObject.SetActive(false);
        }

    }
    public void ClickRetry()
    {
        LoadingCall.Clear();
        PlayfabHelperFunctions.instance.ReLogin();
        SceneManager.LoadScene(0);
        Startup._instance.Refresh(0.1f);
    }
    public void ShowLoading(string function)
    {
        timer = -1;
     //   transform.GetChild(0).gameObject.SetActive(true);
     //   LoadingGOList.SetActive(true);
        LoadingCall.Add(function);
       // log.text = "Start:" + function;

        float loadingTime = ((float)LoadingCall.Count / 15f);
        if (loadingTime > 1)
            loadingTime = 1;
        log.text = (100 * loadingTime).ToString("F0") + "%";
    }
    public void DoneLoading(string aFunction)
    {
        LoadingCall.Remove(aFunction);
        if(LoadingCall.Count<=0)
        {
            timer = 0.50f;
        }
     //   log.text = "Done:" + aFunction;
    }
    public void ShowLoadingFullscreen(string function)
    {
        timer = -1;
          transform.GetChild(0).gameObject.SetActive(true);

        transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 1;
        targetAlpha = 1;
        LoadingGOList.SetActive(true);
        LoadingCall.Add(function);
        //log.text = "Start:" + function;

        float loadingTime = ((float)LoadingCall.Count / 15f);
        if (loadingTime > 1)
            loadingTime = 1;
        log.text = (100 * loadingTime).ToString("F0") + "%";

    }

}
