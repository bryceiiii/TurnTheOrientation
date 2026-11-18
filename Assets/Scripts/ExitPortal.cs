using UnityEngine;

namespace TurnTheOrientation
{
    [RequireComponent(typeof(CircleCollider2D))]
    public sealed class ExitPortal : MonoBehaviour
    {
        private TurnGame _game;
        private Transform _visual;
        private SpriteRenderer _renderer;
        private bool _triggered;

        public void Setup(TurnGame game, Transform visual)
        {
            _game = game;
            _visual = visual;
            _renderer = visual.GetComponent<SpriteRenderer>();
            CircleCollider2D trigger = GetComponent<CircleCollider2D>();
            trigger.isTrigger = true;
            trigger.radius = 0.48f;
        }

        private void Update()
        {
            if (_visual == null)
            {
                return;
            }

            _visual.localRotation = Quaternion.Euler(0f, 0f, Time.unscaledTime * 35f);
            float pulse = 1f + Mathf.Sin(Time.unscaledTime * 4f) * 0.06f;
            _visual.localScale = Vector3.one * pulse;
            if (_renderer != null && _game != null)
            {
                _renderer.color = _game.ExitIsArmed
                    ? Color.white
                    : Color.Lerp(Color.white, GamePalette.Accent, 0.62f);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryComplete(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TryComplete(other);
        }

        private void TryComplete(Collider2D other)
        {
            if (_triggered ||
                _game == null ||
                !_game.ExitIsArmed ||
                other.GetComponentInParent<TurnPlayer>() == null)
            {
                return;
            }

            _triggered = true;
            _game.CompleteLevel();
        }
    }
}
