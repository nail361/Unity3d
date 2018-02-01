using System.Collections;
using UnityEngine;

public class Player : MovingObject
{
    private enum STATES:int{IDLE, WALK};

    public static Player instance = null;
    
    private Animator animator;
    [HideInInspector]
    public Bubble bubble;

    [SerializeField]
    private GameObject WaiteRingPref;
    //способности

    //Камок шерсти
    [SerializeField]
    private GameObject DustPref;
    //Коробка
    [SerializeField]
    private GameObject BoxPref;
    private GameObject box;
    private bool boxHide = false;
    //Валерьянка
    [SerializeField]
    private GameObject SpeedPref;
    private GameObject speedObject;
    //------------------------
    [SerializeField]
    private GameObject HeliPref;
    private GameObject helicopter;

    private float god_mode = 0f;
    //-------

    [SerializeField]
    private GameObject DirectionPref;
    private DirectionStatus direction_status;

    private bool haveStar = false;

    private uint star_count;

    private GameHUD hud;

    private float fast_end = 300f;
    private uint score = 0;

    private bool canSlow = true;

    private float last_tab_time = 0;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(instance);
        }
    }

    public void Init()
    {
        name = "Player";
        animator = GetComponent<Animator>();
        ChangeAnimation(STATES.IDLE);

        direction_status = ((GameObject)Instantiate(DirectionPref, transform.parent)).GetComponent<DirectionStatus>();

        speed = 1.0f;

        if (GameManager.instance.cur_mode == 0) star_count = 0;
        else star_count = 3;

        hud = GameObject.Find("HUD").GetComponent<GameHUD>();

        for (uint i = 0; i < star_count; i++) hud.AddStar(i);

        GameManager.instance.pause = false;

        bubble = GameObject.Find("HUD/bubble").GetComponent<Bubble>();
        bubble.ShowMessage(LanguageManager.GetText("SayStart"));
    }

    void Update()
    {
        if (GameManager.instance.pause) return;

        fast_end -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.CancelQuit();
        }

        if (god_mode > 0) god_mode -= Time.deltaTime;

        if (!canControl) return;

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            start_pos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector2 new_dir = (Vector2)Input.mousePosition - start_pos;

            if (new_dir.sqrMagnitude > 150.0f) {

                if (Mathf.Abs(new_dir.x) >= Mathf.Abs(new_dir.y)) new_dir.y = 0;
                else new_dir.x = 0;

                move_dir = new_dir.normalized;
            }
            else
            {
                if (Time.unscaledTime - last_tab_time < 0.5f)
                {
                    hud.ToggleSlow();
                }
                last_tab_time = Time.unscaledTime;
            }
        }

#else
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                start_pos = Input.GetTouch(0).position;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                Vector2 new_dir = Input.GetTouch(0).position - start_pos;

                if (new_dir.sqrMagnitude > 50.0f)
                {
                    if (Mathf.Abs(new_dir.x) >= Mathf.Abs(new_dir.y)) new_dir.y = 0;
                    else new_dir.x = 0;

                    move_dir = new_dir.normalized;
                }
                else
                {
                    if (Time.unscaledTime - last_tab_time < 0.5f)
                    {
                        hud.ToggleSlow();
                    }
                    last_tab_time = Time.unscaledTime;
                }
            }
        }
#endif

    }

    void FixedUpdate()
    {
        if (move_dir.sqrMagnitude > 0)
        {
            if (move_dir != last_dir)
            {
                direction_status.Show(move_dir, move_end);
            }

            if (isMoving) return;

            ChangeAnimation(STATES.WALK);

            if (direction_status.isShow()) direction_status.Hide();

            if (boxHide)
            {
                if (box == null)
                {
                    move_dir = new Vector2(0, 0);
                    box = (GameObject)Instantiate(BoxPref, new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0), Quaternion.identity);
                    return;
                }
                else
                {
                    Destroy(box);
                    box = null;
                    boxHide = false;
                }
            }
            else if(helicopter!=null & god_mode <= 0)
            {
                increase_speed -= 0.5f;
                Destroy(helicopter);
            }

            if (CheckMove(move_dir))
            {
                ChangeRotation();
                StartCoroutine("Movement");
            }
            else
            {
                switch (hit_collider.tag)
                {
                    case "Box":
                        ChangeRotation();
                        if (hit_collider.GetComponent<Box>().Move(move_dir, CurrentSpeed()))
                        {
                            CancelInvoke("SwitchSlowOn");
                            canSlow = false;
                            Invoke("SwitchSlowOn", 2.0f);
                            StartCoroutine("Movement");
                        }
                        else
                            ChangeDirection();
                        break;
                    case "Block":
                        ChangeDirection();
                        break;
                    default:
                        ChangeRotation();
                        StartCoroutine("Movement");
                        break;
                }
            }
        }

    }

    protected override void ChangeDirection()
    {
        Vector2[] checkMoves = new Vector2[3];

        int movement = Random.value > 0.5 ? 1 : -1;

        if (move_dir.x == 0) {
            
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
            else if (hit_collider.tag != "Block")
            {
                move_dir = direction;
                break;
            }
        }

        ChangeRotation();
    }

    private void ChangeRotation()
    {
        if (move_dir != last_dir)
        {
            Vector3 new_rotation = transform.localRotation.eulerAngles;
            if (move_dir.x > 0) new_rotation.z = 0;
            else if (move_dir.x < 0) new_rotation.z = 180;
            else if (move_dir.y > 0) new_rotation.z = 90;
            else new_rotation.z = 270;

            transform.localRotation = Quaternion.Euler(new_rotation.x, new_rotation.y, new_rotation.z);
        }
    }

    override protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Star")
        {
            if (haveStar)
            {
                bubble.ShowMessage(LanguageManager.GetText("SaySosige"));
            }
            else
            {
                haveStar = true;
                bubble.ShowMessage(LanguageManager.GetText("SayExit"));
                if (GameManager.instance.cur_mode == 0) hud.HaveStar(star_count);
                Destroy(other.gameObject);
            }
        }
        else if (other.tag == "Exit")
        {
            switch (GameManager.instance.cur_mode)
            {
                case 0:
                    if (haveStar)
                    {
                        haveStar = false;
                        hud.AddStar(star_count);
                        star_count++;

                        if (star_count == 3)
                        {
                            EndGame("GameWin");
                            GameManager.instance.LevelComplete(star_count, score + (uint)fast_end);
                        }
                        else
                        {
                            bubble.ShowMessage(LanguageManager.GetText("SayNextSosige"));
                        }
                    }
                    break;
                case 1:
                    if (haveStar)
                    {
                        Timers.StopTimer("stop_watch", true);
                        EndGame("GameWin");
                        float time = Timers.GetTime("stop_watch")/1000;
                        uint star = 1; //(uint)Mathf.CeilToInt( (float)Timers.GetTime("stop_watch") / (GameManager.instance.level_param / 3) );

                        if (time > GameManager.instance.level_params[2]) star++;
                        if (time > GameManager.instance.level_params[1]) star++;

                        GameManager.instance.LevelComplete(star, (uint)fast_end);
                        Timers.RemoveTimer("stop_watch");
                    }
                    break;
                case 2:
                    EndGame("GameWin");
                    GameManager.instance.LevelComplete(star_count, (uint)fast_end);
                    break;
            }

        }
        else if (other.tag == "Enemy" && god_mode <= 0)
        {
            if (!other.gameObject.GetComponent<Enemy>().isActive) return;

            Handheld.Vibrate();

            switch (GameManager.instance.cur_mode)
            {
                case 0:
                    if (star_count > 0) {
                        GameManager.instance.pause = true;
                        move_dir = new Vector2(0, 0);
                        hud.GameOver(star_count,score);
                        //EndGame("GameWin");
                        //GameManager.instance.LevelComplete(star_count, score);
                    }
                    else
                        EndGame("GameOver");
                    break;
                case 1:
                    Timers.RemoveTimer("stop_watch");
                    EndGame("GameOver");
                    break;
                case 2:
                    star_count--;
                    hud.RemoveStar(star_count);

                    other.gameObject.GetComponent<Enemy>().Deactivate(5.0f);

                    if (star_count == 0) EndGame("GameOver");
                    break;
            }
           
        }
        else if ((other.tag == "FastFloor" || other.tag == "SlowFloor") && hud.IsSlow)
        {
            hud.ToggleSlow();
        }

        base.OnTriggerEnter2D(other);
    }

    private void ChangeAnimation(STATES anim_state)
    {
        animator.SetInteger("state", (int)anim_state);
    }

    protected override void OnTvEnter(float watch_time)
    {
        if (hud.IsSlow) hud.ToggleSlow();
        move_dir = new Vector2(0, 0);
        (Instantiate(WaiteRingPref, transform) as GameObject).GetComponent<TimeRing>().Init(watch_time);
        Invoke("OnTvExit", watch_time);
    }

    private void OnTvExit()
    {
        move_dir = last_dir;
    }

    public void Continue()
    {
        GodMode( 3.0f );

        switch ( GameManager.instance.cur_mode )
        {
            case 0: break;
            case 1: hud.ContinueTimer();  break;
            case 2: star_count++; hud.AddStar(0); break;
        }

        GameManager.instance.pause = false;
    }

    public void GodMode( float time )
    {
        god_mode = time;
    }

    public void AddScore( uint score )
    {
        this.score += score;
    }

    public Vector2 ClosetIn( Vector3 enter_pos, Vector3 end_pos, Quaternion closet_rotation )
    {
        canControl = false;

        StopCoroutine("Movement");

        Vector2 direction = move_dir;

        move_dir = new Vector2(0,0);
        ChangeAnimation(STATES.IDLE);
        transform.rotation = closet_rotation;

        move_end = enter_pos;
        
        StartCoroutine(ClosetInWaite(end_pos));
        
        return direction;
    }

    IEnumerator ClosetInWaite(Vector3 end_pos)
    {
        float old_speed = speed;

        speed = 2;
        StartCoroutine("Movement");

        while (isMoving) { yield return null; }

        move_end = end_pos;
        StartCoroutine("Movement");

        while (isMoving) { yield return null; }

        speed = old_speed;

        yield return null;
    }

    public void ClosetOut( Vector2 pos, Vector2 direction )
    {
        move_end = pos;
        ChangeAnimation(STATES.WALK);
        StartCoroutine(ClosetOutWaite(direction));
    }

    IEnumerator ClosetOutWaite(Vector2 direction)
    {
        float old_speed = speed;
        speed = 1;

        StartCoroutine("Movement");

        while (isMoving) { yield return null; }

        speed = old_speed;
        move_dir = direction;

        canControl = true;
    }

    public bool UseItem( ItemIndex index )
    {
        if (!canControl) return false;

        switch (index)
        {
            case ItemIndex.Dust:
                if (box != null && helicopter != null) return false;
                SoundManager.instance.PlaySound("dust");
                bubble.ShowMessage(LanguageManager.GetText("SayDust"));
                SocialsPlatform.IncreaseAchievements(GameServicesClass.ACH_DIRTY, 1);
                Instantiate(DustPref, new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0), Quaternion.identity); break;
            case ItemIndex.Box:
                if (box != null || helicopter != null) return false;
                SoundManager.instance.PlaySound("box");
                bubble.ShowMessage(LanguageManager.GetText("SayBox"));
                SocialsPlatform.IncreaseAchievements(GameServicesClass.ACH_I_AM_SAFE, 1);
                boxHide = true;
                break;
            case ItemIndex.Helicopter:
                if (helicopter != null || box != null) return false;
                helicopter = Instantiate(HeliPref, transform, false) as GameObject;
                if (hud.IsSlow) hud.ToggleSlow();
                increase_speed += 0.5f;
                SoundManager.instance.PlaySound("helicopter");
                bubble.ShowMessage(LanguageManager.GetText("SayHelicopter"));
                SocialsPlatform.IncreaseAchievements(GameServicesClass.ACH_FLYING_CAT, 1);
                GodMode(4.0f);
                break;
            case ItemIndex.Voltage:
                SoundManager.instance.PlaySound("voltage");
                bubble.ShowMessage(LanguageManager.GetText("SayVoltage"));
                SocialsPlatform.IncreaseAchievements(GameServicesClass.ACH_AN_ELECTRICIAN, 1);
                foreach (Enemy enemy in Enemy.ENEMIES)
                {
                    enemy.Voltage(5.0f);
                }
                break;
            case ItemIndex.Speed:
                speedObject = Instantiate(SpeedPref);
                speedObject.transform.SetParent(transform,false);
                if (hud.IsSlow) hud.ToggleSlow();
                increase_speed += 2.0f;
                bubble.ShowMessage(LanguageManager.GetText("SaySpeed"));
                SocialsPlatform.IncreaseAchievements(GameServicesClass.ACH_FASTER_THAN_THE_WIND, 1);
                Invoke("DisableSpeed", 5.0f);
                break;
            case ItemIndex.Map:
                SoundManager.instance.PlaySound("map");
                bubble.ShowMessage(LanguageManager.GetText("SayMap"));
                SocialsPlatform.IncreaseAchievements(GameServicesClass.ACH_BIRDSEYE, 1);
                Camera.main.GetComponent<CameraFollow>().ChangeZoom(5.0f, 4.0f);
                break;
        }

        return true;
    }

    private void SwitchSlowOn()
    {
        canSlow = true;
    }

    private void DisableSpeed()
    {
        increase_speed -= 2.0f;
        Destroy(speedObject);
        speedObject = null;
    }

    public bool SwitchSlow() {
        if (increase_speed != 0 || floor_speed != 0 || speed == 0 || !canControl || !canSlow) return false;
        else
        {
            speed = hud.IsSlow ? 1.0f : 0.5f;
            return true;
        }
    }

    protected override void OnSpeedValuesChange()
    {
        animator.SetFloat("speed", CurrentSpeed());
        animator.SetFloat("floor_speed", floor_speed);
    }

    public void EndGame(string method_name)
    {
        ChangeAnimation(STATES.IDLE);
        GameManager.instance.pause = true;
        move_dir = new Vector2(0, 0);
        hud.Invoke(method_name, 1.0f);
    }

}