using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.PlatfromGame
{
    public class FreeMovingEnemy : Enemy
    {
        public bool canRotate;      // Biến boolean để xác định liệu kẻ địch có thể xoay hay không.
        private float m_movePosXR;  // Vị trí di chuyển tối đa theo trục X về phía phải.
        private float m_movePosXL;  // Vị trí di chuyển tối đa theo trục X về phía trái.
        private float m_movePosYT;  // Vị trí di chuyển tối đa theo trục Y về phía trên.
        private float m_movePosYD;  // Vị trí di chuyển tối đa theo trục Y về phía dưới.

        private bool m_haveMovingPos; // Biến boolean để kiểm tra xem đã có vị trí di chuyển hay chưa.
        private Vector2 m_movingPos; // Vị trí di chuyển hiện tại.
        protected override void Awake()
        {
            base.Awake();       // Gọi phương thức Awake của lớp cha (Enemy).
            FSMInit(this);      // Khởi tạo Finite State Machine (FSM) cho đối tượng này.
        }
        protected override void Update()
        {
            base.Update();      // Gọi phương thức Update của lớp cha (Enemy).
            GetTargetDir();     // Lấy hướng đến mục tiêu.
        }
        // Tìm tọa độ điểm đích
        public void FindMaxMovePos()
        {
            m_movePosXR = m_startingPos.x + movingDist;         // Tính vị trí di chuyển tối đa theo trục X về phía phải.
            m_movePosXL = m_startingPos.x - movingDist;         // Tính vị trí di chuyển tối đa theo trục X về phía trái.
            m_movePosYT = m_startingPos.y + movingDist;         // Tính vị trí di chuyển tối đa theo trục Y về phía trên.
            m_movePosYD = m_startingPos.y - movingDist;         // Tính vị trí di chuyển tối đa theo trục Y về phía dưới.
        }
        public override void Move()
        {
            if (m_isKnockBack) return; // Nếu đang bị đẩy lùi, không thực hiện di chuyển.

            // Nếu kẻ địch chưa có vị trí di chuyển
            if (!m_haveMovingPos)
            {
                float randPosX = Random.Range(m_movePosXL, m_movePosXR); // Tạo vị trí ngẫu nhiên theo trục X trong phạm vi cho phép.
                float randPosY = Random.Range(m_movePosYD, m_movePosYT); // Tạo vị trí ngẫu nhiên theo trục Y trong phạm vi cho phép.

                m_movingPos = new Vector2(randPosX, randPosY); // Gán vị trí di chuyển mới.
                m_movingDir = (m_movingPos - (Vector2)transform.position).normalized; // Tính hướng di chuyển hiện tại

                m_movingDirBackup = m_movingDir; // Lưu lại hướng di chuyển hiện tại.
                m_haveMovingPos = true; // Đánh dấu đã có vị trí di chuyển.
            }

            float angle = 0f;       // góc quay

            // Nếu kẻ địch có thể xoay
            if (canRotate)
            {
                angle = Mathf.Atan2(m_movingDir.y, m_movingDir.x) * Mathf.Rad2Deg; // Tính góc xoay dựa trên hướng di chuyển.
            }

            // Nếu hướng di chuyển theo trục X là dương (di chuyển sang phải)
            if (m_movingDir.x > 0)
            {
                // Nếu có thể xoay, giới hạn góc xoay và xoay đối tượng.
                if (canRotate)
                {
                    angle = Mathf.Clamp(angle, -41, 41);                    // Giới hạn góc xoay.
                    transform.rotation = Quaternion.Euler(0f, 0f, angle);   // Xoay đối tượng.
                }
                Flip(Direction.Right);      // Lật đối tượng sang phải.
            }
            // Nếu hướng di chuyển theo trục X là âm (di chuyển sang trái)
            else if (m_movingDir.x < 0)
            {
                // Nếu có thể xoay, tính góc xoay mới, giới hạn góc xoay và xoay đối tượng.
                if (canRotate)
                {
                    float newAngle = angle + 180f;                              // Tính góc xoay mới.
                    newAngle = Mathf.Clamp(newAngle, 25, 325);                  // Giới hạn góc xoay.
                    transform.rotation = Quaternion.Euler(0f, 0f, newAngle);    // Xoay đối tượng.
                }
                Flip(Direction.Left);       // Lật đối tượng sang trái.
            }

            DestReachedChecking();          // Kiểm tra xem đã đến đích chưa.
        }
        private void DestReachedChecking()
        {
            if(Vector2.Distance(transform.position, m_movingPos) <= 0.5f)
            {
                m_haveMovingPos = false;        // Nếu đã đến đích, đánh dấu chưa có vị trí di chuyển.
            }
            else
            {
                m_rb.velocity = m_movingDir * m_curSpeed;   // Di chuyển đối tượng theo hướng và tốc độ hiện tại.
            }
        }
        #region FSM
            protected override void Moving_Enter()
            {
                base.Moving_Enter();            // Gọi phương thức Moving_Enter của lớp cha (Enemy).
                m_haveMovingPos = false;        // Đánh dấu chưa có vị trí di chuyển.
                FindMaxMovePos();               // Tìm vị trí di chuyển tối đa.
            }

            protected override void Chasing_Update()
            {
                base.Chasing_Update();          // Gọi phương thức Chasing_Update của lớp cha (Enemy).
                m_movingDir = m_targetDir;      // Cập nhật hướng di chuyển theo hướng mục tiêu.
            }

            protected override void GotHit_Update()
            {
                if (m_isKnockBack)
                {
                    KnockBackMove(m_targetDir.y); // Di chuyển khi bị đẩy lùi.
                }else
                {
                    m_fsm.ChangeState(EnemyAnimState.Moving); // Chuyển trạng thái FSM sang Moving.
                }
            }
        #endregion
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, new Vector3(
                transform.position.x + movingDist, transform.position.y, transform.position.z
                )); // Vẽ đường gizmo màu vàng từ vị trí hiện tại đến vị trí di chuyển tối đa theo trục X về phía phải.

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, new Vector3(
                transform.position.x - movingDist, transform.position.y, transform.position.z
                )); // Vẽ đường gizmo màu vàng từ vị trí hiện tại đến vị trí di chuyển tối đa theo trục X về phía trái.

            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, new Vector3(
                transform.position.x, transform.position.y + movingDist, transform.position.z
                )); // Vẽ đường gizmo màu trắng từ vị trí hiện tại đến vị trí di chuyển tối đa theo trục Y về phía trên.

            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, new Vector3(
                transform.position.x, transform.position.y - movingDist, transform.position.z
                )); // Vẽ đường gizmo màu trắng từ vị trí hiện tại đến vị trí di chuyển tối đa theo trục Y về phía dưới.
        }
    }
}