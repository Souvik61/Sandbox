using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use this manager to control the simulator not otherwise
/// </summary>
public class PhysicsSimulatorManager : MonoBehaviour
{

    public static PhysicsSimulatorManager Instance;

    public int pickableLayer;

    public PhysicsSimulator sim;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //RunSimulation();
    }

    /// <summary>
    /// Run the simulator
    /// </summary>
    public void RunSimulation()
    {
        if (sim.IsRunning) return;

        sim.ChangeState(PhysicsSimulator.SimulationState.RUNNING);

    }

    /// <summary>
    /// Pause the simulator
    /// </summary>
    public void PauseSimulation()
    {
        if (!sim.IsRunning) return;

        sim.ChangeState(PhysicsSimulator.SimulationState.PAUSED);

    }

    public string GetStatusText()
    {
        string status = "";

        switch (sim.simState)
        {
            case PhysicsSimulator.SimulationState.RUNNING:
                status = "RUNNING";
                break;
            case PhysicsSimulator.SimulationState.PAUSED:
                status = "PAUSED";
                break;
            default:
                break;
        }
        return status;
    }

    //-------------------------------------------------
    //Some utility functions can used by other classes
    //-------------------------------------------------

    /// <summary>
    /// Raycast if there is a 2d rigidbody at this world pos
    /// </summary>
    /// <returns></returns>
    public Rigidbody2D Get2dRigidbodyAtPosition(Vector3 worldPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject.layer == pickableLayer)
        {
            return hit.collider.gameObject.GetComponent<Rigidbody2D>() ? hit.collider.gameObject.GetComponent<Rigidbody2D>() : hit.collider.gameObject.GetComponentInParent<Rigidbody2D>();
        }

        return null;
    }

    /// <summary>
    /// Raycast if there is a 2d rigidbody at this world pos with layer mask
    /// </summary>
    /// <returns></returns>
    public Rigidbody2D Get2dRigidbodyAtPosition(Vector3 worldPos, int layerMask)
    {
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null && ((1 << hit.collider.gameObject.layer) & layerMask) != 0)
        {
            return hit.collider.gameObject.GetComponent<Rigidbody2D>() ? hit.collider.gameObject.GetComponent<Rigidbody2D>() : hit.collider.gameObject.GetComponentInParent<Rigidbody2D>();
        }

        return null;
    }
}
