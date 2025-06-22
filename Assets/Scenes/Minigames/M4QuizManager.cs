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

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        quizzes = M4QuizGenerator.GenerateQuiz(animalDatabase, 20);

        DisplayCurrentQuiz();

        Invoke(nameof(PlayCurrentSound), 1f); // Delay to allow UI update
    }

    public void SelectAnswer(M4QuizAnswer answer)
    {
        if (quizzes.Count > currentQuizIndex)
        {
            M4QuizData currentQuiz = quizzes[currentQuizIndex];

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
            M4QuizData currentQuiz = quizzes[currentQuizIndex];

            for (int i = 0; i < answerItems.Count; i++)
            {
                answerItems[i].SetAnswer(currentQuiz.answers[i]);
            }
        }
        else
        {
            Debug.Log("No more quizzes available.");
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

        Invoke(nameof(PlayCurrentSound), 1f); // Delay to allow UI update
    }

    public M4QuizData GetCurrentQuestion()
    {
        if (currentQuizIndex < quizzes.Count)
        {
            return quizzes[currentQuizIndex];
        }
        return null;
    }
}

public class M4QuizGenerator
{
    public static List<M4QuizData> GenerateQuiz(M4AnimalData[] animalDatabase, int questionCount)
    {
        if (animalDatabase == null || animalDatabase.Length == 0 || questionCount <= 0)
        {
            return null;
        }

        List<M4QuizData> quizzes = new List<M4QuizData>();

        for (int i = 0; i < questionCount; i++)
        {
            M4QuizData quizData = new M4QuizData();

            // Select a random animal for the question
            int questionIndex = Random.Range(0, animalDatabase.Length);
            quizData.question = animalDatabase[questionIndex];

            // Prepare answers
            List<M4QuizAnswer> answers = new List<M4QuizAnswer>();
            HashSet<int> usedIndices = new HashSet<int>();
            // Add the correct answer
            M4QuizAnswer correctAnswer = new M4QuizAnswer
            {
                animalData = quizData.question,
                isCorrect = true
            };
            answers.Add(correctAnswer);
            usedIndices.Add(questionIndex);

            // Add distractor answers
            while (answers.Count < 4)
            {
                int distractorIndex = Random.Range(0, animalDatabase.Length);
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

            // Shuffle answers
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