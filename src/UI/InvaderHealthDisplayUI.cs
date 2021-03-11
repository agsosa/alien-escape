using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvaderHealthDisplayUI : MonoBehaviour
{
    public Color DeadBGColor;
    public Color NormalBGColor;

    public Image HealthImage;
    public Image BGImage;

    public InvaderController AttachedInvaderController;

    float UpdateRate = 0.5f;

    public void Initialize(InvaderController AttachedInvaderController, float UpdateRate)
    {
        this.UpdateRate = UpdateRate;
        this.AttachedInvaderController = AttachedInvaderController;
        gameObject.SetActive(true);
        AttachedInvaderController.OnDeath += OnCharacterDeath; // TODO: Remove & use update
        BGImage.color = NormalBGColor;
    }

    private void OnDisable()
    {
        AttachedInvaderController.OnDeath -= OnCharacterDeath;
    }

    float NextUpdate = 0;
    private void Update()
    {
        if (Time.time > NextUpdate && AttachedInvaderController != null)
        {
            if (AttachedInvaderController.Health >= 0)
            {
                if (BGImage.color != NormalBGColor && AttachedInvaderController.Health > 0) BGImage.color = NormalBGColor;
                float pct = GameUtils.GetPercentage(AttachedInvaderController.Health, AttachedInvaderController.MaxHealth, 1);
                HealthImage.fillAmount = pct;
            }
            NextUpdate = Time.time + UpdateRate;
        }
    }

    void OnCharacterDeath()
    {
        BGImage.color = DeadBGColor;
        HealthImage.fillAmount = 0;
    }
}
