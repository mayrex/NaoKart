using UnityEngine;
using Unity.Barracuda;

public class ONNXRunner : MonoBehaviour
{
    public NNModel modelAsset;
    private Model model;
    private IWorker worker;
    float[] rawInput = new float[]
  {
            475.0f, -44.0f, 0.0f, -160.0f, 0.0f, 0.0f, -0.03f, 0.03f, -0.18f,
            31.0f, 2.6f, 0.1f, 0.06f, 170f, 307f, 203f, 108f
  };
    void Start()
    {
        model = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, model);
        RunInference(rawInput);
      
    }

    public float[] RunInference(float[] inputArray)
    {
        Debug.Log("Model expected input shape: " + string.Join(",", model.inputs[0].shape));
        Debug.Log("Actual input length: " + inputArray.Length);

        if (inputArray.Length != model.inputs[0].shape.Length)
        {
            Debug.LogWarning("Dimensione input non corretta per il modello.");
            return null;
        }

        Tensor input = new Tensor(1, inputArray.Length, inputArray);
        worker.Execute(input);
        Tensor output = worker.PeekOutput();
        float[] result = output.ToReadOnlyArray();

        input.Dispose();
        output.Dispose();

        return result;
    }

    private void OnDestroy()
    {
        worker?.Dispose();
    }
}
