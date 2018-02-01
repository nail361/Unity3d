using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour {

    public GameObject[] listOfProduction;
    public Transform productContainer;
    public Button list_item;
    public Transform lists_container;

    public LayerMask layerMask;

    public Toggle auto_rotate;
    public Toggle auto_switch;

    private int curProductIndex = 0;

    private float cur_time = 0.0f;
    private float time_to_switch = 15.0f;
    private float rotation_speed = 5.0f;

    private Ray ray;
    private RaycastHit hit;
    private Transform hitted_object;

    void Start () {
        FillListContainer();
        SwitchProduct(0);
    }


    void FillListContainer()
    {
        int index = 0;
        foreach(GameObject product in listOfProduction)
        {
            int new_index = index;
            Button new_list_item = Instantiate(list_item, lists_container) as Button;
            new_list_item.GetComponentInChildren<Text>().text = product.name;
            new_list_item.onClick.AddListener(delegate { SwitchProduct(new_index); });
            index++;
        }
    }

    void Update () {

        if (auto_rotate.isOn)
        {
            productContainer.Rotate(Vector3.up, Time.deltaTime * rotation_speed);
        }

        if (auto_switch.isOn)
        {
            cur_time += Time.deltaTime;

            if (cur_time >= time_to_switch)
            {
                cur_time = 0.0f;
                curProductIndex++;
                if (curProductIndex >= listOfProduction.Length)
                    curProductIndex = 0;

                SwitchProduct(curProductIndex);
            }
        }

        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out hit, 50, layerMask.value);
                if (hit.collider)
                {
                    hitted_object = hit.transform;
                }
                else
                {
                    hitted_object = null;
                }
            }
            else if (Input.GetMouseButton(0) && hitted_object){
                hitted_object.transform.Rotate(
                    new Vector3(
                        Input.GetAxis("Mouse Y"),
                        -Input.GetAxis("Mouse X"),
                        0) * Time.deltaTime * rotation_speed*20, Space.World);
            }
        }
        else if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    Physics.Raycast(ray, out hit, 50, layerMask.value);
                    if (hit.collider)
                    {
                        hitted_object = hit.transform;
                    }
                    else
                    {
                        hitted_object = null;
                    }
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Moved && hitted_object)
                {
                    hitted_object.transform.Rotate(
                        new Vector3(
                            Input.GetTouch(0).deltaPosition.y,
                            -Input.GetTouch(0).deltaPosition.x,
                            0) * Time.deltaTime * rotation_speed * 20, Space.World);
                }
            }
        }

    }

    public void SwitchProduct(int productIndex) {
        if (productContainer.childCount > 0)
            Destroy(productContainer.GetChild(0).gameObject);

        curProductIndex = productIndex;

        Instantiate(listOfProduction[productIndex], productContainer, false);
    }
}
