using Bot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : Projectile
{
    [SerializeField]
    private float speed = 1;

    private Collider2D col;
    private LineRenderer lr;
    private Rigidbody2D rb;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        col.enabled = false;
    }

    public override void Init()
    {
        rb.velocity = transform.right * speed;
        col.enabled = true;
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject != shooter.gameObject) {
            if (c.TryGetComponent<Health>(out Health health)) {
                health.TakeDamage(1);
            }
            Pools.Instance.Free(gameObject);
        }
    }
}
