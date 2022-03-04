using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class UserInfoWindow : MonoBehaviour
{
public PlayerProfileModel theProfile;
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
