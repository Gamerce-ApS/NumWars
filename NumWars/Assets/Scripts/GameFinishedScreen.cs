using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameFinishedScreen : MonoBehaviour
{
    public static GameFinishedScreen instance;

    public Text p1_name;
    public Text p1_thropies;
    public Text p1_score;
    public Text p1_wins;
    public GameObject p1_wins_go;

    public Text p2_name;
    public Text p2_thropies;
    public Text p2_score;
    public Text p2_wins;
    public GameObject p2_wins_go;

    public List<GameObject> ElementsToMoveOut = new List<GameObject>();



    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Show(BoardData bf)
    {
        PlayfabHelperFunctions.instance.AddAiGameToOldGames(CompressString.StringCompressor.CompressString(bf.GetJson()));



        for (int i = 0; i< ElementsToMoveOut.Count;i++)
        {
            ElementsToMoveOut[i].transform.DOMoveX(10, 0.5f).SetEase(Ease.InOutQuart);
        }

        transform.GetChild(0).gameObject.SetActive(true);

        float posX = transform.GetChild(0).transform.position.x;
        transform.GetChild(0).transform.position -= new Vector3(10, 0, 0);
        transform.GetChild(0).DOMoveX(posX, 0.5f).SetEase(Ease.InOutQuart);

        transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(0, 0, 0, 0);
        transform.GetChild(0).GetChild(0).GetComponent<Image>().DOFade(107f / 255f, 0.5f).SetEase(Ease.InOutQuart).SetDelay(0.1f);


        if (Startup._instance.MyPlayfabID == bf.player1_PlayfabId)
        {
            p1_name.text = bf.player1_displayName;
            p1_thropies.text = "-";
            p1_score.text = bf.player1_score;
            p1_wins.text = "-";

            p2_name.text = bf.player2_displayName;
            p2_thropies.text = "-";
            p2_score.text = bf.player2_score;
            p2_wins.text = "-";

            if(int.Parse(bf.player1_score) > int.Parse(bf.player2_score))
                Startup._instance.AdjustThropies(30);
            else
                Startup._instance.AdjustThropies(-15);

        }
        else
        {
            p2_name.text = bf.player1_displayName;
            p2_thropies.text = "-";
            p2_score.text = bf.player1_score;
            p2_wins.text = "-";

            p1_name.text = bf.player2_displayName;
            p1_thropies.text = "-";
            p1_score.text = bf.player2_score;
            p1_wins.text = "-";

            if (int.Parse(bf.player2_score) > int.Parse(bf.player1_score))
                Startup._instance.AdjustThropies(30);
            else
                Startup._instance.AdjustThropies(-15);

        }

        if(int.Parse(p1_score.text) > int.Parse(p2_score.text))
        {
            p1_wins_go.SetActive(true);
            p2_wins_go.SetActive(false);
        }
        else
        {
            p1_wins_go.SetActive(false);
            p2_wins_go.SetActive(true);
        }



    }
    public void PressContinue()
    {


        SceneManager.LoadScene(0);
        Startup._instance.Refresh(0.1f);
    }
}
