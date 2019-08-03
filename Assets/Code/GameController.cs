using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

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

    private void Awake() {
        ToggleUi();

        _playButton.onClick.AddListener(PlayAction);
        _restartButton.onClick.AddListener(RestartAction);
        _backButton.onClick.AddListener(BackAction);
        _nextButton.onClick.AddListener(NextAction);

        _currentLevelIndex = PlayerPrefs.GetInt(_currentLevelKey, 0);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            RestartLevel();
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ExitLevel();
        }
#if UNITY_EDITOR
        // Clear current level and prefs
        if (Input.GetKeyDown(KeyCode.C)) {
            _currentLevelIndex = 0;
            PlayerPrefs.DeleteAll();
        }
#endif
    }

    private void StartLevel() {
        ToggleUi(true);
        
        _player = Instantiate<PlayerController>(_playerPrefab, _startPosition, Quaternion.identity);
        _player.GameController = this;
        _camera.transform.position = _startPosition;
        _camera.Player = _player.transform;
        _currentLevel = Instantiate(_levels[_currentLevelIndex], _startPosition, Quaternion.identity);
    }

    public void RestartLevel() {
        CleanLevel();
        StartLevel();
    }

    public void FinishLevel() {
        _player.LockInput();

        var nextLevelIndex = _currentLevelIndex + 1;
        if (nextLevelIndex >= _levels.Count) {
            _finishPanel.gameObject.SetActive(true);
            _nextButton.gameObject.SetActive(false);
            return;
        }

        _currentLevelIndex++;
        PlayerPrefs.SetInt(_currentLevelKey, _currentLevelIndex);
        PlayerPrefs.Save();

        _finishPanel.gameObject.SetActive(true);
        _nextButton.gameObject.SetActive(true);
    }

    private void StartNextLevel() {
        CleanLevel();
        StartLevel();
    }

    private void ExitLevel() {
        CleanLevel();
        ToggleUi();
    }

    private void CleanLevel() {
        Destroy(_player.gameObject);
        Destroy(_currentLevel);
    }

    private void ToggleUi(bool game = false) {
        _menuPanel.SetActive(!game);
        _gamePanel.SetActive(game);
        if (game) {
            _restartButton.gameObject.SetActive(true);
            _finishPanel.gameObject.SetActive(false);
        }
    }

    #region ButtonActions
    private void PlayAction() {
        StartLevel();
    }

    private void RestartAction() {
        RestartLevel();
    }

    private void BackAction() {
        ExitLevel();
    }

    private void NextAction() {
        StartNextLevel();
    }
    #endregion

}
