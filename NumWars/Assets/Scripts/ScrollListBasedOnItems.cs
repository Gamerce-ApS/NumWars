using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollListBasedOnItems : MonoBehaviour
{

    public Transform parent;
    public Image visualImage;
    public RectTransform rectTranform;

    public int startValue = 1300;
    public float dif = 160;
    public float updateTimer = -1;
    float timer = -1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(updateTimer != -1)
        {
            timer -= Time.deltaTime;
            if(timer<0)
            {
                RefreshLayout();
                timer = updateTimer;
            }
        }






    }
    public void RefreshLayout()
    {
        int amount = 0;
        for (int i = 0; i < parent.childCount;i++)
        {
            if (parent.GetChild(i).gameObject.active)
                amount++;
        }
        var newContentWidth = startValue + (dif) * amount;
        if(rectTranform !=null)
        rectTranform.sizeDelta = new Vector2(rectTranform.sizeDelta.x, newContentWidth);
        if(visualImage != null)
        visualImage.rectTransform.sizeDelta = new Vector2(visualImage.rectTransform.sizeDelta.x, rectTranform.sizeDelta.y - 1000);
    }
}
