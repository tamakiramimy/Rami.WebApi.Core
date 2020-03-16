using SqlSugar;
using StackExchange.Profiling;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Rami.WebApi.Core.Framework
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    public class SugarDbContext
    {
        /// <summary>
        /// 日志(可改用Log4记录SQL执行日志)
        /// </summary>
        private static readonly ILogHelper<SugarDbContext> _loggerHelper;

        /// <summary>
        /// 构造方法
        /// </summary>
        static SugarDbContext()
        {
            _loggerHelper = StaticServiceProvider.Current.GetRequiredService<ILogHelper<SugarDbContext>>();
        }

        /// <summary>
        /// 数据连接对象 
        /// Blog.Core 
        /// </summary>
        public SqlSugarClient Db { get; private set; }

        /// <summary>
        /// 数据库上下文实例（自动关闭连接）
        /// Blog.Core 
        /// </summary>
        public static SugarDbContext Context
        {
            get
            {
                return new SugarDbContext();
            }
        }

        /// <summary>
        /// 数据库配置
        /// </summary>
        private string ConnConf { get; set; } = "AppSettings:DefDbConn";

        /// <summary>
        /// 链接字符串
        /// </summary>
        private static string ConnectionString { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        private static DbType DbType { get; set; }

        /// <summary>
        /// 功能描述:构造函数
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="isAutoClose">是否自动关闭连接</param>
        public SugarDbContext(bool isAutoClose = true)
        {
            InitDbContext(ConnConf, isAutoClose);
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="_connConf"></param>
        /// <param name="_isAutoClose"></param>
        public SugarDbContext(string _connConf, bool _isAutoClose)
        {
            InitDbContext(_connConf, _isAutoClose);
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="_conn"></param>
        /// <param name="_dbType"></param>
        /// <param name="_isAutoClose"></param>
        public SugarDbContext(string _conn, DbType _dbType, bool _isAutoClose = true)
        {
            InitDbContext(_conn, _dbType, _isAutoClose);
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="_connConf"></param>
        /// <param name="_isAutoClose"></param>
        private void InitDbContext(string _connConf, bool _isAutoClose)
        {
            var connConf = string.IsNullOrWhiteSpace(_connConf) ? _connConf : ConnConf;
            var conf = Appsettings.Section(_connConf);

            var dbType = (DbType)Enum.Parse(typeof(DbType), conf["DbType"]);
            InitDbContext(conf["ConnectionString"], dbType, _isAutoClose);
        }

        /// <summary>
        /// 初始化数据库上下文
        /// </summary>
        /// <param name="_conn"></param>
        /// <param name="_dbType"></param>
        /// <param name="_isAutoClose"></param>
        private void InitDbContext(string _conn, DbType _dbType, bool _isAutoClose)
        {
            ConnectionString = _conn;
            DbType = _dbType;
            InitDbContext(_isAutoClose);
        }

        /// <summary>
        /// 初始化数据库上下文
        /// </summary>
        /// <param name="isAutoClose"></param>
        private void InitDbContext(bool isAutoClose)
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new ArgumentNullException("数据库连接字符串为空");
            }

            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConnectionString,
                DbType = DbType,
                IsAutoCloseConnection = isAutoClose,
                IsShardSameThread = false,
                InitKeyType = InitKeyType.Attribute,
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    //DataInfoCacheService = new HttpRuntimeCache()
                },
                MoreSettings = new ConnMoreSettings()
                {
                    //IsWithNoLockQuery = true,
                    IsAutoRemoveDataCache = true
                },
            });

            if (ComHelper.GetConf("AppSettings:SqlAOP:Enabled").ObjToBool())
            {
                Db.Aop.OnLogExecuting = (sql, pars) => //SQL执行中事件
                {
                    Parallel.For(0, 1, e =>
                    {
                        var paras = GetParas(pars);
                        var msg = $"执行SQL：{paras}   【SQL语句】：{sql}";

                        MiniProfiler.Current.CustomTiming("SQL", msg);
                        //LogLock.OutSql2Log("SqlLog", new string[] { GetParas(pars), "【SQL语句】：" + sql });
                        _loggerHelper.SqlLog(msg);
                    });
                };
            }
        }

        /// <summary>
        /// 获取Sql参数列表
        /// </summary>
        /// <param name="pars"></param>
        /// <returns></returns>
        private string GetParas(SugarParameter[] pars)
        {
            string key = "【SQL参数】：";
            foreach (var param in pars)
            {
                key += $"{param.ParameterName}:{param.Value}\n";
            }

            return key;
        }

        #region 实例方法

        /// <summary>
        /// 功能描述:获取数据库处理对象
        /// 作　　者:Blog.Core
        /// </summary>
        /// <returns>返回值</returns>
        public SimpleClient GetEntityDB()
        {
            return new SimpleClient(Db);
        }

        /// <summary>
        /// 功能描述:获取数据库处理对象
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="db">db</param>
        /// <returns>返回值</returns>
        public SimpleClient GetEntityDB(SqlSugarClient db)
        {
            return new SimpleClient(db);
        }

        #region 根据数据库表生产实体类

        /// <summary>
        /// 功能描述:根据数据库表生产实体类
        /// 作　　者:Blog.Core
        /// </summary>       
        /// <param name="strPath">实体类存放路径</param>
        public void CreateClassFileByDBTalbe(string strPath)
        {
            CreateClassFileByDBTalbe(strPath, "Rami.WebApi.Core.Domain.Entity");
        }

        /// <summary>
        /// 功能描述:根据数据库表生产实体类
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="strPath">实体类存放路径</param>
        /// <param name="strNameSpace">命名空间</param>
        public void CreateClassFileByDBTalbe(string strPath, string strNameSpace)
        {
            CreateClassFileByDBTalbe(strPath, strNameSpace, null);
        }

        /// <summary>
        /// 功能描述:根据数据库表生产实体类
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="strPath">实体类存放路径</param>
        /// <param name="strNameSpace">命名空间</param>
        /// <param name="lstTableNames">生产指定的表</param>
        public void CreateClassFileByDBTalbe(
            string strPath,
            string strNameSpace,
            string[] lstTableNames)
        {
            CreateClassFileByDBTalbe(strPath, strNameSpace, lstTableNames, string.Empty);
        }

        /// <summary>
        /// 功能描述:根据数据库表生产实体类
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="strPath">实体类存放路径</param>
        /// <param name="strNameSpace">命名空间</param>
        /// <param name="lstTableNames">生产指定的表</param>
        /// <param name="strInterface">实现接口</param>
        /// <param name="blnSerializable">是否自动释放</param>
        public void CreateClassFileByDBTalbe(
          string strPath,
          string strNameSpace,
          string[] lstTableNames,
          string strInterface,
          bool blnSerializable = false)
        {
            if (lstTableNames != null && lstTableNames.Length > 0)
            {
                Db.DbFirst.Where(lstTableNames).IsCreateDefaultValue().IsCreateAttribute()
                    .SettingClassTemplate(p => p = @"
{using}

namespace {Namespace}
{
    {ClassDescription}{SugarTable}" + (blnSerializable ? "[Serializable]" : "") + @"
    public partial class {ClassName}" + (string.IsNullOrEmpty(strInterface) ? "" : (" : " + strInterface)) + @"
    {
        public {ClassName}()
        {
{Constructor}
        }
{PropertyName}
    }
}
")
                    .SettingPropertyTemplate(p => p = @"
            {SugarColumn}
            public {PropertyType} {PropertyName}
            {
                get
                {
                    return _{PropertyName};
                }
                set
                {
                    if(_{PropertyName}!=value)
                    {
                        base.SetValueCall(" + "\"{PropertyName}\",_{PropertyName}" + @");
                    }
                    _{PropertyName}=value;
                }
            }")
                    .SettingPropertyDescriptionTemplate(p => p = "          private {PropertyType} _{PropertyName};\r\n" + p)
                    .SettingConstructorTemplate(p => p = "              this._{PropertyName} ={DefaultValue};")
                    .CreateClassFile(strPath, strNameSpace);
            }
            else
            {
                Db.DbFirst.IsCreateAttribute().IsCreateDefaultValue()
                    .SettingClassTemplate(p => p = @"
{using}

namespace {Namespace}
{
    {ClassDescription}{SugarTable}" + (blnSerializable ? "[Serializable]" : "") + @"
    public partial class {ClassName}" + (string.IsNullOrEmpty(strInterface) ? "" : (" : " + strInterface)) + @"
    {
        public {ClassName}()
        {
{Constructor}
        }
{PropertyName}
    }
}
")
                    .SettingPropertyTemplate(p => p = @"
            {SugarColumn}
            public {PropertyType} {PropertyName}
            {
                get
                {
                    return _{PropertyName};
                }
                set
                {
                    if(_{PropertyName}!=value)
                    {
                        base.SetValueCall(" + "\"{PropertyName}\",_{PropertyName}" + @");
                    }
                    _{PropertyName}=value;
                }
            }")
                    .SettingPropertyDescriptionTemplate(p => p = "          private {PropertyType} _{PropertyName};\r\n" + p)
                    .SettingConstructorTemplate(p => p = "              this._{PropertyName} ={DefaultValue};")
                    .CreateClassFile(strPath, strNameSpace);
            }
        }

        #endregion

        #region 根据实体类生成数据库表

        /// <summary>
        /// 功能描述:根据实体类生成数据库表
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="blnBackupTable">是否备份表</param>
        /// <param name="lstEntitys">指定的实体</param>
        public void CreateTableByEntity<T>(bool blnBackupTable, params T[] lstEntitys) where T : class, new()
        {
            Type[] lstTypes = null;
            if (lstEntitys != null)
            {
                lstTypes = new Type[lstEntitys.Length];
                for (int i = 0; i < lstEntitys.Length; i++)
                {
                    T t = lstEntitys[i];
                    lstTypes[i] = typeof(T);
                }
            }

            CreateTableByEntity(blnBackupTable, lstTypes);
        }

        /// <summary>
        /// 功能描述:根据实体类生成数据库表
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="blnBackupTable">是否备份表</param>
        /// <param name="lstEntitys">指定的实体</param>
        public void CreateTableByEntity(bool blnBackupTable, params Type[] lstEntitys)
        {
            if (blnBackupTable)
            {
                Db.CodeFirst.BackupTable().InitTables(lstEntitys); //change entity backupTable            
            }
            else
            {
                Db.CodeFirst.InitTables(lstEntitys);
            }
        }

        #endregion

        #endregion

        #region 静态方法

        /// <summary>
        /// 功能描述:获得一个DbContext
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="isAutoClose">是否自动关闭连接（如果为false，则使用接受时需要手动关闭Db）</param>
        /// <returns>返回值</returns>
        public static SugarDbContext GetDbContext(bool isAutoClose = true)
        {
            return new SugarDbContext(isAutoClose);
        }

        /// <summary>
        /// 功能描述:设置初始化参数
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="connConf">数据库链接配置路径字符串</param>
        /// <param name="isAutoClose">是否自动关闭连接</param>
        public static SugarDbContext GetDbContextByConnPath(string connConf, bool isAutoClose = true)
        {
            return new SugarDbContext(connConf, isAutoClose);
        }

        /// <summary>
        /// 功能描述:设置初始化参数
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="conn">连接字符串</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="isAutoClose">是否自动关闭连接</param>
        public static SugarDbContext GetDbContextByConnDbType(string conn, DbType dbType = DbType.SqlServer, bool isAutoClose = true)
        {
            return new SugarDbContext(conn, dbType, isAutoClose);
        }

        /// <summary>
        /// 功能描述:创建一个链接配置
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="blnIsAutoCloseConnection">是否自动关闭连接</param>
        /// <param name="blnIsShardSameThread">是否夸类事务</param>
        /// <returns>ConnectionConfig</returns>
        public static ConnectionConfig GetConnectionConfig(bool blnIsAutoCloseConnection = true, bool blnIsShardSameThread = false)
        {
            ConnectionConfig config = new ConnectionConfig()
            {
                ConnectionString = ConnectionString,
                DbType = DbType,
                IsAutoCloseConnection = blnIsAutoCloseConnection,
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    //DataInfoCacheService = new HttpRuntimeCache()
                },
                IsShardSameThread = blnIsShardSameThread
            };

            return config;
        }

        /// <summary>
        /// 功能描述:获取一个自定义的DB
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="config">config</param>
        /// <returns>返回值</returns>
        public static SqlSugarClient GetCustomDB(ConnectionConfig config)
        {
            return new SqlSugarClient(config);
        }

        /// <summary>
        /// 功能描述:获取一个自定义的数据库处理对象
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="sugarClient">sugarClient</param>
        /// <returns>返回值</returns>
        public static SimpleClient GetCustomEntityDB(SqlSugarClient sugarClient)
        {
            return new SimpleClient(sugarClient);
        }

        /// <summary>
        /// 功能描述:获取一个自定义的数据库处理对象
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="config">config</param>
        /// <returns>返回值</returns>
        public static SimpleClient GetCustomEntityDB(ConnectionConfig config)
        {
            SqlSugarClient sugarClient = GetCustomDB(config);
            return GetCustomEntityDB(sugarClient);
        }

        #endregion
    }

    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DataBaseType
    {
        /// <summary>
        /// MySql
        /// </summary>
        MySql = 0,

        /// <summary>
        /// SqlServer
        /// </summary>
        SqlServer = 1,

        /// <summary>
        /// Sqlite
        /// </summary>
        Sqlite = 2,

        /// <summary>
        /// Oracle
        /// </summary>
        Oracle = 3,

        /// <summary>
        /// PostgreSQL
        /// </summary>
        PostgreSQL = 4
    }
}
