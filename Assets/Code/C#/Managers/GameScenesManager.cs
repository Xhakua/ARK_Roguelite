using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameScenesManager : MonoBehaviour
{
    public enum SceneEnum
    {
        MainMenu,
        MainWorld,
        Dreamland,
        Loading,
        Tutorial,
        AdventurersGuild,
        Cave




    }
    public static GameScenesManager Instance { get; private set; }
    private static SceneEnum targetScene;


    public static void LoadScene(SceneEnum sceneEnum)
    {
        GameScenesManager.targetScene = sceneEnum;
        SceneManager.LoadScene(SceneEnum.Loading.ToString());
    }

    public static void LoadSceneCallback()
    {
        SceneManager.LoadScene(GameScenesManager.targetScene.ToString());
    }
}
