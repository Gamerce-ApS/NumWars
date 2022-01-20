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
        isOpen = true;
        bg.gameObject.SetActive(true);
        bg.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        bg.GetComponent<Image>().DOFade(194f / 255f, 0.5f).SetEase(Ease.InOutQuart);

        parent.gameObject.SetActive(true);

       // parent.GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.InOutQuart);


        parent.transform.position = new Vector3(originalPosX - 10, parent.transform.position.y, parent.transform.position.z);
        parent.transform.DOMoveX(originalPosX, 0.3f).SetEase(Ease.InOutQuart);


        for (int i = 0; i < PlayerBoard.instance.myTiles.Count;i++)
        {
            PlayerBoard.instance.myTiles[i].ReturnFromBoard();
        }

    }
    public void myCompleteFunction()
    {
        parent.gameObject.SetActive(false);
        bg.gameObject.SetActive(false);
    }
    public void SwapYes()
    {

        for (int i = SelectedTiles.Count - 1; i >= 0; i--)
        {
            Tile currentT = SelectedTiles[i];

            PlayerBoard.instance.AllTilesNumbers.Add(int.Parse(currentT.textLabel.text));

            PlayerBoard.instance.myTiles.Remove(currentT);
            Destroy(currentT.gameObject);
            SelectedTiles.Remove(currentT);
        }



            


        PlayerBoard.instance.AddNewPlayerTiles();

        CloseWindow();
    }
}
