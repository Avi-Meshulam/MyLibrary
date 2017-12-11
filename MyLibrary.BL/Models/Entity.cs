using MyLibrary.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyLibrary.BL
{
    public delegate void PropertyChangingEventHandler(object sender, PropertyChangingEventArgs e);

    [Serializable]
    public abstract class Entity<T> : IEntity<T> where T : class
    {
        [field: NonSerialized()]
        // Allows subscribers to cancel a property change
        internal event PropertyChangingEventHandler PropertyChanging;

        [field: NonSerialized()]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Returns Entity's ID value
        /// </summary>
        public abstract object EntityId();

        [NonSerialized]
        private Type _type;
        public Type Type { get { return _type = GetType(); } }

        /// <summary>
        /// Validates arguments according to their type
        /// </summary>
        /// <param name="args">KeyValue Pair array of arguments names and values</param>
        /// <returns></returns>
        protected virtual void Validate(params KeyValuePair<string, object>[] args)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var arg in args)
            {
                if(arg.Value.GetType() == typeof(string))
                {
                    if (string.IsNullOrEmpty(arg.Value.ToString()))
                    {
                        if (sb.Length > 0)
                            sb.AppendLine();
                        sb.Append($"{arg.Key} is reuired");
                    }
                }
            }

            if(sb.Length > 0)
                throw new ArgumentException(sb.ToString());
        }

        /// <summary>
        /// Compares field's old and new values, and if different - set new value.
        /// Raises a PropertyChanging event, allowing subscribers to cancel operation.
        /// Raises a PropertyChanged event, allowing subscribers to react.
        /// </summary>
        /// <typeparam name="F"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetField<F>(ref F field, F value, [CallerMemberName] string propertyName = null)
        {
            var e = new PropertyChangingEventArgs();
            OnPropertyChanging(e);
            if (e.Cancel)
                throw new OperationCanceledException(e.CancellationMessage);

            if (EqualityComparer<F>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void OnPropertyChanging(PropertyChangingEventArgs e)
        {
            PropertyChanging?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Updates entity in storage
        /// </summary>
        public void Update()
        {
            (this as T).Update(EntityId());
        }

        public abstract bool Equals(T other);
    }
}
