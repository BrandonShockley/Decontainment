using Bot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Beam : Projectile 
{
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
    }

    protected override void Init() {
        StartCoroutine(beamRoutine());
    }

    void Update() 
    {
        LayerMask mask = LayerMask.GetMask("Obstacle", "Bot");
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.right, maxDistance, mask);
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

    private IEnumerator beamRoutine() {
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
            do {
                if (doOnce)
                    break;

                LayerMask mask = LayerMask.GetMask("Obstacle", "Bot");
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.right, maxDistance, mask);
                int hitIndex = -1;
                for (int i = 0; i < hits.Length; ++i) {
                    if (hits[i].collider.gameObject != shooter.gameObject) {
                        hitIndex = i;
                        break;
                    }
                }
                if (hitIndex != -1 && hits[hitIndex].collider.TryGetComponent<Health>(out Health bh)) {
                    HealthEffect(bh);
                }

                doOnce = true;
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

    protected virtual void HealthEffect(Health bh) {}
}
