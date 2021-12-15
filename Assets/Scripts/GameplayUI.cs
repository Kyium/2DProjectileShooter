using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] GameObject HealthDisplay;
    [SerializeField] private GameObject FuelDisplay;
    [SerializeField] private GameObject JetpackDisplay;
    // Start is called before the first frame update
    private void Start()
    {
        HealthDisplay.GetComponent<Text>().text = "Health: 100";
        FuelDisplay.GetComponent<Text>().text = "Fuel: 150";
        JetpackDisplay.GetComponent<Text>().text = "Jetpack: 80";
    }
}
