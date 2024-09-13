using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using HtmlAgilityPack;

public class NaverPostLoader : MonoBehaviour
{
	public GameObject loadingIcon;
	public Button myButton;
	public string postNum;
	string _head = "https://blog.naver.com/PostView.naver?blogId=babplus123";
	string _tail = "redirect=Dlog&widgetTypeCall=true&noTrackingCode=true&directAccess=false";

	public string targetURL = "";
	public List<NaverURLInfo> infoList = new List<NaverURLInfo>();
	public static string TargetHost = "https://postfiles.pstatic.net";


	private Texture2D _menuTexture;
	public Texture2D MenuTexture {
		get {
			return _menuTexture;
		}
	}

	public bool Ready => _menuTexture != null;

	private void Start()
	{
		Initialize();
	}


	public void Initialize()
	{
		Texture2D texture = TextureManager.LoadTextureFromFile(TextureManager.GetFormatFileName(postNum));
		if (texture != null) {
			_menuTexture = texture;
			loadingIcon.gameObject.SetActive(false);
			myButton.interactable = true;
		}
		else {
			Initialize($"{_head}&logNo={postNum}&{_tail}");
		}
	}


	private void Initialize(string url)
	{
		targetURL = url;
		loadingIcon.gameObject.SetActive(true);
		myButton.interactable = false;
		StartCoroutine(GetBlogImageLink(targetURL, DownloadImages));
	}

	private void DownloadImages()
	{
		//for(int i = 0; i < infoList.Count; ++i) {
			StartCoroutine(DownloadImage(infoList[0].BigQueryImage,
				(t) => {
					if (t != null) {
						_menuTexture = t;
						TextureManager.SaveTextureAsJPG(t, TextureManager.GetFormatFileName(postNum));
					}
					else {
						//TODO: 다른 이미지로 변경
						_menuTexture = null;
					}

					if (Ready) {
						loadingIcon.gameObject.SetActive(false);
						myButton.interactable = true;
					}
				}));
		//}
	}

	private IEnumerator GetBlogImageLink(string url, Action callback = null)
	{
		Debug.Log($"Sending Request...: {url}");
		UnityWebRequest request = UnityWebRequest.Get(url);
		yield return request.SendWebRequest();

		if (request.result != UnityWebRequest.Result.Success) {
			Debug.LogError("Error fetching blog post: " + request.result);
			yield break;
		}

		string htmlContent = request.downloadHandler.text;
		HtmlDocument document = new HtmlDocument();
		document.LoadHtml(htmlContent);

		List<string> imageUrls = new List<string>();

		foreach (var imgNode in document.DocumentNode.SelectNodes("//img")) {
			string imageUrl = imgNode.GetAttributeValue("src", "");
			if (!string.IsNullOrEmpty(imageUrl)) {
				imageUrls.Add(imageUrl);
			}
		}

		infoList = imageUrls.Select(s => new NaverURLInfo(s)).ToList();
		infoList.RemoveAll(i => i.HOST != TargetHost);
		callback?.Invoke();
	}

	private IEnumerator DownloadImage(string imageUrl, Action<Texture2D> resultCallback)
	{
		UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(imageUrl);
		yield return imageRequest.SendWebRequest();

		if (imageRequest.result == UnityWebRequest.Result.Success) {
			Texture2D texture = DownloadHandlerTexture.GetContent(imageRequest);
			texture.name = imageUrl;

			resultCallback?.Invoke(texture);
			Debug.Log($"Image successfully loaded!: {imageUrl}");
		}
		else {
			resultCallback?.Invoke(null);
			Debug.LogError("Error downloading image: " + imageRequest.error);
		}
	}
}

[Serializable]
public class NaverURLInfo
{
	private Uri _uri;
	[SerializeField] private string _defaultURL;
	public string HOST => _uri.Scheme + "://" + _uri.Host;
	public string PATH => _uri.AbsolutePath.Substring(1);
	public string QUERY => _uri.Query.Substring(1);
	private string _bigQuery = "type=w773";

	public string BigQueryImage => $"{HOST}/{PATH}?{_bigQuery}";

	public NaverURLInfo(string url)
	{
		_defaultURL = url;
		_uri = new Uri(url);
	}

	public override string ToString()
	{
		return $"HOST: {HOST}\nPATH: {PATH}\nQUERY: {QUERY}";
	}
}
