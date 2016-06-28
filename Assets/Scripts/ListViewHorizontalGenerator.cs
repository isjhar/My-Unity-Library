using UnityEngine;
using UnityEngine.UI;
using System.Collections;



public class ListViewHorizontalGenerator : MonoBehaviour 
{
	[SerializeField] private RectTransform _mask = null;
	[SerializeField] private Transform _parentList = null;
	[SerializeField] private Scrollbar _bar = null;
	[SerializeField] private HorizontalLayoutGroup _layout = null;
	
	private float _additionalWidth;
	private float _totalWidth;
	private float _addWidth;
	private float _percentageFromShowedPanelOfTotalWidth;

	// generate item list
	public void GenerateList(int index, GameObject item, bool isEndData)
	{
		// set parent
		item.transform.SetParent(_parentList, false);
		if(index == 0)
		{
			_additionalWidth = _layout.padding.left + _layout.padding.right;
			RectTransform rect = item.GetComponent<RectTransform>();
			_addWidth = rect.rect.width;
			_totalWidth += _addWidth;
		}
		else
		{
			_totalWidth += _layout.spacing + _addWidth;
		}


		if(isEndData)
		{
			CalculateWidth();
		}
	}

	public void CalculateWidth()
	{
		RectTransform rectParent = _parentList.gameObject.GetComponent<RectTransform>();
		_totalWidth += _additionalWidth;
		if(rectParent.rect.height < _totalWidth)
		{
			rectParent.sizeDelta = new Vector2(_totalWidth, rectParent.rect.height);
			// reset postion
			rectParent.anchoredPosition = new Vector2(0f, rectParent.anchoredPosition.y);
		}
	}
	
	public void ClearList()
	{
		_totalWidth = 0f;
		RectTransform rectParent = _parentList.gameObject.GetComponent<RectTransform>();
		rectParent.sizeDelta = new Vector2(rectParent.rect.width, _totalWidth);
	}

	// get total item
	public int GetAmountOfItemShowedInPanel()
	{
		if(_mask != null)
		{
			float width = _mask.rect.width;
			int result = (int) (width / _addWidth);
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
		if(_percentageFromShowedPanelOfTotalWidth < 1f)
		{
			float barValue = (1f - _bar.value);
			int result = (int) ( barValue / _percentageFromShowedPanelOfTotalWidth);
			return result;
		}
		else
		{
			return 0;
		}
	}

	// calculate percentage scroll value based on height
	private void CalculatePercentageMaskWidthBasedTotalWidth()
	{
		// get total item yang dapat ditampilkan pada mask
		int amountOfItemShowed = GetAmountOfItemShowedInPanel();
		_percentageFromShowedPanelOfTotalWidth = (_addWidth * amountOfItemShowed) / _totalWidth;

		if(_percentageFromShowedPanelOfTotalWidth > 1f)
			_percentageFromShowedPanelOfTotalWidth = 1f;
	}
}