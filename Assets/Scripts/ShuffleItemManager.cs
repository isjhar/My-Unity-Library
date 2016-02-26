using UnityEngine;
using System.Collections;

public static class ShuffleItemManager
{
	/// <summary>
	/// Shuffle the specified items.
	/// </summary>
	/// <param name="items">item that want to be shuffled</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	/// <returns>result of shuffle</returns>
	public static T[] Shuffle<T>(T[] items)
	{
		int numberShuffle = 1000;
		T temp;
		int swapIndex;
		for(int i = 0; i < numberShuffle; i++)
		{
			swapIndex = Random.Range(0, items.Length);
			temp = items[0];
			items[0] = items[swapIndex];
			items[swapIndex] = temp;
		}
		return items;
	}

	/// <summary>
	/// Nons the repeatable item position shuffle. item would not have same position as before shuffle
	/// </summary>
	/// <returns>items have been shuffled</returns>
	/// <param name="items">item that want to bo shuffled</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static T[] NonRepeatableItemPositionShuffle<T>(T[] items)
	{
		T temp;

		int[] swapIndexList = new int[items.Length];
		for(int i = 0; i < swapIndexList.Length; i++)
		{
			swapIndexList[i] = i;
		}

		swapIndexList = Shuffle<int>(swapIndexList);

		int swapIndex;
		int swapNextIndex;
		for(int i = 0; i < swapIndexList.Length - 1; i++)
		{
			swapIndex = swapIndexList[i];
			swapNextIndex = swapIndexList[i + 1];
			temp = items[swapIndex];
			items[swapIndex] = items[swapNextIndex];
			items[swapNextIndex] = temp;
		}
		return items;
	}
}
