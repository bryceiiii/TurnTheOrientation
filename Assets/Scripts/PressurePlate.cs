using System.Collections.Generic;
using UnityEngine;

namespace TurnTheOrientation
{
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class PressurePlate : MonoBehaviour
    {
        private readonly HashSet<Collider2D> _bodiesOnPlate = new HashSet<Collider2D>();
        private TurnGame _game;
        private SpriteRenderer _renderer;
        private Transform _visual;
        private bool _latched;

        public bool IsLatched => _latched;

        public void Setup(TurnGame game, Transform visual)
        {
            _game = game;
            _visual = visual;
            _renderer = visual.GetComponent<SpriteRenderer>();
            BoxCollider2D trigger = GetComponent<BoxCollider2D>();
            trigger.isTrigger = true;
            trigger.size = new Vector2(1.15f, 1.15f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponentInParent<PushableCrate>() != null)
            {
                _bodiesOnPlate.Add(other);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _bodiesOnPlate.Remove(other);
        }

        private void FixedUpdate()
        {
            if (_latched || _bodiesOnPlate.Count == 0)
            {
                return;
            }

            _latched = true;
            _game.NotifyPlateLatched(this);
            FxBurst.Play(transform.position, GamePalette.Plate, 10, 2.8f, _game.RoomRoot);
            _game.Audio.PlayPlate();
        }

        private void Update()
        {
            if (_visual == null)
            {
                return;
            }

            float targetScale = _latched ? 1.08f : 1f;
            float pulse = _latched ? 1f + Mathf.Sin(Time.unscaledTime * 5f) * 0.035f : 1f;
            _visual.localScale = Vector3.one * targetScale * pulse;
            if (_renderer != null)
            {
                _renderer.color = _latched
                    ? Color.Lerp(GamePalette.Plate, GamePalette.Success, 0.55f)
                    : Color.white;
            }
        }
    }
}
