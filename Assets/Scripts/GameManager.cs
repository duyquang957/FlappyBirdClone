using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }
    [SerializeField] TextMeshProUGUI scoreText = default;
    protected int score = 0;
    public int Score
    {
        get => score; set
        {
            score = value;
            scoreText.text = ConvertNumberToSpriteText(score.ToString());
            addScoreAudio.Play();
            if (!bird.speedIncrease) bird.speedIncrease = true;
        }
    }

    [SerializeField] BirdBehaviour bird = default;
    [SerializeField] Background bg = default;

    [SerializeField] GameObject panelMenu = default;
    [SerializeField] GameObject panelGame = default;
    [SerializeField] Button btnPlay = default;
    [SerializeField] Button btnTap = default;
    [SerializeField] TextMeshProUGUI menuHighScoreText = default;
    [SerializeField] GameObject panelOver = default;
    [SerializeField] Button btnRestart = default;
    [SerializeField] TextMeshProUGUI overScoreText = default;
    [SerializeField] TextMeshProUGUI overHighScoreText = default;
    [SerializeField] Image medalImage = default;
    [SerializeField] Sprite[] medalSprites = default;

    private void Start()
    {
        bg.bird = bird;
        panelMenu.SetActive(true);
        panelGame.SetActive(false);
        panelOver.SetActive(false);
        btnTap.gameObject.SetActive(false);
        btnPlay.onClick.AddListener(() =>
        {
            btnPlay.gameObject.SetActive(false);
            btnTap.gameObject.SetActive(true);
            menuHighScoreText.gameObject.SetActive(false);
            buttonAudio.Play();
        });
        btnTap.onClick.AddListener(TapToStart);

        btnRestart.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            buttonAudio.Play();
        });

        if (!PlayerPrefs.HasKey("highscore")) PlayerPrefs.SetInt("highscore", 0);
        var highscore = PlayerPrefs.GetInt("highscore");
        menuHighScoreText.text = (highscore > 0) ? string.Format("BEST : {0}", highscore) : "";
    }

    public void TapToStart()
    {
        panelMenu.SetActive(false);
        panelGame.SetActive(true);
        bird.isActive = true;
        bg.isActive = true;
        bird.Jump();
        bird.SetAnimState(BirdAnimState.fly);
        buttonAudio.Play();
    }

    public void GameOver()
    {
        if (!bird.isActive) return;
        panelOver.SetActive(true);
        overScoreText.text = ConvertNumberToSpriteText(score.ToString());
        var highscore = PlayerPrefs.GetInt("highscore");
        overHighScoreText.text = ConvertNumberToSpriteText(highscore.ToString());
        if (score > highscore) PlayerPrefs.SetInt("highscore", score);
        ShowMedal(highscore);
        scoreText.gameObject.SetActive(false);
        fallAudio.Play();

        bird.isActive = false;
        bg.isActive = false;
        bird.SetAnimState(BirdAnimState.hurt);
        bird.transform.rotation = Quaternion.identity;
        bird.BirdHit();
        
    }

    void ShowMedal(int highscore)
    {
        if (score < 5)
        {
            medalImage.sprite = medalSprites[2];
            return;
        }
        else if (score > highscore)
        {
            medalImage.sprite = medalSprites[0];
            return;
        }

        medalImage.sprite = medalSprites[1];
    }

    string ConvertNumberToSpriteText(string number)
    {
        var result = "";
        
        for (int i = 0; i < number.Length; i++)
        {
            result += string.Format("<sprite={0}>", number[i]);
        }

        return result;
    }

    [SerializeField] AudioSource buttonAudio = default;
    [SerializeField] AudioSource addScoreAudio = default;
    [SerializeField] AudioSource fallAudio = default;

    
}
