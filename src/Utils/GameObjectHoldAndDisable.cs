using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectHoldAndDisable : MonoBehaviour
{
    public float HoldSeconds = 3f;
    IEnumerator DisableCoroutine;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (DisableCoroutine != null) StopCoroutine(DisableCoroutine);
        DisableCoroutine = DODisable();
        StartCoroutine(DisableCoroutine);
    }

    IEnumerator DODisable()
    {
        yield return new WaitForSeconds(HoldSeconds);
        gameObject.SetActive(false);
    }
}
