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

    public Texture2D encoded;

    private WebCamTexture camTexture;
    private Thread qrThread;

    private Color32[] c;
    private int W, H;

    private bool isQuit;
    private bool canCheck = true;

    private string QR_result = "";
    
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
        encoded = new Texture2D(256, 256);

        camTexture = new WebCamTexture(300, 300, 30);
        rawimage.texture = camTexture;
        rawimage.material.mainTexture = camTexture;
        OnEnable();

        qrThread = new Thread(DecodeQR);
        qrThread.Start();
    }

    void Update()
    {
        if (c == null)
        {
            c = camTexture.GetPixels32();
        }

        var textForEncoding = QR_result;
        if (canCheck && textForEncoding.Length > 0)
        {
            var color32 = Encode(textForEncoding, encoded.width, encoded.height);
            encoded.SetPixels32(color32);
            encoded.Apply();
            
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

            if (QR_result.IndexOf("mebel-qr.mediaidea.net") != -1) {
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
        barcodeReader.AutoRotate = false;
        barcodeReader.Options.TryHarder = false;

        while (true)
        {
            if (isQuit)
                break;

            try
            {
                // decode the current frame
                var result = barcodeReader.Decode(c, W, H);
                if (result != null)
                {
                    print(result.Text);
                    QR_result = result.Text;
                }

                // Sleep a little bit and set the signal to get the next frame
                Thread.Sleep(2000);
                c = null;
            }
            catch
            {
            }
        }
    }

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

    private IEnumerator HideWarning()
    {
        yield return new WaitForSeconds(2);
        canCheck = true;
        warningTextField.text = "";
    }
}