using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static void LoadGame(string Name)
    {
        AsyncOperation op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Name");
        op.allowSceneActivation = false;
        //Run smooth anim here
        op.allowSceneActivation = true;
    }
}
