using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameListItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Text _NameOfChallenger;
    public Text _Score;

    public GameObject YourTurnGO;
    public GameObject OtherTurnGO;

    public GameObject DeleteButton;

    public BoardData bd = null;
    public bool _pressed = false;
    public float _pressTimer = 0;
    public bool _lockUntilRelease = false;
    public GameObject quit;

    public GameObject won;
    public GameObject lost;

    public bool isAiGame = false;

    RectTransform rc;
    // Start is called before the first frame update
    void Start()
    {
        rc = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_pressed)
        {
            _pressTimer += Time.deltaTime;
            if (_pressTimer > 1)
            {

                if(DeleteButton.activeSelf)
                {
                    GetComponent<Button>().enabled = true;
                    DeleteButton.SetActive(false);
                 
                    _lockUntilRelease = true;
                }
                else
                {
                    GetComponent<Button>().enabled = false;
                    DeleteButton.SetActive(true);
    
                    _lockUntilRelease = true;
                }

                _pressTimer = 0;
                _pressed = false;
            }
        }


        if( !System.Single.IsNaN(rc.localPosition.x) )
            rc.localPosition = new Vector3(rc.localPosition.x, rc.localPosition.y, 0);

    }
    public IEnumerator unlockClick( )
    {
        yield return new WaitForSeconds(0.1f);
        _lockUntilRelease = false;

    }
    public void OnPointerDown(PointerEventData eventData)
    {
        _pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        _pressed = false;
        StartCoroutine(unlockClick());
    }

    public void Init(BoardData aBd,bool hasFinished= false,bool aIsAiGame=false)
    {
        aBd.hasFinished = hasFinished;
        isAiGame = aIsAiGame;
        bd = aBd;
        string otherPlayerID = "";
        if ( Startup._instance.MyPlayfabID == bd.player1_PlayfabId)
        {
            _NameOfChallenger.text = bd.player2_displayName;
            _Score.text = bd.player1_score+" vs "+bd.player2_score;
            otherPlayerID = bd.player2_PlayfabId;
        }
        else
        {
            _NameOfChallenger.text = bd.player1_displayName;
            _Score.text = bd.player2_score + " vs " + bd.player1_score;
            otherPlayerID = bd.player1_PlayfabId;
        }

        

        if( bd.playerTurn == "0" && Startup._instance.MyPlayfabID == bd.player1_PlayfabId ||
            bd.playerTurn == "1" && Startup._instance.MyPlayfabID == bd.player2_PlayfabId ||
            isAiGame)
        {
            YourTurnGO.SetActive(true);
            OtherTurnGO.SetActive(false);
        }
        else
        {
            YourTurnGO.SetActive(false);
            OtherTurnGO.SetActive(true);
        }



        if (hasFinished)
        {
            if (aBd.player1_score == "")
                aBd.player1_score = "0";
            if (aBd.player2_score == "")
                aBd.player2_score = "0";

            if (aBd.player1_PlayfabId == Startup._instance.MyPlayfabID)
            {
                if (int.Parse(aBd.player1_score) > int.Parse(aBd.player2_score))
                {
                    won.SetActive(true);
                    lost.SetActive(false);
                }
                else
                {
                    won.SetActive(false);
                    lost.SetActive(true);
                }
            }
            else
            {
                if (int.Parse(aBd.player2_score) > int.Parse(aBd.player1_score))
                {
                    won.SetActive(true);
                    lost.SetActive(false);
                }
                else
                {
                    won.SetActive(false);
                    lost.SetActive(true);
                }
            }
            YourTurnGO.SetActive(false);
            OtherTurnGO.SetActive(false);

            if(aBd.player1_abandon=="1")
            {
                quit.SetActive(true);
                YourTurnGO.SetActive(false);
                OtherTurnGO.SetActive(false);
                won.SetActive(false);
                lost.SetActive(false);
                if(aBd.player1_PlayfabId == Startup._instance.MyPlayfabID)
                    quit.transform.GetChild(0).GetComponent<Text>().text = "YOU\nLEFT";
                else
                    quit.transform.GetChild(0).GetComponent<Text>().text = "PLAYER\nLEFT";
            }
            if (aBd.player2_abandon == "1")
            {
                quit.SetActive(true);
                YourTurnGO.SetActive(false);
                OtherTurnGO.SetActive(false);
                won.SetActive(false);
                lost.SetActive(false);
                if (aBd.player1_PlayfabId == Startup._instance.MyPlayfabID)
                    quit.transform.GetChild(0).GetComponent<Text>().text = "PLAYER\nLEFT";
                else
                    quit.transform.GetChild(0).GetComponent<Text>().text = "YOU\nLEFT";
            }
        }




        if(!isAiGame)
        {
            PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
            {
                PlayFabId = otherPlayerID,
                ProfileConstraints = new PlayerProfileViewConstraints()
                {
                    ShowDisplayName = true,
                    ShowAvatarUrl = true
                }
            }, result => {

                LoadAvatarURL(result.PlayerProfile.AvatarUrl);


            }, (error) => {
                Debug.Log("Got error retrieving user data:");
                Debug.Log(error.GenerateErrorReport());
            });
        }



        // Set pending and accept challenge text
        if(bd.player1_score == "0" && bd.player2_score=="0" && isAiGame == false &&( bd.player1_abandon == "0"|| bd.player1_abandon == "") && (bd.player2_abandon == "0"|| bd.player2_abandon == ""))
        {
            if (bd.playerTurn == "1" && Startup._instance.MyPlayfabID == bd.player1_PlayfabId)
            {
                OtherTurnGO.transform.GetChild(1).GetComponent<Text>().text = "PENDING";
            }
            else if (bd.playerTurn == "1" && Startup._instance.MyPlayfabID == bd.player2_PlayfabId)
            {
               // YourTurnGO.transform.GetChild(0).GetComponent<Text>().text = "ACCEPT \nCHALLENGE";
               // YourTurnGO.transform.GetChild(0).GetComponent<Text>().fontSize = 23;

                ChallengeWindow.instance.AddChallengeGame(this);
            }
            
        }
     



    }

    public void LoadAvatarURL(string aURL)
    {
        if(this != null)
        {
            if (aURL != null)
            {
                ProfilePictureManager.instance.SetPicture(aURL, img);
                img.enabled = true;
            }

            //StartCoroutine(GetFBProfilePicture(aURL, this));
        }

    }
    public Image img;
    //public static IEnumerator GetFBProfilePicture(string aURL, GameListItem chWin)
    //{

    //    //string url = "https" + "://graph.facebook.com/10159330728290589/picture";
    //    if (aURL != null)
    //    {
    //        WWW www = new WWW(aURL + "&access_token=GG|817150566351647|GXmlbSYVrHYJ1h7CJj7t9cGxwrE");
    //        yield return www;
    //        Texture2D profilePic = www.texture;


    //        chWin.img.sprite = Sprite.Create((Texture2D)profilePic, new Rect(0, 0, profilePic.height, profilePic.width), new Vector2());
    //        chWin.img.rectTransform.sizeDelta = new Vector2(88, 88);
    //        chWin.img.enabled = true;
    //    }


    //    //aSprite = Sprite.Create((Texture2D)profilePic, new Rect(0, 0, profilePic.height, profilePic.width), new Vector2());
    //    //img.rectTransform.sizeDelta = new Vector2(88, 88);
    //    //img.enabled = true;





    //}

    public void ClickLoadGame()
    {



        if (_lockUntilRelease)
            return;

        Debug.Log("Start Game!");
        Startup._instance.GameToLoad = bd;


        if(isAiGame)
         Startup._instance.GameToLoad = null;

        SceneManager.LoadScene(1);
        Startup._instance.PlaySoundEffect(1);
    }
    public void ClickDeleteGame()
    {
        if (_lockUntilRelease)
            return;

        if(isAiGame)
        {
            PlayerPrefs.DeleteKey("AIGame");
            PlayfabHelperFunctions.instance.Refresh();
        }
        else
        {
            Debug.Log("Delete game:" + bd.RoomName);
            PlayfabHelperFunctions.instance.DeleteGame(bd.RoomName);
        }



    }
}
