using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;

namespace UDEV.PlatfromGame
{
    public class Enemy : Actor
    {
        [Header("Moving Enemy Script: ")]
        public float movingDist;                        // Khoảng cách mà kẻ địch sẽ di chuyển qua lại từ vị trí ban đầu

        protected PlayerDectect m_playerDetect;         // Dùng để phát hiện người chơi (sử dụng một script riêng PlayerDectect).
        protected EnemyStat m_curStat;                  // Dùng để lưu trạng thái hiện tại của kẻ địch (sử dụng một script riêng EnemyStat).
        protected Vector2 m_movingDir;                  // Hướng di chuyển hiện tại.
        protected Vector2 m_movingDirBackup;            // Lưu trữ hướng di chuyển trước đó (dùng để phục hồi khi cần)
        protected Vector2 m_startingPos;                // Vị trí ban đầu của kẻ địch khi spawn.
        protected Vector2 m_targetDir;                  // Hướng đến mục tiêu (người chơi).
        protected StateMachine<EnemyAnimState> m_fsm;   // FSM (Máy trạng thái hữu hạn) để quản lý trạng thái hoạt hình (vd: Moving, Chasing, Dead).

        // Thuộc tính chỉ đọc (read-only) kiểm tra xem trạng thái hiện tại của FSM có phải là Dead không.
        public bool IsDead
        {
            get => m_fsm.State == EnemyAnimState.Dead;
        }
        // Hàm khởi tạo
        protected override void Awake()
        {
            base.Awake();                                       // Gọi hàm Awake() từ lớp cha Actor
            m_playerDetect = GetComponent<PlayerDectect>();     // Gán script PlayerDectect cho biến m_playerDetect.
            m_startingPos = transform.position;                 // Lưu vị trí ban đầu của kẻ địch vào m_startingPos.
            // FSM_MethodGen.Gen<EnemyAnimState>();
        }
        // Khởi tạo FSM với trạng thái mặc định là Moving.
        protected void FSMInit(MonoBehaviour behav)
        {
            // Khởi tạo FSM với đối tượng `behav` (trong trường hợp này là chính đối tượng kẻ địch).
            // `StateMachine<EnemyAnimState>` là một lớp generic quản lý các trạng thái của kẻ địch.
            // `EnemyAnimState` là một enum định nghĩa các trạng thái khác nhau của kẻ địch (vd: Moving, Chasing, Dead).
            m_fsm = StateMachine<EnemyAnimState>.Initialize(behav);
            // Chuyển trạng thái hiện tại của FSM sang trạng thái `Moving`.
            // Điều này có nghĩa là kẻ địch sẽ bắt đầu ở trạng thái di chuyển khi được khởi tạo.
            m_fsm.ChangeState(EnemyAnimState.Moving);
        }
        protected override void Init()
        {
            // Nếu stat (trạng thái cơ bản) không null, gán nó cho m_curStat (dạng EnemyStat).
            if (stat != null)
            {
                m_curStat = (EnemyStat)stat;
                // Debug.Log("m_curStat has been initialized: " + m_curStat);
            }
        }
        protected virtual void Update()
        {
            // Nếu kẻ địch đã chết, chuyển trạng thái FSM sang Dead.
            if (IsDead)
            {
                m_fsm.ChangeState(EnemyAnimState.Dead);
            }

            // Nếu bị knockback hoặc đã chết, thoát hàm.
            if (m_isKnockBack || IsDead) return;

            // Nếu phát hiện ra player(người chơi) thì chuyển sang trạng thái Chasing (đuổi theo)
            if (m_playerDetect.IsDetected)
            {
                m_fsm.ChangeState(EnemyAnimState.Chasing);
            }
            // Nếu không phát hiện, chuyển sang Moving
            else
            {
                m_fsm.ChangeState(EnemyAnimState.Moving);
            }
            // Nếu tốc độ rơi (trục Y) <= -50, gọi hàm Dead() để xử lý cái chết.
            if(m_rb.velocity.y <= -50)
            {
                Dead();
            }
        }
        protected virtual void FixedUpdate()
        {
            if (m_isKnockBack || IsDead) return;
            // Gọi hàm Move() để xử lý logic di chuyển trong FixedUpdate (thích hợp cho vật lý).
            Move();
        }
        protected override void Dead()
        {
            base.Dead();
            m_fsm.ChangeState(EnemyAnimState.Dead);
        }
        // Lấy ra hướng mục tiêu (Player)
        protected void GetTargetDir()
        {
            // vị trí vủa mục tiêu - vị trí của enemy
            m_targetDir = m_playerDetect.Target.transform.position - transform.position;
            m_targetDir.Normalize();    // về tỷ lệ 0-1
        }
        public virtual void Move()
        {

        }
        // Xử lý việc kẻ địch nhận sát thương
        public override void TakeDamage(int dmg, Actor whoHit = null)
        {
            if (IsDead) return;                     // Nếu đã chết, không xử lý.
            base.TakeDamage(dmg, whoHit);           // Gọi hàm TakeDamage từ lớp cha Actor
            // Nếu kẻ địch vẫn còn máu sau khi nhận sát thương (m_curHp>0) và ko đang ở trạng thái bất bại
            if(m_curHp > 0 && !m_isInvincible)
            {
                // chuyển trạng thái FSM sang GotHit (trạng thái kẻ địch bị đánh).
                m_fsm.ChangeState(EnemyAnimState.GotHit);
            }
        }
        #region FSM
            protected virtual void Moving_Enter() { }
            protected virtual void Moving_Update() {
                m_curSpeed = m_curStat.moveSpeed;
                Helper.PlayAnim(m_anim, EnemyAnimState.Moving.ToString());
            }
            protected virtual void Moving_Exit() { }
            protected virtual void Chasing_Enter() { }
            protected virtual void Chasing_Update() {
                m_curSpeed = m_curStat.chasingSpeed;
                Helper.PlayAnim(m_anim, EnemyAnimState.Chasing.ToString());
            }
            protected virtual void Chasing_Exit() { }
            protected virtual void GotHit_Enter() { }
            protected virtual void GotHit_Update() { }
            protected virtual void GotHit_Exit() { }
            protected virtual void Dead_Enter() {
                if (deadVfxPb)
                {
                    Instantiate(deadVfxPb, transform.position, Quaternion.identity);
                }
                gameObject.SetActive(false);    // ẩn con enemy đi
                // AudioController.ins.PlaySound(AudioController.ins.enemyDead);
            }
            protected virtual void Dead_Update() { }
            protected virtual void Dead_Exit() { }
        #endregion
    }
}