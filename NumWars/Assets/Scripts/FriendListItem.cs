using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class FriendListItem : MonoBehaviour
{
PlayerProfileModel myProfile;
    string displayN;
    string thropies;
    Image profileP;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ClickOnItem()
    {

        FriendsListWindow.instance.ClickOnItem(myProfile, displayN, thropies, profileP.sprite);




    }
    public void Init(PlayerProfileModel profile,string adisplayN,string athropies,Image aprofileP)
    {
        myProfile = profile;
         displayN= adisplayN;
         thropies= athropies;
         profileP= aprofileP;

    }


    public void FailureCallback(PlayFabError error)
    {
        Debug.Log("Failed");
    }
}
