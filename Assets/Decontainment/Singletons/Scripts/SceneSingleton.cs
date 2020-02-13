using UnityEngine;

/// <summary>
/// Inherit from this base class to create a scene singleton.
/// e.g. public class MyClassName : SceneSingleton<MyClassName> {}
/// from: https://wiki.unity3d.com/index.php/Singleton
/// Scene singletons must be present in a scene in order to use them
///
/// </summary>
public class SceneSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static object m_Lock = new object();
    private static T m_Instance;

    /// <summary>
    /// Access singleton instance through this property.
    /// </summary>
    public static T Instance
    {
        get {
            lock (m_Lock) {
                if (m_Instance == null) {
                    // Search for existing instance.
                    m_Instance = (T)FindObjectOfType(typeof(T));
                }

                return m_Instance;
            }
        }
    }
}