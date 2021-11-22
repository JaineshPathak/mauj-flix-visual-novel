using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

[CommandInfo("WrittingSpeed", "SetWrittingSpeed", "You can set the writting speed")]
public class WrittingSpeed : Command
{
    public float writtingSpeed;
    public Writer writer;

   
    public override void OnEnter()
    {
        writer.writingSpeed = writtingSpeed;
        Continue();
    }

    public override string GetSummary()
    {
        return "WrittingSpeed : " + writtingSpeed.ToString();
    }

    public override Color GetButtonColor()
    {
        return Color.cyan;
    }
}
