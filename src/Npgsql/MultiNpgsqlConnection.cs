using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Npgsql
{

    public class MultiNpgsqlConnection : DbConnection
    {
        [ThreadStatic]
        private static Dictionary<string, List<NpgsqlConnection>> globalPool;
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
                return GetAvailableConnection(true).Database;
            }
        }
        public override string DataSource
        {
            get
            {
                return GetAvailableConnection(true).DataSource;
            }
        }
        public override string ServerVersion
        {
            get
            {
                return GetAvailableConnection(true).ServerVersion;
            }
        }
        public override ConnectionState State
        {
            get
            {
                return ConnectionState.Open;
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
        }
        public override void Open()
        {
        }
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return this.GetAvailableConnection(true).BeginTransaction(isolationLevel);
        }
        protected override DbCommand CreateDbCommand()
        {
            return this.GetAvailableConnection(true).CreateCommand();
        }
        public async Task<DbCommand> CreateCommandAsync()
        {
            var conn = this.GetAvailableConnection(false);
            if (conn.State == ConnectionState.Closed)
            {
                await Task.Run(() => conn.Open());
                var pool = GetPool(false);
                pool.Add(conn);
            }
            return conn.CreateCommand();
        }
        private NpgsqlConnection GetAvailableConnection(bool open)
        {
            if (globalPool == null) globalPool = new Dictionary<string, List<NpgsqlConnection>>();
            List<NpgsqlConnection> l = GetPool(true);
            var now = Environment.TickCount;
            for (int i = 0; i<l.Count; i++)
            {
                var current = l[i];
            
            
                if (current.FullState == ConnectionState.Open && current.ActiveCommands == 0 && (current.Connector == null || current.Connector.Transaction == null))
                {
                    var lastUsed = current.Connector.Buffer.LastReceivedMessageTickCount;
                    if (now - lastUsed > 20000 || now < lastUsed)
                    {
                        Task.Run(() => current.Dispose());
                        l.RemoveAt(i);
                        i--;
                        continue;
                    }
                    return current;
                }
            }
            NpgsqlConnection npgsqlConnection = new NpgsqlConnection(this.connectionString);
            if (open)
            {
                npgsqlConnection.Open();
                l.Add(npgsqlConnection);
            }

            return npgsqlConnection;
        }

        private List<NpgsqlConnection> GetPool(bool removeBroken)
        {
            List<NpgsqlConnection> l;
            if (!globalPool.TryGetValue(connectionString, out l))
            {
                l = new List<NpgsqlConnection>();
                globalPool.Add(connectionString, l);
            }
            if (removeBroken)
            {
                for (int i = 0; i<l.Count; i++)
                {
                    var c = l[i];
                    if (c.FullState == ConnectionState.Broken)
                    {
                        l.RemoveAt(i);
                        i--;
                    }
                }
            }
            return l;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            //if (disposing && this.pool != null)
            //{
            //    using (List<NpgsqlConnection>.Enumerator enumerator = this.pool.GetEnumerator())
            //    {
            //        while (enumerator.MoveNext())
            //        {
            //            enumerator.Current.Dispose();
            //        }
            //    }
            //    this.pool = null;
            //}
        }
    }
}