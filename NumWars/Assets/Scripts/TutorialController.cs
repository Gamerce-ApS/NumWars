using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialAction
{
    public TutorialAction(string d, int aId, GameObject aHighlight=null, GameObject aHighlight2 = null)
    {
        TheDialouge = d;
        ID = aId;
        Highlight = aHighlight;
        Highlight2 = aHighlight2;
    }
    public int ID = 0;
    public string TheDialouge = "";
    GameObject Highlight = null;
    GameObject Highlight2 = null;

    public void Set()
    {
        TutorialController.instance.Character.GetComponent<Animator>().Play("jump");

        string oldD = TutorialController.instance.chatBoxLabel.text;
        TutorialController.instance.chatBoxLabel.text = TheDialouge;
        if (Highlight != null)
            Highlight.SetActive(true);
        if (Highlight2 != null)
            Highlight2.SetActive(true);

        if (ID == 3 || ID == 4 || ID == 6 || ID == 8 || ID == 9)
            TutorialController.instance.DoneButton.SetActive(false);
        else
        {
            TutorialController.instance.DoneButton.SetActive(true);
        }

        if (ID == 8)
        {
            TutorialController.instance.chatBoxLabel.transform.parent.transform.position += new Vector3(0, 1, 0);
            TutorialController.instance._TextFlyInBoxoriginalPos = TutorialController.instance.chatBoxLabel.transform.parent.position;

            //RectTransform rt = TutorialController.instance.Character.GetComponent<RectTransform>();
            //rt.anchoredPosition = new Vector2(rt.anchoredPosition.x,  - 6.3f);

            TutorialController.instance.Character.transform.position += new Vector3(0, 1, 0);

        }


        if (TheDialouge.Length == 0)
        {
            TutorialController.instance.chatBoxLabel.transform.gameObject.SetActive(false);
            TutorialController.instance.chatBoxLabel.transform.parent.gameObject.SetActive(false);
            TutorialController.instance.Character.gameObject.SetActive(false);

            
        }
        else
        {
            TutorialController.instance.chatBoxLabel.transform.gameObject.SetActive(true);
            TutorialController.instance.chatBoxLabel.transform.parent.gameObject.SetActive(true);
            TutorialController.instance.Character.gameObject.SetActive(true);

        }
        

        if(ID == 10)
        {
            RectTransform rt = TutorialController.instance.Character.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(-209.72f, rt.anchoredPosition.y);

            TutorialController.instance._TextFlyInBoxoriginalPos = TutorialController.instance.chatBoxLabel.transform.parent.position;
        }
        if (ID == 11)
        {
            SceneManager.LoadScene(0);
            Startup._instance.Refresh(0.1f);
            return;
        }

        TutorialController.instance.MoveBoxIn(oldD, TheDialouge);

    }
    public bool Done()
    {
        if (Highlight != null)
            Highlight.SetActive(false);
        if (Highlight2 != null)
            Highlight2.SetActive(false);
        return true;
    }
    public void TileLifted(bool aValue)
    {
        if(aValue)
        {
            if (Highlight != null)
                Highlight.SetActive(false);
            if (Highlight2 != null)
                Highlight2.SetActive(false);
        }
        else
        {
            if (Highlight != null)
                Highlight.SetActive(true);
            if (Highlight2 != null)
                Highlight2.SetActive(true);
        }

    }
}
public class TutorialController : MonoBehaviour
{
    public List<TutorialAction> myActions= new List<TutorialAction>();
    public List<GameObject> myHighlights = new List<GameObject>();
    public List<GameObject> myHighlightsSecond = new List<GameObject>();

    public bool IsTutorial = false;
    public Text chatBoxLabel;
    public static TutorialController instance;
    public int CurrentIndex = 0;
    public GameObject DoneButton;
    public GameObject Character;
    public Vector3 _TextFlyInBoxoriginalPos;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        IsTutorial = true;

        _TextFlyInBoxoriginalPos = chatBoxLabel.transform.parent.position;
        myActions.Add(new TutorialAction("Hello!\nI am here to show you how to play!\nLet's get started!",0, myHighlights[0], myHighlightsSecond[0]));
        myActions.Add(new TutorialAction("Here are your tiles", 1, myHighlights[1], myHighlightsSecond[1]));
        myActions.Add(new TutorialAction("Let's try to place some of them", 1, myHighlights[1], myHighlightsSecond[1]));

        myActions.Add(new TutorialAction("A tile needs to be placed beside 2 other tiles", 2, myHighlights[2], myHighlightsSecond[2]));
        myActions.Add(new TutorialAction("You need to use + - / * to try and get the result to match you number.", 2, myHighlights[2], myHighlightsSecond[2]));
        myActions.Add(new TutorialAction("Let's try to place our number 3 on the board, 1+2=3 so let's place it here!", 3, myHighlights[3], myHighlightsSecond[3]));
        myActions.Add(new TutorialAction("Great work! Let's place our number 2 here! (4/2=2)", 4, myHighlights[4], myHighlightsSecond[4]));
        myActions.Add(new TutorialAction("Be sure to plan your moves as you can also use the tiles you just placed", 5, myHighlights[0], myHighlightsSecond[0]));
        myActions.Add(new TutorialAction("Let's use our previus numbers to put down number 6. This is a special tile that requires multiplication", 6, myHighlights[5], myHighlightsSecond[5]));
        myActions.Add(new TutorialAction("Greate! These special tiles will give you 10 extra points!", 7, myHighlights[6], myHighlightsSecond[6]));
        myActions.Add(new TutorialAction("The higher number you place the more points you get", 7, myHighlights[0], myHighlightsSecond[0]));
        // myActions.Add(new TutorialAction("3*4=12 and then 3*12 = 36", 7));
        myActions.Add(new TutorialAction("If you place all tiles you get BINGO and score 50 extra points!", 7, myHighlights[0], myHighlightsSecond[0]));

        myActions.Add(new TutorialAction("Let's press done and end our turn!", 8, myHighlights[7], myHighlightsSecond[7]));
        myActions.Add(new TutorialAction("", 9));

        myActions.Add(new TutorialAction("Oh no we got bad tiles, no problem! We can swap them.", 10,  myHighlights[8], myHighlightsSecond[8]));
        myActions.Add(new TutorialAction("When swapping you get new tiles from the pile but lose your turn.", 10, myHighlights[8], myHighlightsSecond[8]));
        myActions.Add(new TutorialAction("You are now ready to OutNumber your opponents!", 10, myHighlights[0], myHighlightsSecond[0]));
        myActions.Add(new TutorialAction("Enjoy the game and if you need any more help you can find the rules and this tutorial in the menu!", 10, myHighlights[0], myHighlightsSecond[0]));
        myActions.Add(new TutorialAction("", 11, myHighlights[0], myHighlightsSecond[0]));


        myActions[CurrentIndex].Set();





    }

    public void MoveBoxIn(string d1, string d2)
    {

        TutorialController.instance.chatBoxLabel.text = d1;
        // NewGameWindow.transform.GetChild(0).GetComponent<Image>().DOFade(157f / 255f, 0).SetEase(Ease.InOutQuart);

        //chatBoxLabel.transform.position = new Vector3(_TextFlyInBoxoriginalPos.x - 10, _TextFlyInBoxoriginalPos.y, _TextFlyInBoxoriginalPos.z);
        chatBoxLabel.transform.parent.DOMoveX(_TextFlyInBoxoriginalPos.x-10, 0.2f).SetEase(Ease.InOutQuart).OnComplete(() => {

            TutorialController.instance.chatBoxLabel.text = d2;
            chatBoxLabel.transform.parent.position = new Vector3(_TextFlyInBoxoriginalPos.x + 10, _TextFlyInBoxoriginalPos.y, _TextFlyInBoxoriginalPos.z);
            chatBoxLabel.transform.parent.DOMoveX(_TextFlyInBoxoriginalPos.x, 0.2f).SetEase(Ease.InOutQuart);

        });


    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TapToContinue()
    {
        if(myActions[CurrentIndex].Done())
        {
            CurrentIndex++;
            myActions[CurrentIndex].Set();
        }
    }

    public void BeginDragTile()
    {
        myActions[CurrentIndex].TileLifted(true);
    }
    public void EndDragTile()
    {
        myActions[CurrentIndex].TileLifted(false);
    }
}
