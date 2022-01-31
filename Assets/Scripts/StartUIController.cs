using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class StartUIController : MonoBehaviour
{
    [SerializeField]
    private Transform appName;
    [SerializeField]
    private Transform startUIObj;
    [SerializeField]
    private Transform exitUIObj;
    [SerializeField]
    private AudioClip buttonSound;

    private AudioSource m_audioSource;

    private void Start()
    {
        Time.timeScale = 1.0f;
        if (buttonSound == null)
        {
            Debug.LogError("Button Sound Missing");
        }
        
        appName.LeanScale(new Vector3(1.1f, 1.1f, 1.1f), 0.8f).setLoopPingPong();

        startUIObj.LeanMoveLocalY(-702f, 1.2f).setEase(LeanTweenType.easeSpring);
        exitUIObj.LeanMoveLocalY(-702f, 1.2f).setEase(LeanTweenType.easeSpring);

        m_audioSource = GetComponent<AudioSource>();
    }
    public void StartUIPressed()
    {
        startUIObj.LeanScale(new Vector3(1.1f, 1.1f, 1.1f), 0.1f);
        m_audioSource.PlayOneShot(buttonSound);
    }
    public void StartUIReleased()
    {
        startUIObj.LeanScale(new Vector3(1f, 1f, 1f), 0.1f);
        StartCoroutine(startScene("Main"));
    }
    private IEnumerator startScene(string sceneName)
    {
        AsyncOperation op =  SceneManager.LoadSceneAsync(sceneName);
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
