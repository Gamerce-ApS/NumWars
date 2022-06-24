using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ValidationScreen : MonoBehaviour
{
    public Image bg;
    public Transform parent;

    float originalPosX = 0;

    public static ValidationScreen instance =null;

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
 
    public void CloseWindow()
    {


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




        isOpen = true;
        bg.gameObject.SetActive(true);
        bg.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        bg.GetComponent<Image>().DOFade(194f / 255f, 0.5f).SetEase(Ease.InOutQuart);

        parent.gameObject.SetActive(true);

       // parent.GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.InOutQuart);


        parent.transform.position = new Vector3(originalPosX - 10, parent.transform.position.y, parent.transform.position.z);
        parent.transform.DOMoveX(originalPosX, 0.3f).SetEase(Ease.InOutQuart);




    }
    public void myCompleteFunction()
    {
        parent.gameObject.SetActive(false);
        bg.gameObject.SetActive(false);
    }
    public void SwapYes()
    {

        GameManager.instance.IsSendingData = true;


        CloseWindow();




        ScoreScreen.instance.ShowScore(new List<Tile>(), PlayerBoard.instance.myPlayer);
    }
}
