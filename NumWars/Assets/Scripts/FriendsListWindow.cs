using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        RequestLeaderboard();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    //Get the players with the top 10 high scores in the game
    public void RequestLeaderboard()
    {
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
        {
            IncludeFacebookFriends=true,
             ProfileConstraints = new PlayerProfileViewConstraints()
             {
                 ShowDisplayName = true,
                 ShowAvatarUrl = true
             }

        }, result => GetLeadoardOfFriends(result), FailureCallback);




    }


    private void FailureCallback(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your API call. Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }


    public void GetLeadoardOfFriends(GetFriendsListResult result)
    {
        PlayFabClientAPI.GetFriendLeaderboard(new GetFriendLeaderboardRequest
        {
            StatisticName = "Highscore",

            MaxResultsCount = 100,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,
                ShowAvatarUrl = true
            }

        }, result2 => DisplayLeaderboard(result,result2), FailureCallback);
    }


public void DisplayLeaderboard(GetFriendsListResult result, GetLeaderboardResult result2)
    {
        foreach (Transform child in _parent)
        {
            GameObject.Destroy(child.gameObject);
        }


        for (int i = 0; i < result.Friends.Count;i++)
        {
            GameObject go =  GameObject.Instantiate(templateItem, _parent);

            go.transform.GetChild(2).GetComponent<Text>().text = result.Friends[i].Profile.DisplayName;
            go.transform.GetChild(4).GetComponent<Text>().text = "";
            go.transform.GetChild(1).GetComponent<Text>().text = "";

            for(int j = 0; j< result2.Leaderboard.Count;j++)
            {
                if( result2.Leaderboard[j].PlayFabId == result.Friends[i].FriendPlayFabId)
                {
                    go.transform.GetChild(1).GetComponent<Text>().text = result2.Leaderboard[j].StatValue.ToString();
                }
            }


            

            string avatarURL = "";

            if(result.Friends[i].Profile.AvatarUrl != null)
            avatarURL = result.Friends[i].Profile.AvatarUrl;

            Image img = go.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            go.GetComponent<FriendListItem>().Init(result.Friends[i].Profile, result.Friends[i].Profile.DisplayName, go.transform.GetChild(1).GetComponent<Text>().text, img);

            if (avatarURL.Length>0)
            StartCoroutine(SetPicture(avatarURL, img));

            
        }

    }

    private IEnumerator SetPicture(string aURL, Image aImage)
    {
        WWW www = new WWW(aURL + "&access_token=GG|817150566351647|GXmlbSYVrHYJ1h7CJj7t9cGxwrE");
        yield return www;
        Texture2D profilePic = www.texture;

        aImage.sprite = Sprite.Create((Texture2D)profilePic, new Rect(0, 0, profilePic.height, profilePic.width), new Vector2());
        aImage.rectTransform.sizeDelta = new Vector2(88, 88);


    }

    public void AddFriend()
    {
        Debug.Log("Adding Friend:" + inputF.text);
        

        PlayFabClientAPI.AddFriend(new AddFriendRequest
        {
            FriendTitleDisplayName= inputF.text

        }, result => AddeFriendCallback(result), FailureCallbackAdd);
    }
    public void AddeFriendCallback(AddFriendResult result)
    {
        RequestLeaderboard();

        errorText.text = "";
    }
    private void FailureCallbackAdd(PlayFabError error)
    {
        errorText.text = "Could not add friend";
        Debug.LogWarning("Cant Add Friend");
        Debug.LogError(error.GenerateErrorReport());
    }


}
