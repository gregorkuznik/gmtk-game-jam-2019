using UnityEngine;

public class BlockController : MonoBehaviour {

    private const string PlayerTag = "Player";

    private bool _scaleOut;
    private Vector3 _scaleStart;
    private Quaternion _rotationStart;
    private float _scaleTimer = 0.0f;
    private float _scaleTime = 0.5f;
    
    private void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag != PlayerTag) return;
        ScaleOut();
    }

    private void Update() {
        UpdateScale();
    }

    private void UpdateScale() {
        if (!_scaleOut) return;

        var lerp = Mathf.Clamp01(_scaleTimer / _scaleTime);
        transform.localScale = Vector3.Lerp(_scaleStart, Vector3.zero, lerp);

        if (transform.localScale.magnitude <= 0.2f) {
            _scaleOut = false;
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
