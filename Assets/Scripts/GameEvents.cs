using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class GameEvents 
{
    public static Action<Vector3> OnOriginChanged;
    public static Action OnPlayerCollideWithPlatform;
    public static Action OnPlatformMoved;
    public static Action<Transform> OnPlayerCollideWithCollectable;
    
    public static void  OriginChanged(Vector3 offset)
    {
        OnOriginChanged?.Invoke(offset);
    }
    public static void PlayerCollideWithPlatform()
    {
        OnPlayerCollideWithPlatform?.Invoke();
    }
    public static void PlatformMoved()
    {
        OnPlatformMoved?.Invoke();
    }
    public static void PlayerCollideWithCollectable(Transform obj)
    {
        OnPlayerCollideWithCollectable?.Invoke(obj);
    }
}
