using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreWindow : MonoBehaviour
{
    public GameObject templateItem;
    public GameObject templateMore;
    public Transform _parent;

    public GameObject me;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void OnEnable()
    {
        StartNumber = 0;
        RequestLeaderboard();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    GetLeaderboardResult rs = new GetLeaderboardResult();
    PlayerLeaderboardEntry myInfo=null;
    //Get the players with the top 10 high scores in the game
    public void RequestLeaderboard()
    {

        AWSBackend.instance.AWSClientAPI("phpBackend/GetLeadboard.php", "",
            result =>
          {
              Debug.Log(result);


              AWSBackend.LeadboardList list = JsonUtility.FromJson<AWSBackend.LeadboardList>("{\"myLeadboardList\":" + result + "}");
              rs.Leaderboard = new List<PlayerLeaderboardEntry>();
              for (int i = 0; i < list.myLeadboardList.Length; i++)
              {
                  //Debug.Log(list.myLeadboardList[i].DisplayName + " : " + list.myLeadboardList[i].PlayerID);


                  PlayerLeaderboardEntry finfo = new PlayerLeaderboardEntry();
                  finfo.Profile = new PlayerProfileModel();
                  finfo.Profile.AvatarUrl = list.myLeadboardList[i].avatarURL;
                  finfo.DisplayName = list.myLeadboardList[i].DisplayName;
                  finfo.Profile.PlayerId = list.myLeadboardList[i].PlayerID;
                  finfo.StatValue = int.Parse(list.myLeadboardList[i].Value);
                  //finfo.Position = int.Parse(list.myLeadboardList[i].rank);
                  finfo.Profile.Tags = new List<TagModel>();
                  TagModel tM = new TagModel();
                  tM.TagValue = (list.myLeadboardList[i].XP);
                  finfo.Profile.Tags.Add(tM);
                  rs.Leaderboard.Add(finfo);

                  //if (list.myLeadboardList[i].PlayerID == Startup.instance.MyPlayfabID)
                  //    myInfo = finfo;

              }
              DisplayLeaderboard(rs);
          },
           error =>
           {
               PlayFabError pfE = new PlayFabError();
               FailureCallback(pfE);
           }
          );







        //PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
        //{ 
        //    //PlayFabId=Startup._instance.MyPlayfabID,
        //    StatisticName = "Highscore",
        //    StartPosition=StartNumber,
        //    MaxResultsCount = 20,
        //     ProfileConstraints = new PlayerProfileViewConstraints()
        //     {
        //         ShowDisplayName = true,
        //         ShowAvatarUrl = true,
        //         ShowStatistics = true
        //     }

        //}, result => DisplayLeaderboard(result), FailureCallback);



        //PlayFabClientAPI.GetLeaderboardAroundPlayer(new GetLeaderboardAroundPlayerRequest
        //{ 
        //    StatisticName =  "Highscore" ,
        //    MaxResultsCount = 1,

        //}, result => DisplayMyStats(result), FailureCallback);

        
    }
    public void DisplayMyStats(PlayerLeaderboardEntry result)
    {
        _parent.GetComponent<ScrollListBasedOnItems>().Reset();
      


            me.transform.GetChild(4).GetComponent<Text>().text = result.DisplayName;
            me.transform.GetChild(5).GetComponent<Text>().text = (result.Position + 1).ToString() + ".";
            me.transform.GetChild(3).GetComponent<Text>().text = result.StatValue.ToString();



            string avatarURL = Startup.instance.avatarURL;


            Image img = me.transform.GetChild(2).GetChild(0).GetComponent<Image>();

            if (avatarURL != null && avatarURL.Length > 0)
            {
                ProfilePictureManager.instance.SetPicture(avatarURL, Startup.instance.MyPlayfabID, img);
                //StartCoroutine(SetPicture(avatarURL, img));

            }

 
     
                    me.transform.GetChild(6).GetComponent<Text>().text = HelperFunctions.XPtoLevel(Startup._instance.myData["XP"].Value).ToString();
  



        
    }

    public int StartNumber = 0;
    public void ClickLoadMore()
    {
        StartNumber += 20;
        DisplayLeaderboard(rs);
    }
    private void FailureCallback(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your API call. Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }

    public void DisplayLeaderboard(GetLeaderboardResult result)
    {
        foreach (Transform child in _parent)
        {
            GameObject.Destroy(child.gameObject);
        }


        List<PlayerLeaderboardEntry> SortedList = result.Leaderboard.OrderByDescending(o => o.StatValue).ToList();


            for (int i = 0; i < SortedList.Count; i++)
            {
                if(SortedList[i].Profile.PlayerId == Startup.instance.MyPlayfabID)
                {
                    SortedList[i].Position = i;
                    DisplayMyStats(SortedList[i]);

                }
            }

  


        for (int i = StartNumber; i < StartNumber + 20;i++)
        {
            if (i > SortedList.Count-1)
                continue;

            GameObject go =  GameObject.Instantiate(templateItem, _parent);

            go.transform.GetChild(4).GetComponent<Text>().text = SortedList[i].DisplayName;
            go.transform.GetChild(5).GetComponent<Text>().text = (i+1).ToString()+".";
            go.transform.GetChild(3).GetComponent<Text>().text = SortedList[i].StatValue.ToString();
            go.transform.GetChild(6).GetComponent<Text>().text = "";


            string avatarURL = "";

            if(SortedList[i].Profile.AvatarUrl != null)
            avatarURL = SortedList[i].Profile.AvatarUrl;

            Image img = go.transform.GetChild(2).GetChild(0).GetComponent<Image>();

            if (avatarURL.Length>0)
            {
                ProfilePictureManager.instance.SetPicture(avatarURL, SortedList[i].Profile.PlayerId, img);
                //StartCoroutine(SetPicture(avatarURL, img));

            }

            //bool hasFoundxp = false;
            ////for(int j = 0; j < result.Leaderboard[i].Profile.Statistics.Count;j++)
            ////{
            ////    if (result.Leaderboard[i].Profile.Statistics[j].Name == "Experience")
            ////    {
            ////        go.transform.GetChild(6).GetComponent<Text>().text = HelperFunctions.XPtoLevel(result.Leaderboard[i].Profile.Statistics[1].Value.ToString()).ToString();
            ////        hasFoundxp = true;
            ////    }
            ////}

                
            //if(hasFoundxp == false)
            //{
            //        go.transform.GetChild(6).gameObject.SetActive(false);
            //}

          go.transform.GetChild(6).gameObject.SetActive(true);
            if(SortedList[i].Profile.Tags[0].TagValue.Length>0)
          go.transform.GetChild(6).GetComponent<Text>().text = HelperFunctions.XPtoLevel(SortedList[i].Profile.Tags[0].TagValue.ToString()).ToString();




        }

        GameObject.Instantiate(templateMore, _parent);
        

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
