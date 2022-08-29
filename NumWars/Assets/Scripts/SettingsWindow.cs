using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VoxelBusters.EssentialKit;

public class SettingsWindow : MonoBehaviour
{
    public Vector3 _TextFlyInBoxoriginalPos;
    public GameObject ResignWindow;
    // Start is called before the first frame update
    void Start()
    {
        _TextFlyInBoxoriginalPos = gameObject.transform.GetChild(1).transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowSettings()
    {

        if (Startup._instance.GameToLoad  != null&& Startup._instance.GameToLoad.GetHasTimeout())
        {
            return;
        }
        if(Startup._instance.GameToLoad != null)
        {
            for (int i = 0; i < Startup.instance.myOldGameList.Count; i++)
            {
                if (Startup.instance.myOldGameList[i].RoomName == Startup._instance.GameToLoad.RoomName)
                {
                    return;
                }
            }
        }




        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.transform.GetChild(0).GetComponent<Image>().DOFade(157f / 255f, 0).SetEase(Ease.InOutQuart);

        gameObject.transform.GetChild(1).gameObject.SetActive(true);
        gameObject.transform.GetChild(1).transform.position = new Vector3(_TextFlyInBoxoriginalPos.x - 10, _TextFlyInBoxoriginalPos.y, _TextFlyInBoxoriginalPos.z);
        gameObject.transform.GetChild(1).transform.DOMoveX(_TextFlyInBoxoriginalPos.x, 0.3f).SetEase(Ease.InOutQuart);
    }

    public void CloseWindow()
    {
        gameObject.transform.GetChild(0).GetComponent<Image>().DOFade(0, 157f / 255f).SetEase(Ease.InOutQuart);
        gameObject.transform.GetChild(1).transform.DOMoveX(_TextFlyInBoxoriginalPos.x + 10, 0.3f).SetEase(Ease.InOutQuart).OnComplete(() => { gameObject.transform.GetChild(0).gameObject.SetActive(false); gameObject.transform.GetChild(1).gameObject.SetActive(false); });
        gameObject.transform.GetChild(2).transform.DOMoveX(_TextFlyInBoxoriginalPos.x + 10, 0.3f).SetEase(Ease.InOutQuart).OnComplete(() => { gameObject.transform.GetChild(0).gameObject.SetActive(false); gameObject.transform.GetChild(2).gameObject.SetActive(false); });


        ResignWindow.SetActive(false);

    }

    public void GoBack()
    {
        if(gameObject.transform.GetChild(2).gameObject.activeSelf)
        {
            gameObject.transform.GetChild(2).transform.DOMoveX(_TextFlyInBoxoriginalPos.x + 10, 0.3f).SetEase(Ease.InOutQuart).OnComplete(() => {  gameObject.transform.GetChild(2).gameObject.SetActive(false); });

            gameObject.transform.GetChild(1).transform.position = new Vector3(_TextFlyInBoxoriginalPos.x - 10, _TextFlyInBoxoriginalPos.y, _TextFlyInBoxoriginalPos.z);
            gameObject.transform.GetChild(1).transform.DOMoveX(_TextFlyInBoxoriginalPos.x, 0.3f).SetEase(Ease.InOutQuart);

            if (GameManager.instance.thePlayers[1].isAI)
            {

                //   GameManager.instance.ClickBack();
                AlertText.instance.ShowAlert("Wait until end of AI turn!");
            }
            if (GameManager.instance.SendingDataDelay > 15)
            {
           
                    GameManager.instance.ClickBack();
            }

        }
        else if(GameManager.instance.IsSendingData)
        {
            gameObject.transform.GetChild(2).transform.DOMoveX(_TextFlyInBoxoriginalPos.x + 10, 0.3f).SetEase(Ease.InOutQuart).OnComplete(() => { gameObject.transform.GetChild(2).gameObject.SetActive(false); });

            gameObject.transform.GetChild(1).transform.position = new Vector3(_TextFlyInBoxoriginalPos.x - 10, _TextFlyInBoxoriginalPos.y, _TextFlyInBoxoriginalPos.z);
            gameObject.transform.GetChild(1).transform.DOMoveX(_TextFlyInBoxoriginalPos.x, 0.3f).SetEase(Ease.InOutQuart);

            if (GameManager.instance.thePlayers[1].isAI)
            {
      
                 //   GameManager.instance.ClickBack();
                AlertText.instance.ShowAlert("Wait until end of AI turn!");
            }
            if (GameManager.instance.SendingDataDelay>5)
            {

                GameManager.instance.ClickBack();
            }
        }
        else
        {
            GameManager.instance.ClickBack();
        }



    }
    public void ShowTileList()
    {
        gameObject.transform.GetChild(1).transform.DOMoveX(_TextFlyInBoxoriginalPos.x + 10, 0.3f).SetEase(Ease.InOutQuart).OnComplete(() => {  });

        gameObject.transform.GetChild(2).gameObject.SetActive(true);
        gameObject.transform.GetChild(2).transform.position = new Vector3(_TextFlyInBoxoriginalPos.x - 10, _TextFlyInBoxoriginalPos.y, _TextFlyInBoxoriginalPos.z);
        gameObject.transform.GetChild(2).transform.DOMoveX(_TextFlyInBoxoriginalPos.x, 0.3f).SetEase(Ease.InOutQuart);

    }
    public void OpenResignWindow()
    {
       // gameObject.transform.GetChild(0).GetComponent<Image>().DOFade(0, 157f / 255f).SetEase(Ease.InOutQuart);
        gameObject.transform.GetChild(1).transform.DOMoveX(_TextFlyInBoxoriginalPos.x + 10, 0.3f).SetEase(Ease.InOutQuart).OnComplete(() => {  gameObject.transform.GetChild(1).gameObject.SetActive(false); });
        gameObject.transform.GetChild(2).transform.DOMoveX(_TextFlyInBoxoriginalPos.x + 10, 0.3f).SetEase(Ease.InOutQuart).OnComplete(() => {  gameObject.transform.GetChild(2).gameObject.SetActive(false); });

        ResignWindow.SetActive(true);
    }
    public void ResignGame()
    {
        ResignWindow.SetActive(false);
        if (GameManager.instance.thePlayers[1].isAI)
        {
            PlayerPrefs.DeleteKey("AIGame");
            GameManager.instance.ClickBack();
        }
  
        else
        {
            PlayfabHelperFunctions.instance.DeleteGame(Startup._instance.GameToLoad.RoomName, DeletedFinished);
        }


        //GameManager.instance.ClickBack();
    }

    public void DeletedFinished()
    {
        GameManager.instance.ClickBack();
    }
}
