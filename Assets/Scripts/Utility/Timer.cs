using System;

public class Timer
{
    public enum TimerMode { CountDown, CountUp };

    public TimerMode timerMode { get; private set; }
    public float time { get; private set; }
    public bool isRunning { get; private set; }

    public event EventHandler OnTimerStart;
    public event EventHandler OnTimerStop;

    public Timer(float time)
    {
        this.time = time;
        timerMode = TimerMode.CountDown;
        isRunning = false;
    }

    public Timer (float time, TimerMode mode)
    {
        this.time = time;
        timerMode = mode;
        isRunning = false;
    }

    public void Start()
    {
        isRunning = true;
        OnTimerStart?.Invoke(this, EventArgs.Empty);
    }

    public void Stop()
    {
        isRunning = false;
        OnTimerStop?.Invoke(this, EventArgs.Empty);
    }

    public void SetTime(float time)
    {
        this.time = time;
    }

    public void Tick(float deltaTime)
    {
        if (isRunning)
        {
            if (timerMode == TimerMode.CountDown)
            {
                time -= deltaTime;
                if (time < 0)
                {
                    time = 0;
                    Stop();
                    return;
                }
            }
            else
            {
                time += deltaTime;
            }
        }
    }
}
