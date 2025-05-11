using UnityEngine;
using Unity.Barracuda;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

public class ONNXInferenceController : MonoBehaviour
{
    public NNModel onnxModelAsset;  // Dragga il modello .onnx da Unity
    private IWorker worker;
    private StandardScaler inputScaler;
    private StandardScaler outputScaler;

    void Start()
    {
        // === 1. Carica modello ONNX ===
        var model = ModelLoader.Load(onnxModelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, model);

        // === 2. Carica gli scaler ===
        string scalerXPath = Path.Combine(Application.streamingAssetsPath, "standard_scaler_x.json");
        string scalerYPath = Path.Combine(Application.streamingAssetsPath, "standard_scaler_y.json");
        inputScaler = new StandardScaler(scalerXPath);
        outputScaler = new StandardScaler(scalerYPath);

        // === 3. Esegui esempio di inferenza ===
        float[] rawInput = new float[]
        {
            475.0f, -44.0f, 0.0f, -160.0f, 0.0f, 0.0f, -0.03f, 0.03f, -0.18f,
            31.0f, 2.6f, 0.1f, 0.06f, 170f, 307f, 203f, 108f
        };

        float[] predicted = Predict(rawInput);

        Debug.Log($"Predicted output: Steering={predicted[0]:F3}, Throttle={predicted[1]:F3}, Brake={predicted[2]:F3}");
    }

    public float[] Predict(float[] input)
    {
        // === 1. Normalizza l'input ===
        float[] normalizedInput = inputScaler.Transform(input);

        // === 2. Crea Tensor e invia al modello ===
        Tensor inputTensor = new Tensor(1, normalizedInput.Length);
        for (int i = 0; i < normalizedInput.Length; i++)
            inputTensor[0, i] = normalizedInput[i];

        worker.Execute(inputTensor);
        Tensor outputTensor = worker.PeekOutput(); // o .PeekOutput("output") se hai un nome specifico

        // === 3. Estrai output e denormalizza ===
        float[] output = new float[outputTensor.length];
        for (int i = 0; i < outputTensor.length; i++)
            output[i] = outputTensor[i];

        float[] realOutput = outputScaler.InverseTransform(output);
        Debug.Log($"Predicted output: Steering={realOutput[0]:F3}, Throttle={realOutput[1]:F3}, Brake={realOutput[2]:F3}");

        // Cleanup
        inputTensor.Dispose();
        outputTensor.Dispose();

        return realOutput;
    }

    void OnDestroy()
    {
        worker?.Dispose();
    }
}
