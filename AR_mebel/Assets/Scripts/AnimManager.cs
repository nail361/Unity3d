using UnityEngine;

public class AnimManager : MonoBehaviour {

    Animation anim = null;
    AnimationClip[] clips;

    private void Awake()
    {
        anim = GetComponent<Animation>();
        anim.playAutomatically = false;
        anim.wrapMode = WrapMode.PingPong;
    }

    public void PlayAnim()
    {
        if (anim.isPlaying) {
            anim.Stop();
            anim.Rewind();
        }
        else
            anim.Play();
    }
}
