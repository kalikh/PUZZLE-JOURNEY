using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CaravanSecrets.Features.Gameplay
{
    public sealed class GameplayFeedback : MonoBehaviour
    {
        [SerializeField] private AudioClip selectionSound;
        [SerializeField] private AudioClip moveSound;
        [SerializeField] private AudioClip invalidSound;
        [SerializeField] private AudioClip completionSound;
        private AudioSource _audioSource;
        private ParticleSystem _dust;

        private void Awake()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            CreateDust();
        }

        public void PlaySelection(Transform target) { Play(selectionSound); StartCoroutine(Pulse(target, 1.08f, 0.12f)); }
        public void PlayMove(Vector3 position) { Play(moveSound); _dust.transform.position = position; _dust.Emit(7); }
        public void PlayInvalid(Transform target) { Play(invalidSound); StartCoroutine(Shake(target)); }
        public void PlayCompletion(IEnumerable<SpriteRenderer> gates)
        { Play(completionSound); foreach (var gate in gates) if (gate != null) StartCoroutine(Pulse(gate.transform, 1.14f, 0.4f)); }
        private void Play(AudioClip clip) { if (clip != null) _audioSource.PlayOneShot(clip); }

        private IEnumerator Pulse(Transform target, float amount, float duration)
        {
            var original = target.localScale;
            for (var elapsed = 0f; elapsed < duration; elapsed += Time.unscaledDeltaTime)
            { var wave = Mathf.Sin(Mathf.Clamp01(elapsed / duration) * Mathf.PI); target.localScale = original * Mathf.Lerp(1, amount, wave); yield return null; }
            target.localScale = original;
        }

        private static IEnumerator Shake(Transform target)
        {
            var original = target.position;
            for (var step = 0; step < 6; step++)
            { target.position = original + Vector3.right * (step % 2 == 0 ? 0.08f : -0.08f); yield return new WaitForSecondsRealtime(0.035f); }
            target.position = original;
        }

        private void CreateDust()
        {
            var item = new GameObject("Cart Dust", typeof(ParticleSystem)); item.transform.SetParent(transform, false);
            _dust = item.GetComponent<ParticleSystem>();
            var main = _dust.main; main.loop = false; main.playOnAwake = false; main.startLifetime = 0.45f; main.startSpeed = 0.5f;
            main.startSize = 0.11f; main.startColor = new Color(0.82f, 0.62f, 0.30f, 0.65f); main.simulationSpace = ParticleSystemSimulationSpace.World;
            var emission = _dust.emission; emission.enabled = false;
            var shape = _dust.shape; shape.shapeType = ParticleSystemShapeType.Circle; shape.radius = 0.18f;
            var renderer = item.GetComponent<ParticleSystemRenderer>(); renderer.sortingOrder = 40;
            var shader = Shader.Find("Sprites/Default"); if (shader != null) renderer.material = new Material(shader);
        }
    }
}
