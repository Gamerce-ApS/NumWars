﻿using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{

    public Text _Name;
    public Text _Thropies;
    public Transform _GameListParent;
    public GameObject NewGameWindow;
    public Vector3 _TextFlyInBoxoriginalPos;

    public static MainMenuController instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        _TextFlyInBoxoriginalPos = NewGameWindow.transform.GetChild(1).transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PressPlayOnline()
    {
        PressCloseNewGameWindow();
        if (Startup._instance.GetHasActiveGameSearch()) // if you have a searching entry you need to wait
        {
    
            return;
        }
        else
        {
            LoadingOverlay.instance.ShowLoading("JoinRandomRoom");

            PhotonNetwork.JoinRandomRoom(); // Joina random or create a new room and shared data entry
        }

    }
    public void PressPlayAI()
    {

    }
    public void PressPlayPratice()
    {
        Startup._instance.GameToLoad = null;
        SceneManager.LoadScene(1);
    }
    public void PressOpenNewGameWindow()
    {
        NewGameWindow.SetActive(true);
        NewGameWindow.transform.GetChild(0).GetComponent<Image>().DOFade(157f / 255f,0 ).SetEase(Ease.InOutQuart);

        NewGameWindow.transform.GetChild(1).transform.position = new Vector3(_TextFlyInBoxoriginalPos.x-10, _TextFlyInBoxoriginalPos.y, _TextFlyInBoxoriginalPos.z);
        NewGameWindow.transform.GetChild(1).transform.DOMoveX(_TextFlyInBoxoriginalPos.x, 0.3f).SetEase(Ease.InOutQuart);

    }
    public void PressCloseNewGameWindow()
    {
        NewGameWindow.transform.GetChild(0).GetComponent<Image>().DOFade(0, 157f / 255f).SetEase(Ease.InOutQuart);
        NewGameWindow.transform.GetChild(1).transform.DOMoveX(_TextFlyInBoxoriginalPos.x + 10, 0.3f).SetEase(Ease.InOutQuart).OnComplete( ()=> { NewGameWindow.SetActive(false); } );
    }
}