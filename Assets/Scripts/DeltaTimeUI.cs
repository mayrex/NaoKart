using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeltaTimeUI : MonoBehaviour
{
    [Header("Delta Bar")]
    public RectTransform barFill;
    public float maxDelta = 2.0f; // massimo delta in secondi (es. ±2.0)
    public float maxWidth = 200f; // larghezza massima della barra (px)

    [Header("Lap Times")]
    public TextMeshProUGUI bestLapText;
    public TextMeshProUGUI lastLapText;

    public void UpdateDeltaBar(float delta)
    {
        delta = Mathf.Clamp(delta, -maxDelta, maxDelta);
        float width = (delta / maxDelta) * maxWidth;

        // Sposta e ridimensiona la barra
        barFill.anchoredPosition = new Vector2(width / 2f, 0); // centro + offset
        barFill.sizeDelta = new Vector2(Mathf.Abs(width), barFill.sizeDelta.y);

        // Colore verde o rosso
        Color color = delta < 0 ? Color.green : Color.red;
        barFill.GetComponent<Image>().color = color;
    }

    public void UpdateLapTimes(float best, float last)
    {
        bestLapText.text = $"Best Lap: {best:F2}s";
        lastLapText.text = $"Last Lap: {last:F2}s";
    }
}
