using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
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
    SetPictureIE
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
        else if (myType == PlayfabCalls.LoginWithIOSDeviceID)
            LoginWithIOSDeviceID(playerID, onDone, onError);
        else if (myType == PlayfabCalls.GetOtherPlayerProfile)
            GetOtherPlayerProfile(playerID, onDonePlayerProfileResult, onError);
        else if (myType == PlayfabCalls.SetPictureIE)
            SetPictureIE( aURL, playerID,  aImage, onDoneN, onErrorN);



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
            GUILayout.TextField( PlayFab.Internal.PlayFabUnityHttp.AmountOfCalls[i], style);
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
    public void GetUserData(System.Action<GetUserDataResult> onDone, System.Action<PlayFabError> onError)
    {
        PlayfabFunctionCall a = new PlayfabFunctionCall();
        a.myType = PlayfabCalls.GetUserData;
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


}
