using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.PlatfromGame
{
    [RequireComponent(typeof(LineMoving))]          // Required LineMoving
    public class LineMovingEnemy : Enemy
    {
        private LineMoving lineMoving;

        protected override void Awake()
        {
            base.Awake();
            lineMoving = GetComponent<LineMoving>();
            FSMInit(this);
        }

        public override void Start()
        {
            base.Start();
            // khoảng đường di chuyển của LineMoving bằng khoảng đường di chuyển của enemy
            movingDist = lineMoving.movingDist;
        }

        public override void Move()
        {
            if (m_isKnockBack) return;

            lineMoving.Move();
            Flip(lineMoving.moveDir);
        }
        #region FSM
            protected override void Moving_Update()
            {
                base.Moving_Update();
                // hướng mục tiêu bằng với hướng ngược lại của lineMoving
                m_targetDir = lineMoving.BackDir;
                // cập nhật lại speed
                lineMoving.speed = m_curSpeed;
                // gọi SwitchDirChecking()
                lineMoving.SwitchDirChecking();
            }
            // đuổi theo player
            protected override void Chasing_Enter()
            {
                base.Chasing_Enter();
                // lấy ra hướng của player
                GetTargetDir();
                // thay đổi về hướng player
                lineMoving.SwitchDir(m_targetDir);
            }

            protected override void Chasing_Update()
            {
                base.Chasing_Update();
                // lấy ra hướng của player
                GetTargetDir();
                // cập nhật lại speed
                lineMoving.speed = m_curSpeed;
            }

            protected override void Chasing_Exit()
            {
                base.Chasing_Exit();
                // gọi SwitchDirChecking() thay đổi hướng cho con enemy lại
                lineMoving.SwitchDirChecking();
            }

            protected override void GotHit_Update()
            {
                base.GotHit_Update();
                // gọi SwitchDirChecking() thay đổi hướng cho con enemy lại
                lineMoving.SwitchDirChecking();
                // lấy hướng đến player
                GetTargetDir();
                // nếu đang bị đẩy lùi
                if (m_isKnockBack)
                {
                    KnockBackMove(0.55f);
                }else
                {
                    // chuyển sang trạng thái moving
                    m_fsm.ChangeState(EnemyAnimState.Moving);
                }
            }
        #endregion
    }
}