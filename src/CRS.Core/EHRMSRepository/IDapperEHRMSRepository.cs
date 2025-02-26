using Dapper;
using System.Data;
using System.Linq.Expressions;
using Z.Dapper.Plus;

namespace CRS.Core.EHRMSRepository;

public interface IDapperEHRMSRepository<T> where T : class
{
    Task<List<T>> FindAllAsync();
    Task<T> FindByIdAsync(int id);
    object UpdateFields<TS>(T param, IDbConnection connection, IDbTransaction transaction = null, int? commandTimeOut = null);
    Task<List<T>> GetTableData<T>(IDbConnection connection, IDbTransaction trns, string sWhere = "", string sOrderBy = "");
    Task<List<T>> GetTableData<T>(string sQuery, IDbConnection connection, IDbTransaction trans = null);
    Task<List<T>> GetTableData<T>(string sQuery, object dbParam, IDbConnection connection, IDbTransaction trans = null);
    Task<List<T>> GetTableData<T>(string sQuery);
    Task<T> GetEntityData<T>(string sQuery, object parameters = null);
    Task<int> GetEntityCount(string sQuery, object parameters = null);
    Task<List<T>> GetTableData<T>(string sQuery, object parameters = null);
    Task<List<T>> GetTableDataExec<T>(string sQuery, object dbParam = null);

    Task<int> ExecuteAddAsync(T entity, IDbConnection dbConnection, IDbTransaction transaction = null, int? timeOut = null);
    Task<bool> ExecuteUpdateAsync(T entity, IDbConnection dbConnection, IDbTransaction transaction = null, int? timeOut = null);
    Task<bool> ExecuteDeleteAsync(int id, IDbConnection dbConnection, IDbTransaction transaction = null, int? timeOut = null);
    Task<int> AddAsync(T entity);
    Task<DapperPlusActionSet<T>> AddRangeAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteTableData<T>(IDbConnection connection, IDbTransaction trans = null, string sWhere = "");
    Task<bool> DeleteTableData<T>(IDbConnection connection, IDbTransaction trans = null, string sWhere = "", object paramsObject = null);
    Task<List<T>> GetFilterAll(Expression<Func<T, bool>> filter);
    Task<List<T>> GetFilterAll(Expression<Func<T, bool>> filter, Expression<Func<T, bool>> orderBy);
    Task<T> GetFilter(Expression<Func<T, bool>> filter);
    Task<bool> Exists(Expression<Func<T, bool>> filter);
    Task<List<T>> GetDynamicQuery(string query, object dynamicParameters);
    Task<bool> IsExists(string query, object dynamicParameters);

    Task<bool> ExecuteQuery(string query, object dynamicParameters);

    Task<List<TS>> ExecuteQuery<TS>(string query, object dynamicParameters);

    Task<List<T>> GetQueryAll(string query);
    Task<int> GetStoredProcedure(string storedProcedure, DynamicParameters dynamicParameters);
    Task<List<T>> GetAllPagedAsync(int limit, int offset, string sWhere = "", string sOrderBy = "");

    Task<List<T>> GetSPData(string spName = "", DynamicParameters spInput = null);
    Task<List<T1>> GetSPData<T1>(string spName = "", DynamicParameters spInput = null);
    Task<List<T1>> GetSPData<T1>(string spName = "", object spInput = null);
    Task<List<T>> GetSPData<T>(IDbConnection connection, IDbTransaction trans = null, string spName = "", DynamicParameters spInput = null);

    void CallProcedure<TInput, TOutput>(
       string storedProcedure,
       TInput inputObject,
       TOutput outputObject,
       string connectionId,
       params Expression<Func<TOutput, object>>[] outputExpressions
       );

    Task CallProcedureAsync<TInput, TOutput>(
        string storedProcedure,
        TInput inputObject,
        TOutput outputObject,
        string connectionId = "Default",
        params Expression<Func<TOutput, object>>[] outputExpressions
        );

    Task<bool> ExecuteListData<T>(List<T> listData, string sQuery, bool isCommitRollback = false);

    Task<bool> RunSQLCommand(string sQuery);

}
