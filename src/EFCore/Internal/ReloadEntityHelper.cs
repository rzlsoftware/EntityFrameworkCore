using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.Internal
{
    /// <summary>
    /// ReloadEntityHelper offers a synchronization of InternalEntityEntry to ValueBuffer.
    /// </summary>
    public static class ReloadEntityHelper
    {
        /// <summary>
        /// Set the Original- and CurrentValues of the DbContext-EntityEntry to the Values of the ValueBuffer.
        /// </summary>
        /// <param name="entry">Internal EntityEntry, which should be refreshed</param>
        /// <param name="valueBuffer">ValueBuffer, which is loaded from Database</param>
        /// <param name="dbContext">DbContext of the EntityEntry</param>
        /// <param name="entityFactory">EntityFactory, which creates an new entity of the ValueBuffer</param>
        public static void RefreshInternalEntityEntry(InternalEntityEntry entry, in ValueBuffer valueBuffer, DbContext dbContext, Func<object> entityFactory)
        {
            //var values = new object[valueBuffer.Count];
            //for (var i = 0; i < valueBuffer.Count; i++)
            //{
            //    values[i] = valueBuffer[i];
            //}

            //var dbContextEntry = dbContext.Entry(entry.Entity);
            //var arrayPropertyValues = new ArrayPropertyValues(entry, values);
            //dbContextEntry.CurrentValues.SetValues(arrayPropertyValues);
            //dbContextEntry.OriginalValues.SetValues(arrayPropertyValues);
            //dbContextEntry.State = EntityState.Unchanged;

            // REMARK: this code above does not work, because the value order of the value buffer is different to the properties-order in the entityentry
            // Alternative solution (slower -> we create a new entity only for the assignment)

            var dbContextEntry = new EntityEntry(entry);
            var entityWithNewValues = entityFactory();
            new CurrentPropertyValues(entry).SetValues(entityWithNewValues);
            new OriginalPropertyValues(entry).SetValues(entityWithNewValues);
            dbContextEntry.State = EntityState.Unchanged;
        }
    }
}
