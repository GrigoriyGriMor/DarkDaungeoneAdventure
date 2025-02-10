using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class RenderScaleController : MonoBehaviour
{
    [SerializeField] private Slider renderScaleSlider; // Ползунок для управления Render Scale
    private UniversalRenderPipelineAsset urpAsset; // URP настройки

    void Start()
    {
        // Получаем текущий Universal Render Pipeline Asset
        urpAsset = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;

        if (urpAsset == null)
        {
            Debug.LogError("URP не найден! Убедитесь, что в проекте используется Universal Render Pipeline.");
            return;
        }

        // Устанавливаем начальное значение ползунка
        renderScaleSlider.value = urpAsset.renderScale;
        
        // Подписываемся на изменение ползунка
        renderScaleSlider.onValueChanged.AddListener(SetRenderScale);
    }

    void SetRenderScale(float value)
    {
        if (urpAsset != null)
        {
            urpAsset.renderScale = Mathf.Clamp(value, 0.5f, 2.0f); // Ограничиваем масштаб (0.5 - 2.0)
            Debug.Log($"Render Scale изменён: {urpAsset.renderScale}");
        }
    }
}
