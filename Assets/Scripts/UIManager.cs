using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // References
    public static UIManager instance;   // Static instance is created for easy access
    [SerializeField] private GameObject countDownPanel;
    [SerializeField] private TMP_Text countDownText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject failText;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject resumeButton;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject enterNamePanel;
    [SerializeField] private GameObject leaderBoardPanel;
    [SerializeField] private TMP_InputField playerNameinputField;
    [SerializeField] private TMP_Text[] playerNameTexts;
    [SerializeField] private TMP_Text[] scoreTexts;    
    [SerializeField] private TMP_Text scoreText;   //current score updating text
    [SerializeField] private int enemyKillPoint=100;
    [SerializeField] private GameObject startCam;
    [SerializeField] private GameObject gameCam;


    public int score=0;
    public int enemiesKilled=0;
    

    int timer=3;  // timer for countdown
    bool win=false;
    public bool fail=false;
    int leaderBoardCount;
    int[] highscores=new int[3];
    string[] players=new string[3];
    bool newHighScore;




    void Awake()
    {
        if(instance!=null && instance!=this)
        {
            Destroy(this);
        }
        else
        {
            instance=this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        startCam.SetActive(false);
        gameCam.SetActive(true);
        ClearAllPanels();
        StartCoroutine(CountDown());

        leaderBoardCount=PlayerPrefs.GetInt("LeaderBoardCount",0);
        name=playerNameinputField.text;

        for(int i=0;i<3;i++)
        {

            if(i<leaderBoardCount)
            {
                highscores[i]=PlayerPrefs.GetInt("HighScore"+i,0);
                players[i]=PlayerPrefs.GetString("PlayerName"+i,"....");
            }
            else
            {
                highscores[i]=0;
            }
            playerNameTexts[i].text=players[i];
            scoreTexts[i].text=highscores[i].ToString();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(enemiesKilled==5)
        {
            win=true;
        }
        if(win || fail)
        {
            GameOver();
        }
    }

    //Called by enemys on their death
    public void ScoreUpdate()
    {
        score=score+enemyKillPoint;
        scoreText.text="Score :"+score.ToString();
    }

    //activates gameoverpanel and asks for player name when a new highscore is created
    void GameOver()
    {
        scoreText.gameObject.SetActive(false);
        pauseButton.SetActive(false);
        if(leaderBoardCount>0)
        {
            leaderBoardPanel.SetActive(true);
        }
        
        if(win)
        {
            winText.SetActive(true);
            failText.SetActive(false);
            newHighScore=false;
            for(int i=0;i<3;i++)
            {
                if(score>highscores[i])
                {
                    newHighScore=true;
                }
            }
            if(newHighScore)
            {
                restartButton.SetActive(false);
                resumeButton.SetActive(false);
                enterNamePanel.SetActive(true);
            }

            gameOverPanel.SetActive(true);
        }
        else if(fail)
        {
            Time.timeScale=0f;
            winText.SetActive(false);
            failText.SetActive(true);
            restartButton.SetActive(true);
            gameOverPanel.SetActive(true);
        }
    }

    

    // Make all panels not active
    void ClearAllPanels()
    {
        countDownPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        enterNamePanel.SetActive(false);
        leaderBoardPanel.SetActive(false);
        resumeButton.SetActive(false);
    } 

    // Coroutine for countdown updates
    IEnumerator CountDown()
    {
        Time.timeScale=0f;
        countDownPanel.SetActive(true);
        while(timer>=0)
        {
            countDownText.text=timer.ToString();
            yield return new WaitForSecondsRealtime(1f);
            timer--;
        }
        countDownPanel.SetActive(false);
        Time.timeScale=1f;
        timer=3;
    }

    // called by pause button...pauses the game and show pause panel and deactivates pause button
    public void PauseBtn()
    {
        Time.timeScale=0f;
        pauseButton.SetActive(false);
        pausePanel.SetActive(true);
    }

    // called by resume button..closes pause panel and activates pause button
    public void ResumeBtn()
    {
        Time.timeScale=1f;
        pausePanel.SetActive(false);
        pauseButton.SetActive(true);
    }

    // called by restart button ...loads current scene
    public void RestartButton()
    {
        Time.timeScale=1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    

    // Saves the text from input field and set it on corresponding player prefs 
    public void Submit()
    {
        leaderBoardCount++;
        PlayerPrefs.SetInt("LeaderBoardCount",leaderBoardCount);
        name=playerNameinputField.text;
        playerNameinputField.text="";
        for(int i=0;i<3;i++)
        {
            if(score>highscores[i])
            {
                for(int j=i;j<2;j++)
                {
                    highscores[j]=highscores[j+1];
                    players[j]=players[j+1];
                }
                highscores[i]=score;
                players[i]=name;
                break;
            }
        }
        for(int i=0;i<3;i++)
        {
            PlayerPrefs.SetInt("HighScore"+i,highscores[i]);
            PlayerPrefs.SetString("PlayerName"+i,players[i]);


            playerNameTexts[i].text=players[i];
            scoreTexts[i].text=highscores[i].ToString();
        }
        if(leaderBoardCount>0)
        {
            leaderBoardPanel.SetActive(true);
        }
        restartButton.SetActive(true);
        resumeButton.SetActive(false);
        enterNamePanel.SetActive(false);
        newHighScore=false;
    }

    public void QuitBtn()
    {
        Application.Quit();
    }

}
