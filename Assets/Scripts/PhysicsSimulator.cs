using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSimulator : MonoBehaviour
{

    public enum SimulationState { RUNNING, PAUSED };

    public SimulationState simState;

    public bool IsRunning => simState == SimulationState.RUNNING;

    private void Awake()
    {
        simState = SimulationState.PAUSED;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void FixedUpdate()
    {
        if (simState == SimulationState.RUNNING)
        {
            Physics2D.Simulate(Time.fixedDeltaTime);
        }
    }

    public void ChangeState(SimulationState state)
    {
        simState = state;
    }

}
