using Bot;
using UnityEngine;

[RequireComponent(typeof(SoundModulator))]
public abstract class Projectile : MonoBehaviour, IPoolable
{
    protected Shooter shooter;

    protected SoundModulator sm;

    [SerializeField]
    private AudioClip shotSound = null;

    private bool managed = false;

    public static void CreateProjectile(Shooter shooter, GameObject prefab, Vector2 position, Vector2 look)
    {
        GameObject go = Pools.Instance.Get(prefab, position, Quaternion.identity);
        Projectile proj = go.GetComponent<Projectile>();
        proj.shooter = shooter;
        proj.transform.right = look;
        if (proj.shooter.weaponData.operateInBotSpace) {
            go.transform.parent = shooter.gameObject.transform;
        }
        proj.sm.PlayClip(proj.shotSound);
        proj.Init();
        BotManager.Instance.Projectiles.Add(proj);
        proj.managed = true;
    }

    public void OnGet()
    {
        SubOnGet();
    }

    public void OnFree()
    {
        if (managed) {
            BotManager.Instance?.Projectiles.Remove(this);
        }
        SubOnFree();
    }

    protected virtual void Init() {}

    protected void Awake()
    {
        sm = GetComponent<SoundModulator>();
        SubAwake();
    }

    protected virtual void SubAwake() {}
    protected virtual void SubOnGet() {}
    protected virtual void SubOnFree() {}
}