using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour {

    [SerializeField]
    private float _smoothing = 5f;

	private Vector3 _initialOffset;

    public Transform Player { private get; set; }

	private void Awake() {
		_initialOffset = transform.position;
	}

    private void FixedUpdate() {
        if (!Player) return;
		
        transform.position = Vector3.Lerp(transform.position, Player.position + _initialOffset, _smoothing * Time.deltaTime);
    }
}