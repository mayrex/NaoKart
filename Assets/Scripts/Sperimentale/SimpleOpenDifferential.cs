using VehiclePhysics;
using System;
public class SimpleOpenDifferential : Block
{
    protected override void Initialize()
    {
        // Declare this block to have a single input and a two outputs

        SetInputs(1);
        SetOutputs(2);
    }

    public override bool CheckConnections()
    {
        // The input and both outputs are required to be connected to other blocks

        return inputs[0] != null && outputs[0] != null && outputs[1] != null;
    }

    public override void ComputeStateUpstream()
    {
        // The state of the input is the sum of the states of the outputs.
        //
        // NOTE: Inertias must be identical for this implementation to work. The calculation
        // for different inertias is more complex (see the Differential block in VPP).
        
        inputs[0].L = outputs[0].L + outputs[1].L;
        inputs[0].I = outputs[0].I + outputs[1].I;
        inputs[0].Tr = outputs[0].Tr + outputs[1].Tr;
    }

    public override void EvaluateTorqueDownstream()
    {
        // An open differential splits the input torque 50-50 between both outputs
        outputs[0].outTd = inputs[0].outTd * 0.5f;
        outputs[1].outTd = inputs[0].outTd * 0.5f;
    }
}