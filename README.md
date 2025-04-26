# Dark Dungeon Adventure - Архитектура проекта

## Общее описание
Проект представляет собой 3D-игру в жанре action-adventure с элементами ролевой игры. Основной фокус сделан на модульной архитектуре, позволяющей легко расширять и модифицировать функциональность.

## Основные компоненты

### 1. Модульная система (PlayerControllers)
Система управления игроком построена на модульном принципе, где каждый модуль отвечает за определенную функциональность:

#### Базовые классы:
- `AbstractModul` - базовый абстрактный класс для всех модулей
- `PlayerController` - основной контроллер, управляющий всеми модулями
- `PlayerData` - контейнер для данных игрока

#### Основные модули:
- `AttackModule` - система атак и комбо
- `MovementModul` - управление передвижением
- `AnimationModule` - управление анимациями
- `CameraModule` - управление камерой
- `HealthModule` - система здоровья
- `StaminaModule` - система выносливости
- `InventoryModule` - система инвентаря

### 2. Система ввода (InputSystem)
- `InputSystem` - абстрактный класс для системы ввода
- `InputSystemMN` - реализация системы ввода для мобильных устройств
- `InputSystemPC` - реализация системы ввода для ПК

### 3. Система анимаций
- `AnimationModule` - основной модуль управления анимациями
- `AnimationController` - контроллер анимаций
- `AnimationStateMachine` - конечный автомат для анимаций

### 4. Система врагов
- `EnemyBase` - базовый класс для всех врагов
- `Warrior1` - конкретная реализация врага-воина
- `EnemyController_Distance` - контроллер для врагов дальнего боя

### 5. Система атак
- `AttackSystem` - базовый интерфейс для системы атак
- `IAttackable` - интерфейс для объектов, которые могут быть атакованы
- `AttackModule` - реализация системы атак для игрока

### 6. Система менеджеров
- `GameManager` - основной менеджер игры
- `LocalizationManager` - система локализации
- `WindowsManager` - система управления UI окнами

#### GameManager
Основной менеджер игры, отвечающий за:
- Инициализацию и управление состоянием игры
- Управление сценами и переходами между ними
- Хранение глобальных настроек игры
- Координацию работы других менеджеров
- Управление игровым временем (пауза, замедление)

```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Initialize()
    {
        // Инициализация других менеджеров
        LocalizationManager.Instance.Initialize();
        WindowsManager.Instance.Initialize();
    }
}
```

#### LocalizationManager
Система локализации, обеспечивающая:
- Загрузку и управление языковыми пакетами
- Динамическое переключение языка
- Кэширование переведенных строк
- Поддержку форматирования строк
- Автоматическое обновление UI элементов

```csharp
public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }
    
    private Dictionary<string, string> _localizedStrings;
    private SystemLanguage _currentLanguage;
    
    public void Initialize()
    {
        LoadLanguage(SystemLanguage.English); // По умолчанию
    }
    
    public string GetLocalizedString(string key)
    {
        if (_localizedStrings.TryGetValue(key, out string value))
            return value;
        return key;
    }
    
    public void ChangeLanguage(SystemLanguage newLanguage)
    {
        LoadLanguage(newLanguage);
        // Оповещение всех подписчиков о смене языка
        OnLanguageChanged?.Invoke();
    }
}
```

#### WindowsManager
Система управления UI окнами, обеспечивающая:
- Управление стеком окон
- Анимации открытия/закрытия окон
- Блокировку фоновых окон
- Систему приоритетов окон
- Автоматическое сохранение состояния

```csharp
public class WindowsManager : MonoBehaviour
{
    public static WindowsManager Instance { get; private set; }
    
    private Stack<Window> _windowStack;
    private Dictionary<string, Window> _registeredWindows;
    
    public void Initialize()
    {
        _windowStack = new Stack<Window>();
        _registeredWindows = new Dictionary<string, Window>();
        RegisterAllWindows();
    }
    
    public void OpenWindow(string windowId)
    {
        if (_registeredWindows.TryGetValue(windowId, out Window window))
        {
            _windowStack.Push(window);
            window.Open();
        }
    }
    
    public void CloseTopWindow()
    {
        if (_windowStack.Count > 0)
        {
            Window window = _windowStack.Pop();
            window.Close();
        }
    }
}
```

## Взаимодействие компонентов

### 1. Инициализация
1. `PlayerController` инициализирует все модули
2. Каждый модуль подписывается на необходимые события ввода
3. Модули регистрируются в `PlayerData`

### 2. Цикл обновления
1. `InputSystem` обрабатывает ввод пользователя
2. `PlayerController` распределяет ввод между модулями
3. Модули обновляют свое состояние в `FixedUpdate` и `Update`
4. `AnimationModule` синхронизирует анимации с состоянием

### 3. Взаимодействие модулей
- Модули могут блокировать друг друга (например, атака блокирует движение)
- Модули могут запрашивать данные друг у друга через `PlayerData`
- Модули могут отправлять события через систему событий Unity

## Ключевые особенности архитектуры

### 1. Модульность
- Каждый модуль отвечает за свою функциональность
- Модули могут быть легко добавлены или удалены
- Модули могут быть заменены на альтернативные реализации

### 2. Расширяемость
- Легко добавлять новые типы врагов через наследование от `EnemyBase`
- Легко добавлять новые типы атак через интерфейс `IAttackable`
- Легко добавлять новые системы ввода через абстрактный класс `InputSystem`

### 3. Гибкость
- Настройка параметров через инспектор Unity
- Возможность динамического включения/выключения модулей
- Возможность переопределения поведения через наследование

## Примеры использования

### 1. Создание нового модуля
```csharp
public class NewModule : AbstractModul
{
    protected override void SubscribeToInput()
    {
        // Подписка на события ввода
    }

    private void FixedUpdate()
    {
        // Обновление состояния
    }
}
```

### 2. Создание нового врага
```csharp
public class NewEnemy : EnemyBase
{
    protected override void Initialize()
    {
        // Инициализация врага
    }

    protected override void UpdateState()
    {
        // Обновление состояния врага
    }
}
```

## Рекомендации по расширению

1. При добавлении нового модуля:
   - Наследуйтесь от `AbstractModul`
   - Реализуйте необходимые методы
   - Зарегистрируйте модуль в `PlayerController`

2. При добавлении нового врага:
   - Наследуйтесь от `EnemyBase`
   - Реализуйте уникальное поведение
   - Настройте параметры в инспекторе

3. При добавлении новой системы:
   - Создайте интерфейс для взаимодействия
   - Реализуйте базовый класс
   - Интегрируйте с существующими модулями

## Заключение
Архитектура проекта спроектирована с учетом масштабируемости и гибкости. Она позволяет легко добавлять новую функциональность и модифицировать существующую без необходимости переписывать большие части кода. 