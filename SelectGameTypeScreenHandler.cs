using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectGameTypeScreenHandler : MonoBehaviour
{
    public void OnBackButtonClick()
    {
        SceneManager.LoadScene(1);
    }
}
