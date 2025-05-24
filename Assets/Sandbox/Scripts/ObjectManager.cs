using SandboxGame;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace SandboxGame
{
    /// <summary>
    /// Responsible for creating/spawning objects
    /// </summary>
    public class ObjectManager : Singleton<ObjectManager>
    {

        public bool IsDrawing;

        public TouchManager tManager;

        public ObjectLinker objectLinker;

        /// <summary>
        /// List of currently active objects
        /// </summary>
        public List<ObjectBase> objectList;

        //Private

        private Vector3 _dragStartPos;
        private Vector3 _dragEndPos;

        private List<IObjectObserver> observers = new();

        // Naming

        private Dictionary<string, int> nameCounts = new Dictionary<string, int>();

        private void OnEnable()
        {
            tManager.OnDragStarted += OnStartedDraging;
            tManager.OnDragEnded += OnEndDraging;
        }

        private void OnDisable()
        {
            tManager.OnDragStarted -= OnStartedDraging;
            tManager.OnDragEnded -= OnEndDraging;
        }

        protected override void Awake()
        {
            base.Awake();

        }

        // Start is called before the first frame update
        void Start()
        {

        }

        private void Update()
        {
            if (IsDrawing)
            {

            }
        }

        void OnStartDrawingRect()
        {
            _dragStartPos = Camera.main.ScreenToWorldPoint(tManager.startMousePositionScreen);
            _dragStartPos.z = 0;

            IsDrawing = true;
        }

        void OnEndDrawingRect()
        {
            _dragEndPos = Camera.main.ScreenToWorldPoint(tManager.mousePositionScreen);
            _dragEndPos.z = 0;

            IsDrawing = false;
            CoroutineExtensions.StartGlobalCoroutine(CoroutineExtensions.NextFrameRoutine(() =>
            {
                Debug.Log("Call next frame");
                //SpawnRect(_dragStartPos, _dragEndPos);

            }));

        }

        //----------------------
        //Spawning
        //----------------------

        public void SpawnRect(Vector3 startPos, Vector3 endPos,Color color)
        {
            var res = Resources.Load("ObjectBase", typeof(GameObject));

            GameObject gO = Instantiate(res) as GameObject;


            float _endXDistance = (endPos.x - startPos.x);
            float _endYDistance = (endPos.y - startPos.y) * -1; // * -1 since scales are swaped negative is up positive is down

            gO.transform.Find("body").transform.localScale = new Vector3(_endXDistance, _endYDistance, 0);
            gO.transform.position = startPos + ((endPos - startPos) / 2);

            gO.GetComponent<ObjectRect>().Init();
            gO.GetComponent<ObjectRect>().size = new Vector2(_endXDistance, _endYDistance);
            gO.GetComponent<ObjectRect>().SetColor(color);

            //naming
            gO.name = GetName("Rectangle");


            objectList.Add(gO.GetComponent<ObjectRect>());
            
            OnObjectSpawn(gO.GetComponent<ObjectRect>());

        }

        public void SpawnSquare(Vector3 startPos, Vector3 endPos, Color color)
        {
            var res = Resources.Load("ObjectBase", typeof(GameObject));

            GameObject gO = Instantiate(res) as GameObject;

            float _endXDistance = (endPos.x - startPos.x);
            float _endYDistance = (endPos.y - startPos.y);

            float sqSize = Mathf.Max(Mathf.Abs(_endXDistance), Mathf.Abs(_endYDistance));

            gO.transform.Find("body").transform.localScale = new Vector3(sqSize, sqSize, 0);
            gO.transform.position = startPos + new Vector3(Mathf.Sign(_endXDistance) * sqSize / 2, Mathf.Sign(_endYDistance) * sqSize / 2, 0);

            gO.GetComponent<ObjectRect>().Init();
            gO.GetComponent<ObjectRect>().size = new Vector2(sqSize, sqSize);
            gO.GetComponent<ObjectRect>().SetColor(color);

            //naming
            gO.name = GetName("Rectangle");

            objectList.Add(gO.GetComponent<ObjectBase>());
            OnObjectSpawn(gO.GetComponent<ObjectBase>());

        }

        public void SpawnCircle(Vector3 startPos, Vector3 endPos, Color color)
        {
            var res = Resources.Load("ObjectCircle", typeof(GameObject));

            GameObject gO = Instantiate(res) as GameObject;

            float radius = Vector3.Distance(endPos, startPos);
            gO.transform.Find("body").transform.localScale = new Vector3(radius * 2, radius * 2, 0);
            gO.transform.position = startPos;

            gO.GetComponent<ObjectCircle>().Init();
            gO.GetComponent<ObjectCircle>().radius = radius;
            gO.GetComponent<ObjectCircle>().SetColor(color);

            //naming
            gO.name = GetName("Circle");

            objectList.Add(gO.GetComponent<ObjectCircle>());
            OnObjectSpawn(gO.GetComponent<ObjectCircle>());

        }

        /// <summary>
        /// Spawn a triangle
        /// </summary>
        /// <param name="startPos">Will be the center of the triangle</param>
        /// <param name="endPos">One of its corners</param>
        public void SpawnTriangle(Vector3 startPos, Vector3 endPos, Color color)
        {
            var res = Resources.Load("ObjectTriangle", typeof(GameObject));

            GameObject gO = Instantiate(res) as GameObject;


            float _endXDistance = (endPos.x - startPos.x);
            float _endYDistance = (endPos.y - startPos.y) * -1; // * -1 since scales are swaped negative is up positive is down

            gO.transform.Find("body").transform.localScale = new Vector3(_endXDistance * 2, _endYDistance * 2, 0);
            gO.transform.position = startPos;

            gO.GetComponent<ObjectTriangle>().Init();
            gO.GetComponent<ObjectTriangle>().size = new Vector2(_endXDistance * 2, _endYDistance * 2);
            gO.GetComponent<ObjectTriangle>().SetColor(color);

            //naming
            gO.name = GetName("Triangle");

            objectList.Add(gO.GetComponent<ObjectTriangle>());
            
            OnObjectSpawn(gO.GetComponent<ObjectTriangle>());

        }

        /// <summary>
        /// Spawn a fixed joint between 2 objects
        /// </summary>
        /// <param name="obj1">ObjectA</param>
        /// <param name="obj2">ObjectB</param>
        /// <param name="pt1">World position of A pivot</param>
        /// <param name="pt2">World position of B pivot</param>
        public void SpawnFixedJoint(ObjectBase obj1, ObjectBase obj2, Vector3 pt1, Vector3 pt2)
        {
            var res = Resources.Load("ObjectFixedJoint", typeof(GameObject));

            GameObject gO = Instantiate(res) as GameObject;
            ObjectFixedJoint obj = gO.GetComponent<ObjectFixedJoint>();
            obj.Init(obj1, obj2);

            //naming
            gO.name = GetName("FixedJoint");

            objectList.Add(gO.GetComponent<ObjectFixedJoint>());
            
            OnObjectSpawn(obj.GetComponent<ObjectFixedJoint>());
        }

        /// <summary>
        /// Delete this object
        /// </summary>
        /// <param name="objectBase"></param>
        public void DeleteObject(ObjectBase objectBase)
        {
            Destroy(objectBase.gameObject);
            objectList.Remove(objectBase);

            OnObjectRemoved(objectBase);
        }

        public void ClearAllObjects()
        {
            foreach (var item in objectList)
            {
                Destroy(item.gameObject);
            }

            objectList.Clear();
        }

        //----------------------
        //Internal object spawn 
        //----------------------

        public void SpawnRectInternal(string name, Vector3 position, Vector2 size, float rotation,Color color)
        {
            var res = Resources.Load("ObjectBase", typeof(GameObject));

            GameObject gO = Instantiate(res) as GameObject;

            gO.transform.Find("body").transform.localScale = new Vector3(size.x, size.y, 0);
            gO.transform.position = position;
            gO.transform.eulerAngles = new Vector3(0, 0, rotation);

            gO.GetComponent<ObjectRect>().Init();
            gO.GetComponent<ObjectRect>().size = size;
            gO.GetComponent<ObjectRect>().SetColor(color);

            //naming
            gO.name = name;

            objectList.Add(gO.GetComponent<ObjectRect>());

        }

        public void SpawnCircleInternal(string name, Vector3 position,float radius,float rotation,Color color)
        {
            var res = Resources.Load("ObjectCircle", typeof(GameObject));

            GameObject gO = Instantiate(res) as GameObject;

            gO.transform.Find("body").transform.localScale = new Vector3(radius * 2, radius * 2, 0);
            gO.transform.position = position;
            gO.transform.eulerAngles = new Vector3(0, 0, rotation);

            gO.GetComponent<ObjectCircle>().Init();
            gO.GetComponent<ObjectCircle>().radius = radius;
            gO.GetComponent<ObjectCircle>().SetColor(color);

            //naming
            gO.name = name;

            objectList.Add(gO.GetComponent<ObjectCircle>());


        }


        public void SpawnTriangleInternal(string name, Vector3 position, Vector2 size, float rotation,Color color)
        {
            var res = Resources.Load("ObjectTriangle", typeof(GameObject));

            GameObject gO = Instantiate(res) as GameObject;

            gO.transform.Find("body").transform.localScale = new Vector3(size.x, size.y, 0);
            gO.transform.position = position;
            gO.transform.eulerAngles = new Vector3(0, 0, rotation);

            gO.GetComponent<ObjectTriangle>().Init();
            gO.GetComponent<ObjectTriangle>().size = size;
            gO.GetComponent<ObjectTriangle>().SetColor(color);


            //naming
            gO.name = name;

            objectList.Add(gO.GetComponent<ObjectTriangle>());

        }

        public void SpawnFixedJointInternal(string name, string objectNameA,string objectNameB)
        {
            var res = Resources.Load("ObjectBlank", typeof(GameObject));

            GameObject gO = Instantiate(res) as GameObject;
            ObjectFixedJoint obj = gO.AddComponent<ObjectFixedJoint>();
            obj.Init(GetPrimitiveObjectByName(objectNameA), GetPrimitiveObjectByName(objectNameB));

            //naming
            gO.name = name;

            objectList.Add(gO.GetComponent<ObjectFixedJoint>());

        }

        //----------------
        // Name generation
        //----------------

        private string GetName(string baseName)
        {
            if (!nameCounts.ContainsKey(baseName))
            {
                nameCounts[baseName] = 1;
                return baseName;
            }
            else
            {
                string newName = $"{baseName} {nameCounts[baseName]}";
                nameCounts[baseName]++;
                return newName;
            }
        }

        //----------------
        // Others
        //----------------

        ObjectBase GetPrimitiveObjectByName(string name)
        {
            for (int i = 0; i < objectList.Count; i++)
            {
                if (objectList[i].type != ObjectType.FIXEDJOINT && name == objectList[i].name)
                {
                    return objectList[i];
                }
            }

            return null;
        }

        public void AddObserver(IObjectObserver observer)
        {
            observers.Add(observer);
        }

        public void RemoveObserver(IObjectObserver observer)
        {
            observers.Remove(observer);
        }

        /// <summary>
        /// When a new object is spawned
        /// </summary>
        /// <param name="objectSpawned"></param>
        private void OnObjectSpawn(ObjectBase objectSpawned)
        {

            //objectList.Add(objectSpawned);
            foreach (var item in observers)
            {
                item.OnObjectAdded(objectSpawned);
            }
        }

        /// <summary>
        /// This object will be removed after this frame
        /// </summary>
        /// <param name="objectSpawned"></param>
        private void OnObjectRemoved(ObjectBase ToBeRemovedObject)
        {
            //objectList.Add(objectSpawned);
            foreach (var item in observers)
            {
                item.OnObjectRemoved(ToBeRemovedObject);
            }
        }

        /// <summary>
        /// Helper function to update its trigger event
        /// </summary>
        public void TriggerUpdate()
        {
            foreach (var item in observers)
            {
                item.OnStateUpdated();
            }
        }

        #region Drawing

        void OnStartedDraging()
        {
            _dragStartPos = Camera.main.ScreenToWorldPoint(tManager.startMousePositionScreen);
            _dragStartPos.z = 0;

            IsDrawing = true;

        }

        void OnEndDraging()
        {

            _dragEndPos = Camera.main.ScreenToWorldPoint(tManager.mousePositionScreen);
            _dragEndPos.z = 0;

            IsDrawing = false;

            var dType = tManager.prevDrawType;

            //Spawn object
            //CoroutineExtensions.StartGlobalCoroutine(CoroutineExtensions.NextFrameRoutine(() =>
            //{
            //    Debug.Log("Call next frame");
            //
            //    switch (dType)
            //    {
            //        case ShapeDrawType.NONE:
            //            break;
            //        case ShapeDrawType.SQUARE:
            //            SpawnSquare(_dragStartPos, _dragEndPos);
            //            break;
            //        case ShapeDrawType.RECT:
            //            SpawnRect(_dragStartPos, _dragEndPos);
            //            break;
            //        case ShapeDrawType.CIRCLE:
            //            SpawnCircle(_dragStartPos, _dragEndPos);
            //            break;
            //        default:
            //            break;
            //    }
            //
            //}));
        }


        #endregion
  

    }
}