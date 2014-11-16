using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
namespace Npgsql
{
    public class MultiNpgsqlConnection : DbConnection
    {
        public NpgsqlConnection primary;
        private List<NpgsqlConnection> pool;
        private string connectionString;
        public override string ConnectionString
        {
            get
            {
                return this.connectionString;
            }
            set
            {
                throw new NotSupportedException();
            }
        }
        public override string Database
        {
            get
            {
                return this.primary.Database;
            }
        }
        public override string DataSource
        {
            get
            {
                return this.primary.DataSource;
            }
        }
        public override string ServerVersion
        {
            get
            {
                return this.primary.ServerVersion;
            }
        }
        public override ConnectionState State
        {
            get
            {
                if (this.pool == null)
                {
                    return ConnectionState.Closed;
                }
                return ConnectionState.Open;
            }
        }
        public IEnumerable<NpgsqlConnection> PooledConnections
        {
            get
            {
                return this.pool;
            }
        }
        public MultiNpgsqlConnection(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public override void ChangeDatabase(string databaseName)
        {
            throw new NotSupportedException();
        }
        public override void Close()
        {
            if (this.pool != null)
            {
                using (List<NpgsqlConnection>.Enumerator enumerator = this.pool.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.Close();
                    }
                }
                this.pool = null;
            }
        }
        public override void Open()
        {
            if (this.pool == null)
            {
                this.pool = new List<NpgsqlConnection>();
                this.primary = new NpgsqlConnection(this.connectionString);
                this.pool.Add(this.primary);
                this.primary.Open();
            }
        }
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return this.GetAvailableConnection().BeginTransaction(isolationLevel);
        }
        protected override DbCommand CreateDbCommand()
        {
            return this.GetAvailableConnection().CreateCommand();
        }
        private NpgsqlConnection GetAvailableConnection()
        {
            foreach (NpgsqlConnection current in this.pool)
            {
                if (current.FullState == ConnectionState.Open)
                {
                    return current;
                }
            }
            NpgsqlConnection npgsqlConnection = new NpgsqlConnection(this.connectionString);
            this.pool.Add(npgsqlConnection);
            npgsqlConnection.Open();
            return npgsqlConnection;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && this.pool != null)
            {
                using (List<NpgsqlConnection>.Enumerator enumerator = this.pool.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.Dispose();
                    }
                }
                this.pool = null;
            }
        }
    }
}
