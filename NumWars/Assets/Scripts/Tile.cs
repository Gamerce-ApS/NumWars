using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tile : MonoBehaviour,  IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{

    PointerEventData m_PointerEventData;
    [SerializeField] EventSystem m_EventSystem;
    [SerializeField] RectTransform canvasRect;

    public Text textLabel;

    Vector3 lastFramePos;


    public Sprite Green;
    public Sprite Red;
    public Sprite Normal;
    public Sprite White;

    Vector3 _BoardPosition;
    Vector3 _OriginalPosition;
    public Vector3 _targetScale = Vector3.one;

    public StaticTile PlacedOnTile = null;
    public Transform TileParent;

    public TileStatus myTileStatus = TileStatus.InBottomBar;
    public enum TileStatus
    {
        InBottomBar,
        Dragging,
        PlacingOnBoard,
        OnBoard,
    }

    // Start is called before the first frame update
    public void Init(int aNumber)
    {
        _OriginalPosition = transform.position;
        _BoardPosition = transform.position;

        textLabel.text = aNumber.ToString();

        textLabel.fontSize = 75;
    }
    Quaternion _targetRotation;

    // Update is called once per frame
    void Update()
    {
        _refreshTimer -= Time.deltaTime;
        if (_refreshTimer < -1)
            _refreshTimer = 0.05f;


        _targetRotation = Quaternion.Lerp(_targetRotation, Quaternion.identity, Time.deltaTime * 15);

        dragObjectInternal.rotation = Quaternion.Lerp(dragObjectInternal.rotation, _targetRotation, Time.deltaTime*4) ;


        if(myTileStatus == TileStatus.PlacingOnBoard)
        {
            float dist = 1;
            float speed = 3;
            if (GameManager.instance.CurrentTurn == 1)
            {
                speed = 10;
                dist = 0.03f;
            }
    

            transform.position = Vector3.Lerp(transform.position, _OriginalPosition, Time.deltaTime * speed);

  
            if (Vector3.Distance(transform.position,_OriginalPosition) < dist)
            {
                myTileStatus = TileStatus.OnBoard;
                transform.position = _OriginalPosition;
            }
        }

        transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, Time.deltaTime * 20);
        // DG.Tweening.DOTween.Shake(transform.position,)

        if(GameManager.instance.CurrentTurn == 0)
        {
            if (myTileStatus == TileStatus.OnBoard)
            {
                if (PlacedOnTile != null)
                {
                    transform.position = PlacedOnTile.transform.position;
                    transform.localScale = new Vector3(0.5f * Board.instance.GetScaleDif(), 0.5f * Board.instance.GetScaleDif(), 0.5f * Board.instance.GetScaleDif());
                    transform.parent = PlayerBoard.instance.MaskedParent.transform;
                }
                else
                {
                    if (transform.parent != TileParent)
                        transform.SetParent( TileParent);

                }

            }
            else
            {
                if(transform.parent != TileParent)
                transform.SetParent(TileParent);
            }
        }
     


    }
    public string GetBoardPosition()
    {
        return PlacedOnTile.GetBoardPosition().ToString();
    }
    public string GetValue()
    {
        if(PlacedOnTile != null)
        {

            int scoreV = int.Parse(textLabel.text) ;


           return (PlacedOnTile.GetScoreMultiplier(scoreV) * GetMultipleValidPositions()).ToString();


        }
        return textLabel.text;
    }
    public int GetMultipleValidPositions()
    {
        int amountValid = Board.instance.CheckAmountValid(PlacedOnTile, textLabel.text);
        return amountValid;
    }
    public void SetWhiteTile()
    {
        GetComponent<Image>().sprite = White;
        textLabel.color = new Color(98f / 255f, 60f / 255f, 95f / 255f);

        Color col;
        ColorUtility.TryParseHtmlString("#FFFE67", out col);
        GetComponent<Image>().color = col;
    }

    public RectTransform dragObject;
    public RectTransform dragArea;
    private Vector2 originalLocalPointerPosition;
    private Vector3 originalPanelLocalPosition;
    private RectTransform dragObjectInternal
    {
        get
        {
            if (dragObject == null)
                return (transform as RectTransform);
            else
                return dragObject;
        }
    }
    private RectTransform dragAreaInternal
    {
        get
        {
            if (dragArea == null)
            {
                RectTransform canvas = transform as RectTransform;
                while (canvas.parent != null && canvas.parent is RectTransform)
                {
                    canvas = canvas.parent as RectTransform;
                }
                return canvas;
            }
            else
                return dragArea;
        }
    }
    public void OnBeginDrag(PointerEventData data)
    {
        if (SwapScreen.instance.isOpen)
            return;
        if (GameManager.instance.CheckIfMyTurn() == false)
            return;

        Startup._instance.PlaySoundEffect(0);

        myTileStatus = TileStatus.Dragging;

        originalPanelLocalPosition = dragObjectInternal.localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(dragAreaInternal, data.position, data.pressEventCamera, out originalLocalPointerPosition);
        _targetScale = Vector3.one;
        _targetScale = new Vector3(1.5f, 1.5f, 1.5f);

        transform.SetAsLastSibling();
        if(PlacedOnTile != null)
            PlacedOnTile.SetValue(0);
        PlacedOnTile = null;
        PlayerBoard.instance.UpdateAllTiles();

        if(TutorialController.instance != null)
        TutorialController.instance.BeginDragTile();
    }
    public void UpdateTile()
    {


        if (PlacedOnTile == null)
            return;

        if (isValidPlaced())
        {
            GetComponent<Image>().sprite = Green;
            PlacedOnTile.SetValue(int.Parse(textLabel.text));
        }
        else
        {
            GetComponent<Image>().sprite = Red;
            PlacedOnTile.SetValue(0);
        }



        textLabel.color = new Color(98f / 255f, 60f / 255f, 95f / 255f);
    }
    public void ResetStartPosition()
    {
        _OriginalPosition = transform.position;
        _BoardPosition = transform.position;

    }
    public void Flip()
    {
        if(isValidPlaced())
        {



            GetComponent<Animator>().Play("tileflip");



            StartCoroutine( DestroyAfterTime() );




          //  GameObject.Instantiate(Board.instance.ScoreEffect, transform.position, Quaternion.identity, transform.parent);
        }
    }
    IEnumerator DestroyAfterTime()
    {


        yield return new WaitForSeconds(1f);
        PlacedOnTile.SetTile(StaticTile.TileType.NormalTile, int.Parse(textLabel.text));

     //   PlacedOnTile.transform.GetChild(0).GetComponent<Image>().color = col;


        GameManager.instance.thePlayers[0].myTiles.Remove(this);
        GameManager.instance.thePlayers[1].myTiles.Remove(this);
        Destroy(gameObject);

    }
    public bool isValidPlaced()
    {
        if (PlacedOnTile == null)
            return false;
        return Board.instance.CheckValid(PlacedOnTile, textLabel.text);
    }
    public void OnEndDrag(PointerEventData data)
    {
        if (SwapScreen.instance.isOpen)
            return;
        if (GameManager.instance.CheckIfMyTurn() == false)
            return;

        Startup._instance.PlaySoundEffect(0);

        GetComponent<Image>().color = new Color(1, 1, 1, 1);
        Debug.Log("EndDrag");
        bool allowTutorialPlace = true;
        if (TutorialController.instance != null)
        {
            
            if (TutorialController.instance.myActions[TutorialController.instance.CurrentIndex].ID == 3)
            {
                if (PlacedOnTile != null && PlacedOnTile.GetBoardPosition().x == 8 && PlacedOnTile.GetBoardPosition().y == 7)
                {
                    allowTutorialPlace = true;
                    TutorialController.instance.TapToContinue();
                }
                else
                    allowTutorialPlace = false;

            }
            else if (TutorialController.instance.myActions[TutorialController.instance.CurrentIndex].ID == 4)
            {
                if (PlacedOnTile != null && PlacedOnTile.GetBoardPosition().x == 7 && PlacedOnTile.GetBoardPosition().y == 8)
                {
                    allowTutorialPlace = true;
                    TutorialController.instance.TapToContinue();
                }
                else
                    allowTutorialPlace = false;

            }
            else if (TutorialController.instance.myActions[TutorialController.instance.CurrentIndex].ID == 6)
            {
                if (PlacedOnTile.GetBoardPosition().x == 9 && PlacedOnTile.GetBoardPosition().y == 7)
                {
                    allowTutorialPlace = true;
                    TutorialController.instance.TapToContinue();
                }
                else
                    allowTutorialPlace = false;

            }
        }


        if (Board.instance.Selection.GetComponent<Image>().enabled && allowTutorialPlace)
        {
            PlaceTileOnSelection();

            Board.instance.Selection.transform.GetChild(0).gameObject.SetActive(false);
            Board.instance.Selection.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            transform.position = _BoardPosition;
            dragObjectInternal.rotation = Quaternion.identity;
            _targetScale = Vector3.one;

            textLabel.fontSize = 75;
            PlacedOnTile = null;
            myTileStatus = TileStatus.InBottomBar;
        }



        Board.instance.Selection.GetComponent<Image>().enabled = false;

        _targetRotation = Quaternion.Euler(0, 0, 0);

        PlayerBoard.instance.UpdateAllTiles();
        if (TutorialController.instance != null)
            TutorialController.instance.EndDragTile();



    }
    public void PlaceTileOnSelection()
    {


        myTileStatus = TileStatus.PlacingOnBoard;
        _OriginalPosition = Board.instance.Selection.transform.position;

        _targetScale = new Vector3(0.5f, 0.5f, 0.5f);

        dragObjectInternal.rotation = Quaternion.identity;


        if( Board.instance.CheckValid(PlacedOnTile,textLabel.text) )
        {
            GetComponent<Image>().sprite = Green;
            PlacedOnTile.SetValue(int.Parse(textLabel.text));
        }
        else
        {
            GetComponent<Image>().sprite = Red;
            PlacedOnTile.SetValue(0);
        }

        textLabel.fontSize = 83;

        textLabel.color = new Color(  98f/ 255f, 60f/ 255f, 95f/ 255f);

    }
    float _refreshTimer = 0;
    public void OnDrag(PointerEventData data)
    {
        if (SwapScreen.instance.isOpen)
            return;
        if (GameManager.instance.CheckIfMyTurn() == false)
            return;

        GetComponent<Image>().sprite = Normal;
        textLabel.color = new Color(1,1,1,1);

        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(dragAreaInternal, data.position, data.pressEventCamera, out localPointerPosition))
        {
            //Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;

            //dragObjectInternal.localPosition = originalPanelLocalPosition + offsetToOriginal;

            Vector3 screenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            screenPos.z = dragObjectInternal.position.z;
            dragObjectInternal.position = screenPos;
        }

      //  ClampToArea();





        if(_refreshTimer<0)
        {
            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the game object


            //  m_PointerEventData.position = Camera.main.WorldToScreenPoint(transform.position) - new Vector3(dragObjectInternal.sizeDelta.x/2, -dragObjectInternal.sizeDelta.y/2,0);

            m_PointerEventData.position = Camera.main.WorldToScreenPoint(transform.position) + new Vector3(0, (dragObjectInternal.sizeDelta.y * 0.1f), 0);
            // GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            EventSystem.current.RaycastAll(m_PointerEventData, results);
            if (results.Count > 0)
            {
                bool didHitTile = false;
                for (int i = 0; i < results.Count; i++)
                {
                    if (results[i].gameObject.name.Contains("TileEmtpy"))
                    {
                        StaticTile theTile = results[i].gameObject.GetComponent<StaticTile>();

                      
                        if (theTile.myTileType != StaticTile.TileType.StartTile && theTile.myTileType != StaticTile.TileType.NormalTile && CheckIfHasTile(theTile) == false)
                        {
                            Board.instance.Selection.transform.GetComponent<RectTransform>().position = results[i].gameObject.GetComponent<RectTransform>().position;
                            Board.instance.Selection.GetComponent<Image>().enabled = true;
                            didHitTile = true;
                            PlacedOnTile = theTile;
                        }
     
                    }
                }
                if(didHitTile == false)
                {
                    StaticTile t = GetClosestAvalibleTile();
                    if(t != null)
                    {
                        Board.instance.Selection.transform.GetComponent<RectTransform>().position = t.gameObject.GetComponent<RectTransform>().position;
                        Board.instance.Selection.GetComponent<Image>().enabled = true;
                        didHitTile = true;
                        PlacedOnTile = t;
                    }
                    else
                    {
                        Board.instance.Selection.GetComponent<Image>().enabled = false;
                        PlacedOnTile = null;
                    }



                }

            }
            _refreshTimer = 0.05f;
        }


        float moveD = (lastFramePos.x - dragObjectInternal.localPosition.x);

        float maxD = 15;
        if (moveD > maxD)
            moveD = maxD;
        if (moveD < -maxD)
            moveD = -maxD;


        _targetRotation = Quaternion.Euler(0,0, _targetRotation.eulerAngles.z+ moveD);


            lastFramePos = dragObjectInternal.localPosition;
    }
    public bool CheckIfHasTile(StaticTile aTile)
    {
        for (int j = 0; j < PlayerBoard.instance.myPlayer.myTiles.Count; j++)
        {
            if (PlayerBoard.instance.myPlayer.myTiles[j].PlacedOnTile == aTile && PlayerBoard.instance.myPlayer.myTiles[j] != this)
            {
                return true;
            }
        }
        return false;
    }
    public StaticTile GetClosestAvalibleTile()
    {
        float closestDist = 1.5f;
        StaticTile reutrnTile = null;
        for (int j = 0; j < 10; j++)
        {

            StaticTile hit1 = DoRaycast(new Vector3(1*j, 0, 0));
            StaticTile hit2 = DoRaycast(new Vector3(1*j, 1 * j, 0));
            StaticTile hit3 = DoRaycast(new Vector3(0, 1 * j, 0));
            StaticTile hit4 = DoRaycast(new Vector3(-1 * j, 1 * j, 0));
            StaticTile hit5 = DoRaycast(new Vector3(-1 * j, 0, 0));
            StaticTile hit6 = DoRaycast(new Vector3(-1 * j, -1 * j, 0));
            StaticTile hit7 = DoRaycast(new Vector3(0, -1 * j, 0));
            StaticTile hit8 = DoRaycast(new Vector3(1 * j, -1 * j, 0));

   
            if (hit1 != null)
            {
                if (CheckIfHasTile(hit1) == false && Vector3.Distance(hit1.transform.position, transform.position) < closestDist)
                {
                    closestDist = Vector3.Distance(hit1.transform.position, transform.position);
                    reutrnTile = hit1;
                }
            }
            if (hit2 != null)
            {
                if (CheckIfHasTile(hit2) == false && Vector3.Distance(hit2.transform.position, transform.position) < closestDist)
                {
                    closestDist = Vector3.Distance(hit2.transform.position, transform.position);
                    reutrnTile = hit2;
                }
            }
            if (hit3 != null)
            {
                if (CheckIfHasTile(hit3) == false && Vector3.Distance(hit3.transform.position, transform.position) < closestDist)
                {
                    closestDist = Vector3.Distance(hit3.transform.position, transform.position);
                    reutrnTile = hit3;
                }
            }
            if (hit4 != null)
            {
                if (CheckIfHasTile(hit4) == false && Vector3.Distance(hit4.transform.position, transform.position) < closestDist)
                {
                    closestDist = Vector3.Distance(hit4.transform.position, transform.position);
                    reutrnTile = hit4;
                }
            }
            if (hit5 != null)
            {
                if (CheckIfHasTile(hit5) == false && Vector3.Distance(hit5.transform.position, transform.position) < closestDist)
                {
                    closestDist = Vector3.Distance(hit5.transform.position, transform.position);
                    reutrnTile = hit5;
                }
            }
            if (hit6 != null)
            {
                if (CheckIfHasTile(hit6) == false && Vector3.Distance(hit6.transform.position, transform.position) < closestDist)
                {
                    closestDist = Vector3.Distance(hit6.transform.position, transform.position);
                    reutrnTile = hit6;
                }
            }
            if (hit7 != null)
            {
                if (CheckIfHasTile(hit7) == false && Vector3.Distance(hit7.transform.position, transform.position) < closestDist)
                {
                    closestDist = Vector3.Distance(hit7.transform.position, transform.position);
                    reutrnTile = hit7;
                }
            }
            if (hit8 != null)
            {
                if (CheckIfHasTile(hit8) == false && Vector3.Distance(hit8.transform.position, transform.position) < closestDist)
                {
                    closestDist = Vector3.Distance(hit8.transform.position, transform.position);
                    reutrnTile = hit8;
                }
            }





        }
        return reutrnTile;
        return null;


    }
    public StaticTile DoRaycast(Vector3 offset)
    {
        PointerEventData m_PointerEventData2 = new PointerEventData(m_EventSystem);
        m_PointerEventData2.position = Camera.main.WorldToScreenPoint(transform.position) + new Vector3(dragObjectInternal.sizeDelta.x/2* offset.x, (dragObjectInternal.sizeDelta.y/2* offset.y), 0);
        List<RaycastResult> resultsClose = new List<RaycastResult>();
        EventSystem.current.RaycastAll(m_PointerEventData2, resultsClose);
        if (resultsClose.Count > 0)
        {

            for (int i = 0; i < resultsClose.Count; i++)
            {
                if (resultsClose[i].gameObject.name.Contains("TileEmtpy"))
                {
                    StaticTile theTile = resultsClose[i].gameObject.GetComponent<StaticTile>();

                    if (theTile.myTileType != StaticTile.TileType.StartTile && theTile.myTileType != StaticTile.TileType.NormalTile)
                    {
                        return theTile;
                    }

                }
            }
        }
        return null;
    }
    private void ClampToArea()
    {
        Vector3 pos = dragObjectInternal.localPosition;

        Vector3 minPosition = dragAreaInternal.rect.min - dragObjectInternal.rect.min;
        Vector3 maxPosition = dragAreaInternal.rect.max - dragObjectInternal.rect.max;

        pos.x = Mathf.Clamp(dragObjectInternal.localPosition.x, minPosition.x, maxPosition.x);
        pos.y = Mathf.Clamp(dragObjectInternal.localPosition.y, minPosition.y, maxPosition.y);

        dragObjectInternal.localPosition = pos;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        SwapScreen.instance.ClickedTile(this);

    }

    public void ReturnFromBoard()
    {
        if(PlacedOnTile != null)
        {



            PlacedOnTile.SetValue(0);
            PlacedOnTile = null;


   


            transform.position = _BoardPosition;
            dragObjectInternal.rotation = Quaternion.identity;
            _targetScale = Vector3.one;

            textLabel.fontSize = 75;
            Board.instance.Selection.GetComponent<Image>().enabled = false;
            _targetRotation = Quaternion.Euler(0, 0, 0);
            PlayerBoard.instance.UpdateAllTiles();
            GetComponent<Image>().sprite = Normal;
            textLabel.color = new Color(1, 1, 1, 1);

            myTileStatus = TileStatus.InBottomBar;
        }

    }
}
