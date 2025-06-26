using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class M3QuizAnswerItem : MonoBehaviour, IPointerClickHandler
{
    public M3QuizChoice answerData;
    public TMP_Text answerText;

    public void SetAnswer(M3QuizChoice answer)
    {
        answerData = answer;
        answerText.text = answer.value.ToString();
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
