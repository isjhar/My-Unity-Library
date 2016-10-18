using UnityEngine;
using System.Collections;

namespace Isjhar.Library.Sample
{
	public class ShuffleSample : MonoBehaviour 
	{
		[SerializeField] int[] _intList;

		private void Start()
		{
			Debug.Log("Before Shuffle : " + ToString(_intList));	
			Debug.Log("Normal Shuffle : " + ToString(ShuffleItemManager.Shuffle<int>(_intList)));
			Debug.Log("Non Repeatable Shuffle" + ToString(ShuffleItemManager.NonRepeatableItemPositionShuffle<int>(_intList)));
		}

		private string ToString(int[] intList)
		{
			string output = intList[0].ToString();
			for(int i = 1; i < intList.Length; i++)
			{
				output += ", " + intList[i];
			}
			return output;
		}
	}
}