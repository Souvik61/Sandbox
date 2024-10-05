using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;


namespace SandboxGame
{

    /// <summary>
    /// Controls the overall editor state
    /// Most of the request pass through this
    /// Edit context
    /// Bird's eye view of the editor
    /// </summary>
    public class EditController : Singleton<EditController>
    {
        //Public

        public class ProjectInfo
        {
            public string name;
            //Full path with filename
            public string osPath;
        }

        public enum ProjectLoadState { NONE, UNLOADED, LOADED }

        [Header("DEBUG")]
        public ToolType currentToolType;

        public TouchManager tManager;
        public ObjectManager oManager;
        public DragController dragController;
        public DragTarget dragTarget;

        [Header("UI")]
        public PNL_Shapes shapesPanel;
        public PNL_SaveMenu saveMenuPanel;

        public ToolBase currentTool;

        public Material spritedefMaterial;
        public Material outlineMaterial;

        [Header("CAMERA")]
        public float camZoomMultiplier;
        public float camZoomTime;
        public float camCurrentZoom;
        public float camTargetZoom;
        public float camZoomOrthMin;
        public float camZoomOrthMax;
        public float camMoveTime;
        public float camMoveDeltaMultiplier;

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
        private float camZoomVelocity;
        private float camMoveVelocity;
        private Vector3 camTargetPosition;

        // Start is called before the first frame update
        void Start()
        {
            projState = ProjectLoadState.UNLOADED;
            tManager = TouchManager.Instance;
            oManager = ObjectManager.Instance;

            //Setup camera
            camCurrentZoom = Camera.main.orthographicSize;
            camTargetZoom = camCurrentZoom;

            // Set filters (optional)
            // It is sufficient to set the filters just once (instead of each time before showing the file browser dialog), 
            // if all the dialogs will be using the same filters
            FileBrowser.SetFilters(false, new FileBrowser.Filter("Json", ".json"));

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
            if (FilesystemManager.Instance.IsAnyDialogOpen) return;

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
                camTargetZoom = Mathf.Clamp(camTargetZoom - Input.GetAxis("Mouse ScrollWheel") * camZoomMultiplier, camZoomOrthMin, camZoomOrthMax);
                camCurrentZoom = Mathf.SmoothDamp(camCurrentZoom, camTargetZoom, ref camZoomVelocity, camZoomTime);

                Vector2 mouseWorldPosBeforeZoom = TouchManager.Instance.MousePositionWorld;

                SetCameraZoom(camCurrentZoom);

                Vector2 mouseWorldPosAfterZoom = TouchManager.Instance.MousePositionWorld;
                Vector3 diff = mouseWorldPosBeforeZoom - mouseWorldPosAfterZoom;

                //If diff in world pos
                if (Mathf.Abs(diff.magnitude) > 0.005f)
                {
                    //TouchManager.Instance.MousePositionWorld
                    Camera.main.transform.position = Camera.main.transform.position + diff;
                }


            }

        }

        /// <summary>
        /// Should be called from ui for tool changing
        /// </summary>
        /// <param name="type"></param>
        public void SetToolWithChecking(ToolType type)
        {
            if (projectInfo == null)
            {
            }
            else
            {
                SetToolUI(type);
                SetTool(type);
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
        /// Update ui to this tool 
        /// </summary>
        /// <param name="type"></param>
        void SetToolUI(ToolType type)
        {
            switch (type)
            {
                case ToolType.NONE:
                    break;
                case ToolType.DRAW_RECT:
                    shapesPanel.EnableButtonOutlineOnly("RECT");
                    break;
                case ToolType.DRAW_CIRCLE:
                    shapesPanel.EnableButtonOutlineOnly("CIRCLE");
                    break;
                case ToolType.DRAW_TRI:
                    shapesPanel.EnableButtonOutlineOnly("TRI");
                    break;
                case ToolType.EDIT_MOVE:
                    shapesPanel.EnableButtonOutlineOnly("MOVE");
                    break;
                case ToolType.EDIT_ROTATE:
                    shapesPanel.EnableButtonOutlineOnly("ROTATE");
                    break;
                case ToolType.EDIT_SCALE:
                    break;
                case ToolType.EDIT_DRAG:
                    shapesPanel.EnableButtonOutlineOnly("DRAG");
                    break;
                case ToolType.Count:
                    break;
                default:
                    break;
            }

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
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - incr, camZoomOrthMin, camZoomOrthMax);
        }

        void SetCameraZoom(float zoom)
        {
            Camera.main.orthographicSize = Mathf.Clamp(zoom, camZoomOrthMin, camZoomOrthMax);
        }


        //--------------------
        //Events
        //--------------------

        public void OnNewButtonClicked()
        {
            StartCoroutine(NewFileRoutine());
        }

        public void OnLoadButtonClicked()
        {
            StartCoroutine(LoadFileRoutine());
        }

        public void OnSaveButtonClicked()
        {
            StartCoroutine(SaveFileRoutine());
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


        //------------------------
        //Coroutines
        //------------------------

        /// <summary>
        /// Coroutine when new file button is pressed
        /// </summary>
        /// <returns></returns>
        IEnumerator NewFileRoutine()
        {
            yield return FilesystemManager.Instance.OpenNewDialogRoutine();

            if (!FileBrowser.Success) yield break;

            //Get path
            string path = FileBrowser.Result[0];
            string fName, dir;

            ExtractPathAndName(path, out dir, out fName);

            Debug.Log("Path: " + path);
            Debug.Log("Filename: " + fName);

            //Setup project info
            projectInfo = new ProjectInfo() { name = fName, osPath = dir };
            projState = ProjectLoadState.LOADED;

            oManager.objectList.Clear();
            //Set project input field text to fName
            saveMenuPanel.projectInputField.text = fName;


            //Done
        }

        /// <summary>
        /// Coroutine when save file button is pressed
        /// </summary>
        /// <returns></returns>
        IEnumerator SaveFileRoutine()
        {
            Debug.Log("Save!");
            //If no project loaded
            if (projectInfo == null)
            {
                ToastNotification.Show("No project loaded");
                yield break;
            }

            //Validate all rigid body model before saving
            //oManager->rbManager->computeAllRigidBodies();

            string json = SerializeProject();

            Debug.Log(json);

            //string fullPath = Path.Combine(projectInfo.osPath, projectInfo.name);

            bool saveSuccess = FilesystemManager.Instance.SaveToFile(json, projectInfo.osPath);

            if (saveSuccess)
            {

            }
            else
            {
                ToastNotification.Show("Something went wrong");
                yield break;
            }

            //try
            //{
            //    //Create directory if it doesnt exists
            //    Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            //
            //    //Write serialized data to file
            //    using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            //    {
            //        using (StreamWriter writer = new StreamWriter(stream))
            //        {
            //            writer.Write(json);
            //        }
            //    }
            //
            //}
            //catch (System.Exception e)
            //{
            //
            //    Debug.LogError("Error while storing data to file: " + fullPath + "\n" + e);
            //    ToastNotification.Show("Something went wrong");
            //    yield break;
            //
            //}

            //oManager->prjManager->saveFile();
            //
            //oManager->sTracker->setAllModelClean();
            //

            //Show saved notification
            ToastNotification.Show("Saved successfully.");
        }

        /// <summary>
        /// Coroutine when save file button is pressed
        /// </summary>
        /// <returns></returns>
        IEnumerator LoadFileRoutine()
        {

            //std::string fP = FileDialogs::openFile("JSON (*.json)\0*.json\0");
            yield return FilesystemManager.Instance.OpenLoadDialogRoutine();

            if (!FileBrowser.Success) yield break;

            //Get path
            string path = FileBrowser.Result[0];
            string fName, dir;

            ExtractPathAndName(path, out dir, out fName);
            //string fullPath = Path.Combine(dir, fName);
            var fileData = FilesystemManager.Instance.LoadFromFile(dir);

            var jsonData = JsonUtility.FromJson<SaveJson>(fileData);

            //Deserialize project
            DeserializeProject(jsonData);

            //Setup project info
            projectInfo = new ProjectInfo() { name = fName, osPath = dir };

            saveMenuPanel.projectInputField.text = fName;


            //    oManager->rbManager->clearModels();
            //    oManager->prjManager->loadFileNew(fP);
            //    oManager->rbManager->internalUpdate();
            //
            //    //Set ui panel display of project name
            //    std::string a = oManager->prjManager->projectName;
            //    oManager->uiSystem->prjPanelUI->setProjectNameText(a);
            //
            //    oManager->rbManager->selectModelByIndex(0);
            //

            //Less goooooo!!
        }

        //-----------------------
        //Serialize/Deserialize
        //-----------------------


        ObjectRectJson ObjectRectToJson(ObjectRect obj)
        {
            return new ObjectRectJson()
            {
                name = obj.name,
                size = obj.size,
                type = "RECT",
                position = obj.transform.position,
                rotation = obj.transform.eulerAngles.z
            };
        }

        ObjectCircJson ObjectCircToJson(ObjectCircle obj)
        {
            return new ObjectCircJson()
            {
                name = obj.name,
                radius = obj.radius,
                type = "CIRCLE",
                position = obj.transform.position,
                rotation = obj.transform.eulerAngles.z
            };
        }

        ObjectTriJson ObjectTriToJson(ObjectTriangle obj)
        {
            return new ObjectTriJson()
            {
                name = obj.name,
                size = obj.size,
                type = "TRIANGLE",
                position = obj.transform.position,
                rotation = obj.transform.eulerAngles.z
            };
        }

        /// <summary>
        /// Serialize all gameobjects in array
        /// </summary>
        SaveJson SerializeGameObjects()
        {
            SaveJson saveJson = new SaveJson();

            saveJson.gameObjectsRect = new List<ObjectRectJson>();
            saveJson.gameObjectsCircle = new List<ObjectCircJson>();
            saveJson.gameObjectsTriangle = new List<ObjectTriJson>();

            List<ObjectJson> jsonObjectList = new List<ObjectJson>();

            foreach (var item in oManager.objectList)
            {
                //ObjectJson obj = null;

                switch (item.type)
                {
                    case ObjectType.CIRCLE:
                        {
                            saveJson.gameObjectsCircle.Add(ObjectCircToJson((ObjectCircle)item));
                        }
                        break;
                    case ObjectType.RECT:
                        {
                            saveJson.gameObjectsRect.Add(ObjectRectToJson((ObjectRect)item));
                        }
                        break;
                    case ObjectType.TRIANGLE:
                        {
                            saveJson.gameObjectsTriangle.Add(ObjectTriToJson((ObjectTriangle)item));
                        }
                        break;
                    default:
                        break;
                }
            }

            return saveJson;
        }

        string SerializeProject()
        {
            SaveJson saveJson = SerializeGameObjects();

            return JsonUtility.ToJson(saveJson);
        }

        /// <summary>
        /// Setup this project as this json data
        /// </summary>
        /// <param name="jsonData"></param>
        void DeserializeProject(SaveJson jsonData)
        {
            //Spawn Rects
            foreach (var item in jsonData.gameObjectsRect)
            {
                oManager.SpawnRectInternal(item.position, item.size, item.rotation);
            }

            //Spawn Circles
            foreach (var item in jsonData.gameObjectsCircle)
            {
                oManager.SpawnCircleInternal(item.position, item.radius, item.rotation);
            }

            //Spawn Triangles
            foreach (var item in jsonData.gameObjectsTriangle)
            {
                oManager.SpawnTriangleInternal(item.position, item.size, item.rotation);
            }
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

        void ExtractPathAndName(string path, out string dir, out string file)
        {
#if UNITY_ANDROID
            file=FileBrowserHelpers.GetFilename(path);
            dir = path;
#else
            dir = path;
            file = Path.GetFileName(path);
#endif

        }

        string TypeToString(ObjectType type)
        {
            return type.ToString();
        }



    }
}