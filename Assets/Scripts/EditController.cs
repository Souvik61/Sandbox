using SandboxGame;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;


namespace SandboxGame {

    /// <summary>
    /// Controls the overall editor state
    /// Edit context
    /// </summary>
    public class EditController : Singleton<EditController>
    {

        [Header("DEBUG")]
        public ToolType currentToolType;

        public TouchManager tManager;
        public ObjectManager oManager;
        public DragController dragController;

        public ToolBase currentTool;

        public Material spritedefMaterial;
        public Material outlineMaterial;

        //Private 

        /// <summary>
        /// Selected object (if any)
        /// </summary>
        [SerializeField]
        ObjectBase selectedObject;

        // Start is called before the first frame update
        void Start()
        {
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
                    output = true;
                    break;
                case ToolType.EDIT_MOVE:
                case ToolType.DRAW_RECT:
                case ToolType.DRAW_CIRCLE:
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
                case ToolType.EDIT_MOVE:
                    tool = new ToolEditMove(this);
                    break;
                case ToolType.EDIT_ROTATE:
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