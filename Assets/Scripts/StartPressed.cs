using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPressed : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ButtonPressed()
    {
        this.GetComponent<Button>().enabled = false;
    }
}
