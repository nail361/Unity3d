using UnityEngine;
using System.Collections;

public class Oven : InteractiveObject
{
    private Vector2 unFredge_dir;
    private ParticleSystem _particleSystem;
    private Sprite open_img;
    private Sprite close_img;

    private int layerMask;

    public override void Init(int index, int special, InteractiveObjects interactive_component)
    {
        base.Init(index,special,interactive_component);

        close_img = _image.sprite;
        open_img = LevelConstructor.atlasHolder.GetInteractiveByName(_image.sprite.name + "_1");

        layerMask = (1 << 9) | (1 << 11);

        _particleSystem = GetComponentInChildren<ParticleSystem>();

        switch (special)
        {
            case 0: unFredge_dir = new Vector2(1, 0); break;
            case 1: unFredge_dir = new Vector2(0, 1); break;
            case 2: unFredge_dir = new Vector2(-1, 0); break;
            case 3: unFredge_dir = new Vector2(0, -1); break;
        }
    }

    public override void Action()
    {
        Player.instance.bubble.ShowMessage(LanguageManager.GetText("SayHot"));
        StartCoroutine("UnFressing");
    }

    IEnumerator UnFressing()
    {
        interactive_component.DeActivate();

        _image.sprite = open_img;
        _particleSystem.Play();

        yield return new WaitForSeconds(0.5f);

        Vector2 pos = (Vector2)transform.position + unFredge_dir;

        while (CheckPos(pos))
        {
            pos += unFredge_dir;
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);
        _image.sprite = close_img;

        yield return new WaitForSeconds(5.0f);

        interactive_component.Activate();

        yield return null;
    }

    private bool CheckPos(Vector2 pos)
    {
       Collider2D collider = Physics2D.OverlapPoint(pos,layerMask);

        if (collider != null)
        {
            if (collider.tag == "Block")
                return false;
            else if (collider.tag == "FastFloor")
                Destroy(collider.gameObject);
        }

        return true;
    }

}