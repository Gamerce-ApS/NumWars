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

        RequestChat();
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


        });


        }
    }
    public void RequestChat()
    {
        
        PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest()
        {
            SharedGroupId = Startup._instance.GameToLoad.RoomName
        }, result =>
        {
            currentChatData = result.Data["Chat"].Value;

            string[] messages = result.Data["Chat"].Value.Split('≤');

            LoadChat(messages);
            PlayerPrefs.SetString(Startup._instance.GameToLoad.RoomName + "_chat", result.Data["Chat"].Value);

        }, (error) =>
        {
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
                if(messages[i + 1].Contains("<emoji"))
                {
                    go.transform.GetChild(3).gameObject.SetActive(true);
                    go.transform.GetChild(2).GetComponent<Text>().text = "";
                    go.transform.GetChild(3).GetComponent<Image>().sprite = GetEmoji(messages[i + 1]);

                }
            }


        }
        StartCoroutine(ScrollDown());
        chatBox.text = "";


        
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
}
