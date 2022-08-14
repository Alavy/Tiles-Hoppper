using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Transform platForm;
    [SerializeField]
    private int platFormCount = 8;
    [SerializeField]
    private int platformBatchCount = 4;
    [SerializeField]
    private int platformMoveCount = 4;

    [SerializeField]
    private Transform collectable;
    [SerializeField]
    private int collectableCount = 5;

    [SerializeField]
    private Transform gameOverUIObj;
    [SerializeField]
    private float gameOverTextShowTime = 0.5f;
    [SerializeField]
    private TextMeshProUGUI scoreUI;
    [SerializeField]
    private TextMeshPro meterWorldUI;

    [SerializeField]
    private ParticleSystem particle;

    [SerializeField]
    private float timeScaleIncrement = 0.01f;
    [SerializeField]
    private float timeAfter = 20.0f;
    [SerializeField]
    private float xPlatformDisplacemnt=1f;
    [SerializeField]
    private float zPlatformDisplacemnt = 2f;

    private float m_time = 0;
    private float m_timeScale = 1f;

    private int m_platformIndex = 0;

    private int m_startIndex = 0;
    private int m_lastIndex = 9;
    private int m_score = 0;

    private bool m_isFirstTime = false;
    private bool m_isGameOver = false;

    private Transform m_lastPlatformTransform;
    private ParticleSystem m_particle;
    private Transform[] m_platForms;
    private Transform[] m_collecatbles;

    private double m_zOffsetpos=1;

    // references to other Objects
    private Transform m_player;
    

    private void Awake()
    {
        m_platForms = new Transform[platFormCount];
        for (int i = 0; i < platFormCount; i++)
        {
            m_platForms[i] = Instantiate(platForm);
            if (i < platformBatchCount)
            {
                m_platForms[i].position = new Vector3(-xPlatformDisplacemnt, 0,
                    i * zPlatformDisplacemnt);

            }
            else
            {
                m_platForms[i].position = new Vector3(xPlatformDisplacemnt,
                    0, i * zPlatformDisplacemnt);
            }
        }
        m_collecatbles = new Transform[collectableCount];
        for (int i = 0; i < collectableCount; i++)
        {
            m_collecatbles[i] = Instantiate(collectable);
        }
        m_particle = Instantiate(particle);
        m_particle.gameObject.SetActive(false);

        m_lastPlatformTransform = m_platForms[platFormCount - 1];

        m_startIndex = 0;
        m_lastIndex = platformMoveCount-1;

        placeCollectable(m_lastIndex-1);

    }
    private void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player").transform;
        if (gameOverUIObj == null)
        {
            Debug.LogError("Missing Game Over UI Object");
        }
        if (scoreUI == null)
        {
            Debug.LogError("Missing Score UI Object");
        }
        if (m_player == null)
        {
            Debug.LogError("Missing Player Object");
        }
        gameOverUIObj.gameObject.SetActive(false);

        scoreUI.text = m_score.ToString();
       
    }
    private void onPlayerCollideWithPlatform()
    {

        updateScore(Random.Range(3,6));
    }
    private void updateScore(int score)
    {
        m_score = m_score + score;
        scoreUI.transform.LeanScale(new Vector3(1.1f, 1.1f,
            1f), 0.2f)
            .setEase(LeanTweenType.easeShake)
            .setOnComplete(() => { 
                scoreUI.transform.LeanScale(new Vector3(1f, 1f, 1f), 0.2f)
                .setEase(LeanTweenType.easeShake); 
            });
        scoreUI.text = m_score.ToString();
    }
    private void onOriginChanged(Vector3 offset)
    {
        m_zOffsetpos = m_zOffsetpos + offset.z;
    }
    private void onPlayerCollideWithCollectable(Transform obj)
    {
        updateScore(Random.Range(7, 9));
        for(int i = 0; i < collectableCount; i++)
        {
            if(obj== m_collecatbles[i])
            {
                m_collecatbles[i].gameObject.SetActive(false);
                break;
            }
        }
        m_particle.transform.position = obj.position;
        m_particle.gameObject.SetActive(true);
        m_particle.Play();

    }
    private void OnEnable()
    {
        GameEvents.OnPlayerCollideWithPlatform += onPlayerCollideWithPlatform;
        GameEvents.OnOriginChanged += onOriginChanged;
        GameEvents.OnPlayerCollideWithCollectable += onPlayerCollideWithCollectable;
    }
    private void OnDisable()
    {
        GameEvents.OnPlayerCollideWithPlatform -= onPlayerCollideWithPlatform;
        GameEvents.OnOriginChanged -= onOriginChanged;
        GameEvents.OnPlayerCollideWithCollectable -= onPlayerCollideWithCollectable;
    }
    private void movePlatform()
    {
        int index = 1;
        float lastZdispl = m_lastPlatformTransform.position.z;
        float xPlayerPos = m_player.position.x;
        float tweenZdispl = 4;
        float tweenTime = 0.3f;

        List<Transform> pts = new List<Transform>();

        for (int i = m_startIndex; i <= m_lastIndex; i++)
        {
            if (m_platformIndex % 2 == 0)
            {
                m_platForms[i].position = new Vector3(
                    -xPlatformDisplacemnt 
                    - Random.Range(-0.3f,0.3f), 0, 
                    (lastZdispl 
                    + (index) * zPlatformDisplacemnt)+ tweenZdispl);

                m_platForms[i].gameObject.SetActive(false);
                pts.Add(m_platForms[i]);
                index++;
               

            }
            else
            {
                m_platForms[i].position = new Vector3(
                    xPlatformDisplacemnt +
                    Random.Range(-0.3f, 0.3f), 0, 
                    (lastZdispl + (index) 
                    * zPlatformDisplacemnt)
                    + tweenZdispl);

                m_platForms[i].gameObject.SetActive(false);
                pts.Add(m_platForms[i]);
                index++;
               
            }
        }
        var sq = LeanTween.sequence();
        foreach (var item in pts)
        {
           sq.append(()=> { item.gameObject.SetActive(true); });
           sq.append(item.LeanMoveZ(item.position.z-tweenZdispl, tweenTime));
           sq.append(() => { item.gameObject.SetActive(true); });

        }
        int placeIndex = m_lastIndex-1;
        sq.append(()=> {
            placeCollectable(placeIndex);
            placeMeterObj();
            GameEvents.PlatformMoved();
        });
        
        m_lastPlatformTransform = m_platForms[m_lastIndex];

        m_startIndex = (m_startIndex + platformMoveCount) % platFormCount;
        m_lastIndex = (m_lastIndex + platformMoveCount) % platFormCount;
    }
    private void placeCollectable(int index)
    {
        Vector3 startPos = m_platForms[index - 1].position;
        Vector3 targetPos = m_platForms[index].position;
        Vector3 middlePos = (targetPos + startPos) * 0.5f
           + new Vector3(0, 2, 0);

        float deltaInc = 1.0f / (collectableCount+1);

        float t = 0;
        for(int i = 0; i < collectableCount; i++)
        {
            t = t + deltaInc;
            m_collecatbles[i].gameObject.SetActive(true);
            Vector3 p1 = Vector3.Lerp(startPos, middlePos, t);
            Vector3 p2 = Vector3.Lerp(middlePos, targetPos, t);
            Vector3 p3 = Vector3.Lerp(p1, p2, t);

            m_collecatbles[i].position = p3;
            m_collecatbles[i].GetComponent<MeshRenderer>().material.color = Random.ColorHSV(.2f, 1f, 1f, 1f, 0.7f, 1f);

        }
    }
    private void Update()
    {
        if (!m_isGameOver)
        {
            if (m_time > timeAfter)
            {
                m_time = 0;
                m_timeScale = m_timeScale + timeScaleIncrement;
                Time.timeScale = m_timeScale;
            }
            else
            {
                m_time = m_time + Time.unscaledDeltaTime;
            }
        }
    }
    private void placeMeterObj()
    {
        meterWorldUI.gameObject.SetActive(true);
        Vector3 meterPos = m_lastPlatformTransform.position;
        meterWorldUI.transform.position = new Vector3(meterPos.x,
            meterWorldUI.transform.position.y, meterPos.z);
        meterWorldUI.text = (m_player.position.z + m_zOffsetpos).ToString("f0") + " m";
    }
   
    public Vector3 NextPlatform()
    {
        m_platformIndex++;
        m_platformIndex = m_platformIndex % platFormCount;
        if (m_platformIndex % 6 == 0 && m_platformIndex!=0)
        {
            movePlatform();
            m_isFirstTime = true;
        }
        else if (m_platformIndex == 1 && m_isFirstTime)
        {
            movePlatform();
        }
        
        return m_platForms[m_platformIndex].position;
    }
    public void GameOver()
    {
        LeanTween.cancelAll();
        m_isGameOver = true;
        Time.timeScale = 0;
        gameOverUIObj.localScale = Vector3.zero;
        gameOverUIObj.gameObject.SetActive(true);
        gameOverUIObj.LeanScale(Vector3.one, gameOverTextShowTime)
            .setEase(LeanTweenType.easeInElastic)
            .setIgnoreTimeScale(true)
            .setOnComplete(()=> {
            StartCoroutine(startScene("Restart",2.0f));
        });
    }
    private IEnumerator startScene(string sceneName,float afterSecnd)
    {
        yield return new WaitForSecondsRealtime(afterSecnd);
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            yield return null;
        }

    }
    public Vector3 CurrentPlatform()
    {
        return m_platForms[m_platformIndex].position;
    }
}