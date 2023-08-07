using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Text Settings")]
    [SerializeField] private TextMeshProUGUI redText;
    [SerializeField] private TextMeshProUGUI blueText;
    [SerializeField] private GameObject startButton;

    //[Header("Scene Settings")]
    //public float waitTime;      //0.5
    //public string sceneName;

    private bool blueCheck;
    private bool redCheck;

    public GameObject leftPlayer;
    public GameObject rightPlayer;

    private Animator leftSide;
    private Animator rightSide;
    private void Awake()
    {
        startButton.SetActive(false);
        leftSide = leftPlayer.GetComponent<Animator>();
        rightSide = rightPlayer.GetComponent<Animator>();
    }

    //private void Start()
    //{
    //   leftSide = leftPlayer.GetComponent<Animator>();   
    //   rightSide = rightPlayer.GetComponent<Animator>();   
    //}

    private void Update()
    {
        CheckBluePlayer();
        CheckRedPlayer();
        PlayerReady();
    }

    private void CheckBluePlayer()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            blueText.text = "This player is ready to destroy!";
            blueCheck = true;
            leftSide.SetBool("isReady", true);
        }
    }

    private void CheckRedPlayer()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            redText.text = "This player is ready to crush his opponent!";
            redCheck = true;
            rightSide.SetBool("isReady", true);
        }
    }
    private void PlayerReady()
    {
        if (redCheck && blueCheck)
        {
            startButton.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                LoadScene("LevelScene");
            }
        }
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
