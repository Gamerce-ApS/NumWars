using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Internal;
using UnityEngine;
using UnityEngine.UI;

public enum PlayfabCalls
{
    LoginWithCustomID,
    GetTitleData,
    GetPlayerProfile,
    GetSharedGroupData,
    GetUserData,
    LoginWithIOSDeviceID,
    GetOtherPlayerProfile,
    SetPictureIE,
    UpdateUserData,
    UpdatePlayerStatistics,
    RemoveSharedGroupMembers,
    GetUserDataOldGames,
    UpdateUserDataGames,
    GetFilesRequest,
    InitiateFileUploads,
    GetAllAvatarGrouped,
    GetSharedDataGrouped2,
    UpdateUserDataGrouped,
    UpdatePlayerStatisticsGrouped,

};

public enum Status
{
    Waiting,
    Running,
    Finished
};
public class PlayfabFunctionCall
{

    public Status myStatus = Status.Waiting;
    public PlayfabCalls myType;

    public string playerID;
    public string SharedGroupId;
    public System.Action<LoginResult> onDone;
    public System.Action<GetTitleDataResult> onDoneTitleDataResult;
    public System.Action<GetPlayerProfileResult> onDonePlayerProfileResult;
    public System.Action<GetSharedGroupDataResult> onDoneGetSharedGroupDataResult;
    public System.Action<GetUserDataResult> onDoneGetUserDataResult;
    public System.Action<PlayFabError> onError;
    public bool shouldCancel = false;
    public System.Action onDoneN;
    public System.Action onErrorN;
    public string aURL;
    public Image aImage;

    public System.Action<string> OnDoneOnGetFileMeta;
    public System.Action<string> onErrorOnSharedFailure;



    public string aEntry;

    public string aValue;

    public int PlayerScore;
    public string myTable;

    public List<string> PlayFabIds;

    public string aGames;
    public string aOldGames;

    public System.Action<ExecuteCloudScriptResult> GetAllAvatarGroupedResult;
    public Dictionary<string, string> GroupedData;
    public List<StatisticUpdate> GroupedStatsData;
    

    public void Run()
    {
        if (myType == PlayfabCalls.LoginWithCustomID)
            LoginWithCustomID(playerID, onDone, onError);
        else if (myType == PlayfabCalls.GetTitleData)
            GetTitleData(onDoneTitleDataResult, onError);
        else if (myType == PlayfabCalls.GetPlayerProfile)
            GetPlayerProfile(onDonePlayerProfileResult, onError);
        else if (myType == PlayfabCalls.GetSharedGroupData)
            GetSharedGroupData(SharedGroupId, onDoneGetSharedGroupDataResult, onError);
        else if (myType == PlayfabCalls.GetUserData)
            GetUserData(onDoneGetUserDataResult, onError);
        else if (myType == PlayfabCalls.LoginWithIOSDeviceID)
            LoginWithIOSDeviceID(playerID, onDone, onError);
        else if (myType == PlayfabCalls.GetOtherPlayerProfile)
            GetOtherPlayerProfile(playerID, onDonePlayerProfileResult, onError);
        else if (myType == PlayfabCalls.SetPictureIE)
            SetPictureIE(aURL, playerID, aImage, onDoneN, onErrorN);
        else if (myType == PlayfabCalls.UpdateUserData)
            UpdateUserData(aEntry, aValue, onDoneN, onErrorN);
        else if (myType == PlayfabCalls.UpdatePlayerStatistics)
            UpdatePlayerStatistics(PlayerScore, myTable, onDoneN, onErrorN);
        else if (myType == PlayfabCalls.RemoveSharedGroupMembers)
            RemoveSharedGroupMembers(SharedGroupId, PlayFabIds, onDoneN, onErrorN);
        else if (myType == PlayfabCalls.GetUserDataOldGames)
            GetUserDataOldGames(onDoneGetUserDataResult, onError);
        else if (myType == PlayfabCalls.GetFilesRequest)
            GetFilesRequest(OnDoneOnGetFileMeta, onErrorOnSharedFailure);
        else if (myType == PlayfabCalls.InitiateFileUploads)
            InitiateFileUploads(OnDoneOnGetFileMeta, onErrorOnSharedFailure);
        else if (myType == PlayfabCalls.GetAllAvatarGrouped)
            GetAllAvatarGrouped(GetAllAvatarGroupedResult, onError);
        else if (myType == PlayfabCalls.GetSharedDataGrouped2)
            GetSharedDataGrouped2(playerID, GetAllAvatarGroupedResult, onError);
        else if (myType == PlayfabCalls.UpdateUserDataGrouped)
            UpdateUserDataGrouped(GroupedData, onDoneN, onErrorN);
        else if (myType == PlayfabCalls.UpdatePlayerStatisticsGrouped)
            UpdatePlayerStatisticsGrouped(GroupedStatsData, onDoneN, onErrorN);

        



    }


    void LoginWithCustomID(string playerID, System.Action<LoginResult> onDone, System.Action<PlayFabError> onError)
    {
        myStatus = Status.Running;

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
                  if (shouldCancel)
                      return;
                  myStatus = Status.Finished;
                  onDone(result);



              },
              error =>
              {
                  if (shouldCancel)
                      return;
                  myStatus = Status.Finished;
                  onError.Invoke(error);
              });
    }
    void LoginWithIOSDeviceID(string playerID, System.Action<LoginResult> onDone, System.Action<PlayFabError> onError)
    {
        myStatus = Status.Running;
        PlayFabClientAPI.LoginWithIOSDeviceID(new LoginWithIOSDeviceIDRequest()
        {

            TitleId = PlayFabSettings.TitleId,
            DeviceId = SystemInfo.deviceUniqueIdentifier,
            OS = SystemInfo.operatingSystem,
            DeviceModel = SystemInfo.deviceModel,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
            {
                GetUserAccountInfo = true,
                GetPlayerProfile = true
            }
        },
result =>
{




    if (shouldCancel)
        return;
    myStatus = Status.Finished;
    onDone(result);

   // LoginSucess(result);
},
error =>
{
    if (shouldCancel)
        return;
    myStatus = Status.Finished;
    onError.Invoke(error);
});







    }
    public void GetTitleData(System.Action<GetTitleDataResult> onDone, System.Action<PlayFabError> onError)
    {
        myStatus = Status.Running;
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(),
                  result2 => {
                      if (shouldCancel)
                          return;
                      myStatus = Status.Finished;
                      onDone.Invoke(result2);


                  },
                  error => {
                      if (shouldCancel)
                          return;
                      myStatus = Status.Finished;
                      onError.Invoke(error);
                  }
              );
    }
    public void GetAllAvatarGrouped(System.Action<ExecuteCloudScriptResult> onDone, System.Action<PlayFabError> onError)
    {
        myStatus = Status.Running;
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "GetAllAvatarGrouped",
            FunctionParameter = new Dictionary<string, object>() {
            { "PlayFabId", Startup.instance.MyPlayfabID }
        }
        }, result2 => {
            if (shouldCancel)
                return;
            myStatus = Status.Finished;
            onDone.Invoke(result2);


        },
        error => {
            if (shouldCancel)
                return;
            myStatus = Status.Finished;
            onError.Invoke(error);
        }
    );
    }

    public void GetSharedDataGrouped2(string aID, System.Action<ExecuteCloudScriptResult> onDone, System.Action<PlayFabError> onError)
    {
        myStatus = Status.Running;


        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "GetSharedDataGrouped2",
            FunctionParameter = new Dictionary<string, object>() {
            { "PlayFabId", aID }
        }
        }, result2 => {
            if (shouldCancel)
                return;
            myStatus = Status.Finished;
            onDone.Invoke(result2);


        },
        error => {
            if (shouldCancel)
                return;
            myStatus = Status.Finished;
            onError.Invoke(error);
        }
    );
    }



    public void UpdateUserData(string aEntry, string aValue, System.Action onDone, System.Action onError)
    {
        myStatus = Status.Running;
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
                {aEntry, aValue}

            },
            Permission = UserDataPermission.Public,
        },
        result =>
        {
            if (shouldCancel)
                return;
            myStatus = Status.Finished;
            Debug.Log("Successfully updated user data");
            if (onDone != null)
                onDone.Invoke();
               },
        error => {
            if (shouldCancel)
                return;
            Debug.Log("Got error setting user data Ancestor to Arthur");
           Debug.Log(error.GenerateErrorReport());
            myStatus = Status.Finished;
            if (onError != null)
                onError.Invoke();
        });
    }
    public void UpdateUserDataGrouped(Dictionary<string, string> aEntry, System.Action onDone, System.Action onError)
    {
        myStatus = Status.Running;
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = aEntry,
            Permission = UserDataPermission.Public,
        },
        result =>
        {
            if (shouldCancel)
                return;
            myStatus = Status.Finished;
            Debug.Log("Successfully updated user data");
            if (onDone != null)
                onDone.Invoke();
        },
        error => {
            if (shouldCancel)
                return;
            Debug.Log("Got error setting user data Ancestor to Arthur");
            Debug.Log(error.GenerateErrorReport());
            myStatus = Status.Finished;
            if (onError != null)
                onError.Invoke();
        });
    }



    
    public void GetUserDataOldGames(string aGames, string aOldGames, System.Action onDone, System.Action onError)
    {
        myStatus = Status.Running;
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
                                        {"MyGames",aGames},
                                        {"OldGames",aOldGames},
                                    }
        },
        result =>
        {
            if (shouldCancel)
                return;
            myStatus = Status.Finished;
            Debug.Log("Successfully updated user data");
            if (onDone != null)
                onDone.Invoke();
        },
        error => {
            if (shouldCancel)
                return;
            Debug.Log("Got error setting user data Ancestor to Arthur");
            Debug.Log(error.GenerateErrorReport());
            myStatus = Status.Finished;
            if (onError != null)
                onError.Invoke();
        });
    }


    
    public void UpdatePlayerStatistics(int aPlayerStats,string aTable, System.Action onDone, System.Action onError)
    {



        myStatus = Status.Running;
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate> {
            new StatisticUpdate {
                StatisticName = aTable,
                Value = aPlayerStats
            }
        }
        }, result =>
        {
            if (shouldCancel)
                return;
            myStatus = Status.Finished;
            Debug.Log("Successfully submitted high score");
            if(onDone != null)
            onDone.Invoke();
        },
        error => {
            if (shouldCancel)
                return;
            Debug.Log("Got error setting stats");
            Debug.Log(error.GenerateErrorReport());
            myStatus = Status.Finished;
            if (onError != null)
                onError.Invoke();
        });
    }
    public void UpdatePlayerStatisticsGrouped(List<StatisticUpdate> aEntry, System.Action onDone, System.Action onError)
    {

        myStatus = Status.Running;
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = aEntry
        }, result =>
        {
            if (shouldCancel)
                return;
            myStatus = Status.Finished;
            Debug.Log("Successfully submitted high score");
            if (onDone != null)
                onDone.Invoke();
        },
        error => {
            if (shouldCancel)
                return;
            Debug.Log("Got error setting stats");
            Debug.Log(error.GenerateErrorReport());
            myStatus = Status.Finished;
            if (onError != null)
                onError.Invoke();
        });
    }

    
    public void RemoveSharedGroupMembers(string SharedGroupId, List<string> aPlayfabId, System.Action onDone, System.Action onError)
    {
        myStatus = Status.Running;

        PlayFabClientAPI.RemoveSharedGroupMembers(new RemoveSharedGroupMembersRequest()
        {
            SharedGroupId = SharedGroupId,
            PlayFabIds = aPlayfabId
        }, result => {
            if (shouldCancel)
                return;
            myStatus = Status.Finished;
            Debug.Log("Successfully RemoveSharedGroupMembers");
            if (onDone != null)
                onDone.Invoke();

        }, error => {
            if (shouldCancel)
                return;
            myStatus = Status.Finished;
            Debug.Log("Got error setting stats");
            Debug.Log(error.GenerateErrorReport());
            if (onError != null)
                onError.Invoke();
        });




    }


    


    public void GetOtherPlayerProfile(string aPlayfabID, System.Action<GetPlayerProfileResult> onDone, System.Action<PlayFabError> onError)
    {
        myStatus = Status.Running;
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
        {
            PlayFabId = aPlayfabID,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,
                ShowAvatarUrl = true
            }
        },
        result =>
        {
            if (shouldCancel)
                return;
            myStatus = Status.Finished;

            if (onDone != null)
                onDone.Invoke(result);
        },
        error =>
        {
            if (shouldCancel)
                return;
            myStatus = Status.Finished;
            if (onError != null)
                onError.Invoke(error);
        }

        );
    }
    public void GetFilesRequest(System.Action<string> onDone, System.Action<string> onError)
    {
        myStatus = Status.Running;


       var request = new PlayFab.DataModels.GetFilesRequest { Entity = new PlayFab.DataModels.EntityKey { Id = PlayfabHelperFunctions.instance.entityId, Type = PlayfabHelperFunctions.instance.entityType } };
       PlayFabDataAPI.GetFiles(request, (OnGetFileMeta)=>
       {
           Debug.Log("Loading " + OnGetFileMeta.Metadata.Count + " files");

           foreach (var eachFilePair in OnGetFileMeta.Metadata)
           {
               PlayFabHttp.SimpleGetCall(eachFilePair.Value.DownloadUrl,
                result =>
                {
                    myStatus = Status.Finished;

                    if (onDone != null)
                        onDone.Invoke(Encoding.UTF8.GetString(result));
                }, 
                error =>
                {
                    myStatus = Status.Finished;
                    Debug.Log(error);
                    if (onError != null)
                        onError.Invoke(error);

                }
            );


           }


           if(OnGetFileMeta.Metadata.Count==0)
           {
               myStatus = Status.Finished;
               if (onError != null)
                   onError.Invoke("Error!!!");
           }


       }, (res)=> {
           Debug.LogError(res.GenerateErrorReport());
           myStatus = Status.Finished;
           if (onError != null)
               onError.Invoke("Error!!!");
       });

    }
    public void InitiateFileUploads(System.Action<string> onDone, System.Action<string> onError)
    {
        myStatus = Status.Running;

        var request = new PlayFab.DataModels.InitiateFileUploadsRequest
        {
            Entity = new PlayFab.DataModels.EntityKey { Id = PlayfabHelperFunctions.instance.entityId, Type = PlayfabHelperFunctions.instance.entityType },
            FileNames = new List<string> { "OldGamesData" },
        };
        PlayFabDataAPI.InitiateFileUploads(request, (response) => {


            string payloadStr = PlayerPrefs.GetString("OldGames");
            var payload = Encoding.UTF8.GetBytes(payloadStr);

                PlayFabHttp.SimplePutCall(response.UploadDetails[0].UploadUrl,
                    payload,
                    (data) =>
                    {
                        var request = new PlayFab.DataModels.FinalizeFileUploadsRequest
                        {
                            Entity = new PlayFab.DataModels.EntityKey { Id = PlayfabHelperFunctions.instance.entityId, Type = PlayfabHelperFunctions.instance.entityType },
                            FileNames = new List<string> { "OldGamesData" },
                        };
                        PlayFabDataAPI.FinalizeFileUploads(request, (OnUploadSuccess)=> {
                            Debug.Log("File upload success: " + "OldGamesData");

                            myStatus = Status.Finished;

                            if (onDone != null)
                                onDone.Invoke("1");


                        }, (error) => {
                            Debug.LogError(error.GenerateErrorReport());
                            myStatus = Status.Finished;
                            if (onError != null)
                                onError.Invoke(error.ErrorMessage);
                        });
                    },
                    error => { Debug.Log(error);
                        myStatus = Status.Finished;
                        if (onError != null)
                            onError.Invoke(error);
                    }
                );


        }, (error) =>
        {
            if (error.Error == PlayFabErrorCode.EntityFileOperationPending)
            {
                // This is an error you should handle when calling InitiateFileUploads, but your resolution path may vary
                var request = new PlayFab.DataModels.AbortFileUploadsRequest
                {
                    Entity = new PlayFab.DataModels.EntityKey { Id = PlayfabHelperFunctions.instance.entityId, Type = PlayfabHelperFunctions.instance.entityType },
                    FileNames = new List<string> { "OldGamesData" },
                };
                PlayFabDataAPI.AbortFileUploads(request, (result) => {

                    //   PlayfabCallbackHandler.instance.InitiateFileUploads(onDone, onError);
                    Debug.LogError(result);

                }, (OnSharedFailure) => { Debug.LogError(error.GenerateErrorReport()); });

            }
            else
            {
                Debug.LogError(error.GenerateErrorReport());
                myStatus = Status.Finished;
                if (onError != null)
                    onError.Invoke("Error!!!");
            }


        });
    }




    public void GetPlayerProfile(System.Action<GetPlayerProfileResult> onDone, System.Action<PlayFabError> onError)
    {
        myStatus = Status.Running;
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
        {
            PlayFabId = Startup._instance.MyPlayfabID,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true
            }
        },
        result =>
        {
            if (shouldCancel)
                return;
            myStatus = Status.Finished;



            onDone.Invoke(result);
        },
        error =>
        {
            if (shouldCancel)
                return;
            myStatus = Status.Finished;
            onError.Invoke(error);
        }

        );
    }

    public void GetSharedGroupData(string aSharedGroupId, System.Action<GetSharedGroupDataResult> onDone, System.Action<PlayFabError> onError)
    {
        myStatus = Status.Running;
        PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest()
        {
            SharedGroupId = aSharedGroupId
        }, result =>
        {
            if (shouldCancel)
                return;
            myStatus = Status.Finished;

            onDone.Invoke(result);

        }, (error) =>
        {
            if (shouldCancel)
                return;
            myStatus = Status.Finished;
            onError.Invoke(error);
        });
    }

    public void GetUserData( System.Action<GetUserDataResult> onDone, System.Action<PlayFabError> onError)
    {
        myStatus = Status.Running;

        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = Startup._instance.MyPlayfabID,
            Keys = new List<string> { "Achivments", "MyGames", "Ranking", "StatsData","XP" },
        }, result => {
            if (shouldCancel)
                return;
            myStatus = Status.Finished;


            onDone.Invoke(result);

        }, (error) => {
            if (shouldCancel)
                return;
            myStatus = Status.Finished;
            onError.Invoke(error);
        });
    }
    public void GetUserDataOldGames(System.Action<GetUserDataResult> onDone, System.Action<PlayFabError> onError)
    {
        myStatus = Status.Running;

        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = Startup._instance.MyPlayfabID,
            Keys = new List<string> { "OldGames" },
        }, result => {
            if (shouldCancel)
                return;
            myStatus = Status.Finished;


            onDone.Invoke(result);

        }, (error) => {
            if (shouldCancel)
                return;
            myStatus = Status.Finished;
            onError.Invoke(error);
        });
    }
    public void SetPictureIE(string aURL, string playfabID, Image aImage, System.Action onDone, System.Action onError)
    {
        myStatus = Status.Running;

        //PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        //{
        //    PlayFabId = Startup._instance.MyPlayfabID,
        //    Keys = null
        //}, result => {
        //    if (shouldCancel)
        //        return;
        //    myStatus = Status.Finished;
        //    onDone.Invoke(result);

        //}, (error) => {
        //    if (shouldCancel)
        //        return;
        //    myStatus = Status.Finished;
        //    onError.Invoke(error);
        //});

        PlayfabCallbackHandler.instance.SetPictureIEC(aURL, playfabID, aImage, () =>
        {
            myStatus = Status.Finished;
        }, onError);



    }

    //void OnGetFileMeta(PlayFab.DataModels.GetFilesResponse result)
    //{
    //    Debug.Log("Loading " + result.Metadata.Count + " files");

    //    foreach (var eachFilePair in result.Metadata)
    //    {
    //        GetActualFile(eachFilePair.Value);
    //    }

    //}
    //void GetActualFile(PlayFab.DataModels.GetFileMetadata fileData)
    //{

    //    PlayFabHttp.SimpleGetCall(fileData.DownloadUrl,
    //        result => {

    //            Debug.Log(Encoding.UTF8.GetString(result));

    //        }, // Finish Each SimpleGetCall
    //        error => { Debug.Log(error); }
    //    );
    //}
   



}

public class PlayfabCallbackHandler : MonoBehaviour
{

    public List<PlayfabFunctionCall> myPlayfabFunctionCall = new List<PlayfabFunctionCall>();
    public static PlayfabCallbackHandler _instance=null;
    public static PlayfabCallbackHandler instance
    {
        get
        {
            if(_instance == null)
                _instance = GameObject.Find("Startup").GetComponent<PlayfabCallbackHandler>();
            return _instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
     
    }
    public bool IsLoadingGames()
    {
        for(int i = 0; i < myPlayfabFunctionCall.Count;i++)
        {
            if( myPlayfabFunctionCall[i].myType == PlayfabCalls.GetSharedGroupData)
            {
                return true;
            }
            if (myPlayfabFunctionCall[i].myType == PlayfabCalls.GetUserData)
            {
                return true;
            }
            if (myPlayfabFunctionCall[i].myType == PlayfabCalls.GetPlayerProfile)
            {
                return true;
            }
        }
        return false;
    }
    public void CancelAllCalls()
    {
        for (int i = myPlayfabFunctionCall.Count - 1; i >= 0; i--)
        {
            myPlayfabFunctionCall[i].shouldCancel = true;
        }

        myPlayfabFunctionCall.Clear();
        if(LoadingOverlay.instance != null)
        LoadingOverlay.instance.LoadingCall.Clear();

  
    }
    // Update is called once per frame
    void Update()
    {
        bool isRunning = false;
        for(int i = myPlayfabFunctionCall.Count-1; i >= 0 ;i--)
        {
            if (myPlayfabFunctionCall[i].myStatus == Status.Running)
                isRunning = true;

            if (myPlayfabFunctionCall[i].myStatus == Status.Finished)
                myPlayfabFunctionCall.RemoveAt(i);
        }


        // Nothing is running start a call
        if(isRunning == false && myPlayfabFunctionCall.Count>0)
        {
            if(myPlayfabFunctionCall[0].myStatus == Status.Waiting)
            myPlayfabFunctionCall[0].Run();
        }

    }
    void OnGUI()
    {
        if (Startup.DEBUG_TOOLS == false)
            return;

        var style = GUI.skin.GetStyle("label");
        style.fontSize = 24; // whatever you set


        for (int i = myPlayfabFunctionCall.Count - 1; i >= 0; i--)
        {
            GUILayout.TextField(myPlayfabFunctionCall[i].myType.ToString(), style);
        }

        
        GUILayout.TextField("PhotonNetwork.IsConnected"+ PhotonNetwork.IsConnected, style);
        GUILayout.TextField("PlayFabClientAPI.IsClientLoggedIn()" + PlayFabClientAPI.IsClientLoggedIn(), style);

        GUI.color = Color.black;
        GUILayout.TextField("AmountOfCalls:", style);

        for (int i = 0; i < PlayFab.Internal.PlayFabUnityHttp.AmountOfCalls.Count; i++)
        {
            GUILayout.TextField(PlayFab.Internal.PlayFabUnityHttp.AmountOfCalls[i], style);
        }


    }

    public void LoginWithCustomID(string playerID, System.Action<LoginResult> onDone, System.Action<PlayFabError> onError)
    {
        PlayfabFunctionCall a = new PlayfabFunctionCall();
        a.myType = PlayfabCalls.LoginWithCustomID;
        a.playerID = playerID;
        a.onDone = onDone;
        a.onError = onError;
        myPlayfabFunctionCall.Add(a);
    }
    public void LoginWithIOSDeviceID(string playerID, System.Action<LoginResult> onDone, System.Action<PlayFabError> onError)
    {
        PlayfabFunctionCall a = new PlayfabFunctionCall();
        a.myType = PlayfabCalls.LoginWithIOSDeviceID;
        a.playerID = playerID;
        a.onDone = onDone;
        a.onError = onError;
        myPlayfabFunctionCall.Add(a);
    }
    public void GetTitleData(System.Action<GetTitleDataResult> onDone, System.Action<PlayFabError> onError)
    {
        PlayfabFunctionCall a = new PlayfabFunctionCall();
        a.myType = PlayfabCalls.GetTitleData;
        a.onDoneTitleDataResult = onDone;
        a.onError = onError;
        myPlayfabFunctionCall.Add(a);
    }

    public void GetPlayerProfile(System.Action<GetPlayerProfileResult> onDone, System.Action<PlayFabError> onError)
    {
        PlayfabFunctionCall a = new PlayfabFunctionCall();
        a.myType = PlayfabCalls.GetPlayerProfile;
        a.onDonePlayerProfileResult = onDone;
        a.onError = onError;
        myPlayfabFunctionCall.Add(a);
    }
    public void GetOtherPlayerProfile(string aPlayfab, System.Action<GetPlayerProfileResult> onDone, System.Action<PlayFabError> onError)
    {
        PlayfabFunctionCall a = new PlayfabFunctionCall();
        a.myType = PlayfabCalls.GetOtherPlayerProfile;
        a.onDonePlayerProfileResult = onDone;
        a.onError = onError;
        a.playerID = aPlayfab;
        myPlayfabFunctionCall.Add(a);
    }
    
    public void GetSharedGroupData(string aSharedGroupId, System.Action<GetSharedGroupDataResult> onDone, System.Action<PlayFabError> onError)
    {
        PlayfabFunctionCall a = new PlayfabFunctionCall();
        a.myType = PlayfabCalls.GetSharedGroupData;
        a.SharedGroupId = aSharedGroupId;
        a.onDoneGetSharedGroupDataResult = onDone;
        a.onError = onError;
        myPlayfabFunctionCall.Add(a);
    }
    public void GetAllAvatarGrouped( System.Action<ExecuteCloudScriptResult> onDone, System.Action<PlayFabError> onError)
    {
        PlayfabFunctionCall a = new PlayfabFunctionCall();
        a.myType = PlayfabCalls.GetAllAvatarGrouped;
        a.GetAllAvatarGroupedResult = onDone;
        a.onError = onError;
        myPlayfabFunctionCall.Add(a);
    }
    public void GetSharedDataGrouped2(string aID, System.Action<ExecuteCloudScriptResult> onDone, System.Action<PlayFabError> onError)
    {
        PlayfabFunctionCall a = new PlayfabFunctionCall();
        a.playerID = aID;
        a.myType = PlayfabCalls.GetSharedDataGrouped2;
        a.GetAllAvatarGroupedResult = onDone;
        a.onError = onError;
        myPlayfabFunctionCall.Add(a);
    }
    
    public void GetUserData(System.Action<GetUserDataResult> onDone, System.Action<PlayFabError> onError)
    {
        PlayfabFunctionCall a = new PlayfabFunctionCall();
        a.myType = PlayfabCalls.GetUserData;
        a.onDoneGetUserDataResult = onDone;
        a.onError = onError;
        myPlayfabFunctionCall.Add(a);
    }
    public void GetUserDataOldGames(System.Action<GetUserDataResult> onDone, System.Action<PlayFabError> onError)
    {
        PlayfabFunctionCall a = new PlayfabFunctionCall();
        a.myType = PlayfabCalls.GetUserDataOldGames;
        a.onDoneGetUserDataResult = onDone;
        a.onError = onError;
        myPlayfabFunctionCall.Add(a);
    }
    
    public void SetPictureIE(string aURL, string playfabID, Image aImage, System.Action onDoneN, System.Action onErrorN)
    {
        PlayfabFunctionCall a = new PlayfabFunctionCall();
        a.aURL = aURL;
        a.playerID = playfabID;
        a.aImage = aImage;
        a.myType = PlayfabCalls.SetPictureIE;
        a.onDoneN = onDoneN;
        a.onErrorN = onErrorN ;
        myPlayfabFunctionCall.Add(a);
    }
    public void UpdateUserData(string aEntry, string aValue, System.Action onDoneN, System.Action onErrorN)
    {
        PlayfabFunctionCall a = new PlayfabFunctionCall();
        a.aEntry = aEntry;
        a.aValue = aValue;
        a.myType = PlayfabCalls.UpdateUserData;

        a.onDoneN = onDoneN;
        a.onErrorN = onErrorN;
        myPlayfabFunctionCall.Add(a);
    }
    public void UpdateUserDataGrouped(Dictionary<string, string> aEntry, System.Action onDoneN, System.Action onErrorN)
    {
        PlayfabFunctionCall a = new PlayfabFunctionCall();
        a.GroupedData = aEntry;
        a.myType = PlayfabCalls.UpdateUserDataGrouped;

        a.onDoneN = onDoneN;
        a.onErrorN = onErrorN;
        myPlayfabFunctionCall.Add(a);
    }

    
    public void UpdateUserDataGames(string aGames, string aOldGames, System.Action onDoneN, System.Action onErrorN)
    {
        PlayfabFunctionCall a = new PlayfabFunctionCall();
        a.aGames = aGames;
        a.aOldGames = aOldGames;
        a.myType = PlayfabCalls.UpdateUserDataGames;

        a.onDoneN = onDoneN;
        a.onErrorN = onErrorN;
        myPlayfabFunctionCall.Add(a);
    }
    
    public void UpdatePlayerStatistics(int playerScore,string aTable, System.Action onDoneN, System.Action onErrorN)
    {
        PlayfabFunctionCall a = new PlayfabFunctionCall();
        a.PlayerScore = playerScore;
        a.myType = PlayfabCalls.UpdatePlayerStatistics;
        a.myTable = aTable;
        a.onDoneN = onDoneN;
        a.onErrorN = onErrorN;
        myPlayfabFunctionCall.Add(a);
    }
    public void UpdatePlayerStatisticsGrouped(List<StatisticUpdate> aEntry, System.Action onDoneN, System.Action onErrorN)
    {
        PlayfabFunctionCall a = new PlayfabFunctionCall();
        a.GroupedStatsData = aEntry;
        a.myType = PlayfabCalls.UpdatePlayerStatisticsGrouped;
        a.onDoneN = onDoneN;
        a.onErrorN = onErrorN;
        myPlayfabFunctionCall.Add(a);
    }
    public void RemoveSharedGroupMembers(string SharedGroupId, List<string> aPlayfabID, System.Action onDoneN, System.Action onErrorN)
    {
        PlayfabFunctionCall a = new PlayfabFunctionCall();
        a.SharedGroupId = SharedGroupId;
        a.myType = PlayfabCalls.RemoveSharedGroupMembers;
        a.PlayFabIds = aPlayfabID;
        a.onDoneN = onDoneN;
        a.onErrorN = onErrorN;
        myPlayfabFunctionCall.Add(a);
    }




    public void SetPictureIEC(string aURL, string playfabID, Image aImage, System.Action onDone, System.Action onError)
    {
        StartCoroutine(SetPictureIE2(aURL,  playfabID,  aImage,  onDone, onError));
    }
    public IEnumerator SetPictureIE2(string aURL, string playfabID, Image aImage, System.Action onDone, System.Action onError)
    {
        var profilePic = new Texture2D(2, 2, TextureFormat.RGBA32, false, false)
        {
            wrapMode = TextureWrapMode.Clamp
        };

        var fileName = Path.Combine(Application.persistentDataPath, playfabID + ".png");
        Debug.Log(fileName);
        if( File.Exists(fileName))
        {
            byte[] bytes = File.ReadAllBytes(fileName);

            profilePic.LoadImage(bytes);

        }
        else
        {

            WWW www = new WWW(aURL + "&access_token=GG|817150566351647|GXmlbSYVrHYJ1h7CJj7t9cGxwrE");
            yield return www;

             profilePic = www.texture;

            if (www.error != null)
                profilePic = ProfilePictureManager.instance.StandardPicture;
        }






        //If texture is null add a standard one.. This is for facebook error
        ProfileData pf = new ProfileData();
        pf.URL = aURL;
        pf.theSprite = Sprite.Create((Texture2D)profilePic, new Rect(0, 0, profilePic.width, profilePic.height), new Vector2());
        pf.playfabID = playfabID;
        ProfilePictureManager.instance.myPictures.Add(pf);
        if (aImage != null && pf != null)
        {
            aImage.sprite = pf.theSprite;
            aImage.rectTransform.sizeDelta = new Vector2(88, 88);

 
        }

        if (onDone != null)
            onDone.Invoke();


        File.WriteAllBytes(Application.persistentDataPath + "/"+playfabID+".png", profilePic.EncodeToPNG());

    }

    public void GetFilesRequest(System.Action<string> onDoneN, System.Action<string> onErrorN)
    {
        PlayfabFunctionCall a = new PlayfabFunctionCall();
        a.myType = PlayfabCalls.GetFilesRequest;
        a.OnDoneOnGetFileMeta = onDoneN;
        a.onErrorOnSharedFailure = onErrorN;
        myPlayfabFunctionCall.Add(a);
    }
    public void InitiateFileUploads(System.Action<string> onDoneN, System.Action<string> onErrorN)
    {
        PlayfabFunctionCall a = new PlayfabFunctionCall();
        a.myType = PlayfabCalls.InitiateFileUploads;
        a.OnDoneOnGetFileMeta = onDoneN;
        a.onErrorOnSharedFailure = onErrorN;
        myPlayfabFunctionCall.Add(a);
    }
    //public void GetFilesRequest()
    //{
    //    var request = new PlayFab.DataModels.GetFilesRequest { Entity = new PlayFab.DataModels.EntityKey { Id = PlayfabHelperFunctions.instance.entityId, Type = PlayfabHelperFunctions.instance.entityType } };
    //    PlayFabDataAPI.GetFiles(request, OnGetFileMeta, OnSharedFailure);
    //}
    public void SetOldGameDataFromPlayerPrefs()
    {

    }





}
