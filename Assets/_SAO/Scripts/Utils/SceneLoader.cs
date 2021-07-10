using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

public enum SceneType
{
    Startup = 0,
    Interface = 1
}

public class SceneLoader : Singleton<SceneLoader>
{
    public static event Action SwitchSceneStart;
    public static event Action<SceneType> SwitchSceneComplete;

    private Coroutine loadRoutine;
    private int lastLoadedScene = -1;
    private int previousScene = -1;

    internal override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// Load the previous scene.
    /// </summary>
    /// <param name="index"></param>
    public void LoadPreviousScene(bool fade, Color? fadeColor = null)
    {
        LoadScene(previousScene, fade, fadeColor);
    }

    /// <summary>
    /// Load the scene with the index given
    /// </summary>
    /// <param name="index"></param>
    public void LoadScene(int index, bool fade, Color? fadeColor = null)
    {
        // Check if we are not loading the same scene again
        if (lastLoadedScene == index)
            return;

        previousScene = lastLoadedScene;
        lastLoadedScene = index;
        if (loadRoutine != null)
            return;

        SwitchSceneStart?.Invoke();

        if (fade)
        {
            OverlayEffects.Instance.FadeInOverlay(1, fadeColor ?? Color.black, 0, () =>
            {
                loadRoutine = StartCoroutine(LoadSceneRoutine(index, fade, fadeColor));
            });
        }
        else
        {
            loadRoutine = StartCoroutine(LoadSceneRoutine(index, fade, fadeColor));
        }
    }

    /// <summary>
    /// The routine where we hide the loading screen when done loading
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private IEnumerator LoadSceneRoutine(int index, bool fade, Color? fadeColor = null)
    {
        // Load the scene
        yield return SceneManager.LoadSceneAsync(index);
        SwitchSceneComplete?.Invoke((SceneType)lastLoadedScene);

        if (fade)
            OverlayEffects.Instance.FadeOutOverlay(1, fadeColor ?? Color.black);

        loadRoutine = null;
    }

    /// <summary>
    /// Method to fetch the last loaded scene
    /// </summary>
    /// <returns></returns>
    internal SceneType GetPreviousScene() { return (SceneType)previousScene; }
}