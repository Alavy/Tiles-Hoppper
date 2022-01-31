using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField]
    private float jumpDuration = 1f;
    [SerializeField]
    private float smoothTime = 3f;
    [SerializeField]
    private float movementSensitivity = 1f;
    [SerializeField]
    private float lookAngle = 15.0f;
    [SerializeField]
    private float jumpHeight = 2f;
    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip jumpSound;
    [SerializeField]
    private AudioClip coinSound;

    [SerializeField]
    private Vector3 sqashAmonut = new Vector3(1.5f,.95f,1.5f);

    private Vector3 m_linVelocity = Vector3.zero;
    private float m_timeElapsed = 0f;

    private Vector3 m_middlePos = Vector3.zero;
    private Vector3 m_targetPos = Vector3.zero;
    private Vector3 m_startPos = Vector3.zero;

    private Vector3 m_p1 = Vector3.zero;
    private Vector3 m_p2 = Vector3.zero;
    private Vector3 m_p3 = Vector3.zero;

    private Vector2 m_dir;
    private Vector3 m_orgnlScale = Vector3.one;

    private bool m_isGameOver = false;

    // References to other objects
    private Camera m_camera;
    private GameManager m_gameManager;
    private AudioSource m_audioSource;
    void Start()
    {
        m_gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        m_audioSource = GetComponent<AudioSource>();

        if (m_gameManager == null)
        {
            Debug.LogError("Missing Game Manager");
        }
        if (jumpSound == null)
        {
            Debug.LogError("Jump Sound Missing");
        }
        planPath(m_gameManager.CurrentPlatform());
        
        InputManager.OnSwipe += onSwipe;
        GameEvents.OnOriginChanged += onOriginChanged;
        Time.timeScale = 1.0f;
        m_camera = Camera.main;
        m_orgnlScale = transform.localScale;
    }

    private void onSwipe(Vector2 dir)
    {
        m_dir = m_dir + dir * Time.deltaTime * movementSensitivity;
    }
    private void onOriginChanged(float zOffset)
    {
        m_startPos = transform.position;
        m_targetPos = m_gameManager.CurrentPlatform();
        m_middlePos = (m_targetPos + m_startPos) * 0.5f
           + new Vector3(0, jumpHeight, 0);
    }

    private void sniffingNextPlatForm()
    {
        Vector3 nextPlatform = m_gameManager.CurrentPlatform();
        float angle = Vector3.Angle(transform.forward, nextPlatform - transform.position);

        if (angle < lookAngle && m_targetPos != nextPlatform)
        {
            planPath(nextPlatform);
        }
    }
    private void sniffingNextPlatFormTest()
    {
        Vector3 nextPlatform = m_gameManager.CurrentPlatform();
        if (m_targetPos != nextPlatform)
        {
            planPath(nextPlatform);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        sniffingNextPlatForm();
        //sniffingNextPlatFormTest();
        float perCentile = m_timeElapsed / jumpDuration;
        
        m_p1 = Vector3.Lerp(m_startPos, m_middlePos, perCentile);
        m_p2 = Vector3.Lerp(m_middlePos, m_targetPos, perCentile);
        m_p3 = Vector3.Lerp(m_p1, m_p2, perCentile);

        transform.position = Vector3.SmoothDamp(transform.position,
            m_p3 + new Vector3(m_dir.x, 0, 0),
            ref m_linVelocity, smoothTime);

        m_timeElapsed += Time.deltaTime;

        // Keep Inside view Volume
        Vector3 viewPos = m_camera.WorldToViewportPoint(transform.position);
        viewPos.x = Mathf.Clamp(viewPos.x,0.1f,0.9f);
        //viewPos.y = Mathf.Clamp(viewPos.y, 0.1f, 0.9f);
        transform.position = m_camera.ViewportToWorldPoint(viewPos);

        if (transform.position.y < 0.001f && !m_isGameOver)
        {
            m_isGameOver = true;
            m_gameManager.GameOver();
        }
    }
    private void OnDisable()
    {
        InputManager.OnSwipe -= onSwipe;
        GameEvents.OnOriginChanged -= onOriginChanged;
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Platform"))
        {
            //for smoothing the motion

            m_dir = Vector2.zero;
            Vector3 nextPlatform = m_gameManager.NextPlatform();

            float angle = Vector3.Angle(transform.forward, nextPlatform - transform.position);
            if (angle >= lookAngle)
            {
                planPath(transform.position + 3 * transform.forward - new Vector3(0, 1f, 0));
            }
            transform.LeanScale(new Vector3(m_orgnlScale.x * sqashAmonut.x, m_orgnlScale.y * sqashAmonut.y, m_orgnlScale.z * sqashAmonut.z),
                0.1f).setOnComplete(()=> {
                transform.LeanScale(m_orgnlScale, 0.1f);
            });
            m_audioSource.PlayOneShot(jumpSound);
            GameEvents.PlayerCollideWithPlatform();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Collectable"))
        {
            GameEvents.PlayerCollideWithCollectable(other.transform);
            m_audioSource.PlayOneShot(coinSound);

        }
    }

    private void planPath(Vector3 target)
    {
        m_startPos = transform.position;
        m_targetPos = target;
        m_middlePos = (m_targetPos + m_startPos) * 0.5f
            + new Vector3(0, jumpHeight, 0);
        m_timeElapsed = 0;
    }
}
