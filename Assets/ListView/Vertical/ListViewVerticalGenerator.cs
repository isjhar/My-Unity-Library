using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Isjhar.Library
{
	public class ListViewVerticalGenerator : MonoBehaviour 
	{
		[SerializeField] private RectTransform _mask = null;
		[SerializeField] private Transform _parentList = null;
		[SerializeField] private Scrollbar _bar = null;
		[SerializeField] private VerticalLayoutGroup _layout = null;

		private float _additionalHeight;	
		private float _totalHeight;
		private float _addHeight;
		private float _percentageFromShowedPanelOfTotalHeight;

		// generate item list
		public void GenerateList(int index, GameObject item, bool isEndData)
		{
			// set parent
			item.transform.SetParent(_parentList, false);
			
			if(index == 0)
			{
				_additionalHeight = _layout.padding.top + _layout.padding.bottom;
				RectTransform rect = item.GetComponent<RectTransform>();
				_addHeight = rect.rect.height;
				_totalHeight += _addHeight;
			}
			else
			{
				_totalHeight += _layout.spacing + _addHeight;
			}

			if(isEndData)
			{
				CalculateHeight();
				CalculatePercentageMaskHeightBasedTotalHeight();
			}
		}

		// calculate list height
		public void CalculateHeight()
		{
			RectTransform rectParent = _parentList.gameObject.GetComponent<RectTransform>();
			_totalHeight += _additionalHeight;
			if(rectParent.rect.height < _totalHeight)
			{
				rectParent.sizeDelta = new Vector2(rectParent.rect.width, _totalHeight);
				// reset position
				rectParent.anchoredPosition = new Vector2(rectParent.anchoredPosition.x, 0f);
			}
		}

		// clear list
		public void ClearList()
		{
			_totalHeight = 0f;
			RectTransform rectParent = _parentList.gameObject.GetComponent<RectTransform>();
			rectParent.sizeDelta = new Vector2(rectParent.rect.width, _totalHeight);
		}

		// get total item
		public int GetAmountOfItemShowedInPanel()
		{
			if(_mask != null)
			{
				float height = _mask.rect.height;
				int result = (int) (height / _addHeight);
				return result;
			}
			else
			{
				return 0;
			}
		}

		// get index number item showed in panel
		public int GetCurrentIndexOfItemShowed()
		{
			if(_percentageFromShowedPanelOfTotalHeight < 1f)
			{
				float barValue = (1f - _bar.value);
				int result = (int) ( barValue / _percentageFromShowedPanelOfTotalHeight);
				return result;
			}
			else
			{
				return 0;
			}
		}


		// calculate percentage scroll value based on height
		private void CalculatePercentageMaskHeightBasedTotalHeight()
		{
			int amountOfItemShowed = GetAmountOfItemShowedInPanel();
			_percentageFromShowedPanelOfTotalHeight = (_addHeight * amountOfItemShowed) / _totalHeight;

			if(_percentageFromShowedPanelOfTotalHeight > 1f)
				_percentageFromShowedPanelOfTotalHeight = 1f;
		}
	}
}