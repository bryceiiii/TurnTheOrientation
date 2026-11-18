using UnityEngine;

namespace TurnTheOrientation
{
    public sealed class LevelSceneController : MonoBehaviour
    {
        [SerializeField] private string levelTitle = "UNTITLED ROOM";
        [SerializeField] private string levelHint = string.Empty;
        [SerializeField] private TurnPlayer player;
        [SerializeField] private DoorGate gate;
        [SerializeField] private ExitPortal exit;
        [SerializeField] private PushableCrate[] crates = new PushableCrate[0];
        [SerializeField] private PressurePlate[] plates = new PressurePlate[0];

        public string LevelTitle => levelTitle;
        public string LevelHint => levelHint;
        public TurnPlayer Player => player;
        public DoorGate Gate => gate;
        public ExitPortal Exit => exit;
        public PushableCrate[] Crates => crates;
        public PressurePlate[] Plates => plates;
        public Vector2 SpawnPoint => player != null ? player.transform.localPosition : Vector2.zero;

        public void Prepare(TurnGame game)
        {
            if (player == null)
            {
                Debug.LogError($"{name} has no player reference.", this);
                return;
            }

            Transform playerVisual = player.transform.Find("Visual");
            player.Setup(game, playerVisual, player.transform.localPosition);

            for (int i = 0; i < crates.Length; i++)
            {
                if (crates[i] != null)
                {
                    crates[i].Setup(crates[i].transform.localPosition);
                }
            }

            for (int i = 0; i < plates.Length; i++)
            {
                if (plates[i] != null)
                {
                    plates[i].Setup(game, plates[i].transform.Find("Visual"));
                }
            }

            if (gate != null)
            {
                gate.Setup(Vector2.one);
            }

            if (exit != null)
            {
                exit.Setup(game, exit.transform.Find("Visual"));
            }
        }

        [ContextMenu("Refresh Level References")]
        private void RefreshLevelReferences()
        {
            player = GetComponentInChildren<TurnPlayer>(true);
            gate = GetComponentInChildren<DoorGate>(true);
            exit = GetComponentInChildren<ExitPortal>(true);
            crates = GetComponentsInChildren<PushableCrate>(true);
            plates = GetComponentsInChildren<PressurePlate>(true);
        }
    }
}
