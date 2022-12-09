using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameAnalyticsSDK;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class FriendsListWindow : MonoBehaviour
{

    public static FriendsListWindow instance;
    public GameObject templateItem;
    public Transform _parent;

    public InputField inputF;
    public Text errorText;
    public GameObject UserInfoWindow;
    public Text _displayName;
    public Text _thropies;
    public Image _profilepicture;
    public Vector3 _TextFlyInBoxoriginalPos;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        _TextFlyInBoxoriginalPos = UserInfoWindow.transform.GetChild(1).transform.position;

    }
    public void ClickOnItem(PlayerProfileModel aProfile,string displayName, string thropies, Sprite profileP)
    {
        UserInfoWindow.SetActive(true);

        _displayName.text = displayName;
        _thropies.text = thropies;
        _profilepicture.sprite = profileP;






        UserInfoWindow.SetActive(true);
        UserInfoWindow.transform.GetChild(0).GetComponent<Image>().DOFade(157f / 255f, 0).SetEase(Ease.InOutQuart);

        UserInfoWindow.transform.GetChild(1).gameObject.SetActive(true);
        UserInfoWindow.transform.GetChild(1).transform.position = new Vector3(_TextFlyInBoxoriginalPos.x - 10, _TextFlyInBoxoriginalPos.y, _TextFlyInBoxoriginalPos.z);
        UserInfoWindow.transform.GetChild(1).transform.DOMoveX(_TextFlyInBoxoriginalPos.x, 0.3f).SetEase(Ease.InOutQuart);

        UserInfoWindow.GetComponent<UserInfoWindow>().theProfile = aProfile;
        UserInfoWindow.GetComponent<UserInfoWindow>().Init();

    }
    void OnEnable()
    {
        errorText.text = "";
        inputF.text = "";
        foreach (Transform child in _parent)
        {
            GameObject.Destroy(child.gameObject);
        }

        RequestLeaderboard();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    //Get the players with the top 10 high scores in the game
    public void RequestLeaderboard(bool forceUpdate = false)
    {

        if(PlayerPrefs.HasKey("GetLeaderboardResult") )
        {
            PlayerPrefs.DeleteKey("GetLeaderboardResult");
            PlayerPrefs.DeleteKey("GetFriendsListResult");
        }


        if(PlayerPrefs.HasKey("GetFriendsListResult") && forceUpdate == false)
        {
            LoadLocalInfo();
        }
        else
        {
            AWSBackend.instance.AWSClientAPI("phpBackend/GetFriendsList.php?PlayerID=" + Startup.instance.MyPlayfabID, "",
             result =>
             {
             if (result.Contains("Error: user not found!"))
             {
                 return;
             }

             GetFriendsListResult rs = new GetFriendsListResult();
            //rs.Friends

            AWSBackend.FriendsList list = JsonUtility.FromJson<AWSBackend.FriendsList>("{\"myFriendsList\":" + result + "}");
             rs.Friends = new List<FriendInfo>();
             for (int i = 0; i < list.myFriendsList.Length; i++)
             {
                 Debug.Log(list.myFriendsList[i].DisplayName + " : " + list.myFriendsList[i].PlayerId);


                 FriendInfo finfo = new FriendInfo();
                 finfo.Profile = new PlayerProfileModel();
                 finfo.Profile.AvatarUrl = list.myFriendsList[i].avatarURL;
                 finfo.Profile.DisplayName = list.myFriendsList[i].DisplayName;
                 finfo.Profile.PlayerId = list.myFriendsList[i].PlayerId;
                 finfo.Tags = new List<string>();
                 finfo.Tags.Add(list.myFriendsList[i].Ranking);
                 finfo.Tags.Add(list.myFriendsList[i].XP);
                 rs.Friends.Add(finfo);

                }
                GetLeadoardOfFriends(rs);
            },
             error =>
             {
                 PlayFabError pfE = new PlayFabError();
                 FailureCallback(pfE);
             }
            );


            //PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
            // {
            //     IncludeFacebookFriends = true,
            //     ProfileConstraints = new PlayerProfileViewConstraints()
            //     {
            //         ShowDisplayName = true,
            //         ShowAvatarUrl = true,
            //         ShowStatistics = true
            //     }

            // }, result => GetLeadoardOfFriends(result), FailureCallback);
        }



        


    }
    public void LoadLocalInfo()
    {
        var json = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);
        GetFriendsListResult res = json.DeserializeObject<GetFriendsListResult>(PlayerPrefs.GetString("GetFriendsListResult"));
        //GetLeaderboardResult res2 = json.DeserializeObject<GetLeaderboardResult>(PlayerPrefs.GetString("GetLeaderboardResult"));

        DisplayLeaderboard(res,null);



        //PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
        //{
        //    IncludeFacebookFriends = true,
        //    ProfileConstraints = new PlayerProfileViewConstraints()
        //    {
        //        ShowDisplayName = true,
        //        ShowAvatarUrl = true,
        //        ShowStatistics = true
        //    }

        //}, result => GetLeadoardOfFriends(result), FailureCallback);

    }

    private void FailureCallback(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your API call. Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }


    public void GetLeadoardOfFriends(GetFriendsListResult result)
    {
        //PlayFabClientAPI.GetFriendLeaderboard(new GetFriendLeaderboardRequest
        //{
        //    StatisticName = "Highscore",

        //    MaxResultsCount = 100,
        //    ProfileConstraints = new PlayerProfileViewConstraints()
        //    { 
        //        ShowDisplayName = true,
        //        ShowAvatarUrl = true,
        //        ShowStatistics=true
        //    }

        //}, result2 => DisplayLeaderboard(result,result2,true), FailureCallback);

        DisplayLeaderboard(result, null, true);
    }


public void DisplayLeaderboard(GetFriendsListResult result, GetLeaderboardResult result2,bool newData = false)
    {
        if(newData)
        {
            PlayerPrefs.SetString("GetFriendsListResult", result.ToJson());
            //PlayerPrefs.SetString("GetLeaderboardResult", result2.ToJson());
            foreach (Transform child in _parent)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        
            for (int i = 0; i < result.Friends.Count; i++)
            {
                GameObject go = GameObject.Instantiate(templateItem, _parent);

                go.transform.GetChild(4).GetComponent<Text>().text = result.Friends[i].Profile.DisplayName;
                go.transform.GetChild(3).GetComponent<Text>().text = result.Friends[i].Tags[0];
                go.transform.GetChild(6).GetComponent<Text>().text = "";
            //   go.transform.GetChild(5).GetChild(0).GetComponent<Text>().text = HelperFunctions.XPtoLevel();


            //for (int j = 0; j < result2.Leaderboard.Count; j++)
            //{
            //    if (result2.Leaderboard[j].PlayFabId == result.Friends[i].FriendPlayFabId)
            //    {
            //        go.transform.GetChild(3).GetComponent<Text>().text = result2.Leaderboard[j].StatValue.ToString();
            //    }
            //}


           

                string avatarURL = "";

                if (result.Friends[i].Profile.AvatarUrl != null)
                    avatarURL = result.Friends[i].Profile.AvatarUrl;

                Image img = go.transform.GetChild(2).GetChild(0).GetComponent<Image>();
                go.GetComponent<FriendListItem>().Init(result.Friends[i].Profile, result.Friends[i].Profile.DisplayName, go.transform.GetChild(3).GetComponent<Text>().text, img);

                if (avatarURL.Length > 0)
                {
                    ProfilePictureManager.instance.SetPicture(avatarURL, result.Friends[i].Profile.PlayerId, img);
                    //StartCoroutine(SetPicture(avatarURL, img));
                }




                bool hasFoundxp = false;
                //for (int j = 0; j < result.Friends[i].Profile.Statistics.Count; j++)
                //{
                //    if (result.Friends[i].Profile.Statistics[j].Name == "Experience")
                //    {
                //        go.transform.GetChild(6).GetComponent<Text>().text = HelperFunctions.XPtoLevel(result.Friends[i].Profile.Statistics[1].Value.ToString()).ToString();
                //        hasFoundxp = true;
                //    }
                //}


                //if (hasFoundxp == false)
                //{
                //    go.transform.GetChild(6).gameObject.SetActive(false);
                //}
                if(result.Friends[i].Tags.Count>1 && result.Friends[i].Tags[1] !=null && result.Friends[i].Tags[1].Length>0)
            {
                go.transform.GetChild(6).gameObject.SetActive(true);
                go.transform.GetChild(6).GetComponent<Text>().text = HelperFunctions.XPtoLevel(result.Friends[i].Tags[1]).ToString();

            }



        }
        
    }

    //private IEnumerator SetPicture(string aURL, Image aImage)
    //{
    //    WWW www = new WWW(aURL + "&access_token=GG|817150566351647|GXmlbSYVrHYJ1h7CJj7t9cGxwrE");
    //    yield return www;
    //    Texture2D profilePic = www.texture;

    //    aImage.sprite = Sprite.Create((Texture2D)profilePic, new Rect(0, 0, profilePic.height, profilePic.width), new Vector2());
    //    aImage.rectTransform.sizeDelta = new Vector2(88, 88);


    //}

    public void AddFriend()
    {
        Debug.Log("Adding Friend:" + inputF.text);


        //PlayFabClientAPI.AddFriend(new AddFriendRequest
        //{
        //    FriendTitleDisplayName= inputF.text

        //}, result => AddeFriendCallback(result), FailureCallbackAdd);


        string jsonString = JsonConvert.SerializeObject(new Dictionary<string, string>() {
                    {"PlayerID", Startup.instance.MyPlayfabID},
                    {"PlayerId_Friend", inputF.text},
                    {"DisplayName_Friend", inputF.text} });

        AWSBackend.instance.AWSClientAPI("phpBackend/AddFriend.php", jsonString, (result) => {
            Debug.Log("Done!" + result);
            AddeFriendCallback();
        }, (error) => {
            Debug.Log("Error!" + error);
            PlayFabError pfE = new PlayFabError();
            FailureCallbackAdd(pfE);
        });

    }
    public void AddeFriendCallback()
    {
        GameAnalytics.NewDesignEvent("AddFriend");

        foreach (Transform child in _parent)
        {
            GameObject.Destroy(child.gameObject);
        }
        RequestLeaderboard(true);

        errorText.text = "";
        inputF.text = "";
    }
    private void FailureCallbackAdd(PlayFabError error)
    {

        var json = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);
        GetFriendsListResult res = json.DeserializeObject<GetFriendsListResult>(PlayerPrefs.GetString("GetFriendsListResult"));

        for (int i = 0; i < res.Friends.Count; i++)
        {
            if(res.Friends[i] != null && res.Friends[i].Profile != null && res.Friends[i].Profile.DisplayName == inputF.text)
            {
                errorText.text = "Already friends!";
                return;
            }
            
  

        }


        //if (error.Error == PlayFabErrorCode.UsersAlreadyFriends)
        {
            errorText.text = "Could not add friend!";
            return;
        }

        //errorText.text = "Could not add friend";
        //Debug.LogWarning("Cant Add Friend");
        //Debug.LogError(error.GenerateErrorReport());
        
        //MainMenuController.instance.Share();
    }


}
