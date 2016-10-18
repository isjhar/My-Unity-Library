using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Isjhar.Library
{
	public class ScreenExchange : MonoBehaviour
	{
		[SerializeField] private List<GameObject> _screenList;

		private Dictionary<string, GameObject> _screenDictionary = new Dictionary<string, GameObject>();
		private GameObject _fromScreen;

		private void Awake()
		{
			foreach(GameObject screen in _screenList)
			{
				_screenDictionary.Add(screen.name, screen);
			}
		}

		public void ChangeScreenFrom(string screenName)
		{
			_fromScreen = _screenDictionary[screenName];
		}

		public void ChangeScreenTo(string screenName)
		{
			if(_fromScreen != null && _screenDictionary.ContainsKey(screenName))
			{
				_fromScreen.SetActive(false);
				_screenDictionary[screenName].SetActive(true);
			}
		}
	}
}