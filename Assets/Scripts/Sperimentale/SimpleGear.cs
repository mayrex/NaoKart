using VehiclePhysics;

public class SimpleGear : Block
{
    public float ratio = 1.0f;

    protected override void Initialize()
    {
        // Declare this block to have a single input and a single output

        SetInputs(1);
        SetOutputs(1);
    }

    public override bool CheckConnections()
    {
        // Both input and output are required to be connected to other blocks

        return inputs[0] != null && outputs[0] != null;
    }

    public override void ComputeStateUpstream()
    {
        // Take the state from the output connection, process it,
        // and put the result at the input connection (upstream flow).
        // L = angular momentum, I = inertia, Tr = reaction torque

        inputs[0].L = outputs[0].L / ratio;
        inputs[0].I = outputs[0].I / ratio / ratio;
        inputs[0].Tr = outputs[0].Tr / ratio;
    }

    public override void EvaluateTorqueDownstream()
    {
        // Take the torque from the input connection, process it,
        // and put the result at the output connection (downstream flow).

        outputs[0].outTd = inputs[0].outTd * ratio;
    }
}