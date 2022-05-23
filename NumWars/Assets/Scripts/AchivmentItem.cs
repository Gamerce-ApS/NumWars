using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchivmentItem : MonoBehaviour
{

    public Achivment myAchivment;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Claim()
    {
        
        for(int i = 0; i < AchivmentController.instance.myAchivments.Count;i++)
        {
            if( AchivmentController.instance.myAchivments[i].title == myAchivment.title &&
                AchivmentController.instance.myAchivments[i].description == myAchivment.description)
            {
                Startup._instance.AddXP(myAchivment.reward);
                AchivmentController.instance.myAchivments[i].isClaimed = true;
                AchivmentController.instance.UpdatePlayfab();


                gameObject.transform.GetChild(4).gameObject.SetActive(false);
                gameObject.transform.GetChild(5).gameObject.SetActive(true);
            }
        }
    }
}
