using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.BL
{
    public interface IEntityManager<T> where T : IEntity<T>
    {
        T this[object ID] { get; }
        int Count { get; }
        T Add(T item);
        bool Delete(T entity);
        List<T> Search(Predicate<T> predicate = null);
        void Clear();
    }
}
