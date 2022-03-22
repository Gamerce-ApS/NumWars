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
};

[System.Serializable]
public class Achivment
{
    public Achivment(int aID,string aTitle, string aDescription, int aCurrent, int aTarget, int aReward, AchivmentTypeEnum aAchivmentType,bool aIsCalimed)
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

    public int current;
    public int target;

    public int reward;

    public AchivmentTypeEnum myAchivmentType;

    public bool isClaimed;

}
[System.Serializable]
public class AchivmentList
{
    public AchivmentList(List<Achivment> aList)
    {
        myAchivemnts = aList;
    }
    public List<Achivment> myAchivemnts;
}
[System.Serializable]
public class AchivmentController
{

    public List<Achivment> myAchivments = new List<Achivment>();
    public static AchivmentController instance;

    // Start is called before the first frame update
    public void Init(string achivmentsData)
    {

        instance = this;

        myAchivments = JsonUtility.FromJson<AchivmentList>(achivmentsData).myAchivemnts;


    }
    public void UpdatePlayfab()
    {
        string data = JsonUtility.ToJson(new AchivmentList(myAchivments));
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

        string data = JsonUtility.ToJson(new AchivmentList(emptyAchivemnts));
        return data;
    }





    // Game functions
    public void WonGame()
    {
        for(int i = 0; i < myAchivments.Count;i++)
        {
            if ( myAchivments[i].myAchivmentType == AchivmentTypeEnum.WIN)
            {
                myAchivments[i].current++;
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
        UpdatePlayfab();

    }
    public void Scored(int aScore)
    {
        for(int i = 0; i < myAchivments.Count; i++)
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
        UpdatePlayfab();

    }
}
