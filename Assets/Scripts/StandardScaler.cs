using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;


[Serializable]


public class StandardScalerData
{
    public List<float> mean;
    public List<float> scale;
}

public class StandardScaler
{
    private List<float> mean;
    private List<float> scale;

    public StandardScaler(string jsonPath)
    {
        string json = File.ReadAllText(jsonPath);
        var data = JsonConvert.DeserializeObject<StandardScalerData>(json);
        mean = data.mean;
        scale = data.scale;
    }

    public float[] Transform(float[] input)
    {
        if (input.Length != mean.Count)
        {
            throw new Exception($"Input size ({input.Length}) does not match scaler size ({mean.Count})");
        }

        float[] output = new float[input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            output[i] = (input[i] - mean[i]) / scale[i];
        }

        return output;
    }

    public float[] InverseTransform(float[] input)
    {
        if (input.Length != mean.Count)
        {
            throw new Exception($"Input size ({input.Length}) does not match scaler size ({mean.Count})");
        }

        float[] output = new float[input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            output[i] = (input[i] * scale[i]) + mean[i];
        }

        return output;
    }
}
