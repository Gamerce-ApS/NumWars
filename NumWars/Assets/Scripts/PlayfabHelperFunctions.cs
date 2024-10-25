﻿using System.Collections;
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
using System.Text;
using UnityEngine.SignInWithApple;
using System;
using PlayerProfileModel = PlayFab.ClientModels.PlayerProfileModel;
using System.IO.Compression;
using System.IO;
using PlayFab.Json;
//using PlayFab.PfEditor.Json;
using JsonObject = PlayFab.Json.JsonObject;
using PlayFab.DataModels;
using PlayFab.Internal;
//using AppodealAds.Unity.Api;
//using AppodealAds.Unity.Common;
using System.Threading;
using Newtonsoft.Json;
using System.Web;

#if UNITY_IOS
using Unity.Advertisement.IosSupport;

#endif

[Serializable]
public class SendRequest
{
    public string GetMembers;
    public string Keys;
    public string SharedGroupId;
    public string AuthenticationContext;
}
[Serializable]
public class messageValue
{
    public string[] sharedgroupdataArray;
}


public class PlayfabHelperFunctions : MonoBehaviour
{
    public bool TESTING_PLAYER_1=false;



    public GameObject _GameListItem;
    public GameObject _FinishedTitleListItem;
    public GameObject SearchingForGamePrefab;

    public List<int> LevelSettings = new List<int>();

    public static PlayfabHelperFunctions instance;

#if UNITY_IOS
 //   private IAppleAuthManager appleAuthManager;
#endif

    public void ReLogin()
    {





        if (PlayerPrefs.HasKey("AppleUserIdKey"))
        {
            MainMenuController.instance.SetAppleLinked(true);
            StartCoroutine(DoAppleQuickLogin());

        }
        else if (PlayerPrefs.HasKey("FacebookLink"))
        {
            MainMenuController.instance.SetFBLinked(true);
            OnFacebookInitialized();
        }
        else
        {
            Login();
        }
    }

    public void Login()
    {


      //  PlayfabCallbackHandler.instance.UpdateUserDataGrouped(
      //new Dictionary<string, string>() {
      //          {"pushnotification_tooken", Startup.instance.MyPlayfabID}}, null, null);

        Startup.instance.StartPushSer();

        if (Startup.instance.MyPlayfabID.Length>0)
        {
            if (LoadingOverlay.instance != null)
                LoadingOverlay.instance.ShowLoadingFullscreen("Updating..");
            Refresh();
            return;
        }


        string playerID = "";
        if (TESTING_PLAYER_1)
            playerID = "asdafsfsdf";
        else
            playerID = "asdafsfsdf2";

        //PlayerPrefs.SetString("FacebookTooken", "10159330728290589");
        //PlayerPrefs.SetInt("FacebookLink", 1);

        //playerID = "sdfsdfewwr2";
        //playerID = "NewUser1";
        //playerID = "PaxMM";
        //playerID = "PatrikB_3";

        // playerID = "asdafsfsdf3";

        //  playerID = "asdafsfsdf4";
        //     playerID = "peterag";
        //playerID = "peterag2";

        // playerID = "A4244F897FA3FE09";


        // playerID = "Villads123";
        //   playerID = "PaxMM";
        // playerID = "steffen123";
        //   playerID = "hmt";
        //   playerID = "kasper";
        //   playerID = "mike";
        // playerID = "Kvotekongen";
        //    playerID = "skatemin";
        //    playerID = "Kristine";
        //   playerID = "vibe";
        // playerID = "maj";
        //   playerID = "annellla";
        // playerID = "kudis";
        //playerID = "asdafe3eewrer";
        // playerID = "badboymike";

        playerID = "newacc1";
        instance = this;
        LoadingOverlay.instance.ShowLoadingFullscreen("LoginWithCustomID");



        if (FB.IsInitialized)
            OnFacebookInitialized();
        else
        {
            try
            {

                Debug.Log("1: OnFacebookInitialized");
                FB.Init(OnFacebookInitialized);

            }
            catch
            {
                Debug.Log("2: OnFacebookInitialized");
                OnFacebookInitialized();
            }

        }


        if (PlayerPrefs.HasKey("FacebookLink"))
        {
            return;
        }

        if (PlayerPrefs.HasKey("AppleUserIdKey"))
        {
            StartCoroutine( DoAppleQuickLogin() );
            return;
        }










#if UNITY_EDITOR


        PlayfabCallbackHandler.instance.LoginWithCustomID(playerID, LoginSucess, error =>{Debug.LogError(error.GenerateErrorReport());} );


        //PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
        //{
        //    CreateAccount = true,
        //    CustomId = playerID,
        //    InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
        //    {
        //        GetUserAccountInfo = true,
        //        GetPlayerProfile = true,

        //    }
        //},
        //result =>
        //{
        //    LoginSucess(result);
        //},
        //error =>
        //{
        //    Debug.LogError(error.GenerateErrorReport());
        //});


#elif UNITY_IOS

        PlayfabCallbackHandler.instance.LoginWithIOSDeviceID(playerID, LoginSucess, error =>{Debug.LogError(error.GenerateErrorReport());} );


//    PlayFabClientAPI.LoginWithIOSDeviceID(new LoginWithIOSDeviceIDRequest()
//        { 

//            TitleId = PlayFabSettings.TitleId,
//            DeviceId = SystemInfo.deviceUniqueIdentifier,
//            OS = SystemInfo.operatingSystem,
//            DeviceModel = SystemInfo.deviceModel,
//            CreateAccount = true,
//            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
//            {
//                GetUserAccountInfo =true,
//                GetPlayerProfile = true
//            }
//        },
//result =>
//{
//    LoginSucess(result);
//},
// error => Debug.LogError(error.GenerateErrorReport()));

#elif UNITY_ANDROID

        PlayfabCallbackHandler.instance.LoginWithIOSDeviceID(playerID, LoginSucess, error =>{Debug.LogError(error.GenerateErrorReport());} );

    //    PlayFabClientAPI.LoginWithAndroidDeviceID(new LoginWithAndroidDeviceIDRequest()
    //    { 

    //        TitleId = PlayFabSettings.TitleId,
    //        AndroidDeviceId = SystemInfo.deviceUniqueIdentifier,
    //        OS = SystemInfo.operatingSystem,
    //        AndroidDevice = SystemInfo.deviceModel,
    //        CreateAccount = true,
    //        InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
    //        {
    //            GetUserAccountInfo = true,
    //            GetPlayerProfile = true
    //        }
    //    },
    //result =>
    //{
    //    LoginSucess(result);
    //},
    // error => Debug.LogError(error.GenerateErrorReport()));
#endif




    }
    public SignInWithApple signInWithApple;

    public IEnumerator DoAppleQuickLogin()
    {


        Debug.Log("DoAppleQuickLogin");


        yield return new WaitForSeconds(0.2f);


        if (PlayerPrefs.HasKey("AppleidentityToken"))
        {
            string tooken = PlayerPrefs.GetString("AppleidentityToken");

            string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    {"DefaultAchivements", Startup._instance.myAchivmentController.GetDefault()},});

            AWSBackend.instance.AWSClientAPI("phpBackend/LoginWithCustomID.php?appleID=" + tooken, jsonString,
                  result =>
                  {
                      Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);

                      LoginResult lr = new LoginResult();
                      lr.AuthenticationContext = new PlayFabAuthenticationContext();
                      lr.AuthenticationContext.PlayFabId = dic["PlayerID"];
                      lr.InfoResultPayload = new GetPlayerCombinedInfoResultPayload();
                      lr.InfoResultPayload.PlayerProfile = new PlayerProfileModel();
                      lr.InfoResultPayload.PlayerProfile.DisplayName = dic["DisplayName"];
                      lr.InfoResultPayload.AccountInfo = new UserAccountInfo();
                      lr.InfoResultPayload.AccountInfo.TitleInfo = new UserTitleInfo();
                      lr.InfoResultPayload.AccountInfo.TitleInfo.AvatarUrl = dic["avatarURL"];
                      lr.NewlyCreated = false;

                      lr.InfoResultPayload.AccountInfo.AppleAccountInfo = new UserAppleIdInfo();
                      lr.InfoResultPayload.AccountInfo.AppleAccountInfo.AppleSubjectId = PlayerPrefs.GetString("AppleidentityToken");


                      LoginSucess(lr);

                  },
                  error =>
                  {
                      Debug.LogError(error);

                      //PlayerPrefs.DeleteKey("FacebookLink");
                      //LoadingOverlay.instance.LoadingCall.Clear();
                      //PlayfabHelperFunctions.instance.ReLogin();
                      //SceneManager.LoadScene(0);
                      //Startup._instance.Refresh(0.1f);

                      PlayerPrefs.DeleteKey("AppleidentityToken");
                      PlayerPrefs.DeleteKey("AppleUserIdKey");
                      LoadingOverlay.instance.LoadingCall.Clear();
                      PlayfabHelperFunctions.instance.ReLogin();
                      SceneManager.LoadScene(0);
                      Startup._instance.Refresh(0.1f);
             

                  });


            //string tooken = PlayerPrefs.GetString("AppleidentityToken");
            //PlayFabClientAPI.LoginWithApple(new LoginWithAppleRequest()
            //{

            //    TitleId = PlayFabSettings.TitleId,
            //    IdentityToken = tooken,
            //    InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
            //    {
            //        GetUserAccountInfo = true,
            //        GetPlayerProfile = true
            //    }
            //},
            //result2 =>
            //{
            //    LoginSucess(result2);
            //},
            //error =>
            //{
            //    PlayerPrefs.DeleteKey("AppleidentityToken");
            //    PlayerPrefs.DeleteKey("AppleUserIdKey");
            //    SceneManager.LoadScene(0);
            //    PlayfabHelperFunctions.instance.ReLogin();

            //    Debug.LogError(error.GenerateErrorReport());
            //});

         }
        else
        {
            signInWithApple.Login((args) =>
            {
                if (!string.IsNullOrEmpty(args.error))
                {
                    Debug.Log("Apple -> sign in error:" + args.error);

                    PlayerPrefs.DeleteKey("AppleidentityToken");
                    PlayerPrefs.DeleteKey("AppleUserIdKey");
                    SceneManager.LoadScene(0);
                    PlayfabHelperFunctions.instance.ReLogin();

                    return;
                }
                string idToken = args.userInfo.idToken;
                //LogMessage("Apple --> Identity Token:\n" + idToken);



                PlayFabClientAPI.LoginWithApple(new LoginWithAppleRequest()
                {

                    TitleId = PlayFabSettings.TitleId,
                    IdentityToken = idToken,
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
                LoadingOverlay.instance.DoneLoading("Apple login");
                PlayerPrefs.DeleteKey("AppleidentityToken");
                PlayerPrefs.DeleteKey("AppleUserIdKey");
                SceneManager.LoadScene(0);
                PlayfabHelperFunctions.instance.ReLogin();
                // use idToken to login to PlayFab or any other web services
            });
        }
 






//#if UNITY_IOS_
        //var quickLoginArgs = new AppleAuthQuickLoginArgs();

        //this.appleAuthManager.QuickLogin(
        //    quickLoginArgs,
        //    credential =>
        //    {
        //        // Received a valid credential!
        //        // Try casting to IAppleIDCredential or IPasswordCredential

        //        // Previous Apple sign in credential
        //        var appleIdCredential = credential as IAppleIDCredential;

        //        // Saved Keychain credential (read about Keychain Items)
        //             var passwordCredential = credential as IPasswordCredential;

        //        var identityToken = Encoding.UTF8.GetString(
        //       appleIdCredential.IdentityToken,
        //       0,
        //       appleIdCredential.IdentityToken.Length);
        //               Debug.Log("LoginWithApplePlayfab");
        //        PlayFabClientAPI.LoginWithApple(new LoginWithAppleRequest()
        //        { 

        //            TitleId = PlayFabSettings.TitleId,
        //            IdentityToken = identityToken,
        //            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
        //            {
        //                GetUserAccountInfo = true,
        //                GetPlayerProfile = true
        //            }
        //        },
        //            result2 =>
        //            {
        //                StartCoroutine(AfteTimeLogin(result2));

        //            },
        //            error => Debug.LogError(error.GenerateErrorReport())); ;
        //                            LoadingOverlay.instance.DoneLoading("Apple login");


        //    },
        //    error =>
        //    {
        //            Debug.Log("Quick login failed. The user has never used Sign in With Apple on your app. Go to login screen");
        //            Debug.Log(error.Domain);
        //        Debug.Log(error.Code);


        //        // Quick login failed. The user has never used Sign in With Apple on your app. Go to login screen
        //    });
//#endif
    }
    public string entityId = "";
    public string entityType = "";

    public void LoginSucess(LoginResult result)
    {

        GetOpenOnlineGamesForLabel(); 
        //entityId = result.EntityToken.Entity.Id;
        // The expected entity type is title_player_account.
        //entityType = result.EntityToken.Entity.Type;


        Debug.Log("LoginSucess_ DoAppleQuickLogin");


        LoadingOverlay.instance.ShowLoadingFullscreen("Loading data!");

       
        PlayfabCallbackHandler.instance.GetTitleData(result2 => {
            if (result2.Data == null || !result2.Data.ContainsKey("TilesAmount"))
                Debug.Log("No TilesAmount");
            else
            {
                Startup._instance.StaticServerData = result2.Data;
            }
            LoadingOverlay.instance.DoneLoading("Loading data!");

            if (int.Parse(Startup.LIVE_VERSION) < int.Parse(Startup._instance.StaticServerData["LIVE_VERSION"]))
            {
                MainMenuController.instance.UpdateWindow.SetActive(true);
            }

        },
              error => {
                  Debug.Log("Got error getting titleData:");
                  Debug.Log(error.GenerateErrorReport());
              });

        //PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(),
        //      result2 => {
        //          if (result2.Data == null || !result2.Data.ContainsKey("TilesAmount"))
        //              Debug.Log("No TilesAmount");
        //          else
        //          {
        //              Startup._instance.StaticServerData = result2.Data;
        //          }
        //          LoadingOverlay.instance.DoneLoading("Loading data!");


        //          if( int.Parse(Startup.LIVE_VERSION) < int.Parse(Startup._instance.StaticServerData["LIVE_VERSION"]))
        //          {
        //              MainMenuController.instance.UpdateWindow.SetActive(true);
        //          }

        //      },
        //      error => {
        //          Debug.Log("Got error getting titleData:");
        //          Debug.Log(error.GenerateErrorReport());
        //      }
        //  );


       for(int i = 0; i< 200;i++)
        {
            LevelSettings.Add((500*i) +(i*30));

        }






        LoadingOverlay.instance.DoneLoading("LoginWithCustomID");
        GetComponent<Startup>().MyPlayfabID = result.AuthenticationContext.PlayFabId;

        if(result.InfoResultPayload.PlayerProfile.DisplayName.Length<=0 || result.InfoResultPayload.PlayerProfile.DisplayName.Contains("TEMP_NAME_"))
        {
            PlayerPrefs.DeleteAll();
            //PlayerPrefs.SetInt("HasDoneTutorial", 1);
            MainMenuController.instance.OpenSetNameWidnow(true);
        }


        if (result.NewlyCreated)
        {
            //SetUserData();
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


            if (result.InfoResultPayload.AccountInfo.AppleAccountInfo != null && result.InfoResultPayload.AccountInfo.AppleAccountInfo.AppleSubjectId.Length > 0)
            {
                MainMenuController.instance.SetAppleLinked(true);
                Startup._instance.Refresh(0.1f);

                return;

            }
            else
            {
                MainMenuController.instance.SetAppleLinked(false);
            }


            Refresh();
        }

#if UNITY_ANDROID
        Startup.instance.RegisterForPush();
#endif
    }
 

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        if (AddingUsersToGame > 0)
            AddingUsersToGame -= Time.deltaTime;

        if (Input.GetKeyUp(KeyCode.U))
        {
            CloseSearchGame("qwe","asd","2");
        }


        if (Input.GetKeyUp(KeyCode.H))
        {
            //string[] stringSeparators = new string[] { "[splitter]" };

            //string[] oldGameList = null;


            //oldGameList = PlayerPrefs.GetString("OldGames").Split(stringSeparators, System.StringSplitOptions.None);



            //byte[] bytesOneGame = new byte[0];

            //List<BoardData> oldG = new List<BoardData>();
            //for (int i = 0; i < oldGameList.Length; i++)
            //{

            //    if (oldGameList[i].Length > 2)
            //    {
            //        BoardData bd = new BoardData(CompressString.StringCompressor.DecompressString(oldGameList[i]));

            //        bytesOneGame = Encoding.ASCII.GetBytes(oldGameList[i]);
            //        bd.BoardTiles = new List<string>();
            //        oldG.Add(bd);
            //    }
            //}



            //byte[] bytes = Encoding.ASCII.GetBytes(PlayerPrefs.GetString("OldGames"));


            //Debug.LogWarning("Amount: " + oldGameList.Length);
            //Debug.LogWarning("SizeAll: " + bytes.Length);
            //Debug.LogWarning("Size1Game: " + bytesOneGame.Length);


            //string newSmalSize = "";
            //for (int i = 0; i < oldG.Count; i++)
            //{
            //    newSmalSize += (oldG[i].GetJson()) + "[splitter]";
            //    bytesOneGame = Encoding.ASCII.GetBytes(CompressString.StringCompressor.CompressString(oldG[i].GetJson()));
            //}
            //byte[] bytes2 = Encoding.ASCII.GetBytes(CompressString.StringCompressor.CompressString(newSmalSize));
            //Debug.LogWarning("SizeAll_compressed: " + bytes2.Length);
            //Debug.LogWarning("Size1Game_compressed: " + bytesOneGame.Length);



            //    PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest()
            //    { 
            //        FunctionName = "getfileTest",
            //        FunctionParameter = new Dictionary<string, object>() {
            //    { "PlayFabId", Startup.instance.MyPlayfabID }
            //}
            //    }, result => {

            //        Debug.Log(result.CustomData);

            //    }, error => Debug.LogError(error.GenerateErrorReport()));



        //    PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        //    {
        //        FunctionName = "ClearOldGames",
        //        FunctionParameter = new Dictionary<string, object>() {
        //    { "PlayFabId", Startup.instance.MyPlayfabID }
        //}
        //    }, result2 => {



        //        Debug.Log(result2.Logs);


        //    }, error =>
        //    {
        //        Debug.LogError(error.GenerateErrorReport());
        //    }
        //    );


        }


        if (Input.GetKeyUp(KeyCode.J))
        {

        }



    }




    public void UpdateDisplayName(string aName)
    {
        LoadingOverlay.instance.ShowLoading("UpdateUserTitleDisplayName");


        PlayfabCallbackHandler.instance.UpdateUserData("DisplayName", aName, () => {
            LoadingOverlay.instance.DoneLoading("UpdateUserTitleDisplayName");
            MainMenuController.instance.SetNameGO.SetActive(false);

            StartCoroutine(GetComponent<Startup>().DelayRefresh());
        }, () => {
            LoadingOverlay.instance.DoneLoading("UpdateUserTitleDisplayName");

            MainMenuController.instance.nameSettingTextError.text = "Invalid name";
        });



        //PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
        //{
        //    DisplayName = aName 
        //}, result =>
        //{
            
        //    LoadingOverlay.instance.DoneLoading("UpdateUserTitleDisplayName");
        //    Debug.Log("The player's display name is now: " + result.DisplayName);
        //    MainMenuController.instance.SetNameGO.SetActive(false);

        //    StartCoroutine(GetComponent<Startup>().DelayRefresh());

        //}, error =>{
        //                Debug.LogError(error.GenerateErrorReport());
        //                LoadingOverlay.instance.DoneLoading("UpdateUserTitleDisplayName");

        //    MainMenuController.instance.nameSettingTextError.text = "Invalid name";

        //});
    }

    public void SetPlayfabCreatedRoom(string playfabId, string roomName)
    {
        //LoadingOverlay.instance.ShowLoading("CreateSharedGroup");

        //PlayFabClientAPI.CreateSharedGroup(new CreateSharedGroupRequest()
        //{
        //    SharedGroupId = roomName
        //}, result3 => {
        //    LoadingOverlay.instance.DoneLoading("CreateSharedGroup");

        //    AddGameToSharedGroup(new List<string>() { playfabId }, roomName);


        //}, (error) => {
        //    Debug.Log(error.GenerateErrorReport());
        //});







        //string StaticServerDataV = "-1";

        //if (Startup._instance != null && Startup._instance.StaticServerData != null && Startup._instance.StaticServerData.ContainsKey("TilesAmount"))
        //    StaticServerDataV = Startup._instance.StaticServerData["TilesAmount"];

        //Board.instance.GenerateStartBoard(int.Parse(StaticServerDataV), PlayerPrefs.GetInt("BoardLayout", 0).ToString());
        //BoardData bd = new BoardData(playfabId, playfabId2, "1", Board.instance.BoardTiles, roomName, new List<string>(), Board.instance.GetTilesLeft(), "0", Board.instance.p1_tiles, Board.instance.p2_tiles, System.DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
        //bd.player1_displayName = Startup._instance.displayName;
        //bd.player2_displayName = player2DisplayName;
        //bd.player1_avatarURL = Startup.instance.avatarURL;
        //bd.player1_score = "0";
        //bd.player2_score = "0";
        //bd.EmptyTurns = "0";



        string secondPlayer = "";
        BoardData bd = new BoardData(playfabId, secondPlayer, "0", Board.instance.BoardTiles, roomName, new List<string>(), new List<string>(), "0", new List<string>(), new List<string>(), System.DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
        bd.player1_displayName = Startup._instance.displayName;
        bd.player1_avatarURL = Startup._instance.avatarURL;

        string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    {"SharedGroupId", roomName},
                    {"data", bd.GetJson()}});

        AWSBackend.instance.AWSClientAPI("phpBackend/UpdateSharedGroupData.php", jsonString, (result4) =>
        {
            //List<string> aUser = new List<string>();
            //aUser.Add(playfabId2);

            //string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
            //                        {"PlayerID_1", playfabId},
            //                        {"PlayerID_2", playfabId2},
            //                        {"SharedGroupId", roomName}});
            //AWSBackend.instance.AWSClientAPI("phpBackend/AddPlayerToSharedGroupAndToGameList_cloudscript.php", jsonString,
            //           (result) =>
            //           {
            //               Debug.Log(result);
            //           }, error =>
            //           {
            //               Debug.LogError(error);
            //           }
            //        );


            AddSharedGroupToGameList(roomName);






        },
        error =>
        {
            Debug.LogError("Got error setting user data Ancestor to Arthur");
        });







    }
    public void ChallengePlayer(string playfabId, string playfabId2,string player2DisplayName, string roomName)
    {

        //PlayFabClientAPI.CreateSharedGroup(new CreateSharedGroupRequest()
        //{
        //    SharedGroupId = roomName
        //}, result3 => {

            if(PlayerPrefs.HasKey("BoardLayout") == false)
                PlayerPrefs.SetInt("BoardLayout", 0);

            string StaticServerDataV = "-1";

            if (Startup._instance != null && Startup._instance.StaticServerData != null && Startup._instance.StaticServerData.ContainsKey("TilesAmount"))
                StaticServerDataV = Startup._instance.StaticServerData["TilesAmount"];

            Board.instance.GenerateStartBoard(int.Parse(StaticServerDataV), PlayerPrefs.GetInt("BoardLayout", 0).ToString());
            BoardData bd = new BoardData(playfabId, playfabId2, "1", Board.instance.BoardTiles, roomName, new List<string>(), Board.instance.GetTilesLeft(), "0", Board.instance.p1_tiles, Board.instance.p2_tiles, System.DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
            bd.player1_displayName = Startup._instance.displayName;
            bd.player2_displayName = player2DisplayName;
            bd.player1_avatarURL = Startup.instance.avatarURL;
            bd.player1_score = "0";
            bd.player2_score = "0";
            bd.EmptyTurns = "0";



        //PlayFabClientAPI.UpdateSharedGroupData(new UpdateSharedGroupDataRequest()
        //                    {
        //                        SharedGroupId = roomName,
        //                        Data = new Dictionary<string, string>() {
        //                                {roomName, bd.GetJson()}

        //                    }
        //                    },
        string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    {"SharedGroupId", roomName},
                    {"data", bd.GetJson()}});

        AWSBackend.instance.AWSClientAPI("phpBackend/UpdateSharedGroupData.php", jsonString, (result4) =>
                                {
                                    Debug.Log(result4);
                                    Debug.Log("Successfully updated user data with new player id's");

                                            List<string> aUser = new List<string>();
                                            aUser.Add(playfabId2);

                                    string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                                    {"PlayerID_1", playfabId},
                                    {"PlayerID_2", playfabId2},
                                    {"SharedGroupId", roomName}});

                                    //PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
                                    //{
                                    //    FunctionName = "AddPlayerToSharedGroupAndToGameList",
                                    //    FunctionParameter = new Dictionary<string, object>() {
                                    //    { "PlayFabIds", aUser },
                                    //    { "SharedGroupId", roomName },
                                    //    { "P1", playfabId },
                                    //    { "P2", playfabId2 }
                                    //}
                                    AWSBackend.instance.AWSClientAPI("phpBackend/AddPlayerToSharedGroupAndToGameList_cloudscript.php", jsonString,
                                               (result) =>
                                            {
                                                Debug.Log(result);
                                            }, error =>
                                            {
                                                Debug.LogError(error);
                                            }
                                            );


                                    SendPushToUser(bd.player2_PlayfabId, "", "" + Startup._instance.displayName +" has challenged you!");

                                },
                                error =>
                                {
                                    Debug.LogError("Got error setting user data Ancestor to Arthur");
                                });


        //}, (error) => {
        //    Debug.Log(error.GenerateErrorReport());
        //});

    }

    


    //public void AddGameToSharedGroup(List<string> playfabId, string roomName)
    //{

    //    string secondPlayer = "";
    //    if (playfabId.Count > 1)
    //        secondPlayer = playfabId[1];

    //    BoardData bd = new BoardData(playfabId[0], secondPlayer, "0", Board.instance.BoardTiles, roomName,new List<string>(), new List<string>(), "0", new List<string>(), new List<string>(), System.DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
    //    bd.player1_displayName = Startup._instance.displayName;
    //    bd.player1_avatarURL = Startup._instance.avatarURL;
        
    //    LoadingOverlay.instance.ShowLoading("UpdateSharedGroupData");

    //    PlayFabClientAPI.UpdateSharedGroupData(new UpdateSharedGroupDataRequest()
    //    {
    //        SharedGroupId = roomName,
    //        Data = new Dictionary<string, string>(){{roomName, bd.GetJson()}}
    //    },
    //    result2 =>
    //    {
    //        LoadingOverlay.instance.DoneLoading("UpdateSharedGroupData");
    //        Debug.Log("Successfully updated user data");
    //        AddSharedGroupToGameList(roomName);
    //        // Startup._instance.Refresh();
    //    },
    //    error =>
    //    {
    //        Debug.Log("Got error setting user data Ancestor to Arthur");
    //        Debug.Log(error.GenerateErrorReport());
    //    });
    //}

    public void AddPlayerToSharedGroup(string player1_playfabID, string player2_playfabID,string player1_displayName, string roomName,string boardLayout)
    {
        SetSharedDataForNewGame(player1_playfabID, player2_playfabID, player1_displayName, roomName, boardLayout);


        //List<string> aUser = new List<string>();
        //aUser.Add(player2_playfabID);

        //PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        //{
        //    FunctionName = "AddPlayerToSharedGroup",
        //    FunctionParameter = new Dictionary<string, object>() {
        //    { "PlayFabIds", aUser },
        //    { "SharedGroupId", roomName }
        //}
        //}, result => {
        //    Debug.Log(result.FunctionResult);
        //    SetSharedDataForNewGame(player1_playfabID, player2_playfabID, player1_displayName, roomName,boardLayout);
        //}, error =>
        //{
        //    SetSharedDataForNewGame(player1_playfabID, player2_playfabID, player1_displayName, roomName, boardLayout);
        //    Debug.LogError(error.GenerateErrorReport());
        //}
//);






    }
    public void AddSharedGroupToGameList(string aSharedGroupName)
    {
        GetComponent<Startup>().myData["MyGames"].Value += ","+ aSharedGroupName;

        LoadingOverlay.instance.ShowLoading("UpdateUserData");
        Dictionary<string, string> updateInfo = new Dictionary<string, string>();
        updateInfo.Add("MyGames", Startup._instance.myData["MyGames"].Value);

        PlayfabCallbackHandler.instance.UpdateUserDataGrouped(updateInfo, 
            () =>
            {
                LoadingOverlay.instance.DoneLoading("UpdateUserData");

                StartCoroutine(GetComponent<Startup>().DelayRefresh());

            },
            () => {
                LoadingOverlay.instance.DoneLoading("UpdateUserData");

                Debug.LogError("Bug updating data");
            });



            //PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            //{
            //    Data = new Dictionary<string, string>() {
            //    {"MyGames", GetComponent<Startup>().myData["MyGames"].Value},
            //}
            //},
            //result =>
            //{
            //    LoadingOverlay.instance.DoneLoading("UpdateUserData");

            //    StartCoroutine(GetComponent<Startup>().DelayRefresh());

            //},
            //error => {
            //    Debug.Log(error.GenerateErrorReport());
            //});
        }
    public void AddAiGameToOldGames(string aCompresserdAiBoard)
    {
        PlayfabCallbackHandler.instance.GetUserData(
        result =>
        {



            if (result.Data.ContainsKey("OldGames") == true)
                PlayerPrefs.SetString("OldGames", result.Data["OldGames"].Value);


            string st = (aCompresserdAiBoard);

            result.Data["OldGames"].Value += "[splitter]" + st;

            PlayerPrefs.SetString("OldGames", result.Data["OldGames"].Value);

            if (LoadingOverlay.instance != null)
                LoadingOverlay.instance.ShowLoading("UpdateUserData");
            Dictionary<string, string> updateInfo = new Dictionary<string, string>();
            updateInfo.Add("OldGames", result.Data["OldGames"].Value);

            PlayfabCallbackHandler.instance.UpdateUserDataGrouped(updateInfo, () => {

                if (LoadingOverlay.instance != null)
                    LoadingOverlay.instance.DoneLoading("UpdateUserData");

                if (LoadingOverlay.instance != null)
                    LoadingOverlay.instance.DoneLoading("Removing game!");


                    StartCoroutine(Startup._instance.DelayRefresh());
          
                PlayerPrefs.SetString("AIGame", "");

            }, () => {

                Debug.LogError("Bug  updating games!!");
            });



        }, (error) =>
        {


            Debug.LogError("BUG GETTING OLD GAMES!");
        }
            );





        //Need to load old games from online





        //            PlayfabCallbackHandler.instance.GetFilesRequest((result) => {

        //                if (LoadingOverlay.instance != null)
        //                    LoadingOverlay.instance.DoneLoading("Getting old games!");

        //                if (LoadingOverlay.instance != null)
        //                    LoadingOverlay.instance.ShowLoadingFullscreen("Removing AI!");

        //                result += "[splitter]" + aCompresserdAiBoard;
        //                PlayerPrefs.SetString("OldGames", result);

        //                PlayfabCallbackHandler.instance.InitiateFileUploads((result) => {

        //                    PlayerPrefs.SetString("AIGame", "");

        //                    Debug.Log("Removed game");
        //                    if (LoadingOverlay.instance != null)
        //                        LoadingOverlay.instance.DoneLoading("Removing AI!");
        //                },
        //                    error => {


        //                        if (LoadingOverlay.instance != null)
        //                            LoadingOverlay.instance.DoneLoading("Removing AI!");
   
        //                    });
        //}, (error) => {
        //    if (LoadingOverlay.instance != null)
        //        LoadingOverlay.instance.DoneLoading("Getting old games!");

        //    Debug.LogError("BUG GETTING OLD GAMES!");
        //});





    }
    public void RemoveSharedGroupToGameList(string aSharedGroupName, System.Action onComplete, string aDBjson )
    {


        //Need to load old games from online
        if (LoadingOverlay.instance != null)
            LoadingOverlay.instance.ShowLoadingFullscreen("Getting old games!");



        PlayfabCallbackHandler.instance.GetUserData(
        result =>
             {
                 if (LoadingOverlay.instance != null)
                     LoadingOverlay.instance.DoneLoading("Getting old games!");


                 if (result.Data.ContainsKey("OldGames") == true)
                    PlayerPrefs.SetString("OldGames", result.Data["OldGames"].Value);

                 Startup._instance.myData["MyGames"].Value = Startup._instance.myData["MyGames"].Value.Replace("," + aSharedGroupName, "");

                 string st = (aDBjson);


                 BoardData bd = new BoardData(CompressString.StringCompressor.DecompressString(aDBjson));
                 // Adding this if challenge is rejected dont add to old games
                 bool addToOld = true;
                 if ((bd.player1_score == "" || bd.player1_score == "0") && (bd.player2_score == "" || bd.player2_score == "0"))
                     addToOld = false;

                 if (addToOld)
                     result.Data["OldGames"].Value += "[splitter]" + st;



                 PlayerPrefs.SetString("OldGames", result.Data["OldGames"].Value);


                 if (LoadingOverlay.instance != null)
                     LoadingOverlay.instance.ShowLoading("UpdateUserData");
                 Dictionary<string, string> updateInfo = new Dictionary<string, string>();
                 updateInfo.Add("MyGames", Startup._instance.myData["MyGames"].Value);
                 updateInfo.Add("OldGames", result.Data["OldGames"].Value);

                 PlayfabCallbackHandler.instance.UpdateUserDataGrouped(updateInfo, ()=> {

                     if (LoadingOverlay.instance != null)
                         LoadingOverlay.instance.DoneLoading("UpdateUserData");
                     //Debug.Log("Removed non existing SharedData room in UserData");



                     //if (LoadingOverlay.instance != null)
                     //    LoadingOverlay.instance.ShowLoadingFullscreen("Removing game!");
                     //PlayfabCallbackHandler.instance.InitiateFileUploads((result) =>
                     //{
                         if (LoadingOverlay.instance != null)
                             LoadingOverlay.instance.DoneLoading("Removing game!");
                         // This is for if we find a abandoned game we want to remove a potentail next one
                         if (onComplete == null)
                         {
                             StartCoroutine(Startup._instance.DelayRefresh());
                         }
                         else
                         {
                             onComplete.Invoke();
                         }



                     //},
                     //error => {

                     //    GameFinishedScreen.instance.errorButtonRetry.SetActive(true);
                     //    GameFinishedScreen.instance.errorButtonRetry.transform.GetChild(1).GetComponent<Text>().text = "Error: InitiateFileUploads" + error;
                     //    if (LoadingOverlay.instance != null)
                     //        LoadingOverlay.instance.DoneLoading("Removing game!");









                     //});





                 }, () => {
                     GameFinishedScreen.instance.errorButton.SetActive(true);
                     GameFinishedScreen.instance.errorButton.transform.GetChild(1).GetComponent<Text>().text = "Error: UpdateUserData";
                     Debug.LogError("Bug  updating games!!");
                 });




       








             }, (error) =>
             {
                 if (LoadingOverlay.instance != null)
                     LoadingOverlay.instance.DoneLoading("Getting old games!");
                 GameFinishedScreen.instance.errorButton.SetActive(true);
                 GameFinishedScreen.instance.errorButton.transform.GetChild(1).GetComponent<Text>().text = "Error: GetFilesRequest";
                 Debug.LogError("BUG GETTING OLD GAMES!");
             }
            );














    }

    public void RetryUploadFile(System.Action onComplete)
    {
        GameFinishedScreen.instance.errorButtonRetry.SetActive(false);
        PlayfabCallbackHandler.instance.InitiateFileUploads((result) =>
        {
            if (LoadingOverlay.instance != null)
                LoadingOverlay.instance.DoneLoading("Removing game!");
            // This is for if we find a abandoned game we want to remove a potentail next one
            if (onComplete == null)
            {
                StartCoroutine(Startup._instance.DelayRefresh());
            }
            else
            {
                onComplete.Invoke();
            }



        },
        error => {

            GameFinishedScreen.instance.errorButtonRetry.SetActive(true);
            GameFinishedScreen.instance.errorButtonRetry.transform.GetChild(1).GetComponent<Text>().text = "Error: InitiateFileUploads" + error;
            if (LoadingOverlay.instance != null)
                LoadingOverlay.instance.DoneLoading("Removing game!");

        });
    }

        public void SetSharedDataForNewGame(string player1_playfabID, string player2_playfabID,string player1_displayName, string roomName, string boardLayout)
    {
        Board.instance.GenerateStartBoard(int.Parse(Startup._instance.StaticServerData["TilesAmount"]),boardLayout);
        BoardData bd = new BoardData(player1_playfabID, player2_playfabID, "0", Board.instance.BoardTiles,roomName, new List<string>(), Board.instance.GetTilesLeft(), "0", Board.instance.p1_tiles, Board.instance.p2_tiles, System.DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
        bd.player1_displayName = player1_displayName;
        bd.player2_displayName = Startup._instance.displayName;
        bd.player2_avatarURL = Startup._instance.avatarURL;
        bd.player1_score = "0";
        bd.player2_score = "0";
        bd.EmptyTurns = "0";
        LoadingOverlay.instance.ShowLoading("UpdateSharedGroupData");


        string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    {"SharedGroupId", roomName},
                    {"data", bd.GetJson()}});


        AWSBackend.instance.AWSClientAPI("phpBackend/UpdateSharedGroupData.php", jsonString, (result4) =>
        {
            LoadingOverlay.instance.DoneLoading("UpdateSharedGroupData");
            Debug.Log(result4);
            Debug.Log("Successfully updated user data with new player id's");

            List<string> aUser = new List<string>();
            aUser.Add(player2_playfabID);

            string jsonString2 = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                                    {"PlayerID_1", player1_playfabID},
                                    {"PlayerID_2", player2_playfabID},
                                    {"SharedGroupId", roomName}});

                AWSBackend.instance.AWSClientAPI("phpBackend/AddPlayerToSharedGroupAndToGameList_cloudscript.php", jsonString2,
                           (result) =>
                           {
                               SendPushToUser(bd.player2_PlayfabId, "", "" + Startup._instance.displayName + " has challenged you!");
                               Startup._instance.Refresh(0.1f);
                               Debug.Log(result);
                           }, error =>
                           {
                               Debug.LogError(error);
                           });



                    },
        error =>
        {
            Debug.LogError("Got error setting user data Ancestor to Arthur");
        });



        //PlayFabClientAPI.UpdateSharedGroupData(new UpdateSharedGroupDataRequest()
        //{
        //    SharedGroupId = roomName,
        //    Data = new Dictionary<string, string>() {
        //                {roomName, bd.GetJson()}

        //    }
        //},
        //result3 =>
        //{
        //    LoadingOverlay.instance.DoneLoading("UpdateSharedGroupData");
        //    Debug.Log("Successfully updated user data with new player id's");

        //    AddSharedGroupToGameList(roomName);

        //    SendPushToUser(bd.player1_PlayfabId, "", "You have started a game against "+ Startup._instance.displayName);

        //},
        //error =>
        //{
        //    Debug.Log("Got error setting user data Ancestor to Arthur");
        //    Debug.Log(error.GenerateErrorReport());
        //});


        
    }



    public void SetUserData()
    {


        //LoadingOverlay.instance.ShowLoadingFullscreen("UpdateUserData");
        //PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        //{ 
        //    Data = new Dictionary<string, string>() {
        //    {"Ranking", "0"},
        //    {"Picture", "0"},
        //    {"MyGames", ","},
        //    {"OldGames", "[splitter][splitter]"},
        //    {"StatsData", " "},
        //    {"Achivments", Startup._instance.myAchivmentController.GetDefault()},
        //    {"XP", "0"} },
        //    Permission = UserDataPermission.Public,

        //},
        //result =>
        //{
        //    LoadingOverlay.instance.DoneLoading("UpdateUserData");
        //    Debug.Log("Successfully updated user data");
        //    StartCoroutine(GetComponent<Startup>().DelayRefresh());
        //    MainMenuController.instance.OpenSetNameWidnow(true);

        //},
        //error => {
        //    Debug.Log("Got error setting user data Ancestor to Arthur");
        //    Debug.Log(error.GenerateErrorReport());
        //});
    }
    public void ChangeValueFor(string aEntry,string aValue)
    {
        //LoadingOverlay.instance.ShowLoading("UpdateUserData");


        PlayfabCallbackHandler.instance.UpdateUserData(aEntry, aValue, null, null);


       // PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
       // {
       //     Data = new Dictionary<string, string>() {
       //         {aEntry, aValue}

       //     },
       //     Permission = UserDataPermission.Public,
       // },
       //result =>
       //{
       //    //LoadingOverlay.instance.DoneLoading("UpdateUserData");

       //    Debug.Log("Successfully updated user data");
       //    //StartCoroutine(GetComponent<Startup>().DelayRefresh());

       //},
       //error => {
       //    Debug.Log("Got error setting user data Ancestor to Arthur");
       //    Debug.Log(error.GenerateErrorReport());
       //});


    }
    public void GetUserData()
    {
        if (LoadingOverlay.instance != null)
            LoadingOverlay.instance.ShowLoading("GetUserData");




        PlayfabCallbackHandler.instance.GetUserData(result => {

            if (SceneManager.GetActiveScene().name == "GameScene")
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
            Startup._instance.myAchivmentController.CheckWithLocal();

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

            if (result.Data.ContainsKey("DisplayName") == true)
                GetComponent<Startup>().displayName = result.Data["DisplayName"].Value;

            ProfileButton.instance.Init(GetComponent<Startup>().displayName, GetComponent<Startup>().myData["Ranking"].Value, xp);
            if (!updateHighscoreOnce)
            {
                SubmitHighscore(int.Parse(GetComponent<Startup>().myData["Ranking"].Value));
                updateHighscoreOnce = true;
            }

            bool shouldRest = false;
            if (result.Data.ContainsKey("ResetPlayerPrefs") == true)
            {
                if(result.Data["ResetPlayerPrefs"].Value =="1")
                {

                    Debug.Log("ResetPlayerPrefs");
                    shouldRest = true;
                    PlayerPrefs.DeleteKey("FacebookLink");
                    PlayerPrefs.DeleteKey("AppleidentityToken");
                    PlayerPrefs.DeleteKey("AppleUserIdKey");

                    ChangeValueFor("ResetPlayerPrefs", "0");


                    LoadingOverlay.instance.LoadingCall.Clear();
                    PlayfabHelperFunctions.instance.ReLogin();
                    SceneManager.LoadScene(0);
                    Startup._instance.Refresh(0.1f);
                }
    

            }



            //if( ChallengeAgainWindow.instance != null)
            //{
            //    ChallengeAgainWindow.instance.CheckPlayAgain();
            //}

            if (shouldRest == false)
                UpdateGameList();

            //if (result.Data == null || !result.Data.ContainsKey("Ancestor")) Debug.Log("No Ancestor");
            //else Debug.Log("Ancestor: " + result.Data["Ancestor"].Value);

        },
error => {
    Debug.Log("Got error retrieving user data:");
    Debug.Log(error.GenerateErrorReport());
});

        //PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        //{
        //    PlayFabId = GetComponent<Startup>().MyPlayfabID,
        //    Keys = null
        //}, result => {


        //    if( SceneManager.GetActiveScene().name == "GameScene")
        //    {
        //        return;
        //    }






        //    if (LoadingOverlay.instance != null)
        //        LoadingOverlay.instance.DoneLoading("GetUserData");

        //    Debug.Log("Done getting data");

        //    GetComponent<Startup>().myData = result.Data;

        //    string achivmentsData = "";
        //    if (result.Data.ContainsKey("Achivments") == true)
        //    {
        //        achivmentsData = result.Data["Achivments"].Value;
        //    }
        //    else
        //    {
        //        achivmentsData = Startup._instance.myAchivmentController.GetDefault();
        //    }


        //    Startup._instance.myAchivmentController.Init(achivmentsData);


        //    //GetComponent<Startup>()._infoLabel.text = GetComponent<Startup>().displayName + "\nPicture: " + GetComponent<Startup>().myData["Picture"].Value + "\n" + "Ranking: " + GetComponent<Startup>().myData["Ranking"].Value;
        //    //MainMenuController.instance._Name.text = GetComponent<Startup>().displayName;
        //    //MainMenuController.instance._Thropies.text = GetComponent<Startup>().myData["Ranking"].Value;
        //    string xp = "0";
        //    if (result.Data.ContainsKey("XP") == true)
        //    {
        //        xp = result.Data["XP"].Value;

        //    }
        //    else
        //        ChangeValueFor("XP", "0");

        //    ProfileButton.instance.Init(GetComponent<Startup>().displayName, GetComponent<Startup>().myData["Ranking"].Value, xp);
        //    if(!updateHighscoreOnce)
        //    {
        //        SubmitHighscore(int.Parse(GetComponent<Startup>().myData["Ranking"].Value));
        //        updateHighscoreOnce = true;
        //    }



        //    UpdateGameList();

        //    //if (result.Data == null || !result.Data.ContainsKey("Ancestor")) Debug.Log("No Ancestor");
        //    //else Debug.Log("Ancestor: " + result.Data["Ancestor"].Value);
        //}, (error) => {
        //    Debug.Log("Got error retrieving user data:");
        //    Debug.Log(error.GenerateErrorReport());
        //});

    }
    bool updateHighscoreOnce = false;
    //public List<string> EmptyGamesToRemove = new List<string>();
    //public void RemoveLegacyAndEmptyGames()
    //{
    //    if(EmptyGamesToRemove.Count>0)
    //    {
    //        for(int i = 0; i < EmptyGamesToRemove.Count;i++)
    //        {
    //            Startup._instance.myData["MyGames"].Value = Startup._instance.myData["MyGames"].Value.Replace("," + EmptyGamesToRemove[i], "");
    //        }
    //        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
    //        {
    //            Data = new Dictionary<string, string>() {
    //            {"MyGames",Startup._instance.myData["MyGames"].Value}
    //        }
    //        },
    //        result =>
    //        {
    //            Debug.Log("Removed non existing SharedData room in UserData");
    //        },
    //        error => {
    //            Debug.Log(error.GenerateErrorReport());
    //        });

    //    }
    //    EmptyGamesToRemove.Clear();
    //}
  
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


        GetSharedDataGrouped(Startup.instance.MyPlayfabID);


       // bool shouldAddOldGamesNow = true;
       // for (int i = 0;i < gameList.Length;i++)
       // {
       //     if(gameList[i].Length>1)
       //     {
       //         LoadingOverlay.instance.ShowLoading("GetSharedGroupData0");
       //         shouldAddOldGamesNow = false;

       //         PlayfabCallbackHandler.instance.GetSharedGroupData(gameList[i], result => {

       //             if(result.Data == null || result.Data.Count<=0)
       //             {
       //                 Debug.Log(result.Request.ToJson());
       //                 SendRequest vd = JsonUtility.FromJson<SendRequest>(result.Request.ToJson());

                        
       //                 EmptyGamesToRemove.Add(vd.SharedGroupId);

       //             }


       //             if (LoadingOverlay.instance != null)
       //                 LoadingOverlay.instance.DoneLoading("GetSharedGroupData0");
       //             foreach (KeyValuePair<string, SharedGroupDataRecord> entry in result.Data)
       //             {
       //                 if (entry.Key == "Chat")
       //                     continue;

       //                 BoardData bd = new BoardData(entry.Value.Value);
       //                 //GetComponent<Startup>()._roomListLabel.text += "" + bd.player1_PlayfabId + " vs " + bd.player2_PlayfabId + " turn: " + bd.playerTurn + "\n";


       //                 if (Startup._instance.SearchingForGameObject != null)
       //                 {
       //                     if (Startup._instance.SearchingForGameObject.GetComponent<SearchGameInfo>().NameID == bd.RoomName)
       //                     {
       //                         if (bd.player2_PlayfabId != "")
       //                             Destroy(Startup._instance.SearchingForGameObject);
       //                     }
       //                 }


       //                 if (bd.player1_abandon == "1" || bd.player2_abandon == "1")
       //                 {

       //                 }
       //                 else if (bd.player2_PlayfabId != "")
       //                 {
       //                     GameObject obj = (GameObject)GameObject.Instantiate(_GameListItem, MainMenuController.instance._GameListParent_updating);
       //                     Vector3 rc = obj.GetComponent<RectTransform>().localPosition;
       //                     obj.GetComponent<RectTransform>().localPosition = new Vector3(rc.x, rc.y, 0);
       //                     obj.GetComponent<GameListItem>().Init(bd);

       //                 }
       //                 else
       //                 {
       //                     if (Startup._instance.SearchingForGameObject == null)
       //                     {
       //                         Startup._instance.SearchingForGameObject = (GameObject)GameObject.Instantiate(PlayfabHelperFunctions.instance.SearchingForGamePrefab, MainMenuController.instance._GameListParent_updating);
       //                         Startup._instance.SearchingForGameObject.transform.SetAsFirstSibling();
       //                         Startup._instance.SearchingForGameObject.SetActive(true);
       //                         Vector3 rc = Startup._instance.SearchingForGameObject.GetComponent<RectTransform>().localPosition;
       //                         Startup._instance.SearchingForGameObject.GetComponent<RectTransform>().localPosition = new Vector3(rc.x, rc.y, 0);
       //                         Startup._instance.SearchingForGameObject.GetComponent<SearchGameInfo>().NameID = bd.RoomName;
       //                     }
       //                     else
       //                     {
       //                         Startup._instance.SearchingForGameObject.transform.SetAsFirstSibling();
       //                         Vector3 rc = Startup._instance.SearchingForGameObject.GetComponent<RectTransform>().localPosition;
       //                         Startup._instance.SearchingForGameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

       //                     }

       //                 }


       //                 Startup._instance.openGamesList.Add(bd);
       //             }


       //             Startup._instance.FinishedGettingGameListCheckForOpenGames();

       //         },
       //           error => {
       //                   Debug.Log(error.GenerateErrorReport());
       //           });


       //     }
       // }

       //if(shouldAddOldGamesNow)
       //     Startup._instance.FinishedGettingGameListCheckForOpenGames();

    }
    public void RemoveRoomFromList(string aRoomName,string aRoomJson,System.Action onComplete=null)
    {
        if (LoadingOverlay.instance != null)
            LoadingOverlay.instance.ShowLoading("RemoveSharedGroupMembers");



        //PlayfabCallbackHandler.instance.RemoveSharedGroupMembers(aRoomName, new List<string>() { Startup._instance.MyPlayfabID }, () => {
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
            //Refresh();
        //}, () => {
        //    if (LoadingOverlay.instance != null)
        //        LoadingOverlay.instance.DoneLoading("RemoveSharedGroupMembers");


        //    string st = "";

        //    st = CompressString.StringCompressor.CompressString(aRoomJson);



        //    for (int i = 0; i < Startup._instance.openGamesList.Count; i++)
        //    {
        //        if (Startup._instance.openGamesList[i].player1_PlayfabId == Startup._instance.MyPlayfabID && Startup._instance.openGamesList[i].player2_PlayfabId == "")
        //        {
        //            Startup._instance.openGamesList.RemoveAt(i);
        //            break;
        //        }
        //    }

        //    RemoveSharedGroupToGameList(aRoomName, onComplete, st);

        //    Debug.Log("Removed old inactive SharedData room");
        //    //        Refresh();
        //   // Debug.Log(error.GenerateErrorReport());
        //});



    //    PlayFabClientAPI.RemoveSharedGroupMembers(new RemoveSharedGroupMembersRequest()
    //    { 
    //        SharedGroupId = aRoomName,
    //        PlayFabIds = new List<string>(){Startup._instance.MyPlayfabID}
    //    }, result =>
    //    {
    //        if (LoadingOverlay.instance != null)
    //            LoadingOverlay.instance.DoneLoading("RemoveSharedGroupMembers");


    //        string st = "";
       
    //                st = CompressString.StringCompressor.CompressString(aRoomJson);
     


    //        for (int i = 0; i < Startup._instance.openGamesList.Count; i++)
    //        {
    //            if (Startup._instance.openGamesList[i].player1_PlayfabId == Startup._instance.MyPlayfabID && Startup._instance.openGamesList[i].player2_PlayfabId == "")
    //            {
    //                Startup._instance.openGamesList.RemoveAt(i);
    //                break;
    //            }
    //        }

    //        RemoveSharedGroupToGameList(aRoomName, onComplete, st);

    //        Debug.Log("Removed old inactive SharedData room");
    //        //Refresh();
    //    }, (error) =>
    //    {
    //        if (LoadingOverlay.instance != null)
    //            LoadingOverlay.instance.DoneLoading("RemoveSharedGroupMembers");


    //        string st = "";

    //        st = CompressString.StringCompressor.CompressString(aRoomJson);



    //        for (int i = 0; i < Startup._instance.openGamesList.Count; i++)
    //        {
    //            if (Startup._instance.openGamesList[i].player1_PlayfabId == Startup._instance.MyPlayfabID && Startup._instance.openGamesList[i].player2_PlayfabId == "")
    //            {
    //                Startup._instance.openGamesList.RemoveAt(i);
    //                break;
    //            }
    //        }

    //        RemoveSharedGroupToGameList(aRoomName, onComplete, st);

    //        Debug.Log("Removed old inactive SharedData room");
    ////        Refresh();
    //        Debug.Log(error.GenerateErrorReport());
    //    });
    }
    public void UpdateTargetGame(string aGame)
    {
        if(LoadingOverlay.instance != null)
        LoadingOverlay.instance.ShowLoading("GetSharedGroupData1");



        //PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        //{
        //    FunctionName = "GetTargetGame",
        //    FunctionParameter = new Dictionary<string, object>() {
        //    { "PlayFabId", aGame }
        //}
        //}, result => {

        //    if (SceneManager.GetActiveScene().name != "GameScene")
        //    {
        //        return;
        //    }

        //    if (LoadingOverlay.instance != null)
        //        LoadingOverlay.instance.DoneLoading("GetSharedGroupData1");


        //    JsonObject jsonResult = (JsonObject)result.FunctionResult;



        //    //IEnumerable test = (IEnumerable)result.FunctionResult;

        //    //List<BoardData> gameList = new List<BoardData>();

        //    //foreach (IEnumerable item in test)
        //    //{

        //    //    char[] t = item.ToString().ToCharArray();

        //    //    List<int> b = new List<int>();

        //    //    for (int i = 0; i < t.Length; i++)
        //    //    {

        //    //        b.Add((t[i]));
        //    //    }


        //    //    string con = Decompress(b);


        //    //    gameList.Add(new BoardData(con));

        //    //}







        //        //foreach (KeyValuePair<string, object> entry in jsonResult)
        //        //{
        //        //if (entry.Key == "Chat")
        //        //{
        //        //    string chatViewed = PlayerPrefs.GetString(aGame + "_chat");

        //        //    if (chatViewed != entry.Value.ToString())
        //        //    {
        //        //        string[] liveMessegaes = entry.Value.ToString().Split('≤');
        //        //        string[] viewedMessegaes = chatViewed.Split('≤');

        //        //        if (GameManager.instance == null)
        //        //            return;

        //        //        GameManager.instance.ChatNotificationIcon.SetActive(true);
        //        //        float dif = Mathf.Abs(liveMessegaes.Length - viewedMessegaes.Length) / 2;
        //        //        if (dif == 0)
        //        //            dif = 1;
        //        //        GameManager.instance.ChatNotificationIcon.transform.GetChild(0).GetComponent<Text>().text = dif.ToString();
        //        //    }
        //        //    else
        //        //    {
        //        //        GameManager.instance.ChatNotificationIcon.SetActive(false);
        //        //    }


        //        //    continue;
        //        //}

        //        BoardData bd = new BoardData(messageValue);
        //        GetComponent<Startup>().GameToLoad = bd;
        //    //}
        //    GameManager.instance.RefreshBackendCallback();






        //}, (error) =>
        //{
        //    GameManager.instance.updateInProgress = false;
        //    Debug.Log(error.GenerateErrorReport());
        //});




        //return;





        PlayfabCallbackHandler.instance.GetSharedGroupData(aGame, result =>
                {

                    if (SceneManager.GetActiveScene().name != "GameScene")
                    {
                        return;
                    }

                    if (LoadingOverlay.instance != null)
                        LoadingOverlay.instance.DoneLoading("GetSharedGroupData1");

                    foreach (KeyValuePair<string, SharedGroupDataRecord> entry in result.Data)
                    {
                        if (entry.Key == "chat")
                        {
                            string chatViewed = PlayerPrefs.GetString(aGame + "_chat");

                            if(chatViewed != entry.Value.Value && entry.Value.Value.Length>0)
                            {
                                string[] liveMessegaes = entry.Value.Value.Split('≤');
                                string[] viewedMessegaes = chatViewed.Split('≤');

                                if (GameManager.instance == null)
                                    return;

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
                        if (entry.Key == "data")
                        {
                            BoardData bd = new BoardData(HttpUtility.UrlDecode(entry.Value.Value));
                            GetComponent<Startup>().GameToLoad = bd;
                        }
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

        PlayfabCallbackHandler.instance.GetSharedGroupData(aGame, result =>
        {
            if (LoadingOverlay.instance != null)
                LoadingOverlay.instance.DoneLoading("GetSharedGroupData2");

            foreach (KeyValuePair<string, SharedGroupDataRecord> entry in result.Data)
            {
                if (entry.Key == "chat")
                {
                    string chatViewed = PlayerPrefs.GetString(aGame + "_chat");

                    if (chatViewed != entry.Value.Value && entry.Value.Value.Length>0)
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
            }

        }, (error) =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }
    public void SendNextTurn(BoardData aBoarddata, System.Action callback=null)
    {



        string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    {"PlayerID", Startup.instance.MyPlayfabID },
                    {"SharedGroupId", aBoarddata.RoomName},
                    {"data", aBoarddata.GetJson()}});

        AWSBackend.instance.AWSClientAPI("phpBackend/UpdateSharedGroupData_cloudscript.php", jsonString, (result) => {
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
 

             //TODO. Enable this again
            SendPushToUser(userToSend,"", "It's your turn against "+displayName +"!", aBoarddata.RoomName);



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





            if (int.Parse(aBoarddata.EmptyTurns) >= 4)
            {
                GameManager.instance.updateInProgress = false;
                GameManager.instance.EndGameAfterPasses(aBoarddata);
            }
            else
            {
                //GameManager.instance.WaitingOverlay.SetActive(true);
                //GameManager.instance.WaitingOverlay.GetComponent<CanvasGroup>().alpha = 0;
                //GameManager.instance.WaitingOverlay.GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.InOutQuart);




            }



        },
        error =>
        {
            Debug.Log("Got error making turn");
            Debug.LogError(error);

   


            GameManager.instance.updateInProgress = false;

        });


    }
    public void SendPushToUser(string aId,string title, string message,string room ="")
    {
        if (title == "")
            title = "Outnumber";
        if (message == "")
            message = "Outnumber";




        string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    {"TargetPlayerId", aId},
                    { "Title", title},
                    { "Message", message },
                    { "room", "{\"room\":\""+room+"\"}" }});

        AWSBackend.instance.AWSClientAPI("phpBackend/SendPush.php", jsonString, (result) => {
            Debug.Log("Done!" + result);
        }, (error) => {
            Debug.Log("Error!" + error);
        });



        //PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        //{
        //    FunctionName = "ChallengePlayer",
        //    FunctionParameter = new Dictionary<string, object>() {
        //    { "TargetId", aId },
        //    { "Title", title },
        //    { "Message", message },
        //    { "room", "{\"room\":\""+room+"\"}" }

        //}
        //}, result => {
        //    Debug.Log(result.FunctionResult);

        //}, error => Debug.LogError(error.GenerateErrorReport()));





    }
    public float AddingUsersToGame = 0;
    public void CheckIfGameIsInList(string player1, string player2, string roomID)
    {
        if(AddingUsersToGame <=0)
        {
            string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    {"SharedGroupId", roomID},
                         {"PlayerID_1", player1},
                              {"PlayerID_2", player2}
            });

            AWSBackend.instance.AWSClientAPI("phpBackend/CheckIfGameInListElseAdd.php", jsonString, (result2) => {
                //Debug.Log(result2);

            }, error => Debug.LogError(error));



            //PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            //{
            //    FunctionName = "CheckIfGameInListElseAdd",
            //    FunctionParameter = new Dictionary<string, object>() {
            //{ "player1", player1 },
            //{ "player2", player2 },
            //{ "roomID", roomID }

            //}
            //}, result => {
            //    Debug.Log(result.FunctionResult);

            //}, error => Debug.LogError(error.GenerateErrorReport()));


        }

    }
    //public void GetAllAvatarGrouped(string aId)
    //{
    //    PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
    //    {
    //        FunctionName = "GetAllAvatarGrouped",
    //        FunctionParameter = new Dictionary<string, object>() {
    //        { "PlayFabId", aId }
    //    }
    //    }, result => {

    //        IEnumerable test = (IEnumerable)result.FunctionResult;

    //        List<BoardData> gameList = new List<BoardData>();
    //        foreach (IEnumerable item in test)
    //        {
    //            gameList.Add(new BoardData(item.ToString()));

    //        }


    //        bool shouldAddOldGamesNow = true;
    //        for (int i = 0; i < gameList.Count; i++)
    //        {
    //            if (gameList[i] != null)
    //            {

    //                shouldAddOldGamesNow = false;


    //                BoardData bd = gameList[i];


    //                if (Startup._instance.SearchingForGameObject != null)
    //                {
    //                    if (Startup._instance.SearchingForGameObject.GetComponent<SearchGameInfo>().NameID == bd.RoomName)
    //                    {
    //                        if (bd.player2_PlayfabId != "")
    //                            Destroy(Startup._instance.SearchingForGameObject);
    //                    }
    //                }


    //                if (bd.player1_abandon == "1" || bd.player2_abandon == "1")
    //                {

    //                }
    //                else if (bd.player2_PlayfabId != "")
    //                {
    //                    GameObject obj = (GameObject)GameObject.Instantiate(_GameListItem, MainMenuController.instance._GameListParent_updating);
    //                    Vector3 rc = obj.GetComponent<RectTransform>().localPosition;
    //                    obj.GetComponent<RectTransform>().localPosition = new Vector3(rc.x, rc.y, 0);
    //                    obj.GetComponent<GameListItem>().Init(bd);

    //                }
    //                else
    //                {
    //                    if (Startup._instance.SearchingForGameObject == null)
    //                    {
    //                        Startup._instance.SearchingForGameObject = (GameObject)GameObject.Instantiate(PlayfabHelperFunctions.instance.SearchingForGamePrefab, MainMenuController.instance._GameListParent_updating);
    //                        Startup._instance.SearchingForGameObject.transform.SetAsFirstSibling();
    //                        Startup._instance.SearchingForGameObject.SetActive(true);
    //                        Vector3 rc = Startup._instance.SearchingForGameObject.GetComponent<RectTransform>().localPosition;
    //                        Startup._instance.SearchingForGameObject.GetComponent<RectTransform>().localPosition = new Vector3(rc.x, rc.y, 0);
    //                        Startup._instance.SearchingForGameObject.GetComponent<SearchGameInfo>().NameID = bd.RoomName;
    //                    }
    //                    else
    //                    {
    //                        Startup._instance.SearchingForGameObject.transform.SetAsFirstSibling();
    //                        Vector3 rc = Startup._instance.SearchingForGameObject.GetComponent<RectTransform>().localPosition;
    //                        Startup._instance.SearchingForGameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

    //                    }

    //                }


    //                Startup._instance.openGamesList.Add(bd);



    //                //  Startup._instance.FinishedGettingGameListCheckForOpenGames();




    //            }
    //        }

    //        // if (shouldAddOldGamesNow)
    //        Startup._instance.FinishedGettingGameListCheckForOpenGames();












    //    }, error => Debug.LogError(error.GenerateErrorReport()));
    //}


    //public void test( )
    //{
    //    PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
    //    {
    //        FunctionName = "GetAllAvatarGrouped",
    //        FunctionParameter = new Dictionary<string, object>() {
    //        { "PlayFabId", Startup.instance.MyPlayfabID }
    //    }
    //    }, result => {

    //        Debug.Log(result.CustomData);

    //    }, error => Debug.LogError(error.GenerateErrorReport()));
    //}
    public static void CopyTo(Stream src, Stream dest)
    {
        byte[] bytes = new byte[4096];

        int cnt;

        while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
        {
            dest.Write(bytes, 0, cnt);
        }
    }

    public static byte[] Zip(string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);

        using (var msi = new MemoryStream(bytes))
        using (var mso = new MemoryStream())
        {
            using (var gs = new GZipStream(mso, CompressionMode.Compress))
            {
                //msi.CopyTo(gs);
                CopyTo(msi, gs);
            }

            return mso.ToArray();
        }
    }

    public static string Unzip(byte[] bytes)
    {
        using (var msi = new MemoryStream(bytes))
        using (var mso = new MemoryStream())
        {
            using (var gs = new GZipStream(msi, CompressionMode.Decompress))
            {
                //gs.CopyTo(mso);
                CopyTo(gs, mso);
            }

            return Encoding.UTF8.GetString(mso.ToArray());
        }
    }
    public static string Decompress(List<int> compressed)
    {
        // build the dictionary
        Dictionary<int, string> dictionary = new Dictionary<int, string>();
        for (int i = 0; i < 256; i++)
            dictionary.Add(i, ((char)i).ToString());

        string w = dictionary[compressed[0]];
        compressed.RemoveAt(0);
        StringBuilder decompressed = new StringBuilder(w);

        foreach (int k in compressed)
        {
            string entry = null;
            if (dictionary.ContainsKey(k))
                entry = dictionary[k];
            else if (k == dictionary.Count)
                entry = w + w[0];

            decompressed.Append(entry);

            // new sequence; add it to the dictionary
            dictionary.Add(dictionary.Count, w + entry[0]);

            w = entry;
        }

        return decompressed.ToString();
    }
    private static string DecodeUrlString(string url) {
    string newUrl;
    while ((newUrl = Uri.UnescapeDataString(url)) != url)
        url = newUrl;
    return newUrl;
}
    public void GetSharedDataGrouped(string aId)
    {

        if (Startup._instance.openGamesList.Count > 0)
            return;

        PlayfabCallbackHandler.instance.GetAllAvatarGrouped(result => {
            if (Startup._instance.openGamesList.Count > 0)
                return;

            //   Startup.instance.StoredAvatarURLS = (PlayFab.Json.JsonObject)result.FunctionResult;
            string v = (string)result.FunctionResult;
            if (v != "[]" && v.Length>2)
            Startup.instance.StoredAvatarURLS = JsonConvert.DeserializeObject<Dictionary<string, string>>((string)result.FunctionResult);
            //        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            //{
            //    FunctionName = "GetSharedDataGrouped2",
            //    FunctionParameter = new Dictionary<string, object>() {
            //    { "PlayFabId", aId }
            //}
            //},
            PlayfabCallbackHandler.instance.GetSharedDataGrouped2(aId, result2 => {
            if (Startup._instance.openGamesList.Count > 0)
                return;
            if (result2 == null || result2.FunctionResult == null)
                return;

   



                //IEnumerable test = (IEnumerable)result2.FunctionResult;

            List<BoardData> gameList = new List<BoardData>();

                //foreach (IEnumerable item in test)
                string v2 = (string)result2.FunctionResult;
                if (v2 != "[]" && v2.Length > 2)
                {
                    Dictionary<string, string> gamelistreturn = JsonConvert.DeserializeObject<Dictionary<string, string>>((string)result2.FunctionResult);

                    foreach (var item in gamelistreturn)
                    {
                        string con2 = HttpUtility.UrlDecode(item.Value);
                        //   char[] t = con2.ToCharArray();

                        //List<int> b = new List<int>();

                        //for (int i = 0; i < t.Length; i++)
                        //{

                        //    b.Add((t[i]));
                        //}


                        //string con = Decompress(b);





                        gameList.Add(new BoardData(con2));
                    }
                }
              



            bool shouldAddOldGamesNow = true;
            for (int i = 0; i < gameList.Count; i++)
            {
                if (gameList[i] != null)
                {

                    shouldAddOldGamesNow = false;


                             BoardData bd = gameList[i];


                            if (Startup._instance.SearchingForGameObject != null)
                            {
                                if (Startup._instance.SearchingForGameObject.GetComponent<SearchGameInfo>().NameID == bd.RoomName)
                                {
                                    if (bd.player2_PlayfabId != "")
                                        Destroy(Startup._instance.SearchingForGameObject);
                                }
                            }


                            if (bd.player1_abandon == "1" || bd.player2_abandon == "1")
                            {

                            }
                            else if (bd.player2_PlayfabId != "")
                            {
                                GameObject obj = (GameObject)GameObject.Instantiate(_GameListItem, MainMenuController.instance._GameListParent_updating);
                                Vector3 rc = obj.GetComponent<RectTransform>().localPosition;
                                obj.GetComponent<RectTransform>().localPosition = new Vector3(rc.x, rc.y, 0);
                                obj.GetComponent<GameListItem>().Init(bd);

                            }
                            else
                            {
                                if (Startup._instance.SearchingForGameObject == null)
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


                            Startup._instance.openGamesList.Add(bd);
                        


                      //  Startup._instance.FinishedGettingGameListCheckForOpenGames();

                


                }
            }

                GetOpenOnlineGamesForLabel();
                GetOpenOnlineGames_Self();

            AddingUsersToGame = 60;

           // if (shouldAddOldGamesNow)
            Startup._instance.FinishedGettingGameListCheckForOpenGames();












        }, error => Debug.LogError(error.GenerateErrorReport()));








        }, error => Debug.LogError(error.GenerateErrorReport()));
    }
    public void AddGameToPlayerListCloudScript(string aId, string aRoomName)
    {
        //PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        //{
        //    FunctionName = "AddGameToPlayerList",
        //    FunctionParameter = new Dictionary<string, object>() {
        //    { "PlayFabId", aId },
        //    { "RoomName", aRoomName }

        //}
        string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    {"PlayerID_1", aId},
                    {"SharedGroupId", aRoomName}});

        AWSBackend.instance.AWSClientAPI("phpBackend/AddGameToPlayerList.php", jsonString, (result) => {
                Debug.Log(result);

        }, error => Debug.LogError(error));
    }


    
    //public void CreateAndAddToSharedGroup(string[] aPlayFabIds, string aSharedGroupId)
    //{
    //    PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
    //    {
    //        FunctionName = "CreateAndAddSharedGroup",
    //        FunctionParameter = new Dictionary<string, object>() {
    //        { "PlayFabIds", aPlayFabIds },
    //        { "SharedGroupId", aSharedGroupId }
    //    }
    //    }, result => {
    //        Debug.Log(result.FunctionResult);

    //    }, error =>
    //    {
    //        Debug.LogError(error.GenerateErrorReport());
    //    }
    //    );

    //}
    //public void AddPlayerToSharedGroup(string[] aPlayFabIds, string aSharedGroupId)
    //{
    //    PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
    //    {
    //        FunctionName = "AddPlayerToSharedGroup",
    //        FunctionParameter = new Dictionary<string, object>() {
    //        { "PlayFabIds", aPlayFabIds },
    //        { "SharedGroupId", aSharedGroupId }
    //    }
    //    }, result => {
    //        Debug.Log(result.FunctionResult);

    //    }, error =>
    //    {
    //        Debug.LogError(error.GenerateErrorReport());
    //    }
    //    );

    //}



    public void Refresh()
    {
        //if (LoadingOverlay.instance.GetIsLoadingGames())
        //    return;
        if (PlayfabCallbackHandler.instance.IsLoadingGames())
            return;
        //if (LoadingOverlay.instance.LoadingCall.Count > 0)
        //    return;
        
        Startup._instance.DontAutoRefresh = false;

        //if (LoadingOverlay.instance != null)
        //    LoadingOverlay.instance.ShowLoading("GetPlayerProfile");





        //PlayfabCallbackHandler.instance.GetPlayerProfile(result2 => {
        //    if (LoadingOverlay.instance != null)
        //        LoadingOverlay.instance.DoneLoading("GetPlayerProfile");

        //    GetComponent<Startup>().displayName = result2.PlayerProfile.DisplayName;


            GetUserData();

      //  },
      //error => {
      //    Debug.Log(error.GenerateErrorReport());
      //});




        //PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
        //{
        //    PlayFabId = GetComponent<Startup>().MyPlayfabID,
        //    ProfileConstraints = new PlayerProfileViewConstraints()
        //    {
        //        ShowDisplayName = true
        //    }
        //},
        //result =>
        //{
        //    if(LoadingOverlay.instance != null)
        //    LoadingOverlay.instance.DoneLoading("GetPlayerProfile");

        //    GetComponent<Startup>().displayName = result.PlayerProfile.DisplayName;


        //    GetUserData();
        //},
        //error => Debug.LogError(error.GenerateErrorReport()));





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



            
  

            if (Startup._instance.openGamesList[i].player1_abandon == "1" || Startup._instance.openGamesList[i].player2_abandon == "1" /*|| int.Parse(Startup._instance.openGamesList[i].EmptyTurns) >= 4 */|| timeToDeadline<0)
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
        if (LoadingOverlay.instance != null)
            LoadingOverlay.instance.ShowLoadingFullscreen("RemoveGame!");
        SetAbandomeForPlayerInGame(aRoomName, callback);
  

    }
    public void SetAbandomeForPlayerInGame(string aRoomName, System.Action callback = null)
    {
        if(LoadingOverlay.instance != null)
        LoadingOverlay.instance.ShowLoading("GetSharedGroupData3");
        PlayfabCallbackHandler.instance.GetSharedGroupData(aRoomName, result =>
        {
            if (LoadingOverlay.instance != null)
                LoadingOverlay.instance.DoneLoading("GetSharedGroupData3");
            foreach (KeyValuePair<string, SharedGroupDataRecord> entry in result.Data)
            {
                if (entry.Key == "chat")
                    continue;
                if (entry.Key == "data")
                {
                    BoardData bd = new BoardData(HttpUtility.UrlDecode(entry.Value.Value));

                    bd.SetPlayerAbandome();
                    if (LoadingOverlay.instance != null)
                        LoadingOverlay.instance.ShowLoading("UpdateSharedGroupData");


                    string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    {"SharedGroupId", aRoomName},
                    {"data", bd.GetJson()}});

                    AWSBackend.instance.AWSClientAPI("phpBackend/UpdateSharedGroupData.php", jsonString, 
                       result3 =>
                       {
                           if (LoadingOverlay.instance != null)
                               LoadingOverlay.instance.DoneLoading("UpdateSharedGroupData");
                           Debug.Log("Successfully updated SetAbadome in room name:" + aRoomName);
                           RemoveRoomFromList(aRoomName, bd.GetJson(), callback);
                           LoadingOverlay.instance.DoneLoading("RemoveGame!");
                       },
                        error =>
                        {
                            Debug.LogError("Got error SetAbadome");
                            LoadingOverlay.instance.DoneLoading("RemoveGame!");
                        });



                               // PlayFabClientAPI.UpdateSharedGroupData(new UpdateSharedGroupDataRequest()
                               // {
                               //     SharedGroupId = aRoomName,
                               //     Data = new Dictionary<string, string>() {
                               //         {aRoomName, bd.GetJson()}

                               // }
                               // },
                               //result3 =>
                               //{
                               //    if (LoadingOverlay.instance != null)
                               //        LoadingOverlay.instance.DoneLoading("UpdateSharedGroupData");
                               //    Debug.Log("Successfully updated SetAbadome in room name:" + aRoomName);
                               //    RemoveRoomFromList(aRoomName, bd.GetJson(), callback);
                               //    LoadingOverlay.instance.DoneLoading("RemoveGame!");
                               //},
                               //error =>
                               //{
                               //    Debug.Log("Got error SetAbadome");
                               //    Debug.Log(error.GenerateErrorReport());
                               //    LoadingOverlay.instance.DoneLoading("RemoveGame!");
                               //});





                }
            }
        }, (error) =>
        {
            Debug.Log(error.GenerateErrorReport());
            LoadingOverlay.instance.DoneLoading("RemoveGame!");
        });
  
    }
    public void SubmitHighscore(int playerScore)
    {
        PlayfabCallbackHandler.instance.UpdatePlayerStatistics(playerScore, "Highscore",null,null);


        //PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        //{
        //    Statistics = new List<StatisticUpdate> {
        //    new StatisticUpdate {
        //        StatisticName = "Highscore",
        //        Value = playerScore
        //    }
        //}
        //}, result => OnStatisticsUpdated(result), FailureCallback);
    }
    public void SubmitExperience(int playerXP)
    {
        PlayfabCallbackHandler.instance.UpdatePlayerStatistics(playerXP, "Experience", null, null);

        //PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        //{
        //    Statistics = new List<StatisticUpdate> {
        //    new StatisticUpdate {
        //        StatisticName = "Experience",
        //        Value = playerXP
        //    }
        //}
        //}, result => OnStatisticsUpdated(result), FailureCallback);
    }
    //private void OnStatisticsUpdated(UpdatePlayerStatisticsResult updateResult)
    //{
    //    Debug.Log("Successfully submitted high score");
    //}

    //private void FailureCallback(PlayFabError error)
    //{
    //    Debug.LogWarning("Something went wrong with your API call. Here's some debug information:");
    //    Debug.LogError(error.GenerateErrorReport());
    //}


   



    // Facebook

    public void FacebookInit()
    {
        //LoadingOverlay.instance.ShowLoadingFullscreen("Facebook init");
        FB.Init(OnFacebookInitialized);
    }
    public void FacebookLink()
    {

        try
        {
            FB.LogInWithReadPermissions(new List<string>() { "public_profile", "gaming_user_picture" }, OnFacebookLoggedIn);

        }
        catch
        {
            Debug.LogError("Issue with FB login");
        }
    }
    public void AppleLink()
    {
                #if UNITY_IOS
        //var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

        //this.appleAuthManager.LoginWithAppleId(
        //    loginArgs,
        //    credential =>
        //    {
        //        // Obtained credential, cast it to IAppleIDCredential
        //        var appleIdCredential = credential as IAppleIDCredential;
        //        if (appleIdCredential != null)
        //        {
        //            // Apple User ID
        //            // You should save the user ID somewhere in the device
        //            var userId = appleIdCredential.User;
        //            PlayerPrefs.SetString("AppleUserIdKey", userId);

        //            // Email (Received ONLY in the first login)
        //            var email = appleIdCredential.Email;

        //            // Full name (Received ONLY in the first login)
        //            var fullName = appleIdCredential.FullName;

        //            // Identity token
        //            var identityToken = Encoding.UTF8.GetString(
        //                        appleIdCredential.IdentityToken,
        //                        0,
        //                        appleIdCredential.IdentityToken.Length);
        //            PlayerPrefs.SetString("AppleidentityToken", identityToken);
        //            // Authorization code
        //            var authorizationCode = Encoding.UTF8.GetString(
        //                        appleIdCredential.AuthorizationCode,
        //                        0,
        //                        appleIdCredential.AuthorizationCode.Length);


        //                 PlayFabClientAPI.LinkApple(new LinkAppleRequest { IdentityToken = identityToken }, AppleLoginSucess, OnPlayfabAppleAuthFailed);


        //            // 
        //            //WriteTitleEventRequest callback from top
        //     //       SetAppleLinked


        //           //MainMenuController.instance.SetAppleLinked()



        //            // And now you have all the information to create/login a user in your system
        //        }
        //    },
        //    error =>
        //    {
        //        // Something went wrong
        //        var authorizationErrorCode = error.GetAuthorizationErrorCode();

        //    });



        Debug.Log("DoAppleQuickLogin");
        signInWithApple.Login((args) =>
        {
            Debug.Log("signInWithApple Done");
            Debug.Log("signInWithApple " + args.error);
            if (!string.IsNullOrEmpty(args.error))
            {
                Debug.Log("Apple -> sign in error:" + args.error);
                return;
            }
            Debug.Log("signInWithApple " + args.userInfo.idToken);
            string idToken = args.userInfo.idToken;
            //LogMessage("Apple --> Identity Token:\n" + idToken);

            PlayerPrefs.SetString("AppleUserIdKey", args.userInfo.displayName);
            PlayerPrefs.SetString("AppleidentityToken", args.userInfo.userId);
            PlayerPrefs.SetInt("UpdateAppleLogin", 1);



            //PlayFabClientAPI.LinkApple(new LinkAppleRequest { IdentityToken = args.userInfo.idToken }, AppleLoginSucess, OnPlayfabAppleAuthFailed);

            //PlayfabCallbackHandler.instance.UpdateUserData("ApppleID", args.userInfo.userId, () => {
            //    AppleLoginSucess();
            //}, () => {

            //    OnPlayfabAppleAuthFailed();
            //});


            // use idToken to login to PlayFab or any other web services
        });







#endif

    }

    public void AppleLoginSucess()
    {
        MainMenuController.instance.SetAppleLinked(true);



        SceneManager.LoadScene(0);
        // Startup._instance.Refresh(0.1f);
        PlayfabHelperFunctions.instance.ReLogin();

    }

    public void AppleUnLink()
    {
        PlayFabClientAPI.UnlinkApple(new UnlinkAppleRequest { }, OnUnlinkedA, null);

    }
    public void FacebookUnLink()
    {
        PlayFabClientAPI.UnlinkFacebookAccount(new UnlinkFacebookAccountRequest {  }, OnUnlinked, null);

    }

    private void OnUnlinkedA(PlayFab.ClientModels.EmptyResponse obj2)
    {

        PlayerPrefs.DeleteKey("AppleUserIdKey");
        PlayerPrefs.DeleteKey("AppleidentityToken");
        Debug.Log("unlinked facebook");
        MainMenuController.instance.SetFBLinked(false);
    }
    private void OnUnlinked( UnlinkFacebookAccountResult result)
    {
        PlayerPrefs.DeleteKey("FacebookLink");
        Debug.Log("unlinked facebook");
        MainMenuController.instance.SetFBLinked(false);
    }
//    private void RegisterAppForNetworkAttribution()
//    {
//#if UNITY_IOS
//        SkAdNetworkBinding.SkAdNetworkRegisterAppForNetworkAttribution();
//#endif
//    }
    private void OnFacebookInitialized()
    {
        Debug.Log("Logging into Facebook...");

//#if UNITY_IOS
//        Invoke(nameof(RegisterAppForNetworkAttribution), 1);
//#endif
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
            //if(FB.IsLoggedIn == false)
            //    FB.LogInWithReadPermissions(new List<string>() { "public_profile", "gaming_user_picture" }, OnFacebookStartupLogin);
            //else
            {
                Debug.Log("3: FacebookLogin");

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
            Debug.Log("Facebook Auth Complete! Access Token: " + AccessToken.CurrentAccessToken.UserId + "\nLogging into PlayFab...");

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
                

                PlayfabCallbackHandler.instance.UpdateUserData("FacebookTooken", AccessToken.CurrentAccessToken.UserId, () => {
                    OnPlayfabFacebookAuthComplete();
                    PlayerPrefs.DeleteKey("OldGames");
                }, () => {

                    OnPlayfabFacebookAuthFailed();
                    PlayerPrefs.DeleteKey("OldGames");
                });

                //PlayFabClientAPI.LinkFacebookAccount(new LinkFacebookAccountRequest { AccessToken = AccessToken.CurrentAccessToken.TokenString }, OnPlayfabFacebookAuthComplete, OnPlayfabFacebookAuthFailed);
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

                PlayfabCallbackHandler.instance.UpdateUserData("FacebookTooken", AccessToken.CurrentAccessToken.UserId, () => {
                    OnPlayfabFacebookAuthComplete();
                    PlayerPrefs.DeleteKey("OldGames");

                }, () => {

                    OnPlayfabFacebookAuthFailed();
                    PlayerPrefs.DeleteKey("OldGames");
                });


            }



        }
        else
        {
            LoadingOverlay.instance.DoneLoading("Facebook login");
            // If Facebook authentication failed, we stop the cycle with the message
            Debug.Log("Facebook Auth Failed: " + result.Error + "\n" + result.RawResult);
        }

 
    }
    public int failedLoginFacebook = 0;
    public void FacebookLogin()
    {
        Debug.Log("4: FacebookLogin");
        try
        {
            Debug.Log("Facebook Auth UserID: " + AccessToken.CurrentAccessToken.UserId);
            Debug.Log("Facebook Auth Complete! Access Token: " + AccessToken.CurrentAccessToken.TokenString + "\nLogging into PlayFab...");

        }
        catch
        {
            Debug.Log("5: FacebookLogin");
        }




        string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    {"DefaultAchivements", Startup._instance.myAchivmentController.GetDefault()},});


        string accessToken = "";
        Debug.Log("6: FacebookLogin");
        try
        {
            if (AccessToken.CurrentAccessToken != null && AccessToken.CurrentAccessToken.UserId.Length > 0)
            {
                PlayerPrefs.SetString("FacebookTooken", AccessToken.CurrentAccessToken.UserId);
                accessToken = AccessToken.CurrentAccessToken.UserId;
            }
            Debug.Log("7: FacebookLogin");
            if (PlayerPrefs.HasKey("FacebookTooken"))
            {
                accessToken = PlayerPrefs.GetString("FacebookTooken");
            }
        }
        catch
        {
            if (PlayerPrefs.HasKey("FacebookTooken"))
            {
                accessToken = PlayerPrefs.GetString("FacebookTooken");
            }
        }


        Debug.Log("8: FacebookLogin");
        AWSBackend.instance.AWSClientAPI("phpBackend/LoginWithCustomID.php?FacebookTooken=" + accessToken, jsonString,
              result =>
              {
                  Debug.Log("9: FacebookLogin");

                  Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);

                  LoginResult lr = new LoginResult();
                  lr.AuthenticationContext = new PlayFabAuthenticationContext();
                  lr.AuthenticationContext.PlayFabId = dic["PlayerID"];
                  lr.InfoResultPayload = new GetPlayerCombinedInfoResultPayload();
                  lr.InfoResultPayload.PlayerProfile = new PlayerProfileModel();
                  lr.InfoResultPayload.PlayerProfile.DisplayName = dic["DisplayName"];
                  lr.InfoResultPayload.AccountInfo = new UserAccountInfo();
                  lr.InfoResultPayload.AccountInfo.TitleInfo = new UserTitleInfo();
                  lr.InfoResultPayload.AccountInfo.TitleInfo.AvatarUrl = dic["avatarURL"];
                  lr.NewlyCreated = false;

                  lr.InfoResultPayload.AccountInfo.FacebookInfo = new UserFacebookInfo();
                  lr.InfoResultPayload.AccountInfo.FacebookInfo.FacebookId = accessToken;
                  Debug.Log("10: FacebookLogin");
                  LoginSucess(lr);


              },
              error =>
              {
                  Debug.Log("11: FacebookLogin");
                  Debug.LogError(error);
                  failedLoginFacebook++;
                  if(failedLoginFacebook>10)
                    PlayerPrefs.DeleteKey("FacebookLink");

                  LoadingOverlay.instance.LoadingCall.Clear();
                  PlayfabHelperFunctions.instance.ReLogin();
                  Debug.Log("12: FacebookLogin");
                  SceneManager.LoadScene(0);
                  Startup._instance.Refresh(0.1f);
              });





        //PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest()
        //{

        //    TitleId = PlayFabSettings.TitleId,
        //    AccessToken = AccessToken.CurrentAccessToken.TokenString,
        //    InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
        //    {
        //        GetUserAccountInfo = true,
        //        GetPlayerProfile = true
        //    }
        //},
        //result2 =>
        //{
        //    LoginSucess(result2);
        //},
        //error =>
        //{
        //    Debug.LogError(error.GenerateErrorReport());

        //    PlayerPrefs.DeleteKey("FacebookLink");
        //    LoadingOverlay.instance.LoadingCall.Clear();
        //    PlayfabHelperFunctions.instance.ReLogin();
        //    SceneManager.LoadScene(0);
        //    Startup._instance.Refresh(0.1f);
        //}
        //);
        
        LoadingOverlay.instance.DoneLoading("Facebook login");
    }
    public void TryAppleLogin()
    {







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
            if( result.Cancelled)
            {
                PlayerPrefs.DeleteKey("FacebookLink");
                PlayerPrefs.DeleteKey("OldGames");
                LoadingOverlay.instance.LoadingCall.Clear();
                PlayfabHelperFunctions.instance.ReLogin();
                SceneManager.LoadScene(0);
                Startup._instance.Refresh(0.1f);
            }
        }


    }



    // When processing both results, we just set the message, explaining what's going on.
    //LinkFacebookAccountResult result
    private void OnPlayfabFacebookAuthComplete()
    {
        PlayerPrefs.SetInt("FacebookLink", 1);
        PlayerPrefs.SetInt("HasDoneTutorial", 1);

        LoadingOverlay.instance.DoneLoading("Facebook login");
        //Debug.Log("PlayFab Facebook Auth Complete. Session ticket: " + result.ToJson());
        string avatarURL = "https://graph.facebook.com/" + Facebook.Unity.AccessToken.CurrentAccessToken.UserId + "/picture?type=small";

        //PlayFabClientAPI.UpdateAvatarUrl(new UpdateAvatarUrlRequest { ImageUrl = avatarURL }, OnUpdateAvatarURL, OnPlayfabFacebookAuthFailed);

        PlayfabCallbackHandler.instance.UpdateUserData("avatarURL", avatarURL, () => {

        }, () => {

             });



        LoadAvatarURL(avatarURL);

        MainMenuController.instance.SetFBLinked(true);
        // FB.API("me/picture?type=square&height=88&width=88", HttpMethod.GET, FbGetPicture);

    }



    
    private void OnPlayfabFacebookAuthFailed()
    {
        //if(error.Error == PlayFabErrorCode.LinkedAccountAlreadyClaimed || error.Error == PlayFabErrorCode.LinkedIdentifierAlreadyClaimed)
        //{
        //    Debug.Log("Should we merge?");
        //  //  MainMenuController.instance.ShowMerginAlert();


        //  //  if( MainMenuController.instance.FacebookButton.activeSelf )
        //    {
                MainMenuController.instance.ClickRecoveAccount();
        //    }


        //}
        //LoadingOverlay.instance.DoneLoading("Facebook login");
        //Debug.Log("PlayFab Facebook Auth Failed: " + error.GenerateErrorReport());
    }
    public void OnPlayfabAppleAuthFailed()
    {
        //if (error.Error == PlayFabErrorCode.LinkedAccountAlreadyClaimed || error.Error == PlayFabErrorCode.LinkedIdentifierAlreadyClaimed)
        {
            Debug.Log("Should we merge?");

            Startup._instance.PlaySoundEffect(0);
     
            SceneManager.LoadScene(0);
            // Startup._instance.Refresh(0.1f);
            PlayfabHelperFunctions.instance.ReLogin();


        }
        LoadingOverlay.instance.DoneLoading("Facebook login");
        //Debug.LogError("PlayFab Facebook Auth Failed: " + error.GenerateErrorReport());
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
        PlayfabCallbackHandler.instance.GetOtherPlayerProfile(aPlayfabId, result => {

            Debug.Log("Done getting data");


            string xp = "0";
            if (result.Data.ContainsKey("XP") == true)
            {
                xp = result.Data["XP"].Value;
            }

            string rank = "0";
            if (result.Data.ContainsKey("Ranking"))
                rank = result.Data["Ranking"].Value;

            if (SceneManager.GetActiveScene().name != "GameScene")
            {
                return;
            }
            GameManager.instance.SetOpponentData(xp, rank);



        }, (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }
    public void GetOtherUserDataRank(int aOwner , string aPlayfabId)
    {
       

        PlayfabCallbackHandler.instance.GetOtherPlayerProfile(aPlayfabId, result => {

            Debug.Log("Done getting data");


            string xp = "0";
            if (result.Data.ContainsKey("XP") == true)
            {
                xp = result.Data["XP"].Value;
            }

            string rank = "0";
            if (result.Data.ContainsKey("Ranking"))
                rank = result.Data["Ranking"].Value;

            if (SceneManager.GetActiveScene().name != "GameScene")
            {
                return;
            }

            if (aOwner == 1)
            {
                GameFinishedScreen.instance.p1_thropies.text = GameManager.instance.p1_thropies.text ;
                GameFinishedScreen.instance.p2_thropies.text = rank;
            }
            else if(aOwner == 2)
            {
                GameFinishedScreen.instance.p1_thropies.text = GameManager.instance.p1_thropies.text ;
                GameFinishedScreen.instance.p2_thropies.text = rank;
            }
            



        }, (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }
    
    public class StoredData
    {
        public Dictionary<string, UserDataRecord> theData;
        public string playfabID;
    };
    public class StoredDataProfiles
    {
        public PlayerProfileModel theData;
        public string playfabID;
    };


    public List<StoredData> storedRecords = new List<StoredData>();
    public List<StoredDataProfiles> _StoredDataProfiles = new List<StoredDataProfiles>();

    public PlayerProfileModel GetPlayerProfileModel(string aID)
    {
        for (int i = 0; i < _StoredDataProfiles.Count; i++)
        {
            if (_StoredDataProfiles[i].playfabID == aID)
            {
                return _StoredDataProfiles[i].theData;
            }
        }
        return null;
    }

    public void GetOtherUserDataProfile(string aPlayfabId, UserInfoWindow theWidnow)
    {
       for(int i = 0; i < storedRecords.Count;i++)
        {
            if( storedRecords[i].playfabID == aPlayfabId )
            {
                theWidnow.SetData(storedRecords[i].theData, aPlayfabId);
                return;
            }
        }




        PlayfabCallbackHandler.instance.GetOtherPlayerProfile(aPlayfabId, result => {
             StoredData st = new StoredData();
             st.theData = result.Data;
             st.playfabID = aPlayfabId;
             storedRecords.Add(st);
             theWidnow.SetData(result.Data, aPlayfabId);



         }, (error) => {
             Debug.Log("Got error retrieving user data:");
             Debug.Log(error.GenerateErrorReport());
         });


        //PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        //{
        //    PlayFabId = aPlayfabId,
        //    Keys = new List<string> { "Achivments", "XP", "Ranking", "MyGames" }
        //}, result => {
        //    StoredData st = new StoredData();
        //    st.theData = result.Data;
        //    st.playfabID = aPlayfabId;
        //    storedRecords.Add(st);
        //    theWidnow.SetData(result.Data, aPlayfabId);
    


        //}, (error) => {
        //    Debug.Log("Got error retrieving user data:");
        //    Debug.Log(error.GenerateErrorReport());
        //});

    }

    public void InitiateSearchForGame()
    {
        // Get online games
        string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    {"Player1", Startup.instance.MyPlayfabID}});

        AWSBackend.instance.AWSClientAPI("phpBackend/GetOpenOnlineGames.php", jsonString, (result) => {

            Debug.Log("Done!" + result);

            if (result.Length <= 0)
            {
                MainMenuController.instance.onlineGamesLabel.enabled = false;
                // No online games, create new entry
                StartSearchGame();
                //TODO

                return;
            }

            OpenGameList list = JsonUtility.FromJson<OpenGameList>("{\"myOpenGamesList\":" + result + "}");

            int openGames = 0;
            List<OpenGame> avalibleGames = new List<OpenGame>();
            foreach (var item in list.myOpenGamesList)
            {
                if (item.Player1 != Startup.instance.MyPlayfabID)
                {
                    avalibleGames.Add(item);
                    openGames++;
                }
            }

            if (MainMenuController.instance != null)
            {
                if (openGames <= 0)
                {
                    //No online games, we create a new entry
                    MainMenuController.instance.onlineGamesLabel.enabled = false;
                    StartSearchGame();
                }
                else
                {
                    //MainMenuController.instance.onlineGamesLabel.enabled = true;
                    MainMenuController.instance.onlineGamesLabel.text = openGames + " games available!";

                    // If Online game exists
                    // Close game and start new game against user





                    //bool hasActiveGameAgainstPlayer = false;
                    //for (int i = 0; i < Startup._instance.openGamesList.Count; i++)
                    //{
                    //    if (Startup._instance.openGamesList[i].player1_PlayfabId == theProfile.PlayerId || Startup._instance.openGamesList[i].player2_PlayfabId == theProfile.PlayerId)
                    //    {
                    //        hasActiveGameAgainstPlayer = true;
                    //    }
                    //}

                    PlayfabCallbackHandler.instance.GetOtherPlayerProfile(avalibleGames[0].Player1, result => {

                        Debug.Log("Done getting data");

                        

                        string DisplayName = "Undefined";
                        if (result.Data.ContainsKey("DisplayName"))
                            DisplayName = result.Data["DisplayName"].Value;

                        LoadingOverlay.instance.ShowLoadingFullscreen("Challenge in progress..");
                        string newRoomName = string.Format("{0}-{1}", avalibleGames[0].Player1 + "_" + avalibleGames[0].Player2 + "_", UnityEngine.Random.Range(0, 1000000).ToString()) + UnityEngine.Random.Range(0, 1000000).ToString();    // for int, Random.Range is max-exclusive!
                        PlayfabHelperFunctions.instance.ChallengePlayer(Startup.instance.MyPlayfabID, avalibleGames[0].Player1, DisplayName, newRoomName);
                        PlayfabHelperFunctions.instance.AddGameToPlayerListCloudScript(avalibleGames[0].Player1, newRoomName);
                        PlayfabHelperFunctions.instance.AddGameToPlayerListCloudScript(avalibleGames[0].Player2, newRoomName);
                        StartCoroutine(ChallengeProgress());
                        CloseSearchGame(avalibleGames[0].Player1, Startup.instance.MyPlayfabID, avalibleGames[0].id);




                    }, (error) => {
                        Debug.Log("Got error retrieving user data:");
                        Debug.Log(error.GenerateErrorReport());
                    });


            

                }
            }


        }, (error) => {
            Debug.Log("Error!" + error);
            PlayFabError pfE = new PlayFabError();

        });



    }

    public IEnumerator ChallengeProgress()
    {

        yield return new WaitForSeconds(3);
        LoadingOverlay.instance.DoneLoading("Challenge in progress..");
        Startup._instance.Refresh();
        yield return new WaitForSeconds(2);




    }
    public void StartSearchGame()
    {
        string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    {"Player1", Startup.instance.MyPlayfabID}});

        AWSBackend.instance.AWSClientAPI("phpBackend/AddSearchForGame.php", jsonString, (result) => {
            Debug.Log("Done!" + result);
            if (Startup._instance.SearchingForGameObject == null)
            {
                Startup._instance.SearchingForGameObject = (GameObject)GameObject.Instantiate(PlayfabHelperFunctions.instance.SearchingForGamePrefab, MainMenuController.instance._GameListParent);
                Startup._instance.SearchingForGameObject.transform.SetAsFirstSibling();
                Startup._instance.SearchingForGameObject.SetActive(true);
                Vector3 rc = Startup._instance.SearchingForGameObject.GetComponent<RectTransform>().localPosition;
                Startup._instance.SearchingForGameObject.GetComponent<RectTransform>().localPosition = new Vector3(rc.x, rc.y, 0);
                Startup._instance.SearchingForGameObject.GetComponent<SearchGameInfo>().NameID = "LookingForOnlineGame";
            }
            else
            {

            }

        }, (error) => {
            Debug.Log("Error!" + error);
            PlayFabError pfE = new PlayFabError();
            if (Startup._instance.SearchingForGameObject == null)
            {
                Startup._instance.SearchingForGameObject = (GameObject)GameObject.Instantiate(PlayfabHelperFunctions.instance.SearchingForGamePrefab, MainMenuController.instance._GameListParent);
                Startup._instance.SearchingForGameObject.transform.SetAsFirstSibling();
                Startup._instance.SearchingForGameObject.SetActive(true);
                Vector3 rc = Startup._instance.SearchingForGameObject.GetComponent<RectTransform>().localPosition;
                Startup._instance.SearchingForGameObject.GetComponent<RectTransform>().localPosition = new Vector3(rc.x, rc.y, 0);
                Startup._instance.SearchingForGameObject.GetComponent<SearchGameInfo>().NameID = "LookingForOnlineGame";
            }
            else
            {

            }
        });
    }

    [System.Serializable]
    public class OpenGameList
    {
        public OpenGame[] myOpenGamesList;
    }
    [System.Serializable]
    public class OpenGame
    {
        public string id;
        public string PlayerID;
        public string Player1;
        public string Player2;
        public string TimeCreated;
        public string TimeClosed;
        public string isOpen;

    }

    public void GetOpenOnlineGames_Self()
    {
        string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    {"Player1", Startup.instance.MyPlayfabID}});

        AWSBackend.instance.AWSClientAPI("phpBackend/GetOpenOnlineGames_Self.php", jsonString, (result) => {

            Debug.Log("Done!" + result);

            if (result.Length <= 0)
            {
                if (Startup._instance.SearchingForGameObject != null)
                    Destroy(Startup._instance.SearchingForGameObject);
                return;
            }

            OpenGameList list = JsonUtility.FromJson<OpenGameList>("{\"myOpenGamesList\":" + result + "}");

            int openGames = 0;
            bool playerHasAGame = false;
   
            if (list.myOpenGamesList.Length>0)
                playerHasAGame = true;

   
            if (playerHasAGame)
            {
                if (Startup._instance.SearchingForGameObject == null)
                {
                    Startup._instance.SearchingForGameObject = (GameObject)GameObject.Instantiate(PlayfabHelperFunctions.instance.SearchingForGamePrefab, MainMenuController.instance._GameListParent);
                    Startup._instance.SearchingForGameObject.transform.SetAsFirstSibling();
                    Startup._instance.SearchingForGameObject.SetActive(true);
                    Vector3 rc = Startup._instance.SearchingForGameObject.GetComponent<RectTransform>().localPosition;
                    Startup._instance.SearchingForGameObject.GetComponent<RectTransform>().localPosition = new Vector3(rc.x, rc.y, 0);
                    Startup._instance.SearchingForGameObject.GetComponent<SearchGameInfo>().NameID = "LookingForOnlineGame";
                }
                else
                {

                }
            }
            else
            {
                if (Startup._instance.SearchingForGameObject != null)
                    Destroy(Startup._instance.SearchingForGameObject);

            }



        }, (error) => {
            Debug.Log("Error!" + error);
            PlayFabError pfE = new PlayFabError();


        });
    }


    public void GetOpenOnlineGamesForLabel()
    {
        string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    {"Player1", Startup.instance.MyPlayfabID}});

        AWSBackend.instance.AWSClientAPI("phpBackend/GetOpenOnlineGames.php", jsonString, (result) => {

            Debug.Log("Done!" + result);

            if(result.Length<=0)
            {
                MainMenuController.instance.onlineGamesLabel.enabled = false;
                return;
            }

            OpenGameList list = JsonUtility.FromJson<OpenGameList>("{\"myOpenGamesList\":" + result + "}");

            int openGames = 0;
            bool playerHasAGame = false;
            foreach (var item in list.myOpenGamesList)
            {
                if(item.Player1 != Startup.instance.MyPlayfabID)
                {
                    openGames++;
                }
                else
                {
                    if(item.isOpen == "1")
                    playerHasAGame = true;
                    // There is a looking for game that is owned by the user
           
                }
            }
            if(playerHasAGame)
            {
                if (Startup._instance.SearchingForGameObject == null)
                {
                    Startup._instance.SearchingForGameObject = (GameObject)GameObject.Instantiate(PlayfabHelperFunctions.instance.SearchingForGamePrefab, MainMenuController.instance._GameListParent);
                    Startup._instance.SearchingForGameObject.transform.SetAsFirstSibling();
                    Startup._instance.SearchingForGameObject.SetActive(true);
                    Vector3 rc = Startup._instance.SearchingForGameObject.GetComponent<RectTransform>().localPosition;
                    Startup._instance.SearchingForGameObject.GetComponent<RectTransform>().localPosition = new Vector3(rc.x, rc.y, 0);
                    Startup._instance.SearchingForGameObject.GetComponent<SearchGameInfo>().NameID = "LookingForOnlineGame";
                }
                else
                {

                }
            }
            else
            {
                if(Startup._instance.SearchingForGameObject != null)
                    Destroy(Startup._instance.SearchingForGameObject);

            }

            if (MainMenuController.instance != null)
            {
                if (openGames <= 0)
                    MainMenuController.instance.onlineGamesLabel.enabled = false;
                else
                {
                    //MainMenuController.instance.onlineGamesLabel.enabled = true;
                    MainMenuController.instance.onlineGamesLabel.text = openGames + " games available!";

                }
            }           


        }, (error) => {
            Debug.Log("Error!" + error);
            PlayFabError pfE = new PlayFabError();


        });
    }
    public void CloseSearchGame(string p1,string p2, string gameID)
    {
        string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    {"Player1", p1},
                    {"Player2", p2},
                    {"gameID", gameID},
        });

        AWSBackend.instance.AWSClientAPI("phpBackend/CloseSearchGame.php", jsonString, (result) =>
        {
            Debug.Log("Done!" + result);
            //Start online game

        }, (error) => {
            Debug.Log("Error!" + error);
            PlayFabError pfE = new PlayFabError();

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
