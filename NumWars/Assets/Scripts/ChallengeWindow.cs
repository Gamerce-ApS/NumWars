using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeWindow : MonoBehaviour
{
    public static ChallengeWindow instance;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddChallengeGame(GameListItem gameListItem)
    {
        if (Exist(gameListItem))
            return;


        string approved = PlayerPrefs.GetString("ApprovedChallenges", "");
        if (approved.Contains(gameListItem.bd.RoomName))
            return;




        currentChallenges.Add(gameListItem);




        UpdateWindow();

    }
    public bool Exist(GameListItem gameListItem)
    {
        bool exist = false;
        for(int i = 0; i < currentChallenges.Count;i++)
        {
            if (currentChallenges[i].bd.RoomName == gameListItem.bd.RoomName)
                exist = true;
        }

        return exist;
    }
    public void UpdateWindow()
    {
        if(currentChallenges.Count<=0)
        {
            bg.gameObject.SetActive(false);
            window.SetActive(false);
            return;
        }

        if (currentChallenges == null || currentChallenges[0].bd == null || Startup._instance == null)
        {
            bg.gameObject.SetActive(false);
            window.SetActive(false);
            return;
        }

        del.interactable = true;
        aprove.interactable = true;

        bg.gameObject.SetActive(true);
        window.SetActive(true);
        challenger.text = currentChallenges[0].bd.GetOtherPlayer();

        

        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
        {

            PlayFabId = currentChallenges[0].bd.GetOtherPlayerPlayfab(),
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,
                ShowAvatarUrl = true
            }
        }, result => {

            if (result == null)
                return;

            if (currentChallenges == null || currentChallenges.Count <= 0 || Startup._instance == null)
            {
                bg.gameObject.SetActive(false);
                window.SetActive(false);
                return;
            }

            LoadAvatarURL(result.PlayerProfile.AvatarUrl, currentChallenges[0].bd.GetOtherPlayerPlayfab());


        }, (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });

        AmountText.text = "Challenge (1/" + currentChallenges.Count+")";

    }

    public void ClickAccept()
    {
        aprove.interactable = false;
        string approved = PlayerPrefs.GetString("ApprovedChallenges", "");
        approved += "," + currentChallenges[0].bd.RoomName;
        PlayerPrefs.SetString("ApprovedChallenges",approved);

        Startup._instance.Refresh(0.1f);
        currentChallenges.RemoveAt(0);
        UpdateWindow();


    }
    public void ClickReject()
    {
        del.interactable = false;
        PlayfabHelperFunctions.instance.DeleteGame(currentChallenges[0].bd.RoomName,DoneCallbackDelete);

    }
    public void DoneCallbackDelete()
    {
        Startup._instance.Refresh(0.1f);
        currentChallenges.RemoveAt(0);
        UpdateWindow();
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

    //public static IEnumerator GetFBProfilePicture(string aURL, ChallengeWindow chWin)
    //{

    //    //string url = "https" + "://graph.facebook.com/10159330728290589/picture";
    //    if (aURL != null)
    //    {

            


    //        WWW www = new WWW(aURL + "&access_token=GG|817150566351647|GXmlbSYVrHYJ1h7CJj7t9cGxwrE");
    //        yield return www;
    //        Texture2D profilePic = www.texture;


    //        chWin.fbIcon.sprite = Sprite.Create((Texture2D)profilePic, new Rect(0, 0, profilePic.height, profilePic.width), new Vector2());
    //        chWin.fbIcon.rectTransform.sizeDelta = new Vector2(88, 88);
    //        chWin.fbIcon.enabled = true;
    //    }

    //}
}
