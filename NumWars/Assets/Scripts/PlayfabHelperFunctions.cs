﻿using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using UnityEngine;
using UnityEngine.UI;

public class PlayfabHelperFunctions : MonoBehaviour
{
    public bool TESTING_PLAYER_1=false;



    public GameObject _GameListItem;
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
        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
        {
            CreateAccount = true,
            CustomId = playerID,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
            {
                GetPlayerProfile = true
            }
        },
        result =>
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

                Refresh();
            }




        },
        error => Debug.LogError(error.GenerateErrorReport()));
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
            StartCoroutine(GetComponent<Startup>().DelayRefresh());

        }, error => Debug.LogError(error.GenerateErrorReport()));
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

    public void AddGameToSharedGroup(List<string> playfabId, string roomName)
    {

        string secondPlayer = "";
        if (playfabId.Count > 1)
            secondPlayer = playfabId[1];

        BoardData bd = new BoardData(playfabId[0], secondPlayer, "0", Board.instance.BoardTiles, roomName,new List<string>());
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

        PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
        {
            Entity = new PlayFab.CloudScriptModels.EntityKey()
            {
                Id = PlayFabSettings.staticPlayer.EntityId, //Get this from when you logged in,
                Type = PlayFabSettings.staticPlayer.EntityType, //Get this from when you logged in
            },
            FunctionName = "httptrigger1", //This should be the name of your Azure Function that you created.
            FunctionParameter = new Dictionary<string, object>() { { "player1_playfabID", roomName }, { "player2_playfabID", player2_playfabID } }, //This is the data that you would want to pass into your function.
            GeneratePlayStreamEvent = false //Set this to true if you would like this call to show up in PlayStream
        }, (ExecuteFunctionResult result) =>
        {
            LoadingOverlay.instance.DoneLoading("PlayFabCloudScriptAPI.ExecuteFunction");
            if (result.FunctionResultTooLarge ?? false)
            {
                Debug.Log("This can happen if you exceed the limit that can be returned from an Azure Function, See PlayFab Limits Page for details.");
                return;
            }
            Debug.Log($"The {result.FunctionName} function took {result.ExecutionTimeMilliseconds} to complete");
            Debug.Log($"Result: {result.FunctionResult.ToString()}");


            SetSharedDataForNewGame(player1_playfabID, player2_playfabID, player1_displayName, roomName);


        }, (PlayFabError error) =>
        {
            Debug.Log($"Opps Something went wrong: {error.GenerateErrorReport()}");
        });




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
    public void RemoveSharedGroupToGameList(string aSharedGroupName,System.Action onComplete= null)
    {
        Startup._instance.myData["MyGames"].Value = Startup._instance.myData["MyGames"].Value.Replace("," + aSharedGroupName,"");
        GetComponent<Startup>().myData["OldGames"].Value += "," + aSharedGroupName;
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
        BoardData bd = new BoardData(player1_playfabID, player2_playfabID, "0", Board.instance.BoardTiles,roomName, new List<string>());
        bd.player1_displayName = player1_displayName;
        bd.player2_displayName = Startup._instance.displayName;
        bd.player1_score = "0";
        bd.player2_score = "0";
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
            {"MyGames", ""},
            {"OldGames", ""},
        }
        },
        result =>
        {
            LoadingOverlay.instance.DoneLoading("UpdateUserData");
            Debug.Log("Successfully updated user data");
            StartCoroutine(GetComponent<Startup>().DelayRefresh());

        },
        error => {
            Debug.Log("Got error setting user data Ancestor to Arthur");
            Debug.Log(error.GenerateErrorReport());
        });
    }
    public void ChangeValueFor(string aData)
    {
        LoadingOverlay.instance.ShowLoading("UpdateUserData");

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
                {aData, Random.Range(1,100).ToString()}

            }
        },
       result =>
       {
           LoadingOverlay.instance.DoneLoading("UpdateUserData");

           Debug.Log("Successfully updated user data");
           StartCoroutine(GetComponent<Startup>().DelayRefresh());

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
            GameObject.Destroy(child.gameObject);
        }
        Startup._instance.openGamesList.Clear();
        //GetComponent<Startup>()._roomListLabel.text = "";
        string[] gameList = GetComponent<Startup>().myData["MyGames"].Value.Split(',');
        for(int i = 0;i < gameList.Length;i++)
        {
            if(gameList[i].Length>1)
            {
                LoadingOverlay.instance.ShowLoading("GetSharedGroupData");

                PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest()
                {
                    SharedGroupId = gameList[i]
                }, result =>
                {
                    LoadingOverlay.instance.DoneLoading("GetSharedGroupData");
                    foreach (KeyValuePair<string, SharedGroupDataRecord> entry in result.Data)
                    {
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
                            Startup._instance.SearchingForGameObject = (GameObject)GameObject.Instantiate(PlayfabHelperFunctions.instance.SearchingForGamePrefab, MainMenuController.instance._GameListParent);
                            Startup._instance.SearchingForGameObject.transform.SetAsFirstSibling();
                            Startup._instance.SearchingForGameObject.SetActive(true);
                        }

                        Startup._instance.openGamesList.Add(bd);
                    }


                    Startup._instance.FinishedGettingGameListCheckForOpenGames();

                }, (error) =>
                {
                    Debug.Log(error.GenerateErrorReport());
                });
            }
      
        }


    }
    public void RemoveRoomFromList(string aRoomName,System.Action onComplete=null)
    {
        LoadingOverlay.instance.ShowLoading("RemoveSharedGroupMembers");

        PlayFabClientAPI.RemoveSharedGroupMembers(new RemoveSharedGroupMembersRequest()
        { 
            SharedGroupId = aRoomName,
            PlayFabIds = new List<string>(){Startup._instance.MyPlayfabID}
        }, result =>
        {
            LoadingOverlay.instance.DoneLoading("RemoveSharedGroupMembers");
            for (int i = 0; i < Startup._instance.openGamesList.Count; i++)
            {
                if (Startup._instance.openGamesList[i].player1_PlayfabId == Startup._instance.MyPlayfabID && Startup._instance.openGamesList[i].player2_PlayfabId == "")
                {
                    Startup._instance.openGamesList.RemoveAt(i);
                    break;
                }
            }

            RemoveSharedGroupToGameList(aRoomName, onComplete);

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


        },
        error =>
        {
            Debug.Log("Got error making turn");
            Debug.Log(error.GenerateErrorReport());
            GameManager.instance.updateInProgress = false;

        });


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
            if (Startup._instance.openGamesList[i].player1_abandon == "1" || Startup._instance.openGamesList[i].player2_abandon == "1")
            {
                RemoveRoomFromList(Startup._instance.openGamesList[i].RoomName, RemoveAbandonedGamesCO);
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
                   RemoveRoomFromList(aRoomName);

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

}