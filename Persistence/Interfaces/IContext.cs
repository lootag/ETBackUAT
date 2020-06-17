using System;
using System.Collections.Generic;
namespace Persistence.Interfaces
{
    public interface IContext : IDisposable
    {
        IReadOnlyList<dynamic> Query(string sql, object param);
        IList<T> Query<T>(string sql, object param);
        IList<T> Query<T>(string sql);
        T ExecuteScalar<T>(string sql, object param = null);
        int Execute(string sql, object param = null, int? commandTimeout = null);
        void Commit();
    }
}