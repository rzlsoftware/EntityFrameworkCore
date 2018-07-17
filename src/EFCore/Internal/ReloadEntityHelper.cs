using System;
using System.Collections.Generic;
using System.Text;
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
        public static void RefreshInternalEntityEntry(InternalEntityEntry entry, in ValueBuffer valueBuffer, DbContext dbContext)
        {
            var values = new object[valueBuffer.Count];
            for (var i = 0; i < valueBuffer.Count; i++)
            {
                values[i] = valueBuffer[i];
            }
            var dbContextEntry = dbContext.Entry(entry.Entity);
            var arrayPropertyValues = new ArrayPropertyValues(entry, values);
            dbContextEntry.CurrentValues.SetValues(arrayPropertyValues);
            dbContextEntry.OriginalValues.SetValues(arrayPropertyValues);
            dbContextEntry.State = EntityState.Unchanged;
        }
    }
}
