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

    // Integrated UI elements and scoring variables
    public GameObject completeDialog;
    public TMP_Text currentLevelText;
    public string scoreKey = "M3Score"; // Changed scoreKey for M3
    public TMP_Text scoreText;
    private int score;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        quizzes = M3QuizGenerator.GenerateQuiz(20); // Generate 20 quiz questions

        // Load previous score or initialize to 0
        score = PlayerPrefs.GetInt(scoreKey, 0);
        UpdateScoreText(); // Display initial score

        DisplayCurrentQuiz(); // Display the first quiz question
    }

    /// <summary>
    /// Called when an answer item is selected by the user.
    /// Checks if the selected answer is correct, updates score, and proceeds to the next question.
    /// </summary>
    /// <param name="answer">The selected M3QuizChoice object.</param>
    public void SelectAnswer(M3QuizChoice answer)
    {
        // Ensure there are still quizzes to answer
        if (quizzes.Count > currentQuizIndex)
        {
            M3QuizData currentQuiz = quizzes[currentQuizIndex];

            // Check if the selected answer is the correct one
            if (currentQuiz.correctAnswerIndex == System.Array.IndexOf(currentQuiz.choices, answer))
            {
                AudioSource.PlayClipAtPoint(correctAnswerSound, Vector3.zero); // Play correct sound
                score += 1000; // Add 1000 points for correct answer
                NextQuestion(); // Move to the next question
            }
            else
            {
                AudioSource.PlayClipAtPoint(incorrectAnswerSound, Vector3.zero); // Play incorrect sound
                score -= 500; // Subtract 500 points for incorrect answer
            }
            UpdateScoreText(); // Update the score display after each answer
        }
    }

    /// <summary>
    /// Displays the current quiz question and its answers.
    /// Also updates the current level text and handles game completion.
    /// </summary>
    void DisplayCurrentQuiz()
    {
        // Check if there are more quizzes to display
        if (currentQuizIndex < quizzes.Count)
        {
            M3QuizData currentQuiz = quizzes[currentQuizIndex];

            // Update the current level display (e.g., "Level 1/20")
            currentLevelText.text = $"Level {currentQuizIndex + 1}/{quizzes.Count}";

            questionRenderer.SetQuestion(
                currentQuiz.leftObjectCount,
                currentQuiz.rightObjectCount,
                false
            );

            // Set the answers for each answer item UI element
            for (int i = 0; i < answerItems.Count; i++)
            {
                answerItems[i].SetAnswer(currentQuiz.choices[i]);
            }
        }
        else
        {
            // All quizzes are complete
            Debug.Log("No more quizzes available. Game Over!");
            completeDialog.SetActive(true); // Show the complete dialog
            PlayerPrefs.SetInt(scoreKey, score); // Save the final score
            PlayerPrefs.Save(); // Ensure PlayerPrefs are saved to disk
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

    /// <summary>
    /// Updates the score text display.
    /// </summary>
    void UpdateScoreText()
    {
        scoreText.text = $"Score: {score}";
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

            ShuffleLibM3.ShuffleArray(answers, rng); // Use a specific ShuffleLib for M3 if one exists, otherwise use a generic one.

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

// Assuming a ShuffleLibM3 exists or defining a generic one here for completion
public static class ShuffleLibM3
{
    public static void ShuffleArray<T>(List<T> list, System.Random rng)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
