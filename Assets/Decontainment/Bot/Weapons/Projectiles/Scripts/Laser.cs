using Bot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Projectile
{
    private static LayerMask MASK;
    private static Trigger doOnce;

    [SerializeField]
    private int damage = 1;
    [SerializeField]
    private float maxDistance = 100;
    [SerializeField]
    private float laserGrowTime = 1;
    /// Duration after it has finished growing
    [SerializeField]
    private float laserDuration = 1;
    [SerializeField]
    private float laserFadeTime = 0.5f;

    private LineRenderer lr;

    protected override void SubAwake()
    {
        lr = GetComponent<LineRenderer>();

        if (!doOnce.Value) {
            MASK = LayerMask.GetMask("Obstacle", "Bot");
        }
    }

    protected override void Init()
    {
        StartCoroutine(LaserRoutine());
    }

    void Update()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.right, maxDistance, MASK);
        int hitIndex = -1;
        for (int i = 0; i < hits.Length; ++i) {
            if (hits[i].collider.gameObject != shooter.gameObject) {
                hitIndex = i;
                break;
            }
        }

        if (hitIndex == -1) {
            lr.SetPosition(1, Vector3.right * maxDistance);
        } else {
            lr.SetPosition(1, Vector3.right * hits[hitIndex].distance);
        }
    }

    private IEnumerator LaserRoutine()
    {
        // Laser growth
        {
            float startTime = Time.time;
            bool doOnce = false;
            do {
                if (doOnce)
                    yield return null;

                float alpha = Mathf.Lerp(0, 1, (Time.time - startTime) / laserFadeTime);
                lr.endColor = Util.ModifyAlpha(lr.endColor, alpha);
                lr.startColor = Util.ModifyAlpha(lr.endColor, alpha);

                doOnce = true;
            } while (Time.time - startTime < laserGrowTime);
        }

        // Laser life
        {
            float startTime = Time.time;
            bool doOnce = false;
            do {
                if (doOnce)
                    yield return null;

                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.right, maxDistance, MASK);
                int hitIndex = -1;
                for (int i = 0; i < hits.Length; ++i) {
                    if (hits[i].collider.gameObject != shooter.gameObject) {
                        hitIndex = i;
                        break;
                    }
                }
                if (hitIndex != -1 && hits[hitIndex].collider.TryGetComponent<Health>(out Health bh)) {
                    bh.TakeDamage(damage);
                }

                doOnce = true;
            } while (Time.time - startTime < laserDuration);
        }

        // Laser death
        {
            float startTime = Time.time;
            bool doOnce = false;
            do {
                if (doOnce)
                    yield return null;

                float alpha = Mathf.Lerp(1, 0, (Time.time - startTime) / laserFadeTime);
                lr.endColor = Util.ModifyAlpha(lr.endColor, alpha);
                lr.startColor = Util.ModifyAlpha(lr.endColor, alpha);

                doOnce = true;
            } while (Time.time - startTime < laserFadeTime);
        }
        Pools.Instance.Free(gameObject);
    }
}
