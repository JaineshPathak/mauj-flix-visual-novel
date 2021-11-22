using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOnScene : MonoBehaviour
{
    public float startTime;
    public float endTime;

    public float finalTime;

    private void OnApplicationQuit()
    {
        endTime = Time.time;
        print("End Time: " + endTime);

        finalTime = endTime - startTime;

        print("Final Time: " + finalTime);
        print("Time since Scene: " + FormatTime(Time.timeSinceLevelLoad));
    }

    private void Start()
    {
        startTime = Time.time;
        print("Start Time:" + startTime);
    }

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60000;
        int seconds = (int)time / 1000 - 60 * minutes;
        int milliseconds = (int)time - minutes * 60000 - 1000 * seconds;
        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }
}
