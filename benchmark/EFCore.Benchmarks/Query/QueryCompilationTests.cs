// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore.Benchmarks.Models.Orders;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

// ReSharper disable ReturnValueOfPureMethodIsNotUsed
// ReSharper disable InconsistentNaming

namespace Microsoft.EntityFrameworkCore.Benchmarks.Query
{
    [DisplayName(nameof(QueryCompilationTests))]
    public abstract class QueryCompilationTests
    {
        private OrdersContextBase _context;
        private IQueryable<Product> _simpleQuery;
        private IQueryable<DTO> _complexQuery;

        public abstract OrdersFixtureBase CreateFixture();
        public abstract IServiceCollection AddProviderServices(IServiceCollection services);

        [GlobalSetup]
        public virtual void InitializeContext()
        {
            var fixture = CreateFixture();
            fixture.Initialize(0, 0, 0, 0);

            var noQueryCacheServiceProvider = AddProviderServices(new ServiceCollection())
                .AddSingleton<IMemoryCache, NonCachingMemoryCache>()
                .BuildServiceProvider();

            _context = fixture.CreateContext(noQueryCacheServiceProvider);
            _simpleQuery = _context.Products
                .AsNoTracking();
            _complexQuery = _context.Products
                .AsNoTracking()
                .Where(p => p.Retail < 1000)
                .OrderBy(p => p.Name).ThenBy(p => p.Retail)
                .Select(
                    p => new DTO
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Description = p.Description,
                        ActualStockLevel = p.ActualStockLevel,
                        SKU = p.SKU,
                        Savings = p.Retail - p.CurrentPrice,
                        Surplus = p.ActualStockLevel - p.TargetStockLevel
                    });
        }

        [GlobalCleanup]
        public virtual void CleanupContext()
        {
            _context.Dispose();
        }

        [Benchmark]
        public virtual void ToList()
        {
            for (var i = 0; i < 10; i++)
            {
                _simpleQuery.ToList();
            }
        }

        [Benchmark]
        public virtual void FilterOrderProject()
        {
            for (var i = 0; i < 10; i++)
            {
                _complexQuery.ToList();
            }
        }

        private class DTO
        {
            public int ProductId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int ActualStockLevel { get; set; }
            public string SKU { get; set; }
            public decimal Savings { get; set; }
            public int Surplus { get; set; }
        }

        private class NonCachingMemoryCache : IMemoryCache
        {
            public bool TryGetValue(object key, out object value)
            {
                value = null;
                return false;
            }

            public ICacheEntry CreateEntry(object key)
            {
                return new FakeEntry();
            }

            private class FakeEntry : ICacheEntry
            {
                public virtual void Dispose()
                {
                }

                public object Key { get; }
                public object Value { get; set; }
                public DateTimeOffset? AbsoluteExpiration { get; set; }
                public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
                public TimeSpan? SlidingExpiration { get; set; }
                public IList<IChangeToken> ExpirationTokens { get; }
                public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks { get; }
                public CacheItemPriority Priority { get; set; }
                public long? Size { get; set; }
            }

            public virtual void Remove(object key)
            {
            }

            public virtual void Dispose()
            {
            }
        }
    }
}
