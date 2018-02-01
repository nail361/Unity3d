using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalPanel : BaseWindow {

    public virtual void Start()
    {
        transform.SetParent(GameObject.FindGameObjectWithTag("HUD").transform, false);
        transform.SetAsLastSibling();

        _animation = GetComponent<Animation>();
    }

    virtual public void RetryCLICK()
    {
        if (!isActive) return;
        isActive = false;
        StartCoroutine(GameManager.instance.LoadLevel(SceneManager.GetActiveScene().name, 1.5f));
    }

    virtual public void MenuCLICK()
    {
        if (!isActive) return;
        isActive = false;
        StartCoroutine(GameManager.instance.LoadLevel("ChooseLevel", 1.5f));
    }
}