using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Quiz : MonoBehaviour
{
    [Header("Questions")] 
    [SerializeField] private List<QuestionSO> questions = new List<QuestionSO>();
    [SerializeField] TextMeshProUGUI questionText;
    private QuestionSO currentQuestion;
    
    [Header("Answers")]
    [SerializeField] GameObject[] answerButtons;
    private int correctAnswerIndex;
    private bool hasAnsweredEarly = true;
    
    [Header("Buttons")]
    [SerializeField] Sprite defaultAnswerSprite;
    [SerializeField] Sprite correctAnswerSprite;
    
    [Header("Timer")]
    [SerializeField] Image timerImage;
    private Timer timer;

    [Header("Scoring")]
    [SerializeField] private TextMeshProUGUI scoreText;
    private ScoreKeeper scoreKeeper;

    [Header("ProgressBar")] 
    [SerializeField] private Slider progressBar;

    public bool isComplete;

    void Awake()
    {
        timer = FindObjectOfType<Timer>();
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        progressBar.maxValue = questions.Count;
        progressBar.value = 0;
    }

    void Update()
    {
        timerImage.fillAmount = timer.fillFraction;
        if (timer.loadNextQuestion)
        {
            
            if (progressBar.value == progressBar.maxValue)
            {
                isComplete = true;
                return;
            }
            
            hasAnsweredEarly = false;
            GetNextQuestion();
            timer.loadNextQuestion = false;
        }
        else if(!hasAnsweredEarly && !timer.isAnsweringQuestion)
        {
            DisplayAnswer(-1);
            SetButtonState(false);
        }
    }


    public void OnAnswerSelected(int index)
    {
        hasAnsweredEarly = true;
        DisplayAnswer(index);
        SetButtonState(false);
        timer.CancelTimer();
        scoreText.text = "Score: " + scoreKeeper.CalculateScore() + "%";

        

    }

    void DisplayAnswer(int index)
    {
        if (currentQuestion != null)
        {
            Image buttonImage;
            
            if (index == currentQuestion.GetCorrectAnswerIndex())
            {
                questionText.text = "Correct";
                buttonImage = answerButtons[index].GetComponent<Image>();
                buttonImage.sprite = correctAnswerSprite;
                scoreKeeper.IncrementCorrectAnswers();
            }
            else
            {
                correctAnswerIndex = currentQuestion.GetCorrectAnswerIndex();
                string correctAnswer = currentQuestion.GetAnswer(correctAnswerIndex);
                questionText.text = "Sorry, the correct answer was:\n " + correctAnswer;
                buttonImage = answerButtons[correctAnswerIndex].GetComponent<Image>();
                buttonImage.sprite = correctAnswerSprite;
            }
        }

        
    }

    void GetNextQuestion()
    {
        if (questions.Count > 0)
        {
            SetButtonState(true);
            SetDefaultButtonSprites();
            GetRandomQuestion();
            DisplayQuestion();
            progressBar.value++;
            scoreKeeper.IncrementQuestionsSeen();
        }
    }

    void GetRandomQuestion()
    {
        int index = Random.Range(0, questions.Count);
        currentQuestion = questions[index];

        if (questions.Contains(currentQuestion))
        {
            questions.Remove(currentQuestion);
        }
    }

    void DisplayQuestion()
    {
        questionText.text = currentQuestion.GetQuestion();

        TextMeshProUGUI buttonText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            buttonText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = currentQuestion.GetAnswer(i);
        }
    }

    void SetButtonState(bool state)
    {
        Button button;
        for (int i = 0; i < answerButtons.Length; i++)
        {
            button = answerButtons[i].GetComponent<Button>();
            button.interactable = state;
        }
    }

    void SetDefaultButtonSprites()
    {
        Image buttonImage;
        for (int i = 0; i < answerButtons.Length; i++)
        {
            buttonImage = answerButtons[i].GetComponent<Image>();
            buttonImage.sprite = defaultAnswerSprite;
        }
    }


}
