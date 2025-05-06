using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingManager : MonoBehaviour
{
    [SerializeField] private PostProcessVolume ppVolime;
    [SerializeField] private int ppLayerMask = 31; 

    private Vignette vignetteEffect;
    private Coroutine vignetteEffectCorutine;

    private void Awake()
    {
        ppVolime.profile.TryGetSettings(out vignetteEffect);

        if (vignetteEffect == null)
            Debug.LogError("Vignette effect not found");
        else
            vignetteEffect.intensity.value = 0;
    }

    #region Vignette
    [Header("Vignette"), SerializeField]
    private float vignetteSwapSpeed = 0.1f;
    [SerializeField] private float vignetteRange = 0.05f;
    
    float targetValue;

    public void VignetteActivate(bool _isActive, float _targetValue = 1)
    {
        return;
        
        if (vignetteEffectCorutine != null)
            StopCoroutine(vignetteEffectCorutine);

        if (_isActive)
        {
            targetValue = _targetValue;
            vignetteEffectCorutine = StartCoroutine(VignetteUpdate(targetValue));
        }
        else 
            vignetteEffectCorutine = StartCoroutine(VignetteEffectEnd());
    }

    private IEnumerator VignetteUpdate(float _targetValue)
    {
        while (Math.Abs(vignetteEffect.intensity.value - _targetValue) > 0.01f)
        {
            vignetteEffect.intensity.value = vignetteEffect.intensity.value > _targetValue ? 
                vignetteEffect.intensity.value - vignetteSwapSpeed * Time.deltaTime 
                : vignetteEffect.intensity.value + vignetteSwapSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        vignetteEffectCorutine = StartCoroutine(VignetteUpdate(vignetteEffect.intensity.value < targetValue ? targetValue + vignetteRange : targetValue - vignetteRange));
    }

    private IEnumerator VignetteEffectEnd()
    {
        while (vignetteEffect.intensity.value > 0)
        {
            vignetteEffect.intensity.value -= vignetteSwapSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        vignetteEffectCorutine = null;
    }
    #endregion
}
