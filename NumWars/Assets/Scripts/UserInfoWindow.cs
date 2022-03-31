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
            statsData[i].SetFromData(ac);
        }



        int completed = 0;
        for (int i = 0; i < ac.myAchivments.Count; i++)
        {
            if (ac.myAchivments[i].current >= ac.myAchivments[i].target)
                completed++;
        }

        amountCompletedText.text = completed + "/" + ac.myAchivments.Count;
        AchivmenSlider.fillAmount = (float)completed / (float)ac.myAchivments.Count;

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
        FriendsListWindow.instance.RequestLeaderboard();
    }
    public void FailureCallback(PlayFabError error)
    {
        Debug.Log("Failed");
    }

}
