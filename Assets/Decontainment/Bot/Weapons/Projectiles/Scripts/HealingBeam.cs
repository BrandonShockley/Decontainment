using Bot;
using UnityEngine;

public class HealingBeam : Beam 
{
    [SerializeField]
    private int healthRegain = 1;

    protected override void HealthEffect(Health bh) {
        bh.HealUp(healthRegain);
    }
}
