using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollListBasedOnItems : MonoBehaviour
{

    public Transform parent;
    public Transform parent2;
    public Image visualImage;
    public RectTransform rectTranform;

    public int startValue = 1300;
    public float dif = 160;
    public float updateTimer = -1;
    float timer = -1;
    // Start is called before the first frame update
    void Start()
    {
        //Application.targetFrameRate = 60;
    }

    void OnEnable()
    {
        Reset();
    }
    public void Reset()
    {
        if (rectTranform != null)
            rectTranform.transform.localPosition = new Vector3(rectTranform.transform.localPosition.x, 0, rectTranform.transform.localPosition.z);

        //    RefreshLayout();



    }
    // Update is called once per frame
    void Update()
    {
        if(updateTimer != -1 && Input.GetMouseButton(0)== false)
        {
            timer -= Time.deltaTime;
            if(timer<0)
            {
                RefreshLayout();
                timer = updateTimer;
            }
        }

        //if(Input.GetMouseButton(0))
        //{
        //    Application.targetFrameRate = 60;
        //}else
        //    Application.targetFrameRate = 30;


        //float current = 0;
        //current = current = (int)(1f / Time.unscaledDeltaTime);
        //avgFrameRate = (int)current;
        //GameObject go = GameObject.Find("FPS_COUNTER");
        //if (go != null)
        //    go.GetComponent<Text>().text = avgFrameRate.ToString() + " FPS";

        if(float.IsNaN( rectTranform.localPosition.y) )
        {
            rectTranform.transform.localPosition = new Vector3(0, 0, 0);
            Startup.instance.Pasued(false);

        }
            



    }
    //int avgFrameRate = 0;
    public void RefreshLayout(float aTime)
    {
        StartCoroutine(RefreshLayoutIE(aTime));
    }
    IEnumerator RefreshLayoutIE(float aTime)
    {
        yield return new WaitForSeconds(aTime);
        RefreshLayout();
    }
    void OnGUI()
    {
     
        //    GUILayout.TextField(rectTranform.sizeDelta.ToString());
        //GUILayout.TextField(rectTranform.localPosition.ToString());



    }
    public void RefreshLayout()
    {
        int amount = 0;
        for (int i = 0; i < parent.childCount;i++)
        {
            if (parent.GetChild(i).gameObject.active)
                amount++;
        }
        if(amount == 0&& parent2 != null)
        {
            for (int i = 0; i < parent2.childCount; i++)
            {
                if (parent2.GetChild(i).gameObject.active)
                    amount++;
            }
        }

        var newContentWidth = startValue + (dif) * amount;
        if(rectTranform !=null)
        rectTranform.sizeDelta = new Vector2(rectTranform.sizeDelta.x, newContentWidth);
        if(visualImage != null)
        visualImage.rectTransform.sizeDelta = new Vector2(visualImage.rectTransform.sizeDelta.x, rectTranform.sizeDelta.y - 1000f);
    }
}
