using UnityEngine;
using TMPro;
using System.IO;
using System.Collections.Generic;

public class GhostDeltaTimeCalculator : MonoBehaviour
{
    [Tooltip("Transform della macchina del giocatore")]
    public Transform player;

    [Tooltip("TextMeshProUGUI dove mostrare il delta time")]
    public TextMeshProUGUI deltaText;

    private List<GhostFrame> ghostFrames;
    private float playerStartTime;

    void Start()
    {
        playerStartTime = Time.time;

        // Carica il file ghost usato anche nel GhostPlayer
        string path = Path.Combine(Application.persistentDataPath, "facile.json");
        if (!File.Exists(path))
        {
            Debug.LogWarning("GhostDeltaTimeCalculator: ghost non trovato.");
            return;
        }

        string json = File.ReadAllText(path);
        GhostData data = JsonUtility.FromJson<GhostData>(json);
        ghostFrames = data.frames;
    }

    void Update()
    {
        if (ghostFrames == null || ghostFrames.Count == 0) return;

        Vector3 playerPos = player.position;
        float tPlayer = Time.time - playerStartTime;

        // Trova il frame del ghost spatialmente più vicino
        float minDist = float.MaxValue;
        float ghostTimeAtClosest = 0f;

        foreach (var frame in ghostFrames)
        {
            float dist = Vector3.Distance(playerPos, frame.position);
            if (dist < minDist)
            {
                minDist = dist;
                ghostTimeAtClosest = frame.time;
            }
        }

        float delta = tPlayer - ghostTimeAtClosest;
        string sign = delta >= 0 ? "+" : "-";
        deltaText.text = $"Distacco: {sign}{Mathf.Abs(delta):F2} s";
    }
}
