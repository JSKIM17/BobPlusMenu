using System;
using System.Collections;
using System.IO;
using UnityEngine;

public static class TextureManager
{
	public static string lastDate;
#if UNITY_EDITOR
	public static string DirectoryPath = Application.streamingAssetsPath;
#else
	public static string DirectoryPath = Application.persistentDataPath;
#endif

	public static string GetTimeText(DateTime time)
	{
		return time.ToString("yyMMdd");
	}
	public static string GetFormatFileName(string postNum)
	{
		return $"{postNum}_{DateTime.Now.ToString("yyMMdd")}";
	}

	public static string GetFormatFileName(string postNum, string timeText)
	{
		return $"{postNum}_{timeText}";
	}

	public static void SaveTextureAsJPG(Texture2D texture, string fileName)
	{
		byte[] bytes = texture.EncodeToJPG();
		string path = Path.Combine(DirectoryPath, fileName + ".jpg");
		File.WriteAllBytes(path, bytes);
		Debug.Log($"Texture saved to: {path}");
	}

	public static Texture2D LoadTextureFromFile(string fileName)
	{
		string path = Path.Combine(DirectoryPath, fileName + ".jpg");

		if (File.Exists(path)) {
			byte[] bytes = File.ReadAllBytes(path);
			Texture2D texture = new Texture2D(2, 2);
			texture.LoadImage(bytes);

			Debug.Log("Texture loaded from: " + path);
			return texture;
		}
		else {
			Debug.LogWarning("File not found: " + path);
			return null;
		}
	}

	// 저장한 이미지 파일을 삭제하는 함수
	public static void DeleteTextureFile(string fileName)
	{
		// 저장 경로 지정
		string path = Path.Combine(DirectoryPath, fileName + ".jpg");

		// 파일이 존재하는지 확인
		if (File.Exists(path)) {
			// 파일 삭제
			File.Delete(path);
			Debug.Log("File deleted: " + path);
		}
		else {
			//Debug.LogWarning("File not found, cannot delete: " + path);
		}
	}
}
