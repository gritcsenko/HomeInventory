using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeInventory.Core;

public interface ISpannableCollection<T> : IReadOnlyCollection<T>
{
    Span<T> AsSpan();
}
