﻿using System;
using Microsoft.Data.SQLite;

namespace Microsoft.Data.Entity.SQLite.FunctionalTests
{
    public class TestDatabase : IDisposable
    {
        private readonly SQLiteConnection _connection;
        private readonly SQLiteTransaction _transaction;

        public TestDatabase(string connectionString)
        {
            _connection = new SQLiteConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public SQLiteConnection Connection
        {
            get { return _connection; }
        }

        public static TestDatabase Northwind()
        {
            return new TestDatabase("Filename=northwind.db");
        }

        public void Dispose()
        {
            _transaction.Dispose();
            _connection.Dispose();
        }
    }
}
