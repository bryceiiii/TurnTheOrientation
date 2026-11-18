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
        private TurnPlayer _player;
        private DoorGate _gate;
        private ExitPortal _exit;
        private LevelDefinition _level;
        private Transform _roomRoot;
        private int _viewAngle;
        private int _latchedPlates;
        private int _sparks;
        private bool _rotating;
        private float _fallGuard;

        public GameState State { get; private set; } = GameState.Title;
        public int CurrentLevelIndex { get; private set; }
        public int LevelCount => LevelCatalog.All.Count;
        public int RoomRotation => _viewAngle;
        public int LatchedPlates => _latchedPlates;
        public int TotalPlates => _plates.Count;
        public int Sparks => _sparks;
        public PuzzleViewMode Mode => ViewMath.ModeForAngle(_viewAngle);
        public string ModeLabel => Mode == PuzzleViewMode.TopDown ? "TOP" : "SIDE";
        public string LevelTitle => _level != null ? _level.Title : string.Empty;
        public string LevelHint => _level != null ? _level.Hint : string.Empty;
        public Transform RoomRoot => _roomRoot;
        public GameAudio Audio => _audio;
        public TurnPlayer Player => _player;
        public bool CanControlPlayer => State == GameState.Playing && !_rotating;
        public bool ExitIsArmed =>
            State == GameState.Playing &&
            Mode == PuzzleViewMode.SideView &&
            !_rotating &&
            _latchedPlates >= _plates.Count;

        public void Initialize(Camera gameCamera, GameAudio audio, GameHud hud)
        {
            _camera = gameCamera;
            _audio = audio;
            _hud = hud;
            _hud.Initialize(this);
            LoadLevel(0);
            SetState(GameState.Title);
        }

        private void Update()
        {
            _fallGuard = Mathf.Max(0f, _fallGuard - Time.unscaledDeltaTime);
            HandleGlobalInput();
            _hud.Refresh(this);
        }

        public void StartGame()
        {
            if (State != GameState.Title)
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
            if (State == GameState.Title)
            {
                return;
            }

            _audio.PlayClick();
            int index = CurrentLevelIndex;
            LoadLevel(index);
            SetState(GameState.Playing);
            SetBodiesFrozen(false);
            ApplyBodyModes();
        }

        public void RestartDemo()
        {
            _audio.PlayClick();
            CurrentLevelIndex = 0;
            LoadLevel(0);
            SetState(GameState.Playing);
            SetBodiesFrozen(false);
            ApplyBodyModes();
        }

        public void AdvanceLevel()
        {
            if (State != GameState.RoomCleared)
            {
                return;
            }

            if (CurrentLevelIndex >= LevelCount - 1)
            {
                SetState(GameState.AllCleared);
                _audio.PlayWin();
                return;
            }

            _audio.PlayClick();
            LoadLevel(CurrentLevelIndex + 1);
            SetState(GameState.Playing);
            SetBodiesFrozen(false);
            ApplyBodyModes();
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
            if (State != GameState.Playing || _rotating || _fallGuard > 0f)
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

            _player.Respawn(_level.Spawn);
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
            if (keyboard == null)
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
            if (State != GameState.Playing || _rotating)
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

        private void LoadLevel(int index)
        {
            CurrentLevelIndex = Mathf.Clamp(index, 0, LevelCount - 1);
            _level = LevelCatalog.All[CurrentLevelIndex];
            _viewAngle = 0;
            _latchedPlates = 0;
            _sparks = 3;
            _rotating = false;
            _crates.Clear();
            _plates.Clear();

            if (_roomRoot != null)
            {
                Destroy(_roomRoot.gameObject);
            }

            _roomRoot = new GameObject($"Room {CurrentLevelIndex + 1:00}").transform;
            _roomRoot.SetParent(transform, false);
            BuildFloor();
            BuildBoxes();
            BuildPlayer();
            BuildCrates();
            BuildPlates();
            BuildGate();
            BuildExit();
            BuildAmbientDetails();
            _camera.orthographicSize = 6.45f;
            _camera.transform.position = new Vector3(0f, 0f, -10f);
            SetBodiesFrozen(true);
        }

        private void BuildFloor()
        {
            var floorObject = new GameObject("Floor");
            floorObject.transform.SetParent(_roomRoot, false);
            SpriteRenderer renderer = floorObject.AddComponent<SpriteRenderer>();
            renderer.sprite = SpriteFactory.Floor();
            renderer.material = SpriteFactory.SpriteMaterial;
            renderer.drawMode = SpriteDrawMode.Tiled;
            renderer.size = new Vector2(16.1f, 10.2f);
            renderer.sortingOrder = -20;

            var underlay = new GameObject("Floor Underlay");
            underlay.transform.SetParent(_roomRoot, false);
            underlay.transform.position = new Vector3(0f, 0f, 1f);
            SpriteRenderer underlayRenderer = underlay.AddComponent<SpriteRenderer>();
            underlayRenderer.sprite = SpriteFactory.UiPanel(GamePalette.Alpha(GamePalette.Ink, 0.35f), 8);
            underlayRenderer.material = SpriteFactory.SpriteMaterial;
            underlayRenderer.drawMode = SpriteDrawMode.Sliced;
            underlayRenderer.size = new Vector2(17.4f, 11.4f);
            underlayRenderer.sortingOrder = -25;
        }

        private void BuildBoxes()
        {
            for (int i = 0; i < _level.Boxes.Count; i++)
            {
                BoxSpec spec = _level.Boxes[i];
                var boxObject = new GameObject($"Wall {i:00}");
                boxObject.transform.SetParent(_roomRoot, false);
                boxObject.transform.localPosition = spec.Position;
                SpriteRenderer renderer = boxObject.AddComponent<SpriteRenderer>();
                renderer.sprite = SpriteFactory.Block();
                renderer.material = SpriteFactory.SpriteMaterial;
                renderer.drawMode = SpriteDrawMode.Tiled;
                renderer.size = spec.Size;
                renderer.sortingOrder = 0;
                BoxCollider2D collider = boxObject.AddComponent<BoxCollider2D>();
                collider.size = spec.Size;
            }
        }

        private void BuildPlayer()
        {
            var playerObject = new GameObject("Player");
            playerObject.transform.SetParent(_roomRoot, false);
            playerObject.transform.localPosition = _level.Spawn;
            _player = playerObject.AddComponent<TurnPlayer>();
            Rigidbody2D body = playerObject.GetComponent<Rigidbody2D>();
            body.mass = 1.4f;
            body.linearDamping = 8f;
            CapsuleCollider2D collider = playerObject.GetComponent<CapsuleCollider2D>();
            collider.size = new Vector2(0.57f, 0.88f);

            var visualObject = new GameObject("Visual");
            visualObject.transform.SetParent(playerObject.transform, false);
            SpriteRenderer renderer = visualObject.AddComponent<SpriteRenderer>();
            renderer.sprite = SpriteFactory.Player();
            renderer.material = SpriteFactory.SpriteMaterial;
            renderer.sortingOrder = 30;
            visualObject.transform.localScale = Vector3.one * 0.92f;

            var glowObject = new GameObject("Glow");
            glowObject.transform.SetParent(playerObject.transform, false);
            glowObject.transform.localPosition = new Vector3(0f, 0f, 0.1f);
            SpriteRenderer glowRenderer = glowObject.AddComponent<SpriteRenderer>();
            glowRenderer.sprite = SpriteFactory.Glow();
            glowRenderer.material = SpriteFactory.SpriteMaterial;
            glowRenderer.color = GamePalette.Alpha(GamePalette.Player, 0.14f);
            glowRenderer.sortingOrder = 25;
            glowObject.transform.localScale = Vector3.one * 1.4f;

            _player.Setup(this, visualObject.transform, _level.Spawn);
        }

        private void BuildCrates()
        {
            for (int i = 0; i < _level.Crates.Count; i++)
            {
                CrateSpec spec = _level.Crates[i];
                var crateObject = new GameObject($"Core Crate {i + 1:00}");
                crateObject.transform.SetParent(_roomRoot, false);
                crateObject.transform.localPosition = spec.Position;
                PushableCrate crate = crateObject.AddComponent<PushableCrate>();
                Rigidbody2D body = crateObject.GetComponent<Rigidbody2D>();
                body.mass = 0.9f;
                body.linearDamping = 7f;
                BoxCollider2D collider = crateObject.GetComponent<BoxCollider2D>();
                collider.size = new Vector2(0.78f, 0.78f);

                var visualObject = new GameObject("Visual");
                visualObject.transform.SetParent(crateObject.transform, false);
                SpriteRenderer renderer = visualObject.AddComponent<SpriteRenderer>();
                renderer.sprite = SpriteFactory.Crate();
                renderer.material = SpriteFactory.SpriteMaterial;
                renderer.sortingOrder = 20;
                visualObject.transform.localScale = Vector3.one * 0.88f;

                crate.Setup(spec.Position);
                _crates.Add(crate);
            }
        }

        private void BuildPlates()
        {
            for (int i = 0; i < _level.Plates.Count; i++)
            {
                PlateSpec spec = _level.Plates[i];
                var plateObject = new GameObject($"Socket {i + 1:00}");
                plateObject.transform.SetParent(_roomRoot, false);
                plateObject.transform.localPosition = spec.Position;
                PressurePlate plate = plateObject.AddComponent<PressurePlate>();

                var visualObject = new GameObject("Visual");
                visualObject.transform.SetParent(plateObject.transform, false);
                SpriteRenderer renderer = visualObject.AddComponent<SpriteRenderer>();
                renderer.sprite = SpriteFactory.Plate();
                renderer.material = SpriteFactory.SpriteMaterial;
                renderer.sortingOrder = 5;
                visualObject.transform.localScale = Vector3.one * 1.12f;
                plate.Setup(this, visualObject.transform);
                _plates.Add(plate);
            }
        }

        private void BuildGate()
        {
            var gateObject = new GameObject("Signal Gate");
            gateObject.transform.SetParent(_roomRoot, false);
            gateObject.transform.localPosition = _level.Gate.Position;
            SpriteRenderer renderer = gateObject.AddComponent<SpriteRenderer>();
            renderer.sprite = SpriteFactory.Gate();
            renderer.material = SpriteFactory.SpriteMaterial;
            renderer.drawMode = SpriteDrawMode.Sliced;
            renderer.size = _level.Gate.Size;
            renderer.sortingOrder = 12;
            _gate = gateObject.AddComponent<DoorGate>();
            _gate.Setup(_level.Gate.Size);
        }

        private void BuildExit()
        {
            var exitObject = new GameObject("Exit Signal");
            exitObject.transform.SetParent(_roomRoot, false);
            exitObject.transform.localPosition = _level.Exit;
            _exit = exitObject.AddComponent<ExitPortal>();

            var glowObject = new GameObject("Exit Glow");
            glowObject.transform.SetParent(exitObject.transform, false);
            glowObject.transform.localPosition = new Vector3(0f, 0f, 0.2f);
            SpriteRenderer glowRenderer = glowObject.AddComponent<SpriteRenderer>();
            glowRenderer.sprite = SpriteFactory.Glow();
            glowRenderer.material = SpriteFactory.SpriteMaterial;
            glowRenderer.color = GamePalette.Alpha(GamePalette.Success, 0.3f);
            glowRenderer.sortingOrder = 2;
            glowObject.transform.localScale = Vector3.one * 2.3f;

            var visualObject = new GameObject("Visual");
            visualObject.transform.SetParent(exitObject.transform, false);
            SpriteRenderer renderer = visualObject.AddComponent<SpriteRenderer>();
            renderer.sprite = SpriteFactory.Exit();
            renderer.material = SpriteFactory.SpriteMaterial;
            renderer.sortingOrder = 18;
            visualObject.transform.localScale = Vector3.one * 1.18f;
            _exit.Setup(this, visualObject.transform);
        }

        private void BuildAmbientDetails()
        {
            Vector2[] dots =
            {
                new Vector2(-7.2f, 4.8f),
                new Vector2(7.1f, 4.9f),
                new Vector2(-7.5f, -0.4f),
                new Vector2(7.3f, -4.7f),
                new Vector2(0.8f, 4.9f),
                new Vector2(-0.2f, -5f)
            };

            for (int i = 0; i < dots.Length; i++)
            {
                var dotObject = new GameObject($"Signal Dot {i:00}");
                dotObject.transform.SetParent(_roomRoot, false);
                dotObject.transform.localPosition = dots[i];
                SpriteRenderer renderer = dotObject.AddComponent<SpriteRenderer>();
                renderer.sprite = SpriteFactory.Dot();
                renderer.material = SpriteFactory.SpriteMaterial;
                renderer.color = GamePalette.Alpha(i % 2 == 0 ? GamePalette.Player : GamePalette.Accent, 0.45f);
                renderer.sortingOrder = -2;
                dotObject.transform.localScale = Vector3.one * 0.08f;
            }
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
            _hud.Refresh(this);
        }
    }
}
