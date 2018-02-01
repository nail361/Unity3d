using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour
{
    private float _speed = 0.1f;
    protected float speed
    {
        get { return _speed; }
        set { _speed = value; OnSpeedValuesChange(); }
    }

    private float _increase_speed = 0.0f; //индекс увеличения скорости
    protected float increase_speed
    {
        get { return _increase_speed; }
        set { _increase_speed = value; OnSpeedValuesChange(); }
    }

    private float _floor_speed = 0.0f;//индекс скорости в зависимости от поверхности
    protected float floor_speed
    {
        get { return _floor_speed; }
        set { _floor_speed = value; OnSpeedValuesChange(); }
    }

    protected Collider2D hit_collider;
    protected Vector3 move_end;

    protected Vector2 move_dir = new Vector2(0,0);
    protected Vector2 last_dir = new Vector2(0,0);
    protected Vector2 last_pos = new Vector2(0,0);
    protected Vector2 start_pos;

    protected bool canControl = true;

    protected bool isMoving = false;

    private LayerMask layerMask;

    protected virtual void Start()
    {
        layerMask = LayerMask.GetMask("BlockingLayer", "Floor");
        move_end = last_pos = start_pos = transform.position;
    }

    protected bool CheckMove(Vector2 direction)
    {
        move_end = new Vector3(transform.position.x + direction.x, transform.position.y + direction.y, 0);
        hit_collider = Physics2D.OverlapCircle(move_end, 0.3f, layerMask);
        if (hit_collider == null) {
            return true;
        }

        return false;
    }

    protected IEnumerator Movement()
    {
        isMoving = true;

        last_dir = move_dir;

        float sqrRemainingDistance = (transform.position - move_end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, move_end, (Mathf.Max(CurrentSpeed(), 0.1f)) * Time.deltaTime);

            sqrRemainingDistance = (transform.position - move_end).sqrMagnitude;

            yield return null;
        }

        last_pos = transform.position;

        if (hit_collider == null && floor_speed != 0)
        {
            floor_speed = 0.0f;
            canControl = true;
        }

        isMoving = false;
    }

    public float CurrentSpeed()
    {
        return speed + increase_speed + floor_speed;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "FastFloor")
        {
            floor_speed = 1.0f;
            canControl = false;
        }
        else if (other.tag == "SlowFloor")
        {
            floor_speed = -0.3f;
        }
        else if (other.tag == "TV")
        {
            OnTvEnter(other.GetComponentInParent<TV>().watch_time);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        /*
        if (other.tag == "FastFloor" || other.tag == "SlowFloor")
        {
            if (hit_collider != null)
            {
                if (hit_collider.gameObject.layer != LayerMask.NameToLayer("Floor"))
                    return;
            }
            
            floor_speed = 0.0f;
            canControl = true;
        }
        */
    }

    protected virtual void OnSpeedValuesChange() { }
    protected virtual void OnTvEnter(float watch_time) { }
    protected abstract void ChangeDirection();

}
