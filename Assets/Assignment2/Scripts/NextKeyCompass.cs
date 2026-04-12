using UnityEngine;

namespace StealthGame
{
    /// <summary>
    /// Editor-built next-key compass. Wire CompassPivot / CompassNeedle in the Inspector; no runtime primitives.
    /// </summary>
    [RequireComponent(typeof(PlayerMovement))]
    public class NextKeyCompass : MonoBehaviour
    {
        [SerializeField] private Transform compassPivot;
        [SerializeField] private Transform compassNeedle;
        [SerializeField] private ParticleSystem compassParticles;

        [SerializeField] private float minHorizontalDistance = 0.25f;
        [Tooltip("Scales the needle with dot(forward, toKey): smaller when the key is behind you.")]
        [SerializeField] private bool useDotProductForVisibility = true;

        private PlayerMovement m_Player;
        private Vector3 m_NeedleBaseLocalScale = Vector3.one;
        private Transform m_LastKeyTarget;

        private static readonly string[] s_KeyOrder = { "keyRed", "keyBlue" };

        private void Awake()
        {
            m_Player = GetComponent<PlayerMovement>();
            if (compassPivot == null)
            {
                Debug.LogError($"{nameof(NextKeyCompass)} on {name}: assign {nameof(compassPivot)}.", this);
                enabled = false;
            }
        }

        private void Start()
        {
            if (compassNeedle != null)
                m_NeedleBaseLocalScale = compassNeedle.localScale;
        }

        private void LateUpdate()
        {
            if (compassPivot == null || m_Player == null)
                return;

            Transform keyTarget = GetNextKeyTransform();
            if (keyTarget == null)
            {
                SetCompassActive(false);
                return;
            }

            SetCompassActive(true);

            Vector3 playerPos = transform.position;
            Vector3 keyPos = keyTarget.position;

            Vector3 d = keyPos - playerPos;
            Vector3 dHorizontal = new Vector3(d.x, 0f, d.z);
            float distSq = dHorizontal.sqrMagnitude;
            if (distSq < minHorizontalDistance * minHorizontalDistance)
            {
                SetCompassActive(false);
                return;
            }

            Vector3 u = dHorizontal / Mathf.Sqrt(distSq);

            Vector3 f = new Vector3(transform.forward.x, 0f, transform.forward.z);
            if (f.sqrMagnitude < 1e-8f)
                f = Vector3.forward;
            else
                f.Normalize();

            float dot = Vector3.Dot(f, u);
            Vector3 cross = Vector3.Cross(f, u);
            float signedYawRad = Mathf.Atan2(cross.y, dot);
            float signedYawDeg = signedYawRad * Mathf.Rad2Deg;

            compassPivot.localRotation = Quaternion.Euler(0f, signedYawDeg, 0f);

            if (useDotProductForVisibility && compassNeedle != null)
            {
                float t = Mathf.Clamp01((dot + 1f) * 0.5f);
                float scaleMul = Mathf.Lerp(0.35f, 1f, t);
                compassNeedle.localScale = m_NeedleBaseLocalScale * scaleMul;
            }
            else if (compassNeedle != null)
            {
                compassNeedle.localScale = m_NeedleBaseLocalScale;
            }

            if (compassParticles != null && keyTarget != m_LastKeyTarget)
            {
                compassParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                compassParticles.Play();
                m_LastKeyTarget = keyTarget;
            }
        }

        private Transform GetNextKeyTransform()
        {
            foreach (string keyName in s_KeyOrder)
            {
                if (m_Player.OwnKey(keyName))
                    continue;

                Key[] keys = Object.FindObjectsByType<Key>(FindObjectsSortMode.None);
                for (int i = 0; i < keys.Length; i++)
                {
                    if (keys[i] != null && keys[i].KeyName == keyName)
                        return keys[i].transform;
                }
            }

            return null;
        }

        private void SetCompassActive(bool value)
        {
            if (!value)
            {
                m_LastKeyTarget = null;
                if (compassParticles != null)
                    compassParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            if (compassPivot != null)
                compassPivot.gameObject.SetActive(value);
        }
    }
}
