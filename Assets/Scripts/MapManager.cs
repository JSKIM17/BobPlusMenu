using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class MapManager : MonoBehaviour
{
	public MenuPanel menuPanel;
	public List<NaverPostLoader> infoList = new List<NaverPostLoader>();
	/*
		1호점: 221697747131
		2호점: 222378669083
		3호점: 221556102879
		4호점: 223336115425
		5호점: 222371212405
		6호점: 223112282256
		7호점: 222374958935
		8호점: 폐점
	*/
	private void Start()
	{
		Application.targetFrameRate = 120;
		FileAutoRemover();
		SetCloseStream1();
		SetCloseStream2();
	}

	private void SetCloseStream1()
	{
		var clickStream = this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.Escape));
		clickStream
			.Buffer(clickStream.Throttle(TimeSpan.FromMilliseconds(200)))
			.Where(x => x.Count == 1)
			.Subscribe(_ => {
				if (menuPanel.gameObject.activeSelf) {
					menuPanel.gameObject.SetActive(false);
				}
			});
	}

	private void SetCloseStream2()
	{
		var clickStream = this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.Escape));
		clickStream
			.Buffer(clickStream.Throttle(TimeSpan.FromMilliseconds(200)))
			.Where(x => x.Count >= 2)
			.Subscribe(_ => {
				if (menuPanel.gameObject.activeSelf) {
					menuPanel.gameObject.SetActive(false);
				}
				else {
					Application.Quit();
				}
			});
	}

	public void OnTouchLoadHeadImage(int index)
	{
		if (infoList[index].Ready) {
			menuPanel.gameObject.SetActive(true);
			menuPanel.SetImage(infoList[index].MenuTexture);
		}
		else {
			Debug.Log("??");
			//TODO: 뭐하지
		}
	}

	public void OnTouchResetImage()
	{
		var today = DateTime.Now;

		foreach (var info in infoList) {
			TextureManager.DeleteTextureFile(
				TextureManager.GetFormatFileName
				(info.postNum, TextureManager.GetTimeText(today)));
		}

		foreach (var info in infoList) {
			if (info.gameObject.activeSelf) {
				info.Initialize();
			}
		}
	}

	//당일로부터 일주일 내의 파일 제거
	private void FileAutoRemover()
	{
		var today = DateTime.Now;
		List<string> before7Days = new List<string>();
		for(int i = 0; i < 7; ++i) {
			today = today.AddDays(-1);
			before7Days.Add(TextureManager.GetTimeText(today));
		}

		foreach(var info in infoList) {
			for (int i = 0; i < before7Days.Count; ++i) {
				TextureManager.DeleteTextureFile(TextureManager.GetFormatFileName(info.postNum, before7Days[i]));
			}
		}
	}

	[ContextMenu("T")]
	public void Context()
	{
		NaverURLInfo baseInfo = new NaverURLInfo("https://blog.naver.com/PostView.naver?blogId=babplus123&logNo=222378669083&redirect=Dlog&widgetTypeCall=true&noTrackingCode=true&directAccess=false");
		Debug.Log(baseInfo);
	}
}


/*
	//string blogPostUrl = "https://blog.naver.com/PostView.naver?blogId=babplus123&logNo=222378669083&redirect=Dlog&widgetTypeCall=true&noTrackingCode=true&directAccess=false";
	//string TASD = "https://postfiles.pstatic.net/MjAyNDA5MTFfMjc0/MDAxNzI2MDM3MzQzMTI4.JCLTf1RQdObVZpsFyFln6nS7oDsaVmszwiEKl0cJBSwg.7i9IdFHItWIIgP0REeFjd-8xVjQalW9D4QG2eedGJVgg.PNG/%EA%B2%8C%EC%8B%9C%EB%A9%94%EB%89%B4.png?type=w773";
	//string TASD2 = "https://postfiles.pstatic.net/MjAyNDA5MTFfMjc0/MDAxNzI2MDM3MzQzMTI4.JCLTf1RQdObVZpsFyFln6nS7oDsaVmszwiEKl0cJBSwg.7i9IdFHItWIIgP0REeFjd-8xVjQalW9D4QG2eedGJVgg.PNG/%EA%B2%8C%EC%8B%9C%EB%A9%94%EB%89%B4.png?type=w80_blur";
 
	IEnumerator DownloadBlogImages(string url)
	{
		// 1. 네이버 블로그 HTML 가져오기
		UnityWebRequest request = UnityWebRequest.Get(url);
		yield return request.SendWebRequest();

		if (request.result != UnityWebRequest.Result.Success) {
			Debug.LogError("Error fetching blog post: " + request.error);
			yield break;
		}

		string htmlContent = request.downloadHandler.text;
		Debug.Log(htmlContent);
		// 2. HTML 파싱하여 이미지 URL 추출
		HtmlDocument document = new HtmlDocument();
		document.LoadHtml(htmlContent);

		Debug.Log(document.ToString());
		List<string> imageUrls = new List<string>();


		foreach (var imgNode in document.DocumentNode.SelectNodes("//img")) {
			string imageUrl = imgNode.GetAttributeValue("src", "");
			if (!string.IsNullOrEmpty(imageUrl)) {
				imageUrls.Add(imageUrl);
				Debug.Log("Found image URL: " + imageUrl);
			}
		}

		//string path = Application.dataPath + $"/urlList.txt";
		//
		//using (StreamWriter sw = new StreamWriter(path)) {
		//	sw.WriteLine(string.Join("\n", imageUrls));
		//}

		//foreach (string u in imageUrls) {
		//	yield return StartCoroutine(DownloadAndApplyImage(u));
		//}
		//index = 0;
		//targetRenderer.texture = textureList[index];
		// 3. 첫 번째 이미지 다운로드 및 적용 (여러 이미지 중 첫 번째 이미지)
		//if (imageUrls.Count > 0) {
		//	yield return StartCoroutine(DownloadAndApplyImage(imageUrls[0]));
		//}
	}

	public IEnumerator DownloadAndApplyImage(string imageUrl, Action<Texture2D> resultCallback)	
	{
		//if (imageUrl.Contains("?type=")) {
		//	int index = imageUrl.IndexOf(typeKey)
		//	imageUrl = imageUrl.Replace();
		//}
		
		UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(imageUrl);
		yield return imageRequest.SendWebRequest();

		if (imageRequest.result == UnityWebRequest.Result.Success) {
			Texture2D texture = DownloadHandlerTexture.GetContent(imageRequest);
			
			texture.name = imageUrl;
			resultCallback?.Invoke(texture);
			Debug.Log($"Image successfully applied!: {imageUrl}");
			//textureList.Add(texture);
			//targetRenderer.texture = texture;
		}
		else {
			resultCallback?.Invoke(null);
			Debug.LogError("Error downloading image: " + imageRequest.error);
		}
	}

	 
	 
	 */
