using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonOnClickEffect : MonoBehaviour
{
    Button btn;
    Tween t;

    void Start()
    {
        btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() => DOEffect());
        }
    }

    public void DOEffect()
    {
        if (t == null || !t.IsPlaying() || !t.IsActive())
        {
            t = btn.transform.DOPunchRotation(Vector3.one * 1.5f, 1f);
        }
    }

}
