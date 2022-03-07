using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatsData
{
    [SerializeField]
    public List<StatsGame> FinishedGames = new List<StatsGame>();


    public string GetJson()
    {
        return JsonUtility.ToJson(this);
    }
    public StatsData()
    {


    }
    public StatsData(string aJson)
    {
        StatsData bd = JsonUtility.FromJson<StatsData>(aJson);

        FinishedGames = bd.FinishedGames;

    }

}


[System.Serializable]
public class StatsGame
{
  public string PlayfabID;
  public string DisplayName;
  public string Winner;

    public StatsGame(string aPlayfabID,string aDisplayName, string aWinner)
    {
        PlayfabID = aPlayfabID;
        DisplayName = aDisplayName;
        Winner = aWinner;


    }

}