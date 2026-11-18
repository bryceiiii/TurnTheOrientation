using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TurnTheOrientation
{
    public sealed class GameHud : MonoBehaviour
    {
        [SerializeField] private GameObject _playRoot;
        [SerializeField] private GameObject _titleRoot;
        [SerializeField] private GameObject _pauseRoot;
        [SerializeField] private GameObject _clearRoot;
        [SerializeField] private GameObject _finalRoot;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _viewText;
        [SerializeField] private TMP_Text _socketText;
        [SerializeField] private TMP_Text _sparkText;
        [SerializeField] private TMP_Text _hintText;
        [SerializeField] private TMP_Text _controlText;
        [SerializeField] private TMP_Text _clearLabel;
        [SerializeField] private TMP_Text _clearPrompt;
        [SerializeField] private TMP_Text _titlePrompt;
        [SerializeField] private Image _viewBar;

        public void Initialize(TurnGame game)
        {
            if (game == null)
            {
                Debug.LogError("GameHud requires a TurnGame reference.", this);
            }

            if (!HasRequiredReferences())
            {
                Debug.LogError("GameHud prefab is missing one or more UI references.", this);
            }
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

        private bool HasRequiredReferences()
        {
            return _playRoot != null &&
                   _titleRoot != null &&
                   _pauseRoot != null &&
                   _clearRoot != null &&
                   _finalRoot != null &&
                   _levelText != null &&
                   _viewText != null &&
                   _socketText != null &&
                   _sparkText != null &&
                   _hintText != null &&
                   _controlText != null &&
                   _clearLabel != null &&
                   _clearPrompt != null &&
                   _titlePrompt != null &&
                   _viewBar != null;
        }
    }
}
