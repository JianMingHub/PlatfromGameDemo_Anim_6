using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.PlatfromGame
{
    public class Bullet : MonoBehaviour
    {
        public float speed;                     // Tốc độ di chuyển của đạn
        public LayerMask targetLayer;           // Layer của đối tượng mà đạn sẽ va chạm
        private Vector3 m_prevPos;              // Vị trí trước đó của đạn

        [HideInInspector]
        public Actor owner;                     // đối tượng sở hữu viên đạn

        // 
        private void Awake()
        {
            m_prevPos = transform.position;         // Lưu vị trí hiện tại của đạn
        }

        private void Update()
        {
            // Di chuyển đạn theo hướng của nó và tốc độ đã định sẵn (speed)
            transform.Translate(transform.right * speed * Time.deltaTime, Space.World);
        }
        // Xử lý va chạm của đạn với các đối tượng khác
        private void FixedUpdate()
        {
            // Tính hướng di chuyển của đạn
            Vector2 dir = (Vector2)(transform.position - m_prevPos);
            float dist = dir.magnitude;         // khoảng cách di chuyển
            RaycastHit2D hit = Physics2D.Raycast(m_prevPos, dir, dist, targetLayer);        // Kiểm tra va chạm với các đối tượng trong targetLayer
            if(hit && hit.collider)        // Nếu có va chạm trong targetLayer 
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();       // Lấy ra thành phần Enemy từ đối tượng va chạm
                if (enemy)    // Nếu đối tượng là kẻ địch
                {
                    enemy.TakeDamage(owner.stat.damage, owner);         //  Gây sát thương cho kẻ thù bằng sát thương của chủ sở hữu viên đạn.
                }
                gameObject.SetActive(false);        // Vô hiệu hóa viên đạn sau khi va chạm
            }

            m_prevPos = transform.position;         // Cập nhật lại vị trí trước đó của viên đạn
        }
    }
}