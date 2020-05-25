using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//  https://www.youtube.com/watch?v=YMj2qPq9CP8
public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private GameObject loadingUI;

    [SerializeField]
    private Slider progressBar;

    public void LoadScene(string name)
    {
        StartCoroutine(LoadAsync(name));
    }

    IEnumerator LoadAsync(string name)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(name);
        loadingUI.SetActive(true);

        while (!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // operation.progress stops at 0.9f apparently...
            progressBar.value = progress;

            yield return null;
        }
    }
}
