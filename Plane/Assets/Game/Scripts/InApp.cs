using UnityEngine;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Soomla.Store
{

    public class InApp : MonoBehaviour
    {
        private string msg = "";

        private bool SUPPORTED = true;

        void Start()
        {
            StoreEvents.OnMarketPurchase += BuyPoints;
            if (SoomlaStore.Initialized) return;
            StoreEvents.OnSoomlaStoreInitialized += onSoomlaStoreInitialized;
            SoomlaStore.Initialize(new PurshConfig());
            StoreEvents.OnUnexpectedErrorInStore += onError;
            StoreEvents.OnBillingSupported += onBillingSupported;
            StoreEvents.OnBillingNotSupported += onBillingNOTSupported;
        }

        void OnDestroy()
        {
            StoreEvents.OnMarketPurchase -= BuyPoints;
        }

#if DEBUG_MODE
        void OnGUI()
        {
            GUI.Label(new Rect(0, 750, 475, 50), msg);
        }
#endif

        public void ClickBuy()
        {
            if (SUPPORTED)
                StoreInventory.BuyItem(PurshConfig.POINTS.ItemId, Guid.NewGuid().ToString());
            else AndroidNativeUtils.ShowMsg("Sorry, billing is't supported on your device. Try update the GooglePlay.");
        }

        private void onError(string error)
        {
            msg = "error = " + error;
            SUPPORTED = false;
            SoomlaStore.StopIabServiceInBg();
        }

        private void onSoomlaStoreInitialized()
        {
            msg = "Init OK";
        }

        private void onBillingSupported()
        {
            msg += "; Billing OK";
        }

        private void onBillingNOTSupported()
        {
            msg += "; Billing FAILED";
            SUPPORTED = false;
            SoomlaStore.StopIabServiceInBg();
        }

        private void BuyPoints(PurchasableVirtualItem pvi, string payload, Dictionary<string, string> extra)
        {
            if (Verify(extra["originalJson"], extra["signature"]))
            {
                msg = "Buy OK";
                GameManager.instance.BuyPoints();
            }
            else AndroidNativeUtils.ShowMsg("Sorry , purchase failed, refer to the developer");
        }

        private bool Verify(string purchaseJson, string base64Signature)
        {
            using (var provider = new RSACryptoServiceProvider())
            {
                try
                {
                    provider.FromXmlString("<RSAKeyValue><Modulus>kdw5gSQCDOn49jLwSMzlDaciKuuRLuv1g+0guTaXA7AL7YzTH+5Dz3xaEJYLGRy4rAmUq1Mroure30jh+vt2bxyFwBuG+bipv43+9HEdG569T6LgrzAEvxhf0HP11pzaFfYUl4Ct+Nh1wVQnwoPAXO4x+bcf3vLx3le6+UEIWUN4mx1GH4+3ZfFE8TZ1zxUnSwGFMnaVDHwja1nCX2DIE9fvQlcrvDWbZETBtuir8XzCkF78eWy+F7SUtpeLCx99+Wkcwy5JJuzqL1e3S30va5/01/LVLEy6kj6dgtNWhYkyBFFwc6+XAHFvNEXXoHRv+/wWfNX68/Lp7Dz0FoC5zw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");

                    var signature = Convert.FromBase64String(base64Signature);
                    var sha = new SHA1Managed();
                    var data = System.Text.Encoding.UTF8.GetBytes(purchaseJson);

                    return provider.VerifyData(data, sha, signature);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }

                return false;
            }
        }

    }

}
