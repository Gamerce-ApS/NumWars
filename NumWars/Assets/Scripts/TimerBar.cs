using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour
{
    public Image _bar;
    public Text _timeLeftLabel;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    float timer = 0;
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer>1)
        {
            timer = 0;
            SetTime();
        }
    
    }

    public void SetTime()
    {
        if (Board.instance.boardData == null || Board.instance.boardData.LastMoveTimeStamp == null)
            return;

        if (TutorialController.instance != null && TutorialController.instance.IsTutorial)
            return;

        if(GameManager.instance.thePlayers[1].isAI)
        {
            _bar.gameObject.SetActive(false);
            return;
        }

        long a = System.DateTimeOffset.Now.ToUnixTimeSeconds();
        long b = long.Parse(Board.instance.boardData.LastMoveTimeStamp);
        long dif = a-b;

      //  _timeLeftLabel.text = dif.ToString();

        long Future = b + Startup.TIMEOUT;

        long timeToDeadline = Future - a;

        TimeSpan time = TimeSpan.FromSeconds(timeToDeadline);
        _timeLeftLabel.text = (int)time.TotalHours+":"+ time.Minutes+":"+ time.Seconds;

        
        _bar.fillAmount = (timeToDeadline / (Startup.TIMEOUT));

    }
}
