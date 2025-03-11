using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.PlatfromGame
{
    public class FireBullet : MonoBehaviour
    {
        public Player player;                       // Tham chiếu đến đối tượng người chơi
        public Transform firePoint;                 // Vị trí bắn đạn
        public Bullet bulletPb;                     // Prefab viên đạn

        private float m_curSpeed;                   // Tốc độ hiện tại của viên đạn

        public void Fire()
        {
            if (!bulletPb || !player || !firePoint) return;     // Nếu không có prefab viên đạn hoặc người chơi hoặc vị trí bắn, thoát hàm.

            // Lấy tốc độ của viên đạn dựa vào hướng mà người chơi đang nhìn.
            m_curSpeed = player.IsFacingLeft ? -bulletPb.speed : bulletPb.speed;
            // Tạo một viên đạn mới từ prefab.     
            var bulletClone = Instantiate(bulletPb, firePoint.position, Quaternion.identity);       
            bulletClone.speed = m_curSpeed;     // Gán tốc độ cho viên đạn.
            bulletClone.owner = player;         // Gán người chơi là chủ sở hữu của viên đạn.
            // GameManager.Ins.ReduceBullet();

            // AudioController.ins.PlaySound(AudioController.ins.fireBullet);      // Phát âm thanh bắn đạn.
        }
    }
}