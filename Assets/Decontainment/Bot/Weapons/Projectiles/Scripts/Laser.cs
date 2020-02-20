using Bot;
using UnityEngine;

public class Laser : Beam
{
    private static LayerMask MASK;
    private static Trigger doOnce;

    [SerializeField]
    private int damage = 1;
    [SerializeField]
    private int damage = 1;

    protected override void HealthEffect(Health bh) {
        bh.TakeDamage(damage);
    }
}
