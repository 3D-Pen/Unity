using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class CSVWatcher : MonoBehaviour
{
    private FileSystemWatcher watcher;
    public string folderToWatch;  // 監視するCSVファイルのフォルダ
    private Dictionary<string, DateTime> processedFiles = new Dictionary<string, DateTime>(); // 処理済みファイルの管理
    private float stabilityWaitTime = 1.0f; // ファイルが安定するまでの待機時間（秒）

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
        Debug.Log($"CSVファイルが追加されました: {e.FullPath}");
        // メインスレッドで処理を実行
        UnityMainThreadDispatcher.Instance().Enqueue(() => ProcessFileWithDelay(e.FullPath));
    }

    // 指定したファイルを一定時間待機して処理
    private void ProcessFileWithDelay(string filePath)
    {
        StartCoroutine(WaitForFileToStabilize(filePath, stabilityWaitTime));
    }

    // ファイルが安定するまで一定時間待機
    private System.Collections.IEnumerator WaitForFileToStabilize(string filePath, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        // ファイルが安定するまで待機してから移動
        while (!IsFileReady(filePath))
        {
            yield return new WaitForSeconds(0.5f); // 0.5秒待機して再確認
        }

        // すでに処理されたファイルでないかを確認
        if (processedFiles.ContainsKey(filePath))
        {
            // 短時間に同じファイルを処理しないようにフィルタリング
            DateTime lastProcessedTime = processedFiles[filePath];
            if ((DateTime.Now - lastProcessedTime).TotalSeconds < 5.0f)
            {
                Debug.Log("重複するファイル処理をスキップ: " + filePath);
                yield break; // 処理をスキップ
            }
        }

        // ファイルの最終更新時間を記録
        processedFiles[filePath] = DateTime.Now;

        // CSVファイルを処理してオブジェクトを作成
        LoadCSVAndCreateObject(filePath);
    }

    // ファイルが使用中でないか確認
    private bool IsFileReady(string filePath)
    {
        try
        {
            using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                return stream.Length > 0;
            }
        }
        catch (IOException)
        {
            return false; // ファイルが使用中またはロックされている
        }
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