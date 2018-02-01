using UnityEngine;

public class Box : MovingObject {

    protected override void Start () {
        speed = 0.5f;
        base.Start();
	}

    public bool Move(Vector2 direction, float player_speed)
    {
        bool flag;

        CheckBoxMove(direction, out flag);

        speed = player_speed;

        if (flag)
            StartCoroutine("Movement");

        return flag;
    }
    
    private void CheckBoxMove(Vector2 direction, out bool flag)
    {
        flag = CheckMove(direction);
        /*
        Vector3 point = new Vector3(transform.position.x + direction.x*1.5f, transform.position.y + direction.y*1.5f, 0);
        Collider2D _collider2D = Physics2D.OverlapCircle(point, 0.2f, LayerMask.GetMask("BlockingLayer"));

        if (_collider2D != null && _collider2D.tag == "Enemy") flag = false;
        */

        if (flag)
        {
            foreach (Enemy enemy in Enemy.ENEMIES)
            {
                if (enemy.GetMoveEnd() == move_end) flag = false;
            }
        }
        else
        {
            if (hit_collider.tag == "Laser" || hit_collider.tag == "FastFloor" || hit_collider.tag == "SlowFloor") flag = true;
        }
    }

    protected override void ChangeDirection() { }

}