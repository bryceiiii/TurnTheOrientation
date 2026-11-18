using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        private static LevelManager s_instance;
        private Camera _gameCamera;
        private GameAudio _audio;
        private GameHud _hud;
        private TurnGame _game;
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
            EnsurePersistentCamera();
            EnsureGameSystems();
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

            _game.BindLevel(levelScene, startPlaying);
            _isLoading = false;
            _loadRoutine = null;
        }

        private void EnsurePersistentCamera()
        {
            _gameCamera = Camera.main;
            if (_gameCamera == null)
            {
                var cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                _gameCamera = cameraObject.AddComponent<Camera>();
                cameraObject.AddComponent<AudioListener>();
            }
            else
            {
                DontDestroyOnLoad(_gameCamera.gameObject);
            }

            _gameCamera.orthographic = true;
            _gameCamera.orthographicSize = 6.45f;
            _gameCamera.transform.position = new Vector3(0f, 0f, -10f);
            _gameCamera.clearFlags = CameraClearFlags.SolidColor;
            _gameCamera.backgroundColor = GamePalette.Ink;
            _gameCamera.nearClipPlane = 0.1f;
            _gameCamera.farClipPlane = 50f;
        }

        private void EnsureGameSystems()
        {
            _audio = GetComponentInChildren<GameAudio>(true);
            if (_audio == null)
            {
                var audioObject = new GameObject("Game Audio");
                audioObject.transform.SetParent(transform, false);
                _audio = audioObject.AddComponent<GameAudio>();
            }
            _audio.Initialize();

            _hud = GetComponentInChildren<GameHud>(true);
            if (_hud == null)
            {
                var hudObject = new GameObject("Game HUD");
                hudObject.transform.SetParent(transform, false);
                _hud = hudObject.AddComponent<GameHud>();
            }

            _game = GetComponentInChildren<TurnGame>(true);
            if (_game == null)
            {
                var gameObject = new GameObject("TurnGame");
                gameObject.transform.SetParent(transform, false);
                _game = gameObject.AddComponent<TurnGame>();
            }
            _game.Initialize(_gameCamera, _audio, _hud, this);
        }
    }
}
