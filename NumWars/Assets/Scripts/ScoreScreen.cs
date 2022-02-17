using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FakeTileData
{
    public Vector2 Position;
    public int Number;
    public int ScoreValue;
    public int Player;
};
public class ScoreScreen : MonoBehaviour
{
    public static ScoreScreen instance;

    public GameObject ScorePrefab;
    public GameObject StarEffectPrefab;
    public GameObject bg;

    public GameObject TotalScore;

    

    public List<GameObject> createdPoints = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // Replay functions
    public void ShowScoreLastPlay(bool isFromStart)
    {
        List<string> moveHistory = Startup._instance.GameToLoad.History;
        List<FakeTileData> lastMoves = new List<FakeTileData>();

        int myBackednTurn = Startup._instance.GameToLoad.GetPlayerTurn(GameManager.instance.CurrentTurn);


        if(moveHistory[moveHistory.Count-1]== "#SWAP#")
        {
            AlertText.instance.ShowAlert("Player swapped!",0.5f);
            return;
        }




        for (int i = moveHistory.Count-1; i>=0 ;i--)
        {
            string[] moveInfo = moveHistory[i].Split('#');
            FakeTileData ftd = new FakeTileData();
            ftd.Position = StringToVector2(moveInfo[0]);
            ftd.Number = int.Parse(moveInfo[1]);
            ftd.ScoreValue = int.Parse(moveInfo[2]);
            ftd.Player = int.Parse(moveInfo[3]);


            if(myBackednTurn != ftd.Player)
            {
                lastMoves.Add(ftd);
            }
            else
            {
                break;
            }
        }
        Debug.Log("Replay " + lastMoves.Count + " turns");


 

        int totalScore = 0;
        for (int i = 0; i < lastMoves.Count; i++)
        {
            totalScore += lastMoves[i].ScoreValue;
        }

        // if we come from start the score needs to be removed as it has not been updated
        if(isFromStart)
        GameManager.instance.AddScore(GameManager.instance.thePlayers[1], -totalScore,false);
  
        for (int i = 0; i < lastMoves.Count; i++)
        {
            StartCoroutine(ShowScoreAfterTime(1.75f+0.25f + i * 0.6f, lastMoves[i]));
        }
        StartCoroutine(SummarizeAfterTime(0.5f + 0.5f + lastMoves.Count * 0.6f, lastMoves, GameManager.instance.thePlayers[1], totalScore));

    }
    IEnumerator ShowScoreAfterTime(float aTime, FakeTileData aTile)
    {
        yield return new WaitForSeconds(0.01f );

        Transform st = Board.instance.BoardTiles[(int)(aTile.Position.x + aTile.Position.y * 14)].transform.GetChild(0).transform;
        Vector3 targetPos = st.transform.position;
        st.transform.position += new Vector3(0, 10, 0);
        st.transform.DOMove(targetPos, 0.75f+ (float)Random.Range(50, 100) / 100f).SetEase(Ease.InOutQuart);

        yield return new WaitForSeconds(aTime);



        GameObject go = GameObject.Instantiate(ScorePrefab, bg.transform);
        go.transform.position = Board.instance.BoardTiles[(int)(aTile.Position.x + aTile.Position.y * 14)].transform.position;
        go.transform.GetChild(0).Find("Text").GetComponent<Text>().text = aTile.ScoreValue.ToString();
        createdPoints.Add(go);
    }
    IEnumerator SummarizeAfterTime(float aTime, List<FakeTileData> score, Player thePlayer,int totalScore)
    {
        if(score.Count==0)
        {
            if (int.Parse(Startup._instance.GameToLoad.EmptyTurns) >= 4)
            {
                GameFinishedScreen.instance.Show(Startup._instance.GameToLoad);
            }
            yield break;
        }


        yield return new WaitForSeconds(1.5f);
        bg.SetActive(true);
        bg.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        bg.GetComponent<Image>().DOFade(119f / 255f, 1.0f).SetEase(Ease.InOutQuart);

        yield return new WaitForSeconds(aTime);
        if (createdPoints.Count > 1)
        {
            Vector2 avgPos = GetAvgPos();
            for (int i = 0; i < createdPoints.Count; i++)
            {
                createdPoints[i].GetComponent<RectTransform>().DOMove(avgPos, 0.5f).SetEase(Ease.InOutQuart);
            }
        }
        yield return new WaitForSeconds(0.3f);
     
        for (int i = 0; i < createdPoints.Count; i++)
        {
            createdPoints[i].transform.GetChild(0).Find("Text").GetComponent<Text>().text = totalScore.ToString();
            if (createdPoints.Count > 1)
                createdPoints[i].GetComponent<RectTransform>().DOScale(createdPoints[i].transform.localScale * 1.3f, 0.1f).SetEase(Ease.InOutQuart);
        }
        yield return new WaitForSeconds(.95f);
        for (int i = 0; i < createdPoints.Count; i++)
        {

            createdPoints[i].GetComponent<RectTransform>().DOMove(thePlayer.scoreObject.transform.position, 1).SetEase(Ease.InOutQuart);

            createdPoints[i].transform.GetChild(0).GetComponent<Image>().DOFade(0, 1).SetEase(Ease.InOutQuart); ;
        }
        bg.GetComponent<Image>().DOFade(0, 1.5f).SetEase(Ease.InOutQuart); ;


        yield return new WaitForSeconds(0.7f);

        GameManager.instance.AddScore(thePlayer, totalScore);
        //for (int i = 0; i < score.Count; i++)
        //{
        //    score[i].Flip();
        //}

        for (int i = 0; i < createdPoints.Count; i++)
        {
            Destroy(createdPoints[i]);
        }
        createdPoints.Clear();
        yield return new WaitForSeconds(1.0f);
        bg.SetActive(false);



        
        if (int.Parse(Startup._instance.GameToLoad.EmptyTurns) >= 4)
        {
            GameFinishedScreen.instance.Show(Startup._instance.GameToLoad);
        }
    }




    // Scooring in when yor turn
    public void ShowScore(List<Tile> score,Player aPlayer)
    {
        bg.SetActive(true);
        bg.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        bg.GetComponent<Image>().DOFade(119f / 255f, 1.0f).SetEase(Ease.InOutQuart);

        for (int i = 0; i < score.Count;i++)
        {
            StartCoroutine(ShowScoreAfterTime (0.25f+ i * 0.6f, score[i] ));
        }
        StartCoroutine(SummarizeAfterTime(0.5f + score.Count * 0.6f, score, aPlayer));




    }

    IEnumerator ShowScoreAfterTime(float aTime, Tile aTile, bool shouldAddToHistory = true)
    {
        yield return new WaitForSeconds(aTime);
        GameObject go = GameObject.Instantiate(ScorePrefab, bg.transform);
        go.transform.position = aTile.transform.position;
        go.transform.GetChild(0).Find("Text").GetComponent<Text>().text = aTile.GetValue();

        if(shouldAddToHistory)
        {
            if (!GameManager.instance.thePlayers[1].isAI)
            {
                Startup._instance.GameToLoad.History.Add(aTile.GetBoardPosition() + "#" + aTile.textLabel.text + "#" + aTile.GetValue() + "#" + Startup._instance.GameToLoad.GetPlayerTurn(GameManager.instance.CurrentTurn).ToString());
            }
        }
        createdPoints.Add(go);
    }
    Vector2 GetAvgPos()
    {
        Vector3 avgPos = Vector3.zero;
        for (int i = 0; i < createdPoints.Count; i++)
        {
            avgPos += createdPoints[i].GetComponent<RectTransform>().position;
        }
        return avgPos / createdPoints.Count;
    }
    IEnumerator SummarizeAfterTime(float aTime, List<Tile> score,Player thePlayer)
    {
        yield return new WaitForSeconds(aTime);
        if(createdPoints.Count>1)
        {
            Vector2 avgPos = GetAvgPos();
            for (int i = 0; i < createdPoints.Count; i++)
            {
                createdPoints[i].GetComponent<RectTransform>().DOMove(avgPos, 0.5f).SetEase(Ease.InOutQuart);
            }
        }
        yield return new WaitForSeconds(0.3f);
        int totalScore = 0;
        for (int i = 0; i < score.Count; i++)
        {
            totalScore += int.Parse(score[i].GetValue());
        }
        for (int i = 0; i < createdPoints.Count; i++)
        {
            createdPoints[i].transform.GetChild(0).Find("Text").GetComponent<Text>().text = totalScore.ToString();
            if (createdPoints.Count > 1)
                createdPoints[i].GetComponent<RectTransform>().DOScale(createdPoints[i].transform.localScale*1.3f, 0.1f).SetEase(Ease.InOutQuart);
        }
        yield return new WaitForSeconds(.95f);
        for (int i = 0; i < createdPoints.Count; i++)
        {
            
            createdPoints[i].GetComponent<RectTransform>().DOMove(thePlayer.scoreObject.transform.position, 1).SetEase(Ease.InOutQuart);

            createdPoints[i].transform.GetChild(0).GetComponent<Image>().DOFade(0, 1).SetEase(Ease.InOutQuart); ;
        }
        bg.GetComponent<Image>().DOFade(0, 1.5f).SetEase(Ease.InOutQuart); ;


        yield return new WaitForSeconds(0.7f);

        GameManager.instance.AddScore(thePlayer, totalScore);
        for (int i = 0; i < score.Count; i++)
        {
            score[i].Flip();
        }

        for (int i = 0; i < createdPoints.Count; i++)
        {
            Destroy(createdPoints[i]);
        }
        createdPoints.Clear();


        yield return new WaitForSeconds(1.0f);
   
        bg.SetActive(false);
        thePlayer.AddNewPlayerTiles();
        PlayerBoard.instance.RefreshLayout();

        bool isEmptyTurn = false;

        if (score.Count <= 0)
            isEmptyTurn = true;

        GameManager.instance.NextTurn(isEmptyTurn);

    }
    public static Vector2 StringToVector2(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector2 result = new Vector2(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]));

        return result;
    }

}
