using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SwapScreen : MonoBehaviour
{
    public Image bg;
    public Transform parent;

    float originalPosX = 0;

    public static SwapScreen instance=null;

    public List<Tile> SelectedTiles = new List<Tile>();

    public bool isOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        originalPosX = parent.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ClickedTile(Tile aTile)
    {
        if(isOpen)
        {
            if (SelectedTiles.Contains(aTile) == true)
            {
                SelectedTiles.Remove(aTile);
                aTile.GetComponent<Image>().sprite = aTile.Normal;
            }
            else
            {
                SelectedTiles.Add(aTile);
                aTile.GetComponent<Image>().sprite = aTile.Green;
            }
        }
     

    }
    public void CloseWindow()
    {
        for(int i = SelectedTiles.Count-1; i>=0 ;i--)
        {

            SelectedTiles[i].GetComponent<Image>().sprite = SelectedTiles[i].Normal;
            SelectedTiles.Remove(SelectedTiles[i]);
        }

        isOpen = false;
        bg.GetComponent<Image>().DOFade(0, 0.5f).SetEase(Ease.InOutQuart).OnComplete(myCompleteFunction);
        // parent.GetComponent<CanvasGroup>().DOFade(0f, 0.5f).SetEase(Ease.InOutQuart).OnComplete(myCompleteFunction); ;


        parent.transform.position = new Vector3(originalPosX , parent.transform.position.y, parent.transform.position.z);
        parent.transform.DOMoveX(originalPosX + 10, 0.3f).SetEase(Ease.InOutQuart);
    }
    public void OpenWindow()
    {
        if (GameManager.instance.CheckIfMyTurn() == false)
            return;


        if(Board.instance.AllTilesNumbers.Count <=0)
        {
            AlertText.instance.ShowAlert("No tiles to swap!");

            for (int i = 0; i < PlayerBoard.instance.myPlayer.myTiles.Count; i++)
            {
                PlayerBoard.instance.myPlayer.myTiles[i].ReturnFromBoard();
            }

            return;
        }


        isOpen = true;
        bg.gameObject.SetActive(true);
        bg.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        bg.GetComponent<Image>().DOFade(194f / 255f, 0.5f).SetEase(Ease.InOutQuart);

        parent.gameObject.SetActive(true);

       // parent.GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.InOutQuart);


        parent.transform.position = new Vector3(originalPosX - 10, parent.transform.position.y, parent.transform.position.z);
        parent.transform.DOMoveX(originalPosX, 0.3f).SetEase(Ease.InOutQuart);


        for (int i = 0; i < PlayerBoard.instance.myPlayer.myTiles.Count;i++)
        {
            PlayerBoard.instance.myPlayer.myTiles[i].ReturnFromBoard();
        }

    }
    public void myCompleteFunction()
    {
        parent.gameObject.SetActive(false);
        bg.gameObject.SetActive(false);
    }
    public void SwapYes()
    {
        GameManager.instance.IsSendingData = true;
        GameManager.instance.SendingDataDelay = 0;

        if (SelectedTiles.Count > Board.instance.AllTilesNumbers.Count)
        {
            AlertText.instance.ShowAlert("You can only swap "+ Board.instance.AllTilesNumbers.Count+"!");
            return;
        }
        //string swappedHistory = "";
        //for (int i = 0; i < GameManager.instance.thePlayers[0].GetMyTiles().Count;i++)
        //{
            
        //  swappedHistory += "," + GameManager.instance.thePlayers[0].GetMyTiles()[i];

        //}



        for (int i = SelectedTiles.Count - 1; i >= 0; i--)
        {
            Tile currentT = SelectedTiles[i];


            Board.instance.AllTilesNumbers.Add(int.Parse(currentT.textLabel.text));

            PlayerBoard.instance.myPlayer.myTiles.Remove(currentT);
            Destroy(currentT.gameObject);
            SelectedTiles.Remove(currentT);
        }






       List<int> newTiles = PlayerBoard.instance.myPlayer.AddNewPlayerTiles(false);

        string swappedHistory = "";
        for (int i = 0; i < newTiles.Count; i++)
        {
            swappedHistory += "," + newTiles[i].ToString();
        }

        if (Startup._instance.GameToLoad != null && Startup._instance.GameToLoad.BoardTiles != null)
        {
            Startup._instance.GameToLoad.History.Add("#SWAP#" + Startup.instance.MyPlayfabID + "#" + swappedHistory);
        }
        else
            Board.instance.History.Add("#SWAP#" + Startup.instance.MyPlayfabID + "#" + swappedHistory);



        Board.instance.LoadLastUsedTiles(PlayerBoard.instance.myPlayer.myTiles);




        PlayerBoard.instance.RefreshLayout();
        CloseWindow();




        GameManager.instance.NextTurn(false, OnSwapDone);
    }
    public void OnSwapDone()
    {
        GameManager.instance.IsSendingData = false;

        GameManager.instance.WaitingOverlay.SetActive(true);
        GameManager.instance.otherPlayerTurnText.text = "Waiting for " + GameManager.instance.thePlayers[1].Username + "..";
        GameManager.instance.WaitingOverlay.GetComponent<CanvasGroup>().alpha = 0;
        GameManager.instance.WaitingOverlay.GetComponent<CanvasGroup>().DOFade(1, 0.5f * ScoreScreen.instance.Speed).SetEase(Ease.InOutQuart);

    }
}
