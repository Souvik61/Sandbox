using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using System.Linq;
using DynamicPanels;
using UnityEngine.Rendering.LookDev;
using System;
using Newtonsoft.Json;


namespace SandboxGame
{

    /// <summary>
    /// Controls the overall editor state
    /// Most of the high level request pass through this
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

        public PNL_ObjectBrowser objectBrowserPanel;

        [Header("CAMERA")]
        public float camZoomMultiplier;
        public float camZoomTime;
        public float camCurrentZoom;
        public float camTargetZoom;
        public float camZoomOrthMin;
        public float camZoomOrthMax;
        public float camMoveTime;
        public float camMoveDeltaMultiplier;

        /// <summary>
        /// Selected object (if any)
        /// </summary>
        [SerializeField]
        ObjectBase selectedObject;

        public ObjectBase SelectedObject { get => selectedObject; }

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

        //Color picker
        public DynamicPanelsCanvas dynamicPanelsCanvas;
        public RectTransform dummyColorPicker;
        private DynamicPanels.Panel _activeColorPickerPanel;

        public ColorManager ColorManager;

        private SaveJson? _lastLoadedProject;

        public Rope2DCreator RopeCreator;

        public PNL_Gizmo gizmoPanel;

        public void Init()
        {

        }

        // Start is called before the first frame update
        void Start()
        {
            projState = ProjectLoadState.UNLOADED;
            tManager = TouchManager.Instance;
            oManager = ObjectManager.Instance;

            ColorManager.Init(GameManager.Instance);

            objectBrowserPanel.Init(oManager, this);

            ObjectManager.Instance.objectLinker.Init(this);

            //Setup camera
            camCurrentZoom = Camera.main.orthographicSize;
            camTargetZoom = camCurrentZoom;

            // Set filters (optional)
            // It is sufficient to set the filters just once (instead of each time before showing the file browser dialog), 
            // if all the dialogs will be using the same filters
            FileBrowser.SetFilters(false, new FileBrowser.Filter("Json", ".json"));

            _lastLoadedProject = null;

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
                    //var rB = PhysicsSimulatorManager.Instance.Get2dRigidbodyAtPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition), 1 << LayerMask.NameToLayer("Object"));
                    var rB = PhysicsSimulatorManager.Instance.Get2dRigidbodyAtPositionOverlap(Camera.main.ScreenToWorldPoint(Input.mousePosition), 1 << LayerMask.NameToLayer("Object"));

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

            if (EventSystem.current.IsPointerOverGameObject()) return;

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
                case ToolType.WELD:
                    shapesPanel.EnableButtonOutlineOnly("WELD");
                    break;
                case ToolType.JOINT_SPRING:
                    shapesPanel.EnableButtonOutlineOnly("SPRING");
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
        //bool ToolCheck()
        //{
        //    bool output = false;
        //
        //    switch (currentToolType)
        //    {
        //        case ToolType.NONE:
        //        case ToolType.EDIT_SCALE:
        //        case ToolType.EDIT_DRAG:
        //        case ToolType.EDIT_MOVE:
        //            output = true;
        //            break;
        //        case ToolType.EDIT_ROTATE:
        //        case ToolType.DRAW_RECT:
        //        case ToolType.DRAW_CIRCLE:
        //        case ToolType.DRAW_TRI:
        //            break;
        //    }
        //    return output;
        //}

        /// <summary>
        /// Some custom tool checking
        /// </summary>
        /// <returns>True if should process the click event</returns>
        bool ToolCheck()
        {
            bool output = true;

            if (currentTool != null)
            {
                output = !currentTool.ShouldBlockOtherEvents();
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
                case ToolType.WELD:
                    tool = new ToolFixedJoint(this);
                    break;
                case ToolType.JOINT_SPRING:
                    tool = new ToolSpringJoint(this);
                    break;
                case ToolType.JOINT_ROPE:
                    tool = new ToolRopeJoint(this);
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

        /// <summary>
        /// On play button clicked from sim panel
        /// </summary>
        public void OnPlayButtonClicked()
        {
            //List<GameObject> objectList = oManager.objectList.Select(obj => obj.gameObject).ToList();
            //PhysicsSimulatorManager.Instance.RunSimulation(objectList);

            StartCoroutine(PlayButtonClickedRoutine());

        }

        /// <summary>
        /// On pause button clicked from sim panel
        /// </summary>
        public void OnPauseButtonClicked()
        {
            //List<GameObject> objectList = oManager.objectList.Select(obj => obj.gameObject).ToList();
            //PhysicsSimulatorManager.Instance.PauseSimulation(objectList);

            StartCoroutine(PauseButtonClickedRoutine());

        }

        /// <summary>
        /// On reset button clicked from sim panel
        /// </summary>
        public void OnResetButtonClicked()
        {
            //List<GameObject> objectList = oManager.objectList.Select(obj => obj.gameObject).ToList();
            //PhysicsSimulatorManager.Instance.PauseSimulation(objectList);
            //
            //ClearObjects();
            //
            //if (_lastLoadedProject.HasValue)
            //{
            //    DeserializeProject(_lastLoadedProject.Value);
            //}

            StartCoroutine(ResetButtonClickedRoutine());

        }

        public void OnColorPickButtonClicked()
        {
            var colorPicker = Instantiate(dummyColorPicker, dummyColorPicker.parent);

            // Create 3 panels
            DynamicPanels.Panel panel1 = PanelUtils.CreatePanelFor(colorPicker, dynamicPanelsCanvas);
            _activeColorPickerPanel = panel1;

            panel1[0].MinSize = new Vector2(400f, 400f); // first tab
            panel1.Detach();

            _activeColorPickerPanel = panel1;

            ObjectManager.Instance.objectLinker.Link(selectedObject, panel1.GetComponentInChildren<PNL_Color>());

            panel1.GetComponentInChildren<PNL_Color>().OnOkButtonPressed += () => { Destroy(panel1.gameObject); };
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

                if (!IsJointType(obj))
                {
                    EnableOutline(obj, true);
                }
                else // is joint type
                { 
                
                }

            }
            else
            {
                ObjectManager.Instance.objectLinker.Link(null, UIManager.Instance.inspectorPanel);

                if (selectedObject)
                    EnableOutline(selectedObject, false);
            }

            selectedObject = obj;

            //manual state update
            objectBrowserPanel.OnStateUpdated();

            //Send event to tools
            currentTool?.OnObjectSelected();

        }

        /// <summary>
        /// "Deletes" an object
        /// </summary>
        public void DeleteObject(ObjectBase obj)
        {
            if (obj != null)
            {
                oManager.DeleteObject(obj);
            }
        }

        //--------------------------
        //Simulation Events
        //--------------------------




        //------------------------
        //Coroutines
        //------------------------

        /// <summary>
        /// Coroutine when new file button is pressed
        /// </summary>
        /// <returns></returns>
        IEnumerator NewFileRoutine()
        {

            if (PhysicsSimulatorManager.Instance.SimRunning)
            {
                ToastNotification.Show("Cannot create while simulation is running.");
                yield break;
            }

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

            ClearObjects();
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
            //If no project loaded
            if (projectInfo == null)
            {
                ToastNotification.Show("No project loaded");
                yield break;
            }

            if (PhysicsSimulatorManager.Instance.SimRunning)
            {
                ToastNotification.Show("Cannot save while simulation is running.");
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

            if (PhysicsSimulatorManager.Instance.SimRunning)
            {
                ToastNotification.Show("Cannot load while simulation is running.");
                yield break;
            }

            //std::string fP = FileDialogs::openFile("JSON (*.json)\0*.json\0");
            yield return FilesystemManager.Instance.OpenLoadDialogRoutine();

            if (!FileBrowser.Success) yield break;

            ClearObjects();

            //Get path
            string path = FileBrowser.Result[0];
            string fName, dir;

            ExtractPathAndName(path, out dir, out fName);
            //string fullPath = Path.Combine(dir, fName);
            var fileData = FilesystemManager.Instance.LoadFromFile(dir);

            SaveJson jsonData = default;
            bool loadSuccess = false;

            try
            {
                jsonData = JsonUtility.FromJson<SaveJson>(fileData);
                loadSuccess = true;
            }
            catch (Exception e)
            {
                ToastNotification.Show("Error while loading json file.");
                Debug.LogError($"JSON Load Exception: {e.Message}");
                loadSuccess = false;
            }

            if (loadSuccess)
            {
                _lastLoadedProject = jsonData;

                // validate
                ValidateJson(ref jsonData);

                //Deserialize project
                DeserializeProject(jsonData);

                //Setup project info
                projectInfo = new ProjectInfo() { name = fName, osPath = dir };

                saveMenuPanel.projectInputField.text = fName;
            }

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
        }

        /// <summary>
        /// Coroutine when play button is pressed
        /// </summary>
        /// <returns></returns>
        IEnumerator PlayButtonClickedRoutine()
        {
            //If no project loaded
            if (projectInfo == null)
            {
                ToastNotification.Show("No project loaded");
                yield break;
            }

            _lastLoadedProject = SerializeGameObjects();

            //List<GameObject> objectList = oManager.objectList.Select(obj => obj.gameObject).ToList();
            PhysicsSimulatorManager.Instance.RunSimulation(oManager.objectList);
        }

        /// <summary>
        /// Coroutine when pause button is pressed
        /// </summary>
        /// <returns></returns>
        IEnumerator PauseButtonClickedRoutine()
        {
            //If no project loaded
            if (projectInfo == null)
            {
                ToastNotification.Show("No project loaded");
                yield break;
            }

            //List<GameObject> objectList = oManager.objectList.Select(obj => obj.gameObject).ToList();
            PhysicsSimulatorManager.Instance.PauseSimulation(oManager.objectList);
        }

        /// <summary>
        /// Coroutine when reset button is pressed
        /// </summary>
        /// <returns></returns>
        IEnumerator ResetButtonClickedRoutine()
        {
            //If no project loaded
            if (projectInfo == null)
            {
                ToastNotification.Show("No project loaded");
                yield break;
            }

            //List<GameObject> objectList = oManager.objectList.Select(obj => obj.gameObject).ToList();
            PhysicsSimulatorManager.Instance.PauseSimulation(oManager.objectList);

            ClearObjects();

            if (_lastLoadedProject.HasValue)
            {
                DeserializeProject(_lastLoadedProject.Value);
            }
        }

        /// <summary>
        /// Clears all objects
        /// </summary>
        void ClearObjects()
        {
            oManager.ClearAllObjects();

            //Spawn object
            CoroutineExtensions.StartGlobalCoroutine(CoroutineExtensions.NextFrameRoutine(() =>
            {
                Debug.Log("Call next frame");

                oManager.TriggerUpdate();

            }));

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
                rotation = obj.transform.eulerAngles.z,
                color = obj.GetColor(),
                propertyJsons = GetPropertiesJson(obj.GetAllProperties())
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
                rotation = obj.transform.eulerAngles.z,
                color = obj.GetColor(),
                propertyJsons = GetPropertiesJson(obj.GetAllProperties())
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
                rotation = obj.transform.eulerAngles.z,
                color = obj.GetColor(),
                propertyJsons = GetPropertiesJson(obj.GetAllProperties())
            };
        }

        ObjectFixedJointJson ObjectFixedJointToJson(ObjectFixedJoint obj)
        {
            return new ObjectFixedJointJson()
            {
                name = obj.name,
                type = "FIXEDJOINT",
                objectAName = obj.objectA.name,
                objectBName = obj.objectB.name,
                propertyJsons = GetPropertiesJson(obj.GetAllProperties())
            };
        }

        ObjectSpringJointJson ObjectSpringJointToJson(ObjectSpringJoint obj)
        {
            return new ObjectSpringJointJson()
            {
                name = obj.name,
                type = "SPRINGJOINT",
                objectAName = obj.objectA.name,
                objectBName = obj.objectB.name,
                propertyJsons = GetPropertiesJson(obj.GetAllProperties())
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
            saveJson.gameObjectsTriangle = new List<ObjectTriJson>();
            saveJson.gameObjectsFixedJoint = new List<ObjectFixedJointJson>();
            saveJson.gameObjectsSpringJoint = new List<ObjectSpringJointJson>();

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
                    case ObjectType.FIXEDJOINT:
                        {
                            saveJson.gameObjectsFixedJoint.Add(ObjectFixedJointToJson((ObjectFixedJoint)item));
                        }
                        break;
                    case ObjectType.SPRINGJOINT:
                        {
                            saveJson.gameObjectsSpringJoint.Add(ObjectSpringJointToJson((ObjectSpringJoint)item));
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
                oManager.SpawnRectInternal(item.name, item.position, item.size, item.rotation, item.color, item.propertyJsons);
            }

            //Spawn Circles
            foreach (var item in jsonData.gameObjectsCircle)
            {
                oManager.SpawnCircleInternal(item.name, item.position, item.radius, item.rotation, item.color, item.propertyJsons);
            }

            //Spawn Triangles
            foreach (var item in jsonData.gameObjectsTriangle)
            {
                oManager.SpawnTriangleInternal(item.name, item.position, item.size, item.rotation, item.color, item.propertyJsons);
            }

            //Spawn Fixed joints
            foreach (var item in jsonData.gameObjectsFixedJoint)
            {
                oManager.SpawnFixedJointInternal(item.name, item.objectAName, item.objectBName, item.propertyJsons);
            }

            oManager.TriggerUpdate();
        }

        List<PropertyJson> GetPropertiesJson(List<ObjectBase.PropertyItem> props)
        {
            List<PropertyJson> outList = new();

            foreach (var item in props)
            {
                var json = new PropertyJson()
                { 
                    id = item.id,
                    type = item.proptype,
                };

                switch (item.proptype)
                {
                    case PropertyType.FLOAT:
                        json.value = item.getter().ToString();
                        break;
                    case PropertyType.STRING:
                        json.value = item.getter().ToString();
                        break;
                    case PropertyType.COLOR:
                        json.value = JsonUtility.ToJson(item.getter());
                        break;
                    case PropertyType.BOOL:
                        json.value = item.getter().ToString();
                        break;
                    default:
                        break;
                }

                outList.Add(json);
            }

            return outList;
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



        Color GetColorPickerProperty(DynamicPanels.Panel colorPickPanel)
        {
            return colorPickPanel.GetComponentInChildren<FlexibleColorPicker>().color;
        }

        void SetColorPickerProperty(DynamicPanels.Panel colorPickPanel, Color color)
        {
            colorPickPanel.GetComponentInChildren<FlexibleColorPicker>().color = color;
        }

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
            file = FileBrowserHelpers.GetFilename(path);
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

        /// <summary>
        /// Given a object tell if it is a joint or not(Note: change later) 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        bool IsJointType(ObjectBase obj)
        {
            return obj.type == ObjectType.FIXEDJOINT;
        }

        /// <summary>
        /// Inplace a validate json input,
        /// for now set default values if values are missing
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        void ValidateJson(ref SaveJson input)
        {
            // for older save files where this is empty
            if (input.gameObjectsFixedJoint == null)
            {
                input.gameObjectsFixedJoint = new();
            }

            //Spawn Rects
            foreach (var item in input.gameObjectsRect)
            {
                // handle the case where color outputs were not given
                if (item.color == Color.clear)
                {
                    item.color = Color.white;
                }
            }

            //Spawn Circles
            foreach (var item in input.gameObjectsCircle)
            {
                // handle the case where color outputs were not given
                if (item.color == Color.clear)
                {
                    item.color = Color.white;
                }
            }

            //Spawn Triangles
            foreach (var item in input.gameObjectsTriangle)
            {
                // handle the case where color outputs were not given
                if (item.color == Color.clear)
                {
                    item.color = Color.white;
                }
            }

        }

    }
}