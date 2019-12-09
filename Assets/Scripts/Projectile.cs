using Bot;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public Shooter shooter;

    public static void CreateProjectile(Shooter shooter, GameObject prefab, Vector2 position, Vector2 look)
    {
        GameObject go = Instantiate(prefab, position, Quaternion.identity);
        Projectile proj = go.GetComponent<Projectile>();
        proj.shooter = shooter;
        proj.transform.right = look;
        BotManager.Instance.Projectiles.Add(go.transform);
    }

    protected void OnDestroy()
    {
        BotManager.Instance?.Projectiles.Remove(transform);
    }
}