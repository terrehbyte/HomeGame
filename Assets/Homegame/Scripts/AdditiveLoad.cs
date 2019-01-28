using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveLoad : MonoBehaviour
{
    public string sceneName;

    void Start()
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }
}