using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileData
{
    public string URL;
    public Sprite theSprite;
}

public class ProfilePictureManager : MonoBehaviour
{
    public List<ProfileData> myPictures = new List<ProfileData>();
    public static ProfilePictureManager instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPicture(string url,Image aImage, System.Action callback=null)
    {
        for(int i = 0; i < myPictures.Count;i++)
        {
            if (myPictures[i].URL == url)
            {
                aImage.sprite = myPictures[i].theSprite;
                aImage.rectTransform.sizeDelta = new Vector2(88, 88);
                if(callback != null)
                callback.Invoke();
                return;
            }

                
        }
        StartCoroutine(SetPictureIE(url, aImage, callback));
    }
    public void SetPicture(string url, Texture2D aImage, System.Action callback = null)
    {
        for (int i = 0; i < myPictures.Count; i++)
        {
            if (myPictures[i].URL == url)
            {
                aImage = myPictures[i].theSprite.texture;
              //  aImage.rectTransform.sizeDelta = new Vector2(88, 88);
                if (callback != null)
                    callback.Invoke();
                return;
            }


        }
        StartCoroutine(SetPictureIE(url, aImage, callback));
    }

    private IEnumerator SetPictureIE(string aURL, Image aImage, System.Action callback = null)
    {
        WWW www = new WWW(aURL + "&access_token=GG|817150566351647|GXmlbSYVrHYJ1h7CJj7t9cGxwrE");
        yield return www;
        Texture2D profilePic = www.texture;
        ProfileData pf = new ProfileData();
        pf.URL = aURL;
        pf.theSprite = Sprite.Create((Texture2D)profilePic, new Rect(0, 0, profilePic.height, profilePic.width), new Vector2());
        myPictures.Add(pf);
        aImage.sprite = pf.theSprite;
        aImage.rectTransform.sizeDelta = new Vector2(88, 88);

        if (callback != null)
            callback.Invoke();
    }
    private IEnumerator SetPictureIE(string aURL, Texture2D aImage, System.Action callback = null)
    {
        WWW www = new WWW(aURL + "&access_token=GG|817150566351647|GXmlbSYVrHYJ1h7CJj7t9cGxwrE");
        yield return www;
        Texture2D profilePic = www.texture;
        ProfileData pf = new ProfileData();
        pf.URL = aURL;
        pf.theSprite = Sprite.Create((Texture2D)profilePic, new Rect(0, 0, profilePic.height, profilePic.width), new Vector2());
        myPictures.Add(pf);
        aImage = pf.theSprite.texture;
//        aImage.rectTransform.sizeDelta = new Vector2(88, 88);

        if (callback != null)
            callback.Invoke();
    }
}
