using UnityEngine;

public class Vase : InteractiveObject
{
    private Sprite empty_place;
    private Sprite dust_sprite;
    [SerializeField]
    private GameObject dustPref;

    public override void Init(int index, int special, InteractiveObjects interactive_component)
    {
        base.Init(index, special, interactive_component);

        empty_place = LevelConstructor.atlasHolder.GetInteractiveByName(_image.sprite.name + "_1");
        dust_sprite = LevelConstructor.atlasHolder.GetInteractiveByName(_image.sprite.name + "_2");
    }
    
    public override void Action()
    {
        Player.instance.bubble.ShowMessage(LanguageManager.GetText("SayOops"));
        _image.sprite = empty_place;

        Vector3 pos = new Vector3(Mathf.Round(lastEnterCollider.transform.position.x), Mathf.Round(lastEnterCollider.transform.position.y), 0);

        GameObject dust = (GameObject)Instantiate(dustPref, pos, transform.rotation);
        dust.GetComponent<SpriteRenderer>().sprite = dust_sprite;

        interactive_component.DeActivate();
    }

}