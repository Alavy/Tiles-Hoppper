using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class InputManager : MonoBehaviour
{
    public static Action<Vector2> OnSwipe;

    [SerializeField]
    private float fingerMoveRange = 60f;

    private TouchControls m_inputActions;
    private Vector2 m_startFingPos;
    private bool m_isFingerDown = false;

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
            
            Vector2 deltaPos = Vector2.ClampMagnitude(fingPos - m_startFingPos, fingerMoveRange);
            deltaPos = deltaPos / fingerMoveRange;
            OnSwipe?.Invoke(deltaPos);
            m_startFingPos = fingPos;
        }
    }
}
