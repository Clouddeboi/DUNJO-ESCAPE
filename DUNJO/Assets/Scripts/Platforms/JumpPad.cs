using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField ]private float bounce = 30f;
    //AudioManager AudioManager;

    // private void Awake()
    // {
    //     //AudioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    // }

    private void OnCollisionEnter2D(Collision2D collision)//check if something collided with object
    {
        if(collision.gameObject.CompareTag("Player"))//object collided set to as player
        {
            //AudioManager.PlaySFX(AudioManager.JumpPad);
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bounce, ForceMode2D.Impulse);//direction multiplied by bounce value
        }
    }
}