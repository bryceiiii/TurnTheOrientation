using UnityEngine;

namespace TurnTheOrientation
{
    [DefaultExecutionOrder(-1000)]
    public sealed class GameBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            if (GetComponent<LevelManager>() == null)
            {
                gameObject.AddComponent<LevelManager>();
            }
        }
    }
}
