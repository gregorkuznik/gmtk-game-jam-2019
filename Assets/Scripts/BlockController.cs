using UnityEngine;

public class BlockController : MonoBehaviour {

    private const string PlayerTag = "Player";

    [SerializeField]
    private Color _defaultColor;
    [SerializeField]
    private Color _touchedColor;
    [SerializeField]
    private SpriteRenderer _renderer;
    [SerializeField]
    private ParticleSystem _touchedVfx;

    private bool _touched;
    private bool _scaleOut;
    private Vector3 _scaleStart;
    private float _scaleTimer = 0.0f;
    private float _scaleTime = 0.2f;

    private void Awake() {
        _renderer.color = _defaultColor;
        _touchedVfx.Stop();
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if (!_touched && col.gameObject.tag != PlayerTag) return;
        _touched = true;
        _renderer.color = _touchedColor;
        _touchedVfx.Play();
    }

    private void OnCollisionExit2D(Collision2D col) {
        if (_scaleOut) return;
        if (col.gameObject.tag != PlayerTag) return;
        ScaleOut();
    }

    private void Update() {
        UpdateScale();
    }

    private void UpdateScale() {
        if (!_scaleOut) return;
        if (_scaleTimer > _scaleTime) {
            _scaleOut = false;
            Destroy(gameObject);
            return;
        }

        var lerp = Mathf.Clamp01(_scaleTimer / _scaleTime);
        transform.localScale = Vector3.Lerp(_scaleStart, Vector3.zero, lerp);

        _scaleTimer += Time.deltaTime;
    }

    private void ScaleOut() {
        _scaleOut = true;
        _scaleTimer = 0.0f;
        _scaleStart = transform.localScale;
    }

}
