using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollowBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform followedObject;

    [SerializeField]
    private float followSpeedZ = 0.2f;
    [SerializeField]
    private float followSpeedX = 0.4f;
    [SerializeField]
    private float thresold;

    private List<Transform> m_moveableObjs;
    private Vector3 m_offsetFromParent;
    private bool m_isAbleTochangedOrigin = false;
    private float m_zLinVel;
    private float m_xLinVel;

    private void Start()
    {
        m_moveableObjs = new List<Transform>();

        if (followedObject == null)
        {
            Debug.LogError("Can't Find the Follow Object");
        }
        m_offsetFromParent = transform.position - followedObject.position;
        GameObject[] platObjs = GameObject.FindGameObjectsWithTag("Platform");
        GameObject[] coltObjs = GameObject.FindGameObjectsWithTag("Collectable");
        GameObject meterObj = GameObject.FindGameObjectWithTag("MeterText");

        for (int i = 0; i < platObjs.Length; i++)
        {
            m_moveableObjs.Add(platObjs[i].transform);
        }
        for (int i = 0; i < coltObjs.Length; i++)
        {
            m_moveableObjs.Add(coltObjs[i].transform);
        }
        m_moveableObjs.Add(meterObj.transform);
    }
    private void OnEnable()
    {
        GameEvents.OnPlatformMoved += onPlatformMoved;

    }
    private void OnDisable()
    {
        GameEvents.OnPlatformMoved -= onPlatformMoved;

    }
    private void onPlatformMoved()
    {
        if (m_isAbleTochangedOrigin)
        {
            Vector3 followPos = followedObject.position;

            // move Environments
            foreach (var item in m_moveableObjs)
            {
                Vector3 itemPos = (item.position - followPos);
                item.position = new Vector3(itemPos.x, item.position.y, itemPos.z);

            }

            Vector3 fPos = (followedObject.position - followPos);
            followedObject.position = new Vector3(fPos.x, followedObject.position.y, fPos.z);

            Vector3 cPos = (transform.position - followPos);
            transform.position = new Vector3(cPos.x, transform.position.y, cPos.z);

            GameEvents.OriginChanged(followPos);
        }
    }
    private void Update()
    {
        //transform.position = Vector3.SmoothDamp(transform.position,
            //followedObject.position + m_offsetFromParent, ref m_linVelocity, followSpeed);

        Vector3 offsetedPos = followedObject.position + m_offsetFromParent;

        float cZpos = Mathf.SmoothDamp(transform.position.z, offsetedPos.z, ref m_zLinVel, followSpeedZ);
        float cXpos = Mathf.SmoothDamp(transform.position.x, offsetedPos.x, ref m_xLinVel, followSpeedX);

        transform.position = new Vector3(cXpos, transform.position.y,cZpos);
    }
    private void LateUpdate()
    {
        if (followedObject.position.magnitude > thresold)
        {

            m_isAbleTochangedOrigin = true;
        }
        else
        {
            m_isAbleTochangedOrigin = false;
        }

    }
}
