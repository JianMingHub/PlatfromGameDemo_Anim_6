using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.PlatfromGame
{
    public class Collectable : MonoBehaviour
    {
        public CollectableType type;
        public int minBonus;
        public int maxBonus;
        public AudioClip collisionSfx;
        public GameObject destroyVfxPb;

        protected int m_bonus;
        protected Player m_player;

        private void Start()
        {
            m_player = GameManager.Ins.player;

            if (!m_player) return;

            m_bonus = Random.Range(minBonus, maxBonus);

            Init();
        }

        public virtual void Init()
        {
            DestroyWhenLevelPassed();
        }

        protected virtual void TriggerHandle()
        {

        }

        protected void DestroyWhenLevelPassed()
        {
            // if (GameData.Ins.IsLevelPassed(LevelManager.Ins.CurLevelId))
            // {
            //     Destroy(gameObject);
            // }
        }

        public void Trigger()
        {
            TriggerHandle();

            if (destroyVfxPb)
            {
                Instantiate(destroyVfxPb, transform.position, Quaternion.identity);
            }
            
            Destroy(gameObject);

            AudioController.ins.PlaySound(collisionSfx);
        }
    }
}

