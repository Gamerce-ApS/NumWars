using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using LoginIdentityProvider = PlayFab.ClientModels.LoginIdentityProvider;
using LoginResult = PlayFab.ClientModels.LoginResult;

public class PlayfabHelperFunctions : MonoBehaviour
{
    public bool TESTING_PLAYER_1=false;



    public GameObject _GameListItem;
    public GameObject _FinishedTitleListItem;
    public GameObject SearchingForGamePrefab;


    public static PlayfabHelperFunctions instance;


    public void Login()
    {
        string playerID = "";
        if (TESTING_PLAYER_1)
            playerID = "asdafsfsdf";
        else
            playerID = "asdafsfsdf2";
        instance = this;
        LoadingOverlay.instance.ShowLoading("LoginWithCustomID");



#if UNITY_EDITOR
        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
        {
            CreateAccount = true,
            CustomId = playerID,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
            { GetUserAccountInfo =true,
                GetPlayerProfile = true,
               
            }
        },
        result =>
        {
            LoginSucess(result);
        },
        error => Debug.LogError(error.GenerateErrorReport()));









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

            //for (int i = 0; i < Startup._instance.PlayerProfile.LinkedAccounts.Count; i++)
            //{
            //    if (Startup._instance.PlayerProfile.LinkedAccounts[i].Platform == LoginIdentityProvider.Facebook)
            //    {
            //        isLinked = true;

            //    }
            //}

         //   if(isLinked)
            {
                FacebookInit();


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

                                Board.instance.GenerateStartBoard();
                                BoardData bd = new BoardData(playfabId, playfabId2, "0", Board.instance.BoardTiles, roomName, new List<string>(), Board.instance.GetTilesLeft(), "0", Board.instance.p1_tiles, Board.instance.p2_tiles);
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

        BoardData bd = new BoardData(playfabId[0], secondPlayer, "0", Board.instance.BoardTiles, roomName,new List<string>(), new List<string>(), "0", new List<string>(), new List<string>());
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

    public void AddPlayerToSharedGroup(string player1_playfabID, string player2_playfabID,string player1_displayName, string roomName)
    {

        LoadingOverlay.instance.ShowLoading("PlayFabCloudScriptAPI.ExecuteFunction");

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
            SetSharedDataForNewGame(player1_playfabID, player2_playfabID, player1_displayName, roomName);
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
          
        

        GetComponent<Startup>().myData["OldGames"].Value += "[splitter]" + st;
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
    public void SetSharedDataForNewGame(string player1_playfabID, string player2_playfabID,string player1_displayName, string roomName)
    {
        Board.instance.GenerateStartBoard();
        BoardData bd = new BoardData(player1_playfabID, player2_playfabID, "0", Board.instance.BoardTiles,roomName, new List<string>(), Board.instance.GetTilesLeft(), "0", Board.instance.p1_tiles, Board.instance.p2_tiles);
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
        LoadingOverlay.instance.ShowLoading("UpdateUserData");
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
            {"Ranking", "0"},
            {"Picture", "0"},
            {"MyGames", ","},
            {"OldGames", "[splitter][splitter]"},
            {"StatsData", " "},
            
        }
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

            }
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
        LoadingOverlay.instance.ShowLoading("GetUserData");

        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = GetComponent<Startup>().MyPlayfabID,
            Keys = null
        }, result => {

            LoadingOverlay.instance.DoneLoading("GetUserData");

            Debug.Log("Done getting data");

            GetComponent<Startup>().myData = result.Data;




            //GetComponent<Startup>()._infoLabel.text = GetComponent<Startup>().displayName + "\nPicture: " + GetComponent<Startup>().myData["Picture"].Value + "\n" + "Ranking: " + GetComponent<Startup>().myData["Ranking"].Value;
            MainMenuController.instance._Name.text = GetComponent<Startup>().displayName;
            MainMenuController.instance._Thropies.text = GetComponent<Startup>().myData["Ranking"].Value;

            UpdateGameList();

            //if (result.Data == null || !result.Data.ContainsKey("Ancestor")) Debug.Log("No Ancestor");
            //else Debug.Log("Ancestor: " + result.Data["Ancestor"].Value);
        }, (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });

    }
    public void UpdateGameList()
    {
        foreach (Transform child in MainMenuController.instance._GameListParent.transform)
        {

            if(child.gameObject.name.Contains("SerachingForGame") == false)
            {
                GameObject.Destroy(child.gameObject);
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

                Debug.Log(CompressString.StringCompressor.DecompressString(oldGameList[i]));
            }
             
        }


        string jsonAIBoard = PlayerPrefs.GetString("AIGame", "");
        if (jsonAIBoard  != "")
        {
            BoardData aiGameBoard = new BoardData(jsonAIBoard);

           
                GameObject obj2 = (GameObject)GameObject.Instantiate(_GameListItem, MainMenuController.instance._GameListParent);
                obj2.GetComponent<GameListItem>().Init(aiGameBoard, false, true);
            

        }




        bool shouldAddOldGamesNow = true;
        for (int i = 0;i < gameList.Length;i++)
        {
            if(gameList[i].Length>1)
            {
                LoadingOverlay.instance.ShowLoading("GetSharedGroupData");
                shouldAddOldGamesNow = false;
                PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest()
                {
                    SharedGroupId = gameList[i]
                }, result =>
                {
                    LoadingOverlay.instance.DoneLoading("GetSharedGroupData");
                    foreach (KeyValuePair<string, SharedGroupDataRecord> entry in result.Data)
                    {
                        if (entry.Key == "Chat")
                            continue;

                        BoardData bd = new BoardData(entry.Value.Value);
                        //GetComponent<Startup>()._roomListLabel.text += "" + bd.player1_PlayfabId + " vs " + bd.player2_PlayfabId + " turn: " + bd.playerTurn + "\n";

                        if(bd.player1_abandon == "1" || bd.player2_abandon == "1")
                        {

                        }
                        else if(bd.player2_PlayfabId != "")
                        {
                            GameObject obj = (GameObject)GameObject.Instantiate(_GameListItem, MainMenuController.instance._GameListParent);
                            obj.GetComponent<GameListItem>().Init(bd);

                        }else
                        {
                            if(Startup._instance.SearchingForGameObject == null)
                            {
                                Startup._instance.SearchingForGameObject = (GameObject)GameObject.Instantiate(PlayfabHelperFunctions.instance.SearchingForGamePrefab, MainMenuController.instance._GameListParent);
                                Startup._instance.SearchingForGameObject.transform.SetAsFirstSibling();
                                Startup._instance.SearchingForGameObject.SetActive(true);
                            }
                            else
                            {
                                Startup._instance.SearchingForGameObject.transform.SetAsFirstSibling();
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
        LoadingOverlay.instance.ShowLoading("RemoveSharedGroupMembers");

        PlayFabClientAPI.RemoveSharedGroupMembers(new RemoveSharedGroupMembersRequest()
        { 
            SharedGroupId = aRoomName,
            PlayFabIds = new List<string>(){Startup._instance.MyPlayfabID}
        }, result =>
        {
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
        LoadingOverlay.instance.ShowLoading("GetSharedGroupData");

        PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest()
                {
                    SharedGroupId = aGame
                }, result =>
                {
                    if (LoadingOverlay.instance != null)
                        LoadingOverlay.instance.DoneLoading("GetSharedGroupData");

                    foreach (KeyValuePair<string, SharedGroupDataRecord> entry in result.Data)
                    {
                        if (entry.Key == "Chat")
                            continue;
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
    public void SendNextTurn(BoardData aBoarddata)
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
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "ChallengePlayer",
            FunctionParameter = new Dictionary<string, object>() {
            { "TargetId", aId },
            { "Title", title },
            { "Message", message },

        }
        }, null, error => Debug.LogError(error.GenerateErrorReport()));
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
            if (Startup._instance.openGamesList[i].player1_abandon == "1" || Startup._instance.openGamesList[i].player2_abandon == "1" || int.Parse(Startup._instance.openGamesList[i].EmptyTurns) >= 4)
            {
                RemoveRoomFromList(Startup._instance.openGamesList[i].RoomName, Startup._instance.openGamesList[i].GetJson(), RemoveAbandonedGamesCO);
                Startup._instance.openGamesList.RemoveAt(i);
                return;
            }
        }
       // 


    }

    public void DeleteGame(string aRoomName)
    {
        SetAbandomeForPlayerInGame(aRoomName);
  

    }
    public void SetAbandomeForPlayerInGame(string aRoomName)
    {
        LoadingOverlay.instance.ShowLoading("GetSharedGroupData");

        PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest()
        {
            SharedGroupId = aRoomName
        }, result =>
        {
            LoadingOverlay.instance.DoneLoading("GetSharedGroupData");
            foreach (KeyValuePair<string, SharedGroupDataRecord> entry in result.Data)
            {
                if (entry.Key == "Chat")
                    continue;
                BoardData bd = new BoardData(entry.Value.Value);

                bd.SetPlayerAbandome();

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
                   LoadingOverlay.instance.DoneLoading("UpdateSharedGroupData");
                   Debug.Log("Successfully updated SetAbadome in room name:" + aRoomName);
                   RemoveRoomFromList(aRoomName, bd.GetJson());

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
        FB.Init(OnFacebookInitialized);
    }
    public void FacebookLink()
    {
        FB.LogInWithReadPermissions(null, OnFacebookLoggedIn);
    }
    public void FacebookUnLink()
    {
        PlayFabClientAPI.UnlinkFacebookAccount(new UnlinkFacebookAccountRequest {  }, OnUnlinked, null);

    }
    private void OnUnlinked( UnlinkFacebookAccountResult result)
    {
        Debug.Log("unlinked facebook");
        MainMenuController.instance.SetFBLinked(false);
    }
    private void OnFacebookInitialized()
    {
        Debug.Log("Logging into Facebook...");

        // Once Facebook SDK is initialized, if we are logged in, we log out to demonstrate the entire authentication cycle.
        //if (FB.IsLoggedIn)
        //    FB.LogOut();

        // We invoke basic login procedure and pass in the callback to process the result



        // FB.API("me/picture?type=square&height=88&width=88", HttpMethod.GET, FbGetPicture);

        if(GetComponent<Startup>().avatarURL != null)
        if (GetComponent<Startup>().avatarURL.Length > 0)
            LoadAvatarURL(GetComponent<Startup>().avatarURL);

    }
    private void OnFacebookLoggedIn(ILoginResult result)
    {
        
        // If result has no errors, it means we have authenticated in Facebook successfully
        if ((result == null || string.IsNullOrEmpty(result.Error) ) && result.Cancelled == false)
        {
            Debug.Log("Facebook Auth Complete! Access Token: " + AccessToken.CurrentAccessToken.TokenString + "\nLogging into PlayFab...");

            /*
             * We proceed with making a call to PlayFab API. We pass in current Facebook AccessToken and let it create
             * and account using CreateAccount flag set to true. We also pass the callback for Success and Failure results
             */


            

            bool isLinked = false;
            if (GetComponent<Startup>().UserAccount.FacebookInfo != null && GetComponent<Startup>().UserAccount.FacebookInfo.FacebookId.Length > 0)
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
            }



        }
        else
        {
            // If Facebook authentication failed, we stop the cycle with the message
            Debug.Log("Facebook Auth Failed: " + result.Error + "\n" + result.RawResult);
        }
    }
    // When processing both results, we just set the message, explaining what's going on.
    private void OnPlayfabFacebookAuthComplete(LinkFacebookAccountResult result)
    {
        
        Debug.Log("PlayFab Facebook Auth Complete. Session ticket: " + result.ToJson());
        string avatarURL = "https://graph.facebook.com/" + Facebook.Unity.AccessToken.CurrentAccessToken.UserId + "/picture?type=small";

        PlayFabClientAPI.UpdateAvatarUrl(new UpdateAvatarUrlRequest { ImageUrl = avatarURL }, OnUpdateAvatarURL, OnPlayfabFacebookAuthFailed);
        LoadAvatarURL(avatarURL);

        MainMenuController.instance.SetFBLinked(true);
        // FB.API("me/picture?type=square&height=88&width=88", HttpMethod.GET, FbGetPicture);

    }

    private void OnPlayfabFacebookAuthFailed(PlayFabError error)
    {
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

        StartCoroutine(GetFBProfilePicture(aURL));
    }

    public static IEnumerator GetFBProfilePicture(string aURL)
    {

        //string url = "https" + "://graph.facebook.com/10159330728290589/picture";
        WWW www = new WWW(aURL + "&access_token=GG|817150566351647|GXmlbSYVrHYJ1h7CJj7t9cGxwrE");
        yield return www;
        Texture2D profilePic = www.texture;

        MainMenuController.instance.ProfilePicture.sprite = Sprite.Create((Texture2D)profilePic, new Rect(0, 0, profilePic.height, profilePic.width), new Vector2());
        MainMenuController.instance.ProfilePicture.rectTransform.sizeDelta = new Vector2(88, 88);
        MainMenuController.instance.ProfilePicture.enabled = true;
        




    }


    public void OnUpdateAvatarURL(EmptyResponse response)
    {
    }


}
