using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    AsyncOperation loadSceneOperation;
    private bool isComplete;
    private IEnumerator Start()
    {
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;

        loadSceneOperation = SceneManager.LoadSceneAsync(1);
        loadSceneOperation.allowSceneActivation = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<TestPlayer>(out var player))
        {
            GlobalFadeCanvas.Instance.On(() =>
            {
                loadSceneOperation.allowSceneActivation = true;
                SceneManager.LoadScene(2, LoadSceneMode.Additive);
            });
        }
    }
}
