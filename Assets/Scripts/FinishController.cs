using UnityEngine;

public class FinishController : MonoBehaviour {
    [SerializeField]
    private GameObject _finishBlock;

    private const string PlayerTag = "Player";

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag != PlayerTag) return;
        _finishBlock.SetActive(true);
        Destroy(gameObject);
    }
}
