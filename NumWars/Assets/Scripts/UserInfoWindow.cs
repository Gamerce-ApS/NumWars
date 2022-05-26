using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoWindow : MonoBehaviour
{
public PlayerProfileModel theProfile;
    public Text ChallengeButton;

    public Text winT;
    public Text LoseT;

    public Text amountCompletedText;
    public Image AchivmenSlider;

    public Text _level;

    public List<StatsDataText> statsData = new List<StatsDataText>();

    public Image _profilePic;

    public Text _name;
    public Text _thropies;

    public bool isAIInfo = false;

    public void InitUser(int aUserId)
    {
        _name.text = "";
        _thropies.text = "";
        _level.text = "";
        amountCompletedText.text = "";
        winT.text = "";
        LoseT.text = "";
        _profilePic.enabled = false;
        for (int i = 0; i < statsData.Count; i++)
        {
            statsData[i].SetText("");
        }


        if (aUserId == 1 && GameManager.instance.thePlayers[1].isAI == false)
        {
            PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
            {
                PlayFabId = Startup._instance.GameToLoad.GetOtherPlayerPlayfab(),
                 ProfileConstraints = new PlayerProfileViewConstraints()
                 {
                     ShowDisplayName = true,
                     ShowAvatarUrl = true
                 }
            }, result => {

                theProfile = result.PlayerProfile;
                InitUserAfterDataSet(aUserId);

            }, (error) => {
                Debug.Log("Got error retrieving user data:");
                Debug.Log(error.GenerateErrorReport());
            });
        }
        else
        {
            InitUserAfterDataSet(aUserId);
        }
    }

    public void InitUserAfterDataSet(int aUserId)
    {

     









 
        isAIInfo = false;
            StatsData _statsData = Startup._instance.GetStatsData();
        int amountLost = 0;
        int amountWin = 0;
        for (int i = 0; i < _statsData.FinishedGames.Count; i++)
        {
            if (_statsData.FinishedGames[i].PlayfabID == theProfile.PlayerId)
            {
                if (_statsData.FinishedGames[i].Winner == Startup._instance.MyPlayfabID)
                    amountWin++;
                else
                    amountLost++;
            }
        }
        winT.text = amountWin.ToString();
        LoseT.text = amountLost.ToString();


        if(aUserId ==1)
        {
            if( GameManager.instance.thePlayers[1].isAI )
            {
                _profilePic.enabled = false;
                _name.text = "AI";
                _thropies.text = "";
                SetAiStats();
                isAIInfo = true;
            }
            else
            {
                PlayfabHelperFunctions.instance.GetOtherUserDataProfile(theProfile.PlayerId, this);
                LoadAvatarURL(theProfile.AvatarUrl, theProfile.PlayerId, _profilePic);
                _name.text = theProfile.DisplayName;
            }
        }
        else
        {
            PlayfabHelperFunctions.instance.GetOtherUserDataProfile(Startup._instance.MyPlayfabID, this);
            LoadAvatarURL(Startup._instance.avatarURL, Startup._instance.MyPlayfabID, _profilePic);
            _name.text = Startup._instance.displayName;
        }


    }
    public void SetAiStats()
    {



        for (int i = 0; i < statsData.Count; i++)
        {
            statsData[i].SetFromData(AchivmentController.instance, true);
        }

    }
    public void LoadAvatarURL(string aURL,string playfabid,Image img)
    {
    //    StartCoroutine(GetFBProfilePicture(aURL, img));


        if(aURL != null && aURL.Length>3)
        {
            ProfilePictureManager.instance.SetPicture(aURL, playfabid, img);
            img.enabled = true;
        }
        else
        {
            img.sprite = Startup._instance.EmptyProfilePicture;
        }

    



    }
    Sprite ProfilePictureSprite = null;
    //public static IEnumerator GetFBProfilePicture(string aURL, Image img)
    //{

    //    //string url = "https" + "://graph.facebook.com/10159330728290589/picture";
    //    WWW www = new WWW(aURL + "&access_token=GG|817150566351647|GXmlbSYVrHYJ1h7CJj7t9cGxwrE");
    //    yield return www;
    //    Texture2D profilePic = www.texture;

    //    img.sprite = Sprite.Create((Texture2D)profilePic, new Rect(0, 0, profilePic.height, profilePic.width), new Vector2());
    //    img.rectTransform.sizeDelta = new Vector2(88, 88);
    //    img.enabled = true;





    //}



    public void Init()
    {
        bool hasActiveGameAgainstPlayer = false;
        for(int i = 0; i < Startup._instance.openGamesList.Count;i++)
        {
            if(Startup._instance.openGamesList[i].player1_PlayfabId == theProfile.PlayerId || Startup._instance.openGamesList[i].player2_PlayfabId == theProfile.PlayerId)
            {
                hasActiveGameAgainstPlayer = true;
            }
        }


        if(hasActiveGameAgainstPlayer)
        {
            ChallengeButton.text = "Game in progress..";
        }
        else
        {
            ChallengeButton.text = "Challenge";
        }





        StatsData _statsData = Startup._instance.GetStatsData();
        int amountLost = 0;
        int amountWin = 0;
        for (int i = 0; i < _statsData.FinishedGames.Count; i++)
        {
            if (_statsData.FinishedGames[i].PlayfabID == theProfile.PlayerId)
            {
                if (_statsData.FinishedGames[i].Winner == Startup._instance.MyPlayfabID)
                    amountWin++;
                else
                    amountLost++;
            }
        }
        winT.text = amountWin.ToString();
        LoseT.text = amountLost.ToString();

        PlayfabHelperFunctions.instance.GetOtherUserDataProfile(theProfile.PlayerId,this);

    }
    public void SetData(Dictionary<string,UserDataRecord> profileData)
    {

        _level.text = HelperFunctions.XPtoLevel(profileData["XP"].Value).ToString();


        AchivmentController ac = new AchivmentController();
        ac.Init(profileData["Achivments"].Value);

        for (int i = 0; i< statsData.Count;i++)
        {
            statsData[i].SetFromData(ac,isAIInfo);
        }



        int completed = 0;
        for (int i = 0; i < ac.myAchivments.Count; i++)
        {
            if (ac.myAchivments[i].current >= ac.myAchivments[i].target)
                completed++;
        }

        amountCompletedText.text = completed + "/" + ac.myAchivments.Count;
        AchivmenSlider.fillAmount = (float)completed / (float)ac.myAchivments.Count;


        if(_thropies != null)
        _thropies.text = profileData["Ranking"].Value;

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChallengePlayer()
    {
        bool hasActiveGameAgainstPlayer = false;
        for (int i = 0; i < Startup._instance.openGamesList.Count; i++)
        {
            if (Startup._instance.openGamesList[i].player1_PlayfabId == theProfile.PlayerId || Startup._instance.openGamesList[i].player2_PlayfabId == theProfile.PlayerId)
            {
                hasActiveGameAgainstPlayer = true;
            }
        }


        if(hasActiveGameAgainstPlayer == false && ChallengeButton.text != "Game in progress..")
        {
            LoadingOverlay.instance.ShowLoadingFullscreen("Challenge in progress..");
            string newRoomName = string.Format("{0}-{1}", Startup._instance.displayName + "_" + Startup._instance.MyPlayfabID + "_", Random.Range(0, 1000000).ToString()) + Random.Range(0, 1000000).ToString();
            //PlayfabHelperFunctions.instance.SetPlayfabCreatedRoom(Startup._instance.MyPlayfabID, newRoomName);


            PlayfabHelperFunctions.instance.ChallengePlayer(Startup._instance.MyPlayfabID, theProfile.PlayerId, theProfile.DisplayName, newRoomName);
            PlayfabHelperFunctions.instance.AddGameToPlayerListCloudScript(theProfile.PlayerId, newRoomName);
            PlayfabHelperFunctions.instance.AddGameToPlayerListCloudScript(Startup._instance.MyPlayfabID, newRoomName);
            ChallengeButton.text = "Game in progress..";
            StartCoroutine(ChallengeProgress());

        }




    }





    public IEnumerator ChallengeProgress( )
    {
      
        yield return new WaitForSeconds(3);
  
        LoadingOverlay.instance.DoneLoading("Challenge in progress..");
        gameObject.SetActive(false);
        FriendsListWindow.instance.gameObject.SetActive(false);
        Startup._instance.Refresh();
        yield return new WaitForSeconds(2);




    }
    public void RemoveFriend()
    {
        PlayFabClientAPI.RemoveFriend(new RemoveFriendRequest
        {
            FriendPlayFabId = theProfile.PlayerId,

        }, result => RemovedSucess(result), FailureCallback);
    }
    public void RemovedSucess(RemoveFriendResult result)
    {
        FriendsListWindow.instance.UserInfoWindow.SetActive(false);

        foreach (Transform child in FriendsListWindow.instance._parent)
        {
            GameObject.Destroy(child.gameObject);
        }

        FriendsListWindow.instance.RequestLeaderboard();
    }
    public void FailureCallback(PlayFabError error)
    {
        Debug.Log("Failed");
    }

}
