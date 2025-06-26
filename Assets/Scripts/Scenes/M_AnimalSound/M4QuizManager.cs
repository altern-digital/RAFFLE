using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class M4QuizManager : MonoBehaviour
{
    public static M4QuizManager Instance;
    public M4AnimalData[] animalDatabase;

    public List<M4QuizData> quizzes;
    public List<M4QuizAnswerItem> answerItems;
    public int currentQuizIndex = 0;
    public AudioClip correctAnswerSound;
    public AudioClip incorrectAnswerSound;

    public GameObject completeDialog;
    public TMP_Text currentLevelText;
    public string scoreKey = "M4Score";
    public TMP_Text scoreText;
    private int score;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        quizzes = M4QuizGenerator.GenerateQuiz(animalDatabase, 20);

        score = PlayerPrefs.GetInt(scoreKey, 0);
        UpdateScoreText();

        DisplayCurrentQuiz();

        Invoke(nameof(PlayCurrentSound), 1f);
    }

    public void SelectAnswer(M4QuizAnswer answer)
    {
        if (quizzes.Count > currentQuizIndex)
        {
            M4QuizData currentQuiz = quizzes[currentQuizIndex];

            if (currentQuiz.correctAnswerIndex == System.Array.IndexOf(currentQuiz.answers, answer))
            {
                AudioSource.PlayClipAtPoint(correctAnswerSound, Vector3.zero);
                score += 1000;
                NextQuestion();
            }
            else
            {
                AudioSource.PlayClipAtPoint(incorrectAnswerSound, Vector3.zero);
                score -= 500;
            }
            UpdateScoreText();
        }
    }

    void DisplayCurrentQuiz()
    {
        if (currentQuizIndex < quizzes.Count)
        {
            M4QuizData currentQuiz = quizzes[currentQuizIndex];

            currentLevelText.text = $"Level {currentQuizIndex + 1}/{quizzes.Count}";

            for (int i = 0; i < answerItems.Count; i++)
            {
                answerItems[i].SetAnswer(currentQuiz.answers[i]);
            }
        }
        else
        {
            Debug.Log("No more quizzes available. Game Over!");
            completeDialog.SetActive(true);
            PlayerPrefs.SetInt(scoreKey, score);
            PlayerPrefs.Save();
        }
    }

    public void PlayCurrentSound()
    {
        M4QuizData currentQuiz = GetCurrentQuestion();

        if (currentQuiz != null)
        {
            AudioSource.PlayClipAtPoint(currentQuiz.question.audio, Vector3.zero);
        }
    }

    public void NextQuestion()
    {
        currentQuizIndex++;
        DisplayCurrentQuiz();

        if (currentQuizIndex < quizzes.Count)
        {
            Invoke(nameof(PlayCurrentSound), 1f);
        }
    }

    public M4QuizData GetCurrentQuestion()
    {
        if (currentQuizIndex < quizzes.Count)
        {
            return quizzes[currentQuizIndex];
        }
        return null;
    }

    void UpdateScoreText()
    {
        scoreText.text = $"Score: {score}";
    }
}

public class M4QuizGenerator
{
    public static List<M4QuizData> GenerateQuiz(M4AnimalData[] animalDatabase, int questionCount)
    {
        if (animalDatabase == null || animalDatabase.Length == 0 || questionCount <= 0)
        {
            Debug.LogError("Animal database is empty or question count is invalid.");
            return null;
        }

        List<M4QuizData> quizzes = new List<M4QuizData>();
        System.Random rng = new System.Random();

        for (int i = 0; i < questionCount; i++)
        {
            M4QuizData quizData = new M4QuizData();

            int questionIndex = rng.Next(0, animalDatabase.Length);
            quizData.question = animalDatabase[questionIndex];

            List<M4QuizAnswer> answers = new List<M4QuizAnswer>();
            HashSet<int> usedIndices = new HashSet<int>();

            M4QuizAnswer correctAnswer = new M4QuizAnswer
            {
                animalData = quizData.question,
                isCorrect = true
            };
            answers.Add(correctAnswer);
            usedIndices.Add(questionIndex);

            while (answers.Count < 4)
            {
                int distractorIndex = rng.Next(0, animalDatabase.Length);
                if (!usedIndices.Contains(distractorIndex))
                {
                    M4QuizAnswer distractor = new M4QuizAnswer
                    {
                        animalData = animalDatabase[distractorIndex],
                        isCorrect = false
                    };
                    answers.Add(distractor);
                    usedIndices.Add(distractorIndex);
                }
            }

            ShuffleLib.ShuffleArray(answers);
            quizData.answers = answers.ToArray();

            quizData.correctAnswerIndex = System.Array.IndexOf(quizData.answers, correctAnswer);

            quizzes.Add(quizData);
        }

        return quizzes;
    }
}


[System.Serializable]
public class M4AnimalData
{
    public AudioClip audio;
    public string name;
}

[System.Serializable]
public class M4QuizData
{
    public M4AnimalData question;
    public M4QuizAnswer[] answers;
    public int correctAnswerIndex;
}

[System.Serializable]
public class M4QuizAnswer
{
    public M4AnimalData animalData;
    public bool isCorrect;
}
