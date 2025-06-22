using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChoiceManager4 : MonoBehaviour
{
    public static ChoiceManager4 Instance;

    public List<AnimalData> animalDatabase;
    public int questionIndex;

    public AnimalData currentQuestion;
    public List<AnimalData> choices;

    void Awake()
    {
        Instance = this;
    }

    void NextQuestion()
    {
        questionIndex++;
        if (questionIndex < animalDatabase.Count)
        {
            currentQuestion = animalDatabase[questionIndex];
        }
        else
        {
            // Handle end of questions (e.g., show results)
        }
    }
}

public class AnimalData
{
    public AudioClip animalAudio;
    public string animalName;
}