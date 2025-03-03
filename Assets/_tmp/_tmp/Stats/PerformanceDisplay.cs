using UnityEngine;
using TMPro;

public class PerformanceMonitor : MonoBehaviour
{
    public TMP_Text fpsText;        // Assign your FPS TextMeshPro UI component here in the inspector
    public TMP_Text memoryText;     // Assign your Memory TextMeshPro UI component here in the inspector
    public TMP_Text cpuText;        // Assign your CPU Load TextMeshPro UI component here in the inspector
    public TMP_Text gpuText;        // Assign your GPU Load TextMeshPro UI component here in the inspector
    public TMP_Text platformText;   // Assign your Platform Info TextMeshPro UI component here in the inspector
    public TMP_Text screenText;     // Assign your Screen Info TextMeshPro UI component here in the inspector

    [Tooltip("Интервал обновления данных (в секундах). Например: 1 = раз в секунду, 0.5 = каждые полсекунды.")]
    public float updateInterval = 1f; // Интервал обновления данных (публичное поле)

    private float deltaTime = 0.0f;
    private float frameStartTime = 0f;
    private float timer = 0f;

    void Start()
    {
        // Начало отсчета времени для первого кадра
        frameStartTime = Time.realtimeSinceStartup;

        // Отображаем информацию о платформе при старте
        UpdatePlatformInfo();
        UpdateScreenInfo();
    }

    void Update()
    {
        // Вычисляем дельту времени для сглаживания FPS
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

        // Увеличиваем таймер
        timer += Time.deltaTime;

        // Проверяем, прошло ли достаточно времени для обновления данных
        if (timer >= updateInterval)
        {
            UpdatePerformanceMetrics();
            timer = 0f; // Сбрасываем таймер
        }
    }

    private void UpdatePerformanceMetrics()
    {
        // Измеряем время выполнения текущего кадра
        float frameEndTime = Time.realtimeSinceStartup;
        float cpuTimeMs = (frameEndTime - frameStartTime) * 1000f;
        frameStartTime = frameEndTime;

        // Отображаем FPS
        if (fpsText != null)
        {
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.} fps ({1:0.0} ms)", fps, msec);
            fpsText.text = text;
        }

        // Отображаем использование памяти
        if (memoryText != null)
        {
            long totalMemory = System.GC.GetTotalMemory(false); // Более точное измерение памяти
            string memoryTextValue = string.Format("{0} mb", (totalMemory / 1024 / 1024));
            memoryText.text = memoryTextValue;
        }

        // Отображаем время выполнения CPU
        if (cpuText != null)
        {
            cpuText.text = string.Format("{0:0.0} ms", cpuTimeMs);
        }

        // Отображаем заглушку для GPU
        if (gpuText != null)
        {
            gpuText.text = "n/a"; // GPU Load пока недоступен через стандартные API
        }
    }

    private void UpdatePlatformInfo()
    {
        // Получаем информацию о платформе и рендеринге
        string platformInfo = $"Platform: {Application.platform}, ";
        string graphicsInfo = $"{SystemInfo.graphicsDeviceName} (0x{SystemInfo.deviceUniqueIdentifier.GetHashCode():X}) ";
        string shaderInfo = $"Shader Level: {SystemInfo.graphicsShaderLevel}, Rendering API: {SystemInfo.graphicsDeviceVersion}";
        string unityVersion = $"Unity Version: {Application.unityVersion}";

        // Формируем полную строку
        string fullInfo = platformInfo + graphicsInfo + shaderInfo + ", " + unityVersion;

        // Отображаем информацию о платформе
        if (platformText != null)
        {
            platformText.text = fullInfo;
        }
    }

    private void UpdateScreenInfo()
    {
        // Получаем информацию о разрешении экрана
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;
        int refreshRate = Screen.currentResolution.refreshRate;

        // Формируем строку с информацией о экране
        string screenInfo = $"{screenWidth}x{screenHeight}, {refreshRate}Hz";

        // Отображаем информацию о экране
        if (screenText != null)
        {
            screenText.text = screenInfo;
        }
    }
}