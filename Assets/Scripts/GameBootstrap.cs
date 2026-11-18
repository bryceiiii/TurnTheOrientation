using UnityEngine;

namespace TurnTheOrientation
{
    [DefaultExecutionOrder(-1000)]
    public sealed class GameBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 60;

            Camera gameCamera = Camera.main;
            if (gameCamera == null)
            {
                var cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                gameCamera = cameraObject.AddComponent<Camera>();
                cameraObject.AddComponent<AudioListener>();
            }

            gameCamera.orthographic = true;
            gameCamera.orthographicSize = 6.45f;
            gameCamera.transform.position = new Vector3(0f, 0f, -10f);
            gameCamera.clearFlags = CameraClearFlags.SolidColor;
            gameCamera.backgroundColor = GamePalette.Ink;
            gameCamera.nearClipPlane = 0.1f;
            gameCamera.farClipPlane = 50f;

            var audioObject = new GameObject("Game Audio");
            audioObject.transform.SetParent(transform, false);
            GameAudio audio = audioObject.AddComponent<GameAudio>();
            audio.Initialize();

            var hudObject = new GameObject("Game HUD");
            hudObject.transform.SetParent(transform, false);
            GameHud hud = hudObject.AddComponent<GameHud>();

            var gameRoot = new GameObject("TurnGame");
            gameRoot.transform.SetParent(transform, false);
            TurnGame game = gameRoot.AddComponent<TurnGame>();
            game.Initialize(gameCamera, audio, hud);
        }
    }
}
