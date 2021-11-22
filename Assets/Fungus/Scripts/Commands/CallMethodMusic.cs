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

        protected override void CallTheMethod()
        {
            targetObject.SendMessage(methodName, musicIndex);
        }
    }
}