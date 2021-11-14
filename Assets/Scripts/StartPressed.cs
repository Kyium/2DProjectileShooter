using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartPressed : MonoBehaviour
{
    [SerializeField] string StartSceneName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ButtonPressed()
    {
        SceneManager.LoadScene(StartSceneName);
    }
}
