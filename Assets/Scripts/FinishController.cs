using UnityEngine;

public class FinishController : MonoBehaviour {

    [SerializeField]
    private GameObject _finishBlock;

    private const string PlayerTag = "Player";

    private bool _scaleOut;
    private Vector3 _scaleStart;
    private Quaternion _rotationStart;
    private float _scaleTimer = 0.0f;
    private float _scaleTime = 0.3f;

    private void OnTriggerEnter2D(Collider2D col) {
        if (_scaleOut) return;
        if (col.gameObject.tag != PlayerTag) return;
        _finishBlock.SetActive(true);
        ScaleOut();
    }

    private void Update() {
        UpdateScale();
    }

    private void UpdateScale() {
        if (!_scaleOut) return;

        var lerp = Mathf.Clamp01(_scaleTimer / _scaleTime);
        transform.localScale = Vector3.Lerp(_scaleStart, Vector3.zero, lerp);
        transform.localRotation = Quaternion.Lerp(_rotationStart, Quaternion.Euler(0, 0, 180), lerp);

        if (transform.localScale.magnitude <= 0.2f) {
            Destroy(gameObject);
        }

        _scaleTimer += Time.deltaTime;
    }

    private void ScaleOut() {
        _scaleOut = true;
        _scaleTimer = 0.0f;
        _scaleStart = transform.localScale;
        _rotationStart = transform.localRotation;
    }
}
