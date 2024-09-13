using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasAutoScaler : MonoBehaviour
{
	private bool _isTablet = false;
	private Vector2 _currentScreenSize;
	private float _aspect;
	private CanvasScaler _scaler;

	private void Awake()
	{
		if(_scaler == null) {
			_scaler = gameObject.GetComponent<CanvasScaler>();
			if(_scaler == null) {
				return;
			}
		}

		_currentScreenSize = new Vector2(Screen.width, Screen.height);
		_isTablet = IsTablet(Screen.width, Screen.height);
	}

	private bool IsTablet(float width, float height)
	{
		float aspectRatio = width / height;
		_aspect = aspectRatio;
		if (_aspect >= 0.5f) {
			_scaler.matchWidthOrHeight = 0.75f;
		}
		else {
			_scaler.matchWidthOrHeight = 0;
		}
		return aspectRatio <= 1.3f;
	}
}