using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ScoreScreen : MonoBehaviour
{
    public static ScoreScreen instance;

    public GameObject ScorePrefab;
    public GameObject StarEffectPrefab;
    public GameObject bg;

    public GameObject TotalScore;

    

    public List<GameObject> createdPoints = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowScore(List<Tile> score)
    {
        bg.SetActive(true);
        for (int i = 0; i < score.Count;i++)
        {
            StartCoroutine(ShowScoreAfterTime (0.25f+ i * 1.0f, score[i] ));
        }
        StartCoroutine(SummarizeAfterTime(0.5f + score.Count * 1.0f, score));




    }

    IEnumerator ShowScoreAfterTime(float aTime, Tile aTile)
    {
        yield return new WaitForSeconds(aTime);
        GameObject go = GameObject.Instantiate(ScorePrefab, bg.transform);
        go.transform.position = aTile.transform.position;
        go.transform.GetChild(0).Find("Text").GetComponent<Text>().text = aTile.textLabel.text;


        createdPoints.Add(go);
    }

    IEnumerator SummarizeAfterTime(float aTime, List<Tile> score)
    {
        yield return new WaitForSeconds(aTime);




        for (int i = 0; i < createdPoints.Count; i++)
        {
            //createdPoints[i].GetComponent<RectTransform>().DOMove(TotalScore.transform.position-new Vector3(25,0,0), 1).SetEase(Ease.InOutQuart);

            createdPoints[i].GetComponent<RectTransform>().DOMove(GameManager.instance.p1_score.transform.position, 1).SetEase(Ease.InOutQuart);

           


        }


   
        yield return new WaitForSeconds(0.3f);
        TotalScore.SetActive(false);
        TotalScore.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        int totalScore = 0;
        for (int i = 0; i < score.Count; i++)
        {
            totalScore += int.Parse(score[i].textLabel.text);
        }
        GameManager.instance.AddScore(0, totalScore);
        for (int i = 0; i < createdPoints.Count; i++)
        {
            Destroy(createdPoints[i]);
        }
        createdPoints.Clear();


        yield return new WaitForSeconds(3f);

        for (int i = 0; i < createdPoints.Count; i++)
        {
            Destroy(createdPoints[i]);
        }
        createdPoints.Clear();

        TotalScore.GetComponent<Animator>().Play("scoreOutro");
        yield return new WaitForSeconds(1f);
        bg.SetActive(false);

        for (int i = 0; i < score.Count; i++)
        {
            score[i].Flip();
        }
        yield return new WaitForSeconds(1f);
        PlayerBoard.instance.AddNewPlayerTiles();
    }

  
}
