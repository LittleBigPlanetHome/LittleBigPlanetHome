using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClick : MonoBehaviour
{
    public void LoadLoaderScene()
    {
        SceneManager.LoadScene("Loader");
    }
}