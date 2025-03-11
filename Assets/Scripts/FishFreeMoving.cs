using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.PlatfromGame
{
    public class FishFreeMoving : FreeMovingEnemy
    {
        protected override void Update()
        {
            if (!GameManager.Ins.player.obstacleChker.IsOnWater)
            {
                m_fsm.ChangeState(EnemyAnimState.Moving);
                return;
            }

            base.Update();
        }
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            
            if (!GameManager.Ins.player.obstacleChker.IsOnWater)
            {
                m_fsm.ChangeState(EnemyAnimState.Moving);
            }
        }
    }
}

