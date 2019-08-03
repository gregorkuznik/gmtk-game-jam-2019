using UnityEngine;

public class BlockController : MonoBehaviour {

    private const string PlayerTag = "Player";
    private bool _touched;
    
    private void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag != PlayerTag) return;
        _touched = true;
    }

    private void OnCollisionExit2D(Collision2D col) {
        if (col.gameObject.tag != PlayerTag || !_touched) return;
        Destroy(gameObject);
    }

}
