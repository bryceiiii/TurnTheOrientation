using System.Collections.Generic;
using UnityEngine;

namespace TurnTheOrientation
{
    public static class SpriteFactory
    {
        private const float WorldPixelsPerUnit = 64f;
        private static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();
        private static Material s_spriteMaterial;

        public static Material SpriteMaterial
        {
            get
            {
                if (s_spriteMaterial != null)
                {
                    return s_spriteMaterial;
                }

                Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
                if (shader == null)
                {
                    shader = Shader.Find("Sprites/Default");
                }

                s_spriteMaterial = new Material(shader)
                {
                    name = "Runtime Sprite Unlit",
                    color = Color.white
                };
                return s_spriteMaterial;
            }
        }

        public static Sprite Block()
        {
            return Cached("block", () =>
            {
                Texture2D tex = NewTexture(64, 64);
                FillRect(tex, 0, 0, 64, 64, GamePalette.Wall);
                FillRect(tex, 2, 2, 60, 60, GamePalette.Wall);
                FillRect(tex, 4, 4, 56, 56, Color.Lerp(GamePalette.Wall, GamePalette.Floor, 0.18f));
                FillRect(tex, 5, 56, 54, 4, GamePalette.WallEdge);
                FillRect(tex, 5, 4, 4, 52, GamePalette.Alpha(GamePalette.WallEdge, 0.38f));
                FillRect(tex, 25, 25, 14, 14, GamePalette.Alpha(GamePalette.Ink, 0.2f));
                FillCircle(tex, 11, 11, 3, GamePalette.Alpha(GamePalette.WallEdge, 0.55f));
                FillCircle(tex, 53, 11, 3, GamePalette.Alpha(GamePalette.Ink, 0.35f));
                tex.Apply();
                return BuildSprite(tex, Vector4.zero);
            });
        }

        public static Sprite Floor()
        {
            return Cached("floor", () =>
            {
                Texture2D tex = NewTexture(64, 64);
                FillRect(tex, 0, 0, 64, 64, GamePalette.Floor);
                FillRect(tex, 0, 0, 64, 2, GamePalette.FloorLine);
                FillRect(tex, 0, 0, 2, 64, GamePalette.FloorLine);
                FillRect(tex, 30, 14, 4, 4, GamePalette.Alpha(GamePalette.WallEdge, 0.18f));
                FillRect(tex, 14, 48, 4, 4, GamePalette.Alpha(GamePalette.Plate, 0.12f));
                tex.Apply();
                return BuildSprite(tex, Vector4.zero);
            });
        }

        public static Sprite Player()
        {
            return Cached("player", () =>
            {
                Texture2D tex = NewTexture(64, 64);
                FillEllipse(tex, 32, 19, 22, 18, GamePalette.Shadow);
                FillEllipse(tex, 32, 28, 21, 23, GamePalette.Shadow);
                FillEllipse(tex, 32, 30, 17, 19, GamePalette.Player);
                FillEllipse(tex, 32, 25, 12, 13, GamePalette.PlayerSoft);
                FillRect(tex, 23, 31, 6, 7, GamePalette.Ink);
                FillRect(tex, 35, 31, 6, 7, GamePalette.Ink);
                FillRect(tex, 25, 33, 2, 3, GamePalette.Text);
                FillRect(tex, 37, 33, 2, 3, GamePalette.Text);
                FillRect(tex, 30, 48, 4, 9, GamePalette.Shadow);
                FillCircle(tex, 32, 56, 6, GamePalette.Shadow);
                FillCircle(tex, 32, 56, 4, GamePalette.Plate);
                FillCircle(tex, 32, 56, 2, GamePalette.PlayerSoft);
                FillRect(tex, 18, 20, 28, 5, GamePalette.Accent);
                FillRect(tex, 17, 20, 6, 9, GamePalette.Accent);
                FillRect(tex, 23, 8, 7, 5, GamePalette.Shadow);
                FillRect(tex, 34, 8, 7, 5, GamePalette.Shadow);
                tex.Apply();
                return BuildSprite(tex, Vector4.zero);
            });
        }

        public static Sprite Crate()
        {
            return Cached("crate", () =>
            {
                Texture2D tex = NewTexture(64, 64);
                FillRect(tex, 5, 5, 54, 54, GamePalette.Shadow);
                FillRect(tex, 7, 7, 50, 50, GamePalette.CrateDark);
                FillRect(tex, 11, 11, 42, 42, GamePalette.Crate);
                for (int i = 0; i < 7; i++)
                {
                    FillRect(tex, 14 + i * 2, 14 + i * 2, 5, 22, GamePalette.Alpha(GamePalette.Text, 0.18f));
                }

                FillRect(tex, 14, 14, 36, 4, GamePalette.Alpha(GamePalette.Ink, 0.3f));
                FillRect(tex, 14, 46, 36, 4, GamePalette.Alpha(GamePalette.Text, 0.35f));
                FillCircle(tex, 18, 18, 3, GamePalette.PlayerSoft);
                FillCircle(tex, 46, 18, 3, GamePalette.PlayerSoft);
                FillCircle(tex, 18, 46, 3, GamePalette.PlayerSoft);
                FillCircle(tex, 46, 46, 3, GamePalette.PlayerSoft);
                tex.Apply();
                return BuildSprite(tex, Vector4.zero);
            });
        }

        public static Sprite Plate()
        {
            return Cached("plate", () =>
            {
                Texture2D tex = NewTexture(64, 64);
                FillCircle(tex, 32, 32, 25, GamePalette.Alpha(GamePalette.Shadow, 0.7f));
                FillCircle(tex, 32, 32, 22, GamePalette.CrateDark);
                FillCircle(tex, 32, 32, 17, GamePalette.Ink);
                FillCircle(tex, 32, 32, 12, GamePalette.Alpha(GamePalette.Plate, 0.55f));
                FillCircle(tex, 32, 32, 6, GamePalette.Plate);
                for (int i = 0; i < 4; i++)
                {
                    float angle = i * 90f * Mathf.Deg2Rad;
                    int x = 32 + Mathf.RoundToInt(Mathf.Cos(angle) * 19f);
                    int y = 32 + Mathf.RoundToInt(Mathf.Sin(angle) * 19f);
                    FillRect(tex, x - 2, y - 2, 4, 4, GamePalette.PlayerSoft);
                }
                tex.Apply();
                return BuildSprite(tex, Vector4.zero);
            });
        }

        public static Sprite Gate()
        {
            return Cached("gate", () =>
            {
                Texture2D tex = NewTexture(64, 64);
                FillRect(tex, 4, 0, 56, 64, GamePalette.Shadow);
                FillRect(tex, 8, 0, 48, 64, GamePalette.Violet);
                for (int y = 3; y < 64; y += 10)
                {
                    FillRect(tex, 10, y, 44, 3, GamePalette.Alpha(GamePalette.PlayerSoft, 0.65f));
                }
                FillRect(tex, 28, 0, 8, 64, GamePalette.Alpha(GamePalette.Ink, 0.35f));
                tex.Apply();
                return BuildSprite(tex, new Vector4(10f, 10f, 10f, 10f));
            });
        }

        public static Sprite Exit()
        {
            return Cached("exit", () =>
            {
                Texture2D tex = NewTexture(64, 64);
                FillCircle(tex, 32, 32, 28, GamePalette.Alpha(GamePalette.Shadow, 0.5f));
                FillCircle(tex, 32, 32, 24, GamePalette.Alpha(GamePalette.Success, 0.3f));
                FillCircle(tex, 32, 32, 18, GamePalette.Ink);
                FillCircle(tex, 32, 32, 13, GamePalette.Success);
                FillCircle(tex, 32, 32, 7, GamePalette.Text);
                FillRect(tex, 30, 19, 4, 26, GamePalette.Success);
                tex.Apply();
                return BuildSprite(tex, Vector4.zero);
            });
        }

        public static Sprite UiPanel(Color color, int cornerRadius = 12)
        {
            string key = $"ui-{ColorUtility.ToHtmlStringRGBA(color)}-{cornerRadius}";
            return Cached(key, () =>
            {
                Texture2D tex = NewTexture(64, 64);
                FillRoundedRect(tex, 1, 1, 62, 62, cornerRadius, color);
                FillRect(tex, cornerRadius, 58, 64 - cornerRadius * 2, 2, GamePalette.Alpha(GamePalette.Text, 0.08f));
                tex.Apply();
                return BuildSprite(tex, new Vector4(cornerRadius, cornerRadius, cornerRadius, cornerRadius));
            });
        }

        public static Sprite Dot()
        {
            return Cached("dot", () =>
            {
                Texture2D tex = NewTexture(16, 16);
                FillCircle(tex, 8, 8, 7, Color.white);
                tex.Apply();
                return BuildSprite(tex, Vector4.zero);
            });
        }

        public static Sprite Glow()
        {
            return Cached("glow", () =>
            {
                Texture2D tex = NewTexture(64, 64);
                for (int y = 0; y < 64; y++)
                {
                    for (int x = 0; x < 64; x++)
                    {
                        float d = Vector2.Distance(new Vector2(x, y), new Vector2(31.5f, 31.5f)) / 32f;
                        float a = Mathf.Clamp01(1f - d);
                        tex.SetPixel(x, y, new Color(1f, 1f, 1f, a * a));
                    }
                }
                tex.Apply();
                return BuildSprite(tex, Vector4.zero);
            });
        }

        private static Sprite Cached(string key, System.Func<Sprite> factory)
        {
            if (!Cache.TryGetValue(key, out Sprite sprite) || sprite == null)
            {
                sprite = factory();
                Cache[key] = sprite;
            }

            return sprite;
        }

        private static Sprite BuildSprite(Texture2D texture, Vector4 border)
        {
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;
            return Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                WorldPixelsPerUnit,
                0,
                SpriteMeshType.FullRect,
                border);
        }

        private static Texture2D NewTexture(int width, int height)
        {
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point
            };
            var clear = new Color[width * height];
            texture.SetPixels(clear);
            return texture;
        }

        private static void FillRect(Texture2D texture, int x, int y, int width, int height, Color color)
        {
            for (int py = y; py < y + height; py++)
            {
                for (int px = x; px < x + width; px++)
                {
                    SetPixel(texture, px, py, color);
                }
            }
        }

        private static void FillEllipse(Texture2D texture, int cx, int cy, int radiusX, int radiusY, Color color)
        {
            float invX = 1f / Mathf.Max(1, radiusX);
            float invY = 1f / Mathf.Max(1, radiusY);
            for (int y = cy - radiusY; y <= cy + radiusY; y++)
            {
                for (int x = cx - radiusX; x <= cx + radiusX; x++)
                {
                    float dx = (x - cx) * invX;
                    float dy = (y - cy) * invY;
                    if (dx * dx + dy * dy <= 1f)
                    {
                        SetPixel(texture, x, y, color);
                    }
                }
            }
        }

        private static void FillCircle(Texture2D texture, int cx, int cy, int radius, Color color)
        {
            FillEllipse(texture, cx, cy, radius, radius, color);
        }

        private static void FillRoundedRect(
            Texture2D texture,
            int x,
            int y,
            int width,
            int height,
            int radius,
            Color color)
        {
            for (int py = y; py < y + height; py++)
            {
                for (int px = x; px < x + width; px++)
                {
                    float dx = Mathf.Max(x + radius - px, 0f, px - (x + width - radius - 1));
                    float dy = Mathf.Max(y + radius - py, 0f, py - (y + height - radius - 1));
                    if (dx * dx + dy * dy <= radius * radius)
                    {
                        SetPixel(texture, px, py, color);
                    }
                }
            }
        }

        private static void SetPixel(Texture2D texture, int x, int y, Color color)
        {
            if (x < 0 || x >= texture.width || y < 0 || y >= texture.height)
            {
                return;
            }

            Color current = texture.GetPixel(x, y);
            texture.SetPixel(x, y, Color.Lerp(current, color, color.a));
        }
    }
}
