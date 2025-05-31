using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SandboxGame
{

    /// <summary>
    /// Use this manager to control the simulator not otherwise
    /// </summary>
    public class PhysicsSimulatorManager : MonoBehaviour
    {

        public static PhysicsSimulatorManager Instance;

        public int pickableLayer;

        public bool SimRunning { get => IsRunning; }

        public enum SimulationState { RUNNING, PAUSED };

        public SimulationState simState;

        public bool IsRunning => simState == SimulationState.RUNNING;

        private void Awake()
        {
            if (Instance == null)
            {
                simState = SimulationState.PAUSED;
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
        /// Run the simulator (deprecated)
        /// </summary>
        public void RunSimulation()
        {
            if (IsRunning) return;

            ChangeState(SimulationState.RUNNING);

        }

        /// <summary>
        /// Pause the simulator (deprecated)
        /// </summary>
        public void PauseSimulation()
        {
            if (!IsRunning) return;

            ChangeState(SimulationState.PAUSED);

        }

        /// <summary>
        /// Run the simulation for these objects
        /// </summary>
        public void RunSimulation(List<GameObject> objects)
        {
            if (IsRunning) return;

            //Set list of objects to be kinematic
            SetKinematic(objects, false);

            ChangeState(SimulationState.RUNNING);

        }

        /// <summary>
        /// Run the simulation for these objects
        /// </summary>
        public void RunSimulation(List<ObjectBase> objects)
        {
            if (IsRunning) return;

            List<GameObject> lst = new();

            foreach (var item in objects)
            {
                if (item is ObjectCircle || item is ObjectRect || item is ObjectTriangle)
                {
                    if (((bool)item.GetProperty("_static").getter()) == false)
                    {
                        lst.Add(item.gameObject);
                    }
                }
            }

            //Set list of objects to be kinematic
            SetKinematic(lst, false);

            // for rope joint specially i have to do this 
            foreach (var item in objects)
            {
                if (item is ObjectRopeJoint)
                {
                    SetKinematicRope(item.transform, false);
                }
            }

            ChangeState(SimulationState.RUNNING);

        }

        /// <summary>
        /// Pause the simulation for these objects
        /// </summary>
        public void PauseSimulation(List<GameObject> objects)
        {
            if (!IsRunning) return;

            //Set list of objects to be kinematic
            SetKinematic(objects, true);

            ChangeState(SimulationState.PAUSED);

        }

        /// <summary>
        /// Pause the simulation for these objects
        /// </summary>
        public void PauseSimulation(List<ObjectBase> objects)
        {
            if (!IsRunning) return;

            List<GameObject> lst = new();

            foreach (var item in objects)
            {
                if (item is ObjectCircle || item is ObjectRect || item is ObjectTriangle)
                {
                    if (((bool)item.GetProperty("_static").getter()) == false)
                    {
                        lst.Add(item.gameObject);
                    }
                }
            }

            //Set list of objects to be kinematic
            SetKinematic(lst, true);

            // for rope joint specially i have to do this 
            foreach (var item in objects)
            {
                if (item is ObjectRopeJoint)
                {
                    SetKinematicRope(item.transform, true);
                }
            }

            ChangeState(SimulationState.PAUSED);

        }

        public string GetStatusText()
        {
            string status = "";

            switch (simState)
            {
                case SimulationState.RUNNING:
                    status = "RUNNING";
                    break;
                case SimulationState.PAUSED:
                    status = "PAUSED";
                    break;
                default:
                    break;
            }
            return status;
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
                if (rb)
                {
                    rb.bodyType = enable ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
                    rb.velocity = Vector2.zero;
                    rb.angularVelocity = 0.0f;
                }
            }
        }

        /// <summary>
        /// Given a rope set kinematic
        /// </summary>
        /// <param name="bodies"></param>
        public void SetKinematicRope(Transform ropeRoot, bool enable)
        {
            foreach (Transform item in ropeRoot)
            {
                var rb = item.GetComponent<Rigidbody2D>();
                if (rb)
                {
                    rb.bodyType = enable ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
                    rb.velocity = Vector2.zero;
                    rb.angularVelocity = 0.0f;
                }
            }
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

        /// <summary>
        /// Overlap if there is a 2d rigidbody at this world pos with layer mask
        /// </summary>
        /// <returns></returns>
        public Rigidbody2D Get2dRigidbodyAtPositionOverlap(Vector3 worldPos, int layerMask)
        {
            Collider2D hit = Physics2D.OverlapPoint(worldPos, layerMask);

            if (hit != null && ((1 << hit.gameObject.layer) & layerMask) != 0)
            {
                return hit.gameObject.GetComponent<Rigidbody2D>() ? hit.gameObject.GetComponent<Rigidbody2D>() : hit.gameObject.GetComponentInParent<Rigidbody2D>();
            }

            return null;
        }
    }
}