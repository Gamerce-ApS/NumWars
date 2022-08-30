using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameFinishedScreen : MonoBehaviour
{
    public static GameFinishedScreen _instance = null;
    public static GameFinishedScreen instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameFinishedScreen>();
            }

            return _instance;

        }

    }
    public GameObject errorButton;
    public GameObject errorButtonRetry;
    
public Text p1_name;
    public Text p1_thropies;
    public Text p1_score;
    public Text p1_wins;
    public GameObject p1_wins_go;

    public Text p2_name;
    public Text p2_thropies;
    public Text p2_score;
    public Text p2_wins;
    public GameObject p2_wins_go;

    public List<GameObject> ElementsToMoveOut = new List<GameObject>();

    BoardData lastBF;

    public bool isShowing = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Show()
    {
        isShowing = true;
        for (int i = 0; i < ElementsToMoveOut.Count; i++)
        {
            if (ElementsToMoveOut[i] != null)
                ElementsToMoveOut[i].transform.DOMoveX(10, 0.5f).SetEase(Ease.InOutQuart);
        }

        transform.GetChild(0).gameObject.SetActive(true);

        float posX = transform.GetChild(0).transform.position.x;
        transform.GetChild(0).transform.position -= new Vector3(10, 0, 0);
        transform.GetChild(0).DOMoveX(posX, 0.5f).SetEase(Ease.InOutQuart);

        transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(0, 0, 0, 0);
        transform.GetChild(0).GetChild(0).GetComponent<Image>().DOFade(107f / 255f, 0.5f).SetEase(Ease.InOutQuart).SetDelay(0.1f);
        p1_wins_go.SetActive(true);
        p2_wins_go.SetActive(false);
    }
    public void Show(BoardData bf)
    {
        isShowing = true;
        lastBF = bf;
        bool isOpenGame = false;
        for (int i = 0; i < Startup._instance.openGamesList.Count; i++)
        {
            if (Startup._instance.openGamesList[i].RoomName == bf.RoomName)
            {
                isOpenGame = true;
            }
        }


        if (bf.hasFinished == false && bf.player2_displayName == "AI")
        PlayfabHelperFunctions.instance.AddAiGameToOldGames(CompressString.StringCompressor.CompressString(bf.GetJson()));



        for (int i = 0; i< ElementsToMoveOut.Count;i++)
        {
            if(ElementsToMoveOut[i] != null)
            ElementsToMoveOut[i].transform.DOMoveX(10, 0.5f).SetEase(Ease.InOutQuart);
        }

        transform.GetChild(0).gameObject.SetActive(true);

        float posX = transform.GetChild(0).transform.position.x;
        transform.GetChild(0).transform.position -= new Vector3(10, 0, 0);
        transform.GetChild(0).DOMoveX(posX, 0.5f).SetEase(Ease.InOutQuart);

        transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(0, 0, 0, 0);
        transform.GetChild(0).GetChild(0).GetComponent<Image>().DOFade(107f / 255f, 0.5f).SetEase(Ease.InOutQuart).SetDelay(0.1f);

        string opponentPlayfabID = "";
        if (Startup._instance.MyPlayfabID == bf.player1_PlayfabId)
        {
            
            p1_name.text = bf.player1_displayName;
            p1_thropies.text = GameManager.instance.p1_thropies.text;
            p1_score.text = bf.player1_score;
            p1_wins.text = "-";

            p2_name.text = bf.player2_displayName;
            p2_thropies.text = GameManager.instance.p2_thropies.text;
            p2_score.text = bf.player2_score;
            p2_wins.text = "-";

            if (bf.hasFinished == false && isOpenGame && bf.player2_displayName != "AI")
            {
                if (int.Parse(bf.player1_score) > int.Parse(bf.player2_score))
                    Startup._instance.AdjustThropies(30, bf.player2_PlayfabId, bf.player2_displayName, int.Parse(bf.player1_score));
                else
                    Startup._instance.AdjustThropies(-15, bf.player2_PlayfabId, bf.player2_displayName, int.Parse(bf.player1_score));
            }



            opponentPlayfabID = bf.player2_PlayfabId;
        }
        else
        {
            p2_name.text = bf.player1_displayName;
            p2_thropies.text = "-";
            p2_score.text = bf.player1_score;
            p2_wins.text = "-";

            p1_name.text = bf.player2_displayName;
            p1_thropies.text = "-";
            p1_score.text = bf.player2_score;
            p1_wins.text = "-";

            if (bf.hasFinished == false && isOpenGame && bf.player2_displayName != "AI")
            {
                if (int.Parse(bf.player2_score) > int.Parse(bf.player1_score))
                    Startup._instance.AdjustThropies(30, bf.player1_PlayfabId, bf.player1_displayName, int.Parse(bf.player2_score));
                else
                    Startup._instance.AdjustThropies(-15, bf.player1_PlayfabId, bf.player1_displayName, int.Parse(bf.player2_score));
            }


            opponentPlayfabID = bf.player1_PlayfabId;
        }

        int p1Win = 0;
        int p2Win = 0;

        if (int.Parse(p1_score.text) > int.Parse(p2_score.text))
        {
            p1_wins_go.SetActive(true);
            p2_wins_go.SetActive(false);
            p1Win = 1;
        }
        else
        {
            p1_wins_go.SetActive(false);
            p2_wins_go.SetActive(true);
            p2Win = 1;
        }




        if (opponentPlayfabID == "")
            opponentPlayfabID = "AI";

        //StatsData _statsData = Startup._instance.GetStatsData();
        //int amountLost = 0;
        //int amountWin = 0;
        //for(int i = 0; i < _statsData.FinishedGames.Count;i++)
        //{
        //    if ( _statsData.FinishedGames[i].PlayfabID == opponentPlayfabID)
        //    {
        //        if (_statsData.FinishedGames[i].Winner == Startup._instance.MyPlayfabID)
        //            amountWin++;
        //        else
        //            amountLost++;
        //    }
        //}
        //p1_wins.text = amountWin.ToString();
        //p2_wins.text = amountLost.ToString();

        int amountLost = 0;
        int amountWin = 0;
        for (int i = 0; i < Startup.instance.myOldGameList.Count; i++)
        {
            if ((Startup.instance.myOldGameList[i].player1_PlayfabId == opponentPlayfabID || Startup.instance.myOldGameList[i].player2_PlayfabId == opponentPlayfabID))
            {
                if (Startup.instance.myOldGameList[i].GetWinner() == Startup.instance.MyPlayfabID)
                {
                    amountWin++;
                }
                else
                {
                    amountLost++;
                }
            }
        }
        if (amountWin + amountLost > 0)
        {
            p1_wins.text = (amountWin+ p1Win).ToString();
            p2_wins.text = (amountLost+ p2Win).ToString();
        }
        else
        {
            p1_wins.text = "0";
            p2_wins.text = "0";
        }






        if (isOpenGame)
        {
            contineAfterCallButton.enabled = false;
            contineAfterCallButton.transform.GetChild(0).GetComponent<Text>().text = "Loading..";

            PlayfabHelperFunctions.instance.RemoveRoomFromList(bf.RoomName, bf.GetJson(), WhenRemovedGameDone);
            for (int i = 0; i < Startup._instance.openGamesList.Count; i++)
            {
                if (Startup._instance.openGamesList[i].RoomName == bf.RoomName)
                {
                    Startup._instance.openGamesList.RemoveAt(i);
                    return;
                }
            }
        }
    }
    
    public void PressThereWasAnError()
    {
        SceneManager.LoadScene(0);
        Startup._instance.Refresh(0.1f);
        if (Startup._instance.avatarURL != null)
            if (Startup._instance.avatarURL.Length > 0)
            {
                PlayfabHelperFunctions.instance.LoadAvatarURL(Startup._instance.avatarURL);
            }
    }
    public void PressThereWasAnErrorReUpload()
    {
        PlayfabHelperFunctions.instance.RetryUploadFile(WhenRemovedGameDone);
    }
    public void WhenRemovedGameDone()
    {
        contineAfterCallButton.enabled = true;
        contineAfterCallButton.transform.GetChild(0).GetComponent<Text>().text = "Continue";

        PlayfabHelperFunctions.instance.RemoveAbandonedGamesCO();
    }


    public Button contineAfterCallButton;
    public void PressContinue()
    {
        //SceneManager.LoadScene(0);
        //Startup._instance.Refresh(0.1f);
        //if (Startup._instance.avatarURL != null)
        //    if (Startup._instance.avatarURL.Length > 0)
        //    {
        //        PlayfabHelperFunctions.instance.LoadAvatarURL(Startup._instance.avatarURL);
        //    }


        //PlayerPrefs.SetString("ChallengePlayerAgain", lastBF.GetOtherPlayerPlayfab());


        contineAfterCallButton.enabled = false;
        contineAfterCallButton.transform.GetChild(0).GetComponent<Text>().text = "Loading...";


        PlayfabCallbackHandler.instance.GetSharedDataGrouped2(Startup.instance.MyPlayfabID, result2 => {

            if (result2 == null || result2.FunctionResult == null)
            {
                SceneManager.LoadScene(0);
                Startup._instance.Refresh(0.1f);
                if (Startup._instance.avatarURL != null)
                    if (Startup._instance.avatarURL.Length > 0)
                    {
                        PlayfabHelperFunctions.instance.LoadAvatarURL(Startup._instance.avatarURL);
                    }
                return;
            }

            IEnumerable test = (IEnumerable)result2.FunctionResult;
            List<BoardData> gameList = new List<BoardData>();

            if (result2.Error != null)
            {
                Debug.LogError(result2.Error.Error);
                Debug.LogError(result2.Error.Message);
                Debug.LogError(result2.Error.StackTrace);

                SceneManager.LoadScene(0);
                Startup._instance.Refresh(0.1f);
                if (Startup._instance.avatarURL != null)
                    if (Startup._instance.avatarURL.Length > 0)
                    {
                        PlayfabHelperFunctions.instance.LoadAvatarURL(Startup._instance.avatarURL);
                    }


            }
      

            foreach (IEnumerable item in test)
            {

                char[] t = item.ToString().ToCharArray();
                List<int> b = new List<int>();
                for (int i = 0; i < t.Length; i++)
                {
                    b.Add((t[i]));
                }
                string con = PlayfabHelperFunctions.Decompress(b);
                gameList.Add(new BoardData(con));
            }

            bool foundActiveGame = false;
            for (int i = 0; i < gameList.Count; i++)
            {
                if (gameList[i].player1_PlayfabId == Startup._instance.MyPlayfabID || gameList[i].player2_PlayfabId == Startup._instance.MyPlayfabID &&
                    gameList[i].player1_PlayfabId == lastBF.GetOtherPlayerPlayfab() || gameList[i].player2_PlayfabId == lastBF.GetOtherPlayerPlayfab())
                    foundActiveGame = true;
            }



            if (foundActiveGame == false)
            {


                challengeWindow.SetActive(true);

                float posX = challengeWindow.transform.GetChild(0).transform.position.x;
                challengeWindow.transform.GetChild(0).transform.position -= new Vector3(10, 0, 0);
                challengeWindow.transform.GetChild(0).DOMoveX(posX, 0.5f).SetEase(Ease.InOutQuart);

                challengeWindow.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                challengeWindow.GetComponent<Image>().DOFade(107f / 255f, 0.5f).SetEase(Ease.InOutQuart).SetDelay(0.1f);



                float posX2 = transform.GetChild(0).transform.position.x;
                transform.GetChild(0).DOMoveX(posX2 + 10, 0.5f).SetEase(Ease.InOutQuart);


            }
            else
            {

                SceneManager.LoadScene(0);
                Startup._instance.Refresh(0.1f);
                if (Startup._instance.avatarURL != null)
                    if (Startup._instance.avatarURL.Length > 0)
                    {
                        PlayfabHelperFunctions.instance.LoadAvatarURL(Startup._instance.avatarURL);
                    }



            }





        }, error => {
            SceneManager.LoadScene(0);
            Startup._instance.Refresh(0.1f);
            if (Startup._instance.avatarURL != null)
                if (Startup._instance.avatarURL.Length > 0)
                {
                    PlayfabHelperFunctions.instance.LoadAvatarURL(Startup._instance.avatarURL);
                }
            Debug.Log("Got error retrieving user when challenging data:");
            Debug.Log(error.GenerateErrorReport());
        });








        //bool hasActiveGame = false;
        //for (int i = 0; i < Startup.instance.openGamesList.Count; i++)
        //{
        //    if ((Startup.instance.openGamesList[i].player1_PlayfabId == lastBF.GetOtherPlayerPlayfab() || Startup.instance.openGamesList[i].player2_PlayfabId == lastBF.GetOtherPlayerPlayfab())
        //        && lastBF.RoomName != Startup.instance.openGamesList[i].RoomName)
        //    {
        //        hasActiveGame = true;
        //    }
        //}

        //if (hasActiveGame == false)
        //{
        //    challengeWindow.SetActive(true);

        //    float posX = challengeWindow.transform.GetChild(0).transform.position.x;
        //    challengeWindow.transform.GetChild(0).transform.position -= new Vector3(10, 0, 0);
        //    challengeWindow.transform.GetChild(0).DOMoveX(posX, 0.5f).SetEase(Ease.InOutQuart);

        //    challengeWindow.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        //    challengeWindow.GetComponent<Image>().DOFade(107f / 255f, 0.5f).SetEase(Ease.InOutQuart).SetDelay(0.1f);



        //    float posX2 = transform.GetChild(0).transform.position.x;
        //    transform.GetChild(0).DOMoveX(posX2 + 10, 0.5f).SetEase(Ease.InOutQuart);
        //}
        //else
        //{
        //    SceneManager.LoadScene(0);
        //    Startup._instance.Refresh(0.1f);
        //    if (Startup._instance.avatarURL != null)
        //        if (Startup._instance.avatarURL.Length > 0)
        //        {
        //            PlayfabHelperFunctions.instance.LoadAvatarURL(Startup._instance.avatarURL);
        //        }
        //}





    }
    public GameObject challengeWindow;
    public void ChallengeAgainYes()
    {

       // LoadingOverlay.instance.ShowLoadingFullscreen("Challenge in progress..");
        string newRoomName = string.Format("{0}-{1}", Startup._instance.displayName + "_" + Startup._instance.MyPlayfabID + "_", Random.Range(0, 1000000).ToString()) + Random.Range(0, 1000000).ToString();
        //PlayfabHelperFunctions.instance.SetPlayfabCreatedRoom(Startup._instance.MyPlayfabID, newRoomName);


        challengeWindow.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        challengeWindow.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        challengeWindow.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);

        PlayfabCallbackHandler.instance.GetSharedDataGrouped2(Startup.instance.MyPlayfabID, result2 => {

            if (result2 == null || result2.FunctionResult == null)
            {
                SceneManager.LoadScene(0);
                Startup._instance.Refresh(0.1f);
                if (Startup._instance.avatarURL != null)
                    if (Startup._instance.avatarURL.Length > 0)
                    {
                        PlayfabHelperFunctions.instance.LoadAvatarURL(Startup._instance.avatarURL);
                    }
                return;
            }



            IEnumerable test = (IEnumerable)result2.FunctionResult;
            List<BoardData> gameList = new List<BoardData>();
            foreach (IEnumerable item in test)
            {

                char[] t = item.ToString().ToCharArray();
                List<int> b = new List<int>();
                for (int i = 0; i < t.Length; i++)
                {
                    b.Add((t[i]));
                }
                string con = PlayfabHelperFunctions.Decompress(b);
                gameList.Add(new BoardData(con));
            }

            bool foundActiveGame = false;
            for(int i = 0; i < gameList.Count;i++)
            {
                if (gameList[i].player1_PlayfabId == Startup._instance.MyPlayfabID || gameList[i].player2_PlayfabId == Startup._instance.MyPlayfabID &&
                    gameList[i].player1_PlayfabId == lastBF.GetOtherPlayerPlayfab() || gameList[i].player2_PlayfabId == lastBF.GetOtherPlayerPlayfab())
                    foundActiveGame = true;
            }


            
            if(foundActiveGame == false)
            {
                PlayfabHelperFunctions.instance.ChallengePlayer(Startup._instance.MyPlayfabID, lastBF.GetOtherPlayerPlayfab(), lastBF.GetOtherPlayer(), newRoomName);
                //PlayfabHelperFunctions.instance.AddGameToPlayerListCloudScript(lastBF.GetOtherPlayerPlayfab(), newRoomName);
                //PlayfabHelperFunctions.instance.AddGameToPlayerListCloudScript(Startup._instance.MyPlayfabID, newRoomName);

                StartCoroutine(ChallengeProgress());
            }
            else
            {
                ChallengeAgainNo();
            }
      




        }, error => {
            Debug.Log("Got error retrieving user when challenging data:");
            Debug.Log(error.GenerateErrorReport());
        });

    }

    public IEnumerator ChallengeProgress()
    {

        yield return new WaitForSeconds(5);


        SceneManager.LoadScene(0);
        Startup._instance.Refresh(0.1f);
        if (Startup._instance.avatarURL != null)
            if (Startup._instance.avatarURL.Length > 0)
            {
                PlayfabHelperFunctions.instance.LoadAvatarURL(Startup._instance.avatarURL);
            }


    }

    public void ChallengeAgainNo()
    {


        SceneManager.LoadScene(0);
        Startup._instance.Refresh(0.1f);
        if (Startup._instance.avatarURL != null)
            if (Startup._instance.avatarURL.Length > 0)
            {
                PlayfabHelperFunctions.instance.LoadAvatarURL(Startup._instance.avatarURL);
            }
    }
}
