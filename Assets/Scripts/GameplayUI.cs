using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] GameObject HealthDisplay;
    // Start is called before the first frame update
    void Start()
    {
        HealthDisplay.GetComponent<Text>().text = "Health: 100";
    }

    // Update is called once per frame
    void Update()
    {

    }
}
