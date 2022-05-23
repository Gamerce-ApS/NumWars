using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchivmentWindow : MonoBehaviour
{

    public GameObject template;
    public Transform _parent;
    public Sprite Sprite_InProgress;
    public Sprite Sprite_Done;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    void OnEnable()
    {
        Debug.Log("Awake!");
        foreach (Transform child in _parent)
        {
            GameObject.Destroy(child.gameObject);
        }

        for(int i = 0; i< AchivmentController.instance.myAchivments.Count;i++)
        {
            Achivment current = AchivmentController.instance.myAchivments[i];

            GameObject go = GameObject.Instantiate(template, _parent);

            go.GetComponent<AchivmentItem>().myAchivment = current;

            go.SetActive(true);

            go.transform.GetChild(0).GetComponent<Text>().text = current.title;
            go.transform.GetChild(1).GetComponent<Text>().text = current.description;
            go.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = current.current + "/"+current.target;
            go.transform.GetChild(2).GetChild(0).GetComponent<Image>().fillAmount = (float)current.current / (float)current.target;
            if(current.current >= current.target)
                go.transform.GetChild(3).GetComponent<Image>().sprite = Sprite_Done;
            else
                go.transform.GetChild(3).GetComponent<Image>().sprite = Sprite_InProgress;

            if(current.isClaimed)
            {
                go.transform.GetChild(4).gameObject.SetActive(false);
                go.transform.GetChild(5).gameObject.SetActive(true);
            }
            else
            {
                if (current.current >= current.target)
                {
                    go.transform.GetChild(4).gameObject.SetActive(true);
                    go.transform.GetChild(5).gameObject.SetActive(false);
                }
                else
                {
                    go.transform.GetChild(4).gameObject.SetActive(false);
                    go.transform.GetChild(5).gameObject.SetActive(false);
                }

            }


        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
