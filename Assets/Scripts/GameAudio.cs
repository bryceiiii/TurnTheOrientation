using UnityEngine;

namespace TurnTheOrientation
{
    public sealed class GameAudio : MonoBehaviour
    {
        private AudioSource _source;
        private AudioClip _jump;
        private AudioClip _plate;
        private AudioClip _rotate;
        private AudioClip _win;
        private AudioClip _fall;
        private AudioClip _click;

        public void Initialize()
        {
            _source = gameObject.AddComponent<AudioSource>();
            _source.playOnAwake = false;
            _source.spatialBlend = 0f;
            _source.volume = 0.48f;
            _jump = MakeSweep("Jump", 280f, 620f, 0.14f, 0.28f);
            _plate = MakeSweep("Plate", 430f, 850f, 0.22f, 0.32f);
            _rotate = MakeSweep("Rotate", 190f, 95f, 0.28f, 0.2f);
            _fall = MakeSweep("Fall", 220f, 70f, 0.32f, 0.24f);
            _win = MakeArpeggio("Win", new[] { 392f, 523.25f, 659.25f, 783.99f }, 0.16f, 0.25f);
            _click = MakeSweep("Click", 660f, 440f, 0.08f, 0.18f);
        }

        public void PlayJump() => Play(_jump);
        public void PlayPlate() => Play(_plate);
        public void PlayRotate() => Play(_rotate);
        public void PlayWin() => Play(_win);
        public void PlayFall() => Play(_fall);
        public void PlayClick() => Play(_click);

        private void Play(AudioClip clip)
        {
            if (_source != null && clip != null)
            {
                _source.PlayOneShot(clip);
            }
        }

        private static AudioClip MakeSweep(
            string name,
            float startFrequency,
            float endFrequency,
            float duration,
            float volume)
        {
            const int sampleRate = 44100;
            int count = Mathf.CeilToInt(duration * sampleRate);
            var samples = new float[count];
            float phase = 0f;
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)count;
                float frequency = Mathf.Lerp(startFrequency, endFrequency, t);
                phase += frequency / sampleRate * Mathf.PI * 2f;
                float envelope = Mathf.Sin(Mathf.PI * Mathf.Pow(t, 0.8f)) * (1f - t);
                samples[i] = Mathf.Sin(phase) * envelope * volume;
            }

            AudioClip clip = AudioClip.Create(name, count, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private static AudioClip MakeArpeggio(
            string name,
            float[] frequencies,
            float stepDuration,
            float volume)
        {
            const int sampleRate = 44100;
            int stepSamples = Mathf.CeilToInt(stepDuration * sampleRate);
            var samples = new float[stepSamples * frequencies.Length];
            for (int note = 0; note < frequencies.Length; note++)
            {
                float phase = 0f;
                for (int i = 0; i < stepSamples; i++)
                {
                    float t = i / (float)stepSamples;
                    phase += frequencies[note] / sampleRate * Mathf.PI * 2f;
                    float envelope = (1f - t) * (1f - t);
                    samples[note * stepSamples + i] =
                        (Mathf.Sin(phase) * 0.8f + Mathf.Sin(phase * 2f) * 0.15f) * envelope * volume;
                }
            }

            AudioClip clip = AudioClip.Create(name, samples.Length, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
