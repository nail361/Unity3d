using UnityEngine;
using System;
using System.Collections.Generic;

public delegate void finish_callback_del();
public delegate void tick_callback_del();

public class Timers : MonoBehaviour {

    private static bool isRunning = false;

    private static List<Timer> timers = new List<Timer>();

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public static void AddTimer(string timer_name, int start_time, int stop_time, int update_time = 1000, tick_callback_del on_timer_tick = null, finish_callback_del on_timer_finish = null)
    {
        if (timers.Exists(x => x.timer_name == timer_name)) return;
        Timer timer = new Timer();
        timer.timer_name = timer_name;
        timer.start_time = start_time;
        timer.stop_time = stop_time;
        timer.delta_time = start_time > stop_time ? -1000 : 1000;
        timer.update_time = update_time;
        timer.infinity = (start_time == stop_time);
        timer.Init();

        if (on_timer_finish != null) timer.finish_callback = new finish_callback_del(on_timer_finish);
        if (on_timer_tick != null) timer.tick_callback = new tick_callback_del(on_timer_tick);

        timers.Add(timer);

        isRunning = true;
    }

    public static bool ShiftStopTime(string timer_name, int value)
    {
        for (var i = 0; i < timers.Count; i++)
        {
            if (timers[i].timer_name == timer_name)
            {
                timers[i].stop_time += value;
                return true;
            }
        }

        return false;
    }

    public static bool RemoveTimer(string timer_name)
    {
        for (var i = 0; i < timers.Count; i++)
        {
            if (timers[i].timer_name == timer_name)
            {
                timers.RemoveAt(i);
                if (timers.Count == 0) Stop();
                return true;
            }
        }

        return false;
    }

    void Update()
    {
        if (!isRunning) return;

        for (int i = 0; i < timers.Count; i++)
        {
            if (timers[i].stop) continue;

            timers[i].TimerUpdate(Time.deltaTime);
        }
    }

    private static void Stop()
    {
        isRunning = false;
    }

    public static bool StopTimer(string timer_name, bool stop)
    {
        for (var i = 0; i < timers.Count; i++)
        {
            if (timers[i].timer_name == timer_name)
            {
                timers[i].stop = stop;
                return true;
            }
        }

        return false;
    }

    public static int GetTime(string timer_name)
    {
        for (var i = 0; i < timers.Count; i++)
        {
            if (timers[i].timer_name == timer_name)
            {
                return (int)Mathf.Floor(timers[i].cur_time);
            }
        }

        return 0;
    }

    public static int GetElapsedTime(string timer_name)
    {
        for (var i = 0; i < timers.Count; i++)
        {
            if (timers[i].timer_name == timer_name)
            {
                return (int)Mathf.Floor(timers[i].elapsed_time);
            }
        }

        return 0;
    }

    public static bool HasTimer( string timer_name )
    {
        for (var i = 0; i < timers.Count; i++)
            if (timers[i].timer_name == timer_name) return true;

        return false;
    }
}

[Serializable]
class Timer
{
    public finish_callback_del finish_callback;
    public tick_callback_del tick_callback;

    public string timer_name = "";
    public bool stop = false;
    public int start_time;
    public int stop_time;
    public int delta_time;
    public int update_time;
    public float cur_time;
    public float elapsed_time = 0;
    public bool infinity = false;

    private float cur_update_time;

    public void Init()
    {
        cur_time = start_time;
    }

    public void TimerUpdate( float unity_delta_time )
    {

        elapsed_time += (1000*unity_delta_time);
        cur_update_time += (1000 * unity_delta_time);
        cur_time += (delta_time * unity_delta_time);

        if (tick_callback != null & cur_update_time > update_time)
        {
            cur_update_time = 0;
            tick_callback();
        }

        if (infinity) return;

        if (delta_time > 0)
        {
            if (cur_time > stop_time) cur_time = stop_time;
        }
        else
            if (cur_time < stop_time) cur_time = stop_time;

        if (tick_callback != null) tick_callback();

        if (cur_time == stop_time)
        {
            Timers.RemoveTimer(timer_name);
            if (finish_callback != null) finish_callback();
        }
    }

}