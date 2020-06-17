using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Persistence.Interfaces;
using Dapper;

namespace Persistence
{
    public class DapperContext : IContext
    {
        private bool isTransactionCommitted;
        private bool isDisposed;

        public DapperContext(SqlTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            this.Connection = transaction.Connection;
            this.Transaction = transaction;

            isDisposed = false;
            isTransactionCommitted = false;
        }
        public SqlConnection Connection { get; }
        public SqlTransaction Transaction { get; }
        public IReadOnlyList<dynamic> Query(string sql, object param)
        {
            return Connection.Query(sql, param, Transaction).ToList();
        }
        public IList<T> Query<T>(string sql, object param)
        {          
            return Connection.Query<T>(sql, param, Transaction).ToList();
        }
        public IList<T> Query<T>(string sql)
        {
            return Connection.Query<T>(sql, null, Transaction).ToList();
        }
        public T ExecuteScalar<T>(string sql, object param = null)
        {
            return (T)Connection.ExecuteScalar(sql, param, Transaction);
        }
        public int Execute(string sql, object param = null,int? commandTimeout=null)
        {
            return Connection.Execute(sql, param, Transaction,commandTimeout: commandTimeout);
        }
        public virtual void Commit()
        {
            Transaction.Commit();
            isTransactionCommitted = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~DapperContext()
        {
            Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }
            if (disposing)
            {
                if (Transaction != null)
                {
                    if (!isTransactionCommitted) { Transaction.Rollback(); }

                    Transaction.Dispose();
                }
                if (Connection != null) { Connection.Dispose(); }
            }
            isDisposed = true;
        }
    }
}