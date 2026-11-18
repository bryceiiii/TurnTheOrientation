using UnityEngine;
using UnityEngine.InputSystem;

namespace TurnTheOrientation
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
    public sealed class TurnPlayer : MonoBehaviour
    {
        private const float MoveSpeed = 5.2f;
        private const float GroundAcceleration = 55f;
        private const float JumpSpeed = 9.6f;
        private const float GroundCheckDistance = 0.1f;

        private TurnGame _game;
        private Rigidbody2D _body;
        private CapsuleCollider2D _collider;
        private Transform _visual;
        private Vector2 _moveInput;
        private bool _jumpQueued;
        private bool _frozen;
        private bool _wasGrounded;
        private float _visualBaseScale = 1f;
        private Vector2 _spawnLocal;

        public Rigidbody2D Body => _body;
        public bool IsFrozen => _frozen;
        public Vector2 MoveInput => _moveInput;

        public void Setup(TurnGame game, Transform visual, Vector2 spawnLocal)
        {
            _game = game;
            _visual = visual;
            _visualBaseScale = visual != null ? visual.localScale.x : 1f;
            _body = GetComponent<Rigidbody2D>();
            _collider = GetComponent<CapsuleCollider2D>();
            _spawnLocal = spawnLocal;
            _body.freezeRotation = true;
            _body.interpolation = RigidbodyInterpolation2D.Interpolate;
            _body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        public void SetFrozen(bool frozen)
        {
            _frozen = frozen;
            if (_body == null)
            {
                return;
            }

            if (frozen)
            {
                _body.linearVelocity = Vector2.zero;
                _body.angularVelocity = 0f;
            }

            _body.simulated = !frozen;
        }

        public void Respawn(Vector2 localPosition)
        {
            _spawnLocal = localPosition;
            transform.localPosition = localPosition;
            if (_body != null)
            {
                _body.position = transform.position;
                _body.linearVelocity = Vector2.zero;
                _body.angularVelocity = 0f;
            }
        }

        private void Update()
        {
            if (_game == null || _frozen || !_game.CanControlPlayer)
            {
                _moveInput = Vector2.zero;
                return;
            }

            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            float x = 0f;
            float y = 0f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                x -= 1f;
            }

            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            {
                x += 1f;
            }

            if (_game.Mode == PuzzleViewMode.TopDown)
            {
                if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                {
                    y += 1f;
                }

                if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
                {
                    y -= 1f;
                }
            }

            _moveInput = new Vector2(x, y).normalized;

            if (_game.Mode == PuzzleViewMode.SideView &&
                (keyboard.spaceKey.wasPressedThisFrame ||
                 keyboard.wKey.wasPressedThisFrame ||
                 keyboard.upArrowKey.wasPressedThisFrame))
            {
                _jumpQueued = true;
            }

            UpdateVisual();

            Vector3 world = transform.position;
            if (world.y < -13f || world.y > 14f || world.x < -15f || world.x > 15f)
            {
                _game.PlayerFell();
            }
        }

        private void FixedUpdate()
        {
            if (_game == null || _body == null || _frozen || !_game.CanControlPlayer)
            {
                return;
            }

            if (_game.Mode == PuzzleViewMode.TopDown)
            {
                _body.gravityScale = 0f;
                _body.linearVelocity = _moveInput * MoveSpeed;
                _jumpQueued = false;
                _wasGrounded = false;
                return;
            }

            _body.gravityScale = 1.55f;
            bool grounded = CheckGrounded();
            Vector2 velocity = _body.linearVelocity;
            float targetHorizontal = _moveInput.x * MoveSpeed;
            velocity.x = Mathf.MoveTowards(velocity.x, targetHorizontal, GroundAcceleration * Time.fixedDeltaTime);

            if (_jumpQueued && grounded)
            {
                velocity.y = JumpSpeed;
                _game.Audio.PlayJump();
                FxBurst.Play(transform.position + Vector3.down * 0.35f, GamePalette.PlayerSoft, 5, 2.1f, _game.RoomRoot);
            }

            _jumpQueued = false;
            _body.linearVelocity = velocity;
            _wasGrounded = grounded;
        }

        private bool CheckGrounded()
        {
            Bounds bounds = _collider.bounds;
            Vector2 origin = new Vector2(bounds.center.x, bounds.min.y + 0.03f);
            Vector2 size = new Vector2(bounds.size.x * 0.78f, 0.04f);
            RaycastHit2D[] hits = Physics2D.BoxCastAll(
                origin,
                size,
                0f,
                Vector2.down,
                GroundCheckDistance);

            for (int i = 0; i < hits.Length; i++)
            {
                Collider2D hit = hits[i].collider;
                if (hit == null || hit.isTrigger || hit == _collider)
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        private void UpdateVisual()
        {
            if (_visual == null || _game == null)
            {
                return;
            }

            float counterAngle = -_game.RoomRotation;
            _visual.localRotation = Quaternion.Euler(0f, 0f, counterAngle);

            float squash = 1f;
            if (_game.Mode == PuzzleViewMode.SideView && !_wasGrounded && _body != null)
            {
                squash = Mathf.Lerp(1.06f, 0.94f, Mathf.InverseLerp(9f, -9f, _body.linearVelocity.y));
            }
            else
            {
                float breathe = 1f + Mathf.Sin(Time.unscaledTime * 3.2f) * 0.025f;
                squash = breathe;
            }

            _visual.localScale = new Vector3(
                _visualBaseScale * (2f - squash),
                _visualBaseScale * squash,
                _visualBaseScale);
        }
    }
}
