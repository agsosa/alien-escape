using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

// Main menu UI skin selector

public class SkinSelector : MonoBehaviour
{
    public Button UnlockByVideoRewardBtn;
    public Transform SkinContainersParent;
    public Button LeftNavigator;
    public Button RightNavigator;
    public TextMeshProUGUI SkinExtraSecondsCount;
    public TextMeshProUGUI SkinHealsCount;
    public TextMeshProUGUI SkinReviveCount;
    public TextMeshProUGUI SkinPowerCount;
    public GameObject SkinLockedPanel;
    public GameObject UnlockAllSkins;
    public TextMeshProUGUI SkinUnlockRequeriments;
    public GameObject SkinUnlockedPreviousFirst;

    public RuntimeAnimatorController AnimatorController;

    public int ViewingSkinID = 0;
    public int SelectedSkinID = 0;

    public List<Skin> LoadedSkins = new List<Skin>();

    private void Start()
    {
        LeftNavigator.onClick.AddListener(() => LeftNavigatorClickHandler());
        RightNavigator.onClick.AddListener(() => RightNavigatorClickHandler());
        UnlockByVideoRewardBtn.onClick.AddListener(() => UnlockByVideoReward());

        // CHECK FOR UNLOCKED SKINS

        // Instantiate skin prefabs and set skincontainers
        // List is sorted by ID
        foreach(Skin s in SkinManager.Instance.SkinPrefabs)
        {
            Skin o = Instantiate(s, SkinContainersParent);
            o.gameObject.transform.localPosition = Vector3.zero;
            o.gameObject.transform.localRotation = Quaternion.identity;
            o.SkinMeshRenderer.enabled = false;
            Animator anim = o.gameObject.AddComponent<Animator>();
            anim.runtimeAnimatorController = AnimatorController;
            anim.avatar = o.AnimatorAvatar;
            anim.applyRootMotion = false;

            LoadedSkins.Add(o);
        }

        SelectSkin(DataManager.Instance.PlayerData.UsingSkinID);
    }

    public void SelectSkin(int id)
    {
   //     Debug.LogError("SelectingSkin id = " + id + " viewing id = " + ViewingSkinID);
        if (id != ViewingSkinID)
        {
            Skin lastSelected = LoadedSkins.Find(s => s.ID == ViewingSkinID);
            if (lastSelected != null) lastSelected.SkinMeshRenderer.enabled = false;
        }

        ViewingSkinID = id;
        Skin CurrSelected = LoadedSkins[ViewingSkinID];
        CurrSelected.SkinMeshRenderer.enabled = true;

        SkinExtraSecondsCount.SetText(string.Format("+{0}s", CurrSelected.ExtraSeconds.ToString()));
        SkinHealsCount.SetText(CurrSelected.Heals.ToString());
        SkinReviveCount.SetText(CurrSelected.Revives.ToString());
        SkinPowerCount.SetText(CurrSelected.Power.ToString());

        if (SkinUnlockedPreviousFirst.activeSelf) SkinUnlockedPreviousFirst.SetActive(false);
        if (SkinLockedPanel.activeSelf) SkinLockedPanel.SetActive(false);

        bool allUnlocked = DataManager.Instance.PlayerData.AllCharsUnlocked.Value == 10000;

        RefreshNavigators();

        UnlockAllSkins.SetActive(!IsSkinUnlocked(CurrSelected) && !allUnlocked);

        if (IsSkinUnlocked(CurrSelected) || allUnlocked)
        {
            SkinLockedPanel.SetActive(false);
            SkinUnlockedPreviousFirst.SetActive(false);
            SelectedSkinID = id;
            DataManager.Instance.PlayerData.UsingSkinID = id;
            DataManager.Instance.SaveData();
        }
        else
        {
            if (ViewingSkinID - 1 >= 0 && !IsSkinUnlocked(LoadedSkins[ViewingSkinID-1]))
            {
                SkinLockedPanel.SetActive(false);
                SkinUnlockedPreviousFirst.SetActive(true);
            }
            else
            {
                SkinUnlockRequeriments.SetText(Lean.Localization.LeanLocalization.GetTranslationText("CharUnlockRequeriment").Replace("{0}", CurrSelected.RequiredSavedAliens.ToString()).Replace("{1}", CurrSelected.RequiredStolenBriefcases.ToString()));
                SkinLockedPanel.SetActive(true);
                UnlockAllSkins.SetActive(true);
            }
        }
    }

    void LeftNavigatorClickHandler()
    {
        SelectSkin(Mathf.Max(0, ViewingSkinID - 1));
    }

    void RightNavigatorClickHandler()
    {
        SelectSkin(Mathf.Min(ViewingSkinID + 1, LoadedSkins.Count - 1));
    }

    void RefreshNavigators()
    {
        RightNavigator.gameObject.SetActive(HasNextSkin());
        LeftNavigator.gameObject.SetActive(HasPreviousSkin());
    }

    bool HasNextSkin()
    {
        return !(ViewingSkinID + 1 > LoadedSkins.Count - 1);
    }

    bool HasPreviousSkin()
    {
        return (ViewingSkinID - 1 >= 0);
    }

    bool IsSkinUnlocked(Skin skin)
    {
        return (DataManager.Instance.PlayerData.TotalSavedAliens >= skin.RequiredSavedAliens && DataManager.Instance.PlayerData.TotalBriefcases >= skin.RequiredStolenBriefcases) || UnlockedByVideoReward(skin);
    }

    bool UnlockedByVideoReward(Skin skin)
    {
        return DataManager.Instance.PlayerData.PrematureUnlockedCharacters.Contains(skin.ID);
    }

    public void RefreshSkinsUnlockStatus(bool hideUnlockAllSkinsUI = true)
    {
        SelectSkin(ViewingSkinID);
        SkinLockedPanel.SetActive(false);
        SkinUnlockedPreviousFirst.SetActive(false);
        if (hideUnlockAllSkinsUI) UnlockAllSkins.SetActive(false);
    }

    void UnlockByVideoReward()
    {
        ADManager.Instance.TryShowVideoReward(() =>
        {
            DataManager.Instance.PlayerData.PrematureUnlockedCharacters.Add(ViewingSkinID);
            DataManager.Instance.SaveData();
            RefreshSkinsUnlockStatus(false);
            GlobalUIManager.Instance.ShowConfirmationPopup(GameUtils.GetTranslatedText("CharUnlockedByVideo"));
        });
    }
}
