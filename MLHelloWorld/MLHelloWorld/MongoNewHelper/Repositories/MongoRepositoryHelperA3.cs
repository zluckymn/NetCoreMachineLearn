using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Bson;
using MZ.MongoProvider;
using System.Linq.Expressions;
using MZ.Metadata;
using System.Threading.Tasks;

namespace MZ.MongoProvider.Core
{
    public class MongoRepositoryHelperA3 : IDisposable
    {
        #region 私有字段
       
        private IMongoDatabase _mongoDatabase = null;
        private MongoClient client =null;
        private const string AppSettingDBConnectionKey = "MongoDBConnectionString";
        public   List<Task> taskList = new List<Task>();
        #endregion

        #region 构造函数
        /// <summary>
        /// 默认构造函数,读取配置项中的数据库
        /// </summary>
        public MongoRepositoryHelperA3()
        {
            
            string connStr = "mongodb://sa:dba@localhost/A3";       //默认数据库连接串

            if (MongoConfig.DefaultInstance.MongConnectionString!= "")      //读取数据库连接串
            {
                connStr = MongoConfig.DefaultInstance.MongConnectionString;       //默认数据库连接串
            }

            ConnectionDataBase(connStr);        //连接数据库
        }

        /// <summary>
        /// 构造函数,串入数据库连接串,进行数据库连接
        /// </summary>
        /// <param name="connectionString"></param>
        public MongoRepositoryHelperA3(string connStr)
        {
            if (string.IsNullOrEmpty(connStr)) throw new Exception("数据库连接串为空!!");

            ConnectionDataBase(connStr);    //连接数据库
        }

       

        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public bool ConnectionDataBase(string connStr)
        {
            bool isSuccess = false;

            try
            {
                if (!connStr.Contains("connectTimeoutMS"))
                {
                    if (connStr.Contains("?"))
                    {
                        connStr += "&connectTimeoutMS=180000";
                    }
                    else
                    {
                        connStr += "?connectTimeoutMS=180000";
                    }
                }
                var mongoUrl = new MongoUrl(connStr);
                client = new MongoClient(mongoUrl);
                _mongoDatabase =client.GetDatabase(mongoUrl.DatabaseName);
            }
            catch(Exception ex)
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public bool ConnectionDataBase(string connStr,string dataBaseName)
        {
            bool isSuccess = false;

            try
            {
                var mongoUrl = new MongoUrl(connStr);
                client = new MongoClient(connStr);
                _mongoDatabase = client.GetDatabase(dataBaseName);
            }
            catch
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        /// <summary>
        /// 切换使用的数据库
        /// </summary>
        /// <param name="dbName">切换的数据库名称</param>
        /// <returns></returns>
        public bool UseDataBase(string dbName)
        {
            bool isSuccess = true;

            try
            {
                _mongoDatabase = client.GetDatabase(dbName);      //切换数据库
            }
            catch
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        /// <summary>
        /// 获取当前数据库
        /// </summary>
        /// <returns></returns>
        public IMongoDatabase GetDataBase()
        {
            return this._mongoDatabase;
        }

        /// <summary>
        /// 获取所需集合
        /// </summary>
        /// <param name="collName"></param>
        /// <returns></returns>
        public IMongoCollection<BsonDocument> GetCollection(string collName)
        {
            return this._mongoDatabase.GetCollection<BsonDocument>(collName);
             
        }

        #endregion

        #region 查询方法
        /// <summary>
        /// 查找集合中的默认第一条记录
        /// </summary>
        /// <param name="collName"></param>
        /// <returns></returns>
        public BsonDocument FindOne(string collName)
        {
            BsonDocument entity = this.GetCollection(collName).Find(_=>true).FirstOrDefault();

            //if (entity == null) entity = new BsonDocument();

            return entity;
        }

        /// <summary>
        /// 根据搜索条件查找一条记录
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public BsonDocument FindOne(string collName, FilterDefinition<BsonDocument> query)
        {
            BsonDocument entity = this.GetCollection(collName).Find(query).FirstOrDefault();

            //if (entity == null) entity = new BsonDocument();

            return entity;
        }
      
      
        /// <summary>
        /// 根据搜索条件查找一条记录
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public BsonDocument FindOne(string collName, FilterDefinition<BsonDocument> query, string[] fields)
        {
             BsonDocument entity = this.GetCollection(collName).Find(query).SetFields(fields).FirstOrDefault();
           return entity;
         }

        /// <summary>
        /// 查找集合中所有记录
        /// </summary>
        /// <param name="collName"></param>
        /// <returns></returns>
        public IFindFluent<BsonDocument, BsonDocument> FindAll(string collName)
        {
            var entityCursor = this.GetCollection(collName).Find(_ => true);
            return entityCursor;
        }

        /// <summary>
        /// 查找集合中所有记录
        /// </summary>
        /// <param name="collName"></param>
        /// <returns></returns>
        public IFindFluent<BsonDocument, BsonDocument> FindAll(string collName, string[] feilds)
        {
           var entityCursor = this.GetCollection(collName).Find(_=>true).SetFields(feilds);
            return entityCursor;
        }
       
        /// <summary>
        /// 查找集合中所有记录
        /// </summary>
        /// <param name="collName"></param>
        /// <returns></returns>
        public IFindFluent<BsonDocument, BsonDocument> FindPaging(string collName, int pageSize, int page)
        {
            int skipNum = (page > 0 ? page - 1 : 0) * pageSize;

            IFindFluent<BsonDocument, BsonDocument> entityCursor = this.GetCollection(collName).Find(_=>true).Skip(skipNum).Limit(pageSize);

            return entityCursor;
        }

        /// <summary>
        /// 分页查找记录
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="pageSize"></param>
        /// <param name="page"></param>
        /// <param name="selectFeilds"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public IFindFluent<BsonDocument, BsonDocument> FindPaging(string collName, int pageSize, int page, string[] selectFeilds)
        {
            int skipNum = (page > 0 ? page - 1 : 0) * pageSize;

            IFindFluent<BsonDocument, BsonDocument> entityCursor = this.GetCollection(collName).Find(_=>true).Skip(skipNum).Limit(pageSize).SetFields(selectFeilds);

            return entityCursor;
        }

        /// <summary>
        /// 分页查找记录
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="pageSize"></param>
        /// <param name="page"></param>
        /// <param name="selectFeilds"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public IFindFluent<BsonDocument, BsonDocument> FindPaging(string collName, int pageSize, int page, SortByDocument sort)
        {
            int skipNum = (page > 0 ? page - 1 : 0) * pageSize;

            IFindFluent<BsonDocument, BsonDocument> entityCursor = this.GetCollection(collName).Find(_=>true).Skip(skipNum).Limit(pageSize).SetSortOrder(sort);

            return entityCursor;
        }
        /// <summary>
        /// 分页查找记录
        /// </summary>
        /// <param name="collName"></param>
        /// <returns></returns>
        public IFindFluent<BsonDocument, BsonDocument> FindPaging(string collName, FilterDefinition<BsonDocument> query, int pageSize, int page, string[] selectFeilds)
        {
            int skipNum = (page > 0 ? page - 1 : 0) * pageSize;

            IFindFluent<BsonDocument, BsonDocument> entityCursor = this.GetCollection(collName).Find(query).Skip(skipNum).Limit(pageSize).SetFields(selectFeilds);

            return entityCursor;
        }


        /// <summary>
        /// 分页查找记录
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="query"></param>
        /// <param name="pageSize"></param>
        /// <param name="page"></param>
        /// <param name="selectFeilds"></param>
        /// <param name="sort">{"name":-1,"age":1} 1 升序 -1降序</param>
        /// <returns></returns>
        public IFindFluent<BsonDocument, BsonDocument> FindPaging(string collName, FilterDefinition<BsonDocument> query, int pageSize, int page, string[] selectFeilds, SortByDocument sort)
        {
            int skipNum = (page > 0 ? page - 1 : 0) * pageSize;

            IFindFluent<BsonDocument, BsonDocument> entityCursor = this.GetCollection(collName).Find(query).Skip(skipNum).Limit(pageSize).SetSortOrder(sort).SetFields(selectFeilds);

            return entityCursor;
        }

        /// <summary>
        /// 分页查找记录
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="query"></param>
        /// <param name="pageSize"></param>
        /// <param name="page"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public IFindFluent<BsonDocument, BsonDocument> FindPaging(string collName, FilterDefinition<BsonDocument> query, int pageSize, int page, SortByDocument sort)
        {
            int skipNum = (page > 0 ? page - 1 : 0) * pageSize;

            IFindFluent<BsonDocument, BsonDocument> entityCursor = this.GetCollection(collName).Find(query).Skip(skipNum).Limit(pageSize).SetSortOrder(sort);

            return entityCursor;
        }

        /// <summary>
        /// 查找集合中所有记录
        /// </summary>
        /// <param name="collName"></param>
        /// <returns></returns>
        public IFindFluent<BsonDocument, BsonDocument> FindPaging(string collName, FilterDefinition<BsonDocument> query, int pageSize, int page)
        {
            int skipNum = (page > 0 ? page - 1 : 0) * pageSize;

            IFindFluent<BsonDocument, BsonDocument> entityCursor = this.GetCollection(collName).Find(query).Skip(skipNum).Limit(pageSize);

            return entityCursor;
        }

        /// <summary>
        /// 查找集合中所有记录
        /// </summary>
        /// <param name="collName"></param>
        /// <returns></returns>
        public IFindFluent<BsonDocument, BsonDocument> FindPaging(string collName, FilterDefinition<BsonDocument> query, string[] selectFeilds)
        {
            IFindFluent<BsonDocument, BsonDocument> entityCursor = this.GetCollection(collName).Find(query).SetFields(selectFeilds);

            return entityCursor;
        }

        /// <summary>
        /// 根据搜索条件查找多条记录
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public IFindFluent<BsonDocument, BsonDocument> FindByQuery(string collName, FilterDefinition<BsonDocument> query)
        {
            if (query == null)
            {
                query = Query.EQ("_id", "-1");
            }
            IFindFluent<BsonDocument, BsonDocument> entityCursor = this.GetCollection(collName).Find(query);

            return entityCursor;
        }
        /// <summary>
        /// 根据搜索条件查找多条记录
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public IFindFluent<BsonDocument, BsonDocument> FindByQuery(string collName, FilterDefinition<BsonDocument> query, string[] selectFeilds)
        {
            if ( query == null)
            {
                query = Query.EQ("_id", "-1");
            }
            IFindFluent<BsonDocument, BsonDocument> entityCursor = this.GetCollection(collName).Find(query).SetFields(selectFeilds);

            return entityCursor;
        }
        /// <summary>
        /// 根据搜索条件查找多条记录
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public IFindFluent<BsonDocument, BsonDocument> FindByQuery(string collName, FilterDefinition<BsonDocument> query, int pageSize, int page, string[] selectFeilds)
        {
            if ( query == null)
            {
                query = Query.EQ("_id", "-1");
            }

            int skipNum = (page > 0 ? page - 1 : 0) * pageSize;

            IFindFluent<BsonDocument, BsonDocument> entityCursor = this.GetCollection(collName).Find(query).Skip(skipNum).Limit(pageSize).SetFields(selectFeilds);

            return entityCursor;
        }


        /// <summary>
        /// 根据搜索条件查找多条记录
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public IFindFluent<BsonDocument, BsonDocument> FindAll(string collName, FilterDefinition<BsonDocument> query)
        {
            IFindFluent<BsonDocument, BsonDocument> entityCursor = this.GetCollection(collName).Find(query);

            return entityCursor;
        }

        /// <summary>
        /// 返回查询表的总个数
        /// </summary>
        /// <param name="collName"></param>
        /// <returns></returns>
        public long FindCount(string collName)
        {
            long count = this.GetCollection(collName).Count(_=>true);

            //if (entity == null) entity = new BsonDocument();

            return count;
        }
        /// <summary>
        /// 返回查询表的总个数
        /// </summary>
        /// <param name="collName"></param>
        /// <returns></returns>
        public long FindCount(string collName, FilterDefinition<BsonDocument> query)
        {
            long count = 0;
             count = this.GetCollection(collName).Count(query);
            //if (entity == null) entity = new BsonDocument();

            return count;
        }
        /// <summary>
        /// 根据搜索条件查找
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="query"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public IFindFluent<BsonDocument, BsonDocument> Find(string collName, FilterDefinition<BsonDocument> query, SortByDocument sortBy, int skip, int limit)
        {
            IFindFluent<BsonDocument, BsonDocument> entityCursor;
            if (query != null)
            {
                entityCursor = this.GetCollection(collName).Find(query).SetSortOrder(sortBy).Skip(skip).Limit(limit);
            }
            else
            {
                entityCursor = this.GetCollection(collName).Find(_=>true).SetSortOrder(sortBy).Skip(skip).Limit(limit); ;
            }

            return entityCursor;
        }


        /// <summary>
        /// 执行原生查询
        /// </summary>
        /// <param name="queryStr"></param>
        /// <returns></returns>
        public BsonValue EvalNativeQuery(string queryStr)
        {
            BsonValue resultVal = null;
            //BsonJavaScript queryScript = new BsonJavaScript(queryStr);
            try
            {
                resultVal = _mongoDatabase.RunCommand<BsonDocument>(queryStr);
            }
            catch
            {
                resultVal = null;
            }

            return resultVal;
        }

        #endregion

        #region 操作方法
        /// <summary>
        /// 插入单条数据
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="saveDoc"></param>
        /// <returns></returns>
        public InvokeResult Save(string collName, BsonDocument saveDoc)
        {
            InvokeResult result = new InvokeResult() { Status = Status.Failed };

            try
            {
                var entityColl = GetCollection(collName);
                entityColl.InsertOne(saveDoc);
                result.Status = Status.Successful;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
        public InvokeResult SaveAsync(string collName, BsonDocument saveDoc)
        {
            InvokeResult result = new InvokeResult() { Status = Status.Failed };

            try
            {
                var entityColl = GetCollection(collName);
                var task=entityColl.InsertOneAsync(saveDoc);
                taskList.Add(task);
                result.Status = Status.Successful;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="saveDocList"></param>
        /// <returns></returns>
        public InvokeResult Save(string collName, List<BsonDocument> saveDocList)
        {
            InvokeResult result = new InvokeResult() { Status = Status.Failed };

            try
            {
                 var _collection = GetCollection(collName);
                 _collection.InsertMany(saveDocList);
                 result.Status = Status.Successful;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="saveDocList"></param>
        /// <returns></returns>
        public InvokeResult SaveAsync(string collName, List<BsonDocument> saveDocList)
        {
            InvokeResult result = new InvokeResult() { Status = Status.Failed };

            try
            {
                var _collection = GetCollection(collName);
                var task=_collection.InsertManyAsync(saveDocList);
                taskList.Add(task);
                result.Status = Status.Successful;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
        private UpdateDefinition<BsonDocument> BuildUpdateDoc(BsonDocument saveDoc)
        {
             var update = Builders<BsonDocument>.Update;
            var updates = new List<UpdateDefinition<BsonDocument>>();
            foreach (var elem in saveDoc)
            {
                updates.Add(update.Set(elem.Name, saveDoc[elem.Name]));
            }
            return update.Combine(updates);
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="searchDoc">可为空,为空为插入</param>
        /// <param name="saveDoc"></param>
        /// <returns></returns>
        public InvokeResult Save(string collName, FilterDefinition<BsonDocument> query, BsonDocument saveDoc)
        {
            InvokeResult result = new InvokeResult() { Status = Status.Failed };

            try
            {

               
                if (FindCount(collName, query) < 0)
                {
                    Save(collName, saveDoc);
                }
                else
                {
                    var entityColl = GetCollection(collName);
                    entityColl.UpdateMany(query, BuildUpdateDoc(saveDoc));
                }
                result.Status = Status.Successful;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="searchDoc">可为空,为空为插入</param>
        /// <param name="saveDoc"></param>
        /// <returns></returns>
        public InvokeResult SaveAsync(string collName, FilterDefinition<BsonDocument> query, BsonDocument saveDoc)
        {
            InvokeResult result = new InvokeResult() { Status = Status.Failed };

            try
            {


                if (FindCount(collName, query) < 0)
                {
                    SaveAsync(collName, saveDoc);
                }
                else
                {
                    var entityColl = GetCollection(collName);
                    var task= entityColl.UpdateManyAsync(query, BuildUpdateDoc(saveDoc));
                    taskList.Add(task);
                }
                result.Status = Status.Successful;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }



        /// <summary>
        /// 更新文档
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="query"></param>
        /// <param name="updateDoc"></param>
        /// <returns></returns>
        public InvokeResult Update(string collName, FilterDefinition<BsonDocument> query, BsonDocument updateDoc)
        {
            InvokeResult result = new InvokeResult() { Status = Status.Failed };
            try
            {
                 var _collection = GetCollection(collName);
                _collection.UpdateMany(query, BuildUpdateDoc(updateDoc), new UpdateOptions()
                {
                    IsUpsert = true,
                    BypassDocumentValidation = true,
                });

                result.Status = Status.Successful;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
        /// <summary>
        /// 更新文档
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="query"></param>
        /// <param name="updateDoc"></param>
        /// <returns></returns>
        public InvokeResult UpdateAsync(string collName, FilterDefinition<BsonDocument> query, BsonDocument updateDoc)
        {
            InvokeResult result = new InvokeResult() { Status = Status.Failed };
            try
            {
                var _collection = GetCollection(collName);
               var task= _collection.UpdateManyAsync(query, BuildUpdateDoc(updateDoc), new UpdateOptions()
                {
                    IsUpsert = true,
                    BypassDocumentValidation = true,
                });
                taskList.Add(task);
                result.Status = Status.Successful;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }


        /// <summary>
        /// 更新或插入（记录不存在则插入）
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="query"></param>
        /// <param name="updateDoc"></param>
        /// <returns></returns>
        public InvokeResult UpdateOrInsert(string collName, FilterDefinition<BsonDocument> query, BsonDocument updateDoc)
        {
            InvokeResult result = new InvokeResult() { Status = Status.Failed };
            try
            {
                var entityColl = GetCollection(collName);
                entityColl.FindOneAndUpdate(query, BuildUpdateDoc(updateDoc));

                result.Status = Status.Successful;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 更新或插入（记录不存在则插入）
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="query"></param>
        /// <param name="updateDoc"></param>
        /// <returns></returns>
        public InvokeResult UpdateOrInsertAsync(string collName, FilterDefinition<BsonDocument> query, BsonDocument updateDoc)
        {
            InvokeResult result = new InvokeResult() { Status = Status.Failed };
            try
            {
                var entityColl = GetCollection(collName);
                var task=entityColl.FindOneAndUpdateAsync(query, BuildUpdateDoc(updateDoc));
                taskList.Add(task);
                result.Status = Status.Successful;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }


        /// <summary>
        /// 删除文档字段
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="queryString"></param>
        /// <param name="feilds"></param>
        /// <returns></returns>
        public InvokeResult DeleteFeilds(string collName, FilterDefinition<BsonDocument> query, string[] feilds)
        {
         
                BsonDocument updateDoc = new BsonDocument();
                foreach (var feild in feilds)
                {
                    updateDoc.Add(feild,null);
                }
            return Update(collName, query, updateDoc);
        }
        /// <summary>
        /// 删除文档字段
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="queryString"></param>
        /// <param name="feilds"></param>
        /// <returns></returns>
        public InvokeResult DeleteFeildsAsync(string collName, FilterDefinition<BsonDocument> query, string[] feilds)
        {

            BsonDocument updateDoc = new BsonDocument();
            foreach (var feild in feilds)
            {
                updateDoc.Add(feild, null);
            }
            return UpdateAsync(collName, query, updateDoc);
        }
        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public InvokeResult Delete(string collName, FilterDefinition<BsonDocument> query)
        {
            InvokeResult result = new InvokeResult() { Status = Status.Failed };

            try
            {
                 var entityColl = GetCollection(collName);

                entityColl.DeleteMany(query);

                result.Status = Status.Successful;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="collName"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public InvokeResult DeleteAsync(string collName, FilterDefinition<BsonDocument> query)
        {
            InvokeResult result = new InvokeResult() { Status = Status.Failed };

            try
            {
                var entityColl = GetCollection(collName);

               var task= entityColl.DeleteManyAsync(query);
                taskList.Add(task);
                result.Status = Status.Successful;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            if (this._mongoDatabase != null)
            {
                this._mongoDatabase = null;
            }
            if (this.client != null)
            {
                this.client = null;
            }
        }

        #endregion
    }
}
