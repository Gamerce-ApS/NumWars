using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Facebook.Unity;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using LoginResult = PlayFab.ClientModels.LoginResult;

public class MainMenuController : MonoBehaviour
{

    public Text _Name;
    public Text _Thropies;
    public Transform _GameListParent;
    public GameObject NewGameWindow;
    public Vector3 _TextFlyInBoxoriginalPos;

    public static MainMenuController instance;

    public InputField setNameTextLabel;
    public GameObject SetNameGO;

    public Text nameSettingTextError;
    public Image ProfilePicture;



    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        _TextFlyInBoxoriginalPos = NewGameWindow.transform.GetChild(1).transform.position;
    }

    float UpdateTimer = 0;
    // Update is called once per frame
    void Update()
    {
    


        if (SetNameGO.activeSelf ||
            NewGameWindow.activeSelf)
        {
            UpdateTimer = 0;
            return;
        }

        UpdateTimer += Time.deltaTime;

        if(UpdateTimer>10)
        {
            Startup._instance.Refresh();
            UpdateTimer = 0;
        }
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
            Startup._instance.JoinRandomRoom();
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
    public void OpenSetNameWidnow(bool isAnewAccount = false)
    {
        SetNameGO.SetActive(true);
        

        if (isAnewAccount)
            setNameTextLabel.text = "What's your name?";
        else
        {
            setNameTextLabel.text = _Name.text;
        }
    }
    public void ClickSetName()
    {
        if (setNameTextLabel.text == "What's your name?" || setNameTextLabel.text.Length<3)
        {
            nameSettingTextError.text = "Invalid name";
            return;
        }


        if(setNameTextLabel.text == _Name.text)
        {
            SetNameGO.SetActive(false);
            return;

        }
    //    SetNameGO.SetActive(false);
        PlayfabHelperFunctions.instance.UpdateDisplayName(setNameTextLabel.text);
    }

    public void ClearData()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(0);
    }

    public void ClickLoginWithFacebook()
    {
        PlayfabHelperFunctions.instance.FacebookLink();
    }
    public void ClickUnlinkWithFacebook()
    {
        PlayfabHelperFunctions.instance.FacebookUnLink();
    }
    public GameObject LinkFBButton;
    public GameObject UnlinkFBButton;

    public void SetFBLinked(bool isLinked)
    {
        if( isLinked )
        {
            UnlinkFBButton.SetActive(true);

            LinkFBButton.SetActive(false);
        }else
        {
            UnlinkFBButton.SetActive(false);

            LinkFBButton.SetActive(true);
        }

    }

}
