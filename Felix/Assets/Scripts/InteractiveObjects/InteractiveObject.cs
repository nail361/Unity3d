using UnityEngine;

public class InteractiveObject : MonoBehaviour {

    protected int index;
    protected int special;
    protected InteractiveObjects interactive_component;
    protected SpriteRenderer _image;
    protected Collider2D _collider2D;
    protected Collider2D lastEnterCollider;

    public virtual void Init(int index, int special, InteractiveObjects interactive_component)
    {
        this.index = index;
        this.special = special;
        this.interactive_component = interactive_component;

        _image = GetComponent<SpriteRenderer>();
        _collider2D = GetComponent<Collider2D>();

        _image.sprite = LevelConstructor.atlasHolder.GetObject(index);

        gameObject.transform.SetParent(interactive_component.transform);

        name = _image.sprite.name;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            interactive_component.action_btn.gameObject.SetActive(true);
            lastEnterCollider = other;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            interactive_component.action_btn.gameObject.SetActive(false);
        }
    }

    public virtual void Action() {}

    public virtual void Activate()
    {
        _collider2D.enabled = true;

        if (_collider2D.IsTouching(lastEnterCollider)) OnTriggerEnter2D(lastEnterCollider);
    }

    public virtual void DeActivate()
    {
        _collider2D.enabled = false;
        interactive_component.action_btn.gameObject.SetActive(false);
    }
}