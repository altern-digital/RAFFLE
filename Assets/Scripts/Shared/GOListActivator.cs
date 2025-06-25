using System.Collections.Generic;
using UnityEngine;

public class GOListActivator : MonoBehaviour
{
    public List<GameObject> gameObjects;

    public int activationIndex = 0;

    private void Update()
    {
        for (int i = 0; i < gameObjects.Count; i++) gameObjects[i]?.SetActive(i < activationIndex);
    }
}
