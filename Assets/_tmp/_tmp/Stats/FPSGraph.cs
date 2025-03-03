using UnityEngine;
using UnityEngine.UI;

public class FPSGraph : MonoBehaviour
{
    // Настройки графика
    public RawImage graphImage; // Assign your RawImage component here in the inspector
    public Color normalColor = Color.green; // Цвет для нормальных значений FPS
    public Color lowFPSColor = Color.red; // Цвет для FPS ниже 60
    public Color highFPSColor = Color.blue; // Цвет для FPS выше частоты обновления экрана
    public Color backgroundColor = new Color(0, 0, 0, 1); // Цвет фона (полностью непрозрачный)
    [Tooltip("Интервал обновления графика (в секундах). Например: 1 = раз в секунду, 0.5 = каждые полсекунды.")]
    public float updateInterval = 1f; // Интервал обновления данных
    public int columnWidth = 20; // Ширина каждого столбца в пикселях
    public int spacing = 4; // Отступ между столбцами

    // Внутренние переменные
    private Texture2D graphTexture;
    private int textureWidth;
    private int textureHeight;
    private int maxColumns; // Максимальное количество столбцов
    private float timer;
    private float[] fpsValues;
    private int currentIndex;
    private int monitorRefreshRate;

    void Start()
    {
        // Инициализация частоты обновления экрана
        monitorRefreshRate = Screen.currentResolution.refreshRate;

        // Проверка назначения RawImage
        if (!graphImage)
        {
            Debug.LogError("RawImage is not assigned! Please assign a RawImage in the Inspector.");
            return;
        }

        // Инициализация текстуры
        InitializeTexture();

        // Рассчитываем максимальное количество столбцов
        maxColumns = Mathf.FloorToInt(textureWidth / (float)(columnWidth + spacing));
        if (maxColumns <= 0)
        {
            Debug.LogError("Calculated maxColumns is invalid. Check columnWidth and spacing values.");
            return;
        }

        // Инициализация массива значений FPS
        fpsValues = new float[maxColumns];
        ClearGraph();
    }

    private void InitializeTexture()
    {
        // Если текстура не назначена, создаем её по умолчанию
        if (!graphImage.texture)
        {
            Debug.LogWarning("Texture for RawImage is not assigned. Creating a default texture...");
            graphTexture = new Texture2D(154, 46); // Размер по умолчанию
            graphTexture.filterMode = FilterMode.Point;
            graphImage.texture = graphTexture;
        }
        else
        {
            graphTexture = graphImage.texture as Texture2D;
            if (!graphTexture)
            {
                Debug.LogError("The texture assigned to RawImage is not a Texture2D!");
                return;
            }
        }

        // Получаем размеры текстуры
        textureWidth = graphTexture.width;
        textureHeight = graphTexture.height;
    }

    void Update()
    {
        // Увеличиваем таймер
        timer += Time.deltaTime;

        // Обновляем график через заданный интервал
        if (timer >= updateInterval)
        {
            AddNewFPSValue();
            DrawGraph();
            timer = 0f;
        }
    }

    private void ClearGraph()
    {
        // Заполняем текстуру фоновым цветом
        for (int x = 0; x < textureWidth; x++)
        {
            for (int y = 0; y < textureHeight; y++)
            {
                graphTexture.SetPixel(x, y, backgroundColor);
            }
        }
        graphTexture.Apply();
    }

    private void AddNewFPSValue()
    {
        // Вычисляем текущий FPS и добавляем его в массив
        float currentFPS = 1f / Time.deltaTime;
        fpsValues[currentIndex] = currentFPS;
        currentIndex = (currentIndex + 1) % maxColumns;
    }

    private void DrawGraph()
    {
        ClearGraph();

        // Рисуем все столбцы
        for (int i = 0; i < maxColumns; i++)
        {
            int index = (currentIndex + i) % maxColumns;
            float normalizedFPS = Mathf.Clamp01(fpsValues[index] / 200f); // Нормализуем FPS (максимум 200 FPS)
            int height = Mathf.RoundToInt(normalizedFPS * textureHeight);

            // Вычисляем позицию столбца (справа налево)
            int startX = textureWidth - (i + 1) * (columnWidth + spacing);

            // Определяем цвет столбца
            Color columnColor = normalColor;
            if (fpsValues[index] < 60)
            {
                columnColor = lowFPSColor;
            }
            else if (fpsValues[index] > monitorRefreshRate)
            {
                columnColor = highFPSColor;
            }

            // Рисуем столбец
            for (int x = startX; x < startX + columnWidth && x < textureWidth; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    graphTexture.SetPixel(x, y, columnColor);
                }
            }
        }

        graphTexture.Apply();
    }
}