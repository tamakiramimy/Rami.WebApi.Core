using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Framework
{
    /// <summary>
    /// 数据仓储接口
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// 数据库实例
        /// </summary>
        SugarDbContext Context { get; }

        #region 新增

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        int Add<T>(T item) where T : class, new();

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="item"></param>
        /// <param name="insertColumns"></param>
        /// <returns></returns>
        int Add<T>(T item, Expression<Func<T, object>> insertColumns = null) where T : class, new();

        /// <summary>
        /// 批量插入实体(速度快)
        /// </summary>
        /// <param name="lstData"></param>
        /// <returns></returns>
        int Add<T>(List<T> lstData) where T : class, new();

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<int> AddAsync<T>(T item) where T : class, new();

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="item"></param>
        /// <param name="insertColumns"></param>
        /// <returns></returns>
        Task<int> AddAsync<T>(T item, Expression<Func<T, object>> insertColumns = null) where T : class, new();

        /// <summary>
        /// 批量插入实体(速度快)
        /// </summary>
        /// <param name="lstData"></param>
        /// <returns></returns>
        Task<int> AddAsync<T>(List<T> lstData) where T : class, new();

        #endregion

        #region 保存

        /// <summary>
        /// 保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Save<T>(T item) where T : class, new();

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lstData"></param>
        /// <returns></returns>
        List<T> Save<T>(List<T> lstData) where T : class, new();

        /// <summary>
        /// 保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<T> SaveAsync<T>(T item) where T : class, new();

        /// <summary>
        /// 批量保存 不支持多组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lstData"></param>
        /// <returns></returns>
        Task<List<T>> SaveAsync<T>(List<T> lstData) where T : class, new();

        #endregion

        #region 删除

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteById<T>(object id) where T : class, new();

        /// <summary>
        /// 根据实体删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool Delete<T>(T model) where T : class, new();

        /// <summary>
        /// 根据主键列表删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        bool DeleteByIds<T>(object[] ids) where T : class, new();

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteByIdAsync<T>(object id) where T : class, new();

        /// <summary>
        /// 根据实体删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync<T>(T model) where T : class, new();

        /// <summary>
        /// 根据主键列表删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<bool> DeleteByIdsAsync<T>(object[] ids) where T : class, new();

        #endregion

        #region 更新

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool Update<T>(T model) where T : class, new();

        /// <summary>
        /// 根据条件更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        bool Update<T>(T entity, string strWhere) where T : class, new();

        /// <summary>
        /// 根据Sql更新实体
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        bool Update<T>(string strSql, SugarParameter[] parameters = null) where T : class, new();

        /// <summary>
        /// 根据列、条件更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="lstColumns"></param>
        /// <param name="lstIgnoreColumns"></param>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        bool Update<T>(T entity, List<string> lstColumns = null, List<string> lstIgnoreColumns = null, string strWhere = "") where T : class, new();

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync<T>(T model) where T : class, new();

        /// <summary>
        /// 根据条件更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync<T>(T entity, string strWhere) where T : class, new();

        /// <summary>
        /// 根据Sql更新实体
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync<T>(string strSql, SugarParameter[] parameters = null) where T : class, new();

        /// <summary>
        /// 根据列、条件更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="lstColumns"></param>
        /// <param name="lstIgnoreColumns"></param>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync<T>(T entity, List<string> lstColumns = null, List<string> lstIgnoreColumns = null, string strWhere = "") where T : class, new();

        #endregion

        #region 主键查询

        /// <summary>
        /// 根据ID查询对象
        /// </summary>
        /// <param name="objId"></param>
        /// <returns></returns>
        T QueryById<T>(object objId) where T : class, new();

        /// <summary>
        /// 根据ID查询对象
        /// </summary>
        /// <param name="objId"></param>
        /// <param name="blnUseCache"></param>
        /// <returns></returns>
        T QueryById<T>(object objId, bool blnUseCache = false) where T : class, new();

        /// <summary>
        /// 根据IDS 查询列表
        /// </summary>
        /// <param name="lstIds"></param>
        /// <returns></returns>
        List<T> QueryByIDs<T>(object[] lstIds) where T : class, new();

        /// <summary>
        /// 根据ID查询对象
        /// </summary>
        /// <param name="objId"></param>
        /// <returns></returns>
        Task<T> QueryByIdAsync<T>(object objId) where T : class, new();

        /// <summary>
        /// 根据ID查询对象
        /// </summary>
        /// <param name="objId"></param>
        /// <param name="blnUseCache"></param>
        /// <returns></returns>
        Task<T> QueryByIdAsync<T>(object objId, bool blnUseCache = false) where T : class, new();

        /// <summary>
        /// 根据IDS 查询列表
        /// </summary>
        /// <param name="lstIds"></param>
        /// <returns></returns>
        Task<List<T>> QueryByIDsAsync<T>(object[] lstIds) where T : class, new();

        #endregion

        #region 查询第一条

        /// <summary>
        /// 根据Linq查询第一条数据
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        T FirstOrDefault<T>(Expression<Func<T, bool>> whereExpression) where T : class, new();

        /// <summary>
        /// 根据Where语句查询第一条数据
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        T FirstOrDefault<T>(string strWhere) where T : class, new();

        /// <summary>
        /// 根据Linq查询第一条数据
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> whereExpression) where T : class, new();

        /// <summary>
        /// 根据Where语句查询第一条数据
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        Task<T> FirstOrDefaultAsync<T>(string strWhere) where T : class, new();

        #endregion

        #region 查询列表

        /// <summary>
        /// 查询实体列表
        /// </summary>
        /// <returns></returns>
        List<T> Query<T>() where T : class, new();

        /// <summary>
        /// 根据Para（SQL WHERE LINQ）查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="para"></param>
        /// <returns></returns>
        List<T> Query<T>(Para<T> para) where T : class, new();

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        List<T> Query<T>(string strWhere) where T : class, new();

        /// <summary>
        /// 根据Linq查询
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        List<T> Query<T>(Expression<Func<T, bool>> whereExpression) where T : class, new();

        /// <summary>
        /// 根据Linq查询并排序
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="strOrderByFileds"></param>
        /// <returns></returns>
        List<T> Query<T>(Expression<Func<T, bool>> whereExpression, string strOrderByFileds) where T : class, new();

        /// <summary>
        /// 根据Linq查询和排序
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        List<T> Query<T>(Expression<Func<T, bool>> whereExpression, Expression<Func<T, object>> orderByExpression, bool isAsc = true) where T : class, new();

        /// <summary>
        /// 根据Where居于查询和排序
        /// </summary>
        /// <param name="strWhere"></param>
        /// <param name="strOrderByFileds"></param>
        /// <returns></returns>
        List<T> Query<T>(string strWhere, string strOrderByFileds) where T : class, new();

        /// <summary>
        /// 根据Linq查询和排序，并返回Top条
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="intTop"></param>
        /// <param name="strOrderByFileds"></param>
        /// <returns></returns>
        List<T> Query<T>(Expression<Func<T, bool>> whereExpression, int intTop, string strOrderByFileds) where T : class, new();

        /// <summary>
        /// 根据Where语句和排序，并返回Top条
        /// </summary>
        /// <param name="strWhere"></param>
        /// <param name="intTop"></param>
        /// <param name="strOrderByFileds"></param>
        /// <returns></returns>
        List<T> Query<T>(string strWhere, int intTop, string strOrderByFileds) where T : class, new();

        /// <summary>
        /// 根据Linq查询和排序，并返回分页数据
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="intPageIndex"></param>
        /// <param name="intPageSize"></param>
        /// <param name="strOrderByFileds"></param>
        /// <returns></returns>
        List<T> Query<T>(Expression<Func<T, bool>> whereExpression, int intPageIndex, int intPageSize, string strOrderByFileds) where T : class, new();

        /// <summary>
        /// 根据Where语句查询和排序，并返回分页数据
        /// </summary>
        /// <param name="strWhere"></param>
        /// <param name="intPageIndex"></param>
        /// <param name="intPageSize"></param>
        /// <param name="strOrderByFileds"></param>
        /// <returns></returns>
        List<T> Query<T>(string strWhere, int intPageIndex, int intPageSize, string strOrderByFileds) where T : class, new();

        /// <summary>
        /// 根据Linq查询和排序，并返回分页信息
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="intPageIndex"></param>
        /// <param name="intPageSize"></param>
        /// <param name="strOrderByFileds"></param>
        /// <returns></returns>
        Pager<T> QueryPage<T>(Expression<Func<T, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null) where T : class, new();

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="para"></param>
        /// <returns></returns>
        Pager<T> QueryPage<T>(Para<T> para) where T : class, new();

        /// <summary>
        /// 多表查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="joinExpression"></param>
        /// <param name="selectExpression"></param>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        List<TResult> QueryMuch<T, T2, T3, TResult>(
            Expression<Func<T, T2, T3, object[]>> joinExpression,
            Expression<Func<T, T2, T3, TResult>> selectExpression,
            Expression<Func<T, T2, T3, bool>> whereLambda = null) where T : class, new();

        /// <summary>
        /// 查询实体列表
        /// </summary>
        /// <returns></returns>
        Task<List<T>> QueryAsync<T>() where T : class, new();

        /// <summary>
        /// 根据Para（SQL WHERE LINQ）查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="para"></param>
        /// <returns></returns>
        Task<List<T>> QueryAsync<T>(Para<T> para) where T : class, new();

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        Task<List<T>> QueryAsync<T>(string strWhere) where T : class, new();

        /// <summary>
        /// 根据Linq查询
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        Task<List<T>> QueryAsync<T>(Expression<Func<T, bool>> whereExpression) where T : class, new();

        /// <summary>
        /// 根据Linq查询并排序
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="strOrderByFileds"></param>
        /// <returns></returns>
        Task<List<T>> QueryAsync<T>(Expression<Func<T, bool>> whereExpression, string strOrderByFileds) where T : class, new();

        /// <summary>
        /// 根据Linq查询和排序
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        Task<List<T>> QueryAsync<T>(Expression<Func<T, bool>> whereExpression, Expression<Func<T, object>> orderByExpression, bool isAsc = true) where T : class, new();

        /// <summary>
        /// 根据Where居于查询和排序
        /// </summary>
        /// <param name="strWhere"></param>
        /// <param name="strOrderByFileds"></param>
        /// <returns></returns>
        Task<List<T>> QueryAsync<T>(string strWhere, string strOrderByFileds) where T : class, new();

        /// <summary>
        /// 根据Linq查询和排序，并返回Top条
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="intTop"></param>
        /// <param name="strOrderByFileds"></param>
        /// <returns></returns>
        Task<List<T>> QueryAsync<T>(Expression<Func<T, bool>> whereExpression, int intTop, string strOrderByFileds) where T : class, new();

        /// <summary>
        /// 根据Where语句和排序，并返回Top条
        /// </summary>
        /// <param name="strWhere"></param>
        /// <param name="intTop"></param>
        /// <param name="strOrderByFileds"></param>
        /// <returns></returns>
        Task<List<T>> QueryAsync<T>(string strWhere, int intTop, string strOrderByFileds) where T : class, new();

        /// <summary>
        /// 根据Linq查询和排序，并返回分页数据
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="intPageIndex"></param>
        /// <param name="intPageSize"></param>
        /// <param name="strOrderByFileds"></param>
        /// <returns></returns>
        Task<List<T>> QueryAsync<T>(Expression<Func<T, bool>> whereExpression, int intPageIndex, int intPageSize, string strOrderByFileds) where T : class, new();

        /// <summary>
        /// 根据Where语句查询和排序，并返回分页数据
        /// </summary>
        /// <param name="strWhere"></param>
        /// <param name="intPageIndex"></param>
        /// <param name="intPageSize"></param>
        /// <param name="strOrderByFileds"></param>
        /// <returns></returns>
        Task<List<T>> QueryAsync<T>(string strWhere, int intPageIndex, int intPageSize, string strOrderByFileds) where T : class, new();

        /// <summary>
        /// 根据Linq查询和排序，并返回分页信息
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="intPageIndex"></param>
        /// <param name="intPageSize"></param>
        /// <param name="strOrderByFileds"></param>
        /// <returns></returns>
        Task<Pager<T>> QueryPageAsync<T>(Expression<Func<T, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null) where T : class, new();

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="para"></param>
        /// <returns></returns>
        Task<Pager<T>> QueryPageAsync<T>(Para<T> para) where T : class, new();

        /// <summary>
        /// 多表查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="joinExpression"></param>
        /// <param name="selectExpression"></param>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        Task<List<TResult>> QueryMuchAsync<T, T2, T3, TResult>(
            Expression<Func<T, T2, T3, object[]>> joinExpression,
            Expression<Func<T, T2, T3, TResult>> selectExpression,
            Expression<Func<T, T2, T3, bool>> whereLambda = null) where T : class, new();

        #endregion

        #region 执行SQL返回结果

        /// <summary>
        /// 执行Sql,返回List T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        List<T> SqlQuery<T>(string sql, object parameters = null);

        /// <summary>
        /// 执行Sql，返回T（单条记录）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        T SqlQuerySingle<T>(string sql, object parameters = null);

        /// <summary>
        /// 执行Sql返回影响条数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int ExecuteCommand(string sql, object parameters = null);

        /// <summary>
        /// 执行Sql，返回object
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        object GetScalar(string sql, object parameters = null);

        /// <summary>
        /// 执行Sql，返回T
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        T GetScalar<T>(string sql, object parameters = null);

        /// <summary>
        /// 执行Sql,返回List T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<List<T>> SqlQueryAsync<T>(string sql, object parameters = null);

        /// <summary>
        /// 执行Sql,返回 T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<T> SqlQuerySingleAsync<T>(string sql, object parameters = null);

        /// <summary>
        /// 执行Sql返回影响条数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<int> ExecuteCommandAsync(string sql, object parameters = null);

        /// <summary>
        /// 执行Sql，返回object
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<object> GetScalarAsync(string sql, object parameters = null);

        /// <summary>
        /// 执行Sql，返回T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<T> GetScalarAsync<T>(string sql, object parameters = null);

        #endregion

        #region 事务

        /// <summary>
        /// 开始事务
        /// </summary>
        void BeginTran();

        /// <summary>
        /// 提交事务
        /// </summary>
        void CommitTran();

        /// <summary>
        /// 回滚事务
        /// </summary>
        void RollbackTran();

        /// <summary>
        /// 事务Func
        /// </summary>
        /// <param name="run"></param>
        /// <returns></returns>
        Result WorkTransaction(Func<Result> run);

        /// <summary>
        /// 事务Action
        /// </summary>
        /// <param name="run"></param>
        /// <returns></returns>
        Result RunTransaction(Action run);

        /// <summary>
        /// 事务Func 异步
        /// </summary>
        /// <param name="run"></param>
        /// <returns></returns>
        Task<Result> WorkTransactionAsync(Func<Task<Result>> run);

        #endregion
    }
}
