using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyLibrary.DAL;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows.Threading;
using System.Diagnostics;

namespace MyLibrary.BL
{
    public class EntityManager<T> : IEntityManager<T> where T : Entity<T>
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event EventHandler<Type> RelatedEntitiesAffected;

        public readonly User CurrentUser;
        protected ObservableCollection<T> _entities = new ObservableCollection<T>();

        // Measures time between loading operations from DAL
        private Stopwatch _loadEntitiesStopwatch = new Stopwatch();

        public EntityManager(User currentUser = null)
        {
            CurrentUser = currentUser;
            LoadEntities();
        }

        public int Count { get { return _entities.Count; } }

        public T this[object ID] { get { return _entities.FirstOrDefault(e => e.EntityId().Equals(ID)); } }

        public bool IsReadOnly { get { return !IsOperationAuthorized(OperationType.Edit); } }

        public virtual T Add(T entity)
        {
            if (!IsOperationAuthorized(OperationType.Add))
                throw new UnauthorizedAccessException("Unauthorized operation");

            if (_entities.Contains(entity))
                throw new ArgumentException(
                    $"You cannot have 2 {typeof(T).Name.PluralForm()} with the same identity");

            // Persist to storage
            entity.Insert(entity.EntityId());

            _entities.Add(entity);

            //SubscribeToEntityEvents(entity);

            return entity;
        }

        public bool Delete(T entity)
        {
            if (!IsOperationAuthorized(OperationType.Delete))
                throw new UnauthorizedAccessException("Unauthorized operation");

            DeleteRelatedEntities(entity);

            // Delete from storage
            entity.Delete(entity.EntityId());

            if (_entities.Remove(entity))
            {
                //UnsubscribeFromEntityEvents(entity);
                return true;
            }

            return false;
        }

        private void DeleteRelatedEntities(T entity)
        {
            if (typeof(T) == typeof(Publisher))
            {
                var publisherID = (entity as Publisher).PublisherID;
                var itemsManager = new EntityManager<LibraryItem>(CurrentUser);
                var relatedItems = itemsManager.Search(i => i.PublisherID == publisherID);
                if (relatedItems.Count > 0)
                {
                    // Delete all related items
                    relatedItems.ForEach(i => itemsManager.Delete(i));
                    RelatedEntitiesAffected?.Invoke(this, typeof(LibraryItem));
                }
            }

            if (typeof(T) == typeof(Employee))
            {
                // delete all related user accounts
                var employeeID = (entity as Employee).EmployeeID;
                var usersManager = new EntityManager<User>(CurrentUser);
                var relatedUsers = usersManager.Search(u => u.EmployeeID == employeeID);
                if (relatedUsers.Count > 0)
                {
                    // Delete all related user accounts
                    relatedUsers.ForEach(u => usersManager.Delete(u));
                    RelatedEntitiesAffected?.Invoke(this, typeof(User));
                }
            }
        }

        public void Clear()
        {
            if (!IsOperationAuthorized(OperationType.Delete))
                throw new UnauthorizedAccessException("Unauthorized operation");

            _entities.ToList().ForEach(e =>
            {
                // Delete from storage
                e.Delete(e.EntityId());
                //UnsubscribeFromEntityEvents(e);
            });

            _entities.Clear();
        }

        public List<T> Search<D>(Predicate<D> predicate = null) where D : T
        {
            //Reload data from DAL every time a search is executed
            LoadEntities();

            IEnumerable<T> query = (typeof(D) == typeof(T)) ?
                _entities : _entities.Where(e => e.GetType() == typeof(D));

            query = predicate == null ?
                query : query.Where(e => predicate.Invoke((D)e));

            return query.ToList();
        }

        public List<T> Search(Predicate<T> predicate = null)
        {
            return Search<T>(predicate);
        }

        // Loads entities from DAL
        private void LoadEntities()
        {
            // If data was loaded up to 5 seconds ago - don't load again
            if (_loadEntitiesStopwatch.Elapsed.TotalSeconds > 0 &&
                _loadEntitiesStopwatch.Elapsed.TotalSeconds < 5)
                return;

            if (_entities != null)
                UnsubscribeFromEntitiesEvents();

            _entities = DALManager.Load<T>();

            SubscribeToEntitiesEvents();

            _loadEntitiesStopwatch.Restart();
        }

        private void SubscribeToEntitiesEvents()
        {
            _entities.CollectionChanged += Entities_CollectionChanged;
            //SubscribeToEntityEvents();    // Unmark in order to save entity on each property change
        }

        private void SubscribeToEntityEvents(T entity = null)
        {
            if (entity == null)
            {
                _entities.ToList().ForEach(e =>
                {
                    e.PropertyChanging += Entity_PropertyChanging;
                    e.PropertyChanged += Entity_PropertyChanged;
                });
            }
            else
            {
                entity.PropertyChanging += Entity_PropertyChanging;
                entity.PropertyChanged += Entity_PropertyChanged;
            }
        }

        private void UnsubscribeFromEntitiesEvents()
        {
            _entities.CollectionChanged -= Entities_CollectionChanged;
            //UnsubscribeFromEntityEvents();    // Unmark in order to save entity on each property change
        }

        private void UnsubscribeFromEntityEvents(T entity = null)
        {
            if (entity == null)
            {
                _entities.ToList().ForEach(e => e.PropertyChanging -= Entity_PropertyChanging);
                _entities.ToList().ForEach(e => e.PropertyChanged -= Entity_PropertyChanged);
            }
            else
            {
                entity.PropertyChanging -= Entity_PropertyChanging;
                entity.PropertyChanged -= Entity_PropertyChanged;
            }
        }

        private void Entities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                        e.Action, e.NewItems, e.NewStartingIndex));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                        e.Action, e.OldItems, e.OldStartingIndex));
                    break;
                case NotifyCollectionChangedAction.Replace:
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                        e.Action, e.NewItems, e.OldItems, e.NewStartingIndex));
                    break;
                case NotifyCollectionChangedAction.Move:
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                        e.Action, e.OldItems, e.NewStartingIndex, e.OldStartingIndex));
                    break;
                case NotifyCollectionChangedAction.Reset:
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(e.Action));
                    break;
                default:
                    break;
            }
        }

        private void Entity_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if (!IsOperationAuthorized(OperationType.Edit))
            {
                e.Cancel = true;
                e.CancellationMessage = "Unauthorized operation";
            }
        }

        private void Entity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            (sender as T).Update();
        }

        public bool IsOperationAuthorized(OperationType operationType)
        {
            if ((Permissions.GetUserPermissions<T>(CurrentUser) & operationType) == 0)
                return false;
            return true;
        }
    }
}
