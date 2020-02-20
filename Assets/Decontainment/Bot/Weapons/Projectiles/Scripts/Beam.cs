using Bot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Beam : Projectile 
{
    private static LayerMask MASK;
    private static Trigger doOnce;

    [SerializeField]
    private float maxDistance = 100;
    [SerializeField]
    private float beamGrowTime = 1;
    /// Duration after it has finished growing
    [SerializeField]
    private float beamDuration = 1;
    [SerializeField]
    private float beamFadeTime = 0.5f;

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
        StartCoroutine(beamRoutine());
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

    protected virtual void HealthEffect(Health bh) { }

    private IEnumerator beamRoutine()
    {
        // beam growth
        {
            float startTime = Time.time;
            bool doOnce = false;
            do {
                if (doOnce)
                    yield return null;

                float alpha = Mathf.Lerp(0, 1, (Time.time - startTime) / beamFadeTime);
                lr.endColor = Util.ModifyAlpha(lr.endColor, alpha);
                lr.startColor = Util.ModifyAlpha(lr.endColor, alpha);

                doOnce = true;
            } while (Time.time - startTime < beamGrowTime);
        }

        // beam life
        {
            float startTime = Time.time;
            bool doOnce = false;
            bool hit = false;
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
                    HealthEffect(bh);
                    hit = true;
                }

                doOnce = true;

                if (hit)
                    yield return new WaitForSeconds(beamDuration - (Time.time - startTime));

            } while (Time.time - startTime < beamDuration);
        }

        // beam death
        {
            float startTime = Time.time;
            bool doOnce = false;
            do {
                if (doOnce)
                    yield return null;

                float alpha = Mathf.Lerp(1, 0, (Time.time - startTime) / beamFadeTime);
                lr.endColor = Util.ModifyAlpha(lr.endColor, alpha);
                lr.startColor = Util.ModifyAlpha(lr.endColor, alpha);

                doOnce = true;
            } while (Time.time - startTime < beamFadeTime);
        }
        Pools.Instance.Free(gameObject);
    }
}
