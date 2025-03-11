using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.PlatfromGame
{
    public class GameManager : Singleton<GameManager>
    {
        public GamePlaySetting setting;
        public Player player;
        public FreeParallax map;
        public override void Awake()
        {
            MakeSingleton(false);
        }
        public void SetMapSpeed (float speed) 
        {
            if (map)
            {
                map.Speed = speed;
            }
        }
    }
}

