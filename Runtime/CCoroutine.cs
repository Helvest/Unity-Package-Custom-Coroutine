using System;
using System.Collections;

namespace CustomCoroutine
{
	public class CCoroutine : IEnumerator
	{

		#region Fields

		private readonly Func<IEnumerator> _coroutineSaved;
		private IEnumerator _coroutine;
		private IEnumerator _childIEnumerator;

		#endregion

		#region Constructor

		public CCoroutine(Func<IEnumerator> coroutine)
		{
			_coroutineSaved = coroutine;
			_coroutine = _coroutineSaved();
		}

		#endregion

		#region Current

		public object Current => _coroutine?.Current;

		#endregion

		#region MoveNext

		public bool MoveNext()
		{
			if (_coroutine == null)
			{
				return false;
			}

			do
			{
				if (_childIEnumerator != null)
				{
					if (_childIEnumerator.MoveNext())
					{
						return true;
					}
					else
					{
						_childIEnumerator = null;
					}
				}

				var coroutine = _coroutine;

				if (coroutine.MoveNext())
				{
					if (coroutine.Current is IEnumerator enumerator)
					{
						_childIEnumerator = enumerator;
					}
				}
				else
				{
					if (_coroutine == coroutine)
					{
						_coroutine = null;
					}

					return false;
				}

			} while (_childIEnumerator != null);

			return true;
		}

		#endregion

		#region Reset

		public void Reset()
		{
			if (_coroutineSaved != null)
			{
				_coroutine = _coroutineSaved();
			}
		}

		#endregion

	}

	#region Alternative Class

	public class CCoroutine<T1> : CCoroutine
	{
		public CCoroutine(Func<T1, IEnumerator> coroutine, T1 t1) : base(() => coroutine(t1)) { }
	}

	public class CCoroutine<T1, T2> : CCoroutine
	{
		public CCoroutine(Func<T1, T2, IEnumerator> coroutine, T1 t1, T2 t2) : base(() => coroutine(t1, t2)) { }
	}

	public class CCoroutine<T1, T2, T3> : CCoroutine
	{
		public CCoroutine(Func<T1, T2, T3, IEnumerator> coroutine, T1 t1, T2 t2, T3 t3) : base(() => coroutine(t1, t2, t3)) { }
	}

	public class CCoroutine<T1, T2, T3, T4> : CCoroutine
	{
		public CCoroutine(Func<T1, T2, T3, T4, IEnumerator> coroutine, T1 t1, T2 t2, T3 t3, T4 t4) : base(() => coroutine(t1, t2, t3, t4)) { }
	}

	#endregion

}
