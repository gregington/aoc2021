using System.Collections;
using System.Collections.Generic;

namespace aoc16;
public class PeekEnumerator<T> : IEnumerator<T>
{
    private IEnumerator<T> _enumerator;
    private T _peek;
    private bool _didPeek;

    public PeekEnumerator(IEnumerator<T> enumerator)
    {
        if (enumerator == null) {
            throw new ArgumentNullException("enumerator");
        }
        _enumerator = enumerator;
    }

    #region IEnumerator implementation
    public bool MoveNext()
    {
        return _didPeek ? !(_didPeek = false) : _enumerator.MoveNext();
    }

    public void Reset()
    {
        _enumerator.Reset();
        _didPeek = false;
    }

    object IEnumerator.Current { get { return this.Current; } }
    #endregion

    #region IDisposable implementation
    public void Dispose()
    {
        _enumerator.Dispose();
    }
    #endregion

    #region IEnumerator implementation
    public T Current
    {
        get { return _didPeek ? _peek : _enumerator.Current; }
    }
    #endregion

    private void TryFetchPeek() {
        if (!_didPeek && (_didPeek = _enumerator.MoveNext()))
        {
            _peek = _enumerator.Current;
        }
    }

    public T Peek
    {
        get { 
            TryFetchPeek();
            if (!_didPeek)
                throw new InvalidOperationException("Enumeration already finished.");

            return _peek;
        }
    }
}