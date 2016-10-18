using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Isjhar.Library
{
	public class HorizontalItemGeneratorExample : MonoBehaviour 
	{
		[SerializeField] private GameObject _itemPrefab = null;
		[SerializeField] private RectTransform _listHolder = null;
		[SerializeField] private List<string> _titleList = new List<string>();
		[SerializeField] private ListViewHorizontalGenerator _viewGenerator = null;

		// Use this for initialization
		void Start () 
		{
			GenerateItem();
		}
		
		public void GenerateItem()
		{
			for(int i = 0; i < _titleList.Count; i++)
			{
				GameObject item = Instantiate(_itemPrefab) as GameObject;
				ItemView view = item.GetComponent<ItemView>();
				view.Title.text = _titleList[i];
				_viewGenerator.GenerateList(i, item, i == _titleList.Count - 1);
			}
		}
	}
}