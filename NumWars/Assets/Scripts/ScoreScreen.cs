using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Globalization;
//using AppodealAds.Unity.Api;

public class FakeTileData
{
    public Vector2 Position;
    public int Number;
    public int ScoreValue;
    public int Player;
    public int SpecialTypeTile;
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
        if(Input.GetMouseButtonUp(0))
            Speed = 0.5f;
    }

    public void GetLastMoveForUser(BoardData _bd, int aTurn)
    {


    }
    public void SetLastScoreNotYournTurn(BoardData _bd)
    {
        List<string> moveHistory = _bd.History;
        List<FakeTileData> lastMoves = new List<FakeTileData>();

        int myBackednTurn = _bd.GetPlayerTurn(GameManager.instance.CurrentTurn);

        if (moveHistory == null)
            return;


        int LastMoveIndex = 0;
        for (int i = moveHistory.Count - 1; i >= 0; i--)
        {
            if (moveHistory[i].Contains( "#SWAP#") || moveHistory[i] == "#EMPTY#" || moveHistory[i].Contains("#TILESONHAND"))
            {
                break;
            }
            string[] moveInfo = moveHistory[i].Split('#');
            FakeTileData ftd = new FakeTileData();
            ftd.Position = StringToVector2(moveInfo[0]);
            ftd.Number = int.Parse(moveInfo[1]);
            ftd.ScoreValue = int.Parse(moveInfo[2]);
            ftd.Player = int.Parse(moveInfo[3]);
            ftd.SpecialTypeTile = 0;
            if (moveInfo.Length > 4)
                ftd.SpecialTypeTile = int.Parse(moveInfo[4]);



            if (myBackednTurn != ftd.Player)
            {
                lastMoves.Add(ftd);
            }
            else
            {
                break;
            }
            LastMoveIndex = i;
        }


        int totalScore1 = 0;
        for (int i = 0; i < lastMoves.Count; i++)
        {
            totalScore1 += lastMoves[i].ScoreValue;
        }
        int addition1 = 0;
        if (lastMoves.Count == 6)
            addition1 = 50;
        GameManager.instance.thePlayers[0].LastScore = totalScore1 + addition1;


        lastMoves.Clear();
        if (moveHistory != null)
            if (moveHistory.Count > 0)
                if (moveHistory[moveHistory.Count - 1].Contains( "#SWAP#"))
                {

                    LastMoveIndex = moveHistory.Count - 0;

                }
        if (moveHistory != null)
            if (moveHistory.Count > 0)
                if (moveHistory[moveHistory.Count - 1] == "#EMPTY#")
                {

                    LastMoveIndex = moveHistory.Count - 1;

                }

        for (int i = LastMoveIndex - 2; i >= 0; i--)
        {
            if (moveHistory[i].Contains( "#SWAP#") || moveHistory[i] == "#EMPTY#" || moveHistory[i].Contains("#TILESONHAND"))
            {
                break;
            }
            string[] moveInfo = moveHistory[i].Split('#');
            FakeTileData ftd = new FakeTileData();
            ftd.Position = StringToVector2(moveInfo[0]);
            ftd.Number = int.Parse(moveInfo[1]);
            ftd.ScoreValue = int.Parse(moveInfo[2]);
            ftd.Player = int.Parse(moveInfo[3]);
            ftd.SpecialTypeTile = 0;
            if (moveInfo.Length > 4)
                ftd.SpecialTypeTile = int.Parse(moveInfo[4]);



            if (myBackednTurn == ftd.Player)
            {
                lastMoves.Add(ftd);
            }
            else
            {
                break;
            }
        }

        int totalScore = 0;
        for (int i = 0; i < lastMoves.Count; i++)
        {
            totalScore += lastMoves[i].ScoreValue;
        }
        int addition = 0;
        if (lastMoves.Count == 6)
            addition = 50;
        GameManager.instance.thePlayers[1].LastScore = totalScore + addition;
        GameManager.instance.UpdateUI();












    }
    public void SetYourLastScore(BoardData _bd, int lastMoveIndex)
    {
        List<string> moveHistory = _bd.History;
        List<FakeTileData> lastMoves = new List<FakeTileData>();

        int myBackednTurn = _bd.GetPlayerTurn(GameManager.instance.CurrentTurn);

        //if (moveHistory != null)
        //    if (moveHistory.Count > 0)
        //        if (moveHistory[moveHistory.Count - 1] == "#SWAP#")
        //        {
        //            AlertText.instance.ShowAlert("Player swapped!", 0.5f);
        //            return;
        //        }
        //if (moveHistory != null)
        //    if (moveHistory.Count > 0)
        //        if (moveHistory[moveHistory.Count - 1] == "#EMPTY#")
        //        {
        //            AlertText.instance.ShowAlert("Empty turn!", 0.5f);
        //            return;
        //        }


        if (moveHistory == null)
            return;


        for (int i = lastMoveIndex-1; i >= 0; i--)
        {
            if (moveHistory[i].Contains( "#SWAP#") || moveHistory[i] == "#EMPTY#" || moveHistory[i].Contains("#TILESONHAND"))
            {
                break;
            }
            string[] moveInfo = moveHistory[i].Split('#');
            FakeTileData ftd = new FakeTileData();
            ftd.Position = StringToVector2(moveInfo[0]);
            ftd.Number = int.Parse(moveInfo[1]);
            ftd.ScoreValue = int.Parse(moveInfo[2]);
            ftd.Player = int.Parse(moveInfo[3]);
            ftd.SpecialTypeTile = 0;
            if (moveInfo.Length > 4)
                ftd.SpecialTypeTile = int.Parse(moveInfo[4]);



            if (myBackednTurn == ftd.Player)
            {
                lastMoves.Add(ftd);
            }
            else
            {
                break;
            }
        }

        int totalScore = 0;
        for (int i = 0; i < lastMoves.Count; i++)
        {
            totalScore += lastMoves[i].ScoreValue;
        }
        int addition = 0;
        if (lastMoves.Count == 6)
            addition = 50;
        GameManager.instance.thePlayers[0].LastScore = totalScore+ addition;
        GameManager.instance.UpdateUI();

    }
    // Replay functions
    public void ShowScoreLastPlay(bool isFromStart, BoardData _bd)
    {

        if (GameManager.instance.GameEndedOverlay.activeSelf == true)
            return;

            List<string> moveHistory = _bd.History;
        List<FakeTileData> lastMoves = new List<FakeTileData>();

        int myBackednTurn = _bd.GetPlayerTurn(GameManager.instance.CurrentTurn);

        if(moveHistory != null)
        if(moveHistory.Count>0)
        if(moveHistory[moveHistory.Count-1].Contains( "#SWAP#"))
        {
                    
            AlertText.instance.ShowAlert(GameManager.instance.thePlayers[1].Username+ " swapped!",0.8f);
            SetYourLastScore(_bd, moveHistory.Count - 1);
            return;
        }
        if (moveHistory != null)
        if (moveHistory.Count > 0)
        if (moveHistory[moveHistory.Count - 1] == "#EMPTY#")
        {
                    
            AlertText.instance.ShowAlert(_bd.GetOtherPlayer()+" placed no tiles!", 0.5f);
            SetYourLastScore(_bd, moveHistory.Count - 2);
            return;
        }


        if (moveHistory == null)
            return;
        int LastMoveIndex = 0;
        for (int i = moveHistory.Count-1; i>=0 ;i--)
        {
            if (moveHistory[i].Contains( "#SWAP#") || moveHistory[i] == "#EMPTY#" || moveHistory[i].Contains("#TILESONHAND"))
            {
                break;
            }
            string[] moveInfo = moveHistory[i].Split('#');
            FakeTileData ftd = new FakeTileData();
            ftd.Position = StringToVector2(moveInfo[0]);
            ftd.Number = int.Parse(moveInfo[1]);
            ftd.ScoreValue = int.Parse(moveInfo[2]);
            ftd.Player = int.Parse(moveInfo[3]);
            ftd.SpecialTypeTile = 0;
            if (moveInfo.Length>4)
            ftd.SpecialTypeTile = int.Parse(moveInfo[4]);



            if (myBackednTurn != ftd.Player)
            {
                lastMoves.Add(ftd);
            }
            else
            {
                break;
            }
            LastMoveIndex = i;
        }
        Debug.Log("Replay " + lastMoves.Count + " turns");

        SetYourLastScore(_bd, LastMoveIndex-1);


        int totalScore = 0;
        for (int i = 0; i < lastMoves.Count; i++)
        {
            totalScore += lastMoves[i].ScoreValue;
        }

        // if we come from start the score needs to be removed as it has not been updated
        if(isFromStart)
        {
            int extra = 0;
            if (lastMoves.Count == 6)
                extra = 50;

            GameManager.instance.AddScore(GameManager.instance.thePlayers[1], -totalScore- extra, lastMoves.Count, false, true);

        }

        lastMoves.Sort(HelperFunctions.SortByScoreInverse);

        Speed = 1;
        for (int i = 0; i < lastMoves.Count; i++)
        {
            StartCoroutine(ShowScoreAfterTime(0.4f + 1.75f +0.25f + i * 0.6f, lastMoves[i]));
        }
        StartCoroutine(SummarizeAfterTime(0.4f + 0.5f + 0.5f + lastMoves.Count * 0.6f, lastMoves, GameManager.instance.thePlayers[1], totalScore));

        


    }
    public GameObject CreateTempTile(int aType,Transform st)
    {
        if (aType != 0)
        {
            GameObject goTest = (GameObject)GameObject.Instantiate(ResourceManager.instance.TilePrefabList[aType], st.parent);
            goTest.transform.SetAsFirstSibling();
            goTest.transform.position = st.position;
            goTest.SetActive(true);
            return goTest;
        }

        return null;
    }
    IEnumerator ShowScoreAfterTime(float aTime, FakeTileData aTile)
    {



        yield return new WaitForSeconds(0.01f );
        GameManager.instance.MakeLastPlayedTilesColored();
        Transform st = Board.instance.BoardTiles[(int)(aTile.Position.x + aTile.Position.y * 14)].transform.GetChild(0).transform;

        GameObject goTest = CreateTempTile(aTile.SpecialTypeTile, st);
      


        Vector3 targetPos = st.transform.position;
        st.transform.position += new Vector3(0, 10, 0);
        st.transform.DOMove(targetPos, 0.75f + (float)Random.Range(50, 100) / 100f).SetEase(Ease.InOutQuart).OnComplete(() => { if (goTest != null) Destroy(goTest); });







        for (float timer = aTime; timer >= 0; timer -= Time.deltaTime)
        {
            if (Speed != 1)
            {

                timer = 0;
            }
            else
            {
                yield return null;
            }

        }

       // yield return new WaitForSeconds(aTime);
        Startup._instance.PlaySoundEffect(4);


        GameObject go = GameObject.Instantiate(ScorePrefab, bg.transform);
        go.transform.position = Board.instance.BoardTiles[(int)(aTile.Position.x + aTile.Position.y * 14)].transform.position;
        go.transform.GetChild(0).Find("Text").GetComponent<Text>().text = aTile.ScoreValue.ToString();


        createdPoints.Add(go);
    }
    IEnumerator SummarizeAfterTime(float aTime, List<FakeTileData> score, Player thePlayer,int totalScore)
    {

     


        if (score.Count==0)
        {
            if(Startup._instance.GameToLoad != null && Startup._instance.GameToLoad.BoardTiles != null)
            if (int.Parse(Startup._instance.GameToLoad.EmptyTurns) >= 4)
            {
                    if(Startup._instance.GameToLoad != null && Startup._instance.GameToLoad.BoardTiles != null)
                        GameFinishedScreen.instance.Show(Startup._instance.GameToLoad);
            }
            yield break;
        }





        yield return new WaitForSeconds(0.6f);
        bg.SetActive(true);
        bg.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        bg.GetComponent<Image>().DOFade(119f / 255f, 1.0f).SetEase(Ease.InOutQuart);

        for (float timer = aTime; timer >= 0; timer -= Time.deltaTime)
        {
            if (Speed != 1)
            {

                timer = 0;
                yield return new WaitForSeconds(0.8f);
            }
            else
            {
                yield return null;
            }

        }

      //  yield return new WaitForSeconds(aTime);
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


 

        yield return new WaitForSeconds(.95f* Speed);



        yield return new WaitForSeconds(.25f);
        if (createdPoints.Count == 6)
        {
            bingo.SetActive(false);
            bingo.SetActive(true);
            totalScore += 50;
            if (thePlayer.ID == 0)
                AchivmentController.instance.Bingo();

            yield return new WaitForSeconds(1.8f);
            for (int i = 0; i < createdPoints.Count; i++)
            {
                createdPoints[i].transform.GetChild(0).Find("Text").GetComponent<Text>().text = totalScore.ToString();
                if (createdPoints.Count > 1)
                    createdPoints[i].GetComponent<RectTransform>().DOShakeScale(0.5f,0.9f, 8);
            }
            yield return new WaitForSeconds(1.0f);
        }


        for (int i = 0; i < createdPoints.Count; i++)
        {

            createdPoints[i].GetComponent<RectTransform>().DOMove(thePlayer.scoreObject.transform.position, 1).SetEase(Ease.InOutQuart);

            createdPoints[i].transform.GetChild(0).GetComponent<Image>().DOFade(0, 1* Speed).SetEase(Ease.InOutQuart); ;
        }
        bg.GetComponent<Image>().DOFade(0, 1.5f* Speed).SetEase(Ease.InOutQuart); ;



  


        yield return new WaitForSeconds(0.7f* Speed);


  
        
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



        //for (int i = 0; i < score.Count;i++)
        //{
        //    StartCoroutine(ShowScoreAfterTime (0.4f+0.25f+ i * 0.6f, score[i] ));
        //}
        StartCoroutine(SummarizeAfterTimeSendCall(0.4f + 0.5f + score.Count * 0.6f, score, aPlayer));


        Startup._instance.PlaySoundEffect(3);

    }

    IEnumerator ShowScoreAfterTime(float aTime, Tile aTile, bool shouldAddToHistory = true)
    {

        for (float timer = aTime; timer >= 0; timer -= Time.deltaTime)
        {
            if (Speed != 1)
            {

                timer = 0;
            }
            else
            {
                yield return null;
            }

        }


       // yield return new WaitForSeconds(aTime);
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
             //   Startup._instance.GameToLoad.History.Add(aTile.GetBoardPosition() + "#" + aTile.textLabel.text + "#" + aTile.GetValue() + "#" + Startup._instance.GameToLoad.GetPlayerTurn(GameManager.instance.CurrentTurn).ToString());

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
    IEnumerator SummarizeAfterTimeSendCall(float aTime, List<Tile> score, Player thePlayer)
    {
        yield return new WaitForSeconds(0.1f);

        if (GameManager.instance.thePlayers[1].isAI == false)
            for (int i = 0; i < score.Count; i++)
            {
                Startup._instance.GameToLoad.History.Add(score[i].GetBoardPosition() + "#" + score[i].textLabel.text + "#" + score[i].GetValue() + "#" + Startup._instance.GameToLoad.GetPlayerTurn(GameManager.instance.CurrentTurn).ToString() + "#" + (int)score[i].PlacedOnTile.myTileType);
            }


        int totalScore = 0;
        for (int i = 0; i < score.Count; i++)
        {
            totalScore += int.Parse(score[i].GetValue());
        }
        if (score.Count == 6)
        {
            totalScore += 50;
            if (AchivmentController.instance != null)
                AchivmentController.instance.Bingo();
        }

        yield return new WaitForSeconds(0.1f);

        GameManager.instance.AddScore(thePlayer, totalScore, score.Count,true,false,false);
        yield return new WaitForSeconds(0.1f);
        if (GameManager.instance.thePlayers[1].isAI == false)
        {
            for (int i = 0; i < score.Count; i++)
            {
                score[i].PreDestroy();
            }
        }
            

        Board.instance.LoadLastUsedTiles(PlayerBoard.instance.myPlayer.myTiles);
        // yield return new WaitForSeconds(1.0f);
        yield return new WaitForSeconds(0.1f);
        thePlayer.AddNewPlayerTiles(false);
        PlayerBoard.instance.RefreshLayout();
        yield return new WaitForSeconds(0.1f);
        bool isEmptyTurn = false;

        if (score.Count <= 0)
            isEmptyTurn = true;
        //yield return new WaitForSeconds(0.2f);

        currentaTime = aTime;
        currentscore = score;
        currentPlayer = thePlayer;




        if (GameManager.instance.thePlayers[1].isAI == false)
            GameManager.instance.NextTurn(isEmptyTurn, OnTurnWasSent);

        GameManager.instance.WaitingOverlay.SetActive(false);

        GameManager.instance.MakeLastPlayedTilesColored();

        yield return new WaitForSeconds(0.05f);

        //for (int i = 0; i < score.Count; i++)
        //{
        //    StartCoroutine(ShowScoreAfterTime(0.4f + 0.25f + i * 0.6f, score[i]));
        //}


        //StartCoroutine(SummarizeAfterTime(aTime, score, thePlayer));

        if(GameManager.instance.thePlayers[1].isAI)
        {
            OnTurnWasSent();
        }

        if (PlayerBoard.instance.myPlayer == thePlayer)
            Board.instance.LoadLastUsedTiles(PlayerBoard.instance.myPlayer.myTiles);

    }

    float currentaTime;
    List<Tile> currentscore;
    Player currentPlayer;
    public float Speed = 1;
    public void OnTurnWasSent()
    {
        if(GameManager.instance.thePlayers[1].isAI)
        {
            if(GameManager.instance.CheckIfMyTurn(false))
            {
                //GameManager.instance.IsSendingData = false;
                        
            }
            GameManager.instance.SendingDataDelay = 0;
        }
        else
        {
            GameManager.instance.IsSendingData = false;
        }



        Speed = 1;
        for (int i = 0; i < currentscore.Count; i++)
        {
            StartCoroutine(ShowScoreAfterTime(0.4f + 0.25f + i * 0.6f* Speed, currentscore[i]));
        }


        StartCoroutine(SummarizeAfterTime(currentaTime* Speed, currentscore, currentPlayer));
    }
    IEnumerator SummarizeAfterTime(float aTime, List<Tile> score,Player thePlayer)
    {
        for (float timer = aTime; timer >= 0; timer -= Time.deltaTime)
        {
            if (Speed != 1)
            {

                timer = 0;
                yield return new WaitForSeconds(0.8f);
            }
            else
            {
                yield return null;
            }

        }

     //   yield return new WaitForSeconds(aTime);
        
        if (createdPoints.Count>1)
        {
            Vector2 avgPos = GetAvgPos();
            for (int i = 0; i < createdPoints.Count; i++)
            {
                createdPoints[i].GetComponent<RectTransform>().DOMove(avgPos, 0.5f).SetEase(Ease.InOutQuart);
            }
        }
        Startup._instance.PlaySoundEffect(2);
        yield return new WaitForSeconds(0.15f* Speed);

        yield return new WaitForSeconds(0.15f* Speed);

        int totalScore = 0;
        for (int i = 0; i < score.Count; i++)
        {
            totalScore += int.Parse(score[i].GetValue());
        }
        for (int i = 0; i < createdPoints.Count; i++)
        {
            createdPoints[i].transform.GetChild(0).Find("Text").GetComponent<Text>().text = totalScore.ToString();
            if (createdPoints.Count > 1)
                createdPoints[i].GetComponent<RectTransform>().DOScale(createdPoints[i].transform.localScale*1.3f, 0.1f* Speed).SetEase(Ease.InOutQuart);
        }
        
        yield return new WaitForSeconds(.95f* Speed);


        yield return new WaitForSeconds(.25f);
        if (createdPoints.Count == 6)
        {
            bingo.SetActive(false);
            bingo.SetActive(true);
            totalScore += 50;

            //yield return new WaitForSeconds(0.4f* Speed);
            //createdPoints[createdPoints.Count-1].transform.GetChild(0).Find("Text").GetComponent<Text>().text = totalScore.ToString();
            //yield return new WaitForSeconds(0.4f* Speed);


            yield return new WaitForSeconds(1.8f);
            for (int i = 0; i < createdPoints.Count; i++)
            {
                createdPoints[i].transform.GetChild(0).Find("Text").GetComponent<Text>().text = totalScore.ToString();
                if (createdPoints.Count > 1)
                    createdPoints[i].GetComponent<RectTransform>().DOShakeScale(0.5f, 0.9f, 8);
            }
            yield return new WaitForSeconds(1.0f);

        }












        for (int i = 0; i < createdPoints.Count; i++)
        {
            
            createdPoints[i].GetComponent<RectTransform>().DOMove(thePlayer.scoreObject.transform.position, 1).SetEase(Ease.InOutQuart);

            createdPoints[i].transform.GetChild(0).GetComponent<Image>().DOFade(0, 1).SetEase(Ease.InOutQuart); ;
        }
        bg.GetComponent<Image>().DOFade(0, 1.5f).SetEase(Ease.InOutQuart); ;

        //Startup._instance.PlaySoundEffect(2);

        yield return new WaitForSeconds(0.7f);


        GameManager.instance.UpdateUI();

        //GameManager.instance.AddScore(thePlayer, totalScore, createdPoints.Count);
        for (int i = 0; i < score.Count; i++)
        {
            score[i].Flip();
        }

        for (int i = 0; i < createdPoints.Count; i++)
        {
            Destroy(createdPoints[i]);
        }
        createdPoints.Clear();

        if (GameManager.instance.thePlayers[1].isAI == true)
            Board.instance.LoadLastUsedTiles(PlayerBoard.instance.myPlayer.myTiles);
            yield return new WaitForSeconds(1.0f);




        if (GameManager.instance.thePlayers[1].isAI == true)
        {
            thePlayer.AddNewPlayerTiles();
            PlayerBoard.instance.RefreshLayout();
        }


        bool isEmptyTurn = false;

        if (score.Count <= 0)
            isEmptyTurn = true;

        if(GameManager.instance.thePlayers[1].isAI)
            GameManager.instance.NextTurn(isEmptyTurn);

        yield return new WaitForSeconds(0.03f);
        GameManager.instance.MakeLastPlayedTilesColored();
        bg.SetActive(false);


        GameManager.instance.WaitingOverlay.SetActive(true);
        GameManager.instance.otherPlayerTurnText.text = "Waiting for " + GameManager.instance.thePlayers[1].Username + "..";
        GameManager.instance.WaitingOverlay.GetComponent<CanvasGroup>().alpha = 0;
        GameManager.instance.WaitingOverlay.GetComponent<CanvasGroup>().DOFade(1, 0.5f* Speed).SetEase(Ease.InOutQuart);



        //if (TutorialController.instance == null && UnityEngine.Random.Range(0, 100) < 50)
        //    Appodeal.show(Appodeal.INTERSTITIAL);

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

        //Debug.Log(sVector);
        //Debug.Log(sArray);
        //Debug.Log(sArray[0]);
        //Debug.Log(sArray[1]);


        // store as a Vector3
        Vector2 result = new Vector2(
            float.Parse(sArray[0], CultureInfo.InvariantCulture.NumberFormat),
            float.Parse(sArray[1],CultureInfo.InvariantCulture.NumberFormat));

        return result;
    }

}
