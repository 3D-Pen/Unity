using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LineConnect : MonoBehaviour
{
    public string filePath;  // CSVファイルのパス
    public Material lineMaterial;  // ライン用のマテリアル
    public float lineWidth = 5f; // ラインの太さ
    public int lineEndMarker = 9999; // 線の終了を示すマーカー
    public float positionRange = 200f;         // 位置のランダム範囲
    public Vector2 rotationRangeX = new Vector2(0, 360); // X軸の回転範囲
    public Vector2 rotationRangeY = new Vector2(0, 360); // Y軸の回転範囲
    public Vector2 rotationRangeZ = new Vector2(0, 360); // Z軸の回転範囲

    private static List<GameObject> createdObjects = new List<GameObject>();  // 生成されたオブジェクトを管理するリスト
    private static List<GameObject> parentObjects = new List<GameObject>();  // 親オブジェクトを管理するリスト
    private int maxObjects = 5;  // 最大保持オブジェクト数
    private static int colorIndex = 0;  // 色ローテーション用のインデックス

    // 色のループ用カラーリストを拡張
    private Color[] colors = new Color[]
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        Color.magenta,
        Color.cyan,
        new Color(1.0f, 0.5f, 0.0f), // Orange
        new Color(1.0f, 0.75f, 0.8f), // Pink
        new Color(0.5f, 0.5f, 1.0f), // Light Blue
        new Color(0.5f, 1.0f, 0.5f), // Light Green
        new Color(0.7f, 0.3f, 0.9f)  // Purple
    };

    private Color currentColor; // 現在のオブジェクトの色

    // Awakeメソッドでオブジェクトが生成されるたびに色を変える
    void Awake()
    {
        currentColor = colors[colorIndex];
        colorIndex = (colorIndex + 1) % colors.Length;  // 色リストの中でローテーション
    }

    public void Initialize()
    {
        ReadCSVAndDrawLines();
        ChangePositionAndRotation();
    }

    void ReadCSVAndDrawLines()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("CSVファイルが見つかりません: " + filePath);
            return;
        }

        string[] lines = File.ReadAllLines(filePath);
        List<Vector3> currentLinePoints = new List<Vector3>();
        int segmentCount = 0;  // 各線分に一意の名前を付けるためのカウンター

        foreach (string line in lines)
        {
            string[] data = line.Split(',');

            if (int.TryParse(data[0], out int marker) && marker == lineEndMarker)
            {
                if (currentLinePoints.Count > 0)
                {
                    CreateLineRenderer(currentLinePoints, segmentCount);
                    currentLinePoints.Clear();
                    segmentCount++;
                }
                continue;
            }

            if (data.Length == 3)
            {
                float x = float.Parse(data[0]);
                float y = float.Parse(data[1]);
                float z = float.Parse(data[2]);
                currentLinePoints.Add(new Vector3(x, y, z));
            }
        }

        if (currentLinePoints.Count > 0)
        {
            CreateLineRenderer(currentLinePoints, segmentCount);
        }

        // オブジェクトが最大数を超えた場合、古いオブジェクトを削除
        if (parentObjects.Count > maxObjects)
        {
            DeleteOldestParentObject();
        }
    }

    void CreateLineRenderer(List<Vector3> points, int segmentIndex)
    {
        GameObject lineSegment = new GameObject("LineSegment" + segmentIndex);
        lineSegment.transform.parent = this.transform;

        LineRenderer lineRenderer = lineSegment.AddComponent<LineRenderer>();

        List<Vector3> smoothPoints = InterpolateCatmullRom(points, 20); // Catmull-Rom補間

        lineRenderer.positionCount = smoothPoints.Count;
        lineRenderer.SetPositions(smoothPoints.ToArray());
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        if (lineMaterial != null)
        {
            lineRenderer.material = lineMaterial;
        }
        else
        {
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        lineRenderer.startColor = currentColor;
        lineRenderer.endColor = currentColor;
        lineRenderer.useWorldSpace = false;

        createdObjects.Add(lineSegment);

        // 親オブジェクトをリストに追加
        if (!parentObjects.Contains(this.gameObject))
        {
            parentObjects.Add(this.gameObject);
        }
    }

    void DeleteOldestParentObject()
    {
        if (parentObjects.Count > 0)
        {
            GameObject oldParentObject = parentObjects[0];
            parentObjects.RemoveAt(0);

            // 子オブジェクトも一緒に削除
            foreach (Transform child in oldParentObject.transform)
            {
                DestroyImmediate(child.gameObject);
            }
            DestroyImmediate(oldParentObject);
        }
    }

    void ChangePositionAndRotation()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(-positionRange-100, positionRange+100),
            Random.Range(-positionRange, positionRange),
            Random.Range(-positionRange-100, positionRange+100)
        );
        transform.position = randomPosition;

        Quaternion randomRotation = Quaternion.Euler(
            Random.Range(rotationRangeX.x, rotationRangeX.y),
            Random.Range(rotationRangeY.x, rotationRangeY.y),
            Random.Range(rotationRangeZ.x, rotationRangeZ.y)
        );
        transform.rotation = randomRotation;
    }

    // Catmull-Romスプライン補間
    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2.0f * p1) +
            (-p0 + p2) * t +
            (2.0f * p0 - 5.0f * p1 + 4.0f * p2 - p3) * t2 +
            (-p0 + 3.0f * p1 - 3.0f * p2 + p3) * t3
        );
    }

    // Catmull-Romスプライン補間を用いて滑らかな点を生成
    List<Vector3> InterpolateCatmullRom(List<Vector3> points, int segmentsPerCurve)
    {
        List<Vector3> smoothPoints = new List<Vector3>();

        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 p0 = (i == 0) ? points[i] : points[i - 1];
            Vector3 p1 = points[i];
            Vector3 p2 = points[i + 1];
            Vector3 p3 = (i + 2 < points.Count) ? points[i + 2] : points[i + 1];

            for (int j = 0; j <= segmentsPerCurve; j++)
            {
                float t = j / (float)segmentsPerCurve;
                Vector3 interpolatedPoint = CatmullRom(p0, p1, p2, p3, t);
                smoothPoints.Add(interpolatedPoint);
            }
        }

        return smoothPoints;
    }
}