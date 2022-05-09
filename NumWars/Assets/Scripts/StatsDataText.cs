using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class StatsDataText : MonoBehaviour
{
    public AchivmentTypeEnum myStatsType;
    AchivmentController ac = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OnEnable()
    {
        if (ac == null)
        {
            float v = Mathf.RoundToInt(AchivmentController.instance.GetStats(myStatsType));
            string value = "";
            if (Mathf.Approximately(v, Mathf.RoundToInt(v)) == false)
                value = "F2";
            transform.GetChild(1).GetComponent<Text>().text = v.ToString(value);
        }
        else
            SetFromData(ac);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetFromData(AchivmentController _ac, bool isAI = false)
    {
        ac = _ac;
        float v = Mathf.RoundToInt(ac.GetStats(myStatsType, isAI));
        string value = "";
        if (Mathf.Approximately(v, Mathf.RoundToInt(v)) == false)
            value = "F2";
        transform.GetChild(1).GetComponent<Text>().text = v.ToString(value);
    }
}
