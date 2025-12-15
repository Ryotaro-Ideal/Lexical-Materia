using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadScene : MonoBehaviour
{
    public static void QuickReload(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.P)){
            QuickReload();
        }
    }
}