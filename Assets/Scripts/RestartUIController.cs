using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartUIController : MonoBehaviour
{
    [SerializeField]
    private Transform restartUIObj;
    [SerializeField]
    private Transform exitUIObj;
    [SerializeField]
    private AudioClip buttonSound;

    private AudioSource m_audioSource;

    private void Start()
    {
        Time.timeScale = 1.0f;
        m_audioSource = GetComponent<AudioSource>();

        if (restartUIObj == null)
        {
            Debug.LogError("Restart UI Missing");
        }
        if (buttonSound == null)
        {
            Debug.LogError("Button Sound Missing");
        }
        restartUIObj.LeanScale(new Vector3(1.1f, 1.1f, 1.1f), 0.5f).setLoopPingPong();
    }
    public void RestartUIPressed()
    {
        restartUIObj.LeanScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f);
        m_audioSource.PlayOneShot(buttonSound);

    }
    public void RestartUIReleased()
    {
        restartUIObj.LeanScale(new Vector3(1f, 1f, 1f), 0.1f);
        StartCoroutine(startScene("Main"));
    }
    private IEnumerator startScene(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            yield return null;
        }
    }
    public void ExitUIPressed()
    {
        exitUIObj.LeanScale(new Vector3(1.1f, 1.1f, 1.1f), 0.1f);
        m_audioSource.PlayOneShot(buttonSound);

    }
    public void ExitUIReleased()
    {
        exitUIObj.LeanScale(new Vector3(1f, 1f, 1f), 0.1f);
        Application.Quit();

    }
}
