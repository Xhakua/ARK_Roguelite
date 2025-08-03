using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deeper : MonoBehaviour, ICanMapInteraction
{
    public void OnMapInteraction()
    {
        GameScenesManager.LoadScene(GameScenesManager.SceneEnum.Cave);
    }
}
