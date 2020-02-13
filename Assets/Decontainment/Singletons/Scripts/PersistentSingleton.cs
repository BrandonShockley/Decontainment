using UnityEngine;

/// <summary>
/// Inherit from this base class to create a persistent singleton.
/// e.g. public class MyClassName : PersistentSingleton<MyClassName> {}
/// Persistent singletons are created on first reference and destroyed on application quit
/// from: https://wiki.unity3d.com/index.php/Singleton
/// </summary>
public class PersistentSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static object m_Lock = new object();
    private static T m_Instance;
    private static bool destroyed;

    /// <summary>
    /// Access singleton instance through this propriety.
    /// </summary>
    public static T Instance
    {
        get {
            lock (m_Lock) {
                if (destroyed) {
                    return null;
                }
                if (m_Instance == null) {
                    // Need to create a new GameObject to attach the singleton to.
                    var singletonObject = new GameObject();
                    m_Instance = singletonObject.AddComponent<T>();
                    singletonObject.name = typeof(T).ToString() + " (Singleton)";

                    // Make instance persistent.
                    DontDestroyOnLoad(singletonObject);
                }

                return m_Instance;
            }
        }
    }

    protected void OnDestroy()
    {
        destroyed = true;
    }
}