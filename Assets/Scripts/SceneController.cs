using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UDEV.PlatfromGame
{
    public class SceneController : Singleton<SceneController>
    {
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void LoadLevelScene(int level)
        {
            string sceneName = GameScene.Level_.ToString() + level; //Level_1, Level_2
            LoadScene(sceneName);
        }
    }
}
