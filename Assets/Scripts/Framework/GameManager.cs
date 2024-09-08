using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Redapple
{
    public class GameManager : Singleton<GameManager>
    {
        [Serializable]
        private class ManagerReorderableList : ReorderableArray<GameObject> { }

        // This is used to show to progress of the managers loading
        public static float InitialiseProgress { get; private set; }
        public static string SceneToLoadFromEditor;
        public static SceneName SceneTypeToLoadFromEditor;


        [Header("Game Settings")]
        [SerializeField] private int targetFrameRate = 30;
        [SerializeField] private float minimumLoadingScreenTime = 3;
        [SerializeField] private float minimumManagerLoadingTime = 5;
        [SerializeField] private float internetConnectionCheck = 15.0f;

        [Header("Managers")]
        [SerializeField] private SceneDataDictionaryObject sceneDataDictionary = null;
        [SerializeField][Reorderable] private ManagerReorderableList managerPrefabList = new ManagerReorderableList();
        private List<GameObject> managerInstaniated = new List<GameObject>();

        ///------------------------------------------------------------------------
        //                      	STATE MACHINE
        ///------------------------------------------------------------------------
        private State state_Splash;     // This is the first scene that is loaded before anything
        private State state_Init;       // The scene that init's all the managers
        private State state_Game;       // The main state that controls the updating of the current scene(SIMULATOR)

        // The main state machine that controls the 'game' flow
        private StateMachine stateMachine;

        // Coroutine that controls instaniating the managers and initing the system
        private Coroutine initManagersCoroutine = null;

        #region SceneLoadingVariables
        public SceneName currentGameType = SceneName.None;
        private Coroutine loadingSceneRoutine;
        public static bool SceneLoaded { get; private set; }
        public static bool FinishedLoadingManagers { get => Instance.initManagersCoroutine == null; }

        // The active scene that should be being updated
        //public EQScene ActiveScene { get => activeScene; set => activeScene = value; }
        //private EQScene activeScene = null;
        #endregion

        //Cooldown for checking if we are still connected
        private float internetCheckTimer = 0.0f;
        private static bool isInternetConnected = false;

        private bool firstLoginAttempt = false;

        /// <summary>
        /// Returns if the given scene exists in the game and can be loaded.
        /// </summary>
        public static bool SceneExists(string _sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; ++i)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                if (_sceneName == System.IO.Path.GetFileNameWithoutExtension(path))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Loads back to the main init scene and re-initialises the game. Used when a player re-opens the game
        /// after an extended amount of time and they're Playfab login session has expired. 
        /// </summary>
        public static void ForceRestartGame()
        {
            Debug.Log("Force Restarting the game here");
            //AudioController.StopAll();
            Destroy(Instance.gameObject);
            SceneManager.LoadScene("SCN_Init", LoadSceneMode.Single);
        }

        // This function starts everything
        protected override void Awake()
        {
            base.Awake();
            DontDestroy();
            ///------------------------------------------------------------------------
            //                      	STATES
            ///------------------------------------------------------------------------
            state_Splash = new State(State_Splash_OnEnter, State_Splash_OnUpdate, State_Splash_OnExit, "_Splash");
            state_Init = new State(State_Init_OnEnter, State_Init_OnUpdate, State_Init_OnExit, "_Init");
            state_Game = new State(State_Game_OnEnter, State_Game_OnUpdate, State_Game_OnExit, "_Game");

            stateMachine = new StateMachine(state_Init);

            Application.targetFrameRate = targetFrameRate;
            QualitySettings.vSyncCount = 0;
            DynamicGI.updateThreshold = float.MaxValue;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Time.timeScale = 1;
        }

        public void LoadSplash()
        {
            SceneManager.LoadScene(sceneDataDictionary.GetSceneString(SceneName.Init));
        }

        ///------------------------------------------------------------------------
        //                      	INIT
        ///------------------------------------------------------------------------
        private IEnumerator InitializeAllManagers()
        {
            ///------------------------------------------------------------------------
            //       INIT ALL THE MANAGERS ( WAIT FOR EACH OF THEM TO FINISH ) 
            ///------------------------------------------------------------------------
            float timeStartedLoadingScreen = Time.time;


            int managerCount = managerPrefabList.Count;
            GameObject managerObject;
            Transform managerParent = transform;

            for (int i = 0; i < managerCount; ++i)
            {
                GameObject managerPrefab = managerPrefabList[i];
                if (managerPrefab)
                {
                    managerObject = Instantiate(managerPrefab, managerParent);
                    managerObject.name = managerPrefab.name;

                    // Add To New List
                    managerInstaniated.Add(managerObject);

                    //Set Progress ( This is purely visual if we want to render this on the screen at some point
                    InitialiseProgress = (i + 1) / (float)(managerCount * 2);// Gives the results from 0 to 0.5
                    yield return null;
                }
            }

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            List<Coroutine> lstcoroutines = new List<Coroutine>();

            managerCount = managerInstaniated.Count;
            for (int i = 0; i < managerCount; ++i)
            {
                managerObject = managerInstaniated[i];
                IManager currentManager = managerObject.GetComponent<IManager>();
                if (currentManager != null)
                {
                    Debug.Log($"Init {currentManager.GetType().Name}");
                    stopwatch.Reset();
                    stopwatch.Start();


                    Coroutine coroutine = StartCoroutine(currentManager.Init());
                    if (coroutine != null)
                    {
                        lstcoroutines.Add(coroutine);
                    }


                    var loadingValue = (i + (managerCount * 0.5f)) / (float)(managerCount);
                    InitialiseProgress = loadingValue;
                    Debug.Log($"<color=green>InitialiseProgress:{InitialiseProgress}</color>");
                    stopwatch.Stop();
                    Debug.Log($"Init {currentManager.GetType().Name} complete ({stopwatch.ElapsedMilliseconds}ms)");
                }
                else
                    Debug.LogError($"Manager does not have IManager: {managerObject.name}");


            }
            for (int i = 0; i < lstcoroutines.Count; i++)
            {
                yield return lstcoroutines[i];
            }

            Debug.Log("Manager initialisation complete");

            initManagersCoroutine = null;

        }

        private void Update()
        {
            stateMachine?.Update();
        }

        ///------------------------------------------------------------------------
        //                      	STATES
        ///------------------------------------------------------------------------
        public void ChangeState(State stateToChangeTo)
        {
            stateMachine.ChangeState(stateToChangeTo);
        }

        private void State_Splash_OnEnter(StateMachine _StateMachine)
        {
            // Load Scene Here
            //SceneManager.LoadScene(sceneDataDictionary.GetSceneString(SceneName.Splash));
        }
        private void State_Splash_OnUpdate(StateMachine _StateMachine)
        {
            // Show legal and splash videos here
            stateMachine.ChangeState(state_Init);
        }
        private void State_Splash_OnExit(StateMachine _StateMachine)
        {

        }

        private void State_Init_OnEnter(StateMachine stateMachine)
        {
            if (initManagersCoroutine != null)
                StopCoroutine(initManagersCoroutine);

            initManagersCoroutine = StartCoroutine(InitializeAllManagers());
        }

        private void State_Init_OnUpdate(StateMachine stateMachine)
        {
            if (initManagersCoroutine == null)
            {
                //For now load scene from third index
                SceneManager.LoadScene(2);

                ChangeState(state_Game);
            }
        }

        private void State_Init_OnExit(StateMachine stateMachine)
        {
        }

        private void State_Loading_OnEnter(StateMachine stateMachine)
        {
            // Exit Last Scene

            SceneLoaded = false;
        }
        private void State_Loading_OnUpdate(StateMachine stateMachine)
        {
            if (SceneLoaded)
                stateMachine.ChangeState(state_Game);
        }
        private void State_Loading_OnExit(StateMachine stateMachine)
        {
        }

        private void State_Paused_OnEnter(StateMachine _StateMachine)
        {
        }
        private void State_Paused_OnUpdate(StateMachine _StateMachine)
        {
        }
        private void State_Paused_OnExit(StateMachine _StateMachine)
        {
        }

        private void State_Game_OnEnter(StateMachine _StateMachine)
        {
        }
        private void State_Game_OnUpdate(StateMachine _StateMachine)
        {
            //if (activeScene != null)
            //    activeScene.SceneUpdate();
        }
        private void State_Game_OnExit(StateMachine _StateMachine)
        {
            //activeScene?.OnSceneLeave();
        }

    }
}
