using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkinManager : MonoBehaviourSingleton<SkinManager>
{
    int MaxSkinPower = 28;
    public List<Skin> SkinPrefabs = new List<Skin>();

    public float SkinPriceTakeMultiplier = 0.3f; // Porcentaje que se restara del precio de los prefabs, para evitar descuentos manuales

    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        foreach(Skin s in SkinPrefabs)
        {
            s.transform.position = Vector3.zero;
            s.transform.localPosition = Vector3.zero;
            if (s.Power > MaxSkinPower) MaxSkinPower = s.Power;
            if (SkinPriceTakeMultiplier > 0)
            {
                int newPrice = s.RequiredSavedAliens - (int)(s.RequiredSavedAliens * SkinPriceTakeMultiplier);
                if (newPrice >= 0) {
                    s.RequiredSavedAliens = newPrice;
                }

                newPrice = s.RequiredStolenBriefcases - (int)(s.RequiredStolenBriefcases * SkinPriceTakeMultiplier);
                if (newPrice >= 0)
                {
                    s.RequiredStolenBriefcases = newPrice;
                }
            }
        }
        SkinPrefabs.OrderBy(s => s.ID);
    }

    public Skin GetRandomSkinPrefab()
    {
        return SkinPrefabs[UnityEngine.Random.Range(0, SkinPrefabs.Count)];
    }

    public Skin GetSkinPrefab(int ID)
    {
        return SkinPrefabs.Find(s => s.ID == ID);
    }

    public float GetSkinLinearInterpolationPower(int ID)
    {
        Skin s = GetSkinPrefab(ID);
        return (float)s.Power / (float)MaxSkinPower;
    }
}
