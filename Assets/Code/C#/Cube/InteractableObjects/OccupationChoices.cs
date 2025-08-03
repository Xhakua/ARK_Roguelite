using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccupationChoices : MonoBehaviour, ICanMapInteraction
{
    public void OnMapInteraction()
    {
       UIManager.Instance.ShowOccupationSelectionUI();
    }
}
