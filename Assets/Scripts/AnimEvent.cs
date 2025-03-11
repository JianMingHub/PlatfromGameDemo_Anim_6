using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.PlatfromGame
{
    public class AnimEvent : MonoBehaviour
    {
        public void HammerAttack()
        {
            CamShake.ins.ShakeTrigger(0.3f, 0.1f, 1);
            // AudioController.ins.PlaySound(AudioController.ins.attack);
        }

        // public void PlayFootStepSound()
        // {
        //     AudioController.ins.PlaySound(AudioController.ins.footStep);
        // }
    }
}