using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using DarkTonic.MasterAudio;
using System.Linq;

public class FinishedGamePopup : MonoBehaviour
{
    public Image BG, AlienIcon, BriefcaseIcon;
    public List<Image> HorizontalBars = new List<Image>();
    public Button PlayAgainBtn, SettingsBtn, MainMenuBtn;
    public TextMeshProUGUI CharsUnlockedTxt;
    public TextMeshProUGUI BestScoreTxt, ScoreTxt, SavedAliensTxt, StolenBriefcasesTxt, GameOverText, BestScoreLabel, ScoreLabel, NewBestScoreLabel;

    [Header("Stars")]
    public Image StarsBG;
    public GameObject[] stars;
    public Transform pos_01;
    public Transform pos_02;
    public Transform pos_03;

    IEnumerator AnimCoroutine;

    private void Start()
    {
        MainMenuBtn.onClick.AddListener(() => SceneManager.Instance.LoadScene(SceneLoadType.MAIN_MENU));
        PlayAgainBtn.onClick.AddListener(() => SceneManager.Instance.LoadScene(SceneLoadType.GAME));
        SettingsBtn.onClick.AddListener(() => GlobalUIManager.Instance.ShowSettingsWindow());
    }

    [ContextMenu("Test animation show")]
    public void Show(bool IsBestScore, int oldBestScore, EndGameReason reason)
    {
        Debug.LogError("FinishedGamePopup Show");
        ResetComponents();

        GameState GS = GameSceneManager.Instance.GameState;

        int StarsToActivate = 0;

        if (IsBestScore)
        {
            StarsToActivate = 3;
        }
        else
        {
            if (oldBestScore != 0)
            {
                float pctToBest = (GS.Score * 100) / oldBestScore;

                if (pctToBest >= 33f) StarsToActivate++;
                if (pctToBest >= 66f) StarsToActivate++;
                if (pctToBest >= 99f) StarsToActivate++;
            }
        }

        if (reason == EndGameReason.OUT_OF_TIME)
        {
            GameOverText.SetText(GameUtils.GetTranslatedText("TimeIsUp"));
        }
        else
        {
            GameOverText.SetText("GAME OVER");
        }

        BestScoreTxt.SetText(oldBestScore.ToString());
        ScoreTxt.SetText(GS.Score.ToString());
        SavedAliensTxt.SetText(GS.CurrentSavedAliens.ToString());
        StolenBriefcasesTxt.SetText(GS.CurrentStolenBriefcases.ToString());

        gameObject.SetActive(true);
      //  Debug.LogError("pass 2");
        AnimCoroutine = Animate(IsBestScore, StarsToActivate);
        StartCoroutine(AnimCoroutine);
    }

    void ResetComponents()
    {
        if (AnimCoroutine != null) StopCoroutine(AnimCoroutine);
        gameObject.SetActive(false);

        BG.color = new Color(BG.color.r, BG.color.g, BG.color.b, 0);
        BriefcaseIcon.color = new Color(BriefcaseIcon.color.r, BriefcaseIcon.color.g, BriefcaseIcon.color.b, 0);
        AlienIcon.color = new Color(AlienIcon.color.r, AlienIcon.color.g, AlienIcon.color.b, 0);
        StarsBG.color = new Color(StarsBG.color.r, StarsBG.color.g, StarsBG.color.b, 0);

        BestScoreTxt.SetText("0");
        ScoreTxt.SetText("0");
        SavedAliensTxt.SetText("0");
        StolenBriefcasesTxt.SetText("0");

        CharsUnlockedTxt.color = new Color(CharsUnlockedTxt.color.r, CharsUnlockedTxt.color.g, CharsUnlockedTxt.color.b, 0);
        GameOverText.color = new Color(GameOverText.color.r, GameOverText.color.g, GameOverText.color.b, 0);
        NewBestScoreLabel.color = new Color(NewBestScoreLabel.color.r, NewBestScoreLabel.color.g, NewBestScoreLabel.color.b, 0);
        BestScoreLabel.color = new Color(BestScoreLabel.color.r, BestScoreLabel.color.g, BestScoreLabel.color.b, 0);
        ScoreLabel.color = new Color(ScoreLabel.color.r, ScoreLabel.color.g, ScoreLabel.color.b, 0);
        ScoreTxt.color = new Color(ScoreTxt.color.r, ScoreTxt.color.g, ScoreTxt.color.b, 0);
        BestScoreTxt.color = new Color(BestScoreTxt.color.r, BestScoreTxt.color.g, BestScoreTxt.color.b, 0);
        SavedAliensTxt.color = new Color(SavedAliensTxt.color.r, SavedAliensTxt.color.g, SavedAliensTxt.color.b, 0);
        StolenBriefcasesTxt.color = new Color(StolenBriefcasesTxt.color.r, StolenBriefcasesTxt.color.g, StolenBriefcasesTxt.color.b, 0);

        stars[0].transform.localScale = Vector3.zero;
        stars[1].transform.localScale = Vector3.zero;
        stars[2].transform.localScale = Vector3.zero;
        PlayAgainBtn.transform.localScale = Vector3.zero;
        SettingsBtn.transform.localScale = Vector3.zero;
        MainMenuBtn.transform.localScale = Vector3.zero;

        foreach(Image b in HorizontalBars)
        {
            b.color = new Color(b.color.r, b.color.g, b.color.b, 0);
        }
    }

    IEnumerator Animate(bool IsBestScore, int StarsToActivate)
    {
        MasterAudio.PlaySound("GameOver");
        //Debug.LogError("Animate coroutine");
        BG.DOFade(0.85f, 1);

        yield return new WaitForSeconds(0.75f);

        ADManager.Instance.TryShowInterstitial();

        StarsBG.DOFade(1, 0.15f);
        StartCoroutine(AnimateStars(StarsToActivate));

        GameOverText.DOFade(1, 0.5f);
        GameOverText.transform.DOPunchScale(new Vector3(0.85f, 0.85f, 0.85f), 0.85f);

        yield return new WaitForSeconds(0.5f);

        //    public TextMeshProUGUI BestScoreTxt, ScoreTxt, SavedAliensTxt, StolenBriefcasesTxt, GameOverText;

        AlienIcon.DOFade(1, 0.35f);
        BriefcaseIcon.DOFade(1, 0.35f);

        yield return new WaitForSeconds(0.15f);

        SavedAliensTxt.DOFade(1, 0.35f);
        StolenBriefcasesTxt.DOFade(1, 0.35f);

        yield return new WaitForSeconds(0.15f);

        BestScoreLabel.DOFade(1, 0.25f);
        yield return new WaitForSeconds(0.1f);
        ScoreLabel.DOFade(1, 0.35f);
        yield return new WaitForSeconds(0.1f);
        BestScoreTxt.DOFade(1, 0.35f);
        yield return new WaitForSeconds(0.1f);
        ScoreTxt.DOFade(1, 0.35f);

        if (IsBestScore)
        {
            MasterAudio.PlaySound("NewBestScore");
            NewBestScoreLabel.DOFade(1, 0.5f);
            NewBestScoreLabel.transform.DOPunchScale(new Vector3(0.85f, 0.85f, 0.85f), 0.85f);
        }

        List<int> LockedSkinsAfter = new List<int>();
        LockedSkinsAfter.AddRange(SkinManager.Instance.SkinPrefabs.Where(s => GameUtils.IsSkinLocked(s)).Select(s => s.ID));
        if (LockedSkinsAfter.Count < GameSceneManager.Instance.LockedSkinsBefore.Count)
        {
            CharsUnlockedTxt.DOFade(1, 0.35f);
        }

        yield return new WaitForSeconds(0.3f);

        PlayAgainBtn.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InBounce);
        yield return new WaitForSeconds(0.2f);
        SettingsBtn.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InBounce);
        yield return new WaitForSeconds(0.2f);
        MainMenuBtn.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InBounce);
        yield return new WaitForSeconds(0.2f);

        foreach (Image b in HorizontalBars)
        {
            b.DOFade(0.5f, 1f);
        }
    }

    IEnumerator AnimateStars(int starsToActivate)
    {
        if (starsToActivate >= 1)
        {
            stars[0].transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
            MasterAudio.PlaySound("MagicPop");
            //GameObject fx_01 = (GameObject)Instantiate(Resources.Load("FX_Complete_star_1"), new Vector3(pos_01.position.x, pos_01.position.y, pos_01.position.z), Quaternion.identity);

            if (starsToActivate >= 2)
            {
                yield return new WaitForSeconds(0.35f);
                MasterAudio.PlaySound("MagicPop");
                stars[1].transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);

                if (starsToActivate >= 3)
                {
                    yield return new WaitForSeconds(0.35f);
                    MasterAudio.PlaySound("MagicPop");

                    stars[2].transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
                }
            }
        }
        yield return null;
      //  GameObject fx_02 = (GameObject)Instantiate(Resources.Load("FX_Complete_star_2"), new Vector3(pos_02.position.x, pos_02.position.y, pos_02.position.z), Quaternion.identity);
        //GameObject fx_03 = (GameObject)Instantiate(Resources.Load("FX_Complete_star_3"), new Vector3(pos_03.position.x, pos_03.position.y, pos_03.position.z), Quaternion.identity);
        //yield return new WaitForSeconds(1f);

        /*Destroy(fx_01.gameObject);
        Destroy(fx_02.gameObject);
        Destroy(fx_03.gameObject);*/
    }
}
