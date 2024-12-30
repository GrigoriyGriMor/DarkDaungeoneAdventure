using UnityEngine;

public abstract class WinEffect : MonoBehaviour
{
    public virtual void MoveUI(UIMoveType moveType, bool activate)
    { }

    public virtual void ScaleUI(bool activate)
    { }

    public virtual void OutHideUI(bool activate)
    { }
}