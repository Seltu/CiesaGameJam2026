using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public Animator transitionAnim;
    private static SceneTransitionManager _instance;
    public static SceneTransitionManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
    }

    IEnumerator LoadScene(string sceneName)
    {
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync(sceneName);
        transitionAnim.SetTrigger("Start");
    }

    internal void ChangeSceneAddictive()
    {
        StartCoroutine(LoadSceneAddictive());
    }

    IEnumerator LoadSceneAddictive()
    {
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        transitionAnim.SetTrigger("Start");
    }

    public void CallTransitionOnly()
    {
        StartCoroutine(CallTransitionOnlyCoroutine());
    }

    IEnumerator CallTransitionOnlyCoroutine()
    {
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1);

        transitionAnim.SetTrigger("Start");
    }
}
