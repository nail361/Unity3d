using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimManager : MonoBehaviour {

    Animation anim = null;
    AnimationClip[] clips;

    private void Awake()
    {
        anim = GetComponent<Animation>();
        anim.playAutomatically = false;
    }

    public void PlayAnim()
    {
        anim.Play();
    }

    public bool HasAnim()
    {
        if (anim == null || anim.GetClipCount() == 0 ) return false;
        else return true;
    }
}
