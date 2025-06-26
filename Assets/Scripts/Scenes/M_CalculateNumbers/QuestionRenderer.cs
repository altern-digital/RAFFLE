using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionRenderer : MonoBehaviour
{
    public GOListActivator leftObjectList;
    public GOListActivator rightObjectList;
    public TMP_Text operationText;
    public RectTransform questionContainer;

    public int leftObjectCount = 0;
    public int rightObjectCount = 0;
    public bool isDecrement;

    public void SetQuestion(int leftCount, int rightCount, bool decrement)
    {
        leftObjectCount = leftCount;
        rightObjectCount = rightCount;
        isDecrement = decrement;

        leftObjectList.activationIndex = leftObjectCount;
        rightObjectList.activationIndex = rightObjectCount;
        operationText.text = isDecrement ? "-" : "+";

        LayoutRebuilder.ForceRebuildLayoutImmediate(questionContainer);
    }


    public int GetCorrectAnswer()
    {
        if (isDecrement)
        {
            return leftObjectCount - rightObjectCount;
        }
        else
        {
            return leftObjectCount + rightObjectCount;
        }
    }
}
