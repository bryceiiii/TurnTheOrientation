using System.Collections.Generic;
using UnityEngine;

namespace TurnTheOrientation
{
    public sealed class FxBurst : MonoBehaviour
    {
        [SerializeField] private GameObject sparkPrefab;

        private sealed class Spark
        {
            public Transform Transform;
            public SpriteRenderer Renderer;
            public Vector2 Velocity;
            public float Life;
            public float MaxLife;
            public float Size;
        }

        private readonly List<Spark> _sparks = new List<Spark>();
        private static FxBurst s_instance;

        private void Awake()
        {
            s_instance = this;
        }

        private void OnDestroy()
        {
            if (s_instance == this)
            {
                s_instance = null;
            }
        }

        public static void Play(Vector2 position, Color color, int count, float spread, Transform parent)
        {
            if (s_instance == null || s_instance.sparkPrefab == null || parent == null)
            {
                return;
            }

            s_instance.Spawn(position, color, count, spread, parent);
        }

        private void Spawn(Vector2 position, Color color, int count, float spread, Transform parent)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject sparkObject = Instantiate(sparkPrefab, parent);
                sparkObject.name = $"Spark {i:00}";
                sparkObject.transform.position = position;
                float angle = Random.Range(0f, Mathf.PI * 2f);
                float speed = Random.Range(spread * 0.45f, spread);
                Vector2 velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
                float size = Random.Range(0.12f, 0.24f);
                sparkObject.transform.localScale = Vector3.one * size;
                SpriteRenderer renderer = sparkObject.GetComponent<SpriteRenderer>();
                renderer.color = color;
                _sparks.Add(new Spark
                {
                    Transform = sparkObject.transform,
                    Renderer = renderer,
                    Velocity = velocity,
                    Life = Random.Range(0.35f, 0.6f),
                    MaxLife = Random.Range(0.35f, 0.6f),
                    Size = size
                });
            }
        }

        private void Update()
        {
            for (int i = _sparks.Count - 1; i >= 0; i--)
            {
                Spark spark = _sparks[i];
                spark.Life -= Time.deltaTime;
                if (spark.Life <= 0f)
                {
                    Destroy(spark.Transform.gameObject);
                    _sparks.RemoveAt(i);
                    continue;
                }

                spark.Velocity += Vector2.down * (Time.deltaTime * 5f);
                spark.Transform.localPosition += (Vector3)(spark.Velocity * Time.deltaTime);
                spark.Transform.localScale = Vector3.one * spark.Size * (spark.Life / spark.MaxLife);
                Color color = spark.Renderer.color;
                color.a = Mathf.Clamp01(spark.Life / spark.MaxLife);
                spark.Renderer.color = color;
            }

        }
    }
}
