using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum AchivmentTypeEnum
{
    WIN,
    BINGOS,
    SCORE,
    MORE_THAN_100p,
    MORE_THAN_200p,
    LOST,
    TIE,
    RESIGN,
    TIMEOUT,
    HGS,
    HRS,
    AGS,
    ARS,
    AMS,
    TOTALBINGO,
    AVREAGEBINGOGAME,
    TOTAL_MOVES,
    TOTAL_ROUNDS

};

[System.Serializable]
public class Achivment
{
    public Achivment(int aID,string aTitle, string aDescription, float aCurrent, int aTarget, int aReward, AchivmentTypeEnum aAchivmentType,bool aIsCalimed)
    {
        myID = aID;
        title = aTitle;
        description = aDescription;
        current = aCurrent;
        target = aTarget;
        reward = aReward;
        myAchivmentType = aAchivmentType;
        isClaimed = aIsCalimed;
    }

    public int myID;

    public string title;
    public string description;

    public float current;
    public int target;

    public int reward;

    public AchivmentTypeEnum myAchivmentType;

    public bool isClaimed;

}
[System.Serializable]
public class AchivmentList
{
    public AchivmentList(List<Achivment> aList, List<Achivment> aStats, List<Achivment> aAIStats)
    {
        myAchivemnts = aList;
        myStats = aStats;
        myAIStats = aAIStats;
    }
    public List<Achivment> myAchivemnts;
    public List<Achivment> myStats;
    public List<Achivment> myAIStats;
}
[System.Serializable]
public class AchivmentController
{

    public List<Achivment> myAchivments = new List<Achivment>();
    public List<Achivment> myStatistics = new List<Achivment>();
    public List<Achivment> myAIStatistics = new List<Achivment>();
    public static AchivmentController instance;

    public string playfabid = "";

    // Start is called before the first frame update
    public void Init(string achivmentsData)
    {
        if(instance == null)
            instance = this;

        AchivmentList list = JsonUtility.FromJson<AchivmentList>(achivmentsData);
        myAchivments = list.myAchivemnts;
        myStatistics = list.myStats;
        myAIStatistics = list.myAIStats;

    }

    
    public float GetStats(AchivmentTypeEnum aStats, bool getAIstats = false)
    {
        if(getAIstats)
        {
            if(myAIStatistics.Count==0)
            {
                SetDefaultAIStats();
            }
            for (int i = 0; i < myAIStatistics.Count; i++)
            {
                if (myAIStatistics[i].myAchivmentType == aStats)
                    return myAIStatistics[i].current;
            }
        }

        if(playfabid == Startup.instance.MyPlayfabID || playfabid == "")
        {
            if (AchivmentTypeEnum.WIN == aStats)
            {
                int totalWins = 0;
                for (int i = 0; i < Startup.instance.myOldGameList.Count; i++)
                {
                    if (Startup.instance.myOldGameList[i].player2_displayName != "AI" &&
                    Startup.instance.myOldGameList[i].player2_displayName.Length > 0 &&
                    Startup.instance.myOldGameList[i].GetWinner() == Startup.instance.MyPlayfabID)
                    {
                        //string abPlayer = Startup.instance.myOldGameList[i].GetHasAbboned();
                        //if (abPlayer == "" || abPlayer != Startup.instance.MyPlayfabID)
                        //{
                        totalWins++;

                        //}
                    }
                }
                return totalWins;
            }
            if (AchivmentTypeEnum.LOST == aStats)
            {
                int totalLosts = 0;
                for (int i = 0; i < Startup.instance.myOldGameList.Count; i++)
                {
                    if (Startup.instance.myOldGameList[i].player2_displayName != "AI" &&
                    Startup.instance.myOldGameList[i].player2_displayName.Length > 0 &&
                    Startup.instance.myOldGameList[i].GetWinner() != Startup.instance.MyPlayfabID &&
                    Startup.instance.myOldGameList[i].GetWinner().Length > 0)
                    {
                        totalLosts++;
                    }
                }
                return totalLosts;
            }
            if (AchivmentTypeEnum.TIMEOUT == aStats)
            {
                int totalLosts = 0;
                for (int i = 0; i < Startup.instance.myOldGameList.Count; i++)
                {
                    if (Startup.instance.myOldGameList[i].player2_displayName != "AI" &&
                    Startup.instance.myOldGameList[i].player2_displayName.Length > 0 &&
                    Startup.instance.myOldGameList[i].GetWinner() != Startup.instance.MyPlayfabID &&
                    Startup.instance.myOldGameList[i].GetWinner().Length > 0 &&
                    Startup.instance.myOldGameList[i].WasTimout())
                    {
                        totalLosts++;
                    }
                }
                return totalLosts;
            }
            if (AchivmentTypeEnum.RESIGN == aStats)
            {
                int totalAban = 0;
                for (int i = 0; i < Startup.instance.myOldGameList.Count; i++)
                {
                    if (Startup.instance.myOldGameList[i].player2_displayName != "AI" &&
                    Startup.instance.myOldGameList[i].player2_displayName.Length > 0 &&

                    (Startup.instance.myOldGameList[i].player1_abandon == "1" &&
                    Startup.instance.myOldGameList[i].player1_PlayfabId == Startup.instance.MyPlayfabID) ||
                   (Startup.instance.myOldGameList[i].player2_abandon == "1" &&
                    Startup.instance.myOldGameList[i].player2_PlayfabId == Startup.instance.MyPlayfabID))
                    {
                        totalAban++;
                    }
                }
                return totalAban;
            }
        }
      
        for (int i = 0; i< myStatistics.Count;i++)
        {
            if (myStatistics[i].myAchivmentType == aStats)
                return myStatistics[i].current;
        }
        return 0;
    }
    public void UpdatePlayfab()
    {
        string data = JsonUtility.ToJson(new AchivmentList(myAchivments,myStatistics,myAIStatistics));
        PlayfabHelperFunctions.instance.ChangeValueFor("Achivments", data);
    }
    public string GetDefault()
    {
        List<Achivment> emptyAchivemnts = new List<Achivment>();
        emptyAchivemnts.Add(new Achivment(0, "5 wins", "Win 5 games", 0, 5, 100, AchivmentTypeEnum.WIN,false));
        emptyAchivemnts.Add(new Achivment(0, "10 wins", "Win 10 games", 0, 10, 100, AchivmentTypeEnum.WIN, false));
        emptyAchivemnts.Add(new Achivment(0, "25 wins", "Win 25 games", 0, 25, 100, AchivmentTypeEnum.WIN, false));
        emptyAchivemnts.Add(new Achivment(0, "50 wins", "Win 50 games", 0, 50, 100, AchivmentTypeEnum.WIN, false));
        emptyAchivemnts.Add(new Achivment(0, "100 wins", "Win 100 games", 0, 100, 100, AchivmentTypeEnum.WIN, false));

        emptyAchivemnts.Add(new Achivment(0, "5 bingos", "Get 5 bingos", 0, 5, 100, AchivmentTypeEnum.BINGOS, false));
        emptyAchivemnts.Add(new Achivment(0, "10 bingos", "Get 10 bingos", 0, 10, 100, AchivmentTypeEnum.BINGOS, false));
        emptyAchivemnts.Add(new Achivment(0, "25 bingos", "Get 25 bingos", 0, 25, 100, AchivmentTypeEnum.BINGOS, false));
        emptyAchivemnts.Add(new Achivment(0, "50 bingos", "Get 50 bingos", 0, 50, 100, AchivmentTypeEnum.BINGOS, false));
        emptyAchivemnts.Add(new Achivment(0, "100 bingos", "Get 100 bingos", 0, 100, 100, AchivmentTypeEnum.BINGOS, false));

        emptyAchivemnts.Add(new Achivment(0, "Score", "Score 1000 points", 0, 1000, 100, AchivmentTypeEnum.SCORE, false));
        emptyAchivemnts.Add(new Achivment(0, "Score", "Score 10000 points", 0, 10000, 100, AchivmentTypeEnum.SCORE, false));
        emptyAchivemnts.Add(new Achivment(0, "Score", "Score 100000 points", 0, 100000, 100, AchivmentTypeEnum.SCORE, false));

        emptyAchivemnts.Add(new Achivment(0, "Top scorer 1", "Score more than 100 points in a turn", 0, 5, 100, AchivmentTypeEnum.MORE_THAN_100p, false));
        emptyAchivemnts.Add(new Achivment(0, "Top scorer 2", "Score more than 100 points in a turn", 0, 10, 100, AchivmentTypeEnum.MORE_THAN_100p, false));
        emptyAchivemnts.Add(new Achivment(0, "Top scorer 3", "Score more than 100 points in a turn", 0, 25, 100, AchivmentTypeEnum.MORE_THAN_100p, false));
        emptyAchivemnts.Add(new Achivment(0, "Top scorer 4", "Score more than 100 points in a turn", 0, 50, 100, AchivmentTypeEnum.MORE_THAN_100p, false));

        emptyAchivemnts.Add(new Achivment(0, "Master scorer 1", "Score more than 200 points in a turn", 0, 5, 100, AchivmentTypeEnum.MORE_THAN_200p, false));
        emptyAchivemnts.Add(new Achivment(0, "Master scorer 2", "Score more than 200 points in a turn", 0, 10, 100, AchivmentTypeEnum.MORE_THAN_200p, false));
        emptyAchivemnts.Add(new Achivment(0, "Master scorer 3", "Score more than 200 points in a turn", 0, 25, 100, AchivmentTypeEnum.MORE_THAN_200p, false));
        emptyAchivemnts.Add(new Achivment(0, "Master scorer 4", "Score more than 200 points in a turn", 0, 50, 100, AchivmentTypeEnum.MORE_THAN_200p, false));

        List<Achivment> emptyStats = new List<Achivment>();

        emptyStats.Add(new Achivment(0, "Wins", "wins", 0, -1, 0, AchivmentTypeEnum.WIN, false)); // Done
        emptyStats.Add(new Achivment(0, "Lost", "wins", 0, -1, 0, AchivmentTypeEnum.LOST, false)); // Done
        emptyStats.Add(new Achivment(0, "Ties", "wins", 0, -1, 0, AchivmentTypeEnum.TIE, false));
        emptyStats.Add(new Achivment(0, "Resigns", "wins", 0, -1, 0, AchivmentTypeEnum.RESIGN, false));
        emptyStats.Add(new Achivment(0, "TimeOut", "wins", 0, -1, 0, AchivmentTypeEnum.TIMEOUT, false));
        emptyStats.Add(new Achivment(0, "HighestGameScore", "wins", 0, -1, 0, AchivmentTypeEnum.HGS, false));// Done
        emptyStats.Add(new Achivment(0, "HighestRoundScore", "wins", 0, -1, 0, AchivmentTypeEnum.HRS, false)); // Done
        emptyStats.Add(new Achivment(0, "AvreageGameScore", "wins", 0, -1, 0, AchivmentTypeEnum.AGS, false)); // Done
        emptyStats.Add(new Achivment(0, "AvreageRoundScore", "wins", 0, -1, 0, AchivmentTypeEnum.ARS, false)); // Done
        emptyStats.Add(new Achivment(0, "AvreageMoveScore", "wins", 0, -1, 0, AchivmentTypeEnum.AMS, false)); // Done
        emptyStats.Add(new Achivment(0, "TotalNumberOfBingos", "wins", 0, -1, 0, AchivmentTypeEnum.TOTALBINGO, false)); // Done
        emptyStats.Add(new Achivment(0, "AvreagBingosGame", "wins", 0, -1, 0, AchivmentTypeEnum.AVREAGEBINGOGAME, false)); // Done
        emptyStats.Add(new Achivment(0, "TotalMoves", "wins", 0, -1, 0, AchivmentTypeEnum.TOTAL_MOVES, false)); // Done
        emptyStats.Add(new Achivment(0, "TotalRounds", "wins", 0, -1, 0, AchivmentTypeEnum.TOTAL_ROUNDS, false)); // Done
        emptyStats.Add(new Achivment(0, "TotalScore", "wins", 0, -1, 0, AchivmentTypeEnum.SCORE, false)); // Done



        string data = JsonUtility.ToJson(new AchivmentList(emptyAchivemnts, emptyStats, emptyStats));
        return data;
    }
    public void SetDefaultAIStats()
    {
        List<Achivment> emptyStats = new List<Achivment>();

        emptyStats.Add(new Achivment(0, "Wins", "wins", 0, -1, 0, AchivmentTypeEnum.WIN, false)); // Done
        emptyStats.Add(new Achivment(0, "Lost", "wins", 0, -1, 0, AchivmentTypeEnum.LOST, false)); // Done
        emptyStats.Add(new Achivment(0, "Ties", "wins", 0, -1, 0, AchivmentTypeEnum.TIE, false));
        emptyStats.Add(new Achivment(0, "Resigns", "wins", 0, -1, 0, AchivmentTypeEnum.RESIGN, false));
        emptyStats.Add(new Achivment(0, "TimeOut", "wins", 0, -1, 0, AchivmentTypeEnum.TIMEOUT, false));
        emptyStats.Add(new Achivment(0, "HighestGameScore", "wins", 0, -1, 0, AchivmentTypeEnum.HGS, false));// Done
        emptyStats.Add(new Achivment(0, "HighestRoundScore", "wins", 0, -1, 0, AchivmentTypeEnum.HRS, false)); // Done
        emptyStats.Add(new Achivment(0, "AvreageGameScore", "wins", 0, -1, 0, AchivmentTypeEnum.AGS, false)); // Done
        emptyStats.Add(new Achivment(0, "AvreageRoundScore", "wins", 0, -1, 0, AchivmentTypeEnum.ARS, false)); // Done
        emptyStats.Add(new Achivment(0, "AvreageMoveScore", "wins", 0, -1, 0, AchivmentTypeEnum.AMS, false)); // Done
        emptyStats.Add(new Achivment(0, "TotalNumberOfBingos", "wins", 0, -1, 0, AchivmentTypeEnum.TOTALBINGO, false)); // Done
        emptyStats.Add(new Achivment(0, "AvreagBingosGame", "wins", 0, -1, 0, AchivmentTypeEnum.AVREAGEBINGOGAME, false)); // Done
        emptyStats.Add(new Achivment(0, "TotalMoves", "wins", 0, -1, 0, AchivmentTypeEnum.TOTAL_MOVES, false)); // Done
        emptyStats.Add(new Achivment(0, "TotalRounds", "wins", 0, -1, 0, AchivmentTypeEnum.TOTAL_ROUNDS, false)); // Done
        emptyStats.Add(new Achivment(0, "TotalScore", "wins", 0, -1, 0, AchivmentTypeEnum.SCORE, false)); // Done

        myAIStatistics = emptyStats;
    }




    // Game functions
    public void WonGame( int score)
    {
        for(int i = 0; i < myAchivments.Count;i++)
        {
            if ( myAchivments[i].myAchivmentType == AchivmentTypeEnum.WIN)
            {
                myAchivments[i].current++;
            }
        }

        float totalGames = 0;
        float totalScore = 0;
        for (int i = 0; i < myStatistics.Count; i++)
        {
            if (myStatistics[i].myAchivmentType == AchivmentTypeEnum.WIN)
            {
                myStatistics[i].current++;
                totalGames += myStatistics[i].current;
            }
            if (myStatistics[i].myAchivmentType == AchivmentTypeEnum.HGS)
            {
                if(score > myStatistics[i].current )
                {
                    myStatistics[i].current = score;
                }
            }
            if (myStatistics[i].myAchivmentType == AchivmentTypeEnum.TIE)
            {
                totalGames += myStatistics[i].current;
            }
            if (myStatistics[i].myAchivmentType == AchivmentTypeEnum.LOST)
            {
                totalGames += myStatistics[i].current;
            }
            if (myStatistics[i].myAchivmentType == AchivmentTypeEnum.SCORE)
            {
                totalScore = myStatistics[i].current;
            }
        }
        for (int i = 0; i < myStatistics.Count; i++)
        {
            if (myStatistics[i].myAchivmentType == AchivmentTypeEnum.AGS)
            {
                myStatistics[i].current = (int) (totalScore / totalGames);
            }
        }



            UpdatePlayfab();
    }
    // Game functions
    public void LostGame( int score)
    {
        for (int i = 0; i < myAchivments.Count; i++)
        {
            if (myAchivments[i].myAchivmentType == AchivmentTypeEnum.LOST)
            {
                myAchivments[i].current++;
            }
        }

        float totalGames = 0;
        float totalScore = 0;
        for (int i = 0; i <myStatistics.Count; i++)
        {
            if (myStatistics[i].myAchivmentType == AchivmentTypeEnum.WIN)
            {
                totalGames += myStatistics[i].current;
            }
            if (myStatistics[i].myAchivmentType == AchivmentTypeEnum.TIE)
            {
                totalGames += myStatistics[i].current;
            }
            if (myStatistics[i].myAchivmentType == AchivmentTypeEnum.LOST)
            {
                totalGames += myStatistics[i].current;
            }

            if (myStatistics[i].myAchivmentType == AchivmentTypeEnum.LOST)
            {
                myStatistics[i].current++;
            }
            if (myStatistics[i].myAchivmentType == AchivmentTypeEnum.SCORE)
            {
                totalScore = myStatistics[i].current;
            }
            if (myStatistics[i].myAchivmentType == AchivmentTypeEnum.HGS)
            {
                if (score > myStatistics[i].current)
                {
                    myStatistics[i].current = score;
                }
            }
        }

        for (int i = 0; i < myStatistics.Count; i++)
        {
            if (myStatistics[i].myAchivmentType == AchivmentTypeEnum.AGS)
            {
                myStatistics[i].current = (int)(totalScore / totalGames);
            }
        }
        UpdatePlayfab();
    }
    public void Bingo()
    {
        for (int i = 0; i < myAchivments.Count; i++)
        {
            if (myAchivments[i].myAchivmentType == AchivmentTypeEnum.BINGOS)
            {
                myAchivments[i].current++;
            }
        }
        float totalGames = 0;
        float totalBingos = 0;
        for (int i = 0; i < myStatistics.Count; i++)
        {
            if (myStatistics[i].myAchivmentType == AchivmentTypeEnum.WIN)
            {
                totalGames += myStatistics[i].current;
            }
            if (myStatistics[i].myAchivmentType == AchivmentTypeEnum.TIE)
            {
                totalGames += myStatistics[i].current;
            }
            if (myStatistics[i].myAchivmentType == AchivmentTypeEnum.LOST)
            {
                totalGames += myStatistics[i].current;
            }
            if (myStatistics[i].myAchivmentType == AchivmentTypeEnum.TOTALBINGO)
            {
                myStatistics[i].current++;
                totalBingos = myStatistics[i].current;
            }
        }
        for (int i = 0; i < myStatistics.Count; i++)
        {
            if (myStatistics[i].myAchivmentType == AchivmentTypeEnum.AVREAGEBINGOGAME)
            {
                if (totalBingos == 0 || totalGames == 0)
                    myStatistics[i].current = totalBingos;
                else
                myStatistics[i].current = totalBingos / totalGames;
            }
        }
        UpdatePlayfab();

    }
    public void Scored(int aScore, int amountOfTiles, bool isAI = false)
    {
        List<Achivment> workingList = myStatistics;
        if (isAI)
            workingList = myAIStatistics;


        for (int i = 0; i < myAchivments.Count; i++)
        {
            if (myAchivments[i].myAchivmentType == AchivmentTypeEnum.SCORE)
            {
                myAchivments[i].current+= aScore;
            }
            if (myAchivments[i].myAchivmentType == AchivmentTypeEnum.MORE_THAN_100p)
            {
                if(aScore > 100)
                    myAchivments[i].current += 1;
            }
            if (myAchivments[i].myAchivmentType == AchivmentTypeEnum.MORE_THAN_200p)
            {
                if (aScore > 200)
                    myAchivments[i].current += 1;
            }
        }
        for (int i = 0; i < workingList.Count; i++)
        {
            if (workingList[i].myAchivmentType == AchivmentTypeEnum.SCORE)
            {
                workingList[i].current+= aScore;
            }
            if (workingList[i].myAchivmentType == AchivmentTypeEnum.TOTAL_ROUNDS)
            {
                workingList[i].current++;
            }
            if (workingList[i].myAchivmentType == AchivmentTypeEnum.TOTAL_MOVES)
            {
                workingList[i].current+= amountOfTiles;
            }
            if (workingList[i].myAchivmentType == AchivmentTypeEnum.HRS)
            {
                if(aScore > workingList[i].current)
                {
                    workingList[i].current = aScore;
                }
            }
        }




        float totalGames = 0;
        float totalScore = 0;
        float totalRounds = 0;
        float totalMovess = 0;
        for (int i = 0; i < workingList.Count; i++)
        {
            if (workingList[i].myAchivmentType == AchivmentTypeEnum.WIN)
            {
                totalGames += workingList[i].current;
            }
            if (workingList[i].myAchivmentType == AchivmentTypeEnum.TIE)
            {
                totalGames += workingList[i].current;
            }
            if (workingList[i].myAchivmentType == AchivmentTypeEnum.LOST)
            {
                totalGames += workingList[i].current;
            }
            if (workingList[i].myAchivmentType == AchivmentTypeEnum.SCORE)
            {
                totalScore = workingList[i].current;
            }
            if (workingList[i].myAchivmentType == AchivmentTypeEnum.TOTAL_ROUNDS)
            {
                totalRounds = workingList[i].current;
            }
            if (workingList[i].myAchivmentType == AchivmentTypeEnum.TOTAL_MOVES)
            {
                totalMovess = workingList[i].current;
            }
        }
        for (int i = 0; i < workingList.Count; i++)
        {
            if (workingList[i].myAchivmentType == AchivmentTypeEnum.AMS)
            {
                workingList[i].current = (totalScore/totalMovess);
            }
            if (workingList[i].myAchivmentType == AchivmentTypeEnum.ARS)
            {
                workingList[i].current = (totalScore/totalRounds);
            }
        }


        if (isAI == true)
            myAIStatistics = workingList;
        else
            myStatistics = workingList;


        UpdatePlayfab();

    }

    public void CheckWithLocal()
    {
        int totalWins = 0;
        for (int i = 0; i < Startup.instance.myOldGameList.Count; i++)
        {
            if (Startup.instance.myOldGameList[i].player2_displayName != "AI" &&
            Startup.instance.myOldGameList[i].player2_displayName.Length > 0 &&
            Startup.instance.myOldGameList[i].GetWinner() == Startup.instance.MyPlayfabID)
            {
                totalWins++;
            }
        }
        int totalLosts = 0;
        for (int i = 0; i < Startup.instance.myOldGameList.Count; i++)
        {
            if (Startup.instance.myOldGameList[i].player2_displayName != "AI" &&
            Startup.instance.myOldGameList[i].player2_displayName.Length > 0 &&
            Startup.instance.myOldGameList[i].GetWinner() != Startup.instance.MyPlayfabID &&
            Startup.instance.myOldGameList[i].GetWinner().Length > 0)
            {
                totalLosts++;
            }
        }
        int totalAban = 0;
        for (int i = 0; i < Startup.instance.myOldGameList.Count; i++)
        {
            if (Startup.instance.myOldGameList[i].player2_displayName != "AI" &&
            Startup.instance.myOldGameList[i].player2_displayName.Length > 0 &&

            (Startup.instance.myOldGameList[i].player1_abandon == "1" &&
            Startup.instance.myOldGameList[i].player1_PlayfabId == Startup.instance.MyPlayfabID) ||
           (Startup.instance.myOldGameList[i].player2_abandon == "1" &&
            Startup.instance.myOldGameList[i].player2_PlayfabId == Startup.instance.MyPlayfabID))
            {
                totalAban++;
            }
        }
        int totalTime = 0;
        for (int i = 0; i < Startup.instance.myOldGameList.Count; i++)
        {
            if (Startup.instance.myOldGameList[i].player2_displayName != "AI" &&
            Startup.instance.myOldGameList[i].player2_displayName.Length > 0 &&
            Startup.instance.myOldGameList[i].GetWinner() != Startup.instance.MyPlayfabID &&
            Startup.instance.myOldGameList[i].GetWinner().Length > 0 &&
            Startup.instance.myOldGameList[i].WasTimout())
            {
                totalTime++;
            }
        }

        bool shouldUpdate = false; ;
        for (int i = 0; i < myStatistics.Count; i++)
        {
            if (myStatistics[i].myAchivmentType == AchivmentTypeEnum.WIN)
            {
                if (totalWins != myStatistics[i].current)
                {
                    shouldUpdate = true;
                    myStatistics[i].current = totalWins;

                }
            }
            if (myStatistics[i].myAchivmentType == AchivmentTypeEnum.LOST)
            {
                if (totalLosts != myStatistics[i].current)
                {
                    shouldUpdate = true;
                    myStatistics[i].current = totalLosts;
                }

            }
            if (myStatistics[i].myAchivmentType == AchivmentTypeEnum.TIMEOUT)
            {
                if (totalTime != myStatistics[i].current)
                {
                    shouldUpdate = true;
                    myStatistics[i].current = totalTime;
                }

            }
            if (myStatistics[i].myAchivmentType == AchivmentTypeEnum.RESIGN)
            {
                if (totalAban != myStatistics[i].current)
                {
                    shouldUpdate = true;
                    myStatistics[i].current =totalAban;
                }

            }  
        }
        if(shouldUpdate)
            UpdatePlayfab();


    }
}
