using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelaunayTriangulationVisualisation
{
    public class ValueWrapper<T>
    {
        public T Value { get; set; }

        public ValueWrapper() { }
        public ValueWrapper(T value) => Value = value;

        // Implicit conversion from T to ValueWrapper<T>
        public static implicit operator ValueWrapper<T>(T value)
            => new(value);

        // Implicit conversion from ValueWrapper<T> to T
        public static implicit operator T(ValueWrapper<T> wrapper)
            => wrapper.Value;

        public static List<T> UnwrapWrappedList(IList<ValueWrapper<T>> list)
        {
            List<T> unwrapped = [];

            foreach (var item in list) unwrapped.Add(item.Value);

            return unwrapped;
        }
    }
}
