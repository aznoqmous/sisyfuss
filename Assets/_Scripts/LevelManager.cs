using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
        _levelLoaderAnimator.SetBool("IsLoading", false);
    }

    bool _isLoading = false;
    [SerializeField] Animator _levelLoaderAnimator;

    public void LoadScene(string sceneName)
    {
        Debug.Log(sceneName);
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string scene)
    {
        _isLoading = true;
        _levelLoaderAnimator.SetBool("IsLoading", true);
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(scene);

        _levelLoaderAnimator.SetBool("IsLoading", false);
        _isLoading = false;
    }
}
