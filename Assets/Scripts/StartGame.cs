using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<TestPlayer>(out var player))
        {
            SceneManager.LoadScene(1);
            SceneManager.LoadScene(2,LoadSceneMode.Additive);
            SceneManager.LoadScene(3,LoadSceneMode.Additive);
            SceneManager.LoadScene(4,LoadSceneMode.Additive);
        }
    }
}
