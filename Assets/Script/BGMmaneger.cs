using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMmaneger : MonoBehaviour
{
    public AudioClip[] bgmClips; // BGMのオーディオクリップを格納する配列
    public Material[] skyboxMaterials; // Skyboxのマテリアルを格納する配列
    public float switchInterval = 300f; // BGMを切り替える時間（秒）
    public float fadeDuration = 0f; // フェードにかかる時間（秒）

    private AudioSource audioSource;
    private int currentClipIndex = 0;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // AudioSourceを追加
        audioSource.loop = false; // ループ再生を無効にする
        StartCoroutine(SwitchBGM()); // BGM切り替えのコルーチンを開始
    }

    IEnumerator SwitchBGM()
    {
        while (true)
        {
            // 現在のBGMを再生
            audioSource.clip = bgmClips[currentClipIndex];
            audioSource.Play();

            // BGMが終わるまで待つ
            yield return new WaitWhile(() => audioSource.isPlaying);

            // フェードアウトを開始
            yield return FadeOut();

            // 次のBGMのインデックスを計算
            currentClipIndex = (currentClipIndex + 1) % bgmClips.Length;

            // Skyboxの変更
            ChangeSkybox();

            // フェードインを開始
            yield return FadeIn();
        }
    }

    IEnumerator FadeOut()
    {
        float startVolume = audioSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null; // 1フレーム待機
        }

        audioSource.volume = 0; // 最終的に音量を0に設定
    }

    IEnumerator FadeIn()
    {
        audioSource.volume = 0; // 音量を0に設定してから再生を開始
        audioSource.Play(); // 新しいBGMを再生

        float targetVolume = 1; // 最終的な音量
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, targetVolume, t / fadeDuration);
            yield return null; // 1フレーム待機
        }

        audioSource.volume = targetVolume; // 最終的な音量に設定
    }

    void ChangeSkybox()
    {
        // 次のSkyboxのインデックスを計算
        int skyboxIndex = currentClipIndex % skyboxMaterials.Length;

        // Skyboxを変更
        RenderSettings.skybox = skyboxMaterials[skyboxIndex];
    }
}