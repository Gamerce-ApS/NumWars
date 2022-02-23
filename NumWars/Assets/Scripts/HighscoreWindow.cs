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
          
            MaxResultsCount = 20
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
            go.transform.GetChild(4).GetComponent<Text>().text = result.Leaderboard[i].Position.ToString()+".";
            go.transform.GetChild(1).GetComponent<Text>().text = result.Leaderboard[i].StatValue.ToString();
        }

    }

}
