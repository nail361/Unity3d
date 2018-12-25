using UnityEngine;

public class AnimManager : MonoBehaviour {

    Animation anim = null;
    AnimationClip[] clips;

    private void Awake()
    {
        anim = GetComponent<Animation>();
        anim.playAutomatically = false;
        anim.wrapMode = WrapMode.ClampForever;
    }

    public void PlayAnim()
    {
        if (anim.isPlaying)
        {
            anim.Rewind();
            anim.wrapMode = WrapMode.Once;
            anim.Sample();
            anim.Stop();
        }
        else
        {
            anim.wrapMode = WrapMode.ClampForever;
            anim.Play();
        }
    }
}
