using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Facebook.Unity;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using LoginIdentityProvider = PlayFab.ClientModels.LoginIdentityProvider;
using LoginResult = PlayFab.ClientModels.LoginResult;
using Unity.Advertisement.IosSupport;

public class PlayfabHelperFunctions : MonoBehaviour
{
    public bool TESTING_PLAYER_1=false;



    public GameObject _GameListItem;
    public GameObject _FinishedTitleListItem;
    public GameObject SearchingForGamePrefab;

    public List<int> LevelSettings = new List<int>();

    public static PlayfabHelperFunctions instance;

    public void ReLogin()
    {


        OnFacebookInitialized();
    }

    public void Login()
    {
        string playerID = "";
        if (TESTING_PLAYER_1)
            playerID = "asdafsfsdf";
        else
            playerID = "asdafsfsdf2";

        //  playerID = "asdafsfsdf11";

        // playerID = "Villads123";
        //playerID = "PaxMM";
       // playerID = "steffen123";
        instance = this;
        LoadingOverlay.instance.ShowLoadingFullscreen("LoginWithCustomID");


        if (FB.IsInitialized)
            OnFacebookInitialized();
        else
            FB.Init(OnFacebookInitialized);

        if (PlayerPrefs.HasKey("FacebookLink"))
        {
            return;
        }












#if UNITY_EDITOR
        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
        {
            CreateAccount = true,
            CustomId = playerID,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
            {
                GetUserAccountInfo = true,
                GetPlayerProfile = true,

            }
        },
        result =>
        {
            LoginSucess(result);
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });


#elif UNITY_IOS
    PlayFabClientAPI.LoginWithIOSDeviceID(new LoginWithIOSDeviceIDRequest()
        { 

            TitleId = PlayFabSettings.TitleId,
            DeviceId = SystemInfo.deviceUniqueIdentifier,
            OS = SystemInfo.operatingSystem,
            DeviceModel = SystemInfo.deviceModel,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
            {
                GetUserAccountInfo =true,
                GetPlayerProfile = true
            }
        },
result =>
{
    LoginSucess(result);
},
 error => Debug.LogError(error.GenerateErrorReport()));

#endif


    }
    public void LoginSucess(LoginResult result)
    {
        
                    LoadingOverlay.instance.ShowLoadingFullscreen("Loading data!");

        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(),
              result2 => {
                  if (result2.Data == null || !result2.Data.ContainsKey("TilesAmount"))
                      Debug.Log("No TilesAmount");
                  else
                  {
                      Startup._instance.StaticServerData = result2.Data;
                  }
                  LoadingOverlay.instance.DoneLoading("Loading data!");


                  if( int.Parse(Startup.LIVE_VERSION) < int.Parse(Startup._instance.StaticServerData["LIVE_VERSION"]))
                  {
                      MainMenuController.instance.UpdateWindow.SetActive(true);
                  }

              },
              error => {
                  Debug.Log("Got error getting titleData:");
                  Debug.Log(error.GenerateErrorReport());
              }
          );


       for(int i = 0; i< 200;i++)
        {
            LevelSettings.Add((500*i) +(i*30));

        }






        LoadingOverlay.instance.DoneLoading("LoginWithCustomID");
        GetComponent<Startup>().MyPlayfabID = result.AuthenticationContext.PlayFabId;

        if (result.NewlyCreated)
        {
            SetUserData();
        }
        else
        {
            if (result.InfoResultPayload.PlayerProfile.DisplayName != null)
                GetComponent<Startup>().displayName = result.InfoResultPayload.PlayerProfile.DisplayName;
            GetComponent<Startup>().avatarURL = result.InfoResultPayload.AccountInfo.TitleInfo.AvatarUrl;
            
            GetComponent<Startup>().PlayerProfile = result.InfoResultPayload.PlayerProfile;
            GetComponent<Startup>().UserAccount = result.InfoResultPayload.AccountInfo;

            bool isLinked = false;
            if (result.InfoResultPayload.AccountInfo.FacebookInfo != null &&  result.InfoResultPayload.AccountInfo.FacebookInfo.FacebookId.Length>0)
            {
                isLinked = true;
                MainMenuController.instance.SetFBLinked(true);

            }
            else
            {
                MainMenuController.instance.SetFBLinked(false);
            }


            if (GetComponent<Startup>().avatarURL != null)
                if (GetComponent<Startup>().avatarURL.Length > 0)
                    LoadAvatarURL(GetComponent<Startup>().avatarURL);


            //for (int i = 0; i < Startup._instance.PlayerProfile.LinkedAccounts.Count; i++)
            //{
            //    if (Startup._instance.PlayerProfile.LinkedAccounts[i].Platform == LoginIdentityProvider.Facebook)
            //    {
            //        isLinked = true;

            //    }
            //}

            //   if(isLinked)
            {
                //FacebookInit();


            }


            Refresh();
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
    public void UpdateDisplayName(string aName)
    {
        LoadingOverlay.instance.ShowLoading("UpdateUserTitleDisplayName");
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = aName 
        }, result =>
        {
            
            LoadingOverlay.instance.DoneLoading("UpdateUserTitleDisplayName");
            Debug.Log("The player's display name is now: " + result.DisplayName);
            MainMenuController.instance.SetNameGO.SetActive(false);

            StartCoroutine(GetComponent<Startup>().DelayRefresh());

        }, error =>{
                        Debug.LogError(error.GenerateErrorReport());
                        LoadingOverlay.instance.DoneLoading("UpdateUserTitleDisplayName");

            MainMenuController.instance.nameSettingTextError.text = "Invalid name";

        });
    }

    public void SetPlayfabCreatedRoom(string playfabId, string roomName)
    {
        LoadingOverlay.instance.ShowLoading("CreateSharedGroup");

        PlayFabClientAPI.CreateSharedGroup(new CreateSharedGroupRequest()
        {
            SharedGroupId = roomName
        }, result3 => {
            LoadingOverlay.instance.DoneLoading("CreateSharedGroup");

            AddGameToSharedGroup(new List<string>() { playfabId }, roomName);
            

        }, (error) => {
            Debug.Log(error.GenerateErrorReport());
        });

    }
    public void ChallengePlayer(string playfabId, string playfabId2,string player2DisplayName, string roomName)
    {


        List<string> aUser = new List<string>();
        aUser.Add(playfabId2);

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "AddPlayerToSharedGroup",
            FunctionParameter = new Dictionary<string, object>() {
            { "PlayFabIds", aUser },
            { "SharedGroupId", roomName }
        }
        }, result =>
        {
            Debug.Log(result.FunctionResult);
        }, error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        }

        );




        PlayFabClientAPI.CreateSharedGroup(new CreateSharedGroupRequest()
        {
            SharedGroupId = roomName
        }, result3 => {

                                Board.instance.GenerateStartBoard(int.Parse(Startup._instance.StaticServerData["TilesAmount"]), PlayerPrefs.GetInt("BoardLayout", 0).ToString());
                                BoardData bd = new BoardData(playfabId, playfabId2, "1", Board.instance.BoardTiles, roomName, new List<string>(), Board.instance.GetTilesLeft(), "0", Board.instance.p1_tiles, Board.instance.p2_tiles, System.DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                                bd.player1_displayName = Startup._instance.displayName;
                                bd.player2_displayName = player2DisplayName;
                                bd.player1_score = "0";
                                bd.player2_score = "0";
                                bd.EmptyTurns = "0";

                                PlayFabClientAPI.UpdateSharedGroupData(new UpdateSharedGroupDataRequest()
                                {
                                    SharedGroupId = roomName,
                                    Data = new Dictionary<string, string>() {
                                            {roomName, bd.GetJson()}

                                }
                                },
                                result4 =>
                                {
                                    Debug.Log("Successfully updated user data with new player id's");






                                    SendPushToUser(bd.player2_PlayfabId, "", "" + Startup._instance.displayName +" has challenged you!");

                                },
                                error =>
                                {
                                    Debug.Log("Got error setting user data Ancestor to Arthur");
                                    Debug.Log(error.GenerateErrorReport());
                                });


        }, (error) => {
            Debug.Log(error.GenerateErrorReport());
        });

    }

    


    public void AddGameToSharedGroup(List<string> playfabId, string roomName)
    {

        string secondPlayer = "";
        if (playfabId.Count > 1)
            secondPlayer = playfabId[1];

        BoardData bd = new BoardData(playfabId[0], secondPlayer, "0", Board.instance.BoardTiles, roomName,new List<string>(), new List<string>(), "0", new List<string>(), new List<string>(), System.DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
        bd.player1_displayName = Startup._instance.displayName;
        LoadingOverlay.instance.ShowLoading("UpdateSharedGroupData");

        PlayFabClientAPI.UpdateSharedGroupData(new UpdateSharedGroupDataRequest()
        {
            SharedGroupId = roomName,
            Data = new Dictionary<string, string>(){{roomName, bd.GetJson()}}
        },
        result2 =>
        {
            LoadingOverlay.instance.DoneLoading("UpdateSharedGroupData");
            Debug.Log("Successfully updated user data");
            AddSharedGroupToGameList(roomName);
            // Startup._instance.Refresh();
        },
        error =>
        {
            Debug.Log("Got error setting user data Ancestor to Arthur");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public void AddPlayerToSharedGroup(string player1_playfabID, string player2_playfabID,string player1_displayName, string roomName,string boardLayout)
    {

        //LoadingOverlay.instance.ShowLoading("PlayFabCloudScriptAPI.ExecuteFunction");

        //PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
        //{
        //    Entity = new PlayFab.CloudScriptModels.EntityKey()
        //    {
        //        Id = PlayFabSettings.staticPlayer.EntityId, //Get this from when you logged in,
        //        Type = PlayFabSettings.staticPlayer.EntityType, //Get this from when you logged in
        //    },
        //    FunctionName = "httptrigger1", //This should be the name of your Azure Function that you created.
        //    FunctionParameter = new Dictionary<string, object>() { { "player1_playfabID", roomName }, { "player2_playfabID", player2_playfabID } }, //This is the data that you would want to pass into your function.
        //    GeneratePlayStreamEvent = false //Set this to true if you would like this call to show up in PlayStream
        //}, (ExecuteFunctionResult result) =>
        //{
        //    LoadingOverlay.instance.DoneLoading("PlayFabCloudScriptAPI.ExecuteFunction");
        //    if (result.FunctionResultTooLarge ?? false)
        //    {
        //        Debug.Log("This can happen if you exceed the limit that can be returned from an Azure Function, See PlayFab Limits Page for details.");
        //        return;
        //    }
        //    Debug.Log($"The {result.FunctionName} function took {result.ExecutionTimeMilliseconds} to complete");
        //    Debug.Log($"Result: {result.FunctionResult.ToString()}");


        //    SetSharedDataForNewGame(player1_playfabID, player2_playfabID, player1_displayName, roomName);


        //}, (PlayFabError error) =>
        //{
        //    Debug.Log($"Opps Something went wrong: {error.GenerateErrorReport()}");
        //});


        List<string> aUser = new List<string>();
        aUser.Add(player2_playfabID);

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "AddPlayerToSharedGroup",
            FunctionParameter = new Dictionary<string, object>() {
            { "PlayFabIds", aUser },
            { "SharedGroupId", roomName }
        }
        }, result => {
            Debug.Log(result.FunctionResult);
            SetSharedDataForNewGame(player1_playfabID, player2_playfabID, player1_displayName, roomName,boardLayout);
        }, error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        }
);






    }
    public void AddSharedGroupToGameList(string aSharedGroupName)
    {
        GetComponent<Startup>().myData["MyGames"].Value += ","+ aSharedGroupName;

        LoadingOverlay.instance.ShowLoading("UpdateUserData");
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
            {"MyGames", GetComponent<Startup>().myData["MyGames"].Value},
        }
        },
        result =>
        {
            LoadingOverlay.instance.DoneLoading("UpdateUserData");

            StartCoroutine(GetComponent<Startup>().DelayRefresh());

        },
        error => {
            Debug.Log(error.GenerateErrorReport());
        });
    }
    public void AddAiGameToOldGames(string aCompresserdAiBoard)
    {
        GetComponent<Startup>().myData["OldGames"].Value += "[splitter]" + aCompresserdAiBoard;
        //LoadingOverlay.instance.ShowLoading("UpdateUserData");

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
            {"MyGames",Startup._instance.myData["MyGames"].Value},
            {"OldGames",Startup._instance.myData["OldGames"].Value},
        }
        },
        result =>
        {
            //LoadingOverlay.instance.DoneLoading("UpdateUserData");

            // This is for if we find a abandoned game we want to remove a potentail next one

            //StartCoroutine(Startup._instance.DelayRefresh());

            PlayerPrefs.SetString("AIGame", "");

            Debug.Log("Removed game");

        },
        error => {
            Debug.Log(error.GenerateErrorReport());
        });
    }
    public void RemoveSharedGroupToGameList(string aSharedGroupName, System.Action onComplete, string aDBjson )
    {
        Startup._instance.myData["MyGames"].Value = Startup._instance.myData["MyGames"].Value.Replace("," + aSharedGroupName,"");

       
          
               string st = (aDBjson);


        BoardData bd = new BoardData(CompressString.StringCompressor.DecompressString(aDBjson));
        // Adding this if challenge is rejected dont add to old games
        bool addToOld = true;
        if ((bd.player1_score == "" || bd.player1_score == "0") && (bd.player2_score == "" || bd.player2_score == "0"))
            addToOld = false;

        if(addToOld)
        GetComponent<Startup>().myData["OldGames"].Value += "[splitter]" + st;
        if (LoadingOverlay.instance != null)
            LoadingOverlay.instance.ShowLoading("UpdateUserData");

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
            {"MyGames",Startup._instance.myData["MyGames"].Value},
            {"OldGames",Startup._instance.myData["OldGames"].Value},
        }
        },
        result =>
        {
            if (LoadingOverlay.instance != null)
                LoadingOverlay.instance.DoneLoading("UpdateUserData");

            // This is for if we find a abandoned game we want to remove a potentail next one
            if (onComplete ==null)
            {
                StartCoroutine(Startup._instance.DelayRefresh());
            }
            else
            {
                onComplete.Invoke();
            }

            Debug.Log("Removed non existing SharedData room in UserData");

        },
        error => {
            Debug.Log(error.GenerateErrorReport());
        });
    }
    public void SetSharedDataForNewGame(string player1_playfabID, string player2_playfabID,string player1_displayName, string roomName, string boardLayout)
    {
        Board.instance.GenerateStartBoard(int.Parse(Startup._instance.StaticServerData["TilesAmount"]),boardLayout);
        BoardData bd = new BoardData(player1_playfabID, player2_playfabID, "0", Board.instance.BoardTiles,roomName, new List<string>(), Board.instance.GetTilesLeft(), "0", Board.instance.p1_tiles, Board.instance.p2_tiles, System.DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
        bd.player1_displayName = player1_displayName;
        bd.player2_displayName = Startup._instance.displayName;
        bd.player1_score = "0";
        bd.player2_score = "0";
        bd.EmptyTurns = "0";
        LoadingOverlay.instance.ShowLoading("UpdateSharedGroupData");

        PlayFabClientAPI.UpdateSharedGroupData(new UpdateSharedGroupDataRequest()
        {
            SharedGroupId = roomName,
            Data = new Dictionary<string, string>() {
                        {roomName, bd.GetJson()}

            }
        },
        result3 =>
        {
            LoadingOverlay.instance.DoneLoading("UpdateSharedGroupData");
            Debug.Log("Successfully updated user data with new player id's");

            AddSharedGroupToGameList(roomName);

            SendPushToUser(bd.player1_PlayfabId, "", "You have started a game against "+ Startup._instance.displayName);

        },
        error =>
        {
            Debug.Log("Got error setting user data Ancestor to Arthur");
            Debug.Log(error.GenerateErrorReport());
        });


        //PlayFabClientAPI.CreateSharedGroup(new CreateSharedGroupRequest()
        //{
        //    SharedGroupId = GetComponent<Startup>().MyPlayfabID,

        //}, result3 => {
        //    AddGameToSharedGroup(new List<string>(){ player1_playfabID, player2_playfabID }, roomName);
        //}, (error) => {
        //    AddGameToSharedGroup(new List<string>(){ player1_playfabID, player2_playfabID }, roomName);
        //    Debug.Log(error.GenerateErrorReport());
        //});


        //PlayFabClientAPI.UpdateSharedGroupData(new UpdateSharedGroupDataRequest()
        //{
        //    SharedGroupId = player2_playfabID,
        //    Data = new Dictionary<string, string>() {
        //                {roomName, bd.GetJson()}

        //    }
        //},
        //result3 =>
        //{
        //    Debug.Log("Successfully updated user data");


        //    Refresh();
        //},
        //error =>
        //{
        //    Debug.Log("Got error setting user data Ancestor to Arthur");
        //    Debug.Log(error.GenerateErrorReport());
        //});
    }



    public void SetUserData()
    {


        LoadingOverlay.instance.ShowLoadingFullscreen("UpdateUserData");
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        { 
            Data = new Dictionary<string, string>() {
            {"Ranking", "0"},
            {"Picture", "0"},
            {"MyGames", ","},
            {"OldGames", "[splitter][splitter]"},
            {"StatsData", " "},
            {"Achivments", Startup._instance.myAchivmentController.GetDefault()},
            {"XP", "0"} },
            Permission = UserDataPermission.Public,

        },
        result =>
        {
            LoadingOverlay.instance.DoneLoading("UpdateUserData");
            Debug.Log("Successfully updated user data");
            StartCoroutine(GetComponent<Startup>().DelayRefresh());
            MainMenuController.instance.OpenSetNameWidnow(true);

        },
        error => {
            Debug.Log("Got error setting user data Ancestor to Arthur");
            Debug.Log(error.GenerateErrorReport());
        });
    }
    public void ChangeValueFor(string aEntry,string aValue)
    {
        //LoadingOverlay.instance.ShowLoading("UpdateUserData");

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
                {aEntry, aValue}

            },
            Permission = UserDataPermission.Public,
        },
       result =>
       {
           //LoadingOverlay.instance.DoneLoading("UpdateUserData");

           Debug.Log("Successfully updated user data");
           //StartCoroutine(GetComponent<Startup>().DelayRefresh());

       },
       error => {
           Debug.Log("Got error setting user data Ancestor to Arthur");
           Debug.Log(error.GenerateErrorReport());
       });
    }
    public void GetUserData()
    {
        if (LoadingOverlay.instance != null)
            LoadingOverlay.instance.ShowLoading("GetUserData");

        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = GetComponent<Startup>().MyPlayfabID,
            Keys = null
        }, result => {


            if( SceneManager.GetActiveScene().name == "GameScene")
            {
                return;
            }

 




            if (LoadingOverlay.instance != null)
                LoadingOverlay.instance.DoneLoading("GetUserData");

            Debug.Log("Done getting data");

            GetComponent<Startup>().myData = result.Data;

            string achivmentsData = "";
            if (result.Data.ContainsKey("Achivments") == true)
            {
                achivmentsData = result.Data["Achivments"].Value;
            }
            else
            {
                achivmentsData = Startup._instance.myAchivmentController.GetDefault();
            }
            

            Startup._instance.myAchivmentController.Init(achivmentsData);


            //GetComponent<Startup>()._infoLabel.text = GetComponent<Startup>().displayName + "\nPicture: " + GetComponent<Startup>().myData["Picture"].Value + "\n" + "Ranking: " + GetComponent<Startup>().myData["Ranking"].Value;
            //MainMenuController.instance._Name.text = GetComponent<Startup>().displayName;
            //MainMenuController.instance._Thropies.text = GetComponent<Startup>().myData["Ranking"].Value;
            string xp = "0";
            if (result.Data.ContainsKey("XP") == true)
            {
                xp = result.Data["XP"].Value;
                
            }
            else
                ChangeValueFor("XP", "0");

            ProfileButton.instance.Init(GetComponent<Startup>().displayName, GetComponent<Startup>().myData["Ranking"].Value, xp);
            if(!updateHighscoreOnce)
            {
                SubmitHighscore(int.Parse(GetComponent<Startup>().myData["Ranking"].Value));
                updateHighscoreOnce = true;
            }



            UpdateGameList();

            //if (result.Data == null || !result.Data.ContainsKey("Ancestor")) Debug.Log("No Ancestor");
            //else Debug.Log("Ancestor: " + result.Data["Ancestor"].Value);
        }, (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });

    }
    bool updateHighscoreOnce = false;
    public void UpdateGameList()
    {

        foreach (Transform child in MainMenuController.instance._GameListParent_updating.transform)
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
        //GetComponent<Startup>()._roomListLabel.text = "";
        string[] gameList = GetComponent<Startup>().myData["MyGames"].Value.Split(',');
        string[] stringSeparators = new string[] { "[splitter]" };
        string[] oldGameList = GetComponent<Startup>().myData["OldGames"].Value.Split(stringSeparators, System.StringSplitOptions.None);
        for(int i = 0; i < oldGameList.Length;i++)
        {
            if(oldGameList[i].Length>2)
            {

               // Debug.Log(CompressString.StringCompressor.DecompressString(oldGameList[i]));
            }
             
        }


        string jsonAIBoard = PlayerPrefs.GetString("AIGame", "");
        if (jsonAIBoard  != "")
        {
            BoardData aiGameBoard = new BoardData(jsonAIBoard);

           
                GameObject obj2 = (GameObject)GameObject.Instantiate(_GameListItem, MainMenuController.instance._GameListParent_updating);
            Vector3 rc = obj2.GetComponent<RectTransform>().localPosition;
            obj2.GetComponent<RectTransform>().localPosition = new Vector3(rc.x, rc.y, 0);
            obj2.GetComponent<GameListItem>().Init(aiGameBoard, false, true);
            

        }

        //if((Startup._instance.SearchingForGameObject != null && PhotonNetwork.InRoom && PhotonNetwork.CountOfPlayersInRooms >= 2 ) ||
        //    (Startup._instance.SearchingForGameObject != null && PhotonNetwork.InRoom == false))
        //Destroy(Startup._instance.SearchingForGameObject);


        bool shouldAddOldGamesNow = true;
        for (int i = 0;i < gameList.Length;i++)
        {
            if(gameList[i].Length>1)
            {
                LoadingOverlay.instance.ShowLoading("GetSharedGroupData0");
                shouldAddOldGamesNow = false;
                Debug.Log("Getting game data: " + i + " : " + gameList[i]);
                PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest()
                {
                    SharedGroupId = gameList[i]
                }, result =>
                {
                    if(LoadingOverlay.instance != null)
                    LoadingOverlay.instance.DoneLoading("GetSharedGroupData0");
                    foreach (KeyValuePair<string, SharedGroupDataRecord> entry in result.Data)
                    {
                        if (entry.Key == "Chat")
                            continue;

                        BoardData bd = new BoardData(entry.Value.Value);
                        //GetComponent<Startup>()._roomListLabel.text += "" + bd.player1_PlayfabId + " vs " + bd.player2_PlayfabId + " turn: " + bd.playerTurn + "\n";


                        if(Startup._instance.SearchingForGameObject != null)
                        {
                            if( Startup._instance.SearchingForGameObject.GetComponent<SearchGameInfo>().NameID == bd.RoomName)
                            {
                                if(bd.player2_PlayfabId != "")
                                Destroy(Startup._instance.SearchingForGameObject);
                            }
                        }


                        if(bd.player1_abandon == "1" || bd.player2_abandon == "1")
                        {

                        }
                        else if(bd.player2_PlayfabId != "")
                        {
                            GameObject obj = (GameObject)GameObject.Instantiate(_GameListItem, MainMenuController.instance._GameListParent_updating);
                            Vector3 rc = obj.GetComponent<RectTransform>().localPosition;
                            obj.GetComponent<RectTransform>().localPosition = new Vector3(rc.x, rc.y, 0);
                            obj.GetComponent<GameListItem>().Init(bd);

                        }else
                        {
                            if(Startup._instance.SearchingForGameObject == null)
                            {
                                Startup._instance.SearchingForGameObject = (GameObject)GameObject.Instantiate(PlayfabHelperFunctions.instance.SearchingForGamePrefab, MainMenuController.instance._GameListParent_updating);
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

                        //Debug.Log(entry.Value.Value.Length);

                     //   string st = CompressString.StringCompressor.CompressString(entry.Value.Value);
                      //  Debug.Log(st);
                        //Debug.Log(st.Length);
                       // Debug.Log(CompressString.StringCompressor.DecompressString(st));

                        Startup._instance.openGamesList.Add(bd);
                    }


                    Startup._instance.FinishedGettingGameListCheckForOpenGames();

                }, (error) =>
                {
                    Debug.Log(error.GenerateErrorReport());
                });
            }
        }

       if(shouldAddOldGamesNow)
            Startup._instance.FinishedGettingGameListCheckForOpenGames();

    }
    public void RemoveRoomFromList(string aRoomName,string aRoomJson,System.Action onComplete=null)
    {
        if (LoadingOverlay.instance != null)
            LoadingOverlay.instance.ShowLoading("RemoveSharedGroupMembers");

        PlayFabClientAPI.RemoveSharedGroupMembers(new RemoveSharedGroupMembersRequest()
        { 
            SharedGroupId = aRoomName,
            PlayFabIds = new List<string>(){Startup._instance.MyPlayfabID}
        }, result =>
        {
            if (LoadingOverlay.instance != null)
                LoadingOverlay.instance.DoneLoading("RemoveSharedGroupMembers");


            string st = "";
       
                    st = CompressString.StringCompressor.CompressString(aRoomJson);
     


            for (int i = 0; i < Startup._instance.openGamesList.Count; i++)
            {
                if (Startup._instance.openGamesList[i].player1_PlayfabId == Startup._instance.MyPlayfabID && Startup._instance.openGamesList[i].player2_PlayfabId == "")
                {
                    Startup._instance.openGamesList.RemoveAt(i);
                    break;
                }
            }

            RemoveSharedGroupToGameList(aRoomName, onComplete, st);

            Debug.Log("Removed old inactive SharedData room");

        }, (error) =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }
    public void UpdateTargetGame(string aGame)
    {
        if(LoadingOverlay.instance != null)
        LoadingOverlay.instance.ShowLoading("GetSharedGroupData1");

        PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest()
                {
                    SharedGroupId = aGame
                }, result =>
                {
                    if (LoadingOverlay.instance != null)
                        LoadingOverlay.instance.DoneLoading("GetSharedGroupData1");

                    foreach (KeyValuePair<string, SharedGroupDataRecord> entry in result.Data)
                    {
                        if (entry.Key == "Chat")
                        {
                            string chatViewed = PlayerPrefs.GetString(aGame + "_chat");

                            if(chatViewed != entry.Value.Value)
                            {
                                string[] liveMessegaes = entry.Value.Value.Split('≤');
                                string[] viewedMessegaes = chatViewed.Split('≤');

                                GameManager.instance.ChatNotificationIcon.SetActive(true);
                                float dif = Mathf.Abs(liveMessegaes.Length - viewedMessegaes.Length) / 2;
                                if (dif == 0)
                                    dif = 1;
                                GameManager.instance.ChatNotificationIcon.transform.GetChild(0).GetComponent<Text>().text = dif.ToString();
                            }
                            else
                            {
                                GameManager.instance.ChatNotificationIcon.SetActive(false);
                            }
                            
                                
                            continue;
                        }
                         
                        BoardData bd = new BoardData(entry.Value.Value);
                        GetComponent<Startup>().GameToLoad = bd;
                    }
                    GameManager.instance.RefreshBackendCallback();

                }, (error) =>
                {
                    GameManager.instance.updateInProgress = false;
                    Debug.Log(error.GenerateErrorReport());
                });
            

        
    }
    public void UpdateChatMessages(string aGame)
    {
        if (LoadingOverlay.instance != null)
            LoadingOverlay.instance.ShowLoading("GetSharedGroupData2");

        PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest()
        {
            SharedGroupId = aGame
        }, result =>
        {
            if (LoadingOverlay.instance != null)
                LoadingOverlay.instance.DoneLoading("GetSharedGroupData2");

            foreach (KeyValuePair<string, SharedGroupDataRecord> entry in result.Data)
            {
                if (entry.Key == "Chat")
                {
                    string chatViewed = PlayerPrefs.GetString(aGame + "_chat");

                    if (chatViewed != entry.Value.Value)
                    {
                        string[] liveMessegaes = entry.Value.Value.Split('≤');
                        string[] viewedMessegaes = chatViewed.Split('≤');

                        GameManager.instance.ChatNotificationIcon.SetActive(true);
                        float dif = Mathf.Abs(liveMessegaes.Length - viewedMessegaes.Length) / 2;
                        GameManager.instance.ChatNotificationIcon.transform.GetChild(0).GetComponent<Text>().text = dif.ToString();
                    }
                    else
                    {
                        GameManager.instance.ChatNotificationIcon.SetActive(false);
                    }


                    continue;
                }
            }

        }, (error) =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }
    public void SendNextTurn(BoardData aBoarddata, System.Action callback=null)
    {

       // LoadingOverlay.instance.ShowLoading("UpdateSharedGroupData");

        PlayFabClientAPI.UpdateSharedGroupData(new UpdateSharedGroupDataRequest()
        {
            SharedGroupId = aBoarddata.RoomName,
            Data = new Dictionary<string, string>() {
                        {aBoarddata.RoomName, aBoarddata.GetJson()}

            }
        },
        result3 =>
        {
          //  LoadingOverlay.instance.DoneLoading("UpdateSharedGroupData");
            Debug.Log("Successfully updated shared data with where players took a turn");
            GameManager.instance.updateInProgress = false;

            string userToSend = "";
            string displayName = "";
            if (aBoarddata.playerTurn == "0")
            {
                userToSend = aBoarddata.player1_PlayfabId;
                displayName = aBoarddata.player2_displayName;
            }

            else
            {
                userToSend = aBoarddata.player2_PlayfabId;
                displayName = aBoarddata.player1_displayName;
            }
 


            SendPushToUser(userToSend,"", "It's your turn against "+displayName +"!");



            string bliststring = PlayerPrefs.GetString("SavedGameList");
            if(bliststring != null)
            {
                Startup.BoardList gameList = JsonUtility.FromJson<Startup.BoardList>(bliststring);

                for (int i = 0; i < gameList.myOpenGames.Count; i++)
                {
                    if (gameList.myOpenGames[i].RoomName == aBoarddata.RoomName)
                    {
                        gameList.myOpenGames[i] = aBoarddata;


                        string data = JsonUtility.ToJson(gameList);
                        PlayerPrefs.SetString("SavedGameList", data);

                    }
                }
            }

            if (callback != null)
                callback.Invoke();






        },
        error =>
        {
            Debug.Log("Got error making turn");
            Debug.Log(error.GenerateErrorReport());
            GameManager.instance.updateInProgress = false;

        });


    }
    public void SendPushToUser(string aId,string title, string message)
    {
        if (title == "")
            title = "Outnumber";
        if (message == "")
            message = "Outnumber";

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "ChallengePlayer",
            FunctionParameter = new Dictionary<string, object>() {
            { "TargetId", aId },
            { "Title", title },
            { "Message", message },

        }
        }, result => {
            Debug.Log(result.FunctionResult);

        }, error => Debug.LogError(error.GenerateErrorReport()));
    }
    public void AddGameToPlayerListCloudScript(string aId, string aRoomName)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "AddGameToPlayerList",
            FunctionParameter = new Dictionary<string, object>() {
            { "PlayFabId", aId },
            { "RoomName", aRoomName }

        }
        }, result => {
            Debug.Log(result.FunctionResult);

        }, error => Debug.LogError(error.GenerateErrorReport()));
    }


    
    public void CreateAndAddToSharedGroup(string[] aPlayFabIds, string aSharedGroupId)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "CreateAndAddSharedGroup",
            FunctionParameter = new Dictionary<string, object>() {
            { "PlayFabIds", aPlayFabIds },
            { "SharedGroupId", aSharedGroupId }
        }
        }, result => {
            Debug.Log(result.FunctionResult);

        }, error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        }
        );

    }
    public void AddPlayerToSharedGroup(string[] aPlayFabIds, string aSharedGroupId)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "AddPlayerToSharedGroup",
            FunctionParameter = new Dictionary<string, object>() {
            { "PlayFabIds", aPlayFabIds },
            { "SharedGroupId", aSharedGroupId }
        }
        }, result => {
            Debug.Log(result.FunctionResult);

        }, error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        }
        );

    }



    public void Refresh()
    {
        //if (LoadingOverlay.instance.LoadingCall.Count > 0)
        //    return;

        Startup._instance.DontAutoRefresh = false;

        if (LoadingOverlay.instance != null)
            LoadingOverlay.instance.ShowLoading("GetPlayerProfile");

        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
        {
            PlayFabId = GetComponent<Startup>().MyPlayfabID,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true
            }
        },
        result =>
        {
            if(LoadingOverlay.instance != null)
            LoadingOverlay.instance.DoneLoading("GetPlayerProfile");

            GetComponent<Startup>().displayName = result.PlayerProfile.DisplayName;


            GetUserData();
        },
        error => Debug.LogError(error.GenerateErrorReport()));





        //PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest()
        //{
        //    SharedGroupId = GetComponent<Startup>().MyPlayfabID
        //}, result => {

        //    Debug.Log("Done getting gamelist");
        //    GetComponent<Startup>()._roomListLabel.text = "";
        //    foreach (KeyValuePair<string, SharedGroupDataRecord> entry in result.Data)
        //    {
        //        BoardData bd = new BoardData(entry.Value.Value);

        //        GetComponent<Startup>()._roomListLabel.text += "" + bd.player1_PlayfabId + " vs " + bd.player2_PlayfabId + " turn: " + bd.playerTurn + "\n";
        //        GetComponent<Startup>().openGamesList.Add(bd);
        //    }

        //}, (error) => {

        //    Debug.Log(error.GenerateErrorReport());
        //});

    }

    public void RemoveAbandonedGames()
    {
        RemoveAbandonedGamesCO();
    }
    public void RemoveAbandonedGamesCO()
    {

        for (int i = 0; i < Startup._instance.openGamesList.Count; i++)
        {
            long timeToDeadline = 0;
            if(Startup._instance.openGamesList[i].LastMoveTimeStamp != null)
            {
                long a = System.DateTimeOffset.Now.ToUnixTimeSeconds();
                
                long b = long.Parse(Startup._instance.openGamesList[i].LastMoveTimeStamp);
              //  long Future = b + 60 * 60 * 24 * 2;
                long Future = b + Startup.TIMEOUT;
                timeToDeadline = Future - a;
            }




  

            if (Startup._instance.openGamesList[i].player1_abandon == "1" || Startup._instance.openGamesList[i].player2_abandon == "1" || int.Parse(Startup._instance.openGamesList[i].EmptyTurns) >= 4 || timeToDeadline<0)
            {
                if( Startup._instance.openGamesList[i].playerTurn == "0" && timeToDeadline < 0)
                {
                    Startup._instance.openGamesList[i].player1_score = "0";
                }
                if (Startup._instance.openGamesList[i].playerTurn == "1" && timeToDeadline < 0)
                {
                    Startup._instance.openGamesList[i].player2_score = "0";
                }

                RemoveRoomFromList(Startup._instance.openGamesList[i].RoomName, Startup._instance.openGamesList[i].GetJson(), RemoveAbandonedGamesCO);
                Startup._instance.openGamesList.RemoveAt(i);
                return;
            }
        }
       // 


    }

    public void DeleteGame(string aRoomName, System.Action callback = null)
    {
        SetAbandomeForPlayerInGame(aRoomName, callback);
  

    }
    public void SetAbandomeForPlayerInGame(string aRoomName, System.Action callback = null)
    {
        if(LoadingOverlay.instance != null)
        LoadingOverlay.instance.ShowLoading("GetSharedGroupData3");

        PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest()
        {
            SharedGroupId = aRoomName
        }, result =>
        {
            if (LoadingOverlay.instance != null)
                LoadingOverlay.instance.DoneLoading("GetSharedGroupData3");
            foreach (KeyValuePair<string, SharedGroupDataRecord> entry in result.Data)
            {
                if (entry.Key == "Chat")
                    continue;
                BoardData bd = new BoardData(entry.Value.Value);

                bd.SetPlayerAbandome();
                if (LoadingOverlay.instance != null)
                    LoadingOverlay.instance.ShowLoading("UpdateSharedGroupData");
                PlayFabClientAPI.UpdateSharedGroupData(new UpdateSharedGroupDataRequest()
                {
                    SharedGroupId = aRoomName,
                    Data = new Dictionary<string, string>() {
                        {aRoomName, bd.GetJson()}

                }
                },
               result3 =>
               {
                   if (LoadingOverlay.instance != null)
                       LoadingOverlay.instance.DoneLoading("UpdateSharedGroupData");
                   Debug.Log("Successfully updated SetAbadome in room name:" + aRoomName);
                   RemoveRoomFromList(aRoomName, bd.GetJson(), callback);

               },
               error =>
               {
                   Debug.Log("Got error SetAbadome");
                   Debug.Log(error.GenerateErrorReport());

               });
            }
        }, (error) =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
  
    }
    public void SubmitHighscore(int playerScore)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate> {
            new StatisticUpdate {
                StatisticName = "Highscore",
                Value = playerScore
            }
        }
        }, result => OnStatisticsUpdated(result), FailureCallback);
    }
    public void SubmitExperience(int playerXP)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate> {
            new StatisticUpdate {
                StatisticName = "Experience",
                Value = playerXP
            }
        }
        }, result => OnStatisticsUpdated(result), FailureCallback);
    }
    private void OnStatisticsUpdated(UpdatePlayerStatisticsResult updateResult)
    {
        Debug.Log("Successfully submitted high score");
    }

    private void FailureCallback(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your API call. Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }






    // Facebook

    public void FacebookInit()
    {
        //LoadingOverlay.instance.ShowLoadingFullscreen("Facebook init");
        FB.Init(OnFacebookInitialized);
    }
    public void FacebookLink()
    {

        
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "gaming_user_picture" }, OnFacebookLoggedIn);
    }
    public void FacebookUnLink()
    {
        PlayFabClientAPI.UnlinkFacebookAccount(new UnlinkFacebookAccountRequest {  }, OnUnlinked, null);


    }
    private void OnUnlinked( UnlinkFacebookAccountResult result)
    {
        PlayerPrefs.DeleteKey("FacebookLink");
        Debug.Log("unlinked facebook");
        MainMenuController.instance.SetFBLinked(false);
    }
    private void RegisterAppForNetworkAttribution()
    {
        SkAdNetworkBinding.SkAdNetworkRegisterAppForNetworkAttribution();
    }
    private void OnFacebookInitialized()
    {
        Debug.Log("Logging into Facebook...");
        Invoke(nameof(RegisterAppForNetworkAttribution), 1);
        // Once Facebook SDK is initialized, if we are logged in, we log out to demonstrate the entire authentication cycle.
        //if (FB.IsLoggedIn)
        //    FB.LogOut();

        // We invoke basic login procedure and pass in the callback to process the result



        // FB.API("me/picture?type=square&height=88&width=88", HttpMethod.GET, FbGetPicture);
        LoadingOverlay.instance.DoneLoading("Facebook init");
        if (GetComponent<Startup>().avatarURL != null)
        if (GetComponent<Startup>().avatarURL.Length > 0)
            LoadAvatarURL(GetComponent<Startup>().avatarURL);


        if (PlayerPrefs.HasKey("FacebookLink"))
        {
            if(FB.IsLoggedIn == false)
                FB.LogInWithReadPermissions(new List<string>() { "public_profile", "gaming_user_picture" }, OnFacebookStartupLogin);
            else
            {
                FacebookLogin();

            }

        }

    }
    private void OnFacebookLoggedIn(ILoginResult result)
    {
        LoadingOverlay.instance.ShowLoadingFullscreen("Facebook login");
        // If result has no errors, it means we have authenticated in Facebook successfully
        if ((result == null || string.IsNullOrEmpty(result.Error) ) && result.Cancelled == false)
        {
            Debug.Log("Facebook Auth Complete! Access Token: " + AccessToken.CurrentAccessToken.TokenString + "\nLogging into PlayFab...");

            /*
             * We proceed with making a call to PlayFab API. We pass in current Facebook AccessToken and let it create
             * and account using CreateAccount flag set to true. We also pass the callback for Success and Failure results
             */


            

            bool isLinked = false;
            UserFacebookInfo info = GetComponent<Startup>().UserAccount.FacebookInfo;
            if (info != null && info.FacebookId  != null && info.FacebookId.Length > 0)
            {
                isLinked = true;
                
            }

            if (isLinked == false)
            {
                PlayFabClientAPI.LinkFacebookAccount(new LinkFacebookAccountRequest { AccessToken = AccessToken.CurrentAccessToken.TokenString }, OnPlayfabFacebookAuthComplete, OnPlayfabFacebookAuthFailed);
            }
            else
            {
                //PlayFabClientAPI.UpdateAvatarUrl(new UpdateAvatarUrlRequest { ImageUrl = avatarURL }, OnUpdateAvatarURL, OnPlayfabFacebookAuthFailed);
                //FB.API("me/picture?type=square&height=88&width=88", HttpMethod.GET, FbGetPicture);
                if( GetComponent<Startup>().avatarURL.Length>0 )
                {
                    LoadAvatarURL(GetComponent<Startup>().avatarURL);
                }
                MainMenuController.instance.SetFBLinked(true);
                LoadingOverlay.instance.DoneLoading("Facebook login");
            }



        }
        else
        {
            LoadingOverlay.instance.DoneLoading("Facebook login");
            // If Facebook authentication failed, we stop the cycle with the message
            Debug.Log("Facebook Auth Failed: " + result.Error + "\n" + result.RawResult);
        }

 
    }
    public void FacebookLogin()
    {
        Debug.Log("Facebook Auth Complete! Access Token: " + AccessToken.CurrentAccessToken.TokenString + "\nLogging into PlayFab...");
        PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest()
        {

            TitleId = PlayFabSettings.TitleId,
            AccessToken = AccessToken.CurrentAccessToken.TokenString,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
            {
                GetUserAccountInfo = true,
                GetPlayerProfile = true
            }
        },
        result2 =>
        {
            LoginSucess(result2);
        },
        error => Debug.LogError(error.GenerateErrorReport())); ;
        LoadingOverlay.instance.DoneLoading("Facebook login");
    }

    private void OnFacebookStartupLogin(ILoginResult result)
    {
        LoadingOverlay.instance.ShowLoadingFullscreen("Facebook login");
        // If result has no errors, it means we have authenticated in Facebook successfully
        if ((result == null || string.IsNullOrEmpty(result.Error)) && result.Cancelled == false)
        {
            FacebookLogin();


        }
        else
        {
            LoadingOverlay.instance.DoneLoading("Facebook login");
            // If Facebook authentication failed, we stop the cycle with the message
            Debug.Log("Facebook Auth Failed: " + result.Error + "\n" + result.RawResult);
        }


    }

    

    // When processing both results, we just set the message, explaining what's going on.
    private void OnPlayfabFacebookAuthComplete(LinkFacebookAccountResult result)
    {
        PlayerPrefs.SetInt("FacebookLink", 1);

        LoadingOverlay.instance.DoneLoading("Facebook login");
        Debug.Log("PlayFab Facebook Auth Complete. Session ticket: " + result.ToJson());
        string avatarURL = "https://graph.facebook.com/" + Facebook.Unity.AccessToken.CurrentAccessToken.UserId + "/picture?type=small";

        PlayFabClientAPI.UpdateAvatarUrl(new UpdateAvatarUrlRequest { ImageUrl = avatarURL }, OnUpdateAvatarURL, OnPlayfabFacebookAuthFailed);
        LoadAvatarURL(avatarURL);

        MainMenuController.instance.SetFBLinked(true);
        // FB.API("me/picture?type=square&height=88&width=88", HttpMethod.GET, FbGetPicture);

    }

    private void OnPlayfabFacebookAuthFailed(PlayFabError error)
    {
        if(error.Error == PlayFabErrorCode.LinkedAccountAlreadyClaimed)
        {
            Debug.Log("Should we merge?");
            MainMenuController.instance.ShowMerginAlert();


            if( MainMenuController.instance.FacebookButton.activeSelf )
            {
                MainMenuController.instance.ClickRecoveAccount();
            }


        }
        LoadingOverlay.instance.DoneLoading("Facebook login");
        Debug.Log("PlayFab Facebook Auth Failed: " + error.GenerateErrorReport());
    }

    private static void FbGetPicture(IGraphResult result)
    {
        if (result.Texture != null)
        {
            MainMenuController.instance.ProfilePicture.sprite = Sprite.Create(result.Texture, new Rect(0, 0, result.Texture.height, result.Texture.width), new Vector2());
            MainMenuController.instance.ProfilePicture.rectTransform.sizeDelta = new Vector2(88, 88);
            MainMenuController.instance.ProfilePicture.enabled = true;

        }

    }
    public void LoadAvatarURL(string aURL)
    {
       // Debug.Log(aURL + "&access_token=GG|817150566351647|GXmlbSYVrHYJ1h7CJj7t9cGxwrE");
    //    FB.API(aURL+ "&access_token=GG|817150566351647|GXmlbSYVrHYJ1h7CJj7t9cGxwrE", HttpMethod.GET, FbGetPicture);

        //StartCoroutine(GetFBProfilePicture(aURL));


       
        ProfilePictureManager.instance.SetPicture(aURL, Startup._instance.MyPlayfabID , MainMenuController.instance.ProfilePicture, CallBackProfilePicture);
       



    }
    public void CallBackProfilePicture()
    {

        MainMenuController.instance.ProfilePicture.enabled = true;



        MainMenuController.instance.ProfilePicture2.sprite = Sprite.Create((Texture2D)MainMenuController.instance.ProfilePicture.sprite.texture, new Rect(0, 0, MainMenuController.instance.ProfilePicture.sprite.texture.width , MainMenuController.instance.ProfilePicture.sprite.texture.height), new Vector2());
        MainMenuController.instance.ProfilePicture2.rectTransform.sizeDelta = new Vector2(88, 88);
        MainMenuController.instance.ProfilePicture2.enabled = true;

        PlayfabHelperFunctions.instance.ProfilePictureSprite = MainMenuController.instance.ProfilePicture2.sprite;

    }


    Sprite ProfilePictureSprite = null;
    //public static IEnumerator GetFBProfilePicture(string aURL)
    //{
    //    if (MainMenuController.instance == null)
    //    {
    //        yield break;
    //    }
    //    //string url = "https" + "://graph.facebook.com/10159330728290589/picture";
    //    WWW www = new WWW(aURL + "&access_token=GG|817150566351647|GXmlbSYVrHYJ1h7CJj7t9cGxwrE");
    //    yield return www;
    //    Texture2D profilePic = www.texture;
       
    //    MainMenuController.instance.ProfilePicture.sprite = Sprite.Create((Texture2D)profilePic, new Rect(0, 0, profilePic.height, profilePic.width), new Vector2());
    //    MainMenuController.instance.ProfilePicture.rectTransform.sizeDelta = new Vector2(88, 88);
    //    MainMenuController.instance.ProfilePicture.enabled = true;

    //    MainMenuController.instance.ProfilePicture2.sprite = Sprite.Create((Texture2D)profilePic, new Rect(0, 0, profilePic.height, profilePic.width), new Vector2());
    //    MainMenuController.instance.ProfilePicture2.rectTransform.sizeDelta = new Vector2(88, 88);
    //    MainMenuController.instance.ProfilePicture2.enabled = true;

    //    PlayfabHelperFunctions.instance.ProfilePictureSprite = MainMenuController.instance.ProfilePicture2.sprite;

    //}


    public void OnUpdateAvatarURL(EmptyResponse response)
    {
    }

    public void GetOtherUserData(string aPlayfabId)
    {


        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = aPlayfabId,
            Keys = null
        }, result => {

        Debug.Log("Done getting data");


        string xp = "0";
        if (result.Data.ContainsKey("XP") == true)
        {
            xp = result.Data["XP"].Value;
        }

            string rank = "0";
            if(result.Data.ContainsKey("Ranking"))
                rank = result.Data["Ranking"].Value;


            GameManager.instance.SetOpponentData(xp, rank);
 

        }, (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });

    }
    public void GetOtherUserDataProfile(string aPlayfabId, UserInfoWindow theWidnow)
    {


        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = aPlayfabId,
            Keys = null
        }, result => {


            theWidnow.SetData(result.Data);
    


        }, (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });

    }

    //public void UploadPhoto()
    //{


    //    var request = new PlayFab.DataModels.InitiateFileUploadsRequest
    //    {
    //        Entity = new PlayFab.DataModels.EntityKey { Id = "fileName", Type =  },
    //        FileNames = new List<string> { ActiveUploadFileName },
    //    };
    //    PlayFabDataAPI.InitiateFileUploads(request, OnInitFileUpload, OnInitFailed);
    //}
}
