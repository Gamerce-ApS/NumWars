﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileData
{
    public string URL;
    public Sprite theSprite;
    public string playfabID;
}

public class ProfilePictureManager : MonoBehaviour
{
    public List<ProfileData> myPictures = new List<ProfileData>();
    public static ProfilePictureManager instance;
    public Texture2D StandardPicture;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPicture(string url, string playfabID, Image aImage, System.Action callback=null)
    {
    //    Debug.Log(playfabID);
        if (url.Length<1 || url =="")
        {
            //aImage.sprite = 
           // aImage.rectTransform.sizeDelta = new Vector2(88, 88);

            ProfileData pf = new ProfileData();
            pf.URL = url;
            pf.theSprite = Sprite.Create((Texture2D)StandardPicture, new Rect(0, 0, StandardPicture.width, StandardPicture.height), new Vector2());
            pf.playfabID = playfabID;
            ProfilePictureManager.instance.myPictures.Add(pf);


            if (callback != null)
                callback.Invoke();
            return;
        }
        for(int i = 0; i < myPictures.Count;i++)
        {
            if (myPictures[i].URL == url && myPictures[i].playfabID == playfabID)
            {
                if (aImage == null || aImage.rectTransform == null)
                    return;

                aImage.sprite = myPictures[i].theSprite;
                aImage.rectTransform.sizeDelta = new Vector2(88, 88);
                if(callback != null)
                callback.Invoke();
                return;
            }

                
        }
        PlayfabCallbackHandler.instance.SetPictureIE(url, playfabID, aImage, callback, null);
       // StartCoroutine(SetPictureIE(url, playfabID, aImage, callback));
    }
    public bool HasEntry(string aPlayfabID)
    {
        for (int i = 0; i < myPictures.Count; i++)
        {
            if (myPictures[i].playfabID == aPlayfabID)
                return true;
        }
        return false;
    }
    public string GetURL(string aPlayfabID)
    {
        for (int i = 0; i < myPictures.Count; i++)
        {
            if (myPictures[i].playfabID == aPlayfabID)
                return myPictures[i].URL;
        }
        return "";
    }
    //public void SetPicture(string url, string playfabID, Texture2D aImage, System.Action callback = null)
    //{

    //    for (int i = 0; i < myPictures.Count; i++)
    //    {
    //        if (myPictures[i].URL == url)
    //        {
    //            aImage = myPictures[i].theSprite.texture;
    //          //  aImage.rectTransform.sizeDelta = new Vector2(88, 88);
    //            if (callback != null)
    //                callback.Invoke();
    //            return;
    //        }


    //    }
    //    StartCoroutine(SetPictureIE(url, playfabID, aImage, callback));
    //}

    private IEnumerator SetPictureIE(string aURL,string playfabID, Image aImage, System.Action callback = null)
    {

        WWW www = new WWW(aURL + "&access_token=GG|817150566351647|GXmlbSYVrHYJ1h7CJj7t9cGxwrE");
        yield return www;

        if (playfabID == "54209EA097616244")
            Debug.Log(playfabID);

        Texture2D profilePic = www.texture;
        if (www.error != null)
            profilePic = StandardPicture;
        //If texture is null add a standard one.. This is for facebook error
        ProfileData pf = new ProfileData();
        pf.URL = aURL;
        pf.theSprite = Sprite.Create((Texture2D)profilePic, new Rect(0, 0, profilePic.width, profilePic.height), new Vector2());
        pf.playfabID = playfabID;
        myPictures.Add(pf);
        if (aImage != null && pf != null)
        {
            aImage.sprite = pf.theSprite;
            aImage.rectTransform.sizeDelta = new Vector2(88, 88);

            if (callback != null)
                callback.Invoke();
        }












    }
//    private IEnumerator SetPictureIE(string aURL, string playfabID, Texture2D aImage, System.Action callback = null)
//    {
//        WWW www = new WWW(aURL + "&access_token=GG|817150566351647|GXmlbSYVrHYJ1h7CJj7t9cGxwrE");
//        yield return www;
//        Texture2D profilePic = www.texture;
//        if (www.error != null)
//            profilePic = StandardPicture;
//        ProfileData pf = new ProfileData();
//        pf.URL = aURL;
//        pf.theSprite = Sprite.Create((Texture2D)profilePic, new Rect(0, 0, profilePic.width, profilePic.height), new Vector2());
//        pf.playfabID = playfabID;
//        myPictures.Add(pf);
//        aImage = pf.theSprite.texture;
////        aImage.rectTransform.sizeDelta = new Vector2(88, 88);

//        if (callback != null)
//            callback.Invoke();
//    }
}
