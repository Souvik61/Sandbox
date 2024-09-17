using SandboxGame;
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

        //Private

        private Vector3 _dragStartPos;
        private Vector3 _dragEndPos;

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
                SpawnRect(_dragStartPos, _dragEndPos);

            }));

        }

        //----------------------
        //Spawning
        //----------------------


        public void SpawnRect(Vector3 startPos, Vector3 endPos)
        {
            var res = Resources.Load("ObjectBase", typeof(GameObject));

            GameObject gO = Instantiate(res) as GameObject;


            float _endXDistance = (endPos.x - startPos.x);
            float _endYDistance = (endPos.y - startPos.y) * -1; // * -1 since scales are swaped negative is up positive is down

            gO.transform.Find("body").transform.localScale = new Vector3(_endXDistance, _endYDistance, 0);
            gO.transform.position = startPos + ((endPos - startPos) / 2);

        }

        public void SpawnSquare(Vector3 startPos, Vector3 endPos)
        {
            var res = Resources.Load("ObjectBase", typeof(GameObject));

            GameObject gO = Instantiate(res) as GameObject;

            float _endXDistance = (endPos.x - startPos.x);
            float _endYDistance = (endPos.y - startPos.y);

            float sqSize = Mathf.Max(Mathf.Abs(_endXDistance), Mathf.Abs(_endYDistance));

            gO.transform.Find("body").transform.localScale = new Vector3(sqSize, sqSize, 0);
            gO.transform.position = startPos + new Vector3(Mathf.Sign(_endXDistance) * sqSize / 2, Mathf.Sign(_endYDistance) * sqSize / 2, 0);

        }

        public void SpawnCircle(Vector3 startPos, Vector3 endPos)
        {
            var res = Resources.Load("ObjectCircle", typeof(GameObject));

            GameObject gO = Instantiate(res) as GameObject;

            float radius = Vector3.Distance(endPos, startPos);
            gO.transform.Find("body").transform.localScale = new Vector3(radius * 2, radius * 2, 0);
            gO.transform.position = startPos;

        }

        /// <summary>
        /// Spawn a triangle
        /// </summary>
        /// <param name="startPos">Will be the center of the triangle</param>
        /// <param name="endPos">One of its corners</param>
        public void SpawnTriangle(Vector3 startPos, Vector3 endPos)
        {
            var res = Resources.Load("ObjectTriangle", typeof(GameObject));

            GameObject gO = Instantiate(res) as GameObject;


            float _endXDistance = (endPos.x - startPos.x);
            float _endYDistance = (endPos.y - startPos.y) * -1; // * -1 since scales are swaped negative is up positive is down

            gO.transform.Find("body").transform.localScale = new Vector3(_endXDistance * 2, _endYDistance * 2, 0);
            gO.transform.position = startPos;

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