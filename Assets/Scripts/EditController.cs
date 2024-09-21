using SandboxGame;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


namespace SandboxGame {

    /// <summary>
    /// Controls the overall editor state
    /// Edit context
    /// </summary>
    public class EditController : Singleton<EditController>
    {
        //Public

        public class ProjectInfo
        {
            string name;
            string osPath;
        }

        public enum ProjectLoadState { NONE, UNLOADED, LOADED }

        [Header("DEBUG")]
        public ToolType currentToolType;

        public TouchManager tManager;
        public ObjectManager oManager;
        public DragController dragController;
        public DragTarget dragTarget;

        public ToolBase currentTool;

        public Material spritedefMaterial;
        public Material outlineMaterial;

        [Header("CAMERA")]
        public float zoomOrthMin;
        public float zoomOrthMax;

        //Private 

        /// <summary>
        /// Selected object (if any)
        /// </summary>
        [SerializeField]
        ObjectBase selectedObject;

        /// <summary>
        /// The project is loaded or not
        /// </summary>
        [SerializeField]
        ProjectLoadState projState;

        /// <summary>
        /// Current info of the project if any
        /// </summary>
        ProjectInfo projectInfo;

        //Camera related
        private Vector2 camDragOrigin;

        // Start is called before the first frame update
        void Start()
        {
            projState = ProjectLoadState.UNLOADED;
            tManager = TouchManager.Instance;
            oManager = ObjectManager.Instance;
        }

        // Update is called once per frame
        void Update()
        {
            //Update the current active tool
            currentTool?.OnToolUpdate();

            //Process Input
            ProcessInput();

            ProcessCameraInput();

        }

        private void OnDrawGizmos()
        {
            currentTool?.OnDrawGizmos();
        }

        public void ProcessInput()
        {
            // verify pointer is not on top of GUI; if it is, return
            if (EventSystem.current.IsPointerOverGameObject()) return;

            //Check if clicked on something
            if (Input.GetMouseButtonUp(0))
            {
                //Later Have to check if other tools are not active

                if (ToolCheck())
                {
                    var rB = PhysicsSimulatorManager.Instance.Get2dRigidbodyAtPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition), 1 << LayerMask.NameToLayer("Object"));

                    if (rB)//If a rigidbody is present
                    {
                        SelectObject(rB.GetComponent<ObjectBase>());
                    }
                    else//Nothing is clicked
                    {
                        SelectObject(null);
                    }
                }
            }
        }

        void ProcessCameraInput()
        {
            //Process camera pan
            {
                if (Input.GetMouseButtonDown(1))
                {
                    camDragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }

                if (Input.GetMouseButton(1))
                {
                    Vector2 diff = new Vector3(camDragOrigin.x, camDragOrigin.y) - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Camera.main.transform.position += new Vector3(diff.x, diff.y);
                }
            }

            //Process camera zoom
            {
                CameraZoom(Input.GetAxis("Mouse ScrollWheel"));
            }

        }

        /// <summary>
        /// Set active tool of type
        /// </summary>
        /// <param name="type"></param>
        public void SetTool(ToolType type)
        {
            if (currentTool == null)
            {
                currentTool = CreateToolOfType(type);
                currentTool.OnToolSelected();
            }
            else if (currentTool != null && currentTool.type != type)
            {
                //Call deselected on previous tool
                currentTool.OnToolDeselected();

                currentTool = CreateToolOfType(type);
                currentTool.OnToolSelected();

            }

            currentToolType = type;
        }


        /// <summary>
        /// Some custom tool checking
        /// </summary>
        /// <returns>True if should process the click event</returns>
        bool ToolCheck()
        {
            bool output = false;

            switch (currentToolType)
            {
                case ToolType.NONE:
                case ToolType.EDIT_ROTATE:
                case ToolType.EDIT_SCALE:
                case ToolType.EDIT_DRAG:
                case ToolType.EDIT_MOVE:
                    output = true;
                    break;
                case ToolType.DRAW_RECT:
                case ToolType.DRAW_CIRCLE:
                case ToolType.DRAW_TRI:
                    break;
            }
            return output;
        }

        /// <summary>
        /// Given a type creates a tool object
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        ToolBase CreateToolOfType(ToolType type)
        {
            ToolBase tool = null;

            switch (type)
            {
                case ToolType.NONE:
                    break;
                case ToolType.DRAW_RECT:
                    tool = new ToolDrawRect(this);
                    break;
                case ToolType.DRAW_CIRCLE:
                    tool = new ToolDrawCircle(this);
                    break;
                case ToolType.DRAW_TRI:
                    tool = new ToolDrawTri(this);
                    break;
                case ToolType.EDIT_MOVE:
                    tool = new ToolEditMove(this);
                    break;
                case ToolType.EDIT_ROTATE:
                    tool = new ToolEditRotate(this);
                    break;
                case ToolType.EDIT_SCALE:
                    break;
                case ToolType.EDIT_DRAG:
                    tool = new ToolDrag(this);
                    break;
                case ToolType.Count:
                    break;
                default:
                    break;
            }

            return tool;
        }

        void CameraZoom(float incr)
        {
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - incr, zoomOrthMin, zoomOrthMax);
        }

        //---------------------
        //Helpers
        //---------------------


        /// <summary>
        /// Enable/Disable the outline for this object
        /// </summary>
        /// <param name="objectBase"></param>
        void EnableOutline(ObjectBase objectBase, bool enable = true)
        {
            if (enable)
            {
                var spRend = objectBase.transform.GetComponentInChildren<SpriteRenderer>();
                spRend.material = new Material(outlineMaterial);
            }
            else
            {
                var spRend = objectBase.transform.GetComponentInChildren<SpriteRenderer>();
                spRend.material = spritedefMaterial;
            }
        }

        //------------------------------
        //Selection
        //------------------------------

        /// <summary>
        /// "Selects" an object
        /// Updates inspector
        /// </summary>
        public void SelectObject(ObjectBase obj)
        {

            if (obj != null)
            {
                ObjectManager.Instance.objectLinker.Link(obj, UIManager.Instance.inspectorPanel);

                if (selectedObject)
                    EnableOutline(selectedObject, false);

                EnableOutline(obj, true);
            }
            else
            {
                ObjectManager.Instance.objectLinker.Link(null, UIManager.Instance.inspectorPanel);

                if (selectedObject)
                    EnableOutline(selectedObject, false);
            }

            selectedObject = obj;
        }



        ///------------------------------------------------------------------------
        //                      	STATES
        ///------------------------------------------------------------------------
        public void ChangeState(State stateToChangeTo)
        {
            //stateMachine.ChangeState(stateToChangeTo);
        }

        private void State_Splash_OnEnter(StateMachine _StateMachine)
        {
            // Load Scene Here
            //SceneManager.LoadScene(sceneDataDictionary.GetSceneString(SceneName.Splash));
        }
        private void State_Splash_OnUpdate(StateMachine _StateMachine)
        {
            // Show legal and splash videos here
            //stateMachine.ChangeState(state_Init);
        }
        private void State_Splash_OnExit(StateMachine _StateMachine)
        {

        }
    }
}