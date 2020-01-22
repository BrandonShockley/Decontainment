using Bot;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public Shooter shooter;

    private bool managed = false;

    // TODO: Using Pool.Type is unsafe as other types of pooled objects could be used
    // Maybe in the future I could have a distinct pool for projectiles
    public static void CreateProjectile(Shooter shooter, GameObject prefab, Vector2 position, Vector2 look)
    {
        GameObject go = Pools.Instance.Get(prefab, position, Quaternion.identity);
        Projectile proj = go.GetComponent<Projectile>();
        proj.shooter = shooter;
        proj.transform.right = look;
        proj.Init();
        BotManager.Instance.Projectiles.Add(go.transform);
        proj.managed = true;
    }

    public virtual void Init() {}

    protected void OnDestroy()
    {
        if (managed) {
            BotManager.Instance?.Projectiles.Remove(transform);
        }
    }
}