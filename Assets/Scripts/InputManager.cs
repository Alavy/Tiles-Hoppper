using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class InputManager : MonoBehaviour
{
    public static Action<Vector2> OnSwipe;
    [SerializeField]
    [Range(0.1f,100f)]
    private float fingDistance = 1.0f;
    private TouchControls m_inputActions;
    private Vector2 m_startFingPos;
    private Vector2 m_prevFingPos;
    private bool m_isFingerDown = false;
    private double m_startTime = 0;

    private void Awake()
    {
        m_inputActions = new TouchControls();
    }
    private void OnEnable()
    {
        m_inputActions.Enable();
    }
    private void OnDisable()
    {
        m_inputActions.Disable();
    }
    private void Start()
    {
        m_inputActions.Touch.PrimaryContract.started += ctx
            => primaryContractStarted(ctx);
        m_inputActions.Touch.PrimaryContract.canceled += ctx
            => primaryContractCanceled(ctx);
    }

    private void primaryContractStarted(InputAction.CallbackContext ctx)
    {
        m_startFingPos = m_inputActions.Touch.PrimaryPosition.ReadValue<Vector2>();
        m_startTime = ctx.startTime;
        m_isFingerDown = true;     
    }
    private void primaryContractCanceled(InputAction.CallbackContext ctx)
    {
        m_isFingerDown = false;

    }
    private void Update()
    {
        if (m_isFingerDown)
        {
            Vector2 fingPos = m_inputActions.Touch.PrimaryPosition.ReadValue<Vector2>();
            if (Vector2.Distance(m_prevFingPos, fingPos) < fingDistance)
            {
                m_startFingPos = fingPos;
            }
            m_prevFingPos = fingPos;

            Vector2 deltaPos = (fingPos - m_startFingPos).normalized;
            OnSwipe?.Invoke(deltaPos);
        }
    }
}
