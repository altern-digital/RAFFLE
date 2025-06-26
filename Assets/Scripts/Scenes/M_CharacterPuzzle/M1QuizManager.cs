using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class M1QuizManager : MonoBehaviour
{
    public static M1QuizManager Instance;

    public TMP_Text wordWithBlankText;
    public AudioClip correctSound;
    public AudioClip incorrectSound;

    public List<M1QuizData> quizzes;
    public List<M1QuizAnswerItem> answerButtons; // Each button represents one character
    private int currentIndex = 0;

    // Integrated UI elements and scoring variables
    public GameObject completeDialog;
    public TMP_Text currentLevelText;
    public string scoreKey = "M1Score"; // Changed scoreKey for M1
    public TMP_Text scoreText;
    private int score;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        quizzes = M1QuizGenerator.GenerateQuiz(20);

        // Load previous score or initialize to 0
        score = PlayerPrefs.GetInt(scoreKey, 0);
        UpdateScoreText(); // Display initial score

        DisplayCurrentQuiz();
    }

    public void SelectAnswer(char chosenChar)
    {
        M1QuizData current = GetCurrentQuiz();

        if (current != null) // Ensure there's a current quiz to prevent errors if quiz completes
        {
            if (char.ToUpper(chosenChar) == char.ToUpper(current.missingChar))
            {
                AudioSource.PlayClipAtPoint(correctSound, Vector3.zero);
                score += 1000; // Add 1000 points for correct answer
                NextQuestion();
            }
            else
            {
                AudioSource.PlayClipAtPoint(incorrectSound, Vector3.zero);
                score -= 500; // Subtract 500 points for incorrect answer
            }
            UpdateScoreText(); // Update the score display after each answer
        }
    }

    void DisplayCurrentQuiz()
    {
        if (currentIndex < quizzes.Count)
        {
            M1QuizData current = quizzes[currentIndex];

            // Update the current level display (e.g., "Level 1/20")
            currentLevelText.text = $"Level {currentIndex + 1}/{quizzes.Count}";
            wordWithBlankText.text = current.displayWord;

            // Assign random 4-character options (1 correct + 3 distractors)
            List<char> options = M1QuizGenerator.GenerateOptions(current.missingChar);
            for (int i = 0; i < answerButtons.Count; i++)
            {
                if (i < options.Count)
                {
                    answerButtons[i].SetAnswer(options[i]);
                    answerButtons[i].gameObject.SetActive(true);
                }
                else
                {
                    answerButtons[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            // All quizzes are complete
            wordWithBlankText.text = "Quiz Complete!";
            foreach (var btn in answerButtons) btn.gameObject.SetActive(false);
            Debug.Log("No more quizzes available. Game Over!");
            completeDialog.SetActive(true); // Show the complete dialog
            PlayerPrefs.SetInt(scoreKey, score); // Save the final score
            PlayerPrefs.Save(); // Ensure PlayerPrefs are saved to disk
        }
    }

    void NextQuestion()
    {
        currentIndex++;
        DisplayCurrentQuiz();
    }

    public M1QuizData GetCurrentQuiz()
    {
        if (currentIndex < quizzes.Count)
            return quizzes[currentIndex];
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

public class M1QuizGenerator
{
    static string[] wordBank = new string[]
    {
        "NASI", "ROTI", "SUSU", "PISANG", "APEL",
        "JERUK", "AIR", "GULA", "GARAM", "MAKAN",
        "KUCING", "ANJING", "AYAM", "BEBEK", "IKAN",
        "BURUNG", "KUPU", "SEMUT", "SAPI", "KAMBING",
        "BUKU", "PENSIL", "MEJA", "KURSI", "BOLA",
        "PINTU", "JENDELA", "SEPATU", "BAJU", "TOPI",
        "MATA", "HIDUNG", "TELINGA", "MULUT", "TANGAN",
        "KAKI", "RAMBUT", "PERUT", "GIGI", "LEHER",
        "MATAHARI", "BULAN", "BINTANG", "AWAN", "HUJAN",
        "BUNGA", "POHON", "RUMPUT", "TANAH", "LAUT",
        "DUDUK", "BERDIRI", "TIDUR", "MAIN", "LARI",
        "TULIS", "BACA", "LIHAT", "DENGAR", "SENYUM"
    };

    public static List<M1QuizData> GenerateQuiz(int count)
    {
        List<M1QuizData> quizzes = new List<M1QuizData>();
        System.Random rng = new System.Random();

        for (int i = 0; i < count; i++)
        {
            string word = wordBank[rng.Next(wordBank.Length)];
            int missingIndex = rng.Next(0, word.Length);
            char missingChar = word[missingIndex];
            string display = word.Substring(0, missingIndex) + "_" + word.Substring(missingIndex + 1);

            quizzes.Add(new M1QuizData
            {
                fullWord = word,
                displayWord = display,
                missingChar = missingChar
            });
        }

        return quizzes;
    }

    public static List<char> GenerateOptions(char correctChar)
    {
        List<char> options = new List<char> { correctChar };
        System.Random rng = new System.Random();

        while (options.Count < 4)
        {
            char randomChar = (char)rng.Next('A', 'Z' + 1);
            if (!options.Contains(randomChar))
                options.Add(randomChar);
        }

        // Shuffle
        for (int i = options.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (options[i], options[j]) = (options[j], options[i]);
        }

        return options;
    }
}

[System.Serializable]
public class M1QuizData
{
    public string fullWord;
    public string displayWord;
    public char missingChar;
}
