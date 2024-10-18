using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moving_figure : MonoBehaviour
{
    public float moveSpeed = 1f;  // 移動速度
    public float rotationSpeed = 50f;  // 回転速度
    public float amplitude = 8f;  // 振幅
    public float scaleSpeed = 0.2f;  // スケール変化の速度
    public float minScale = 1f;  // 最小スケール
    public float maxScale = 1.5f;  // 最大スケール

    private Vector3 originalPosition;

    void Start()
    {
        // 初期位置を保存
        originalPosition = transform.position;
    }

    void Update()
    {
        // サイン波で上下に移動する動き
        float newY = Mathf.Sin(Time.time * moveSpeed) * amplitude;
        transform.position = new Vector3(originalPosition.x, originalPosition.y + newY, originalPosition.z);

        // Y軸回転
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // スケールを1倍から1.5倍まで変化させる動き
        float newScale = Mathf.PingPong(Time.time * scaleSpeed, maxScale - minScale) + minScale;
        transform.localScale = new Vector3(newScale, newScale, newScale);
    }
}