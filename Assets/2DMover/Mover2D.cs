using UnityEngine;
using System.Collections;


namespace Isjhar.Library
{
	/// <summary>
	/// Class yang handle fitur penggerak
	/// </summary>
	[RequireComponent(typeof(Rigidbody2D))]
	public class Mover2D : MonoBehaviour 
	{
		[SerializeField] private Vector3 _velocity; // kecepatan 
		[SerializeField] private bool _playOnAwake = false;
		private Rigidbody2D _myRigidbody; // rigidbody reference

		private void Awake()
		{
			_myRigidbody = GetComponent<Rigidbody2D>();
		}

		// Use this for initialization
		void Start () 
		{
			if(_playOnAwake)
				_myRigidbody.velocity = _velocity;
		}

		/// <summary>
		/// Method untuk set kecepatan
		/// </summary>
		/// <param name="velocity">Velocity.</param>
		public void SetVelocity(Vector3 velocity)
		{
			_velocity = velocity;
			if(_myRigidbody != null)
				_myRigidbody.velocity = _velocity;
		}
	}
}
