using System;
using System.ComponentModel;

namespace MyLibrary.BL
{
    public interface IEntity<T> : IEquatable<T>, INotifyPropertyChanged
    {
        object EntityId();
    }
}