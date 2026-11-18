using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace TurnTheOrientation
{
    [DefaultExecutionOrder(-1000)]
    public sealed class LevelManager : MonoBehaviour
    {
        [SerializeField] private string[] levelScenes =
        {
            "Level_01",
            "Level_02",
            "Level_03"
        };
        [SerializeField] private Camera gameCamera;
        [FormerlySerializedAs("audio")]
        [SerializeField] private GameAudio gameAudio;
        [SerializeField] private GameHud hud;
        [SerializeField] private TurnGame game;

        private static LevelManager s_instance;
        private Coroutine _loadRoutine;
        private bool _isLoading;

        public static LevelManager Instance => s_instance;
        public int CurrentLevelIndex { get; private set; }
        public int LevelCount => levelScenes != null ? levelScenes.Length : 0;
        public string CurrentLevelSceneName =>
            levelScenes != null && levelScenes.Length > 0
                ? levelScenes[Mathf.Clamp(CurrentLevelIndex, 0, levelScenes.Length - 1)]
                : string.Empty;

        private void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_instance = this;
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = 60;
            ConfigurePersistentCamera();
            InitializeGameSystems();
        }

        private void Start()
        {
            LoadLevel(0, false);
        }

        private void OnDestroy()
        {
            if (s_instance == this)
            {
                s_instance = null;
            }
        }

        public void LoadLevel(int index, bool startPlaying)
        {
            if (_isLoading || levelScenes == null || levelScenes.Length == 0)
            {
                return;
            }

            int clampedIndex = Mathf.Clamp(index, 0, levelScenes.Length - 1);
            _loadRoutine = StartCoroutine(LoadLevelRoutine(clampedIndex, startPlaying));
        }

        public void ReloadCurrentLevel(bool startPlaying)
        {
            LoadLevel(CurrentLevelIndex, startPlaying);
        }

        private IEnumerator LoadLevelRoutine(int index, bool startPlaying)
        {
            _isLoading = true;
            int previousIndex = CurrentLevelIndex;
            CurrentLevelIndex = index;
            string sceneName = levelScenes[index];

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            if (operation == null)
            {
                CurrentLevelIndex = previousIndex;
                _isLoading = false;
                Debug.LogError($"Could not start loading level scene '{sceneName}'.");
                yield break;
            }

            while (!operation.isDone)
            {
                yield return null;
            }

            LevelSceneController levelScene = FindFirstObjectByType<LevelSceneController>();
            if (levelScene == null)
            {
                Debug.LogError($"Level scene '{sceneName}' has no LevelSceneController.");
                _isLoading = false;
                yield break;
            }

            game.BindLevel(levelScene, startPlaying);
            _isLoading = false;
            _loadRoutine = null;
        }

        private void ConfigurePersistentCamera()
        {
            if (gameCamera == null)
            {
                Debug.LogError("LevelManager requires a Camera reference.", this);
                return;
            }

            DontDestroyOnLoad(gameCamera.gameObject);
            gameCamera.orthographic = true;
            gameCamera.orthographicSize = 6.45f;
            gameCamera.transform.position = new Vector3(0f, 0f, -10f);
            gameCamera.clearFlags = CameraClearFlags.SolidColor;
            gameCamera.backgroundColor = GamePalette.Ink;
            gameCamera.nearClipPlane = 0.1f;
            gameCamera.farClipPlane = 50f;
        }

        private void InitializeGameSystems()
        {
            if (gameAudio == null || hud == null || game == null)
            {
                Debug.LogError(
                    "LevelManager requires Audio, HUD, and TurnGame prefab references.",
                    this);
                return;
            }

            gameAudio.Initialize();
            game.Initialize(gameCamera, gameAudio, hud, this);
        }
    }
}
