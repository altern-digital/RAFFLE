using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class M3QuizAnswerItem : MonoBehaviour, IPointerClickHandler
{
    public M3QuizAnswer answerData;
    public TMP_Text answerText;

    public void SetAnswer(M3QuizAnswer answer)
    {
        answerData = answer;
        answerText.text = answer.answerText;
    }

    public void Select()
    {
        M3QuizManager.Instance?.SelectAnswer(answerData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Select();
    }
}
