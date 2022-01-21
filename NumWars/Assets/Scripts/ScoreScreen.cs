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

    public void ShowScore(List<Tile> score,Player aPlayer)
    {
        bg.SetActive(true);
        bg.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        bg.GetComponent<Image>().DOFade(119f / 255f, 1.0f).SetEase(Ease.InOutQuart);

        for (int i = 0; i < score.Count;i++)
        {
            StartCoroutine(ShowScoreAfterTime (0.25f+ i * 0.6f, score[i] ));
        }
        StartCoroutine(SummarizeAfterTime(0.5f + score.Count * 0.6f, score, aPlayer));




    }

    IEnumerator ShowScoreAfterTime(float aTime, Tile aTile)
    {
        yield return new WaitForSeconds(aTime);
        GameObject go = GameObject.Instantiate(ScorePrefab, bg.transform);
        go.transform.position = aTile.transform.position;
        go.transform.GetChild(0).Find("Text").GetComponent<Text>().text = aTile.textLabel.text;


        createdPoints.Add(go);
    }

    IEnumerator SummarizeAfterTime(float aTime, List<Tile> score,Player thePlayer)
    {

       





        yield return new WaitForSeconds(aTime);


        for (int i = 0; i < createdPoints.Count; i++)
        {
            createdPoints[i].GetComponent<RectTransform>().DOMove(new Vector3(0, 0), 0.5f).SetEase(Ease.InOutQuart);
        }
        yield return new WaitForSeconds(0.3f);
        int totalScore = 0;
        for (int i = 0; i < score.Count; i++)
        {
            totalScore += int.Parse(score[i].textLabel.text);
        }
        for (int i = 0; i < createdPoints.Count; i++)
        {
            createdPoints[i].transform.GetChild(0).Find("Text").GetComponent<Text>().text = totalScore.ToString();
          //  createdPoints[i].transform.localScale *= 1.3f;
            createdPoints[i].GetComponent<RectTransform>().DOScale(createdPoints[i].transform.localScale*1.3f, 0.1f).SetEase(Ease.InOutQuart);

        }

        yield return new WaitForSeconds(.95f);

        for (int i = 0; i < createdPoints.Count; i++)
        {
            //createdPoints[i].GetComponent<RectTransform>().DOMove(TotalScore.transform.position-new Vector3(25,0,0), 1).SetEase(Ease.InOutQuart);

            createdPoints[i].GetComponent<RectTransform>().DOMove(GameManager.instance.p1_score.transform.position, 1).SetEase(Ease.InOutQuart);

            createdPoints[i].transform.GetChild(0).GetComponent<Image>().DOFade(0, 1).SetEase(Ease.InOutQuart); ;
        }
        bg.GetComponent<Image>().DOFade(0, 1.5f).SetEase(Ease.InOutQuart); ;



        // yield return new WaitForSeconds(0.3f);
        //TotalScore.SetActive(false);
        //TotalScore.SetActive(true);

        //    yield return new WaitForSeconds(0.5f);
        //yield return new WaitForSeconds(0.7f);

        //for (int i = 0; i < createdPoints.Count; i++)
        //{

        //    createdPoints[i].transform.GetChild(0).GetComponent<Animator>().enabled = false;
        //   // createdPoints[i].GetComponent<RectTransform>().DOShakePosition( 0.5f,25,25,90,true);

        //}

        yield return new WaitForSeconds(0.7f);

        GameManager.instance.AddScore(thePlayer, totalScore);
        for (int i = 0; i < score.Count; i++)
        {
            score[i].Flip();
        }
        //for (int i = 0; i < createdPoints.Count; i++)
        //{
        //    Destroy(createdPoints[i]);
        //}
        //createdPoints.Clear();


        //yield return new WaitForSeconds(3f);

        //for (int i = 0; i < createdPoints.Count; i++)
        //{
        //    Destroy(createdPoints[i]);
        //}
        //createdPoints.Clear();


        for (int i = 0; i < createdPoints.Count; i++)
        {
            Destroy(createdPoints[i]);
        }
        createdPoints.Clear();


        //TotalScore.GetComponent<Animator>().Play("scoreOutro");
        yield return new WaitForSeconds(1.0f);
   
        //  
        bg.SetActive(false);
        thePlayer.AddNewPlayerTiles();
        PlayerBoard.instance.RefreshLayout();

        GameManager.instance.NextTurn();

    }

  
}
