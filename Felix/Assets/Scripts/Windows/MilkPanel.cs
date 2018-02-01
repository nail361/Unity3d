using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MilkPanel : MonoBehaviour {

    [SerializeField]
    private Text milk_counter_text;
    [SerializeField]
    private Text new_milk_text;
    [SerializeField]
    private Image milk_img;
    [SerializeField]
    private Image new_milk_img;
    [SerializeField]
    private Text milk_timer_text;
    [SerializeField]
    private Button buy_milk_btn;
    [SerializeField]
    private GameObject addItemWindowPref;
    [SerializeField]
    private Text milk_text_old;

    private Color saved_color;

    void Start () {
        GameManager.instance.milk_panel = this;
        milk_timer_text.text = "";
        buy_milk_btn.gameObject.SetActive(false);
        SetMilkCount(GameManager.instance.milk.GetConverted<uint>(), GameManager.instance.new_milk.GetConverted<uint>());

        saved_color = new_milk_img.color;
    }

    public void SetMilkCount(uint milk_count, uint new_milk_count)
    {
        milk_counter_text.text = "x" + milk_count.ToString();

        if (new_milk_count == 0)
        {
            new_milk_img.gameObject.SetActive(false);
        }
        else
        {
            new_milk_img.gameObject.SetActive(true);
            new_milk_text.text = new_milk_count.ToString();
        }

    }

    public void SetMilkTimer(int value)
    {
        if (value == 0)
        {
            milk_timer_text.text = "";
            buy_milk_btn.gameObject.SetActive(false);
        }
        else
        {
            float min = Mathf.Floor(value / 1000 / 60);
            float sec = Mathf.Floor((value - (min * 60 * 1000)) / 1000);
            milk_timer_text.text = min < 10 ? "0" + min.ToString() : min.ToString();
            milk_timer_text.text += ":";
            milk_timer_text.text += sec < 10 ? "0" + sec.ToString() : sec.ToString();

            buy_milk_btn.gameObject.SetActive(true);
        }
    }

    public void BuyMilkCLICK()
    {
        GameObject go = (GameObject)Instantiate(addItemWindowPref, Vector3.zero, Quaternion.identity);
        go.transform.SetParent(GameObject.Find("HUD").transform, false);
        go.GetComponent<AddItem>().SetItem(ItemIndex.Milk, LanguageManager.GetText("Milk"), milk_img.sprite, GameManager.instance.MissMilk());
    }

    public void UseMilk()
    {
        GetComponent<Animation>().Play("milk_old");
        milk_text_old.text = "  " + (GameManager.instance.milk + 1).GetConverted<string>();
        milk_counter_text.text = "x";
        Invoke("UseDelay",0.5f);
    }

    public void FlashNewMilk()
    {
        StopCoroutine("Flashing");
        new_milk_img.color = saved_color;
        StartCoroutine("Flashing");
    }

    IEnumerator Flashing()
    {
        new_milk_img.color = new Color(0.5f, 0.5f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        new_milk_img.color = saved_color;
        yield return new WaitForSeconds(0.5f);
        new_milk_img.color = new Color(0.5f, 0.5f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        new_milk_img.color = saved_color;
        yield return null;
    }

    private void UseDelay()
    {
        SetMilkCount(GameManager.instance.milk.GetConverted<uint>(), GameManager.instance.new_milk.GetConverted<uint>());
    }

    void OnDestroy()
    {
        GameManager.instance.milk_panel = null;
    }

}