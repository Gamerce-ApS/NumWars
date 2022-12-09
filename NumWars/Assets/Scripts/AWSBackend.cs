using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class AWSBackend : MonoBehaviour
{

    public static AWSBackend _instance = null;
    public static AWSBackend instance
    {
        get
        {
            if (_instance == null && GameObject.Find("AWSBackend") != null)
                _instance = GameObject.Find("AWSBackend").GetComponent<AWSBackend>();
            return _instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (AWSBackend._instance != null)
            Destroy(gameObject);

        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //DONE
    //LoginWithCustomID, LoginWithFacebook.LoginWithApple
    //LoginWithIOSDeviceID,
    //GetTitleData,
    //GetPlayerProfile,
    //GetUserData,
    //GetOtherPlayerProfile,
    //GetSharedGroupData,
    //UpdateSharedGroupData (CreateSharedGroup)
    //UpdateUserData (SetUserData) (GetUserDataOldGames) (UpdateUserDataGrouped) (UpdateAvatarUrl) (UpdateUserTitleDisplayName) (LinkFacebookAccount) (UnlinkFacebookAccount) (LinkApple) (UnlinkIOSDeviceID)
    //UpdatePlayerStatistics, (UpdatePlayerStatisticsGrouped)
    //ExecuteCloudScript --> UpdateSharedGroupData  (adds xp and stats)
    //ExecuteCloudScript --> AddPlayerToSharedGroupAndToGameList (much more functionality)
    //ExecuteCloudScript --> CheckIfGameInListElseAdd
    //ExecuteCloudScript --> AddGameToPlayerList
    //GetAllAvatarGrouped,
    //GetSharedDataGrouped2,

    //WIP
    //ChallengePlayer (Send push to user)



    //RemoveFriend
    //AddFriend
    //GetFriendsList
    //GetFriendLeaderboard
    //RegisterForIOSPushNotification
    //LoginWithAndroidDeviceID
    //
    //
    //
    //









    //Not needed
    //RemoveSharedGroupMembers,
    //PlayFabDataAPI.InitiateFileUploads
    //PlayFabDataAPI.FinalizeFileUploads
    //PlayFabDataAPI.AbortFileUploads
    //PlayFabDataAPI.GetFiles
    //GetFilesRequest,
    //InitiateFileUploads,
    //ClearOldGames

    //Maybe
    //AddPlayerToSharedGroup --> (maybe we dont need this?)


    private void OnGUI()
    {
        return;
        if( GUILayout.Button("Login DeviceID") )
        {
            StartCoroutine(SendAPICall("phpBackend/LoginWithCustomID.php?DeviceId="+ SystemInfo.deviceUniqueIdentifier, "",(result)=> {
                Debug.Log("Done!" + result);
            }, (error) => {
                Debug.Log("Error!" + error);
            }));
        }

        if (GUILayout.Button("Login CustomID"))
        {
            StartCoroutine(SendAPICall("phpBackend/LoginWithCustomID.php?CustomID=PatrikB_3", "", (result) => {
                Debug.Log("Done!" + result);
            }, (error) => {
                Debug.Log("Error!" + error);
            }));
        }

        if (GUILayout.Button("SetUserData"))
        {
            string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    {"PlayerID", "1441807856"},
                    {"XP", "13"},
                    {"Ranking", "1"}});

            StartCoroutine(SendAPICall("phpBackend/SetUserData.php", jsonString, (result) => {
                Debug.Log("Done!" + result);
            }, (error) => {
                Debug.Log("Error!" + error);
            }));
        }

        if (GUILayout.Button("GetSharedGroupData"))
        {
            StartCoroutine(SendAPICall("phpBackend/GetSharedGroupData.php?SharedGroupId=RandomRoomNr123", "", (result) => {
                Debug.Log("Done!" + result);
            }, (error) => {
                Debug.Log("Error!" + error);
            }));


        }

        if (GUILayout.Button("UpdateSharedGroupData"))
        {
            string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    {"SharedGroupId", "RandomRoomNr123"},
                    {"data", "theData"}});

            StartCoroutine(SendAPICall("phpBackend/UpdateSharedGroupData.php", jsonString, (result) => {
                Debug.Log("Done!" + result);
            }, (error) => {
                Debug.Log("Error!" + error);
            }));
        }


        if (GUILayout.Button("UpdatePlayerStatistics"))
        {
            string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    {"PlayerID", "1441807856"},
                    //{"Experience", "20"},
                    {"Highscore", "13"}});

            StartCoroutine(SendAPICall("phpBackend/UpdatePlayerStatistics.php", jsonString, (result) => {
                Debug.Log("Done!" + result);
            }, (error) => {
                Debug.Log("Error!" + error);
            }));
        }




        if (GUILayout.Button("UpdateSharedGroupData_cloudscript"))
        {
            string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    {"PlayerID", "1441807856"},
                    {"SharedGroupId", "20"},
                    {"data", "jsonDataForGame2"}});

            StartCoroutine(SendAPICall("phpBackend/UpdateSharedGroupData_cloudscript.php", jsonString, (result) => {
                Debug.Log("Done!" + result);
            }, (error) => {
                Debug.Log("Error!" + error);
            }));
        }


        if (GUILayout.Button("AddPlayerToSharedGroupAndToGameList_cloudscript"))
        {
            string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    {"PlayerID_1", "1441807856"},
                    {"PlayerID_2", "647590885"},
                    {"SharedGroupId", "20"}});

            StartCoroutine(SendAPICall("phpBackend/AddPlayerToSharedGroupAndToGameList_cloudscript.php", jsonString, (result) => {
                Debug.Log("Done!" + result);
            }, (error) => {
                Debug.Log("Error!" + error);
            }));
        }


        if (GUILayout.Button("GetAllAvatarGrouped"))
        {
            string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    {"PlayerID", "1441807856"},});

            StartCoroutine(SendAPICall("phpBackend/GetAllAvatarGrouped.php", jsonString, (result) => {
                Debug.Log("Done!" + result);
            }, (error) => {
                Debug.Log("Error!" + error);
            }));
        }

        if (GUILayout.Button("GetSharedDataGrouped2_cloudscript"))
        {
            string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    {"PlayerID", "1441807856"},});

            StartCoroutine(SendAPICall("phpBackend/GetSharedDataGrouped2_cloudscript.php", jsonString, (result) => {
                Debug.Log("Done!" + result);
            }, (error) => {
                Debug.Log("Error!" + error);
            }));
        }


        if (GUILayout.Button("GetFriendsList"))
        {


            StartCoroutine(SendAPICall("phpBackend/GetFriendsList.php?PlayerID=1441807856", "", (result) => {
                Debug.Log("Done!" + result);
                //result = result.Replace("[", "");
                //result = result.Replace("]", "");
                Debug.Log( result);
                //FriendsList list = JsonUtility.FromJson<FriendsList>(result);

                //var obj = SimpleJSON.JSON.Parse(result);
                //foreach (var kvp in obj)
                //{
                //    Debug.Log("Dict = " + kvp.Key + " : " + kvp.Value.Value);
                //}


                FriendsList list = JsonUtility.FromJson<FriendsList>("{\"myFriendsList\":" + result + "}" );

                for (int i = 0; i < list.myFriendsList.Length; i++)
                {
                    Debug.Log(list.myFriendsList[i].DisplayName + " : " + list.myFriendsList[i].PlayerId + " : " + list.myFriendsList[i].avatarURL);
                }



            }, (error) => {
                Debug.Log("Error!" + error);
            }));
        }
        if (GUILayout.Button("AddFriend"))
        {

            string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    {"PlayerID", "1441807856"},
                    {"PlayerId_Friend", "1243534"+UnityEngine.Random.Range(10,10000)},
                    {"DisplayName_Friend", "paxi"+UnityEngine.Random.Range(10,10000)}});

            StartCoroutine(SendAPICall("phpBackend/AddFriend.php", jsonString, (result) => {
                Debug.Log("Done!" + result);

            }, (error) => {
                Debug.Log("Error!" + error);
            }));
        }
        if (GUILayout.Button("RemoveFriend"))
        {

            string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    {"PlayerID", "1441807856"},
                    {"PlayerId_Friend", "12435344369"}
                    });

            StartCoroutine(SendAPICall("phpBackend/RemoveFriend.php", jsonString, (result) => {
                Debug.Log("Done!" + result);

            }, (error) => {
                Debug.Log("Error!" + error);
            }));
        }

        if (GUILayout.Button("GetLeadboard"))
        {


            StartCoroutine(SendAPICall("phpBackend/GetLeadboard.php", "", (result) =>
            {
                Debug.Log("Done!" + result);

            }, (error) => {
                Debug.Log("Error!" + error);
            }));
        }



        





    }
    [System.Serializable]
    public class FriendsList
    {
        public Friend[] myFriendsList;
    }
    [System.Serializable]
    public class Friend
    {
        public string PlayerId;
        public string DisplayName;
        public string avatarURL;
        public string Ranking;
        public string XP;

    }
    [System.Serializable]
    public class LeadboardList
    {
        public Leadboard[] myLeadboardList;
    }
    [System.Serializable]
    public class Leadboard
    {
        public string PlayerID;
        public string DisplayName;
        public string avatarURL;
        public string Value;
        public string rank;
        public string XP;

    }
    public void AWSClientAPI(string aAPI, string data, System.Action<string> onComplete, System.Action<string> onError)
    {
        StartCoroutine(SendAPICall(aAPI, data, (result) => {
            onComplete.Invoke(result);
        }, (error) => {
            onError.Invoke(error);
        }));
    }



    //NOT NEEDED
    //SetPictureIE,
    public void LoginIO()
    {

    }



    // sends an API request - returns a JSON file
    IEnumerator SendAPICall(string aAPI,string data, System.Action<string> onComplete, System.Action<string> onError)
    {


    
        string serverURL = "http://3.90.160.77/";
        // create the web request and download handler
       UnityWebRequest webReq = new UnityWebRequest(serverURL + aAPI, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(data);
        webReq.downloadHandler = new DownloadHandlerBuffer();
        webReq.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        webReq.SetRequestHeader("Content-Type", "application/json");
        // build the url and query


        yield return webReq.Send();



        if (webReq.result == UnityWebRequest.Result.ConnectionError || webReq.result == UnityWebRequest.Result.ProtocolError || webReq.downloadHandler.text.Contains("Error"))
        {
            onError(webReq.error);
        }
        else
        {
            onComplete(webReq.downloadHandler.text);
        }

        webReq.Dispose();

    }
}
