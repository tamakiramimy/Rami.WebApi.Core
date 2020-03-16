using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Rami.WebApi.Core.Framework
{
    /// <summary>
    /// 数据仓储-SqlSugar实现
    /// </summary>
    public class RepositorySugar : IRepository
    {
        #region 数据仓储上下文

        /// <summary>
        /// 数据库上下文
        /// </summary>
        private SugarDbContext _context;

        /// <summary>
        /// 数据库上下文
        /// </summary>
        public SugarDbContext Context
        {
            get { return _context; }
            set { _context = value; }
        }

        /// <summary>
        /// 数据库实例
        /// </summary>
        private ISqlSugarClient _db;

        /// <summary>
        /// 数据库实例
        /// </summary>
        internal ISqlSugarClient Db
        {
            get { return _db; }
            private set { _db = value; }
        }

        /// <summary>
        /// Ado
        /// </summary>
        private IAdo Ado => Db.Ado;

        /// <summary>
        /// ILogHelper
        /// </summary>
        private readonly ILogHelper<RepositorySugar> logHelper;

        #endregion

        /// <summary>
        /// 数据库链接
        /// </summary>
        private string _connPath;

        /// <summary>
        /// 构造方法
        /// </summary>
        public RepositorySugar()
        {
            _context = SugarDbContext.GetDbContext();
            _db = _context.Db;
            logHelper = StaticServiceProvider.Current.GetRequiredService<ILogHelper<RepositorySugar>>();
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="connPath"></param>
        public RepositorySugar(string connPath)
        {
            _connPath = connPath;
            _context = SugarDbContext.GetDbContextByConnPath(connPath);
            _db = _context.Db;
            logHelper = StaticServiceProvider.Current.GetRequiredService<ILogHelper<RepositorySugar>>();
        }

        #region 新增

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public int Add<T>(T item) where T : class, new()
        {
            var insert = _db.Insertable(item);
            return insert.ExecuteReturnIdentity();
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="insertColumns">指定只插入列</param>
        /// <returns>返回自增量列</returns>
        public int Add<T>(T entity, Expression<Func<T, object>> insertColumns = null) where T : class, new()
        {
            var insert = _db.Insertable(entity);
            if (insertColumns == null)
            {
                return insert.ExecuteReturnIdentity();
            }
            else
            {
                return insert.InsertColumns(insertColumns).ExecuteReturnIdentity();
            }
        }

        /// <summary>
        /// 批量插入实体(速度快)
        /// </summary>
        /// <param name="lstData">实体集合</param>
        /// <returns>影响行数</returns>
        public int Add<T>(List<T> lstData) where T : class, new()
        {
            return _db.Insertable(lstData.ToArray()).ExecuteCommand();
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="item">博文实体类</param>
        /// <returns></returns>
        public async Task<int> AddAsync<T>(T item) where T : class, new()
        {
            var insert = _db.Insertable(item);
            return await insert.ExecuteReturnIdentityAsync();
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="insertColumns">指定只插入列</param>
        /// <returns>返回自增量列</returns>
        public async Task<int> AddAsync<T>(T entity, Expression<Func<T, object>> insertColumns = null) where T : class, new()
        {
            var insert = _db.Insertable(entity);
            if (insertColumns == null)
            {
                return await insert.ExecuteReturnIdentityAsync();
            }
            else
            {
                return await insert.InsertColumns(insertColumns).ExecuteReturnIdentityAsync();
            }
        }

        /// <summary>
        /// 批量插入实体(速度快)
        /// </summary>
        /// <param name="lstData">实体集合</param>
        /// <returns>影响行数</returns>
        public async Task<int> AddAsync<T>(List<T> lstData) where T : class, new()
        {
            return await _db.Insertable(lstData.ToArray()).ExecuteCommandAsync();
        }

        #endregion

        #region 保存

        /// <summary>
        /// 保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Save<T>(T item) where T : class, new()
        {
            return _db.Saveable(item).ExecuteReturnEntity();
        }

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lstData"></param>
        /// <returns></returns>
        public List<T> Save<T>(List<T> lstData) where T : class, new()
        {
            return _db.Saveable(lstData).ExecuteReturnList();
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> SaveAsync<T>(T item) where T : class, new()
        {
            return await _db.Saveable(item).ExecuteReturnEntityAsync();
        }

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lstData"></param>
        /// <returns></returns>
        public async Task<List<T>> SaveAsync<T>(List<T> lstData) where T : class, new()
        {
            return await _db.Saveable(lstData).ExecuteReturnListAsync();
        }

        #endregion

        #region 删除

        /// <summary>
        /// 根据实体删除一条数据
        /// </summary>
        /// <param name="entity">博文实体类</param>
        /// <returns></returns>
        public bool Delete<T>(T entity) where T : class, new()
        {
            return _db.Deleteable(entity).ExecuteCommandHasChange();
        }

        /// <summary>
        /// 删除指定ID的数据
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public bool DeleteById<T>(object id) where T : class, new()
        {
            return _db.Deleteable<T>(id).ExecuteCommandHasChange();
        }

        /// <summary>
        /// 删除指定ID集合的数据(批量删除)
        /// </summary>
        /// <param name="ids">主键ID集合</param>
        /// <returns></returns>
        public bool DeleteByIds<T>(object[] ids) where T : class, new()
        {
            return _db.Deleteable<T>().In(ids).ExecuteCommandHasChange();
        }

        /// <summary>
        /// 根据实体删除一条数据
        /// </summary>
        /// <param name="entity">博文实体类</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync<T>(T entity) where T : class, new()
        {
            return await _db.Deleteable(entity).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 删除指定ID的数据
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public async Task<bool> DeleteByIdAsync<T>(object id) where T : class, new()
        {
            return await _db.Deleteable<T>(id).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 删除指定ID集合的数据(批量删除)
        /// </summary>
        /// <param name="ids">主键ID集合</param>
        /// <returns></returns>
        public async Task<bool> DeleteByIdsAsync<T>(object[] ids) where T : class, new()
        {
            return await _db.Deleteable<T>().In(ids).ExecuteCommandHasChangeAsync();
        }

        #endregion

        #region 更新

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity">博文实体类</param>
        /// <returns></returns>
        public bool Update<T>(T entity) where T : class, new()
        {
            //这种方式会以主键为条件
            return _db.Updateable(entity).ExecuteCommandHasChange();
        }

        /// <summary>
        /// 根据条件更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public bool Update<T>(T entity, string strWhere) where T : class, new()
        {
            return _db.Updateable(entity).Where(strWhere).ExecuteCommandHasChange();
        }

        /// <summary>
        /// 根据Sql更新实体
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool Update<T>(string strSql, SugarParameter[] parameters = null) where T : class, new()
        {
            return _db.Ado.ExecuteCommand(strSql, parameters) > 0;
        }

        /// <summary>
        /// 根据列、条件更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="lstColumns"></param>
        /// <param name="lstIgnoreColumns"></param>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public bool Update<T>(
          T entity,
          List<string> lstColumns = null,
          List<string> lstIgnoreColumns = null,
          string strWhere = "") where T : class, new()
        {
            IUpdateable<T> up = _db.Updateable(entity);
            if (lstIgnoreColumns != null && lstIgnoreColumns.Count > 0)
            {
                up = up.IgnoreColumns(lstIgnoreColumns.ToArray());
            }

            if (lstColumns != null && lstColumns.Count > 0)
            {
                up = up.UpdateColumns(lstColumns.ToArray());
            }

            if (!string.IsNullOrEmpty(strWhere))
            {
                up = up.Where(strWhere);
            }

            return up.ExecuteCommandHasChange();
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity">博文实体类</param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync<T>(T entity) where T : class, new()
        {
            //这种方式会以主键为条件
            return await _db.Updateable(entity).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 根据条件更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync<T>(T entity, string strWhere) where T : class, new()
        {
            return await _db.Updateable(entity).Where(strWhere).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 根据Sql更新实体
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync<T>(string strSql, SugarParameter[] parameters = null) where T : class, new()
        {
            return await _db.Ado.ExecuteCommandAsync(strSql, parameters) > 0;
        }

        /// <summary>
        /// 根据列、条件更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="lstColumns"></param>
        /// <param name="lstIgnoreColumns"></param>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync<T>(
          T entity,
          List<string> lstColumns = null,
          List<string> lstIgnoreColumns = null,
          string strWhere = "") where T : class, new()
        {
            IUpdateable<T> up = _db.Updateable(entity);
            if (lstIgnoreColumns != null && lstIgnoreColumns.Count > 0)
            {
                up = up.IgnoreColumns(lstIgnoreColumns.ToArray());
            }

            if (lstColumns != null && lstColumns.Count > 0)
            {
                up = up.UpdateColumns(lstColumns.ToArray());
            }

            if (!string.IsNullOrEmpty(strWhere))
            {
                up = up.Where(strWhere);
            }

            return await up.ExecuteCommandHasChangeAsync();
        }

        #endregion

        #region 主键查询

        /// <summary>
        /// 功能描述:根据ID查询对象
        /// </summary>
        /// <param name="objId"></param>
        /// <returns></returns>
        public T QueryById<T>(object objId) where T : class, new()
        {
            return _db.Queryable<T>().In(objId).Single();
        }

        /// <summary>
        /// 功能描述:根据ID查询一条数据
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="objId">id（必须指定主键特性 [SugarColumn(IsPrimaryKey=true)]），如果是联合主键，请使用Where条件</param>
        /// <param name="blnUseCache">是否使用缓存</param>
        /// <returns>数据实体</returns>
        public T QueryById<T>(object objId, bool blnUseCache = false) where T : class, new()
        {
            return _db.Queryable<T>().WithCacheIF(blnUseCache).In(objId).Single();
        }

        /// <summary>
        /// 功能描述:根据ID查询数据
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="lstIds">id列表（必须指定主键特性 [SugarColumn(IsPrimaryKey=true)]），如果是联合主键，请使用Where条件</param>
        /// <returns>数据实体列表</returns>
        public List<T> QueryByIDs<T>(object[] lstIds) where T : class, new()
        {
            return _db.Queryable<T>().In(lstIds).ToList();
        }

        /// <summary>
        /// 功能描述:根据ID查询对象
        /// </summary>
        /// <param name="objId"></param>
        /// <returns></returns>
        public async Task<T> QueryByIdAsync<T>(object objId) where T : class, new()
        {
            return await _db.Queryable<T>().In(objId).SingleAsync();
        }

        /// <summary>
        /// 功能描述:根据ID查询一条数据
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="objId">id（必须指定主键特性 [SugarColumn(IsPrimaryKey=true)]），如果是联合主键，请使用Where条件</param>
        /// <param name="blnUseCache">是否使用缓存</param>
        /// <returns>数据实体</returns>
        public async Task<T> QueryByIdAsync<T>(object objId, bool blnUseCache = false) where T : class, new()
        {
            return await _db.Queryable<T>().WithCacheIF(blnUseCache).In(objId).SingleAsync();
        }

        /// <summary>
        /// 功能描述:根据ID查询数据
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="lstIds">id列表（必须指定主键特性 [SugarColumn(IsPrimaryKey=true)]），如果是联合主键，请使用Where条件</param>
        /// <returns>数据实体列表</returns>
        public async Task<List<T>> QueryByIDsAsync<T>(object[] lstIds) where T : class, new()
        {
            return await _db.Queryable<T>().In(lstIds).ToListAsync();
        }

        #endregion

        #region 查询第一条

        /// <summary>
        /// 根据Linq查询第一条数据
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public T FirstOrDefault<T>(Expression<Func<T, bool>> whereExpression) where T : class, new()
        {
            return _db.Queryable<T>().WhereIF(whereExpression != null, whereExpression).First();
        }

        /// <summary>
        /// 根据Where语句查询第一条数据
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public T FirstOrDefault<T>(string strWhere) where T : class, new()
        {
            return _db.Queryable<T>().WhereIF(!string.IsNullOrWhiteSpace(strWhere), strWhere).First();
        }

        /// <summary>
        /// 根据Linq查询第一条数据
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public async Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> whereExpression) where T : class, new()
        {
            return await _db.Queryable<T>().WhereIF(whereExpression != null, whereExpression).FirstAsync();
        }

        /// <summary>
        /// 根据Where语句查询第一条数据
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public async Task<T> FirstOrDefaultAsync<T>(string strWhere) where T : class, new()
        {
            return await _db.Queryable<T>().WhereIF(!string.IsNullOrWhiteSpace(strWhere), strWhere).FirstAsync();
        }

        #endregion

        #region 查询列表

        /// <summary>
        /// Para转ISugarQueryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="para"></param>
        /// <returns></returns>
        private ISugarQueryable<T> GetQueryByPara<T>(Para<T> para) where T : class, new()
        {
            if (!string.IsNullOrEmpty(para.SQL))
            {
                return _db.SqlQueryable<T>(para.SQL)
                    .WhereIF(!string.IsNullOrEmpty(para.WhereSQL), para.WhereSQL)
                    .WhereIF(para.Filter != null, para.Filter)
                    .OrderByIF(!string.IsNullOrEmpty(para.OrderKey), para.OrderKey);
            }
            else
            {
                return _db.Queryable<T>()
                      .WhereIF(!string.IsNullOrEmpty(para.WhereSQL), para.WhereSQL)
                      .WhereIF(para.Filter != null, para.Filter)
                      .OrderByIF(!string.IsNullOrEmpty(para.OrderKey), para.OrderKey);
            }
        }

        /// <summary>
        /// 功能描述:查询所有数据
        /// 作　　者:Blog.Core
        /// </summary>
        /// <returns>数据列表</returns>
        public List<T> Query<T>() where T : class, new()
        {
            return _db.Queryable<T>().ToList();
        }

        /// <summary>
        /// 根据Para（SQL WHERE LINQ）查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="para"></param>
        /// <returns></returns>
        public List<T> Query<T>(Para<T> para) where T : class, new()
        {
            return GetQueryByPara<T>(para).ToList();
        }

        /// <summary>
        /// 功能描述:查询数据列表
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <returns>数据列表</returns>
        public List<T> Query<T>(string strWhere) where T : class, new()
        {
            return _db.Queryable<T>().WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).ToList();
        }

        /// <summary>
        /// 功能描述:查询数据列表
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="whereExpression">whereExpression</param>
        /// <returns>数据列表</returns>
        public List<T> Query<T>(Expression<Func<T, bool>> whereExpression) where T : class, new()
        {
            return _db.Queryable<T>().WhereIF(whereExpression != null, whereExpression).ToList();
        }

        /// <summary>
        /// 功能描述:查询一个列表
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public List<T> Query<T>(Expression<Func<T, bool>> whereExpression, string strOrderByFileds) where T : class, new()
        {
            return _db.Queryable<T>().WhereIF(whereExpression != null, whereExpression)
                .OrderByIF(strOrderByFileds != null, strOrderByFileds).ToList();
        }

        /// <summary>
        /// 功能描述:查询一个列表
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public List<T> Query<T>(Expression<Func<T, bool>> whereExpression, Expression<Func<T, object>> orderByExpression, bool isAsc = true) where T : class, new()
        {
            return _db.Queryable<T>()
                .OrderByIF(orderByExpression != null, orderByExpression, isAsc ? OrderByType.Asc : OrderByType.Desc)
                .WhereIF(whereExpression != null, whereExpression).ToList();
        }

        /// <summary>
        /// 功能描述:查询一个列表
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public List<T> Query<T>(string strWhere, string strOrderByFileds) where T : class, new()
        {
            return _db.Queryable<T>()
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).ToList();
        }

        /// <summary>
        /// 功能描述:查询前N条数据
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intTop">前N条</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public List<T> Query<T>(
            Expression<Func<T, bool>> whereExpression,
            int intTop,
            string strOrderByFileds) where T : class, new()
        {
            return _db.Queryable<T>()
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .WhereIF(whereExpression != null, whereExpression)
                .Take(intTop).ToList();
        }

        /// <summary>
        /// 功能描述:查询前N条数据
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="intTop">前N条</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public List<T> Query<T>(
            string strWhere,
            int intTop,
            string strOrderByFileds) where T : class, new()
        {
            return _db.Queryable<T>()
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .WhereIF(!string.IsNullOrEmpty(strWhere), strWhere)
                .Take(intTop).ToList();
        }

        /// <summary>
        /// 功能描述:分页查询
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public List<T> Query<T>(
            Expression<Func<T, bool>> whereExpression,
            int intPageIndex,
            int intPageSize,
            string strOrderByFileds) where T : class, new()
        {
            return _db.Queryable<T>()
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .WhereIF(whereExpression != null, whereExpression)
                .ToPageList(intPageIndex, intPageSize);
        }

        /// <summary>
        /// 功能描述:分页查询
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public List<T> Query<T>(
          string strWhere,
          int intPageIndex,
          int intPageSize,
          string strOrderByFileds) where T : class, new()
        {
            return _db.Queryable<T>()
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .WhereIF(!string.IsNullOrEmpty(strWhere), strWhere)
                .ToPageList(intPageIndex, intPageSize);
        }

        /// <summary>
        /// 分页查询[使用版本，其他分页未测试]
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns></returns>
        public Pager<T> QueryPage<T>(Expression<Func<T, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null) where T : class, new()
        {
            int totalCount = 0;
            var list = _db.Queryable<T>()
             .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
             .WhereIF(whereExpression != null, whereExpression)
             .ToPageList(intPageIndex, intPageSize, ref totalCount);

            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new Pager<T>() { TotalCount = totalCount, TotalPages = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, Datas = list };
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="para"></param>
        /// <returns></returns>
        public Pager<T> QueryPage<T>(Para<T> para) where T : class, new()
        {
            int totalCount = 0;
            var list = GetQueryByPara<T>(para).ToPageList(para.PageIndex, para.PageSize, ref totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / para.PageSize.ObjToDecimal())).ObjToInt();
            return new Pager<T>() { TotalCount = totalCount, TotalPages = pageCount, PageIndex = para.PageIndex, PageSize = para.PageSize, Datas = list };
        }

        /// <summary> 
        ///查询-多表查询
        /// </summary> 
        /// <typeparam name="T">实体1</typeparam> 
        /// <typeparam name="T2">实体2</typeparam> 
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式 (join1,join2) => new object[] {JoinType.Left,join1.UserNo==join2.UserNo}</param> 
        /// <param name="selectExpression">返回表达式 (s1, s2) => new { Id =s1.UserNo, Id1 = s2.UserNo}</param>
        /// <param name="whereLambda">查询表达式 (w1, w2) =>w1.UserNo == "")</param> 
        /// <returns>值</returns>
        public List<TResult> QueryMuch<T, T2, T3, TResult>(
            Expression<Func<T, T2, T3, object[]>> joinExpression,
            Expression<Func<T, T2, T3, TResult>> selectExpression,
            Expression<Func<T, T2, T3, bool>> whereLambda = null) where T : class, new()
        {
            if (whereLambda == null)
            {
                return _db.Queryable(joinExpression).Select(selectExpression).ToList();
            }

            return _db.Queryable(joinExpression).Where(whereLambda).Select(selectExpression).ToList();
        }

        /// <summary>
        /// 功能描述:查询所有数据
        /// 作　　者:Blog.Core
        /// </summary>
        /// <returns>数据列表</returns>
        public async Task<List<T>> QueryAsync<T>() where T : class, new()
        {
            return await _db.Queryable<T>().ToListAsync();
        }

        /// <summary>
        /// 根据Para（SQL WHERE LINQ）查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="para"></param>
        /// <returns></returns>
        public async Task<List<T>> QueryAsync<T>(Para<T> para) where T : class, new()
        {
            return await GetQueryByPara<T>(para).ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询数据列表
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <returns>数据列表</returns>
        public async Task<List<T>> QueryAsync<T>(string strWhere) where T : class, new()
        {
            return await _db.Queryable<T>().WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询数据列表
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="whereExpression">whereExpression</param>
        /// <returns>数据列表</returns>
        public async Task<List<T>> QueryAsync<T>(Expression<Func<T, bool>> whereExpression) where T : class, new()
        {
            return await _db.Queryable<T>().WhereIF(whereExpression != null, whereExpression).ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询一个列表
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<T>> QueryAsync<T>(Expression<Func<T, bool>> whereExpression, string strOrderByFileds) where T : class, new()
        {
            return await _db.Queryable<T>().WhereIF(whereExpression != null, whereExpression).OrderByIF(strOrderByFileds != null, strOrderByFileds).ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询一个列表
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public async Task<List<T>> QueryAsync<T>(Expression<Func<T, bool>> whereExpression, Expression<Func<T, object>> orderByExpression, bool isAsc = true) where T : class, new()
        {
            return await _db.Queryable<T>()
                .OrderByIF(orderByExpression != null, orderByExpression, isAsc ? OrderByType.Asc : OrderByType.Desc)
                .WhereIF(whereExpression != null, whereExpression)
                .ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询一个列表
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<T>> QueryAsync<T>(string strWhere, string strOrderByFileds) where T : class, new()
        {
            return await _db.Queryable<T>()
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .WhereIF(!string.IsNullOrEmpty(strWhere), strWhere)
                .ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询前N条数据
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intTop">前N条</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<T>> QueryAsync<T>(
            Expression<Func<T, bool>> whereExpression,
            int intTop,
            string strOrderByFileds) where T : class, new()
        {
            return await _db.Queryable<T>()
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .WhereIF(whereExpression != null, whereExpression)
                .Take(intTop).ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询前N条数据
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="intTop">前N条</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<T>> QueryAsync<T>(
            string strWhere,
            int intTop,
            string strOrderByFileds) where T : class, new()
        {
            return await _db.Queryable<T>()
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .WhereIF(!string.IsNullOrEmpty(strWhere), strWhere)
                .Take(intTop).ToListAsync();
        }

        /// <summary>
        /// 功能描述:分页查询
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<T>> QueryAsync<T>(
            Expression<Func<T, bool>> whereExpression,
            int intPageIndex,
            int intPageSize,
            string strOrderByFileds) where T : class, new()
        {
            return await _db.Queryable<T>()
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .WhereIF(whereExpression != null, whereExpression)
                .ToPageListAsync(intPageIndex, intPageSize);
        }

        /// <summary>
        /// 功能描述:分页查询
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<T>> QueryAsync<T>(
          string strWhere,
          int intPageIndex,
          int intPageSize,
          string strOrderByFileds) where T : class, new()
        {
            return await _db.Queryable<T>()
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .WhereIF(!string.IsNullOrEmpty(strWhere), strWhere)
                .ToPageListAsync(intPageIndex, intPageSize);
        }

        /// <summary>
        /// 分页查询[使用版本，其他分页未测试]
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns></returns>
        public async Task<Pager<T>> QueryPageAsync<T>(Expression<Func<T, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null) where T : class, new()
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable<T>()
             .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
             .WhereIF(whereExpression != null, whereExpression)
             .ToPageListAsync(intPageIndex, intPageSize, totalCount);

            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new Pager<T>() { TotalCount = totalCount, TotalPages = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, Datas = list };
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="para"></param>
        /// <returns></returns>
        public async Task<Pager<T>> QueryPageAsync<T>(Para<T> para) where T : class, new()
        {
            RefAsync<int> totalCount = 0;
            var list = await GetQueryByPara<T>(para).ToPageListAsync(para.PageIndex, para.PageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / para.PageSize.ObjToDecimal())).ObjToInt();
            return new Pager<T>() { TotalCount = totalCount, TotalPages = pageCount, PageIndex = para.PageIndex, PageSize = para.PageSize, Datas = list };
        }

        /// <summary> 
        ///查询-多表查询
        /// </summary> 
        /// <typeparam name="T">实体1</typeparam> 
        /// <typeparam name="T2">实体2</typeparam> 
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式 (join1,join2) => new object[] {JoinType.Left,join1.UserNo==join2.UserNo}</param> 
        /// <param name="selectExpression">返回表达式 (s1, s2) => new { Id =s1.UserNo, Id1 = s2.UserNo}</param>
        /// <param name="whereLambda">查询表达式 (w1, w2) =>w1.UserNo == "")</param> 
        /// <returns>值</returns>
        public async Task<List<TResult>> QueryMuchAsync<T, T2, T3, TResult>(
            Expression<Func<T, T2, T3, object[]>> joinExpression,
            Expression<Func<T, T2, T3, TResult>> selectExpression,
            Expression<Func<T, T2, T3, bool>> whereLambda = null) where T : class, new()
        {
            if (whereLambda == null)
            {
                return await _db.Queryable(joinExpression).Select(selectExpression).ToListAsync();
            }

            return await _db.Queryable(joinExpression).Where(whereLambda).Select(selectExpression).ToListAsync();
        }

        #endregion

        #region 执行SQL返回结果

        /// <summary>
        /// 执行Sql,返回List T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<T> SqlQuery<T>(string sql, object parameters = null)
        {
            return _db.Ado.SqlQuery<T>(sql, parameters);
        }

        /// <summary>
        /// 执行Sql，返回T（单条记录）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T SqlQuerySingle<T>(string sql, object parameters = null)
        {
            return _db.Ado.SqlQuerySingle<T>(sql, parameters);
        }

        /// <summary>
        /// 执行Sql返回影响条数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteCommand(string sql, object parameters = null)
        {
            return _db.Ado.ExecuteCommand(sql, parameters);
        }

        /// <summary>
        /// 执行Sql，返回object
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object GetScalar(string sql, object parameters = null)
        {
            return _db.Ado.GetScalar(sql, parameters);
        }

        /// <summary>
        /// 执行Sql，返回T
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T GetScalar<T>(string sql, object parameters = null)
        {
            return (T)GetScalar(sql, parameters);
        }

        /// <summary>
        /// 执行Sql,返回List T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<List<T>> SqlQueryAsync<T>(string sql, object parameters = null)
        {
            return await _db.Ado.SqlQueryAsync<T>(sql, parameters);
        }

        /// <summary>
        /// 执行Sql，返回T（单条记录）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<T> SqlQuerySingleAsync<T>(string sql, object parameters = null)
        {
            return await _db.Ado.SqlQuerySingleAsync<T>(sql, parameters);
        }

        /// <summary>
        /// 执行Sql返回影响条数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<int> ExecuteCommandAsync(string sql, object parameters = null)
        {
            return await _db.Ado.ExecuteCommandAsync(sql, parameters);
        }

        /// <summary>
        /// 执行Sql，返回object
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<object> GetScalarAsync(string sql, object parameters = null)
        {
            return await _db.Ado.GetScalarAsync(sql, parameters);
        }

        /// <summary>
        /// 执行Sql，返回T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<T> GetScalarAsync<T>(string sql, object parameters = null)
        {
            var res = await GetScalarAsync(sql, parameters);
            return (T)res;
        }

        #endregion

        #region 事务

        /// <summary>
        /// 开始事务
        /// </summary>
        public void BeginTran()
        {
            Ado.BeginTran();
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTran()
        {
            try
            {
                Ado.CommitTran();
            }
            catch (Exception ex)
            {
                Ado.RollbackTran();
                logHelper.SqlLog($"CommitTran:执行事务失败：{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTran()
        {
            Db.Ado.RollbackTran();
        }

        /// <summary>
        /// 事务Action
        /// </summary>
        /// <param name="run"></param>
        /// <returns></returns>
        public Result WorkTransaction(Func<Result> run)
        {
            try
            {
                Ado.BeginTran();
                var result = run();
                if (result.IsSucc)
                {
                    Ado.CommitTran();
                    return new Result { IsSucc = true, Message = "事务完成！" };
                }
                else
                {
                    Ado.RollbackTran();
                }
            }
            catch (Exception ex)
            {
                Ado.RollbackTran();
                logHelper.SqlLog($"WorkTransaction:执行事务失败：{ex.Message}", ex);
            }

            return new Result { Message = "事务出错" };
        }

        /// <summary>
        /// 事务Func
        /// </summary>
        /// <param name="run"></param>
        /// <returns></returns>
        public Result RunTransaction(Action run)
        {
            Func<Result> func = () =>
            {
                run();
                return new Result { IsSucc = true };
            };

            return WorkTransaction(func);
        }

        /// <summary>
        /// 事务Action 异步
        /// </summary>
        /// <param name="run"></param>
        /// <returns></returns>
        public async Task<Result> WorkTransactionAsync(Func<Task<Result>> run)
        {
            try
            {
                Ado.BeginTran();
                var result = await run();
                if (result.IsSucc)
                {
                    Ado.CommitTran();
                    return new Result { IsSucc = true, Message = "事务完成！" };
                }
                else
                {
                    Ado.RollbackTran();
                }
            }
            catch (Exception ex)
            {
                Ado.RollbackTran();
                logHelper.SqlLog($"WorkTransaction:执行事务失败：{ex.Message}", ex);
            }

            return new Result { Message = "事务出错" };
        }

        #endregion
    }
}
