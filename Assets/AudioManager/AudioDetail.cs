using UnityEngine;
using System.Collections;

namespace Isjhar.Library
{
	/// <summary>
	/// Class ini untuk menyimpan model data audio clip.
	/// </summary>
	[System.Serializable]
	public class AudioDetail
	{
		[SerializeField] private AudioClip _clip; // ini untuk nyimpan clip nya
		[SerializeField] [Range(0f,1f)] private float _volume = 1f; // menyimpan volumen clip
		[SerializeField] private bool _loop = false;

		public AudioClip Clip { get { return _clip; } set { _clip = value; } } // encapsulation field _clip
		public float Volume { get { return _volume; } set { _volume = value; } } // encapsulation volume
		public bool Loop { get { return _loop; } set { _loop = value; } }

		public AudioDetail()
		{
			
		}

		public AudioDetail(AudioDetail other)
		{
			_clip = other.Clip;
			_volume = other.Volume;
			_loop = other.Loop;
		}
	}
}