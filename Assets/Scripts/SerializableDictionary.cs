using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<SerializableKeyValuePair> _pairList = new List<SerializableKeyValuePair>();

	// save the dictionary to lists
	public void OnBeforeSerialize()
	{
		_pairList.Clear();

		foreach (KeyValuePair<TKey, TValue> pair in this) {
			_pairList.Add(new SerializableKeyValuePair(pair));
		}
	}

	// load dictionary from lists
	public void OnAfterDeserialize()
	{
		this.Clear();

		for (int i = 0; i < _pairList.Count; i++) {
			this.Add(_pairList[i].key, _pairList[i].value);
		}
	}

	[Serializable]
	public class SerializableKeyValuePair
	{
		public TKey key;
		public TValue value;

		public SerializableKeyValuePair(TKey key, TValue value)
		{
			this.key = key;
			this.value = value;
		}

		public SerializableKeyValuePair(KeyValuePair<TKey, TValue> pair)
		{
			key = pair.Key;
			value = pair.Value;
		}
	}
}