using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;


public enum PlayfabCalls
{
    LoginWithCustomID,
    GetTitleData,
    GetPlayerProfile,
    GetSharedGroupData,
    GetUserData,
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

    public void Run()
    {
        if (myType == PlayfabCalls.LoginWithCustomID)
            LoginWithCustomID(playerID, onDone, onError);
        else if (myType == PlayfabCalls.GetTitleData)
            GetTitleData(onDoneTitleDataResult, onError);
        else if (myType == PlayfabCalls.GetPlayerProfile)
            GetPlayerProfile(onDonePlayerProfileResult, onError);
        else if (myType == PlayfabCalls.GetSharedGroupData)
            GetSharedGroupData(SharedGroupId,onDoneGetSharedGroupDataResult, onError);
        else if (myType == PlayfabCalls.GetUserData)
            GetUserData(onDoneGetUserDataResult, onError);
        


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
            Keys = null
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
        //for (int i = myPlayfabFunctionCall.Count - 1; i >= 0; i--)
        //{
        //    GUILayout.TextField(myPlayfabFunctionCall[i].myType.ToString());
        //}

 
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
    public void GetSharedGroupData(string aSharedGroupId, System.Action<GetSharedGroupDataResult> onDone, System.Action<PlayFabError> onError)
    {
        PlayfabFunctionCall a = new PlayfabFunctionCall();
        a.myType = PlayfabCalls.GetSharedGroupData;
        a.SharedGroupId = aSharedGroupId;
        a.onDoneGetSharedGroupDataResult = onDone;
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


}
