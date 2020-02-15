using Bot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunPellet : Projectile
{

    [SerializeField]
    private float lifeTimer = 1.5f;

    [SerializeField]
    private float speed = 1;

    [SerializeField]
    private int damage = 1;

    private Collider2D col;
    private LineRenderer lr;
    private Rigidbody2D rb;

    protected override void SubAwake()
    {
        col = GetComponent<Collider2D>();
        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected override void SubOnEnable()
    {
        col.enabled = false;
    }

    protected override void Init()
    {
        rb.velocity = transform.right * speed;
        col.enabled = true;
        //Putting this co-routine in start proved non-functional
        StartCoroutine(DeathTimer());
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject != shooter.gameObject) {
            if (c.TryGetComponent<Health>(out Health health)) {
                health.TakeDamage(damage);
            }
            Pools.Instance.Free(gameObject);
        }
    }

    private IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(lifeTimer);

        Pools.Instance.Free(gameObject);
    }

}
