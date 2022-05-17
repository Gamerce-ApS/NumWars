using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EmojiMap
{
    public string Name;
    public Sprite theSprite;
}

public class ChatWindow : MonoBehaviour
{
    public GameObject Window;

    public Vector3 _TextFlyInBoxoriginalPos;
    public static ChatWindow instance;


    public Transform _TextListParent;
    public GameObject TextPrefab;

    public GameObject TextPrefabLeft;
    public GameObject TextPrefabRight;


    public InputField chatBox;
    string currentChatData="";

    public List<EmojiMap> Emojis = new List<EmojiMap>();

    public GameObject _loadingGO;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        _TextFlyInBoxoriginalPos = Window.transform.GetChild(1).transform.position;

    }



    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenWindow()
    {
        Window.SetActive(true);
        Window.transform.GetChild(0).GetComponent<Image>().DOFade(157f / 255f, 0).SetEase(Ease.InOutQuart);

        Window.transform.GetChild(1).transform.position = new Vector3(_TextFlyInBoxoriginalPos.x - 10, _TextFlyInBoxoriginalPos.y, _TextFlyInBoxoriginalPos.z);
        Window.transform.GetChild(1).transform.DOMoveX(_TextFlyInBoxoriginalPos.x, 0.3f).SetEase(Ease.InOutQuart);



        _loadingGO.SetActive(true);

        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
        {
            PlayFabId = Startup._instance.GameToLoad.GetOtherPlayerPlayfab(),
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

    public void CloseWindow()
    {
        Window.transform.GetChild(0).GetComponent<Image>().DOFade(0, 157f / 255f).SetEase(Ease.InOutQuart);
        Window.transform.GetChild(1).transform.DOMoveX(_TextFlyInBoxoriginalPos.x + 10, 0.3f).SetEase(Ease.InOutQuart).OnComplete(() => { Window.SetActive(false); });
    }
    public void SendFixedMessage(string aMessage)
    {


        if (aMessage.Length>0)
        {
            if (currentChatData.Length > 0)
                currentChatData += "≤";


            currentChatData += Startup._instance.displayName+ "≤" + aMessage;
            //chatBox.text = "";
            PlayFabClientAPI.UpdateSharedGroupData(new UpdateSharedGroupDataRequest()
            {
                SharedGroupId = Startup._instance.GameToLoad.RoomName,
                Data = new Dictionary<string, string>() {
                        {"Chat", currentChatData }

            }
            },
        result3 =>
        {
            RequestChat();


        },
        error =>
        {
            Debug.Log("Got error making turn");
            Debug.Log(error.GenerateErrorReport());
            _loadingGO.SetActive(false);

        });


        }
    }
    public void RequestChat()
    {
        if (Startup._instance.GameToLoad == null)
            return;

        PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest()
        {
            SharedGroupId = Startup._instance.GameToLoad.RoomName
        }, result =>
        {
            if(result.Data.ContainsKey("Chat"))
            {
                currentChatData = result.Data["Chat"].Value;

                string[] messages = result.Data["Chat"].Value.Split('≤');

                LoadChat(messages);
                PlayerPrefs.SetString(Startup._instance.GameToLoad.RoomName + "_chat", result.Data["Chat"].Value);
            }
            else
                _loadingGO.SetActive(false);
            GameManager.instance.ChatNotificationIcon.SetActive(false);

        }, (error) =>
        {
            _loadingGO.SetActive(false);
            Debug.Log(error.GenerateErrorReport());
        });
    }
    public void LoadChat(string[] messages)
    {
        foreach (Transform child in _TextListParent)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < messages.Length;i+=2)
        {

            if(messages[i] == Startup._instance.displayName)
            {
                GameObject go = GameObject.Instantiate(TextPrefabLeft, _TextListParent);
                go.transform.GetChild(1).GetComponent<Text>().text = messages[i];
                go.transform.GetChild(2).GetComponent<Text>().text = messages[i + 1];

                if(me != null)
                {
                    Image img = go.transform.GetChild(0).GetChild(0).GetComponent<Image>();
                  //  img.sprite = Sprite.Create((Texture2D)me, new Rect(0, 0, me.height, me.width), new Vector2());
                    img.sprite = me.sprite;
                    //  img.rectTransform.sizeDelta = new Vector2(88, 88);
                    img.enabled = true;
                }
    

                if (messages[i + 1].Contains("<emoji"))
                {
              

       
                    go.transform.GetChild(3).gameObject.SetActive(true);
                    go.transform.GetChild(2).GetComponent<Text>().text = "";
                    go.transform.GetChild(3).GetComponent<Image>().sprite = GetEmoji(messages[i + 1]);

                }


            }
            else
            {
                GameObject go = GameObject.Instantiate(TextPrefabRight, _TextListParent);
                go.transform.GetChild(1).GetComponent<Text>().text = messages[i];
                go.transform.GetChild(2).GetComponent<Text>().text = messages[i + 1];

                if(otherP != null)
                {
                    Image img = go.transform.GetChild(0).GetChild(0).GetComponent<Image>();
                   // img.sprite = Sprite.Create((Texture2D)otherP, new Rect(0, 0, otherP.height, otherP.width), new Vector2());
                    img.sprite = otherP.sprite;
                    //img.rectTransform.sizeDelta = new Vector2(88, 88);
                    img.enabled = true;
                }
       

                if (messages[i + 1].Contains("<emoji"))
                {
       

                    go.transform.GetChild(3).gameObject.SetActive(true);
                    go.transform.GetChild(2).GetComponent<Text>().text = "";
                    go.transform.GetChild(3).GetComponent<Image>().sprite = GetEmoji(messages[i + 1]);

                }
            }


        }
        StartCoroutine(ScrollDown());
        chatBox.text = "";

        _loadingGO.SetActive(false);

    }
    public Sprite GetEmoji(string aName)
    {
        for(int i = 0; i < Emojis.Count;i++)
        {
            if( Emojis[i].Name == aName)
            {
                return Emojis[i].theSprite;
            }
        }
        return null;
    }
    private IEnumerator ScrollDown()
    {
        yield return new WaitForSeconds(0.25f);
        _TextListParent.transform.parent.parent.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 0);

    }
    public Image otherP;
    public Image me;


    public void LoadAvatarURL(string aURL)
    {

        ProfilePictureManager.instance.SetPicture(aURL, otherP, OnDoneCallback);

        //   StartCoroutine(GetFBProfilePicture(aURL, this));
    }
    public void OnDoneCallback()
    {
        ProfilePictureManager.instance.SetPicture(Startup._instance.avatarURL, me, OnDoneCallback2);

    }
    public void OnDoneCallback2()
    {
        RequestChat();
    }
    //public static IEnumerator GetFBProfilePicture(string aURL,ChatWindow chWin)
    //{

    //    //string url = "https" + "://graph.facebook.com/10159330728290589/picture";
    //    if(aURL != null)
    //    {
    //        WWW www = new WWW(aURL + "&access_token=GG|817150566351647|GXmlbSYVrHYJ1h7CJj7t9cGxwrE");
    //        yield return www;
    //        Texture2D profilePic = www.texture;
    //        chWin.otherP = www.texture;
    //    }


    //    WWW www2 = new WWW(Startup._instance.avatarURL + "&access_token=GG|817150566351647|GXmlbSYVrHYJ1h7CJj7t9cGxwrE");
    //    yield return www2;

    //    chWin.me = www2.texture;

    //    //aSprite = Sprite.Create((Texture2D)profilePic, new Rect(0, 0, profilePic.height, profilePic.width), new Vector2());
    //    //img.rectTransform.sizeDelta = new Vector2(88, 88);
    //    //img.enabled = true;



    //    chWin.RequestChat();

    //}
}
