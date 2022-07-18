using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeAgainWindow : MonoBehaviour
{
    public static ChallengeAgainWindow instance;

    public Image bg;
    public GameObject window;

    public Text challenger;
    public Image fbIcon;
    public Text AmountText;
    public Button del;
    public Button aprove;


    List<GameListItem> currentChallenges = new List<GameListItem>();
    // Start is called before the first frame update
    void Start()
    {
        instance = this;



       // PlayerPrefs.SetString("ChallengePlayerAgain", "67EC8D5C96914D0");


    }
    //public void CheckPlayAgain()
    //{

    //    if (PlayerPrefs.HasKey("ChallengePlayerAgain"))
    //    {
    //        string chID = PlayerPrefs.GetString("ChallengePlayerAgain");
    //        bool hasActiveGame = false;
    //        for (int i = 0; i< Startup.instance.openGamesList.Count;i++)
    //        {
    //            if( Startup.instance.openGamesList[i].player1_PlayfabId == chID || Startup.instance.openGamesList[i].player2_PlayfabId == chID)
    //            {
    //                hasActiveGame = true;
    //            }
    //        }

    //        if(hasActiveGame == false)
    //        UpdatevWindow(chID);



    //        PlayerPrefs.DeleteKey("ChallengePlayerAgain");
    //    }
    //}
    // Update is called once per frame
    void Update()
    {
        
    }

    public string currentCHid = "";
    public void UpdatevWindow(string playfabID)
    {
        currentCHid = playfabID;





        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
        {
            PlayFabId = playfabID,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,
                ShowAvatarUrl = true
            }
        }, result => {

            LoadAvatarURL(result.PlayerProfile.AvatarUrl, playfabID);
            challenger.text = result.PlayerProfile.DisplayName;

            del.interactable = true;
            aprove.interactable = true;

            bg.gameObject.SetActive(true);
            window.SetActive(true);
        }, (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });


    }

    public void ClickAccept()
    {
        LoadingOverlay.instance.ShowLoadingFullscreen("Challenge in progress..");
        string newRoomName = string.Format("{0}-{1}", Startup._instance.displayName + "_" + Startup._instance.MyPlayfabID + "_", Random.Range(0, 1000000).ToString()) + Random.Range(0, 1000000).ToString();
        //PlayfabHelperFunctions.instance.SetPlayfabCreatedRoom(Startup._instance.MyPlayfabID, newRoomName);


        PlayfabHelperFunctions.instance.ChallengePlayer(Startup._instance.MyPlayfabID, currentCHid, challenger.text, newRoomName);
        PlayfabHelperFunctions.instance.AddGameToPlayerListCloudScript(currentCHid, newRoomName);
        PlayfabHelperFunctions.instance.AddGameToPlayerListCloudScript(Startup._instance.MyPlayfabID, newRoomName);

        StartCoroutine(ChallengeProgress());

        del.interactable = false;
        aprove.interactable = false;


    }
    public void ClickReject()
    {

        bg.gameObject.SetActive(false);
        window.SetActive(false);

    }
    public IEnumerator ChallengeProgress()
    {

        yield return new WaitForSeconds(5);

        LoadingOverlay.instance.DoneLoading("Challenge in progress..");
        bg.gameObject.SetActive(false);
        window.SetActive(false);
        Startup._instance.Refresh();





    }

    public void LoadAvatarURL(string aURL,string playfabID)
    {

        //StartCoroutine(GetFBProfilePicture(aURL, this));
        if (aURL != null)
        {
            ProfilePictureManager.instance.SetPicture(aURL, playfabID, fbIcon);
            fbIcon.enabled = true;
        }

    }

}
