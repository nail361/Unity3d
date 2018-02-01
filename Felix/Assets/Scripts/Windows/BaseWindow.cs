using UnityEngine;

public class BaseWindow : MonoBehaviour {

    protected bool isActive = true;

    protected Animation _animation;
    protected string anim_name;

    protected void ActivateCamera()
    {
        Camera.main.SendMessage("ActiveSwitch", SendMessageOptions.DontRequireReceiver);
    }

    protected void DeactivateCamera()
    {
        Camera.main.SendMessage("DeActiveSwitch", SendMessageOptions.DontRequireReceiver);
    }

    protected void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void ActivateAndDestroy()
    {
        ActivateCamera();
        DestroySelf();
    }

    public virtual void CloseWindow()
    {
        if (!isActive) return;
        isActive = false;

        _animation[anim_name].speed = -1;
        _animation[anim_name].time = _animation[anim_name].length;
        _animation.Play();
        Invoke("ActivateAndDestroy", _animation[anim_name].length);
    }
}
