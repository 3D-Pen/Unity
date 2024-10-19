using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moving_figure : MonoBehaviour
{
    public float minMoveSpeed = 0.5f;     // 移動速度の最小値
    public float maxMoveSpeed = 2f;       // 移動速度の最大値
    public float minRotationSpeed = 30f;  // 回転速度の最小値
    public float maxRotationSpeed = 50f; // 回転速度の最大値
    public float scaleSpeed = 0.2f;       // スケール変化の速度
    public float minScale = 1f;           // 最小スケール
    public float maxScale = 1.5f;         // 最大スケール

    public float minAmplitude = 2f;       // 振幅の最小値
    public float maxAmplitude = 10f;      // 振幅の最大値

    private float moveSpeed;              // 移動速度（ランダムに変更される）
    private float amplitude;              // 振幅（ランダムに変更される）
    private float rotationSpeedX;         // X軸回転速度（ランダムに変更される）
    private float rotationSpeedY;         // Y軸回転速度（ランダムに変更される）
    private float rotationSpeedZ;         // Z軸回転速度（ランダムに変更される）
    private Vector3 originalPosition;     // 初期位置

    void Start()
    {
        // 初期位置を保存
        originalPosition = transform.position;

        // 初期の移動速度をランダムに設定
        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);

        // 初期の振幅をランダムに設定
        amplitude = Random.Range(minAmplitude, maxAmplitude);

        // 各軸の回転速度をランダムに設定
        rotationSpeedX = Random.Range(minRotationSpeed, maxRotationSpeed);
        rotationSpeedY = Random.Range(minRotationSpeed, maxRotationSpeed);
        rotationSpeedZ = Random.Range(minRotationSpeed, maxRotationSpeed);
    }

    void Update()
    {
        // サイン波で上下に移動する動き（初期設定された振幅と移動速度で固定）
        float newY = Mathf.Sin(Time.time * moveSpeed) * amplitude;
        transform.position = new Vector3(originalPosition.x, originalPosition.y + newY, originalPosition.z);

        // X, Y, Z軸回転（それぞれランダムに設定された速度で固定）
        transform.Rotate(new Vector3(rotationSpeedX, rotationSpeedY, rotationSpeedZ) * Time.deltaTime);

        // スケールを1倍から1.5倍まで変化させる動き
        float newScale = Mathf.PingPong(Time.time * scaleSpeed, maxScale - minScale) + minScale;
        transform.localScale = new Vector3(newScale, newScale, newScale);
    }
}