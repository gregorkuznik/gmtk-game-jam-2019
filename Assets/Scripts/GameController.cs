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

    private Vector3 _startPosition = Vector3.zero;
    private PlayerController _player;
    private int _currentLevelIndex;
    private GameObject _currentLevel;
    private GameState _gameState = GameState.Menu;

    private void Awake() {
        ToggleUi();

        _playButton.onClick.AddListener(StartLevel);
        _restartButton.onClick.AddListener(StartLevel);
        _backButton.onClick.AddListener(ExitLevel);
        _nextButton.onClick.AddListener(StartLevel);

        _currentLevelIndex = PlayerPrefs.GetInt(_currentLevelKey, 0);
    }

    private void Update() {
        if (_gameState == GameState.Playing && Input.GetKeyDown(KeyCode.R)) {
            StartLevel();
        }
        if (_gameState != GameState.Menu && Input.GetKeyDown(KeyCode.Escape)) {
            ExitLevel();
        }
        if (_gameState != GameState.Playing && _gameState != GameState.FinishedAll && Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            StartLevel();
        }
#if UNITY_EDITOR
        // Clear current level and prefs
        if (Input.GetKeyDown(KeyCode.C)) {
            _currentLevelIndex = 0;
            PlayerPrefs.DeleteAll();
        }
#endif
    }

    private void InitLevel() {
        ToggleUi(true);

        _player = Instantiate<PlayerController>(_playerPrefab, _startPosition, Quaternion.identity);
        _player.GameController = this;
        _camera.transform.position = _startPosition;
        _camera.Player = _player.transform;
        _currentLevel = Instantiate(_levels[_currentLevelIndex], _startPosition, Quaternion.identity);
    }

    public void StartLevel() {
        _gameState = GameState.Playing;
        CleanLevel();
        InitLevel();
    }

    public void FinishLevel() {
        _player.LockInput();

        var nextLevelIndex = _currentLevelIndex + 1;
        if (nextLevelIndex >= _levels.Count) {
            _gameState = GameState.FinishedAll;
            _finishedAllText.SetActive(true);
            _finishText.SetActive(false);
            _nextButton.gameObject.SetActive(false);
            _finishPanel.gameObject.SetActive(true);
            return;
        }

         _gameState = GameState.Finished;

        _currentLevelIndex++;
        PlayerPrefs.SetInt(_currentLevelKey, _currentLevelIndex);
        PlayerPrefs.Save();

        _finishedAllText.SetActive(false);
        _finishText.SetActive(true);
        _nextButton.gameObject.SetActive(true);
        _finishPanel.gameObject.SetActive(true);
    }

    private void ExitLevel() {
        _gameState = GameState.Menu;
        CleanLevel();
        ToggleUi();
    }

    private void CleanLevel() {
        if (_player) Destroy(_player.gameObject);
        if (_currentLevel) Destroy(_currentLevel);
    }

    private void ToggleUi(bool game = false) {
        _menuPanel.SetActive(!game);
        _gamePanel.SetActive(game);
        if (game) {
            _restartButton.gameObject.SetActive(true);
            _finishPanel.gameObject.SetActive(false);
        }
    }

}
