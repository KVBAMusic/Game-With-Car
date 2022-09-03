using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerBehaviour : MonoBehaviour
{
    public Timer timer;
    void Start()
    {
        timer = new(0f, Timer.TimerMode.CountUp);
    }

    void Update()
    {
        timer.Tick(Time.deltaTime);
    }
}
