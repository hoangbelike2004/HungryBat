using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticalItem : GameUnit
{
    private ParticleSystem par;
    private void Awake()
    {
        par = GetComponent<ParticleSystem>();
    }
    public void ActiveAndWaitForDeactive()
    {
        if (par == null) return;
        par.Play();
        StartCoroutine(DespawnPartical());
    }
    IEnumerator DespawnPartical()
    {
        while (true)
        {
            if (!par.IsAlive())
            {
                SimplePool.Despawn(this);
            }
            yield return null;
        }
    }
}
