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

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        quizzes = M1QuizGenerator.GenerateQuiz(20);
        DisplayCurrentQuiz();
    }

    public void SelectAnswer(char chosenChar)
    {
        M1QuizData current = GetCurrentQuiz();

        if (char.ToUpper(chosenChar) == char.ToUpper(current.missingChar))
        {
            AudioSource.PlayClipAtPoint(correctSound, Vector3.zero);
            NextQuestion();
        }
        else
        {
            AudioSource.PlayClipAtPoint(incorrectSound, Vector3.zero);
        }
    }

    void DisplayCurrentQuiz()
    {
        if (currentIndex < quizzes.Count)
        {
            M1QuizData current = quizzes[currentIndex];
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
            wordWithBlankText.text = "Quiz Complete!";
            foreach (var btn in answerButtons) btn.gameObject.SetActive(false);
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
