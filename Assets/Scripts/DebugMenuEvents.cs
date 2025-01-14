using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DebugMenuEvents : MonoBehaviour
{
    private UIDocument _document;
    private Toggle _debugModetoggle;
    private Toggle _freezeEnemiesToggle;
    private Toggle _freePlayerMovementToggle;
    private Slider _gameSpeedSlider;
    private EventCallback<ChangeEvent<bool>> _debugModeToggleCallback;
    private EventCallback<ChangeEvent<bool>> _freezeEnemiesToggleCallback;
    private EventCallback<ChangeEvent<bool>> _freePlayerMovementToggleCallback;
    private int gameSpeed;

    //
    private void Start()
    {
        _document = GetComponent<UIDocument>();
        if (_document == null)
        {
            Debug.LogError("UIDocument component is missing!");
            return;
        }

        InitializeAll();
        _document.enabled = false;
    }


    private void InitializeAll()
    {
        if (_document == null)
        {
            return;
        }

        var root = _document.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("rootVisualElement is null. Ensure the UIDocument has a valid UXML file.");
            return;
        }
        // Get the toggle and ensure the callback is registered
        _debugModetoggle = _document.rootVisualElement.Q<Toggle>("DebugModeToggle") as Toggle;
        _freezeEnemiesToggle = _document.rootVisualElement.Q<Toggle>("FreezeEnemiesToggle") as Toggle;
        _freePlayerMovementToggle = _document.rootVisualElement.Q<Toggle>("FreePlayerMovementToggle") as Toggle;
        _gameSpeedSlider = _document.rootVisualElement.Q<Slider>("GameSpeedSlider") as Slider;

        if (_debugModetoggle != null)
        {
            _debugModetoggle.value = DebugMode.instance.debugMode;
            _debugModetoggle.RegisterCallback<ChangeEvent<bool>>(evt => DebugModeToggleValueChanged(evt.newValue));

            _freezeEnemiesToggle.value = DebugMode.instance.freezeEnemies;
            _freezeEnemiesToggle.RegisterCallback<ChangeEvent<bool>>(evt => FreezeEnemiesToggleValueChanged(evt.newValue));

            _freePlayerMovementToggle.value = DebugMode.instance.freePlayerMovement;
            _freePlayerMovementToggle.RegisterCallback<ChangeEvent<bool>>(evt => FreePlayerMovementToggleValueChanged(evt.newValue));

            _gameSpeedSlider.value = DebugMode.instance.gameSpeed;
            _gameSpeedSlider.RegisterCallback<ChangeEvent<float>>(evt => GameSpeedSliderValueChanged(evt.newValue));
        }

    }

    private void GameSpeedSliderValueChanged(float value)
    {
        DebugMode.instance.gameSpeed = (int)value;
    }

    private void FreePlayerMovementToggleValueChanged(bool value)
    {
        if (value)
        {
            DebugMode.instance.freePlayerMovement = true;
        }
        else
        {
            DebugMode.instance.freePlayerMovement = false;
        }
    }

    private void FreezeEnemiesToggleValueChanged(bool value)
    {
        if (value)
        {
            DebugMode.instance.freezeEnemies = true;
        }
        else
        {
            DebugMode.instance.freezeEnemies = false;
        }
    }

    private void DebugModeToggleValueChanged(bool value)
    {
        if (value)
        {
            DebugMode.instance.debugMode = true;
        }
        else
        {
            DebugMode.instance.debugMode = false;
        }
    }

    private void OnDisable()
    {
        if (_debugModetoggle != null && _debugModeToggleCallback != null)
        {
            _debugModetoggle.UnregisterCallback(_debugModeToggleCallback);
        }
        if (_freezeEnemiesToggle != null && _freezeEnemiesToggleCallback != null)
        {
            _freezeEnemiesToggle.UnregisterCallback(_freezeEnemiesToggleCallback);
        }
        if (_freePlayerMovementToggle != null && _freePlayerMovementToggleCallback != null)
        {
            _freePlayerMovementToggle.UnregisterCallback(_freePlayerMovementToggleCallback);
        }
    }

    private void OnEnable()
    {
        // Reinitialize the toggle when re-enabling the document
        InitializeAll();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            _document.enabled = !_document.enabled;

            // Handle reinitializing when the document is re-enabled
            if (_document.enabled)
            {
                InitializeAll();
                Time.timeScale = 0;
            }
            else
            {
                if (!DebugMode.instance.debugMode)
                    DebugMode.instance.ResetAll();
                else
                    Time.timeScale = DebugMode.instance.gameSpeed / 100f;
            }
        }
    }
}
