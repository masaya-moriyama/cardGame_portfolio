using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] public ParticleSystem hitParticle;
    [SerializeField] public ParticleSystem regenerateParticle;

    // シングルトン化
    public static ParticleManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void StartHitParticle(Transform target)
    {
        ParticleSystem newParticle = Instantiate(hitParticle, target.parent);
        newParticle.transform.localScale = new Vector3(100, 100, 1);
        newParticle.transform.position = target.transform.position;
        newParticle.Play();
        Destroy(newParticle.gameObject, 5.0f);
    }

    public void StartRegenerateParticle(Transform target)
    {
        ParticleSystem newParticle = Instantiate(regenerateParticle, target);
        newParticle.transform.localScale = new Vector3(30, 30, 1);
        newParticle.transform.position = target.transform.position;
        newParticle.Play();
        Destroy(newParticle.gameObject, 5.0f);
    }
}
