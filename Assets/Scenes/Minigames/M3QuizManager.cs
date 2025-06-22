using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class M3QuizManager : MonoBehaviour
{
    public static M3QuizManager Instance;

    public List<M3QuizData> quizzes;
    public List<M3QuizAnswerItem> answerItems;
    public TMP_Text questionTextUI; // Drag in Unity Inspector
    public int currentQuizIndex = 0;
    public AudioClip correctAnswerSound;
    public AudioClip incorrectAnswerSound;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        quizzes = M3QuizGenerator.GenerateQuiz(20);
        DisplayCurrentQuiz();
    }

    public void SelectAnswer(M3QuizAnswer answer)
    {
        if (quizzes.Count > currentQuizIndex)
        {
            M3QuizData currentQuiz = quizzes[currentQuizIndex];

            if (currentQuiz.correctAnswerIndex == System.Array.IndexOf(currentQuiz.answers, answer))
            {
                AudioSource.PlayClipAtPoint(correctAnswerSound, Vector3.zero);
                NextQuestion();
            }
            else
            {
                AudioSource.PlayClipAtPoint(incorrectAnswerSound, Vector3.zero);
            }
        }
    }

    void DisplayCurrentQuiz()
    {
        if (currentQuizIndex < quizzes.Count)
        {
            M3QuizData currentQuiz = quizzes[currentQuizIndex];
            questionTextUI.text = currentQuiz.questionText;

            for (int i = 0; i < answerItems.Count; i++)
            {
                answerItems[i].SetAnswer(currentQuiz.answers[i]);
            }
        }
    }

    public void NextQuestion()
    {
        currentQuizIndex++;
        DisplayCurrentQuiz();
    }

    public M3QuizData GetCurrentQuestion()
    {
        if (currentQuizIndex < quizzes.Count)
        {
            return quizzes[currentQuizIndex];
        }
        return null;
    }
}

[System.Serializable]
public class M3QuizData
{
    public string questionText;
    public M3QuizAnswer[] answers;
    public int correctAnswerIndex;
}

[System.Serializable]
public class M3QuizAnswer
{
    public string answerText;
    public bool isCorrect;
}

public class M3QuizGenerator
{
    public static List<M3QuizData> GenerateQuiz(int questionCount)
    {
        List<M3QuizData> quizzes = new List<M3QuizData>();
        System.Random rng = new System.Random();

        for (int i = 0; i < questionCount; i++)
        {
            int a = rng.Next(1, 20);
            int b = rng.Next(1, 20);
            int correctAnswerValue = a + b;
            string questionText = $"{a} + {b} = ?";

            List<M3QuizAnswer> answers = new List<M3QuizAnswer>();
            HashSet<int> usedValues = new HashSet<int> { correctAnswerValue };

            M3QuizAnswer correctAnswer = new M3QuizAnswer
            {
                answerText = correctAnswerValue.ToString(),
                isCorrect = true
            };
            answers.Add(correctAnswer);

            while (answers.Count < 4)
            {
                int wrongAnswer = rng.Next(correctAnswerValue - 5, correctAnswerValue + 6);
                if (wrongAnswer != correctAnswerValue && usedValues.Add(wrongAnswer))
                {
                    answers.Add(new M3QuizAnswer
                    {
                        answerText = wrongAnswer.ToString(),
                        isCorrect = false
                    });
                }
            }

            ShuffleLib.ShuffleArray(answers);

            M3QuizData quiz = new M3QuizData
            {
                questionText = questionText,
                answers = answers.ToArray(),
                correctAnswerIndex = System.Array.IndexOf(answers.ToArray(), correctAnswer)
            };

            quizzes.Add(quiz);
        }

        return quizzes;
    }
}
