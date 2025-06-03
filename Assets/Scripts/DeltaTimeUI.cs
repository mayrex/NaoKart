using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeltaTimeUI : MonoBehaviour
{


    [Header("Lap Times")]
    public TextMeshProUGUI bestLapText;
    public TextMeshProUGUI lastLapText;



    public void UpdateLapTimes(float best, float last)
    {
        bestLapText.text = $"Best Lap: {best:F2}s";
        lastLapText.text = $"Last Lap: {last:F2}s";
    }
}
