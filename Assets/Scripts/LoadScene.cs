using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class LoadScene : NetworkBehaviour
{
    [SerializeField] private string sceneToLoadName;

    [Command]
    void CmdLoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        Debug.Log(sceneName);
    }

    public void LoadButtonClick()
    {
        CmdLoadScene(sceneToLoadName);
    }
}
