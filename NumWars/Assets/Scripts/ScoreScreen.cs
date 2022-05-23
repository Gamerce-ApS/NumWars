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

public class ListContainer
{
    public List<string> Data = new List<string>();
    public string timestamp = "";
}

public class ScoreScreen : MonoBehaviour
{
    public static ScoreScreen instance;

    public GameObject ScorePrefab;
    public GameObject StarEffectPrefab;
    public GameObject bg;

    public GameObject TotalScore;
    public GameObject bingo;

    

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
    public void ShowScoreLastPlay(bool isFromStart, BoardData _bd)
    {
        List<string> moveHistory = _bd.History;
        List<FakeTileData> lastMoves = new List<FakeTileData>();

        int myBackednTurn = _bd.GetPlayerTurn(GameManager.instance.CurrentTurn);

        if(moveHistory != null)
        if(moveHistory.Count>0)
        if(moveHistory[moveHistory.Count-1]== "#SWAP#")
        {
            AlertText.instance.ShowAlert("Player swapped!",0.5f);
            return;
        }



        if (moveHistory == null)
            return;
        for (int i = moveHistory.Count-1; i>=0 ;i--)
        {
            if (moveHistory[i] == "#SWAP#" || moveHistory[i] == "#EMPTY#" || moveHistory[i].Contains("#TILESONHAND"))
            {
                break;
            }
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
        GameManager.instance.AddScore(GameManager.instance.thePlayers[1], -totalScore, lastMoves.Count, false,true);

        lastMoves.Sort(HelperFunctions.SortByScoreInverse);


        for (int i = 0; i < lastMoves.Count; i++)
        {
            StartCoroutine(ShowScoreAfterTime(0.4f + 1.75f +0.25f + i * 0.6f, lastMoves[i]));
        }
        StartCoroutine(SummarizeAfterTime(0.4f + 0.5f + 0.5f + lastMoves.Count * 0.6f, lastMoves, GameManager.instance.thePlayers[1], totalScore));

        


    }
    IEnumerator ShowScoreAfterTime(float aTime, FakeTileData aTile)
    {
        
        yield return new WaitForSeconds(0.01f );
        GameManager.instance.MakeLastPlayedTilesColored();
        Transform st = Board.instance.BoardTiles[(int)(aTile.Position.x + aTile.Position.y * 14)].transform.GetChild(0).transform;
        Vector3 targetPos = st.transform.position;
        st.transform.position += new Vector3(0, 10, 0);
        st.transform.DOMove(targetPos, 0.75f+ (float)Random.Range(50, 100) / 100f).SetEase(Ease.InOutQuart);

        yield return new WaitForSeconds(aTime);
        Startup._instance.PlaySoundEffect(4);


        GameObject go = GameObject.Instantiate(ScorePrefab, bg.transform);
        go.transform.position = Board.instance.BoardTiles[(int)(aTile.Position.x + aTile.Position.y * 14)].transform.position;
        go.transform.GetChild(0).Find("Text").GetComponent<Text>().text = aTile.ScoreValue.ToString();


        createdPoints.Add(go);
    }
    IEnumerator SummarizeAfterTime(float aTime, List<FakeTileData> score, Player thePlayer,int totalScore)
    {
        if(score.Count==0)
        {
            if(Startup._instance.GameToLoad != null && Startup._instance.GameToLoad.BoardTiles != null)
            if (int.Parse(Startup._instance.GameToLoad.EmptyTurns) >= 4)
            {
                    if(Startup._instance.GameToLoad != null && Startup._instance.GameToLoad.BoardTiles != null)
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
        yield return new WaitForSeconds(0.15f);
        Startup._instance.PlaySoundEffect(2);
        //Startup._instance.PlaySoundEffect(4);
        yield return new WaitForSeconds(0.15f);

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


        if(createdPoints.Count == 6)
        {
            bingo.SetActive(false);
            bingo.SetActive(true);
            totalScore += 50;
            AchivmentController.instance.Bingo();

        }
        
        GameManager.instance.AddScore(thePlayer, totalScore , createdPoints.Count, true, true);
        //for (int i = 0; i < score.Count; i++)
        //{
        //    score[i].Flip();
        //}

        for (int i = 0; i < createdPoints.Count; i++)
        {
            Destroy(createdPoints[i]);
        }
        createdPoints.Clear();
        Board.instance.LoadLastUsedTiles(PlayerBoard.instance.myPlayer.myTiles);

        yield return new WaitForSeconds(1.0f);
        bg.SetActive(false);



        if(Startup._instance.GameToLoad != null && Startup._instance.GameToLoad.EmptyTurns!= null)
        if (int.Parse(Startup._instance.GameToLoad.EmptyTurns) >= 4)
        {
            GameFinishedScreen.instance.Show(Startup._instance.GameToLoad);
        }

        GameManager.instance.MakeLastPlayedTilesColored();


    }



    public List<string> test = new List<string>();

    // Scooring in when yor turn
    public void ShowScore(List<Tile> score,Player aPlayer)
    {
        bg.SetActive(true);
        bg.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        bg.GetComponent<Image>().DOFade(119f / 255f, 1.0f).SetEase(Ease.InOutQuart);

        score.Sort(HelperFunctions.SortByScoreInverse);


        ListContainer l = new ListContainer();
        l.timestamp = System.DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

        l.Data = aPlayer.GetMyTiles();
        string test2 = JsonUtility.ToJson(l);

        if (GameManager.instance.thePlayers[1].isAI)
        {
            if (Board.instance.History == null)
                Board.instance.History = new List<string>();


            string playfabID = Startup._instance.MyPlayfabID;
            if (aPlayer.isAI)
                playfabID = "AI";
            Board.instance.History.Add("#TILESONHAND_" + playfabID + "#" + test2);

        }
        else
        {
            Startup._instance.GameToLoad.History.Add("#TILESONHAND_" + Startup._instance.MyPlayfabID + "#" + test2);

        }



        for (int i = 0; i < score.Count;i++)
        {
            StartCoroutine(ShowScoreAfterTime (0.4f+0.25f+ i * 0.6f, score[i] ));
        }
        StartCoroutine(SummarizeAfterTime(0.4f + 0.5f + score.Count * 0.6f, score, aPlayer));


        Startup._instance.PlaySoundEffect(3);

    }

    IEnumerator ShowScoreAfterTime(float aTime, Tile aTile, bool shouldAddToHistory = true)
    {
        yield return new WaitForSeconds(aTime);
        if(0.25f != aTime)
            Startup._instance.PlaySoundEffect(4);

        GameObject go = GameObject.Instantiate(ScorePrefab, bg.transform);
        go.transform.position = aTile.transform.position;
        go.transform.GetChild(0).Find("Text").GetComponent<Text>().text = aTile.GetValue();
        if (Startup._instance.isFake)
            go.transform.localScale *= 1.5f;

        if (shouldAddToHistory)
        {
            if (GameManager.instance.thePlayers[1].isAI)
            {
                if (Board.instance.History == null)
                    Board.instance.History = new List<string>();

                Board.instance.History.Add(aTile.GetBoardPosition() + "#" + aTile.textLabel.text + "#" + aTile.GetValue() + "#" + GameManager.instance.CurrentTurn.ToString());
            }
            else
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
        
        if (createdPoints.Count>1)
        {
            Vector2 avgPos = GetAvgPos();
            for (int i = 0; i < createdPoints.Count; i++)
            {
                createdPoints[i].GetComponent<RectTransform>().DOMove(avgPos, 0.5f).SetEase(Ease.InOutQuart);
            }
        }
        Startup._instance.PlaySoundEffect(2);
        yield return new WaitForSeconds(0.15f);
        
        //Startup._instance.PlaySoundEffect(4);
        yield return new WaitForSeconds(0.15f);

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


        if (createdPoints.Count == 6)
        {
            bingo.SetActive(false);
            bingo.SetActive(true);
            totalScore += 50;
            if(AchivmentController.instance != null)
            AchivmentController.instance.Bingo();
            yield return new WaitForSeconds(0.4f);
            createdPoints[createdPoints.Count-1].transform.GetChild(0).Find("Text").GetComponent<Text>().text = totalScore.ToString();
            yield return new WaitForSeconds(0.4f);
        }

        for (int i = 0; i < createdPoints.Count; i++)
        {
            
            createdPoints[i].GetComponent<RectTransform>().DOMove(thePlayer.scoreObject.transform.position, 1).SetEase(Ease.InOutQuart);

            createdPoints[i].transform.GetChild(0).GetComponent<Image>().DOFade(0, 1).SetEase(Ease.InOutQuart); ;
        }
        bg.GetComponent<Image>().DOFade(0, 1.5f).SetEase(Ease.InOutQuart); ;

        //Startup._instance.PlaySoundEffect(2);

        yield return new WaitForSeconds(0.7f);




        GameManager.instance.AddScore(thePlayer, totalScore, createdPoints.Count);
        for (int i = 0; i < score.Count; i++)
        {
            score[i].Flip();
        }

        for (int i = 0; i < createdPoints.Count; i++)
        {
            Destroy(createdPoints[i]);
        }
        createdPoints.Clear();

        Board.instance.LoadLastUsedTiles(PlayerBoard.instance.myPlayer.myTiles);
        yield return new WaitForSeconds(1.0f);




        bg.SetActive(false);
        thePlayer.AddNewPlayerTiles();
        PlayerBoard.instance.RefreshLayout();

        bool isEmptyTurn = false;

        if (score.Count <= 0)
            isEmptyTurn = true;

        GameManager.instance.NextTurn(isEmptyTurn);

        yield return new WaitForSeconds(0.03f);
        GameManager.instance.MakeLastPlayedTilesColored();





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
