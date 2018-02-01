using UnityEngine;

public class TV : InteractiveObject {
    
    [SerializeField]
    private BoxCollider2D watch_collider;
    [SerializeField]
    private SpriteRenderer _light;
    public float watch_time = 2.0f;

    public override void Init(int index, int special, InteractiveObjects interactive_component)
    {
        base.Init(index, special, interactive_component);

        _light.sprite = LevelConstructor.atlasHolder.GetInteractiveByName(_image.sprite.name + "_1");
        _light.enabled = false;

        watch_collider.enabled = false;
    }

    public override void Action()
    {
        Player.instance.bubble.ShowMessage(LanguageManager.GetText("SayTV"));
        interactive_component.DeActivate();
        watch_collider.enabled = true;
        _light.enabled = true;
        Invoke("ActivateInteractive", watch_time);
    }

    private void ActivateInteractive()
    {
        interactive_component.Activate();
        _light.enabled = false;
        watch_collider.enabled = false;
    }

}