using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.PlatfromGame
{
    public class PlayerDectect : MonoBehaviour
    {
        public bool disable;                    // on/off target
        public DetectMethod detectMethod;       // xác định phương pháp phát hiện mục tiêu bằng RayCast/CircleOverlap
        public LayerMask targetLayer;           // xác định lớp (layer) mà đối tượng mục tiêu thuộc về
        public float detectDist;                // phạm vi phát hiện mục tiêu, chiều dài raycast hoặc bán kính của OverlapCircle.

        private Player m_target;                // tham chiếu đến đối tượng mục tiêu (Player)
        private Vector2 m_dirToTarget;          // lưu trữ hướng từ đối tượng chứa script này (PlayerDetect) đến đối tượng mục tiêu (Player) mà nó phát hiện.
        private bool m_isDetected;              // true: mục tiêu được phát hiện, false: không phát hiện mục tiêu (lưu trạng thái phát hiện mục tiêu)

        public Player Target { get => m_target; }
        public Vector2 DirTarget { get => m_dirToTarget; }
        public bool IsDetected { get => m_isDetected; }

        private void Start()
        {
            m_target = GameManager.Ins.player;   // đối tượng mục tiêu (Player) được lấy từ singleton GameManager
        }

        private void FixedUpdate()
        {
            if (!m_target || disable) return;

            if(detectMethod == DetectMethod.RayCast)
            {
                // Tính hướng = vị trí của mục tiêu - vị trí hiện tại của enemy
                m_dirToTarget = m_target.transform.position - transform.position;
                m_dirToTarget.Normalize();  // về tỷ lệ 0-1

                // Tạo một tia ray từ vị trí hiện tại của đối tượng (transform.position) theo hướng m_dirToTarget (chỉ lấy thành phần x).
                // Tia ray có chiều dài detectDist và chỉ phát hiện đối tượng trong lớp targetLayer.
                RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(m_dirToTarget.x, 0), detectDist, targetLayer);

                m_isDetected = hit.collider != null;    // Nếu tia ray chạm một collider, m_isDetected là true. Ngược lại, là false.

            }else if(detectMethod == DetectMethod.CircleOverlap)
            {
                // Vẽ một hình tròn với tâm tại transform.position và bán kính detectDist
                // Chỉ phát hiện các đối tượng trong lớp targetLayer
                Collider2D col = Physics2D.OverlapCircle(transform.position, detectDist, targetLayer);
                m_isDetected = col != null;         // Nếu có một collider nằm trong vùng tròn, m_isDetected là true. Ngược lại, là false.
            }

            // if (m_isDetected)
            // {
            //     Debug.Log("Player was detected!.");
            // }
            // else
            // {
            //     Debug.Log("Player not detected!.");
            // }
        }

        private void OnDrawGizmos()
        {
            if(detectMethod == DetectMethod.RayCast)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position,
                    new Vector3(
                        transform.position.x + detectDist,
                        transform.position.y, transform.position.z
                    ));
            }else if(detectMethod == DetectMethod.CircleOverlap)
            {
                Gizmos.color = Helper.ChangAlpha(Color.green, 0.2f);
                Gizmos.DrawSphere(transform.position, detectDist);
            }
        }
    }
}

