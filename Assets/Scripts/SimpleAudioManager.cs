using UnityEngine;

public class SimpleAudioManager : MonoBehaviour
{
    public static SimpleAudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip bgmClip;
    public AudioSource customBgmSource; // Optional
    public AudioClip collectSFX;
    public AudioClip damageSFX;
    public AudioClip winSFX;
    public AudioClip repairSFX;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure AudioSources exist
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.volume = 0.25f;
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.volume = 0.6f;
        }

        GenerateProceduralAudio();
    }

    private void Start()
    {
        PlayBGM();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayCollect() => PlaySFX(collectSFX);
    public void PlayDamage() => PlaySFX(damageSFX);
    public void PlayWin() => PlaySFX(winSFX);
    public void PlayRepair() => PlaySFX(repairSFX);

    public void PlayBGM()
    {
        if (bgmClip != null && bgmSource != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.Play();
        }
    }

    private void GenerateProceduralAudio()
    {
        // Only generate if not assigned
        if (bgmClip == null) bgmClip = CreateProceduralBGM();
        if (collectSFX == null) collectSFX = CreateProceduralSFX(1000f, 0.15f, true); // High pitch coin beep
        if (damageSFX == null) damageSFX = CreateProceduralSFX(150f, 0.3f, false);  // Low pitch explosion
        if (winSFX == null) winSFX = CreateProceduralWinSFX();                      // Fanfare
        if (repairSFX == null) repairSFX = CreateProceduralSFX(600f, 0.2f, true);   // Repair sound
    }

    private AudioClip CreateProceduralSFX(float frequency, float duration, bool isBeep)
    {
        int sampleRate = 44100;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            if (isBeep)
            {
                // Simple sine wave beep with fade out
                samples[i] = Mathf.Sin(2 * Mathf.PI * frequency * t) * (1f - t / duration);
            }
            else
            {
                // Low noise / explosion with frequency decay
                float decayFreq = frequency * (1f - (t / duration) * 0.8f);
                float sinVal = Mathf.Sin(2 * Mathf.PI * decayFreq * t);
                float noise = (Random.value * 2f - 1f) * 0.3f; // add some noise
                samples[i] = (sinVal + noise) * (1f - t / duration);
            }
        }

        AudioClip clip = AudioClip.Create("ProceduralSFX", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateProceduralWinSFX()
    {
        int sampleRate = 44100;
        float duration = 0.5f;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            // Arpeggio sound: changes pitch halfway through
            float freq = (t < 0.15f) ? 523.25f : (t < 0.3f) ? 659.25f : 783.99f; // C5, E5, G5
            samples[i] = Mathf.Sin(2 * Mathf.PI * freq * t) * (1f - t / duration) * 0.5f;
        }

        AudioClip clip = AudioClip.Create("ProceduralWin", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateProceduralBGM()
    {
        int sampleRate = 22050; // lower sample rate for simple background ambient drone
        float duration = 4.0f; // 4 seconds loop
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            // Pirate-y sea ambient chord drone (mix of a fundamental D minor chords)
            float freq1 = 146.83f; // D3
            float freq2 = 174.61f; // F3
            float freq3 = 220.00f; // A3
            
            // Soothing slow waves
            float waveValue1 = Mathf.Sin(2 * Mathf.PI * freq1 * t + Mathf.Sin(2 * Mathf.PI * 0.5f * t));
            float waveValue2 = Mathf.Sin(2 * Mathf.PI * freq2 * t) * 0.5f;
            float waveValue3 = Mathf.Sin(2 * Mathf.PI * freq3 * t) * 0.3f;

            // Fade in/out slightly at ends to avoid popping on loop
            float envelope = 1f;
            if (t < 0.2f) envelope = t / 0.2f;
            else if (t > duration - 0.2f) envelope = (duration - t) / 0.2f;

            samples[i] = (waveValue1 + waveValue2 + waveValue3) * 0.15f * envelope;
        }

        AudioClip clip = AudioClip.Create("ProceduralBGM", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
