using UnityEngine;

public class PlayerController : MonoBehaviour {

    private const string DeadZoneTag = "DeadZone";
    private const string FinishTag = "Finish";

    [SerializeField]
    private float _maxSpeed = 10.0f;
    [SerializeField]
    private float _jumpForce = 14.0f;
    [SerializeField]
    private LayerMask _whatIsGround;

    private float _startScaleX;
    private bool _grounded = false;
    private bool _secondJump = false;
    private ContactFilter2D _contactFilter;
    private ContactPoint2D[] _contactPoints = new ContactPoint2D[4];
    private Rigidbody2D _rigbody;
    private bool _lockInput;

    private bool _scaleOut;
    private Vector3 _scaleStart;
    private Quaternion _rotationStart;
    private float _scaleTimer = 0.0f;
    private float _scaleTime = 0.5f;

    public GameController GameController { private get; set; }

    private void LockInput() {
        _lockInput = true;
        _rigbody.velocity = Vector3.zero;
        _rigbody.isKinematic = true;
    }

    private void Start() {
        _startScaleX = transform.localScale.x;
        _rigbody = GetComponent<Rigidbody2D>();
        _contactFilter.SetLayerMask(_whatIsGround);
    }

    private void FixedUpdate() {
        _grounded = false;
        int count = _rigbody.GetContacts(_contactFilter, _contactPoints);
        for (int i = 0; i < count; i++) {
            // When contact normal.y is 1 then player is grounded
            if (_contactPoints[i].normal.y == 1) {
                _grounded = true;
                _secondJump = true;
                break;
            }
        }
    }

    private void Update() {
        UpdateMove();
        UpdateScale();
    }

    private void UpdateMove() {
        if (_lockInput) return;

        // Jump
        var canJump = _grounded || _secondJump;
        var jump = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        if (canJump && jump) {
            if (!_grounded && _secondJump) {
                _secondJump = false;
            }

            _rigbody.velocity = new Vector2(_rigbody.velocity.x, _jumpForce);
        }

        // Move left / right
        float move = Input.GetAxis("Horizontal");
        _rigbody.velocity = new Vector2(_maxSpeed * move, _rigbody.velocity.y);

        if (move > 0 && transform.localScale.x == -_startScaleX) {
            Vector3 scale = transform.localScale;
            scale.x = _startScaleX;
            transform.localScale = scale;
        } else if (move < 0 && transform.localScale.x == _startScaleX) {
            Vector3 scale = transform.localScale;
            scale.x = -_startScaleX;
            transform.localScale = scale;
        }
    }

    private void UpdateScale() {
        if (!_scaleOut) return;
        if (_scaleTimer > _scaleTime) {
            _scaleOut = false;
            gameObject.SetActive(false);
            GameController.FinishLevel();
            return;
        }

        var lerp = Mathf.Clamp01(_scaleTimer / _scaleTime);
        transform.localScale = Vector3.Lerp(_scaleStart, Vector3.zero, lerp);
        transform.localRotation = Quaternion.Lerp(_rotationStart, Quaternion.Euler(0, 0, 180), lerp);

        _scaleTimer += Time.deltaTime;
    }

    private void ScaleOut() {
        LockInput();
        _scaleOut = true;
        _scaleTimer = 0.0f;
        _scaleStart = transform.localScale;
        _rotationStart = transform.localRotation;
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (!_scaleOut && col.tag == DeadZoneTag) {
            GameController.RestartLevel();
        }
        if (col.tag == FinishTag) {
            ScaleOut();
        }
    }

}
