using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChoiceItem4 : MonoBehaviour, IPointerClickHandler
{
    public AnimalData animalData;
    public TMP_Text animalNameText;

    public void SetChoice(AnimalData data)
    {
        animalData = data;
        animalNameText.text = data.animalName;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ChoiceManager4 choiceManager = ChoiceManager4.Instance;

        if (choiceManager != null)
        {
            choiceManager.currentQuestion = animalData;
            choiceManager.choices.Clear();
            choiceManager.choices.Add(animalData);
            choiceManager.questionIndex++;
        }
    }
}
