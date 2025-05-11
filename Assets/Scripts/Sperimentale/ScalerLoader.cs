using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class ScalerData
{
    public List<float> mean;
    public List<float> scale;
}

public static class StandardScalerUtility
{
    // Restituisce un array di float normalizzati secondo i parametri del tuo scaler
    public static float[] Standardize(float[] input, ScalerData scaler)
    {
        if (input.Length != scaler.mean.Count || input.Length != scaler.scale.Count)
        {
            Debug.LogError("Dimensioni input e scaler non corrispondono.");
            return null;
        }

        float[] standardized = new float[input.Length];

        for (int i = 0; i < input.Length; i++)
        {
            standardized[i] = (input[i] - scaler.mean[i]) / scaler.scale[i];
        }

        return standardized;

    }
    // Metodo per denormalizzare un array di float
    public static float[] Denormalize(float[] standardized, ScalerData scaler)
    {
        if (standardized.Length != scaler.mean.Count || standardized.Length != scaler.scale.Count)
        {
            Debug.LogError("Dimensioni output e scaler non corrispondono.");
            return null;
        }

        float[] denormalized = new float[standardized.Length];

        for (int i = 0; i < standardized.Length; i++)
        {
            denormalized[i] = standardized[i] * scaler.scale[i] + scaler.mean[i];
        }

        return denormalized;
    }

    // Se vuoi standardizzare una lista
    public static List<float> Standardize(List<float> input, ScalerData scaler)
    {
        if (input.Count != scaler.mean.Count || input.Count != scaler.scale.Count)
        {
            Debug.LogError("Dimensioni input e scaler non corrispondono.");
            return null;
        }

        List<float> standardized = new List<float>();

        for (int i = 0; i < input.Count; i++)
        {
            standardized.Add((input[i] - scaler.mean[i]) / scaler.scale[i]);
        }

        return standardized;
    }
}
public class ScalerLoader : MonoBehaviour
{
    public ScalerData scalerX;
    public ScalerData scalerY;

    void Start()
    {
        LoadScalers();
    }

    void LoadScalers()
    {
        string pathX = Path.Combine(Application.streamingAssetsPath, "standard_scaler_x.json");
        string pathY = Path.Combine(Application.streamingAssetsPath, "standard_scaler_y.json");

        if (File.Exists(pathX) && File.Exists(pathY))
        {
            string jsonX = File.ReadAllText(pathX);
            string jsonY = File.ReadAllText(pathY);

            scalerX = JsonUtility.FromJson<ScalerData>(jsonX);
            scalerY = JsonUtility.FromJson<ScalerData>(jsonY);

            Debug.Log("Scaler X e Y caricati correttamente.");
        }
        else
        {
            Debug.LogError("File JSON non trovati in StreamingAssets.");
        }
    }
}
