using UnityEngine;

public abstract class BaseComponent : MonoBehaviour
{
    protected virtual void Start()
    {
        LoadComponent();
    }

    protected virtual void Reset()
    {
        LoadComponent();
    }

    protected virtual void LoadComponent(){}
}
