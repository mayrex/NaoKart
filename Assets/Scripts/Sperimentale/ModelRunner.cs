using System;
using UnityEngine;
using Unity.Barracuda;

public class ModelRunner : MonoBehaviour
{
    public NNModel modelFile;
    private IWorker worker;

    public ScalerLoader scalerLoader;              // << Assegna via Inspector
    public TelemetryRecorder telemetryRecorder;    // << Assegna via Inspector

    private ScalerData scalerX;
    private ScalerData scalerY;

    void Start()
    {
        // Carica modello
        var model = ModelLoader.Load(modelFile);
        worker = model.CreateWorker();

        // Recupera scaler da ScalerLoader
        if (scalerLoader != null)
        {
            scalerX = scalerLoader.scalerX;
            scalerY = scalerLoader.scalerY;
        }

        if (scalerX == null || scalerY == null)
        {
            Debug.LogError("Scaler X o Y non sono stati caricati correttamente.");
        }
    }

    void FixedUpdate()
    {
        if (telemetryRecorder == null || scalerX == null || scalerY == null)
        {
            Debug.LogWarning("TelemetryRecorder o scaler mancanti.");
            return;
        }

        float[] allTelemetry = telemetryRecorder.GetCurrentTelemetryInput();

        if (allTelemetry == null || allTelemetry.Length < 18)
        {
            Debug.LogWarning("Dati di telemetria insufficienti.");
            return;
        }

        // Prende i 17 valori dopo Time.time
        float[] input = new float[17];
        Array.Copy(allTelemetry, 1, input, 0, 17);

        float[] standardizedInput = StandardScalerUtility.Standardize(input, scalerX);
        if (standardizedInput == null) return;

        using Tensor inputTensor = new Tensor(1, standardizedInput.Length, standardizedInput);

        worker.Execute(inputTensor);

        using Tensor outputTensor = worker.PeekOutput();
        float[] output = outputTensor.ToReadOnlyArray();

        // Assicurati che l'output del modello abbia la stessa lunghezza di scalerY (3 valori)
        if (output.Length != scalerY.mean.Count)
        {
            Debug.LogError("L'output del modello non corrisponde alla dimensione di scalerY.");
            return;
        }

        // Denormalizza solo i 3 valori dell'output
        float[] denormalizedOutput = StandardScalerUtility.Denormalize(output, scalerY);

        if (denormalizedOutput != null)
        {
            Debug.Log("Output denormalizzato: " + string.Join(", ", denormalizedOutput));
        }
        else
        {
            Debug.LogError("Errore nella denormalizzazione.");
        }
    }

    void OnDestroy()
    {
        worker?.Dispose();
    }
}
