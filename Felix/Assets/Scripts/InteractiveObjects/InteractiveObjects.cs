using UnityEngine;
using UnityEngine.UI;

public class InteractiveObjects : MonoBehaviour {

    public Button action_btn;

    [SerializeField]
    private GameObject[] objects;

    private InteractiveObject interactive_object;

    public void Init(int index, int special)
    {
        interactive_object = ((GameObject)Instantiate(objects[index - 85], transform.position, transform.rotation)).GetComponent<InteractiveObject>();
        interactive_object.Init(index, special, this);

        action_btn.transform.rotation = Quaternion.identity;
        action_btn.gameObject.SetActive(false);
    }

    public void Action()
    {
        interactive_object.Action();
    }

    public void Activate()
    {
        interactive_object.Activate();
    }

    public void DeActivate()
    {
        interactive_object.DeActivate();
    }

}