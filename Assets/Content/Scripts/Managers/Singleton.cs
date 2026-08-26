using UnityEngine;

namespace Template.Core
{
    /// <summary>
    /// Persistent singleton base. Handles duplicate destruction and DontDestroyOnLoad.
    /// The generic type check stops accidental inheritance mistakes like HUD : Singleton&lt;AudioManager&gt;.
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (GetType() != typeof(T))
            {
                Debug.LogError(
                    $"Singleton inheritance error: {GetType().FullName} cannot be the singleton for {typeof(T).FullName}.");
                Destroy(gameObject);
                return;
            }

            if (Instance != null && Instance != this)
            {
                Debug.LogWarning($"Duplicate singleton for {typeof(T).Name}. Destroying the new instance.");
                Destroy(gameObject);
                return;
            }

            Instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
    }
}
