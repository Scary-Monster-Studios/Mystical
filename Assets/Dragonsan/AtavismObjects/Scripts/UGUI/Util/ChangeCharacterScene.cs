using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Atavism
{
    public class ChangeCharacterScene : MonoBehaviour
    {

        public void ChangeScene(string sceneName)
        {
            //		Application.LoadLevel(sceneName);
            SceneManager.LoadScene(sceneName);

        }
    }
}