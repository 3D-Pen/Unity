using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class CSVWatcher : MonoBehaviour
{
    private FileSystemWatcher watcher;
    public string folderToWatch;  // 監視するCSVファイルのフォルダ
    private Dictionary<string, float> recentFiles = new Dictionary<string, float>();  // 最近処理されたファイルのリスト
    public float debounceTime = 1f;  // 同じファイルに対するイベントの抑制時間（秒）

    void Start()
    {
        // フォルダが設定されていない場合、デフォルトでpersistentDataPathを使用
        if (string.IsNullOrEmpty(folderToWatch))
        {
            folderToWatch = Application.persistentDataPath + "/CSVFiles";
        }

        StartWatching();
    }

    // フォルダの監視を開始
    void StartWatching()
    {
        if (!Directory.Exists(folderToWatch))
        {
            Directory.CreateDirectory(folderToWatch);
        }

        watcher = new FileSystemWatcher(folderToWatch, "*.csv");
        watcher.Created += OnCSVFileAdded;
        watcher.EnableRaisingEvents = true;

        Debug.Log("フォルダ監視を開始: " + folderToWatch);
    }

    // 新しいCSVファイルが追加された時の処理
    private void OnCSVFileAdded(object sender, FileSystemEventArgs e)
    {
        string filePath = e.FullPath;

        // 最近処理したファイルかどうかを確認し、指定の時間内であれば処理をスキップ
        if (recentFiles.ContainsKey(filePath) && Time.time - recentFiles[filePath] < debounceTime)
        {
            Debug.LogWarning("同じファイルが短時間に検出されました: " + filePath);
            return;
        }

        recentFiles[filePath] = Time.time;  // ファイルと現在の時刻を記録

        Debug.Log($"CSVファイルが追加されました: {filePath}");
        // メインスレッドで処理を実行
        UnityMainThreadDispatcher.Instance().Enqueue(() => LoadCSVAndCreateObject(filePath));
    }

    // CSVファイルを読み込んで新しいオブジェクトを作成
    private void LoadCSVAndCreateObject(string filePath)
    {
        GameObject lineObject = new GameObject("LineObject_" + Path.GetFileNameWithoutExtension(filePath));
        LineConnect lineConnect = lineObject.AddComponent<LineConnect>();
        moving_figure moving_figure = lineObject.AddComponent<moving_figure>();
        lineConnect.filePath = filePath; // 読み込むCSVファイルのパスを設定
        lineConnect.Initialize(); // CSVからラインを描画するための初期化メソッドを呼び出す
    }

    void OnDestroy()
    {
        // 監視を停止する
        if (watcher != null)
        {
            watcher.Dispose();
        }
    }
}