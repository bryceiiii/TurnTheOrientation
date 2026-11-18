using UnityEngine;

namespace TurnTheOrientation
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public sealed class PushableCrate : MonoBehaviour
    {
        private Rigidbody2D _body;
        private Vector2 _spawnLocal;

        public Rigidbody2D Body => _body;

        public void Setup(Vector2 spawnLocal)
        {
            _spawnLocal = spawnLocal;
            _body = GetComponent<Rigidbody2D>();
            _body.freezeRotation = true;
            _body.interpolation = RigidbodyInterpolation2D.Interpolate;
            _body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        public void SetFrozen(bool frozen)
        {
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

        public void ResetToSpawn()
        {
            transform.localPosition = _spawnLocal;
            if (_body != null)
            {
                _body.position = transform.position;
                _body.linearVelocity = Vector2.zero;
                _body.angularVelocity = 0f;
            }
        }
    }
}
