using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void LoadGame()
    {
        StartCoroutine(LoadGameWithDelay());
    }

    private IEnumerator LoadGameWithDelay()
    {
        yield return new WaitForSeconds(0.3f); // Attend 0.3 secondes
        SceneManager.LoadScene(1);
    }
}
