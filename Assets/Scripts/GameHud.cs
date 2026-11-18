using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TurnTheOrientation
{
    public sealed class GameHud : MonoBehaviour
    {
        private TurnGame _game;
        private TMP_FontAsset _font;
        private GameObject _playRoot;
        private GameObject _titleRoot;
        private GameObject _pauseRoot;
        private GameObject _clearRoot;
        private GameObject _finalRoot;
        private TMP_Text _levelText;
        private TMP_Text _viewText;
        private TMP_Text _socketText;
        private TMP_Text _sparkText;
        private TMP_Text _hintText;
        private TMP_Text _controlText;
        private TMP_Text _clearLabel;
        private TMP_Text _clearPrompt;
        private TMP_Text _titlePrompt;
        private Image _viewBar;

        public void Initialize(TurnGame game)
        {
            _game = game;
            _font = TMP_Settings.defaultFontAsset;
            if (_font == null)
            {
                Font sourceFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                _font = TMP_FontAsset.CreateFontAsset(sourceFont);
            }

            Canvas canvas = CreateCanvas();
            _playRoot = CreateFullscreenRoot("PlayHUD", canvas.transform);
            _titleRoot = CreateOverlay("TitleOverlay", canvas.transform, GamePalette.Alpha(GamePalette.Ink, 0.72f));
            _pauseRoot = CreateOverlay("PauseOverlay", canvas.transform, GamePalette.Alpha(GamePalette.Ink, 0.78f));
            _clearRoot = CreateOverlay("ClearOverlay", canvas.transform, GamePalette.Alpha(GamePalette.Ink, 0.68f));
            _finalRoot = CreateOverlay("FinalOverlay", canvas.transform, GamePalette.Alpha(GamePalette.Ink, 0.84f));

            BuildPlayHud();
            BuildTitle();
            BuildPause();
            BuildClear();
            BuildFinal();
        }

        public void Refresh(TurnGame game)
        {
            if (game == null || _playRoot == null)
            {
                return;
            }

            _playRoot.SetActive(game.State != TurnGame.GameState.Title);
            _titleRoot.SetActive(game.State == TurnGame.GameState.Title);
            _pauseRoot.SetActive(game.State == TurnGame.GameState.Paused);
            _clearRoot.SetActive(game.State == TurnGame.GameState.RoomCleared);
            _finalRoot.SetActive(game.State == TurnGame.GameState.AllCleared);

            _levelText.text = $"ROOM {game.CurrentLevelIndex + 1:00} / {game.LevelCount:00}\n{game.LevelTitle}";
            _viewText.text = $"{game.ModeLabel}  //  {game.RoomRotation:000} DEG";
            _viewBar.color = game.Mode == PuzzleViewMode.TopDown ? GamePalette.Player : GamePalette.Accent;
            _socketText.text = $"SOCKETS  {game.LatchedPlates} / {game.TotalPlates}";
            _socketText.color = game.LatchedPlates == game.TotalPlates ? GamePalette.Success : GamePalette.Text;
            _sparkText.text = $"SPARKS  {game.Sparks}";
            _hintText.text = game.LevelHint;
            _controlText.text = game.Mode == PuzzleViewMode.TopDown
                ? "WASD  MOVE     B / N  TURN     R  RESET     ESC  PAUSE"
                : "A D  RUN     SPACE  JUMP     B / N  TURN     R  RESET";

            float pulse = 0.62f + Mathf.Sin(Time.unscaledTime * 3.5f) * 0.38f;
            _titlePrompt.alpha = pulse;
            _clearPrompt.alpha = pulse;
        }

        private void BuildPlayHud()
        {
            RectTransform levelPanel = CreatePanel(
                "LevelPanel",
                _playRoot.transform,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(26f, -24f),
                new Vector2(345f, 86f),
                GamePalette.Alpha(GamePalette.Ink, 0.84f));
            _levelText = CreateText(
                "LevelText",
                levelPanel,
                new Vector2(18f, -12f),
                new Vector2(-16f, 12f),
                22f,
                TextAlignmentOptions.TopLeft,
                GamePalette.Text);
            _levelText.text = "ROOM 01 / 03\nFIRST TURN";

            RectTransform viewPanel = CreatePanel(
                "ViewPanel",
                _playRoot.transform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -24f),
                new Vector2(330f, 64f),
                GamePalette.Alpha(GamePalette.Ink, 0.84f));
            _viewText = CreateText(
                "ViewText",
                viewPanel,
                new Vector2(14f, -8f),
                new Vector2(-14f, -8f),
                22f,
                TextAlignmentOptions.Center,
                GamePalette.Text);
            _viewBar = CreateImage(
                "ViewBar",
                viewPanel,
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 4f),
                new Vector2(210f, 4f),
                GamePalette.Player);

            RectTransform statusPanel = CreatePanel(
                "StatusPanel",
                _playRoot.transform,
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-26f, -24f),
                new Vector2(300f, 86f),
                GamePalette.Alpha(GamePalette.Ink, 0.84f));
            _socketText = CreateText(
                "SocketText",
                statusPanel,
                new Vector2(16f, -12f),
                new Vector2(-16f, 38f),
                23f,
                TextAlignmentOptions.TopRight,
                GamePalette.Text);
            _sparkText = CreateText(
                "SparkText",
                statusPanel,
                new Vector2(16f, -42f),
                new Vector2(-16f, 12f),
                17f,
                TextAlignmentOptions.BottomRight,
                GamePalette.MutedText);

            RectTransform hintPanel = CreatePanel(
                "HintPanel",
                _playRoot.transform,
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 148f),
                new Vector2(940f, 48f),
                GamePalette.Alpha(GamePalette.Ink, 0.72f));
            _hintText = CreateText(
                "HintText",
                hintPanel,
                new Vector2(12f, 0f),
                new Vector2(-12f, 0f),
                18f,
                TextAlignmentOptions.Center,
                GamePalette.MutedText);

            RectTransform controlPanel = CreatePanel(
                "ControlPanel",
                _playRoot.transform,
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 24f),
                new Vector2(1060f, 48f),
                GamePalette.Alpha(GamePalette.Ink, 0.9f));
            _controlText = CreateText(
                "ControlText",
                controlPanel,
                new Vector2(10f, -1f),
                new Vector2(-10f, 1f),
                17f,
                TextAlignmentOptions.Center,
                GamePalette.Text);
        }

        private void BuildTitle()
        {
            CreateAccentLine(_titleRoot.transform, new Vector2(0f, -48f), GamePalette.Accent);

            TMP_Text eyebrow = CreateText(
                "Eyebrow",
                _titleRoot.transform,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                17f,
                TextAlignmentOptions.Center,
                GamePalette.MutedText,
                new Vector2(0f, 150f),
                new Vector2(900f, 36f));
            eyebrow.text = "A TWO-PERSPECTIVE PUZZLE";
            eyebrow.characterSpacing = 11f;

            TMP_Text title = CreateText(
                "Title",
                _titleRoot.transform,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                76f,
                TextAlignmentOptions.Center,
                GamePalette.Text,
                new Vector2(0f, 72f),
                new Vector2(1300f, 120f));
            title.text = "TURN / FLOOR";
            title.characterSpacing = 2f;

            TMP_Text subtitle = CreateText(
                "Subtitle",
                _titleRoot.transform,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                22f,
                TextAlignmentOptions.Center,
                GamePalette.Player,
                new Vector2(0f, -8f),
                new Vector2(1150f, 48f));
            subtitle.text = "Push the room into order. Turn gravity into a route.";

            TMP_Text controls = CreateText(
                "Controls",
                _titleRoot.transform,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                18f,
                TextAlignmentOptions.Center,
                GamePalette.MutedText,
                new Vector2(0f, -84f),
                new Vector2(1200f, 40f));
            controls.text = "WASD MOVE     B / N ROTATE 90 DEG     SPACE JUMP     R RESET";

            _titlePrompt = CreateText(
                "Prompt",
                _titleRoot.transform,
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                20f,
                TextAlignmentOptions.Center,
                GamePalette.Accent,
                new Vector2(0f, 84f),
                new Vector2(700f, 44f));
            _titlePrompt.text = "PRESS ENTER TO TURN THE FIRST ROOM";
        }

        private void BuildPause()
        {
            TMP_Text title = CreateCenteredText(
                _pauseRoot.transform,
                "PAUSED",
                64f,
                GamePalette.Player,
                new Vector2(0f, 60f));
            title.characterSpacing = 8f;
            CreateCenteredText(
                _pauseRoot.transform,
                "ESC  RESUME     R  RESTART ROOM",
                20f,
                GamePalette.MutedText,
                new Vector2(0f, -42f));
        }

        private void BuildClear()
        {
            CreateAccentLine(_clearRoot.transform, new Vector2(0f, 58f), GamePalette.Success);
            _clearLabel = CreateCenteredText(
                _clearRoot.transform,
                "ROOM CLEARED",
                62f,
                GamePalette.Text,
                new Vector2(0f, 92f));
            _clearLabel.characterSpacing = 3f;
            _clearPrompt = CreateCenteredText(
                _clearRoot.transform,
                "ENTER  NEXT ROOM",
                21f,
                GamePalette.Success,
                new Vector2(0f, -26f));
        }

        private void BuildFinal()
        {
            TMP_Text title = CreateCenteredText(
                _finalRoot.transform,
                "SIGNAL RESTORED",
                66f,
                GamePalette.Success,
                new Vector2(0f, 84f));
            title.characterSpacing = 5f;
            CreateCenteredText(
                _finalRoot.transform,
                "ALL THREE ROOMS AGREE WITH YOU.",
                21f,
                GamePalette.Text,
                new Vector2(0f, 4f));
            CreateCenteredText(
                _finalRoot.transform,
                "PRESS ENTER TO PLAY AGAIN",
                19f,
                GamePalette.Accent,
                new Vector2(0f, -86f));
        }

        private Canvas CreateCanvas()
        {
            var canvasObject = new GameObject("Game Canvas", typeof(RectTransform));
            canvasObject.transform.SetParent(transform, false);
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 20;
            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private GameObject CreateFullscreenRoot(string name, Transform parent)
        {
            var root = new GameObject(name, typeof(RectTransform));
            root.transform.SetParent(parent, false);
            RectTransform rect = root.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return root;
        }

        private GameObject CreateOverlay(string name, Transform parent, Color color)
        {
            GameObject root = CreateFullscreenRoot(name, parent);
            Image scrim = root.AddComponent<Image>();
            scrim.sprite = SpriteFactory.UiPanel(Color.white, 1);
            scrim.color = color;
            scrim.raycastTarget = false;
            return root;
        }

        private RectTransform CreatePanel(
            string name,
            Transform parent,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 anchoredPosition,
            Vector2 size,
            Color color)
        {
            var panelObject = new GameObject(name, typeof(RectTransform));
            panelObject.transform.SetParent(parent, false);
            RectTransform rect = panelObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
            Image image = panelObject.AddComponent<Image>();
            image.sprite = SpriteFactory.UiPanel(Color.white);
            image.type = Image.Type.Sliced;
            image.color = color;
            image.raycastTarget = false;
            return rect;
        }

        private Image CreateImage(
            string name,
            Transform parent,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 anchoredPosition,
            Vector2 size,
            Color color)
        {
            var imageObject = new GameObject(name, typeof(RectTransform));
            imageObject.transform.SetParent(parent, false);
            RectTransform rect = imageObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
            Image image = imageObject.AddComponent<Image>();
            image.sprite = SpriteFactory.UiPanel(Color.white, 2);
            image.type = Image.Type.Sliced;
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private TMP_Text CreateText(
            string name,
            Transform parent,
            Vector2 anchorMin,
            Vector2 anchorMax,
            float fontSize,
            TextAlignmentOptions alignment,
            Color color)
        {
            var textObject = new GameObject(name, typeof(RectTransform));
            textObject.transform.SetParent(parent, false);
            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
            text.font = _font;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
            text.textWrappingMode = TextWrappingModes.NoWrap;
            text.overflowMode = TextOverflowModes.Overflow;
            text.raycastTarget = false;
            text.fontStyle = FontStyles.Bold;
            return text;
        }

        private TMP_Text CreateText(
            string name,
            Transform parent,
            Vector2 anchorMin,
            Vector2 anchorMax,
            float fontSize,
            TextAlignmentOptions alignment,
            Color color,
            Vector2 anchoredPosition,
            Vector2 size)
        {
            TMP_Text text = CreateText(name, parent, anchorMin, anchorMax, fontSize, alignment, color);
            RectTransform rect = text.rectTransform;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
            return text;
        }

        private TMP_Text CreateText(
            string name,
            RectTransform parent,
            Vector2 topLeftOffset,
            Vector2 bottomRightOffset,
            float fontSize,
            TextAlignmentOptions alignment,
            Color color)
        {
            var textObject = new GameObject(name, typeof(RectTransform));
            textObject.transform.SetParent(parent, false);
            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(topLeftOffset.x, bottomRightOffset.y);
            rect.offsetMax = new Vector2(bottomRightOffset.x, topLeftOffset.y);
            TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
            text.font = _font;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.overflowMode = TextOverflowModes.Ellipsis;
            text.raycastTarget = false;
            text.fontStyle = FontStyles.Bold;
            return text;
        }

        private TMP_Text CreateCenteredText(
            Transform parent,
            string value,
            float fontSize,
            Color color,
            Vector2 position)
        {
            TMP_Text text = CreateText(
                "CenteredText",
                parent,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                fontSize,
                TextAlignmentOptions.Center,
                color,
                position,
                new Vector2(1200f, 100f));
            text.text = value;
            return text;
        }

        private void CreateAccentLine(Transform parent, Vector2 position, Color color)
        {
            CreateImage(
                "AccentLine",
                parent,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                position,
                new Vector2(240f, 7f),
                color);
        }
    }
}
