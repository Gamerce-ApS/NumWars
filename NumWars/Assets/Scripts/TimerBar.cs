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



    // Update is called once per frame
    void Update()
    {
        SetTime();
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
        _timeLeftLabel.text = time.Hours+":"+ time.Minutes+":"+ time.Seconds;


        _bar.fillAmount = (timeToDeadline / (60f * 60f * 24f * 2f));

    }
}
