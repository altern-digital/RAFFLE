

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class M4QuizAnswerItem : MonoBehaviour, IPointerClickHandler
{
    public M4QuizAnswer answerData;
    public TMP_Text answerText;

    public void SetAnswer(M4QuizAnswer answer)
    {
        answerData = answer;
        answerText.text = answerData.animalData.name;
    }

    public void Select()
    {
        M4QuizManager.Instance?.SelectAnswer(answerData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Select();
    }
}