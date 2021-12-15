using UnityEngine;
using UnityEngine.SceneManagement;

public class StartPressed : MonoBehaviour
{
    [SerializeField] string StartSceneName = "GameScene";
    public void ButtonPressed()
    {
        SceneManager.LoadScene(StartSceneName);
    }
}
