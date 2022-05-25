using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameAnalyticsSDK;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using Unity.Notifications.iOS;
using UnityEngine;
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


    public bool isTutorialGame = false;
    public BoardData GameToLoad = null;

    public static Startup _instance=null;
    public AchivmentController myAchivmentController;
    public bool DontAutoRefresh = true;

    public static long TIMEOUT = 60 * 60 * 24 * 2;

    public static string LIVE_VERSION = "3";

    // public static long TIMEOUT = 60+60+60;

    public bool isFake;
    public GameObject ConfetiPart;
    // Start is called before the first frame update
    void Start()
    {
        GameAnalytics.Initialize();

        //Appodeal.initialize("91f0aae11c6d5b4fe09000ad17edf290d41803497b6ff82f", Appodeal.INTERSTITIAL | Appodeal.REWARDED_VIDEO, true);


        Application.runInBackground = true;
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

        LoadingOverlay.instance.ShowLoadingFullscreen("Connecting to photon");

        if (PhotonNetwork.IsConnected)
        {}else{
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = "1";
        }




        UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Badge | UnityEngine.iOS.NotificationType.Sound, true);


        StartCoroutine(RegisterPush());


        if (PlayerPrefs.GetInt("Music", 1) == 0)
        {

            Startup._instance.GetComponent<AudioSource>().volume = 0;
        }
        else
            Startup._instance.GetComponent<AudioSource>().volume = 0.3f;



    }
    void OnApplicationPause(bool pauseStatus)
    {
        if(!pauseStatus)
        {
            if (PhotonNetwork.IsConnected)
            { }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = "1";

                if(Startup._instance != null)
                {
                    Startup._instance.Refresh(0.1f);
                    if (Startup._instance.avatarURL != null)
                        if (Startup._instance.avatarURL.Length > 0)
                        {
                            PlayfabHelperFunctions.instance.LoadAvatarURL(Startup._instance.avatarURL);
                        }
                    SceneManager.LoadScene(0);
                }
   
            }

        }

    }
    //void OnApplicationFocus(bool hasFocus)
    //{
    //    if (hasFocus)
    //        SceneManager.LoadScene(0);
    //}

    IEnumerator RegisterPush()
    {
        yield return new WaitForSeconds(3);

        byte[] token = UnityEngine.iOS.NotificationServices.deviceToken;
        if (token != null)
        {
            RegisterForIOSPushNotificationRequest request = new RegisterForIOSPushNotificationRequest();
            request.DeviceToken = System.BitConverter.ToString(token).Replace("-", "").ToLower();
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



        // this is for if we get a message while the app is runnings // as playfab cant set ShowInForeground
        iOSNotificationCenter.OnRemoteNotificationReceived += remoteNotification =>
        {
            // When a remote notification is received, modify its contents and show it after 1 second.
            var timeTrigger = new iOSNotificationTimeIntervalTrigger()
            {
                TimeInterval = new System.TimeSpan(0, 0, 1),
                Repeats = false
            };

            iOSNotification notification = new iOSNotification()
            {
                Title =  remoteNotification.Title,
                Body =  remoteNotification.Body,
                Subtitle =  remoteNotification.Subtitle,
                ShowInForeground = true,
                ForegroundPresentationOption = PresentationOption.Sound | PresentationOption.Alert,
                CategoryIdentifier = remoteNotification.CategoryIdentifier,
                ThreadIdentifier = remoteNotification.ThreadIdentifier,
                Trigger = timeTrigger,
            };
            iOSNotificationCenter.ScheduleNotification(notification);
        };





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
        Refresh();
    }
    public void UpdateDisplayName()
    {
        //_PlayfabHelperFunctions.UpdateDisplayName(_txtLabel.text);
    }

    float timer = 0;
    // Update is called once per frame
    void Update()
    {
        if(timer != -1)
        if(!PhotonNetwork.IsConnected)
        {
            timer += Time.deltaTime;
            if(timer>8)
            {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = "1";
                timer = 0;
            }

        }
    }
    public void ChangeValueFor(string aEntry,string aValue)
    {
        _PlayfabHelperFunctions.ChangeValueFor(aEntry, aValue);
    }

    #region MonoBehaviourPunCallbacks CallBacks
    public override void OnConnectedToMaster()
    {
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
    public void JoinRandomRoom()
    {
        if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
        {
            LoadingOverlay.instance.ShowLoading("JoinRandomRoom");

            PhotonNetwork.JoinRandomRoom(); // Joina random or create a new room and shared data entry
        }
        else
        {
            Debug.LogError("Can't join random room now, client is not ready");
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
    public void FinishedGettingGameListCheckForOpenGames()
    {
        for (int i = 0; i < openGamesList.Count; i++)
        {
            if (openGamesList[i].player1_PlayfabId == MyPlayfabID && openGamesList[i].player2_PlayfabId == "")
            {
                if(PhotonNetwork.InRoom == false)
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
            child.parent = null;
        }

        foreach (Transform child in children)
        {
            child.parent = MainMenuController.instance._GameListParent_updating;

            Vector3 rc = child.GetComponent<RectTransform>().localPosition;
            child.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        }
    








        //If no loading in progress we know it's the last call.
        if (LoadingOverlay.instance.LoadingCall.Count== 0)
        {
            GameObject obj = (GameObject)GameObject.Instantiate(_PlayfabHelperFunctions._FinishedTitleListItem, MainMenuController.instance._GameListParent_updating);

            string[] stringSeparators = new string[] { "[splitter]" };
            string[] oldGameList = GetComponent<Startup>().myData["OldGames"].Value.Split(stringSeparators, System.StringSplitOptions.None);
            for (int i = oldGameList.Length-1; i > oldGameList.Length-10; i--)
            {

                if (i>=0 && oldGameList[i].Length > 2)
                {
                    BoardData bd = new BoardData(CompressString.StringCompressor.DecompressString(oldGameList[i]));
                    GameObject obj2 = (GameObject)GameObject.Instantiate(_PlayfabHelperFunctions._GameListItem, MainMenuController.instance._GameListParent_updating);
                    obj2.GetComponent<GameListItem>().Init(bd, true);
                }

            }
  

            //



            foreach (Transform child in MainMenuController.instance._GameListParent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            foreach (Transform child in MainMenuController.instance._GameListParent_updating.transform)
            {
                GameObject.Instantiate(child, MainMenuController.instance._GameListParent.transform);
            }



            ScrollListBasedOnItems[] list = GameObject.FindObjectsOfType<ScrollListBasedOnItems>();
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].gameObject.activeSelf)
                    list[i].RefreshLayout();
            }
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
        Debug.Log("<Color=Red>OnDisconnected</Color> " + cause);
        Debug.LogError("PUN Basics Tutorial/Launcher:Disconnected");
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
