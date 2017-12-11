using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLibrary.DAL;

namespace MyLibrary.BL
{
    public class ItemsManager : EntityManager<LibraryItem>
    {
        public ItemsManager(User currentUser = null) : base(currentUser)
        { }

        public override LibraryItem Add(LibraryItem item)
        {
            LibraryItem existing = _entities.FirstOrDefault(i => i.Equals(item));
            if (existing != null)
            {
                existing.AddCopy();
                existing.Update(item.EntityId());
                return existing;
            }
            else
                return base.Add(item);
        }
    }
}
