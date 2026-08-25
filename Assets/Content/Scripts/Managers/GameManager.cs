using Template.Core;
using UnityEngine;

namespace Template.Managers
{
    /// <summary>
    /// Empty gameplay coordinator stub. Fill this in per project.
    /// </summary>
    public sealed class GameManager : Singleton<GameManager>
    {
        public bool IsPaused { get; private set; }

        public void StartRun()
        {
            Debug.Log("[GameManager] StartRun called. Hook gameplay bootstrap here.");
        }

        public void SetPaused(bool paused)
        {
            IsPaused = paused;
            Time.timeScale = paused ? 0f : 1f;
        }
    }
}
