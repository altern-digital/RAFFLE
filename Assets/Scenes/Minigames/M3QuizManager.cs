using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class M3QuizManager : MonoBehaviour
{
    public static M3QuizManager Instance;

    public List<M3QuizData> quizzes;
    public List<M3QuizAnswerItem> answerItems;
    public QuestionRenderer questionRenderer;
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

    public void SelectAnswer(M3QuizChoice answer)
    {
        if (quizzes.Count > currentQuizIndex)
        {
            M3QuizData currentQuiz = quizzes[currentQuizIndex];

            if (currentQuiz.correctAnswerIndex == System.Array.IndexOf(currentQuiz.choices, answer))
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

            questionRenderer.leftObjectCount = currentQuiz.leftObjectCount;
            questionRenderer.rightObjectCount = currentQuiz.rightObjectCount;

            for (int i = 0; i < answerItems.Count; i++)
            {
                answerItems[i].SetAnswer(currentQuiz.choices[i]);
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
    public int leftObjectCount;
    public int rightObjectCount;
    public M3QuizChoice[] choices;
    public int correctAnswerIndex;
}

[System.Serializable]
public class M3QuizChoice
{
    public int value;
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
            int a = rng.Next(1, 5);
            int b = rng.Next(1, 5);
            int correctAnswerValue = a + b;
            string questionText = $"{a} + {b} = ?";

            List<M3QuizChoice> answers = new List<M3QuizChoice>();
            HashSet<int> usedValues = new HashSet<int> { correctAnswerValue };

            M3QuizChoice correctAnswer = new M3QuizChoice
            {
                value = correctAnswerValue,
                isCorrect = true
            };
            answers.Add(correctAnswer);

            while (answers.Count < 4)
            {
                int wrongAnswer = rng.Next(correctAnswerValue - 5, correctAnswerValue + 6);
                if (wrongAnswer != correctAnswerValue && usedValues.Add(wrongAnswer))
                {
                    answers.Add(new M3QuizChoice
                    {
                        value = wrongAnswer,
                        isCorrect = false
                    });
                }
            }

            ShuffleLib.ShuffleArray(answers);

            M3QuizData quiz = new M3QuizData
            {
                questionText = questionText,
                leftObjectCount = a,
                rightObjectCount = b,
                choices = answers.ToArray(),
                correctAnswerIndex = System.Array.IndexOf(answers.ToArray(), correctAnswer)
            };

            quizzes.Add(quiz);
        }

        return quizzes;
    }
}