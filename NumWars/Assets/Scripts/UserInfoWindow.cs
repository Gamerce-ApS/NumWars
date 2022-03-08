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
