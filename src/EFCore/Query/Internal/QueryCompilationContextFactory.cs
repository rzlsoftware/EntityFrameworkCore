// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.EntityFrameworkCore.Query.Internal
{
    public class QueryCompilationContextFactory : IQueryCompilationContextFactory
    {
        private readonly QueryCompilationContextDependencies _dependencies;

        public QueryCompilationContextFactory(QueryCompilationContextDependencies dependencies)
        {
            _dependencies = dependencies;
        }

        public virtual QueryCompilationContext Create(bool async)
            => new QueryCompilationContext(_dependencies, async);
    }
}
