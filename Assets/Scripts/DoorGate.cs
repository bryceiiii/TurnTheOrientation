using System.Collections;
using UnityEngine;

namespace TurnTheOrientation
{
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class DoorGate : MonoBehaviour
    {
        private BoxCollider2D _collider;
        private SpriteRenderer _renderer;
        private bool _open;

        public void Setup(Vector2 size)
        {
            _collider = GetComponent<BoxCollider2D>();
            _collider.size = size;
            _renderer = GetComponent<SpriteRenderer>();
            _renderer.drawMode = SpriteDrawMode.Sliced;
            _renderer.size = size;
        }

        public void Open()
        {
            if (_open)
            {
                return;
            }

            _open = true;
            _collider.enabled = false;
            StartCoroutine(OpenRoutine());
        }

        private IEnumerator OpenRoutine()
        {
            Vector3 startScale = transform.localScale;
            float elapsed = 0f;
            const float duration = 0.24f;
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                transform.localScale = new Vector3(startScale.x * (1f - t), startScale.y, startScale.z);
                Color color = _renderer.color;
                color.a = 1f - t;
                _renderer.color = color;
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            gameObject.SetActive(false);
        }
    }
}
