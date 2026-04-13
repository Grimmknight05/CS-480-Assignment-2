using UnityEngine;
using System;
namespace StealthGame
{

    public class LittleGuyGoesBomb : MonoBehaviour
    {
        public Material originalMaterial;
        public Material deadMaterial;

        
        //Base function
        [SerializeField] private string KeyName = "key1";

        public enum LittleGuyState //Little Guy State
        {
            Safe,
            Exploding,
            Dead
        }
        public LittleGuyState state = LittleGuyState.Safe;// Safe or Unsafe INIT

        public ParticleSystem explosionParticleSystem; //Particle System

        [SerializeField] public Renderer rend; //Particle System
        private void Start()
        {
            rend = GetComponent<Renderer>();
            explosionParticleSystem.Stop();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            
            PlayerMovement player = other.gameObject.GetComponent<PlayerMovement>();//Get ref to player on trigger
            Debug.Log("Collided");
            Debug.Log($"{player.OwnKey(KeyName)}");

            //this wasn't a player
            if (player == null)
                return;

            if (player.OwnKey(KeyName) && state == LittleGuyState.Safe)//If player has key in inventory, and the thing has not exploded
            {
                Debug.Log("Oh Noes He is Ded.");
                //Destroy(gameObject);
                state = LittleGuyState.Exploding;

            }
        }
        private void Update()
        {
            if (state == LittleGuyState.Exploding)//Exploding state
            {
                explosionParticleSystem.Play();//EXPLODEEE!!
                state = LittleGuyState.Dead;//Little Guy is Dead, break loop
                rend.material = deadMaterial;
            }
        }
    }
}
