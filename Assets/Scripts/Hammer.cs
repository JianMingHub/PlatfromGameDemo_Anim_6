using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.PlatfromGame
{
    public class Hammer : MonoBehaviour
    {
        public LayerMask enemyLayer;            // Layer của enemy
        public float atkRadius;                 // Bán kính tấn công của búa
        public Vector3 offset;                  // Vị trí tấn công của búa
        [SerializeField]
        private Player m_player;                // Tham chiếu đến đối tượng người chơi, được gán trong Unity Editor.

        // Phương thức để thực hiện hành động tấn công.
        public void Attack()
        {
            if (m_player == null) return;       // Nếu không có người chơi, thoát hàm.

            //  Tạo một vòng tròn để kiểm tra va chạm với kẻ thù trong bán kính tấn công.
            Collider2D col = Physics2D.OverlapCircle(transform.position + offset, atkRadius, enemyLayer);

            // Nếu có đối tượng va chạm trong vòng tròn.
            if (col)
            {
                // Lấy thành phần Enemy từ đối tượng va chạm.
                Enemy enemy = col.gameObject.GetComponent<Enemy>();

                // Nếu đối tượng là kẻ thù.
                if (enemy)
                {
                    // Gây sát thương cho kẻ thù bằng sát thương của người chơi.
                    enemy.TakeDamage(m_player.stat.damage, m_player);
                }
            }
        }
        // Lật vị trí vùng gây sát thương
        private void Update()
        {
            if(m_player == null) return;                // Kiểm tra nếu người chơi không tồn tại, thoát khỏi phương thức.

            // Kiểm tra nếu người chơi đang quay mặt sang phải.
            if(m_player.transform.localScale.x > 0)
            {
                // Nếu vị trí tấn công của búa là số âm, chuyển nó thành số dương.
                if(offset.x < 0)
                {
                    offset = new Vector3(offset.x * -1, offset.y, offset.z);
                }
            }
            // Kiểm tra nếu người chơi đang quay mặt sang trái.
            else if(m_player.transform.localScale.x < 0)
            {
                // Nếu vị trí tấn công của búa là số dương, chuyển nó thành số âm.
                if (offset.x > 0)
                {
                    offset = new Vector3(offset.x * -1, offset.y, offset.z);
                }
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Helper.ChangAlpha(Color.yellow, 10.2f);
            Gizmos.DrawSphere(transform.position + offset, atkRadius);
        }
    }
}