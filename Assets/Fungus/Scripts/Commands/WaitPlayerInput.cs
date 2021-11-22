using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Flow",
                 "Wait For Click",
                 "Waits until Player Clicks.")]
    [AddComponentMenu("")]
    public class WaitPlayerInput : Command
    {
        public override void OnEnter()
        {
            StartCoroutine("WaitInputRoutine");
        }

        private IEnumerator WaitInputRoutine()
        {
            bool isOn = true;
            
            while(isOn)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    isOn = false;
                    StopCoroutine("WaitInputRoutine");                    
                    Continue();
                }

                yield return null;
            }
        }

        public override Color GetButtonColor()
        {
            return Color.white;
        }
    }
}