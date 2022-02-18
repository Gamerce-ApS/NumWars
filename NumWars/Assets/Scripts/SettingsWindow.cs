using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingsWindow : MonoBehaviour
{
    public Vector3 _TextFlyInBoxoriginalPos;
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




    }
    public void GoBack()
    {
        if(gameObject.transform.GetChild(2).gameObject.activeSelf)
        {
            gameObject.transform.GetChild(2).transform.DOMoveX(_TextFlyInBoxoriginalPos.x + 10, 0.3f).SetEase(Ease.InOutQuart).OnComplete(() => {  gameObject.transform.GetChild(2).gameObject.SetActive(false); });

            gameObject.transform.GetChild(1).transform.position = new Vector3(_TextFlyInBoxoriginalPos.x - 10, _TextFlyInBoxoriginalPos.y, _TextFlyInBoxoriginalPos.z);
            gameObject.transform.GetChild(1).transform.DOMoveX(_TextFlyInBoxoriginalPos.x, 0.3f).SetEase(Ease.InOutQuart);
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
    public void ResignGame()
    {
        if(GameManager.instance.thePlayers[1].isAI)
            PlayerPrefs.DeleteAll();

        GameManager.instance.ClickBack();
    }
}
