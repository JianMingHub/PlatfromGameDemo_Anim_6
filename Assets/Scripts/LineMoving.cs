using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.PlatfromGame
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class LineMoving : MonoBehaviour
    {
        public Direction moveDir;                   // Hướng di chuyển (Left, Right, Up, Down), kiểu enum
        public float movingDist;                    // Khoảng cách di chuyển từ vị trí ban đầu (độ dài quảng đường tối đa)
        public float speed;                         // Tốc độ di chuyển.
        public bool isOnlyUp;                       // Nếu `true`, đối tượng chỉ di chuyển lên (ngăn di chuyển xuống).
        public bool isAuto;                         // Nếu `true`, tự động di chuyển (nếu không, chỉ di chuyển khi gọi hàm Move).

        private Vector2 m_dest;                     // Điểm đích mà đối tượng sẽ di chuyển đến.
        private Vector3 m_backDir;                  // Hướng ngược lại từ vị trí hiện tại về vị trí ban đầu.
        private Vector3 m_startingPos;              // Vị trí ban đầu của đối tượng.
        private Rigidbody2D m_rb;                   // Tham chiếu đến Rigidbody2D của đối tượng.
        private bool m_isGizmHaveStartPos;

        public Vector2 Dest { get => m_dest;}
        public Vector3 BackDir { get => m_backDir;}

        private void Awake()
        {
            m_rb = GetComponent<Rigidbody2D>();             // Gắn Rigidbody2D vào `m_rb`.
            m_startingPos = transform.position;             // Lưu vị trí ban đầu của đối tượng.
        }        
        void Start()
        {
            GetMovingDest();                                // Tính toán điểm đích dựa trên `moveDir` và `movingDist`.
        }
        void Update()
        {
            m_backDir = m_startingPos - transform.position;         // Hướng từ vị trí hiện tại về vị trí ban đầu.
            m_backDir.Normalize();                                  // Chuẩn hóa hướng thành vector đơn vị (độ dài = 1).
        }
        private void FixedUpdate()
        {
            if (!isAuto) return;        // Nếu `isAuto` là `false`, không di chuyển.

            Move();                     // Thực hiện di chuyển.
            SwitchDirChecking();        // Kiểm tra và đổi hướng nếu cần.
        }
        // Tính toán điểm đích. Xác định vị trí điểm đến (m_dest)
        // sử dụng giá trị ban đầu của vị trí đối tượng (m_startingPos) để tính toán vị trí điểm đến (m_dest) theo hướng đã chỉ định.
        public void GetMovingDest()
        {
            switch (moveDir)
            {
                case Direction.Left:
                    // Di chuyển sang trái. Tọa độ x của điểm đến giảm đi movingDist trong khi tọa độ y giữ nguyên.
                    m_dest = new Vector2(m_startingPos.x - movingDist, transform.position.y);
                    break;
                case Direction.Right:
                    // Di chuyển sang phải. Tọa độ x của điểm đến tăng thêm movingDist trong khi tọa độ y giữ nguyên.
                    m_dest = new Vector2(m_startingPos.x + movingDist, transform.position.y);
                    break;
                case Direction.Up:
                    m_dest = new Vector2(transform.position.x, m_startingPos.y + movingDist);
                    break;
                case Direction.Down:
                    m_dest = new Vector2(transform.position.x, m_startingPos.y - movingDist);
                    break;
            }
        }
        // Kiểm tra xem đối tượng đã vượt quá khoảng cách đến điểm đến (m_dest) hay chưa.
        public bool IsReached()
        {
            float dist1 = Vector2.Distance(m_startingPos, transform.position);      // Tính khoảng cách từ vị trí hiện tại đến vị trí bắt đầu (dist1).
            float dist2 = Vector2.Distance(m_startingPos, m_dest);                  // Tính khoảng cách từ vị trí bắt đầu đến điểm đến (dist2).
            return dist1 > dist2;       // Trả về `true` nếu đối tượng đã vượt quá khoảng cách tới điểm đích (dist1 > dist2). Ngược lại, trả về false
        }
        // Thay đổi hướng di chuyển (moveDir) của đối tượng dựa trên hướng vector (dir) truyền vào.
        public void SwitchDir(Vector2 dir)
        {
            // Nếu hướng hiện tại là trái hoặc phải
            if(moveDir == Direction.Left || moveDir == Direction.Right)
            {
                // Nếu dir.x < 0, đổi hướng sang Left.
                // Nếu dir.x >= 0, đổi hướng sang Right.
                moveDir = dir.x < 0 ? Direction.Left : Direction.Right;         
            }
            // Nếu hướng hiện tại là trên hoặc dưới
            else if(moveDir == Direction.Up || moveDir == Direction.Down)
            {
                // Nếu dir.y < 0, đổi hướng sang Down.
                // Nếu dir.y >= 0, đổi hướng sang Up.
                moveDir = dir.y < 0 ? Direction.Down : Direction.Up;
            }
        }
        // Kiểm tra và đổi hướng nếu đối tượng đã vượt qua điểm đích.
        public void SwitchDirChecking()
        {
            if (IsReached())                // Nếu đã vượt qua điểm đích.
            {
                SwitchDir(m_backDir);       // Đổi hướng dựa trên `m_backDir`.
                GetMovingDest();            // Tính toán lại điểm đích mới.
            }
        }
        // Thực hiện di chuyển
        public void Move()
        {
            switch (moveDir)
            {
                case Direction.Left:
                    m_rb.velocity = new Vector2(-speed, m_rb.velocity.y);       // Di chuyển sang trái.
                    break;
                case Direction.Right:
                    m_rb.velocity = new Vector2(speed, m_rb.velocity.y);        // Di chuyển sang phải.
                    break;
                case Direction.Up:
                    m_rb.velocity = new Vector2(m_rb.velocity.x, speed);        // Di chuyển lên.
                    transform.position = new Vector2(m_startingPos.x, transform.position.y);        // Giữ x cố định.
                    break;
                case Direction.Down:
                    transform.position = new Vector2(m_startingPos.x, transform.position.y);        // Giữ x cố định.
                    
                    if (isOnlyUp) return;       // Nếu chỉ cho phép lên, không di chuyển xuống.
                    m_rb.velocity = new Vector2(m_rb.velocity.x, -speed);       // Di chuyển xuống.
                    break;
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            if (!m_isGizmHaveStartPos)
            {
                GetMovingDest();
                m_isGizmHaveStartPos = true;
            }
            Gizmos.DrawLine(transform.position, m_dest);
        }
    }
}