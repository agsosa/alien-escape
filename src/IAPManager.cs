using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviourSingleton<IAPManager>
{
    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    /*private IStoreController controller;
    private IExtensionProvider extensions;

    /// <summary>
    /// Called when Unity IAP is ready to make purchases.
    /// </summary>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.LogError("IAPManager: Initialized");
        this.controller = controller;
        this.extensions = extensions;
    }

    /// <summary>
    /// Called when Unity IAP encounters an unrecoverable initialization error.
    ///
    /// Note that this will not be called if Internet is unavailable; Unity IAP
    /// will attempt initialization until it becomes available.
    /// </summary>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError("IAPManager: Initialization failed due to " + error.ToString());
    }

    /// <summary>
    /// Called when a purchase completes.
    ///
    /// May be called at any time after OnInitialized().
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        Debug.LogError("IAPManager: ProcessPurchase product = " + e.purchasedProduct.metadata.localizedTitle);
        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// Called when a purchase fails.
    /// </summary>
    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        // TODO: Remove all duplicated listeners
        Debug.LogError("IAPManager: OnPurchaseFailed product = " + i.metadata.localizedTitle + " reason = " + p.ToString());
    }*/

    public void PurchaseCompleted(Product product)
    {
        Debug.LogError("IAPManager: ProcessPurchase product = " + product.definition.id);
        if (product.definition.id == "games.battlemark.alienescape3d.unlockallcharacters")
        {
            Debug.LogError("Unlocking all characters");
            if (DataManager.Instance.PlayerData.AllCharsUnlocked.Value != 10000)
            {
                if (MainMenuSceneManager.Instance != null)
                {
                    MainMenuSceneManager.Instance.SkinSelector.RefreshSkinsUnlockStatus();
                    GlobalUIManager.Instance.ShowConfirmationPopup(GameUtils.GetTranslatedText("AllSkinsUnlocked"));
                }
                DataManager.Instance.PlayerData.AllCharsUnlocked.Value = 10000;
                DataManager.Instance.SaveData();
            }
        }

        if (product.definition.id == "games.battlemark.alienescape3d.removeads")
        {
            Debug.LogError("Disabling Ads");
            if (DataManager.Instance.PlayerData.NoAds.Value != 10000)
            {
                if (MainMenuSceneManager.Instance != null)
                {
                    GlobalUIManager.Instance.ShowConfirmationPopup(GameUtils.GetTranslatedText("AdsRemoved"));
                }
                DataManager.Instance.PlayerData.NoAds.Value = 10000;
                DataManager.Instance.SaveData();
            }
            ADManager.Instance.DestroyBanner();
        }
    }

    public void PurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.LogError("IAPManager: Purchase failed of product = " + product.definition.id + " due to " + reason.ToString());
        if (reason != PurchaseFailureReason.UserCancelled)
        {
            GlobalUIManager.Instance.ShowErrorPopup(GameUtils.GetTranslatedText("ErrorOccuredWhilePurchasing"), 500 + (int)reason);
        }
    }
}
