using UnityEngine;
using UnityEngine.UI;

public class FishPanel : MonoBehaviour {

    [SerializeField]
    private Text fish_text;

    [SerializeField]
    private GameObject buyWindowPref;

    private Color cur_color;

    void Start () {
        cur_color = fish_text.color;
        SetFish();
    }

    private void SetFish()
    {
        string str = GameManager.instance.fish.GetConverted<string>();
        while (str.Length < 5) str = "0" + str;
        fish_text.text = "X " + str;
    }

    public void AddFish(uint count)
    {
        GameManager.instance.fish += count;
        SetFish();
        Flashing(Color.green);
    }

    public bool SubFish(uint count)
    {
        if ( GameManager.instance.fish >= count) {
            GameManager.instance.fish -= count;
            SetFish();

            Flashing(Color.blue);

            return true;
        }

        Flashing(Color.red);

        return false;
    }

    private void Flashing(Color flash_color)
    {
        if (fish_text.color != cur_color) return;

        fish_text.color = flash_color;
        StartCoroutine(GameManager.instance.waitThenCallback(1f, new System.Action(delegate () { fish_text.color = cur_color; })));
    }

    public void BuyWindowCLICK()
    {
        Instantiate(buyWindowPref, Vector3.zero, Quaternion.identity);
    }

}