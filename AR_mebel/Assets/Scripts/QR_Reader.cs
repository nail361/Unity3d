using UnityEngine;
using System;
using System.Collections;

using Vuforia;

using ZXing;
using ZXing.QrCode;
using ZXing.Common;

public class QR_Reader: MonoBehaviour
{
    private bool cameraInitialized;

    private BarcodeReader barCodeReader;

    void Start()
    {
        barCodeReader = new BarcodeReader();
        StartCoroutine("InitializeCamera");
    }

    private IEnumerator InitializeCamera()
    {
        // Waiting a little seem to avoid the Vuforia crashes.
        yield return new WaitForSeconds(1.25f);

        var isFrameFormatSet = CameraDevice.Instance.SetFrameFormat(Image.PIXEL_FORMAT.RGB888, true);
        Debug.Log(String.Format("FormatSet : {0}", isFrameFormatSet));

        // Force autofocus.
        var isAutoFocus = CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        if (!isAutoFocus)
        {
            CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_NORMAL);
        }
        Debug.Log(String.Format("AutoFocus : {0}", isAutoFocus));
        cameraInitialized = true;
        StopCoroutine("InitializeCamera");
    }

    private void Update()
    {
        if (cameraInitialized)
        {
            try
            {
                var cameraFeed = CameraDevice.Instance.GetCameraImage(Image.PIXEL_FORMAT.RGB888);
                if (cameraFeed == null)
                {
                    return;
                }
                
                var data = barCodeReader.Decode(cameraFeed.Pixels, cameraFeed.BufferWidth, cameraFeed.BufferHeight, RGBLuminanceSource.BitmapFormat.RGB24);
                if (data != null)
                {
                    // QRCode detected.
                    cameraInitialized = false;
                    Debug.Log(data.Text);
                    LoadModel();
                }
                else
                {
                    Debug.Log("No QR code detected !");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }

    private void LoadModel()
    {
        StartCoroutine("DownloadAssetBundle");
    }

    public IEnumerator DownloadAssetBundle()
    {
        while (!Caching.ready)
        {
            OnBundleLoaded();
            yield return null;
        }

        using (WWW www = WWW.LoadFromCacheOrDownload("file://" + Application.dataPath + "/AssetBundles/wordsbank", 0))
        {
            yield return www;
            if (www.error != null) throw new Exception("WWW download:" + www.error + " url:" + www.url);

            AssetBundle assetBundle = www.assetBundle;

            AssetBundleRequest request = assetBundle.LoadAssetAsync("model_01.pref", typeof(GameObject));
            yield return request;
            GameObject model = request.asset as GameObject;
            OnBundleLoaded();
            assetBundle.Unload(false);
        }
    }

    private void OnBundleLoaded()
    {
        
    }

}