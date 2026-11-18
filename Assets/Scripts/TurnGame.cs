using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TurnTheOrientation
{
    public sealed class TurnGame : MonoBehaviour
    {
        public enum GameState
        {
            Title,
            Playing,
            Paused,
            RoomCleared,
            AllCleared
        }

        private readonly List<PushableCrate> _crates = new List<PushableCrate>();
        private readonly List<PressurePlate> _plates = new List<PressurePlate>();
        private Camera _camera;
        private GameAudio _audio;
        private GameHud _hud;
        private LevelManager _levelManager;
        private LevelSceneController _levelScene;
        private TurnPlayer _player;
        private DoorGate _gate;
        private ExitPortal _exit;
        private Transform _roomRoot;
        private int _viewAngle;
        private int _latchedPlates;
        private int _sparks;
        private bool _rotating;
        private float _fallGuard;

        public GameState State { get; private set; } = GameState.Title;
        public int CurrentLevelIndex => _levelManager != null ? _levelManager.CurrentLevelIndex : 0;
        public int LevelCount => _levelManager != null ? _levelManager.LevelCount : 0;
        public int RoomRotation => _viewAngle;
        public int LatchedPlates => _latchedPlates;
        public int TotalPlates => _plates.Count;
        public int Sparks => _sparks;
        public PuzzleViewMode Mode => ViewMath.ModeForAngle(_viewAngle);
        public string ModeLabel => Mode == PuzzleViewMode.TopDown ? "TOP" : "SIDE";
        public string LevelTitle => _levelScene != null ? _levelScene.LevelTitle : string.Empty;
        public string LevelHint => _levelScene != null ? _levelScene.LevelHint : string.Empty;
        public Transform RoomRoot => _roomRoot;
        public GameAudio Audio => _audio;
        public TurnPlayer Player => _player;
        public bool HasLevel => _levelScene != null;
        public bool CanControlPlayer => State == GameState.Playing && !_rotating && HasLevel;
        public bool ExitIsArmed =>
            State == GameState.Playing &&
            Mode == PuzzleViewMode.SideView &&
            !_rotating &&
            _plates.Count > 0 &&
            _latchedPlates >= _plates.Count;

        public void Initialize(
            Camera gameCamera,
            GameAudio audio,
            GameHud hud,
            LevelManager levelManager)
        {
            _camera = gameCamera;
            _audio = audio;
            _hud = hud;
            _levelManager = levelManager;
            _hud.Initialize(this);
            SetState(GameState.Title);
        }

        public void BindLevel(LevelSceneController levelScene, bool startPlaying)
        {
            if (levelScene == null)
            {
                Debug.LogError("Cannot bind a null level scene.");
                SetState(GameState.Title);
                return;
            }

            _levelScene = levelScene;
            _roomRoot = levelScene.transform;
            _player = levelScene.Player;
            _gate = levelScene.Gate;
            _exit = levelScene.Exit;
            _crates.Clear();
            _crates.AddRange(levelScene.Crates);
            _plates.Clear();
            _plates.AddRange(levelScene.Plates);

            levelScene.Prepare(this);
            _viewAngle = 0;
            _latchedPlates = 0;
            _sparks = 3;
            _rotating = false;
            _fallGuard = 0f;
            _roomRoot.localRotation = Quaternion.identity;
            _camera.orthographicSize = 6.45f;
            _camera.transform.position = new Vector3(0f, 0f, -10f);
            SetBodiesFrozen(true);

            if (startPlaying)
            {
                SetState(GameState.Playing);
                SetBodiesFrozen(false);
                ApplyBodyModes();
            }
            else
            {
                SetState(GameState.Title);
            }
        }

        private void Update()
        {
            _fallGuard = Mathf.Max(0f, _fallGuard - Time.unscaledDeltaTime);
            HandleGlobalInput();
            if (_hud != null)
            {
                _hud.Refresh(this);
            }
        }

        public void StartGame()
        {
            if (State != GameState.Title || !HasLevel)
            {
                return;
            }

            _audio.PlayClick();
            SetState(GameState.Playing);
            SetBodiesFrozen(false);
            ApplyBodyModes();
        }

        public void RotateClockwise()
        {
            RequestRotation(-1);
        }

        public void RotateCounterClockwise()
        {
            RequestRotation(1);
        }

        public void RestartLevel()
        {
            if (State == GameState.Title || _levelManager == null)
            {
                return;
            }

            _audio.PlayClick();
            _levelManager.LoadLevel(CurrentLevelIndex, true);
        }

        public void RestartDemo()
        {
            if (_levelManager == null)
            {
                return;
            }

            _audio.PlayClick();
            _levelManager.LoadLevel(0, true);
        }

        public void AdvanceLevel()
        {
            if (State != GameState.RoomCleared)
            {
                return;
            }

            if (_levelManager == null || CurrentLevelIndex >= LevelCount - 1)
            {
                SetState(GameState.AllCleared);
                _audio.PlayWin();
                return;
            }

            _audio.PlayClick();
            _levelManager.LoadLevel(CurrentLevelIndex + 1, true);
        }

        public void CompleteLevel()
        {
            if (!ExitIsArmed)
            {
                return;
            }

            SetBodiesFrozen(true);
            SetState(GameState.RoomCleared);
            _audio.PlayWin();
            FxBurst.Play(_exit.transform.position, GamePalette.Success, 28, 5.4f, _roomRoot);
        }

        public void NotifyPlateLatched(PressurePlate plate)
        {
            _latchedPlates = 0;
            for (int i = 0; i < _plates.Count; i++)
            {
                if (_plates[i].IsLatched)
                {
                    _latchedPlates++;
                }
            }

            if (_latchedPlates >= _plates.Count && _gate != null)
            {
                _gate.Open();
                FxBurst.Play(_gate.transform.position, GamePalette.Violet, 16, 3.4f, _roomRoot);
            }
        }

        public void PlayerFell()
        {
            if (State != GameState.Playing || _rotating || _fallGuard > 0f || !HasLevel)
            {
                return;
            }

            _fallGuard = 0.7f;
            _sparks = Mathf.Max(0, _sparks - 1);
            _audio.PlayFall();
            if (_sparks <= 0)
            {
                RestartLevel();
                return;
            }

            _player.Respawn(_levelScene.SpawnPoint);
            _player.SetFrozen(false);
            ApplyBodyModes();
        }

        public void Pause()
        {
            if (State != GameState.Playing || _rotating)
            {
                return;
            }

            SetBodiesFrozen(true);
            SetState(GameState.Paused);
        }

        public void Resume()
        {
            if (State != GameState.Paused)
            {
                return;
            }

            SetState(GameState.Playing);
            SetBodiesFrozen(false);
            ApplyBodyModes();
        }

        private void HandleGlobalInput()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null || !HasLevel)
            {
                return;
            }

            if (State == GameState.Title)
            {
                if (keyboard.enterKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame)
                {
                    StartGame();
                }

                return;
            }

            if (State == GameState.AllCleared)
            {
                if (keyboard.enterKey.wasPressedThisFrame)
                {
                    RestartDemo();
                }

                return;
            }

            if (State == GameState.RoomCleared)
            {
                if (keyboard.enterKey.wasPressedThisFrame)
                {
                    AdvanceLevel();
                }

                if (keyboard.rKey.wasPressedThisFrame)
                {
                    RestartLevel();
                }

                return;
            }

            if (State == GameState.Paused)
            {
                if (keyboard.escapeKey.wasPressedThisFrame || keyboard.enterKey.wasPressedThisFrame)
                {
                    Resume();
                }

                if (keyboard.rKey.wasPressedThisFrame)
                {
                    RestartLevel();
                }

                return;
            }

            if (keyboard.escapeKey.wasPressedThisFrame)
            {
                Pause();
                return;
            }

            if (keyboard.rKey.wasPressedThisFrame)
            {
                RestartLevel();
                return;
            }

            if (keyboard.bKey.wasPressedThisFrame)
            {
                RotateClockwise();
            }
            else if (keyboard.nKey.wasPressedThisFrame)
            {
                RotateCounterClockwise();
            }
        }

        private void RequestRotation(int direction)
        {
            if (State != GameState.Playing || _rotating || !HasLevel)
            {
                return;
            }

            StartCoroutine(RotateRoutine(direction * 90));
        }

        private IEnumerator RotateRoutine(int delta)
        {
            _rotating = true;
            SetBodiesFrozen(true);
            _audio.PlayRotate();

            float startAngle = _viewAngle;
            float endAngle = _viewAngle + delta;
            float startSize = _camera.orthographicSize;
            int targetNormalized = ViewMath.NormalizeAngle(Mathf.RoundToInt(endAngle));
            PuzzleViewMode targetMode = ViewMath.ModeForAngle(targetNormalized);
            float targetSize = targetMode == PuzzleViewMode.TopDown ? 6.45f : 9.1f;
            const float duration = 0.34f;
            float elapsed = 0f;

            _viewAngle = targetNormalized;
            while (elapsed < duration)
            {
                float t = Mathf.Clamp01(elapsed / duration);
                float eased = t * t * (3f - 2f * t);
                float angle = Mathf.Lerp(startAngle, endAngle, eased);
                _roomRoot.localRotation = Quaternion.Euler(0f, 0f, angle);
                _camera.orthographicSize = Mathf.Lerp(startSize, targetSize, eased);
                float wobble = Mathf.Sin(t * Mathf.PI) * 0.08f;
                _camera.transform.position = new Vector3(
                    Mathf.Cos(Time.unscaledTime * 47f) * wobble,
                    Mathf.Sin(Time.unscaledTime * 53f) * wobble,
                    -10f);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            _roomRoot.localRotation = Quaternion.Euler(0f, 0f, endAngle);
            _camera.orthographicSize = targetSize;
            _camera.transform.position = new Vector3(0f, 0f, -10f);
            _rotating = false;
            ApplyBodyModes();
            SetBodiesFrozen(false);
        }

        private void SetBodiesFrozen(bool frozen)
        {
            if (_player != null)
            {
                _player.SetFrozen(frozen);
            }

            for (int i = 0; i < _crates.Count; i++)
            {
                if (_crates[i] != null)
                {
                    _crates[i].SetFrozen(frozen);
                }
            }
        }

        private void ApplyBodyModes()
        {
            bool side = Mode == PuzzleViewMode.SideView;
            if (_player != null && _player.Body != null)
            {
                _player.Body.gravityScale = side ? 1.55f : 0f;
                if (!side)
                {
                    _player.Body.linearVelocity = Vector2.zero;
                }
            }

            for (int i = 0; i < _crates.Count; i++)
            {
                Rigidbody2D body = _crates[i].Body;
                if (body != null)
                {
                    body.gravityScale = side ? 1.35f : 0f;
                    if (!side)
                    {
                        body.linearVelocity = Vector2.zero;
                    }
                }
            }
        }

        private void SetState(GameState state)
        {
            State = state;
            if (_hud != null)
            {
                _hud.Refresh(this);
            }
        }
    }
}
