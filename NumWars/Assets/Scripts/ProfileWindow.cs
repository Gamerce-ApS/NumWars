using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileWindow : MonoBehaviour
{
    public Image AchivmenSlider;
    public Text amountCompletedText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnEnable()
    {
        int completed = 0;
        for (int i = 0; i < AchivmentController.instance.myAchivments.Count; i++)
        {
            if( AchivmentController.instance.myAchivments[i].current  >= AchivmentController.instance.myAchivments[i].target)
            completed++;
        }

        amountCompletedText.text = completed + "/" + AchivmentController.instance.myAchivments.Count;
        AchivmenSlider.fillAmount = (float)completed / (float)AchivmentController.instance.myAchivments.Count;

    }
}
