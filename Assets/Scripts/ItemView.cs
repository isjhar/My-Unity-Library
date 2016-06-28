using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemView : MonoBehaviour 
{
	[SerializeField] private Text _title;

	public Text Title { get { return _title; } set { _title = value; } }
}
