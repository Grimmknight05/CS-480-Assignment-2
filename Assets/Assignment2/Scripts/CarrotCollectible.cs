using UnityEngine;
using System.Collections;

namespace StealthGame
{
    public class CarrotCollectible : MonoBehaviour
    {
        [SerializeField] private ParticleSystem pickupBurstParticles;
        [SerializeField] private AudioClip biteClip;
        [SerializeField] [Range(0f, 1f)] private float biteVolume = 1f;
        [Header("Idle Hover")]
        [SerializeField] private bool hoverWhileIdle = true;
        [SerializeField] private float hoverHeight = 0.1f;
        [SerializeField] private float hoverSpeed = 2f;
        [Header("Optional Speed Boost")]
        [SerializeField] private bool giveSpeedBoost = true;
        [SerializeField] private float speedBoostAmount = 1.5f;
        [SerializeField] private float speedBoostDuration = 3f;
        private Vector3 m_BasePosition;
        private bool m_IsCollected;

        private void Start()
        {
            m_BasePosition = transform.position;
        }

        private void Update()
        {
            if (!hoverWhileIdle || m_IsCollected)
                return;

            float yOffset = Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
            transform.position = m_BasePosition + new Vector3(0f, yOffset, 0f);
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player == null)
                return;
            m_IsCollected = true;

            PlayPickupFeedback();
            ApplySpeedBoost(player);
            Destroy(gameObject);
        }

        private void PlayPickupFeedback()
        {
            if (biteClip != null)
                AudioSource.PlayClipAtPoint(biteClip, transform.position, biteVolume);

            PlayPickupBurstParticles();
        }

        private void PlayPickupBurstParticles()
        {
            if (pickupBurstParticles == null)
                return;

            pickupBurstParticles.transform.SetParent(null);
            pickupBurstParticles.Play();

            ParticleSystem.MainModule main = pickupBurstParticles.main;
            float startLifeUpper = Mathf.Max(main.startLifetime.constantMin, main.startLifetime.constantMax);
            if (startLifeUpper <= 0f)
                startLifeUpper = main.startLifetime.constant;

            float destroyAfter = main.duration + startLifeUpper + 0.25f;
            Destroy(pickupBurstParticles.gameObject, destroyAfter);
        }

        private void ApplySpeedBoost(PlayerMovement player)
        {
            if (!giveSpeedBoost || player == null)
                return;

            player.walkSpeed += speedBoostAmount;
            player.StartCoroutine(RemoveSpeedBoostAfterDelay(player, speedBoostAmount, speedBoostDuration));
        }

        private static IEnumerator RemoveSpeedBoostAfterDelay(PlayerMovement player, float amount, float duration)
        {
            if (duration > 0f)
                yield return new WaitForSeconds(duration);

            if (player != null)
                player.walkSpeed -= amount;
        }
    }
}
