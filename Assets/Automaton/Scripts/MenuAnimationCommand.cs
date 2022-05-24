using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus 
{
    [CommandInfo("MenuAnimation",
                "MoveUpthemenu",
                "makeMenuGoUp")]
    public class MenuAnimationCommand : Command
    {
        [Tooltip("Moving Speed of animation")]
        [SerializeField] protected float speed = 1200f;

        [Tooltip("Put the main Script here")]
        public MenuAnimation menuAnimation;

        public override void OnEnter()
        {
            menuAnimation.enabled = true;
            menuAnimation.isMoving = true;
            menuAnimation.speed = speed;
            Continue();
        }
        public override Color GetButtonColor()
        {
            return Color.gray;
        }
    }
}


