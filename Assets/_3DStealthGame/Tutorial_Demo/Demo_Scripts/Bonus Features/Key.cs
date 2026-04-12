using UnityEngine;

namespace StealthGame
{
    public class Key : MonoBehaviour
    {
        public string KeyName = "key1";

        [SerializeField]
        [Tooltip("Optional pickup burst; unparented on collect so it survives key destroy.")]
        private ParticleSystem pickupParticles;
        [SerializeField] private AudioClip pickupClip;
        [SerializeField] [Range(0f, 1f)] private float pickupVolume = 1f;

        private void OnTriggerEnter(Collider other)
        {
            PlayerMovement player = other.gameObject.GetComponent<PlayerMovement>();

            if (player == null)
                return;

            player.AddKey(KeyName);
            PlayPickupFeedback();
            Destroy(gameObject);
        }

        private void PlayPickupFeedback()
        {
            if (pickupClip != null)
                AudioSource.PlayClipAtPoint(pickupClip, transform.position, pickupVolume);

            if (pickupParticles == null)
                return;

            pickupParticles.transform.SetParent(null);
            pickupParticles.Play();

            ParticleSystem.MainModule main = pickupParticles.main;
            float startLifeUpper = Mathf.Max(main.startLifetime.constantMin, main.startLifetime.constantMax);
            if (startLifeUpper <= 0f)
                startLifeUpper = main.startLifetime.constant;
            float destroyAfter = main.duration + startLifeUpper + 0.25f;
            Destroy(pickupParticles.gameObject, destroyAfter);
        }
    }
}
