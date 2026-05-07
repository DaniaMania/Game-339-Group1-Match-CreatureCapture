using System;
using Game.Runtime;
using UnityEngine;

/// <summary>
/// for implementing UI that depends on something else like a character reference
/// </summary>
public abstract class TypedView<T> : MonoBehaviour 
{
    private bool _isSetup = false;

    public void Initialize(params T[] arg)
    {
        _isSetup = true;
        InitializeView(arg);
    }

    public void Deinitialize()
    {
        if (!_isSetup) return; 
        
        DeinitializeView();
        _isSetup = false;
    }

    protected abstract void InitializeView(T[] arg);
    protected abstract void DeinitializeView();
}

public interface IGamePanel
{
    public void SetVisible(bool visible);
}
