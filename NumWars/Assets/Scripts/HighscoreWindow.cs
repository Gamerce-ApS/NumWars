using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreWindow : MonoBehaviour
{
    public GameObject templateItem;
    public Transform _parent;
    // Start is called before the first frame update
    void Start()
    {
        
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
        PlayFabClientAPI.GetLeaderboardAroundPlayer(new GetLeaderboardAroundPlayerRequest
        {
            PlayFabId=Startup._instance.MyPlayfabID,
            StatisticName = "Highscore",
          
            MaxResultsCount = 20,
             ProfileConstraints = new PlayerProfileViewConstraints()
             {
                 ShowDisplayName = true,
                 ShowAvatarUrl = true,
                 ShowStatistics = true
             }

        }, result => DisplayLeaderboard(result), FailureCallback);
    }


    private void FailureCallback(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your API call. Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }

    public void DisplayLeaderboard(GetLeaderboardAroundPlayerResult result)
    {
        foreach (Transform child in _parent)
        {
            GameObject.Destroy(child.gameObject);
        }


        for (int i = 0; i < result.Leaderboard.Count;i++)
        {
            GameObject go =  GameObject.Instantiate(templateItem, _parent);

            go.transform.GetChild(2).GetComponent<Text>().text = result.Leaderboard[i].DisplayName;
            go.transform.GetChild(4).GetComponent<Text>().text = (result.Leaderboard[i].Position+1).ToString()+".";
            go.transform.GetChild(1).GetComponent<Text>().text = result.Leaderboard[i].StatValue.ToString();



            string avatarURL = "";

            if(result.Leaderboard[i].Profile.AvatarUrl != null)
            avatarURL = result.Leaderboard[i].Profile.AvatarUrl;

            Image img = go.transform.GetChild(0).GetChild(0).GetComponent<Image>();

            if (avatarURL.Length>0)
            {
                ProfilePictureManager.instance.SetPicture(avatarURL, result.Leaderboard[i].Profile.PlayerId, img);
                //StartCoroutine(SetPicture(avatarURL, img));

            }

            bool hasFoundxp = false;
            for(int j = 0; j < result.Leaderboard[i].Profile.Statistics.Count;j++)
            {
                if (result.Leaderboard[i].Profile.Statistics[j].Name == "Experience")
                {
                    go.transform.GetChild(5).GetChild(0).GetComponent<Text>().text = HelperFunctions.XPtoLevel(result.Leaderboard[i].Profile.Statistics[1].Value.ToString()).ToString();
                    hasFoundxp = true;
                }
            }

                
            if(hasFoundxp == false)
            {
                    go.transform.GetChild(5).gameObject.SetActive(false);
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

}
