using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public ParticleSystem HurtEffect;
   public Transform destination;
 GameObject player;

 private void Awake ()
 {
    player = GameObject.FindGameObjectWithTag("Player");
 }

 private void OnTriggerEnter2D(Collider2D collission)
 {
    if(collission.CompareTag("Player"))
    {
        player.transform.position = destination.transform.position;
        HurtEffect.Play();
    }
 }
}
