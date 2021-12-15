using UnityEngine;
using UnityEngine.SceneManagement;

public class Preloader : MonoBehaviour
{
    private void Awake()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i<args.Length; i++)
        {
            if (args[i] == "-server")
            {
                SceneManager.LoadScene("GameScene");
            }
        }
    }
}
