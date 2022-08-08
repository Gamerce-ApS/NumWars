using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowLeadboardItem : MonoBehaviour
{

    public HighscoreWindow _HighscoreWindow;
    Vector3 originalPosition;
    public GameObject originalPositionBottom;
    Vector3 targetPosition;
    // Start is called before the first frame update
    void Start()
    {
        originalPosition = _HighscoreWindow.me.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        string me_name = _HighscoreWindow.me.transform.GetChild(4).GetComponent<Text>().text;

        bool found = false;
        GameObject foundItem = null;
        for (int i = 0; i < _HighscoreWindow._parent.childCount;i++)
        {
            if(_HighscoreWindow._parent.GetChild(i).transform.childCount>4)
            {
                string item_name = _HighscoreWindow._parent.GetChild(i).transform.GetChild(4).GetComponent<Text>().text;

                if (item_name == me_name)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(_HighscoreWindow._parent.parent.GetComponent<RectTransform>(), _HighscoreWindow._parent.GetChild(i).transform.position) == true)
                    {
                        targetPosition = _HighscoreWindow._parent.GetChild(i).transform.position;
                        found = true;
                    }
                    else
                        foundItem = _HighscoreWindow._parent.GetChild(i).gameObject;
                }

            }

        }

        

        if (found == false)
        {
            targetPosition = originalPosition;
            if(foundItem != null)
            {
                if( foundItem.transform.position.y < 0)
                {
                    targetPosition = originalPositionBottom.transform.position;
                }
                else
                    targetPosition = originalPosition;

            }
            else
            {
                targetPosition = originalPositionBottom.transform.position;

            }

        }


        //     if(Vector3.Distance(targetPosition, _HighscoreWindow.me.transform.position) > 0.01f)
        //      _HighscoreWindow.me.transform.position = Vector3.Lerp(_HighscoreWindow.me.transform.position, targetPosition, Time.deltaTime * 20);
        _HighscoreWindow.me.transform.position = targetPosition;

    }
}
