using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PermanentParameterManager.instance.permanentParameters.ResetParameters();
    }

    public void PlayGame()
    {
        SceneTransitionManager.Instance.ChangeScene("TrainingScene");
    }

    public void BackToMenu()
    {
        SceneTransitionManager.Instance.ChangeScene("MenuScene");
    }

    public void StopGame()
    {
        Application.Quit();
    }
}
