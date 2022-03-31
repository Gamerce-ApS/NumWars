using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ProfileButton : MonoBehaviour
{



    public Text _levelText;
    public Image _levelBar;

    public Text _nameText;
    public Text _thropiesText;

    public static ProfileButton instance;
    public void Init(string pName, string Rank,string aXP)
    {
        _nameText.text = pName;
        _thropiesText.text = Rank;

        SetXP(aXP);



        if (gameObject.activeSelf == false)
        {
            gameObject.SetActive(true);

            float x = gameObject.transform.position.x;
            gameObject.transform.position -= new Vector3(10, 0, 0);
            gameObject.transform.DOMoveX(x, 0.3f).SetEase(Ease.InOutQuart);


        }



    }
    void SetXP(string aXP)
    {
        for(int i = 0; i < PlayfabHelperFunctions.instance.LevelSettings.Count;i++)
        {
            if(int.Parse(aXP) <= PlayfabHelperFunctions.instance.LevelSettings[i])
            {
                _levelText.text = (i).ToString();

                float xpAbove = int.Parse(aXP)- PlayfabHelperFunctions.instance.LevelSettings[i - 1];

           
                if(i>0)
                {
                    float dif = PlayfabHelperFunctions.instance.LevelSettings[i] - PlayfabHelperFunctions.instance.LevelSettings[i - 1];
                    _levelBar.fillAmount = xpAbove / dif;
                }
                else
                {
                    float dif = float.Parse(aXP);
                    _levelBar.fillAmount = dif / (float)PlayfabHelperFunctions.instance.LevelSettings[i];
                }


         
                break;
            }
            
        }
    }
    public void Refresh()
    {
        SetXP(Startup._instance.myData["XP"].Value);
    }

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
            instance = this;

        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
