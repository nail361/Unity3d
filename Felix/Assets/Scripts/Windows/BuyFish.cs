using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

public class BuyFish : BaseWindow {

    [SerializeField]
    private Text debug_label;

    [SerializeField]
    private Text[] buy_texts;
    [SerializeField]
    private Button showAds_btn;
    private Text showAds_label;

    private string zoneId = null;

    private FishPanel fishPanel;

    private bool SUPPORTED = true;

    void Start()
    {
        for (int i = 0; i < buy_texts.Length; i++)
        {
            buy_texts[i].text = LanguageManager.GetText("BuyFish") + " " + GameManager.instance.FISH_COST[i] + "$";
        }

        fishPanel = GameObject.Find("FishPanel").GetComponent<FishPanel>();

        showAds_label = showAds_btn.GetComponentInChildren<Text>();
        showAds_label.text = LanguageManager.GetText("ShowAds");

        if (PlayerPrefs.HasKey("ads_time")){

            double time = Convert.ToDouble(PlayerPrefs.GetString("ads_time"));

            time = (DateTime.Now - new DateTime(1970, 1, 1)).TotalMinutes - time;

            if (time > 30)
            {
                PlayerPrefs.DeleteKey("ads_time");
            }
            else
            {
                showAds_btn.enabled = false;
                showAds_label.text = LanguageManager.GetText("AdsWait");
            }
        }

        _animation = GetComponent<Animation>();
        anim_name = "BuyWindow";

        transform.SetParent(GameObject.Find("HUD").transform, false);
        transform.SetAsLastSibling();
        DeactivateCamera();

        if (!Soomla.Store.SoomlaStore.Initialized)
        {
            Soomla.Store.StoreEvents.OnMarketPurchase += OnBuy;
            Soomla.Store.StoreEvents.OnSoomlaStoreInitialized += onSoomlaStoreInitialized;

            Soomla.Store.StoreEvents.OnUnexpectedStoreError += onError;
            Soomla.Store.StoreEvents.OnBillingSupported += onBillingSupported;
            Soomla.Store.StoreEvents.OnBillingNotSupported += onBillingNOTSupported;
            
            Soomla.Store.SoomlaStore.Initialize( new SoomlaAssets() );
        }
    }

    public void BuyClick(int value)
    {
        if (SUPPORTED)
            switch (value)
            {
                case 1: Soomla.Store.StoreInventory.BuyItem(SoomlaAssets.TESTID_ITEM_ID, Guid.NewGuid().ToString()); break;
                case 2: Soomla.Store.StoreInventory.BuyItem(SoomlaAssets.FISH_2_ITEM_ID, Guid.NewGuid().ToString() ); break;
                case 3: Soomla.Store.StoreInventory.BuyItem(SoomlaAssets.FISH_3_ITEM_ID, Guid.NewGuid().ToString()); break;
                case 4: Soomla.Store.StoreInventory.BuyItem(SoomlaAssets.FISH_4_ITEM_ID, Guid.NewGuid().ToString()); break;
            }
        else AndroidNativeUtils.ShowMsg("Sorry, billing is't supported on your device. Try update the PlayMarket.");

    }

    private void onError(int error)
    {
        debug_label.text = "error = " + error;
        SUPPORTED = false;
        Soomla.Store.SoomlaStore.StopIabServiceInBg();
    }

    private void onSoomlaStoreInitialized()
    {
        debug_label.text = "Init OK";
    }

    private void onBillingSupported()
    {
        debug_label.text += "; Billing OK";
    }

    private void onBillingNOTSupported()
    {
        debug_label.text += "; Billing FAILED";
        SUPPORTED = false;
        Soomla.Store.SoomlaStore.StopIabServiceInBg();
    }

    private void OnBuy(Soomla.Store.PurchasableVirtualItem pvi, string payload, Dictionary<string, string> extra)
    {
        if (!Verify(extra["originalJson"], extra["signature"])) {

            debug_label.text = "Buy OK";
            debug_label.text = extra["originalJson"];
            debug_label.text = extra["signature"];
            debug_label.text = pvi.ItemId;
            return;
        }

        debug_label.text = "Buy OK";

        uint count = 0;
        switch (pvi.ItemId)
        {
            case SoomlaAssets.TESTID_ITEM_ID: count = 1; break;
            case SoomlaAssets.SOSIGES_X5_ITEM_ID: count = 50; break;
            case SoomlaAssets.FISH_2_ITEM_ID: count = 150; break;
            case SoomlaAssets.FISH_3_ITEM_ID: count = 400; break;
            case SoomlaAssets.FISH_4_ITEM_ID: count = 1000; break;
        }

        fishPanel.AddFish( count );

        pvi.ResetBalance(0);
    }

    public void AdsClick()
    {
        if (InternetChecker.isConnected && InternetChecker.ping.isDone)
        {
            if (Advertisement.IsReady(zoneId))
            {
                var options = new ShowOptions { resultCallback = HandleShowResult };
                Advertisement.Show(zoneId, options);
            }
            else
            {
                showAds_label.text = LanguageManager.GetText("AdsWait");
            }
        }
        else
        {
            AndroidNativeUtils.ShowMsg(LanguageManager.GetText("NoWeb"));
        }
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                showAds_btn.enabled = false;
                showAds_label.text = LanguageManager.GetText("AdsWait");
                PlayerPrefs.SetString("ads_time", (DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds.ToString() );
                fishPanel.AddFish(15);
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                showAds_label.text = LanguageManager.GetText("AdsWait");
                break;
        }
    }

    private bool Verify(string purchaseJson, string base64Signature)
    {
        using (var provider = new RSACryptoServiceProvider())
        {
            try
            {
                provider.FromXmlString("<RSAKeyValue><Modulus>h8uaZmnP4+f3sPvZrh4X9XxQJxi5mlBfszU4b//2mLw17ZDtVU4keu5A5nZwZnURSHF0FlE5vFej6VfOhwuD3LYh0pcGKqK7tPStxDGaQQ2OIWwXOQPvz0k+P8VjLrClAaCxtL9jJPArCJ69i8sCYW7YzLPbAoi3ItDM0tQicE46WSYJWgZIVMPniRikVb4LtGs1NADEgMDnPt/0MaYDvvNoYombNkGdM0HIWepFlPFN1Of3vJ6crdBBQuW+reU4AfiJccrHyyc57D8ButwZbrW2MsuyeMNRpCtvy184kmTQoyqZQXnK38fhB+36vWL7JXwt9oqE2ZIOgQIF0BIEpw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");

                var signature = Convert.FromBase64String(base64Signature);
                var sha = new SHA1Managed();
                var data = System.Text.Encoding.UTF8.GetBytes(purchaseJson);

                return provider.VerifyData(data, sha, signature);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e);
            }

            return false;
        }
    }

}