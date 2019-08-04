using UnityEngine;

public class ScaleAwake : MonoBehaviour {

    [SerializeField]
    private Vector3 _fromScale;
    [SerializeField]
    private Vector3 _toScale;
    [SerializeField]
    private float _scaleTime = 0.5f;

    private bool _scale;
    private float _scaleTimer = 0.0f;

    private void Awake() {
        _scale = true;
    }

    private void Update() {
        UpdateScale();
    }

    private void UpdateScale() {
        if (!_scale) return;
        if (_scaleTimer > _scaleTime) {
            _scale = false;
            return;
        }

        var lerp = Mathf.Clamp01(_scaleTimer / _scaleTime);
        transform.localScale = Vector3.Lerp(_fromScale, _toScale, lerp);

        _scaleTimer += Time.deltaTime;
    }

}
