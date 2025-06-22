using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class M1QuizAnswerItem : MonoBehaviour, IPointerClickHandler
{
    public TMP_Text answerText;
    private char answerChar;

    public void SetAnswer(char c)
    {
        answerChar = c;
        answerText.text = c.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        M1QuizManager.Instance?.SelectAnswer(answerChar);
    }
}
