using UnityEngine;

public class AbstractAIModule : MonoBehaviour
{
    private bool _moduleIsActive = false;
    public bool ModuleIsActive { get => _moduleIsActive; }

    public BaseAIParametrs _aiParametrs;

    public void Init(BaseAIParametrs _param)
    {
        _aiParametrs = _param;
        _moduleIsActive = true;
    }
}
