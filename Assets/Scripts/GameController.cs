using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    private enum GameState {
        Menu,
        Playing,
        Finished,
        FinishedAll
    }

    private const string _currentLevelKey = "currentLevel";
    private const string _currentLevelRetriesKey = "currentLevelRetries";

    [SerializeField]
    private List<GameObject> _levels;
    [SerializeField]
    private CameraFollowPlayer _camera;
    [SerializeField]
    private PlayerController _playerPrefab;

    [Header("UI"), Space]
    [SerializeField]
    private GameObject _menuPanel;
    [SerializeField]
    private GameObject _gamePanel;
    [SerializeField]
    private GameObject _finishPanel;
    [SerializeField]
    private GameObject _finishText;
    [SerializeField]
    private GameObject _finishedAllText;
    [SerializeField]
    private Button _playButton;
    [SerializeField]
    private Button _restartButton;
    [SerializeField]
    private Button _backButton;
    [SerializeField]
    private Button _nextButton;
    [SerializeField]
    private Text _retriesText;
    [SerializeField]
    private Text _levelText;

    private Vector3 _startPosition = Vector3.zero;
    private PlayerController _player;
    private int _currentLevelIndex;
    private int _currentLevelRetries;
    private GameObject _currentLevel;
    private GameState _gameState = GameState.Menu;

    private void Awake() {
        ToggleGameUi(false);
        ToggleMenuUi(true);

        _playButton.onClick.AddListener(StartLevel);
        _restartButton.onClick.AddListener(RestartLevel);
        _backButton.onClick.AddListener(ExitLevel);
        _nextButton.onClick.AddListener(StartLevel);

        _currentLevelIndex = PlayerPrefs.GetInt(_currentLevelKey, 0);
        _currentLevelRetries = PlayerPrefs.GetInt(_currentLevelRetriesKey, 0);

        SetProgressText();
    }

    private void Update() {
        bool canRestart = _gameState == GameState.Playing && Input.GetKeyDown(KeyCode.R);
        if (canRestart) {
            RestartLevel();
        }

        bool canStart = _gameState != GameState.Playing && _gameState != GameState.FinishedAll && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter));
        if (canStart) {
            StartLevel();
        }

        bool canExit = _gameState != GameState.Menu && Input.GetKeyDown(KeyCode.Escape);
        if (canExit) {
            ExitLevel();
        }

#if UNITY_EDITOR
        // Editor clear level progress and restart level
        if (Input.GetKeyDown(KeyCode.X)) {
            _currentLevelIndex = 0;
            _currentLevelRetries = 0;
            PlayerPrefs.DeleteAll();
            StartLevel();
            SetProgressText();
        }
#endif
    }

    private void InitLevel() {
        ToggleMenuUi(false);
        ToggleGameUi(true);

        _player = Instantiate<PlayerController>(_playerPrefab, _startPosition, Quaternion.identity);
        _player.GameController = this;
        _camera.transform.position = _startPosition;
        _camera.Player = _player.transform;
        _currentLevel = Instantiate(_levels[_currentLevelIndex], _startPosition, Quaternion.identity);
    }

    public void RestartLevel() {
        _currentLevelRetries++;
        PlayerPrefs.SetInt(_currentLevelRetriesKey, _currentLevelRetries);
        PlayerPrefs.Save();

        StartLevel();
    }

    private void StartLevel() {
        _gameState = GameState.Playing;
        SetProgressText();
        CleanLevel();
        InitLevel();
    }

    public void FinishLevel() {
        _player.LockInput();

        var nextLevelIndex = _currentLevelIndex + 1;
        if (nextLevelIndex >= _levels.Count) {
            _gameState = GameState.FinishedAll;
            ToggleFinishUi(true);
            return;
        }

        _gameState = GameState.Finished;

        _currentLevelIndex++;
        PlayerPrefs.SetInt(_currentLevelKey, _currentLevelIndex);
        _currentLevelRetries = 0;
        PlayerPrefs.SetInt(_currentLevelRetriesKey, _currentLevelRetries);
        PlayerPrefs.Save();

        ToggleFinishUi();
    }

    private void ExitLevel() {
        _gameState = GameState.Menu;
        CleanLevel();
        ToggleGameUi(false);
        ToggleMenuUi(true);
    }

    private void CleanLevel() {
        if (_player) Destroy(_player.gameObject);
        if (_currentLevel) Destroy(_currentLevel);
    }

    private void ToggleFinishUi(bool allFinished = false) {
        _finishPanel.gameObject.SetActive(true);
        _finishedAllText.SetActive(allFinished);
        _finishText.SetActive(!allFinished);
        _nextButton.gameObject.SetActive(!allFinished);
        _restartButton.gameObject.SetActive(!allFinished);
    }

    private void ToggleMenuUi(bool enable) {
        _menuPanel.SetActive(enable);
    }

    private void ToggleGameUi(bool enable) {
        _gamePanel.SetActive(enable);
        _restartButton.gameObject.SetActive(enable);
        _finishPanel.gameObject.SetActive(false);
    }

    private void SetProgressText() {
        _levelText.text = $"Level: {_currentLevelIndex + 1}";
        _retriesText.text = $"Retries: {_currentLevelRetries}";
    }

}
