using UnityEngine;
using System.Collections;

public class Closet : InteractiveObject
{

    private bool empty = true;
    private Vector2 player_dir;

    private Vector2 exit_pos;

    [SerializeField]
    private Transform door;

    public override void Init(int index, int special, InteractiveObjects interactive_component)
    {
        base.Init(index, special, interactive_component);
        
        door.gameObject.GetComponentInChildren<SpriteRenderer>().sprite = LevelConstructor.atlasHolder.GetInteractiveByName(_image.sprite.name + "_1");

        exit_pos = transform.position;

        switch (special)
        {
            case 0: exit_pos = new Vector2(exit_pos.x + 1, exit_pos.y); break;
            case 1: exit_pos = new Vector2(exit_pos.x, exit_pos.y + 1); break;
            case 2: exit_pos = new Vector2(exit_pos.x - 1, exit_pos.y); break;
            case 3: exit_pos = new Vector2(exit_pos.x, exit_pos.y - 1); break;
        }
    }

    public override void Action()
    {
        if (empty)
        {
            player_dir = Player.instance.ClosetIn( exit_pos, transform.position, transform.rotation );
            Player.instance.bubble.ShowMessage(LanguageManager.GetText("SayHide"));
            StartCoroutine("Visualize");
        }
        else
        {
            Player.instance.ClosetOut( exit_pos, player_dir );
            StartCoroutine("Visualize");
        }

        empty = !empty;
    }

    IEnumerator Visualize()
    {
        interactive_component.DeActivate();

        float scale = 0.0f;

        Vector3 start = door.localPosition;
        Vector3 end = new Vector3(0.5f,0,0);

        while (door.localPosition != end)
        {
            door.localPosition = Vector3.Lerp(start, end, scale);
            scale += Time.deltaTime * 2;

            if (scale >= 1) door.localPosition = end;

            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        start = door.localPosition;
        end = Vector3.zero;
        scale = 0.0f;

        while (door.localPosition != end)
        {
            door.localPosition = Vector3.Lerp(start, end, scale);
            scale += Time.deltaTime * 2;

            if (scale >= 1) door.localPosition = end;

            yield return null;
        }

        interactive_component.Activate();

        yield return null;
    }

}