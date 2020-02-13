using Bot;
using UnityEngine;

public class Laser : Beam
{
    [SerializeField]
    private int damage = 1;

    protected override void HealthEffect(Health bh) {
        bh.TakeDamage(damage);
    }
}
