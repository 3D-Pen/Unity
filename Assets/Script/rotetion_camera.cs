using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotetion_camera : MonoBehaviour
{
    public Transform target;  // 回転の中心となる対象
    public float speed = 10f;  // 回転速度

    void Start()
    {
        // カメラが回転する中心点が指定されていない場合、原点を設定
        if (target == null)
        {
            GameObject centerObject = new GameObject("Center");
            target = centerObject.transform;
            target.position = Vector3.zero;  // 原点を中心にする
        }
    }

    void Update()
    {
        // カメラを target (原点) を中心にY軸周りに回転させる
        transform.RotateAround(target.position, Vector3.up, speed * Time.deltaTime);
    }
}
