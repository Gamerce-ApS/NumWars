﻿using System;
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
    public GameListItem OnPictureCallback = null;

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
                {
                    quit.GetComponent<Image>().color = new Color(160f/255f,1, 155f / 255f, 1);
                    quit.transform.GetChild(0).GetComponent<Text>().text = "PLAYER\nLEFT";
                }

            }
            if (aBd.player2_abandon == "1")
            {
                quit.SetActive(true);
                YourTurnGO.SetActive(false);
                OtherTurnGO.SetActive(false);
                won.SetActive(false);
                lost.SetActive(false);
                if (aBd.player1_PlayfabId == Startup._instance.MyPlayfabID)
                {
                    quit.GetComponent<Image>().color = new Color(160f / 255f, 1, 155f / 255f, 1);
                    quit.transform.GetChild(0).GetComponent<Text>().text = "PLAYER\nLEFT";
                }

                else
                    quit.transform.GetChild(0).GetComponent<Text>().text = "YOU\nLEFT";
            }
        }




        if(!isAiGame)
        {
            if(ProfilePictureManager.instance.HasEntry(otherPlayerID))
            {
                LoadAvatarURL(ProfilePictureManager.instance.GetURL(otherPlayerID), otherPlayerID);
            }
            else
            {
                //PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
                //{
                //    PlayFabId = otherPlayerID,
                //    ProfileConstraints = new PlayerProfileViewConstraints()
                //    {
                //        ShowDisplayName = true,
                //        ShowAvatarUrl = true
                //    }
                //}, result => {

                //    LoadAvatarURL(result.PlayerProfile.AvatarUrl, otherPlayerID);


                //}, (error) => {
                //    Debug.Log("Got error retrieving user data:");
                //    Debug.Log(error.GenerateErrorReport());
                //});




                //if (Startup.instance.StoredAvatarURLS.ContainsKey(otherPlayerID))
                //{
                //    object outV = "";
                //    if (Startup.instance.StoredAvatarURLS.TryGetValue(otherPlayerID, out outV))
                //    {
                //        string t = outV.ToString();
                //        LoadAvatarURL(t, otherPlayerID);
                //    }


                //}

                StartCoroutine(PicCheck(otherPlayerID));


                //PlayfabCallbackHandler.instance.GetOtherPlayerProfile(otherPlayerID, result2 =>
                //{
                //    LoadAvatarURL(result2.PlayerProfile.AvatarUrl, otherPlayerID);

                //},
                //    error =>
                //    {
                //        Debug.Log("error getting player info");
                //    });

            }
 
        }



        // Set pending and accept challenge text
        if(bd.player1_score == "0" && bd.player2_score=="0" && isAiGame == false &&( bd.player1_abandon == "0"|| bd.player1_abandon == "") && (bd.player2_abandon == "0"|| bd.player2_abandon == ""))
        {
            if (bd.playerTurn == "1" && Startup._instance.MyPlayfabID == bd.player1_PlayfabId)
            {
                OtherTurnGO.transform.GetChild(1).GetComponent<Text>().text = "PENDING";
                PlayfabHelperFunctions.instance.CheckIfGameIsInList(bd.player1_PlayfabId, bd.player2_PlayfabId, bd.RoomName);
            }
            else if (bd.playerTurn == "0" && Startup._instance.MyPlayfabID == bd.player1_PlayfabId)
            {
                TimeSpan diff = DateTime.Now - TimeForCheck;
                if (diff.TotalMinutes >30)
                {
                    PlayfabHelperFunctions.instance.CheckIfGameIsInList(bd.player1_PlayfabId, bd.player2_PlayfabId, bd.RoomName);
                    TimeForCheck = DateTime.Now;

                }
            }
            else if (bd.playerTurn == "1" && Startup._instance.MyPlayfabID == bd.player2_PlayfabId)
            {
               // YourTurnGO.transform.GetChild(0).GetComponent<Text>().text = "ACCEPT \nCHALLENGE";
               // YourTurnGO.transform.GetChild(0).GetComponent<Text>().fontSize = 23;

                ChallengeWindow.instance.AddChallengeGame(this);
            }
            
        }
     



    }
    public DateTime TimeForCheck = DateTime.MinValue;
    public IEnumerator PicCheck(string otherPlayerID)
    {
        yield return new WaitForSeconds(0.1f);

        if (Startup.instance == null || Startup.instance.StoredAvatarURLS == null)
            yield break;

        if (Startup.instance.StoredAvatarURLS.ContainsKey(otherPlayerID))
        {
            string outV = "";
            if (Startup.instance.StoredAvatarURLS.TryGetValue(otherPlayerID, out outV))
            {
                if (outV == null)
                    yield break;
                try
                {
                    string t = outV;
                    LoadAvatarURL(t, otherPlayerID);
                }
                catch
                {

                }

            }


        }

    }
    public void LoadAvatarURL(string aURL,string playfabID)
    {
       // aURL is null so we need to know that so we dont get info each time.
       //     add null entry for user in myprofiles

        if(this != null)
        {
            if (aURL != null)
            {
                if(OnPictureCallback != null)
                    ProfilePictureManager.instance.SetPicture(aURL, playfabID, OnPictureCallback.img);
                else
                    ProfilePictureManager.instance.SetPicture(aURL, playfabID, img);

                img.enabled = true;
            }
            else
            {
                ProfileData pf = new ProfileData();
                pf.URL = aURL;
                pf.theSprite = Sprite.Create((Texture2D)ProfilePictureManager.instance.StandardPicture, new Rect(0, 0, ProfilePictureManager.instance.StandardPicture.width, ProfilePictureManager.instance.StandardPicture.height), new Vector2());
                pf.playfabID = playfabID;
                ProfilePictureManager.instance.myPictures.Add(pf);
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
