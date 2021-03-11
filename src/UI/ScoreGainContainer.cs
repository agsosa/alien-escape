using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ScoreGainContainer : MonoBehaviour
{
    public TextMeshProUGUI AlienSavedLabel, SoldierKilledLabel, StolenBriefcaseLabel, DeadMateLabel;
    public TextMeshProUGUI ScoreGainTxt;
    public TextMeshProUGUI TimeGainTxt;

    public float AnimLocalMoveY = 18f;
    public float AnimHoldSeconds = 1.5f;

    IEnumerator AnimateCoroutine;

    public bool IsAvailable = true;

    private void Start()
    {
        ResetComponents();
        gameObject.SetActive(false);
    }

    void ResetComponents()
    {
        // Reset scale
        AlienSavedLabel.transform.DOScale(0, 0);
        SoldierKilledLabel.transform.DOScale(0, 0);
        StolenBriefcaseLabel.transform.DOScale(0, 0);
        DeadMateLabel.transform.DOScale(0, 0);
        ScoreGainTxt.transform.DOScale(0, 0);
        TimeGainTxt.transform.DOScale(0, 0);

        // Reset Alpha
        AlienSavedLabel.DOFade(1, 0);
        SoldierKilledLabel.DOFade(1, 0);
        StolenBriefcaseLabel.DOFade(1, 0);
        DeadMateLabel.DOFade(1, 0);
        ScoreGainTxt.DOFade(1, 0);
        TimeGainTxt.DOFade(1, 0);

        // Reset position
        AlienSavedLabel.rectTransform.DOLocalMoveY(0, 0);
        SoldierKilledLabel.rectTransform.DOLocalMoveY(0, 0);
        StolenBriefcaseLabel.rectTransform.DOLocalMoveY(0, 0);
        DeadMateLabel.rectTransform.DOLocalMoveY(0, 0);
        ScoreGainTxt.rectTransform.DOLocalMoveY(0, 0);
        TimeGainTxt.rectTransform.DOLocalMoveY(0, 0);
    }

    [ContextMenu("Test Animation")]
    public void Test() // TODO: Delete me
    {
        ScoreGain obj = new ScoreGain();
        obj.Type = ScoreGainType.ALIEN_SAVED;
        obj.Time = 60f;
        obj.Score = 150;
        Show(obj);
    }

    public void Show(ScoreGain obj)
    {
        if (!IsAvailable) return;

        IsAvailable = false;

        ResetComponents();

        ScoreGainTxt.SetText(string.Format("+{0}", obj.Score));
        TimeGainTxt.SetText(string.Format("+{0}", GameUtils.GetFormattedTime(obj.Time)));

        TextMeshProUGUI LabelUsed = null;

        switch(obj.Type)
        {
            case ScoreGainType.ALIEN_SAVED:
                LabelUsed = AlienSavedLabel;
                break;
            case ScoreGainType.DEAD_MATE:
                LabelUsed = DeadMateLabel;
                break;
            case ScoreGainType.SOLDIER_KILL:
                LabelUsed = SoldierKilledLabel;
                break;
            case ScoreGainType.STOLEN_BRIEFCASE:
                LabelUsed = StolenBriefcaseLabel;
                break;
            default:
                Debug.LogError("Fatal Error: ScoreGainContainer received an invalid ScoreGainType!!");
                break;
        }

        gameObject.SetActive(true);
        AnimateCoroutine = Animate(LabelUsed);
        StartCoroutine(AnimateCoroutine);
    }

    IEnumerator Animate(TextMeshProUGUI LabelUsed)
    {
        if (LabelUsed != null)
        {
            LabelUsed.transform.DOScale(1, 0.35f);
            ScoreGainTxt.transform.DOScale(1, 0.35f);
            TimeGainTxt.transform.DOScale(1, 0.35f);

            // Wait to disappear
            yield return new WaitForSeconds(AnimHoldSeconds);

            LabelUsed.rectTransform.DOLocalMoveY(AnimLocalMoveY, 0.75f);
            ScoreGainTxt.rectTransform.DOLocalMoveY(AnimLocalMoveY, 0.75f);
            TimeGainTxt.rectTransform.DOLocalMoveY(AnimLocalMoveY, 0.75f);

            LabelUsed.DOFade(0, 0.75f);
            ScoreGainTxt.DOFade(0, 0.75f);
            TimeGainTxt.DOFade(0, 0.75f);

            yield return new WaitForSeconds(0.85f);
        }

        IsAvailable = true;
    }
}
