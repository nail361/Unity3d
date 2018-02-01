using UnityEngine;
using System.Collections.Generic;

public enum EnemyType{ VC, DOG };
public class Enemy : MovingObject {

    public static List<Enemy> ENEMIES = new List<Enemy>();

    [SerializeField]
    private GameObject WaiteRingPref;

    [SerializeField]
    private GameObject voltagePrefab;

    [HideInInspector]
    public int enemy_level;

    [HideInInspector]
    public int special;

    private EnemyType type;

    private bool active = false;
    [HideInInspector]
    public bool isActive
    {
        get { return active; }
    }

    private float radar_distance; //дальность отображения на радаре
    [HideInInspector]
    public float RadarDistance
    {
        get { return radar_distance; }
    }

    private float type_time; //Если пылесос - как долго чистит, в секундах. Если собака - то другое что-то???

    protected override void Start()
    {
        base.Start();

        name = "Enemy";

        if (enemy_level <= 3) type = EnemyType.VC;

        switch (enemy_level)
        {
            //пылесосы
            case 0: speed = 0.5f; radar_distance = 80f; type_time = 3.0f; break;
            case 1: speed = 0.7f; radar_distance = 75f; type_time = 2.8f; break;
            case 2: speed = 0.8f; radar_distance = 75f; type_time = 2.5f; break;
            case 3: speed = 0.9f; radar_distance = 65f; type_time = 2.2f; break;
            //собаки
            case 4: speed = 1.0f; radar_distance = 55f; type_time = 3.0f; break;
            case 5: speed = 1.0f; radar_distance = 55f; type_time = 3.0f; break;
            case 6: speed = 1.2f; radar_distance = 40f; type_time = 3.0f; break;

            default: speed = 0.5f; radar_distance = 80f; break;
        }

        ENEMIES.Add(this);

        Invoke("StartDirection", 1.0f);
    }

    void Update () {

        if ( GameManager.instance.pause | !active ) return;

        if (move_dir.magnitude > 0 && !isMoving)
        {
            if (enemy_level >= 4) LookForPlayer();

            if (CheckMove(move_dir))
            {
                StartCoroutine(Movement());
            }
            else
            {
                if (hit_collider.gameObject.layer == LayerMask.NameToLayer("BlockingLayer")) {
                    ChangeDirection();
                    return;
                }

                StartCoroutine(Movement());
            }
        }
    }

    public void Voltage(float power)
    {
        if (type == EnemyType.VC)
        {
            //анимация электричества
            GameObject voltageGO = Instantiate(voltagePrefab, transform, false) as GameObject;
            Destroy(voltageGO, power);
            Deactivate(power);
        }
    }

    private void LookForPlayer()
    {
        Vector2 cur_pos = transform.position;
        cur_pos += move_dir;

        Collider2D collider = Physics2D.OverlapPoint(cur_pos);

        while (collider == null || collider.tag == "item")
        {
            cur_pos += move_dir;
            collider = Physics2D.OverlapPoint(cur_pos);
        }

        if (collider.tag == "Player") increase_speed = 0.5f;
        else increase_speed = 0.0f;
    }

    protected override void ChangeDirection()
    {
        if (enemy_level <= 1)
        {
            switch (special)
            {
                case 0:
                case 1:
                    move_dir *= -1;
                    break;
                case 2: move_dir = new Vector3(move_dir.y, -move_dir.x); break;
                case 3: move_dir = new Vector3(-move_dir.y, move_dir.x); break;
            }
        }
        else if (enemy_level <= 3)
        {
            Vector2[] checkMoves = new Vector2[3];
            int movement = 0;
            switch (special)
            {
                case 0:
                    movement = -1;
                    break;
                case 1:
                    movement = 1;
                    break;
                case 2:
                case 3:
                    movement = Random.value > 0.5 ? 1 : -1;
                    break;

            }

            if (move_dir.x == 0)
            {
                checkMoves[0] = new Vector2(movement, 0);
                checkMoves[1] = new Vector2(-movement, 0);
            }
            else
            {
                checkMoves[0] = new Vector2(0, movement);
                checkMoves[1] = new Vector2(0, -movement);
            }

            checkMoves[2] = -move_dir;

            foreach (Vector2 direction in checkMoves)
            {
                if (CheckMove(direction))
                {
                    move_dir = direction;
                    break;
                }
                else if (hit_collider.gameObject.layer == LayerMask.NameToLayer("Floor"))
                {
                    move_dir = direction;
                    break;
                }
            }
        }

        ChangeRotation();
    }

    private void StartDirection()
    {
        if (enemy_level <= 3)
        {
            //движение пылесосов по алгоритму.
            switch (special)
            {
                case 0:
                    move_dir = new Vector2(Random.value > 0.5f ? 1 : -1, 0);
                    break;
                case 1:
                    move_dir = new Vector2(0, Random.value > 0.5f ? 1 : -1);
                    break;
                case 2:
                    move_dir = new Vector2(0, -1);
                    break;
                case 3:
                    move_dir = new Vector2(-1, 0);
                    break;
            }
        }

        /*
        switch (Random.Range(0, 4))
        {
            case 0: move_dir = new Vector2(0, 1); break;
            case 1: move_dir = new Vector2(0, -1); break;
            case 2: move_dir = new Vector2(1, 0); break;
            case 3: move_dir = new Vector2(-1, 0); break;
        }
        */

        active = true;

        ChangeRotation();
    }

    private void ChangeRotation()
    {
        if (move_dir.x == 0)
        {
            if (move_dir.y < 0) transform.localRotation = Quaternion.Euler(0, 0, 270);
            else transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else
        {
            if (move_dir.x > 0) transform.localRotation = Quaternion.Euler(0,0,0);
            else transform.localRotation = Quaternion.Euler(0, 0, 180);
        }
    }

    protected override void OnTvEnter(float watch_time)
    {
        //if dog then stop
    }

    private void OnTvExit()
    {
        //if dog then cont walk
    }

    override protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Dust")
        {
            //Включить анимацию чистки
            Deactivate(type_time);
            other.tag = "Untagged";
            Destroy(other.gameObject, type_time);
        }
        else if (other.tag == "Enemy" || other.tag == "Box")
        {
            StopCoroutine(Movement());
            move_dir *= -1;
            move_end = last_pos;
            StartCoroutine(Movement());
            ChangeRotation();
        }
        else base.OnTriggerEnter2D(other);
    }

    public Vector3 GetMoveEnd()
    {
        return move_end;
    }

    public void Deactivate( float time )
    {
        active = false;
        (Instantiate(WaiteRingPref, transform) as GameObject).GetComponent<TimeRing>().Init(time);
        Invoke("Activate", time);
    }

    public void Activate()
    {
        active = true;
    }

    void OnDestroy()
    {
        ENEMIES.Remove(this);
    }

}