using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AlertText : MonoBehaviour
{
    private static AlertText _instance;
    public static AlertText instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = ((GameObject)GameObject.Instantiate(Resources.Load("AlertText"), GameObject.Find("Canvas").transform)).GetComponent<AlertText>() ;
                _instance._TextFlyInBoxoriginalPosX = _instance.gameObject.transform.GetChild(0).position.x;
            }
               
            return _instance;
        }
    }
    float _TextFlyInBoxoriginalPosX;
    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ShowAlert(string text,float aDelay=0)
    {
        if (isFlyTextRunning == false)
        {
            gameObject.SetActive(true);
            
            StartCoroutine(FlyText(text,aDelay));
        }

    }

    public bool isFlyTextRunning = false;
    IEnumerator FlyText(string text, float aDelay)
    {
        isFlyTextRunning = true;

        gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        gameObject.transform.GetChild(0).GetComponent<Text>().text = "";

        yield return new WaitForSeconds(aDelay);

        gameObject.transform.GetChild(0).GetComponent<Text>().text = text;

        gameObject.GetComponent<Image>().DOFade(107f / 255f, 0.5f).SetEase(Ease.InOutQuart);


        gameObject.transform.GetChild(0).position = new Vector3(_TextFlyInBoxoriginalPosX - 10, gameObject.transform.GetChild(0).position.y, gameObject.transform.GetChild(0).position.z);
        gameObject.transform.GetChild(0).DOMoveX(_TextFlyInBoxoriginalPosX, 0.3f).SetEase(Ease.InOutQuart);

        yield return new WaitForSeconds(0.7f);


        gameObject.SetActive(true);
        gameObject.GetComponent<Image>().DOFade(0, 0.5f).SetEase(Ease.InOutQuart);
        gameObject.transform.GetChild(0).DOMoveX(_TextFlyInBoxoriginalPosX + 10, 0.3f).SetEase(Ease.InOutQuart);

        yield return new WaitForSeconds(0.4f);
        isFlyTextRunning = false;


    }
}
