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


    private void FixedUpdate()
    {
        if (simState == SimulationState.RUNNING)
        {
            //Physics2D.Simulate(Time.fixedDeltaTime);
        }
    }

    public void ChangeState(SimulationState state)
    {
        simState = state;
    }

    /// <summary>
    /// Given a list of rigidbodies set them kinematic/dynamic
    /// </summary>
    /// <param name="bodies"></param>
    public void SetKinematic(List<GameObject> bodies, bool enable)
    {
        foreach (var item in bodies)
        {
            var rb = item.GetComponent<Rigidbody2D>();
            rb.bodyType = enable ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0.0f;
        }
    }

}
