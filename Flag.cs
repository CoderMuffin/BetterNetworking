using System;
namespace BetterAsync {
	/// <summary>
	/// Represents an advanced boolean with events.
	/// </summary>
	public class Flag
	{
		public static implicit operator bool(Flag f)
		{
			return f._up;
		}
		bool _up = false;
		/// <summary>
		/// Whether to silently (no events) lower the <see cref="Flag"/> after being raised or leave it raised.
		/// </summary>
		public bool autoLower;
		/// <summary>
		/// Whether the <see cref="Flag"/> is raised or lowered. Note that writing any state to a <see cref="Flag"/> will fire events, even if it is in the same state.
		/// </summary>
		public bool up
		{
			get => _up;
			set
			{
				_up = value;
				if (autoLower)
                {
					_up = false;
                }
				OnChanged?.Invoke(value);
				if (value)
				{
					OnRaised?.Invoke();
				} else
				{
					OnLowered?.Invoke();
				}
			}
		}
		/// <summary>
		/// Pauses the current thread until the <see cref="Flag"/> is raised.
		/// </summary>
		public void WaitRaised()
		{
			Flow.WaitUntil(()=>up);
		}

		/// <summary>
		/// Pauses the current thread until the <see cref="Flag"/> is changed.
		/// </summary>
		public void WaitChanged()
		{
			bool cs = up;
			Flow.WaitUntil(() => cs!=up);
		}
		/// <summary>
		/// Pauses the current thread until the <see cref="Flag"/> is lowered.
		/// </summary>
		public void WaitLowered()
		{
			Flow.WaitUntil(() => !up);
		}

		public Flag()
		{

		}
		public Flag(bool up)
		{
			_up = up;
		}
		public void Raise()
		{
			up = true;
		}
		public void Lower()
		{
			up = false;
		}
		public bool Toggle()
		{
			up = !up;
			return up;
		}
		public event Action OnRaised;
		public event Action OnLowered;
		public event Action<bool> OnChanged;
	}
}