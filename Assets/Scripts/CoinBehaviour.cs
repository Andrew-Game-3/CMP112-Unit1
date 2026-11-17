using UnityEngine;

public class CoinBehaviour : MonoBehaviour
{
private void OnTriggerEnter2D(Collider2D other) {   
       // Destroy the collectible
       Destroy(gameObject);
}
}
