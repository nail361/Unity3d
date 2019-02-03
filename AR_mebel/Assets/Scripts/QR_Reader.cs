using System.Threading;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using ZXing;
using ZXing.QrCode;

public class QR_Reader : MonoBehaviour
{
    [SerializeField]
    private Text warningTextField;
    [SerializeField]
    public RawImage rawimage;

    //public Texture2D encoded;

    private WebCamTexture camTexture;
    private Thread qrThread;

    private Color32[] camPixels;
    private int W, H;

    private bool isQuit;
    private bool canCheck = true;

    private string QR_result = "";

    Result result;

    void OnEnable()
    {
        if (camTexture != null)
        {
            camTexture.Play();
            W = camTexture.width;
            H = camTexture.height;
        }
    }

    void OnDisable()
    {
        if (camTexture != null)
        {
            camTexture.Pause();
        }
    }

    void OnDestroy()
    {
        qrThread.Abort();
        camTexture.Stop();
    }

    void OnApplicationQuit()
    {
        isQuit = true;
    }

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        //encoded = new Texture2D(256, 256);
        camTexture = new WebCamTexture(Screen.width, Screen.height, 25);
        rawimage.texture = camTexture;

#if UNITY_IOS
        //if (camTexture.videoVerticallyMirrored)
        //{
            Quaternion rotation = Quaternion.Euler(0, 0, 180);
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(Vector3.zero, rotation, new Vector3(-1f, 1f, 1));
            rawimage.material.doubleSidedGI = true;
            rawimage.material.SetMatrix("_Rotation", rotationMatrix);
        //}
#endif

        rawimage.material.mainTexture = camTexture;
        OnEnable();

        qrThread = new Thread(DecodeQR);
        qrThread.Start();
    }

    void Update()
    {
#if UNITY_ANDROID
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
                return;
            }
        }
#endif

        if (W == 16)
        {
            W = camTexture.width;
            H = camTexture.height;
        }
        else
            if (camPixels == null)
            {
                camPixels = camTexture.GetPixels32();
            }

        if (canCheck && QR_result.Length > 0)
        {
            //var color32 = Encode(textForEncoding, encoded.width, encoded.height);
            //encoded.SetPixels32(color32);
            //encoded.Apply();
            
            if (CheckUrl())
            {
                rawimage.texture = null;
                canCheck = false;
                isQuit = true;
                gameObject.GetComponent<ModelLoader>().LoadModel(QR_result);
            }
        }
    }

    private bool CheckUrl()
    {
        bool correct = false;

        if (QR_result.Length > 0)
        {
            //Проверка урла на наш сервер.

            if (QR_result.IndexOf("prodesign") != -1) {
                warningTextField.text = "";
                correct = true;
            }
            else
            {
                warningTextField.text = "QR код не принадлежит нашей системе!";
                canCheck = false;
                QR_result = "";
                StartCoroutine(HideWarning());
            }
        }

        return correct;
    }

    void DecodeQR()
    {
        var barcodeReader = new BarcodeReader();

#if UNITY_IOS
        barcodeReader.AutoRotate = false;
        barcodeReader.TryInverted = false;
        barcodeReader.Options.TryHarder = true;
#else
        barcodeReader.AutoRotate = false;
        barcodeReader.TryInverted = false;
        barcodeReader.Options.TryHarder = false;
#endif

        while (true)
        {
            if (isQuit)
                break;

            try
            {
                // decode the current frame
                if (camPixels != null)
                {
                    print("camPixels :" + camPixels.Length);
                    result = barcodeReader.Decode(camPixels, W, H);
                    print("W & H :" + W + " - " + H);
                    if (result != null)
                    {
                        print("OK :" + result.Text);
                        QR_result = result.Text;
                    }

                    // Sleep a little bit and set the signal to get the next frame
                    Thread.Sleep(2000);
                    camPixels = null;
                }
            }
            catch
            {
                print("ERROR");
            }
        }
    }

    /*
    private static Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }
    */

    private IEnumerator HideWarning()
    {
        yield return new WaitForSeconds(2);
        canCheck = true;
        warningTextField.text = "";
    }
}