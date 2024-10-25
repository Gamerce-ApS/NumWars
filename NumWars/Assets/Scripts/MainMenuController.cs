﻿using System.Collections;
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
using VoxelBusters.EssentialKit;
using VoxelBusters.CoreLibrary;
using GameAnalyticsSDK;
using UnityEngine.EventSystems;
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif
public class MainMenuController : MonoBehaviour
{

    public Text _Name;
    public Text _Thropies;
    public Transform _GameListParent;
    public Transform _GameListParent_updating;
    public GameObject NewGameWindow;
    public Vector3 _TextFlyInBoxoriginalPos;

    public static MainMenuController instance=null;

    public InputField setNameTextLabel;
    public GameObject SetNameGO;

    public Text nameSettingTextError;
    public Image ProfilePicture;
    public Image ProfilePicture2;
    public GameObject FacebookButton;
    public GameObject AppleButton;


    public GameObject FriendsWindow;


    public Button OnlinePlay;
    public Button FriendPlay;
    public Button PraticePlay;

    public GameObject ProfileWindow;

    public GameObject MerginWindow;

    public GameObject UpdateWindow;

    public Text onlineGamesLabel;

    public GameObject adv1;
    public GameObject adv2;
    public GameObject adv3;
    public GameObject adv4;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        _TextFlyInBoxoriginalPos = NewGameWindow.transform.GetChild(1).transform.position;


        SetBoardLayout(PlayerPrefs.GetInt("BoardLayout", 0));
        UpdateTimer = 0;

        buttonRefreshIMG = buttonRefresh.GetComponent<Image>();
    }

    public float UpdateTimer = 0;
    public UnityEngine.Video.VideoPlayer videoPlayer;
    // Update is called once per frame
    void Update()
    {
        if (LoadingOverlay.instance.LoadingCall.Count > 0)
        {
            if(buttonRefreshIMG.enabled)
                buttonRefreshIMG.enabled = (false);
            if(!spinningIcon.activeSelf)
             spinningIcon.SetActive(true);
        }
        else
        {
            if(!buttonRefreshIMG.enabled)
                buttonRefreshIMG.enabled =(true);

            if (spinningIcon.activeSelf)
                spinningIcon.SetActive(false);
        }


            if (SetNameGO.activeSelf ||
            NewGameWindow.activeSelf||
            Startup._instance.DontAutoRefresh)
        {
            UpdateTimer = 0;
            return;
        }

        UpdateTimer += Time.deltaTime;

        if(UpdateTimer>60)
        {
            if(LoadingOverlay.instance.LoadingCall.Count<=0)
                Startup._instance.Refresh();
            UpdateTimer = 0;
        }


        if(videoPlayer.isPlaying)
        {
            if (Input.GetMouseButtonUp(0))
            {

                GameObject.Find("EventSystem").GetComponent<EventSystem>().enabled = true;
                videoPlayer.Stop();

                PlayerPrefs.SetInt("Music", 1);
                Startup._instance.GetComponent<AudioSource>().volume = 0.3f;
            }

        }

    }
    public void PlayHelpVideo()
    {
        videoPlayer.Play();
        videoPlayer.loopPointReached += EndReached;
        GameObject.Find("EventSystem").GetComponent<EventSystem>().enabled = false;
        GameAnalytics.NewDesignEvent("WatchHelpVideo");

            PlayerPrefs.SetInt("Music", 0);
            Startup._instance.GetComponent<AudioSource>().volume = 0;


        }
    void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        GameObject.Find("EventSystem").GetComponent<EventSystem>().enabled = true;
    }
    public void PressPlayOnline()
    {
        GameAnalytics.NewDesignEvent("PressPlayOnline");

        Startup._instance.PlaySoundEffect(0);
        PressCloseNewGameWindow();


        PlayfabHelperFunctions.instance.InitiateSearchForGame();

        //if (Startup._instance.GetHasActiveGameSearch()) // if you have a searching entry you need to wait
        //{
    
        //    return;
        //}
        //else
        //{
        //    Startup._instance.JoinRandomRoom();
        //}

    }
    public void PressPolicy()
    {
        Application.OpenURL("https://outnumber.me/privacy.html");
    }
    public void PressUpdate()
    {
#if UNITY_ANDROID
        Application.OpenURL(Startup._instance.StaticServerData["UpdateLink_android"]);

#else
        Application.OpenURL(Startup._instance.StaticServerData["UpdateLink"]);

#endif
    }
    public void PressPlayAI()
    {
        Startup._instance.PlaySoundEffect(0);
    }
    public void PressPlayPratice()
    {
        GameAnalytics.NewDesignEvent("PressPlayPratice");

        Startup._instance.PlaySoundEffect(0);
        Startup._instance.GameToLoad = null;
        SceneManager.LoadScene(1);
    }
    public void PressPlayTutorial()
    {
        GameAnalytics.NewDesignEvent("PressPlayTutorial");

        Startup._instance.PlaySoundEffect(0);
        if (!PlayerPrefs.HasKey("HasDoneTutorial"))
            Startup._instance.AddXP(85);


        PlayerPrefs.SetInt("HasDoneTutorial", 1);
        Startup._instance.GameToLoad = null;
        Startup._instance.isTutorialGame = true;
        SceneManager.LoadScene(1);


    }
    public void PressOpenNewGameWindow()
    {
        GameAnalytics.NewDesignEvent("PressOpenGameWindow");

        Startup._instance.PlaySoundEffect(0);
        NewGameWindow.SetActive(true);
        NewGameWindow.transform.GetChild(0).GetComponent<Image>().DOFade(157f / 255f,0 ).SetEase(Ease.InOutQuart);

        NewGameWindow.transform.GetChild(1).transform.position = new Vector3(_TextFlyInBoxoriginalPos.x-10, _TextFlyInBoxoriginalPos.y, _TextFlyInBoxoriginalPos.z);
        NewGameWindow.transform.GetChild(1).transform.DOMoveX(_TextFlyInBoxoriginalPos.x, 0.3f).SetEase(Ease.InOutQuart);


        int hasDoneTut = PlayerPrefs.GetInt("HasDoneTutorial",0);
        if(hasDoneTut==1)
        {
            OnlinePlay.interactable = true;
            FriendPlay.interactable = true;
            PraticePlay.interactable = true;
        }
        else
        {
            OnlinePlay.interactable = false;
            FriendPlay.interactable = false;
            PraticePlay.interactable = false;
        }

        //var timeTrigger = new iOSNotificationTimeIntervalTrigger
        //{
        //    TimeInterval = new System.TimeSpan(0, 1, 0),
        //    Repeats = false
        //};

        //var notification = new iOSNotification()
        //{
        //    // You can specify a custom identifier which can be used to manage the notification later.
        //    // If you don't provide one, a unique string will be generated automatically.
        //    Identifier = "_notification_01",
        //    Title = "Outnumber:",
        //    Body = "You have a game that is about to expire!",
        //    Subtitle = "",
        //    ShowInForeground = true,
        //    ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
        //    CategoryIdentifier = "category_a",
        //    ThreadIdentifier = "thread1",
        //    Trigger = timeTrigger,
        //};
        //Debug.Log("Trigger 9 min");
        //iOSNotificationCenter.ScheduleNotification(notification);


    }
public void PressOpenFriendsWindow()
    {
        GameAnalytics.NewDesignEvent("PressOpenFriendsWindow");

        Startup._instance.PlaySoundEffect(0);
        PressCloseNewGameWindow();
        FriendsWindow.SetActive(true);

    }
    public void PressCloseNewGameWindow()
    {
        Startup._instance.PlaySoundEffect(0);
        NewGameWindow.transform.GetChild(0).GetComponent<Image>().DOFade(0, 157f / 255f).SetEase(Ease.InOutQuart);
        NewGameWindow.transform.GetChild(1).transform.DOMoveX(_TextFlyInBoxoriginalPos.x + 10, 0.3f).SetEase(Ease.InOutQuart).OnComplete( ()=> { NewGameWindow.SetActive(false); } );
    }
    public void CloseSetNameWindow()
    {
        Startup._instance.PlaySoundEffect(0);
        SetNameGO.SetActive(false);

    }
    public GameObject SetNameCloseButton;
    public void OpenSetNameWidnow(bool isAnewAccount = false)
    {
        Startup._instance.PlaySoundEffect(0);
        SetNameGO.SetActive(true);
        

        if (isAnewAccount)
        {
            // setNameTextLabel.text = "What's your name?";
            FacebookButton.SetActive(true);
            SetNameCloseButton.SetActive(false);
            AppleButton.SetActive(true);
        }
        else
        {
            SetNameCloseButton.SetActive(true);
            setNameTextLabel.text = _Name.text;
            FacebookButton.SetActive(false);
            AppleButton.SetActive(false);

        }
#if UNITY_ANDROID
        AppleButton.SetActive(false);

#endif
    }
    public void OpenProfileWindow()
    {
        Startup._instance.PlaySoundEffect(0);
        ProfileWindow.SetActive(true);


       
    }
    public void ClickSetName()
    {
        Startup._instance.PlaySoundEffect(0);
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


    }
    public void DeleteAccountYes()
    {

        PlayfabHelperFunctions.instance.FacebookUnLink();
        PlayfabHelperFunctions.instance.AppleUnLink();
        PlayFabClientAPI.UnlinkIOSDeviceID(new UnlinkIOSDeviceIDRequest { }, OnUnlinkediOS, null);

    }
    private void OnUnlinkediOS(UnlinkIOSDeviceIDResult res)
    {

        Startup._instance.PlaySoundEffect(0);
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(0);
        Startup._instance.Refresh(0.1f);
    }
    public void ClickLoginWithFacebook()
    {
        Startup._instance.PlaySoundEffect(0);
        PlayfabHelperFunctions.instance.FacebookLink();
    }
    public void ClickLoginWithApple()
    {
        Startup._instance.PlaySoundEffect(0);
        PlayfabHelperFunctions.instance.AppleLink();
    }
    public void ShowMerginAlert()
    {
        MerginWindow.SetActive(true);
    }
    public void ClickRecoveAccount()
    {
        Startup._instance.PlaySoundEffect(0);
        PlayerPrefs.SetInt("FacebookLink", 1);
        PlayerPrefs.SetInt("HasDoneTutorial", 1);
        SceneManager.LoadScene(0);
        // Startup._instance.Refresh(0.1f);
        PlayfabHelperFunctions.instance.ReLogin();

    }
    public void IgnoreOldAccountAndLinkNew()
    {
        Startup._instance.PlaySoundEffect(0);
        LoadingOverlay.instance.ShowLoading("Facebook login");


        PlayFabClientAPI.LinkFacebookAccount(new LinkFacebookAccountRequest { AccessToken = AccessToken.CurrentAccessToken.TokenString,ForceLink = true }, OnCompleteForceLink, OnPlayfabFacebookAuthFailed);


    }

    private void OnPlayfabFacebookAuthFailed(PlayFabError error)
    {
        LoadingOverlay.instance.DoneLoading("Facebook login");
        Debug.Log("PlayFab Facebook Auth Failed: " + error.GenerateErrorReport());
    }

    private void OnCompleteForceLink(LinkFacebookAccountResult result)
    {
        PlayerPrefs.SetInt("FacebookLink", 1);
        PlayerPrefs.SetInt("HasDoneTutorial", 1);
        SceneManager.LoadScene(0);
        // Startup._instance.Refresh(0.1f);
        PlayfabHelperFunctions.instance.ReLogin();
    }

    public void ClickUnlinkWithFacebook()
    {
        Startup._instance.PlaySoundEffect(0);
        PlayfabHelperFunctions.instance.FacebookUnLink();
    }
    public GameObject LinkFBButton;
    public GameObject UnlinkFBButton;

    public GameObject LinkAppleButton;
    public GameObject UnlinkAppleButton;

    public void SetFBLinked(bool isLinked)
    {
        if (UnlinkFBButton == null)
            return;
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
    public void SetAppleLinked(bool isLinked)
    {
        if (isLinked)
        {
            UnlinkAppleButton.SetActive(true);

            LinkAppleButton.SetActive(false);
        }
        else
        {
            UnlinkAppleButton.SetActive(false);

            LinkAppleButton.SetActive(true);
        }
#if UNITY_ANDROID
        AppleButton.SetActive(false);
        UnlinkAppleButton.SetActive(false);
        LinkAppleButton.SetActive(false);

#endif
    }
    public void Share()
    {
        GameAnalytics.NewDesignEvent("ClickShare");


        Startup._instance.PlaySoundEffect(0);
        ShareSheet shareSheet = ShareSheet.CreateInstance();
        shareSheet.AddText("Hey, test out Outnumber on appstore or google play! It's a great game!");
        VoxelBusters.CoreLibrary.URLString url = new VoxelBusters.CoreLibrary.URLString();

        shareSheet.AddURL(URLString.URLWithPath("https://itunes.apple.com/us/app/keynote/id1610303402?mt=8"));

        shareSheet.SetCompletionCallback((result, error) => {
            Debug.Log("Share Sheet was closed. Result code: " + result.ResultCode);
        });
        shareSheet.Show();


    }

    //public void GlobalReset()
    //{
    //    GameObject[] go = FindObjectsOfType<GameObject>();
    //    for(int i = 0; i< go.Length;i++)
    //    {
    //        if(go[i].name != "CallbackDispatcher" && go[i].name != "EssentialKitManager" && go[i].name != "UnityFacebookSDKPlugin")
    //        Destroy(go[i]);
    //    }

    //    SceneManager.LoadScene(0);
    //}

    public void OpenLeaderboard()
    {
        Startup._instance.PlaySoundEffect(0);
    }
    public void CloseLeaderboard()
    {
        Startup._instance.PlaySoundEffect(0);
    }
    public void OpenFriends()
    {
        Startup._instance.PlaySoundEffect(0);
    }
    public void CloseFriends()
    {
        Startup._instance.PlaySoundEffect(0);
    }
    public GameObject infoWidnow;
    public void OpenInformation()
    {
        infoWidnow.SetActive(true);
    }
    public void OpenSettings()
    {
        GameAnalytics.NewDesignEvent("PressOpenSettingsWindow");

        Startup._instance.PlaySoundEffect(0);


        if (PlayerPrefs.GetInt("Music", 1) ==1)
            MusicText.text = "Music: ON";
        else
            MusicText.text = "Music: OFF";



        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            SoundText.text = "Sound: ON";
        }
        else
        {
            SoundText.text = "Sound: OFF";
        }


        bool isLinked = false;
        UserFacebookInfo info = null;
        if(Startup._instance.UserAccount != null)
            info =Startup._instance.UserAccount.FacebookInfo;
        if (info != null && info.FacebookId != null && info.FacebookId.Length > 0)
        {
      
            MainMenuController.instance.SetFBLinked(true);

        }
        else
            MainMenuController.instance.SetFBLinked(false);

      

    }
    public void CloseSettings()
    {
        Startup._instance.PlaySoundEffect(0);
    }
    public Text MusicText;
    public void ToggleMusic()
    {

        if (Startup._instance.GetComponent<AudioSource>().volume == 0.3f)
        {
            PlayerPrefs.SetInt("Music", 0);
            Startup._instance.GetComponent<AudioSource>().volume = 0;
        }
   
        else
        {
            PlayerPrefs.SetInt("Music", 1);
            Startup._instance.GetComponent<AudioSource>().volume = 0.3f;
        }
           




        if (Startup._instance.GetComponent<AudioSource>().volume == 0.3f)
            MusicText.text = "Music: ON";
        else
            MusicText.text = "Music: OFF";
    }
   
    public Text SoundText;
    public void ToggleSound()
    {
        int sound = PlayerPrefs.GetInt("Sound",1);

       // Startup._instance.GetComponent<AudioSource>().enabled = !Startup._instance.GetComponent<AudioSource>().enabled;

        if (sound == 1)
        {
            SoundText.text = "Sound: OFF";
            PlayerPrefs.SetInt("Sound", 0);
        }
        else
        {
            PlayerPrefs.SetInt("Sound", 1);
            SoundText.text = "Sound: ON";
        }

    }
    public Image LayoutButton1;
    public Image LayoutButton2;

    public Image LayoutButton1_challenge;
    public Image LayoutButton2_challenge;

    public void SetBoardLayout(int aVersion)
    {
        PlayerPrefs.SetInt("BoardLayout", aVersion);
        Color col;
        ColorUtility.TryParseHtmlString("#ADFCFD", out col);


        if (aVersion == 0)
        {
            // Standard
            LayoutButton1.color = Color.white;
            LayoutButton2.color = col;

            LayoutButton1_challenge.color = Color.white;
            LayoutButton2_challenge.color = col;
        }
        if (aVersion == 1)
        {
            // Random
            LayoutButton2.color = Color.white;
            LayoutButton1.color = col;

            LayoutButton2_challenge.color = Color.white;
            LayoutButton1_challenge.color = col;
        }

    }

    public void Contactus()
    {
      

            string email = "info@gamerce.net";

            string subject = MyEscapeURL("Feedback / Ideas for Outnumber");

            string body = MyEscapeURL("Hey! I love the game but have some comments/ideas/feedback:");


            Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);

        

        
    }
    string MyEscapeURL(string url)

    {

        return WWW.EscapeURL(url).Replace("+", "%20");

    }
    public GameObject buttonRefresh;
    public GameObject spinningIcon;
    public Image buttonRefreshIMG;


    public void RefreshList()
    {
        if (LoadingOverlay.instance.LoadingCall.Count <= 0)
            Startup._instance.Refresh();

        buttonRefresh.GetComponent<Image>().enabled = (false);
        spinningIcon.SetActive(true);

    }
    public Text DebugText;
    public void TogglweDebug()
    {
        Startup.DEBUG_TOOLS = !Startup.DEBUG_TOOLS;
        if(Startup.DEBUG_TOOLS)
        {
            PlayerPrefs.SetInt("DebugMode", 1);
            DebugText.text = "Debug: ON";
            Startup.instance.GetComponent<Logger>().enabled = true;
        }

       else
        {
            PlayerPrefs.SetInt("DebugMode", 0);
            DebugText.text = "Debug: OFF";

        }


    }
    public void ShowLogs()
    {
        Logger.ourInstance.ShowLogs();
    }
    int aTest = 0;
    public void ToggleAdvanced()
    {
        bool toggle = !adv2.activeSelf;


            adv2.SetActive(toggle);
        adv3.SetActive(toggle);
        
        aTest++;
        if (aTest>4)
        {
            adv1.SetActive(toggle);
            adv4.SetActive(toggle);
            aTest = 0;

        }
        else
        {
            adv1.SetActive(false);
            adv4.SetActive(false);

        }


    }
    public void SyncData()
    {
        PlayerPrefs.DeleteKey("OldGames");
        SceneManager.LoadScene(0);
        if (LoadingOverlay.instance != null)
            LoadingOverlay.instance.ShowLoadingFullscreen("Updating..");
        Startup._instance.Refresh(0.1f);

    }
}
