using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using DG.Tweening;
//using GameAnalyticsSDK;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using PlayFab.DataModels;
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using AppodealAds.Unity.Api;
//using AppodealAds.Unity.Common;



public class Startup : MonoBehaviourPunCallbacks
{
    public Sprite EmptyProfilePicture;
    public PlayFab.ClientModels.PlayerProfileModel PlayerProfile;
    public UserAccountInfo UserAccount;
    

    public Dictionary<string, UserDataRecord> myData;
    public Dictionary<string, string> StaticServerData;


    public string displayName = "";
    public string MyPlayfabID = "";
    public string avatarURL = "";

    public GameObject SearchingForGameObject=null;

    public List<BoardData> openGamesList = new List<BoardData>();


    PlayfabHelperFunctions _PlayfabHelperFunctions;

    public static bool DEBUG_TOOLS = false;

    public bool isTutorialGame = false;
    public BoardData GameToLoad = null;

    public int CurrentCalls = 0;

    public PlayFab.Json.JsonObject StoredAvatarURLS =null;

    //public static Startup _instance=null;
    public static Startup _instance = null;
    public static Startup instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.Find("Startup").GetComponent<Startup>();
            return _instance;
        }
    }

    public AchivmentController myAchivmentController;
    public bool DontAutoRefresh = true;

    public static long TIMEOUT = 60 * 60 * 24 * 2;

    public static string LIVE_VERSION = "11";

    // public static long TIMEOUT = 60+60+60;

    public bool isFake;
    public GameObject ConfetiPart;


    public static bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        // Implement your own certificate validation here
        return true;
        //return IsCertificateValid(certificate, chain, sslPolicyErrors);
    }
#if UNITY_ANDROID
    Firebase.FirebaseApp app;
#endif
    // Start is called before the first frame update
    void Start()
    {



#if UNITY_ANDROID
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.

                Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
                Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });



#endif


        if (PlayerPrefs.GetInt("DebugMode", 0) == 1)
            DEBUG_TOOLS = true;
        
        float aspect = (float)Screen.height / (float)Screen.width;
        Debug.Log("aspect:" + aspect);
        if (aspect < 1.4f)
        {
            GameObject.Find("Canvas").GetComponent<CanvasScaler>().referenceResolution = new Vector2(1100, 600);
        }


        Debug.Log("Start");
        limitFpsTimer = 10;
        //Screen.SetResolution(Screen.width/2, Screen.height / 2, true);


       // PlayFab.Internal.PlayFabWebRequest.CustomCertValidationHook = ValidateCertificate;
       PlayFab.Internal.PlayFabWebRequest.SkipCertificateValidation();
       //    PlayFab.Internal.PlayFabWebRequest.CustomCertValidationHook

        //  QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        // When the Menu starts, set the rendering to target 20fps
        OnDemandRendering.renderFrameInterval = 1;


#if UNITY_IOS
        UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
        UnityEngine.iOS.NotificationServices.ClearRemoteNotifications();
#endif



     //   GameAnalytics.Initialize();
        //Debug.Log("InitAppodeal");
        //Appodeal.initialize("91f0aae11c6d5b4fe09000ad17edf290d41803497b6ff82f", Appodeal.INTERSTITIAL | Appodeal.REWARDED_VIDEO, true);


        Application.runInBackground = false;
        GameToLoad = null;
        DontAutoRefresh = true;
        myAchivmentController = new AchivmentController();


        if (Startup._instance != null)
        {
            
            Destroy(gameObject);
            return;
        }


        DontDestroyOnLoad(gameObject);
        _instance = this;

        if (isFake)
            return;

        _PlayfabHelperFunctions = gameObject.GetComponent<PlayfabHelperFunctions>();
        Board.instance.Init();

        if (LoadingOverlay.instance != null)
            LoadingOverlay.instance.ShowLoadingFullscreen("Connecting to photon");

        if (PhotonNetwork.IsConnected)
        {
            if (LoadingOverlay.instance != null)
                LoadingOverlay.instance.DoneLoading("Connecting to photon");

            timer = -1;
            if (LoadingOverlay.instance != null)
                LoadingOverlay.instance.ShowLoadingFullscreen("Updating..");
            _PlayfabHelperFunctions.Login();

        }
        else{
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = "1";
        }



#if UNITY_IOS

        StartCoroutine(RequestAuthorization());
       // UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Badge | UnityEngine.iOS.NotificationType.Sound, true);
#endif


 


        if (PlayerPrefs.GetInt("Music", 1) == 0)
        {

            Startup._instance.GetComponent<AudioSource>().volume = 0;
        }
        else
            Startup._instance.GetComponent<AudioSource>().volume = 0.3f;



    }

    string pushToken = "";

#if UNITY_IOS
    IEnumerator RequestAuthorization()
    {

        var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;
        using (var req = new AuthorizationRequest(authorizationOption, true))
        {
            while (!req.IsFinished)
            {
                yield return null;
            };

            string res = "\n RequestAuthorization:";
            res += "\n finished: " + req.IsFinished;
            res += "\n granted :  " + req.Granted;
            res += "\n error:  " + req.Error;
            res += "\n deviceToken:  " + req.DeviceToken;
            pushToken = req.DeviceToken;
            Debug.Log(res);

            
        }

}
#endif


#if UNITY_ANDROID

    private void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        Debug.Log("PlayFab: Received Registration Token: " + token.Token);
        pushToken = token.Token;
        RegisterForPush();
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
    }
    public void RegisterForPush()
    {
        if (string.IsNullOrEmpty(pushToken) )
            return;

        var request = new AndroidDevicePushNotificationRegistrationRequest
        {
            DeviceToken = pushToken,
            SendPushNotificationConfirmation = true,
            ConfirmationMessage = "Push notifications registered successfully"
        };
        PlayFabClientAPI.AndroidDevicePushNotificationRegistration(request, OnPfAndroidReg, OnPfFail);

    }
    private void OnPfFail(PlayFabError error)
    {
        Debug.Log("PlayFab: api error: " + error.GenerateErrorReport());
    }
    private void OnPfAndroidReg(AndroidDevicePushNotificationRegistrationResult result)
    {
        Debug.Log("PlayFab: Push Registration Successful");
    }

#endif
    public void StartPushSer()
    {
        StartCoroutine(RegisterPush());
    }
    float timeSinceFocus = 0;
    private void OnApplicationPause(bool pause)
    {
        Pasued(pause);
    }

    //private void OnApplicationFocus(bool focus)
    //{
    //    Pasued(!focus);
    //}

    public void Pasued(bool pauseStatus)
    {








        Debug.Log("Pause:" + pauseStatus);


        if (!pauseStatus)
        {



            //if (timeSinceFocus <= 0.06f)
            //    return;
            //timeSinceFocus = 0;

            if (MainMenuController.instance != null)
                MainMenuController.instance.UpdateTimer = 0;

            //Startup._instance = null;
            //PlayfabHelperFunctions.instance = null;
            //Destroy(gameObject);

            //SceneManager.LoadScene(0);


            limitFpsTimer = 15;
            //PhotonNetwork.Disconnect();
#if UNITY_IOS
            UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
            UnityEngine.iOS.NotificationServices.ClearRemoteNotifications();







#endif
            PlayfabCallbackHandler.instance.CancelAllCalls();
                        if (PhotonNetwork.IsConnected&& PlayFabClientAPI.IsClientLoggedIn() || SceneManager.GetActiveScene().name == "GameScene" && PlayFabClientAPI.IsClientLoggedIn())
                        {
                            if (Startup._instance != null)
                            {
                                if (LoadingOverlay.instance != null)
                                    LoadingOverlay.instance.ShowLoadingFullscreen("Updating..");
                                Startup._instance.Refresh(0.1f);
                            }
                        }
                        else
                        {
                            //if (Startup._instance != null)
                            //{
                            //    LoadingOverlay.instance.ShowLoadingFullscreen("Updating..");
                            //    Startup._instance.Refresh(0.1f);
                            //}

                            if(!PhotonNetwork.IsConnected)
                            {
                                LoadingOverlay.instance.ShowLoadingFullscreen("Connecting to photon");
                                // #Critical, we must first and foremost connect to Photon Online Server.
                                PhotonNetwork.ConnectUsingSettings();
                                PhotonNetwork.GameVersion = "1";
                            }
                            else
                            {
                                timer = -1;
                                _PlayfabHelperFunctions.Login();
                            }


                            //if (Startup._instance != null)
                            //{

                            //    //Startup._instance.Refresh(0.1f);
                            //    //if (Startup._instance.avatarURL != null)
                            //    //    if (Startup._instance.avatarURL.Length > 0)
                            //    //    {
                            //    //        PlayfabHelperFunctions.instance.LoadAvatarURL(Startup._instance.avatarURL);
                            //    //    }

                            //    //if (LoadingOverlay.instance != null)
                            //    //    LoadingOverlay.instance.ShowLoadingFullscreen("Updating..");

                            //    //SceneManager.LoadScene(0);
                            //    //StartCoroutine(StartLoading());
                            //}

                        }

            if(FindObjectOfType<ScrollListBasedOnItems>() != null)
            FindObjectOfType<ScrollListBasedOnItems>().Reset();
        }

    }
    IEnumerator StartLoading()
    {
        yield return new WaitForSeconds(0.05f);
        if (LoadingOverlay.instance != null)
            LoadingOverlay.instance.ShowLoadingFullscreen("Updating..");
    }
    //void OnApplicationFocus(bool hasFocus)
    //{
    //    if (hasFocus)
    //        SceneManager.LoadScene(0);
    //}

    IEnumerator RegisterPush()
    {
        yield return new WaitForSeconds(3);
#if UNITY_IOS
        if(PlayFabClientAPI.IsClientLoggedIn())
        {
            byte[] token = UnityEngine.iOS.NotificationServices.deviceToken;



            if (token != null)
            {
                RegisterForIOSPushNotificationRequest request = new RegisterForIOSPushNotificationRequest();
                if (pushToken.Length <= 0)
                    request.DeviceToken = System.BitConverter.ToString(token).Replace("-", "").ToLower();
                else
                    request.DeviceToken = pushToken.Replace("-", "").ToLower();

                PlayFabClientAPI.RegisterForIOSPushNotification(request, (RegisterForIOSPushNotificationResult result) =>
                {
                    Debug.Log("Push Registration Successful");
                }, error =>
                {
                    Debug.Log("Got error registering Push");


                });
            }
            else
            {
                Debug.Log("Push Token was null!");
            }

        }

#endif



    }
    public void Refresh()
    {
   

        _PlayfabHelperFunctions.Refresh();
    }
    public void Refresh(float aDelay)
    {
        StartCoroutine(DelayRefresh(aDelay));
    }
    public IEnumerator DelayRefresh(float aDelay = 0.5f)
    {
        yield return new WaitForSeconds(aDelay);
        if (PlayFabClientAPI.IsClientLoggedIn())
            Refresh();
        else
        {
            LoadingOverlay.instance.DoneLoading("Updating..");
            SceneManager.LoadScene(0);
            StartCoroutine(StartLoading());
        }
    }
    public void UpdateDisplayName()
    {
        //_PlayfabHelperFunctions.UpdateDisplayName(_txtLabel.text);
    }

    float timer = 0;

    public float limitFpsTimer = 0;
    // Update is called once per frame
    void Update()
    {



   



        //if(Input.GetKeyUp(KeyCode.R))
        //OnApplicationPause(false);


        //if (Input.GetKeyUp(KeyCode.T))
        //    OnApplicationFocus(true);


        if (timeSinceFocus<1)
            timeSinceFocus += Time.deltaTime;
       
        if (Input.GetMouseButton(0) || (Input.touchCount > 0))
        {
            if(SceneManager.GetActiveScene().name == "GameScene")
                limitFpsTimer = 15;
            else
                limitFpsTimer = 8;
            // If the mouse button or touch detected render at 60 FPS (every frame).
            OnDemandRendering.renderFrameInterval = 1;
        }
        else
        {
            limitFpsTimer -= Time.deltaTime;
            // If there is no mouse and no touch input then we can go back to 20 FPS (every 3 frames).
         
        }

        if(limitFpsTimer<=0)
        {
            OnDemandRendering.renderFrameInterval = 3;
        }
        else
            OnDemandRendering.renderFrameInterval = 1;

        if (Input.GetKeyUp(KeyCode.O))
        {
            PlayfabHelperFunctions.instance.GetSharedDataGrouped(Startup.instance.MyPlayfabID);
        }



        //if (timer != -1)
        //if(!PhotonNetwork.IsConnected)
        //{
        //    timer += Time.deltaTime;
        //    if(timer>8)
        //    {
        //        PhotonNetwork.ConnectUsingSettings();
        //        PhotonNetwork.GameVersion = "1";
        //        timer = 0;
        //    }

        //}


        if (Input.GetKeyUp(KeyCode.L))
            LoadGameList(0.1f);




        float current = 0;
        current = current = (int)(1f / Time.unscaledDeltaTime);
        avgFrameRate = (int)current;
        GameObject go = GameObject.Find("FPS_COUNTER");
        if(go != null)
            go.GetComponent<Text>().text = avgFrameRate.ToString() + " FPS";

    }
    int avgFrameRate = 0;

    public void ChangeValueFor(string aEntry,string aValue)
    {
        _PlayfabHelperFunctions.ChangeValueFor(aEntry, aValue);
    }

#region MonoBehaviourPunCallbacks CallBacks
    public override void OnConnectedToMaster()
    {
        if(joinRandomRoom)
        {
            joinRandomRoom = false;
            JoinRandomRoom();
        }
        



        if (LoadingOverlay.instance != null)
        LoadingOverlay.instance.DoneLoading("Connecting to photon");
        // we don't want to do anything if we are not attempting to join a room. 
        // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
        // we don't want to do anything.
        //if (isConnecting)
        {
            Debug.Log("OnConnectedToMaster: Next -> try to Join Random Room");
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");
        }
        timer = -1;
        _PlayfabHelperFunctions.Login();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        LoadingOverlay.instance.DoneLoading("JoinRandomRoom");
        LoadingOverlay.instance.ShowLoading("CreateRoom");
        Debug.Log("<Color=Red>OnJoinRandomFailed</Color>: Next -> Create a new Room");
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        string newRoomName = string.Format("{0}-{1}", displayName+"_"+ MyPlayfabID+"_", Random.Range(0, 1000000).ToString())+ Random.Range(0, 1000000).ToString();    // for int, Random.Range is max-exclusive!
        TypedLobby tl = new TypedLobby("", LobbyType.AsyncRandomLobby);
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 0;
        ro.EmptyRoomTtl = 300000;
        ro.PlayerTtl = int.MaxValue;
        ro.IsVisible = true;
        ro.IsOpen = true;
        ro.CustomRoomPropertiesForLobby = new string[] { MyPlayfabID, displayName, newRoomName, PlayerPrefs.GetInt("BoardLayout", 0).ToString() };
        PhotonNetwork.CreateRoom(newRoomName, ro, tl);
        
    }
    public override void OnCreatedRoom()
    {
        LoadingOverlay.instance.DoneLoading("CreateRoom");
        Debug.Log("<Color=Green>OnCreatedRoom</Color> ");

        _PlayfabHelperFunctions.SetPlayfabCreatedRoom(PhotonNetwork.CurrentRoom.PropertiesListedInLobby[0], PhotonNetwork.CurrentRoom.PropertiesListedInLobby[2]);
        // Created room and added to players playfab list in shared data, now we wait for people to join

    }
    public bool joinRandomRoom = false;
    public void JoinRandomRoom()
    {
        if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
        {
            LoadingOverlay.instance.ShowLoading("JoinRandomRoom");

            PhotonNetwork.JoinRandomRoom(); // Joina random or create a new room and shared data entry
        }
        else
        {
            //   Debug.LogError("Can't join random room now, client is not ready");

            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = "1";

            joinRandomRoom = true;

        }



    }

    public override void OnJoinedRoom()
    {
        LoadingOverlay.instance.DoneLoading("JoinRandomRoom");
        LoadingOverlay.instance.DoneLoading("CreateRoom");
        Debug.Log("<Color=Green>OnJoinedRoom</Color> with " + PhotonNetwork.CurrentRoom.PlayerCount + " Player(s)");
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running.");

        if( PhotonNetwork.CurrentRoom.PropertiesListedInLobby[0] == MyPlayfabID )
        {
            Debug.Log("Joined your own room, waiting for other player");
            if(SearchingForGameObject == null)
            {
                SearchingForGameObject = (GameObject)GameObject.Instantiate(PlayfabHelperFunctions.instance.SearchingForGamePrefab, MainMenuController.instance._GameListParent_updating);
                SearchingForGameObject.transform.SetAsFirstSibling();
                SearchingForGameObject.SetActive(true);
                Vector3 rc = SearchingForGameObject.GetComponent<RectTransform>().localPosition;
                SearchingForGameObject.GetComponent<RectTransform>().localPosition = new Vector3(rc.x, rc.y, 0);

                Startup._instance.SearchingForGameObject.GetComponent<SearchGameInfo>().NameID = PhotonNetwork.CurrentRoom.Name;
            }
            else
            {
                Startup._instance.SearchingForGameObject.SetActive(true);
                SearchingForGameObject.transform.SetAsFirstSibling();
                Vector3 rc = SearchingForGameObject.GetComponent<RectTransform>().localPosition;
                SearchingForGameObject.GetComponent<RectTransform>().localPosition = new Vector3(rc.x, rc.y, 0);
            }
            _PlayfabHelperFunctions.RemoveAbandonedGames();
            FinishedGettingGameListCheckForOpenGames();
        }
        else
        {
            SetPlayfabSecondPlayerInRoom(PhotonNetwork.CurrentRoom.PropertiesListedInLobby[0],MyPlayfabID, PhotonNetwork.CurrentRoom.PropertiesListedInLobby[1], PhotonNetwork.CurrentRoom.PropertiesListedInLobby[2], PhotonNetwork.CurrentRoom.PropertiesListedInLobby[3]);
        }
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        LoadingOverlay.instance.DoneLoading("JoinRandomRoom");

        LoadingOverlay.instance.DoneLoading("CreateRoom");

        for (int i = 0; i < openGamesList.Count; i++)
        {
            if (openGamesList[i].player1_PlayfabId == MyPlayfabID && openGamesList[i].player2_PlayfabId == "")
            {
                PlayfabHelperFunctions.instance.RemoveRoomFromList(openGamesList[i].RoomName, openGamesList[i].GetJson());
                if(Startup._instance.SearchingForGameObject != null)
                Startup._instance.SearchingForGameObject.SetActive(false);
                return;
            }
        }
        Debug.Log("Failed to join open room, need to remove the serach for new game");

        //_PlayfabHelperFunctions.RemoveAbandonedGames();
    }
    public List<BoardData> myOldGameList = new List<BoardData>();
    public void FinishedGettingGameListCheckForOpenGames()
    {
        for (int i = 0; i < openGamesList.Count; i++)
        {
            if (openGamesList[i].player1_PlayfabId == MyPlayfabID && openGamesList[i].player2_PlayfabId == "")
            {
                if(PhotonNetwork.InRoom == false && PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
                {
                    PhotonNetwork.JoinRoom(openGamesList[i].RoomName);
                    return;
                }
                 
            }
        }

        _PlayfabHelperFunctions.RemoveAbandonedGames();

        if(Startup._instance.SearchingForGameObject == null && PhotonNetwork.InRoom == true)
        {
            PhotonNetwork.LeaveRoom();
        }

        if (MainMenuController.instance == null)
            return;

        for(int i = 0; i < MainMenuController.instance._GameListParent_updating.childCount;i++)
        {
            GameListItem it = MainMenuController.instance._GameListParent_updating.GetChild(i).GetComponent<GameListItem>();

            if(it != null)
            {
                string Gname = "";
                if (it.YourTurnGO.activeSelf)
                    Gname = "0_";
                else
                    Gname = "1_";
                Gname += it.bd.RoomName;

                MainMenuController.instance._GameListParent_updating.GetChild(i).name = Gname;
            }
 

        }




        List<Transform> children = new List<Transform>();
        foreach (Transform child in MainMenuController.instance._GameListParent_updating)
            children.Add(child);
        children = children.OrderBy(o => o.name).ToList();

        foreach (Transform child in children)
        {
            child.SetParent(null);
        }

        foreach (Transform child in children)
        {
            child.SetParent( MainMenuController.instance._GameListParent_updating);

            Vector3 rc = child.GetComponent<RectTransform>().localPosition;
            child.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        }







        LoadingOverlay.instance.DoneLoading("Updating..");

        //If no loading in progress we know it's the last call.
        if (LoadingOverlay.instance.LoadingCall.Count== 0)
        {

            PlayfabHelperFunctions.instance.RemoveLegacyAndEmptyGames();

            GameObject obj = (GameObject)GameObject.Instantiate(_PlayfabHelperFunctions._FinishedTitleListItem, MainMenuController.instance._GameListParent_updating);

            string[] stringSeparators = new string[] { "[splitter]" };



            string[] oldGameList =null;
            if (GetComponent<Startup>().myData.ContainsKey("OldGames"))
            {
                oldGameList = GetComponent<Startup>().myData["OldGames"].Value.Split(stringSeparators, System.StringSplitOptions.None);
            }
            else if(PlayerPrefs.HasKey("OldGames"))
            {
                oldGameList = PlayerPrefs.GetString("OldGames").Split(stringSeparators, System.StringSplitOptions.None);
            }
            else
            {
                //Need to load old games from online
                if (LoadingOverlay.instance != null)
                    LoadingOverlay.instance.ShowLoadingFullscreen("Getting old games!");

                PlayFabClientAPI.GetUserData(new GetUserDataRequest()
                {
                    PlayFabId = Startup._instance.MyPlayfabID,
                    Keys = new List<string> { "OldGames" },
                }, result => {

                    if (LoadingOverlay.instance != null)
                        LoadingOverlay.instance.DoneLoading("Getting old games!");

                    PlayerPrefs.SetString("OldGames", result.Data["OldGames"].Value );

                    Refresh();


                }, (error) => {
                    if (LoadingOverlay.instance != null)
                        LoadingOverlay.instance.DoneLoading("Getting old games!");

                    Debug.LogError("BUG GETTING OLD GAMES!");
                });
                return;
            }



            for (int i = oldGameList.Length-1; i > oldGameList.Length - 10; i--)
            {

                if (i>=0 && oldGameList[i].Length > 2)
                {
                    BoardData bd = new BoardData(CompressString.StringCompressor.DecompressString(oldGameList[i]));
                    GameObject obj2 = (GameObject)GameObject.Instantiate(_PlayfabHelperFunctions._GameListItem, MainMenuController.instance._GameListParent_updating);
                    obj2.GetComponent<GameListItem>().Init(bd, true);
                }

            }

            myOldGameList.Clear();
            for (int i = 0; i < oldGameList.Length; i++)
            {
                if (i >= 0 && oldGameList[i].Length > 2)
                {
                    BoardData bd = new BoardData(CompressString.StringCompressor.DecompressString(oldGameList[i]));
                    myOldGameList.Add(bd);
                }


            }

            //



            foreach (Transform child in MainMenuController.instance._GameListParent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            foreach (Transform child in MainMenuController.instance._GameListParent_updating.transform)
            {
                Transform go2 = GameObject.Instantiate(child, MainMenuController.instance._GameListParent.transform);

                if (go2.GetComponent<GameListItem>() != null)
                    child.GetComponent<GameListItem>().OnPictureCallback = go2.GetComponent<GameListItem>();

                
            }



            ScrollListBasedOnItems[] list = GameObject.FindObjectsOfType<ScrollListBasedOnItems>();
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].gameObject.activeSelf)
                    list[i].RefreshLayout();
            }



            SaveGameList();

            if(PhotonNetwork.InRoom == false)
                PhotonNetwork.Disconnect();

            if(FindObjectOfType<ScrollListBasedOnItems>()!=null)
                FindObjectOfType<ScrollListBasedOnItems>().Reset();

            
        }


#if UNITY_IOS
        iOSNotification not = iOSNotificationCenter.GetLastRespondedNotification();



        if (not != null)
        {
            
            if(pushHandled.Contains(not.Identifier) == false)
            {
                iOSNotificationCenter.RemoveAllDeliveredNotifications();
                UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
                UnityEngine.iOS.NotificationServices.ClearRemoteNotifications();
                pushHandled.Add(not.Identifier);
                string theM = not.Body;

                if (theM.Contains("It's your turn against"))
                {
       
                    theM = theM.Replace("It's your turn against ", "");
                    //Debug.LogError(theM);
                    theM = theM.Remove(theM.Length - 1);
                    //Debug.LogError(theM);

                    for (int i = 0; i < openGamesList.Count; i++)
                    {
                        if (openGamesList[i].player1_displayName == theM || openGamesList[i].player2_displayName == theM)
                        {
                            Startup._instance.GameToLoad = openGamesList[i];
                            SceneManager.LoadScene(1);
                            return;

                        }
                    }
                }
                
            }
      
        }

#endif



    }
    public List<string> pushHandled = new List<string>();
    [System.Serializable]
    public class BoardList
    {
        public List<BoardData> myOpenGames;
    }

    public void SaveGameList()
    {
        BoardList bl = new BoardList();
        bl.myOpenGames = Startup._instance.openGamesList;
        string data = JsonUtility.ToJson(bl);
        PlayerPrefs.SetString("SavedGameList", data);
    }
    public void LoadGameList(float aTime)
    {
        StartCoroutine(LoadGameListIE(aTime));
    }
    IEnumerator LoadGameListIE(float aTime)
    {
        yield return new WaitForSeconds(aTime);
        
        BoardList gameList = JsonUtility.FromJson<BoardList>( PlayerPrefs.GetString("SavedGameList") );

        foreach (Transform child in MainMenuController.instance._GameListParent.transform)
        {

            if (child.gameObject.name.Contains("_SerachingForGame") == false)
            {
                GameObject.Destroy(child.gameObject);
            }
            else
            {
            }
        }

        Startup._instance.openGamesList.Clear();

        string jsonAIBoard = PlayerPrefs.GetString("AIGame", "");
        if (jsonAIBoard != "")
        {
            BoardData aiGameBoard = new BoardData(jsonAIBoard);
            GameObject obj2 = (GameObject)GameObject.Instantiate(_PlayfabHelperFunctions._GameListItem, MainMenuController.instance._GameListParent);
            Vector3 rc = obj2.GetComponent<RectTransform>().localPosition;
            obj2.GetComponent<RectTransform>().localPosition = new Vector3(rc.x, rc.y, 0);
            obj2.GetComponent<GameListItem>().Init(aiGameBoard, false, true);
            obj2.name = "0_" + obj2.name;
        }


        for (int i = 0; i < gameList.myOpenGames.Count; i++)
        {


                BoardData bd = gameList.myOpenGames[i];

                if (bd.player1_abandon == "1" || bd.player2_abandon == "1")
                {

                }
                else if (bd.player2_PlayfabId != "")
                {
                    GameObject obj2 = (GameObject)GameObject.Instantiate(_PlayfabHelperFunctions._GameListItem, MainMenuController.instance._GameListParent);
                    Vector3 rc = obj2.GetComponent<RectTransform>().localPosition;
                    obj2.GetComponent<RectTransform>().localPosition = new Vector3(rc.x, rc.y, 0);
                    obj2.GetComponent<GameListItem>().Init(bd);


                    GameListItem it = obj2.GetComponent<GameListItem>();

                    if (it != null)
                    {
                        string Gname = "";
                        if (it.YourTurnGO.activeSelf)
                            Gname = "0_";
                        else
                            Gname = "1_";
                        Gname += it.bd.RoomName;

                    obj2.name = Gname;
                    }


            }
                else
                {
                    if (Startup._instance.SearchingForGameObject == null)
                    {
                        Startup._instance.SearchingForGameObject = (GameObject)GameObject.Instantiate(PlayfabHelperFunctions.instance.SearchingForGamePrefab, MainMenuController.instance._GameListParent);
                        Startup._instance.SearchingForGameObject.transform.SetAsFirstSibling();
                        Startup._instance.SearchingForGameObject.SetActive(true);
                        Vector3 rc = Startup._instance.SearchingForGameObject.GetComponent<RectTransform>().localPosition;
                        Startup._instance.SearchingForGameObject.GetComponent<RectTransform>().localPosition = new Vector3(rc.x, rc.y, 0);
                        Startup._instance.SearchingForGameObject.GetComponent<SearchGameInfo>().NameID = bd.RoomName;
                    }
                    else
                    {
                        Startup._instance.SearchingForGameObject.transform.SetAsFirstSibling();
                        Vector3 rc = Startup._instance.SearchingForGameObject.GetComponent<RectTransform>().localPosition;
                        Startup._instance.SearchingForGameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

                    }

                }
                Startup._instance.openGamesList.Add(bd);
            


            
        }


        List<Transform> children = new List<Transform>();
        foreach (Transform child in MainMenuController.instance._GameListParent)
            children.Add(child);
        children = children.OrderBy(o => o.name).ToList();

        foreach (Transform child in children)
        {
            child.SetParent(null);
        }

        foreach (Transform child in children)
        {
            child.SetParent(MainMenuController.instance._GameListParent);

            Vector3 rc = child.GetComponent<RectTransform>().localPosition;
            child.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        }





        GameObject obj = (GameObject)GameObject.Instantiate(_PlayfabHelperFunctions._FinishedTitleListItem, MainMenuController.instance._GameListParent);



        string[] stringSeparators = new string[] { "[splitter]" };
        string[] oldGameList = null;
        if (GetComponent<Startup>().myData.ContainsKey("OldGames"))
        {
            oldGameList = GetComponent<Startup>().myData["OldGames"].Value.Split(stringSeparators, System.StringSplitOptions.None);
        }
        else if (PlayerPrefs.HasKey("OldGames"))
        {
            oldGameList = PlayerPrefs.GetString("OldGames").Split(stringSeparators, System.StringSplitOptions.None);
        }

            for (int i = oldGameList.Length - 1; i > oldGameList.Length - 10; i--)
            {

                if (i >= 0 && oldGameList[i].Length > 2)
                {
                    BoardData bd = new BoardData(CompressString.StringCompressor.DecompressString(oldGameList[i]));
                    GameObject obj2 = (GameObject)GameObject.Instantiate(_PlayfabHelperFunctions._GameListItem, MainMenuController.instance._GameListParent);
                    obj2.GetComponent<GameListItem>().Init(bd, true);
                }

            }





            ScrollListBasedOnItems[] list = GameObject.FindObjectsOfType<ScrollListBasedOnItems>();
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].gameObject.activeSelf)
                    list[i].RefreshLayout(0.0f);
            }


    }
    public void SetPlayfabSecondPlayerInRoom(string player1_playfabID,string player2_playfabID,string player1_displayName, string roomName, string boardLayout)
    {
        // The game can start
        //1. Remove room
        //2. Set shared data for p1 and p2
        //3. Start game

        //1
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LeaveRoom();

        //2
        //Run cloud script with data GetSerializedRoomData() and player1_playfabID,player2_playfabID,roomName
        //PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        //{
        //    FunctionName = "SetPlayfabSecondPlayerInRoom", // Arbitrary function name (must exist in your uploaded cloud.js file)
        //    FunctionParameter = new { id1 = player1_playfabID, id2 = player2_playfabID, roomName = roomName, roomData = GetSerializedRoomData() }, // The parameter provided to your function
        //    GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        //}, OnCloudHelloWorld, OnErrorShared);



        // Add SharedGroup
        _PlayfabHelperFunctions.AddPlayerToSharedGroup(player1_playfabID, player2_playfabID, player1_displayName, roomName,boardLayout);




 






         

    }
    public bool GetHasActiveGameSearch()
    {
        // Check from shared data if you have a game serach in progress
        for(int i = 0; i< openGamesList.Count;i++)
        {
            if (openGamesList[i].player1_PlayfabId == MyPlayfabID && openGamesList[i].player2_PlayfabId == "")
                return true;
        }
        return false;
    }



    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("<Color=Failed>OnCreateRoomFailed</Color> ");
        LoadingOverlay.instance.DoneLoading("CreateRoom");
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        //Debug.Log("<Color=Red>OnDisconnected</Color> " + cause);
        //Debug.LogError("PUN Basics Tutorial/Launcher:Disconnected");
        timer = 0;
    }

#endregion




    string GetSerializedRoomData()
    {
        return "";
    }
    public void AdjustThropies(int aValue, string opponentPlayfabId, string opponentDisplayName, int aTotalScore)
    {
        myData["Ranking"].Value = (int.Parse(myData["Ranking"].Value) + aValue).ToString();

        _PlayfabHelperFunctions.SubmitHighscore(int.Parse( myData["Ranking"].Value ));
        
        
        _PlayfabHelperFunctions.ChangeValueFor("Ranking", myData["Ranking"].Value);

        if (opponentPlayfabId == "")
            opponentPlayfabId = "AI";
        string winnerId = opponentPlayfabId;
        if (aValue > 0)
            winnerId = MyPlayfabID ;

        if (myData.ContainsKey("StatsData")  == false || myData["StatsData"].Value.Length<=1)
        {
         
            StatsData newData = new StatsData();
            newData.FinishedGames.Add(new StatsGame(opponentPlayfabId, opponentDisplayName,winnerId));

            UpdateStatsData(newData.GetJson());
        }
        else
        {
            StatsData newData = new StatsData(myData["StatsData"].Value);
            newData.FinishedGames.Add(new StatsGame(opponentPlayfabId, opponentDisplayName, winnerId));
            UpdateStatsData(newData.GetJson());
        }

        if(aValue>0)
            AchivmentController.instance.WonGame(aTotalScore);
        else
            AchivmentController.instance.LostGame(aTotalScore);


        AddXP(85);

    }



        
    public void AddXP(int aValue)
    {
        myData["XP"].Value = (int.Parse(myData["XP"].Value) + aValue).ToString();

        _PlayfabHelperFunctions.ChangeValueFor("XP", myData["XP"].Value);

        _PlayfabHelperFunctions.SubmitExperience(int.Parse(myData["XP"].Value));
    }
    public void UpdateStatsData(string aStatsData)
    {
        myData["StatsData"].Value = aStatsData;

        _PlayfabHelperFunctions.ChangeValueFor("StatsData", myData["StatsData"].Value);
    }
    public StatsData GetStatsData()
    {
        if (myData.ContainsKey("StatsData") && myData["StatsData"].Value.Length > 1)
            return new StatsData(myData["StatsData"].Value);
        else
            return new StatsData();
    }
    public List<AudioClip> myClips;
    public void PlaySoundEffect(int aType)
    {
        if (PlayerPrefs.GetInt("Sound", 1) == 0)
            return;


        GameObject go = new GameObject(myClips[aType].name);
        AudioSource _as = go.AddComponent<AudioSource>();
        _as.clip = myClips[aType];
        _as.Play();
        _as.loop = false;
        go.AddComponent<DestroyAfterTime>();
        DontDestroyOnLoad(go);

    }

 
}
