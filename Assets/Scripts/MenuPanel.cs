using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{
	[SerializeField] private Button _closeButton;
	[SerializeField] private RawImage _myImage;

	private void Start()
	{
		_closeButton.onClick.AddListener(Close);
	}

	public void SetImage(Texture2D texture)
	{
		_myImage.texture = texture;
	}

	private void Close()
	{
		gameObject.SetActive(false);
	}
}
