using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeUtils
{
    public static string GetTimeInFormat(float time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);

        return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }
}
