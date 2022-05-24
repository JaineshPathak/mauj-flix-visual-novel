using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

namespace Fungus
{
    [CommandInfo("Scripting",
                 "Call Method Music",
                 "Calls a named method on a GameObject using the GameObject.SendMessage() system with index for playing music.")]
    public class CallMethodMusic : CallMethod
    {
        [SerializeField] private int musicIndex;

        public void CallMethodMusicInit(GameObject TargetObject, string MethodName, float newDelay, int MIndex)
        {
            targetObject = TargetObject;
            methodName = MethodName;
            delay = newDelay;
            musicIndex = MIndex;
        }

        protected override void CallTheMethod()
        {
            targetObject.SendMessage(methodName, musicIndex);
        }

        public override Color GetButtonColor()
        {
            //return new Color32(235, 191, 217, 255);
            return Color.green;
        }
    }
}