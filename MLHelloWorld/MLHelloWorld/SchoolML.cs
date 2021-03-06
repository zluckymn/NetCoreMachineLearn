﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.ML.Models;
using MZ.MongoProvider.Core;
using MZ.MongoProvider;
using MongoDB.Driver;
using MZ.Extension;
using System.Linq;
using MongoDB.Bson;
using MZ.Enum;
using MZ.Metadata;
using Microsoft.ML.Data;

namespace MLHelloWorld
{
    /// <summary>
    /// 学校人数预测,根据班级，学校类型占地面积预测学生个数据
    /// </summary>
    public class SchoolML
    {
        const string DataPath = @".\Data\buildingArea_School-train.csv";//训练数据
        const string TestDataPath = @".\Data\buildingArea_School-test.csv";//测试数据
        const string ModelPath = @".\Models\school_predic_Model_buildingArea_ClasCount.zip";//模型地址,
        const string ModelDirectory = @".\Models";//模型目录
        
        public class School
        {

            /// <summary>
            /// 评分
            /// </summary>
            [Column(ordinal: "0")]
            public float typeNameIndex;
            ///// <summary>
            ///// 城市guid
            ///// </summary>
            //[Column(ordinal: "1")]
            //public string cityGuid;

            /// <summary>
            /// 班级个数
            /// </summary>
            [Column(ordinal: "0")]
            public float classCount;
            /// <summary>
            /// 教师人数
            /// </summary>
            [Column(ordinal: "1")]
            public float teacher;
            /// <summary>
            /// 学生个数
            /// </summary>
            [Column(ordinal: "2")]
            public float student;
            ///// <summary>
            ///// 建筑面积
            ///// </summary>
            //[Column(ordinal: "5")]
            //public float buildingArea;

        }
        /// <summary>
        ///预测类
        /// </summary>
        public class SchoolStudentPrediction
        {
            [ColumnName("Score")]
            public float student;
           
        }

      
        /// <summary>
        /// 训练并生成模型
        /// </summary>
        /// <returns></returns>
        public static async Task<PredictionModel<School, SchoolStudentPrediction>> Train(IEnumerable<School> trainData,string modelFileName,string labelColumn,string[] oneHotColumns,string[]features)
        { 
            //创建学习管道
            var pipeline = new LearningPipeline();
            //加载和转换您的数据
            //var textLoader = new TextLoader<School>(DataPath, useHeader: true, separator: ",");

            //pipeline.Add(textLoader);
             pipeline.Add(CollectionDataSource.Create(trainData));
            //使用该ColumnCopier()功能将“票价_帐户”列复制到名为“标签”的新列中。此列是标签。
             pipeline.Add(new ColumnCopier((labelColumn, "Label")));
            //一个对象叫ColumnDropper，可以用来在训练开始前舍弃掉不需要的字段，比如id，对结果没有任何影响，因此可以去掉
             //pipeline.Add(new ColumnDropper() { Column = new[] { "typeNameIndex", "cityGuid", "buildingArea" } });
            //进行一些特征工程来转换数据，以便它可以有效地用于机器学习。该训练模型需要算法的数字功能，
            //您变换中的分类数据（vendor_id，rate_code，和payment_type）为数字。
            //该CategoricalOneHotVectorizer()
            //函数为每个列中的值分配一个数字键。通过添加以下代码来转换您的数据：
            if (oneHotColumns.Count() > 0)
            {
                pipeline.Add(new CategoricalOneHotVectorizer(oneHotColumns));
            }
            //数据准备的最后一步是使用该功能将所有功能组合到一个向量中ColumnConcatenator()。这一必要步骤
            //有助于算法轻松处理您的功能。按照您在最后一步中编写的内容添加以下代码：
            //请注意，“trip_time_in_secs”列不包括在内。你已经确定它不是一个有用的预测功能。
            pipeline.Add(new ColumnConcatenator("Features",
                                    features
                                     ));
            //在将数据添加到流水线并将其转换为正确的输入格式之后，您可以选择一种学习算法（学习者）。学习算
            //法训练模型。你为这个问题选择了一个回归任务，所以你增加了一个学习者调用FastTreeRegressor()到
            //使用梯度提升的管道。
            //渐变增强是回归问题的机器学习技术。它以逐步的方式构建每个回归树。它使用预定义的损失函数来测
            //量每个步骤中的错误，并在下一步中对其进行修正。结果是预测模型实际上是较弱预测模型的集合。
              pipeline.Add(new FastTreeRegressor());
             //泊松回归
             //pipeline.Add(new PoissonRegressor());
            //训练模型
            //最后一步是训练模型。在此之前，管道中没有任何东西被执行。该pipeline.Train<T_Input, T_Output>()
            //函数接受预定义的School类类型并输出一个SchoolStudentPrediction类型。将这最后一段代码添加到Train()
            //函数中：
            PredictionModel<School, SchoolStudentPrediction> model = pipeline.Train<School, SchoolStudentPrediction>();
            //改性Train（）方法为异步方法public static async Task<PredictionModel<School, SchoolStudentPrediction>> Train()
            ///通过生么预测什么
            if(!string.IsNullOrEmpty(modelFileName))
            await model.WriteAsync(modelFileName);
            
            return model;

        }
        /// <summary>
        /// 评估模型
        /// </summary>
        /// <param name="model"></param>
        public static void Evaluate(IEnumerable<School> testData,PredictionModel<School, SchoolStudentPrediction> model)
        {
            //var testData = new TextLoader<School>(TestDataPath, useHeader: true, separator: ",");
            var test=CollectionDataSource.Create(testData);
            var evaluator = new RegressionEvaluator();
            RegressionMetrics metrics = evaluator.Evaluate(model, test);
            // Rms should be around 2.795276
            //RMS是评估回归问题的一个指标。它越低，你的模型就越好。将以下代码添加到该Evaluate()函数中以打印模型的RMS。
            Console.WriteLine("Rms=" + metrics.Rms);
            Console.WriteLine("LossFn=" + metrics.LossFn);
            //Squared是评估回归问题的另一个指标。RSquared将是介于0和1之间的值。越接近1，模型越好。将下面的代码添加到该Evaluate()函数中以打印模型的RSquared值。
            Console.WriteLine("RSquared = " + metrics.RSquared);
        }
        static class TestTrips
        {
            internal static readonly School Trip1 = new School
            {
                 typeNameIndex = 0,
                classCount =79 ,//12
                teacher = 300,//203
                student = 3500,//
               // cityGuid= "2d6e1617-78a6-459b-8ddf-d3a427a3a70f",
               // buildingArea = 53489

            };
        }
        public async Task start()
        {
            bool alwayCreateModel = true;//是否建立模型文档
            string labelColumn = "student";//需要计算的字段 student
            string[] oneHotColumns = new string[] {  };//oneHot编码字段
            string[] features = new string[] { "teacher", "classCount" };//特征列, "classCount" "student"
            var curTypeNameIndex =4;//幼儿园 1小学 4中学 5大学 6驾校 7教育其他
            var tableName = "go007_School";
            //条件筛选项
            var studentExistQuery = Query.And(Query.Exists("student_new", true), Query.NE("student_new", ""));
            var teachExistQuery = Query.And(Query.Exists("teacher", true), Query.NE("teacher", ""));
            var classExistQuery = Query.And(Query.Exists("classCount_new", true), Query.NE("classCount_new", ""));
            var studentNoExistQuery =Query.Or(Query.Exists("student_new", false), Query.EQ("student_new", ""));
            var teachNoExistQuery = Query.Or(Query.Exists("teacher", false), Query.EQ("teacher", ""));
            var classNoExistQuery = Query.Or(Query.Exists("classCount_new", false), Query.EQ("classCount_new", ""));
            var conStr = GetConnectionStr("192.168.1.121", "SimpleCrawler");
            var dataop = new MongoRepositoryHelperA3(conStr);
            //训练评估查询语句
            var traningQuery = Query.And(Query.EQ("typeNameIndex", curTypeNameIndex),teachExistQuery, studentExistQuery, classExistQuery);
            //预测语句
            var predicQuery = Query.And(Query.EQ("typeNameIndex", curTypeNameIndex), teachExistQuery, classExistQuery, studentNoExistQuery);

            var allExistSchoolDocList = dataop.FindByQuery(tableName, traningQuery).SetFields("id","typeNameIndex", "teacher","classCount_new", "cityGuid", "student_new", "buildingArea").OrderBy(c=>c["id"]).ToList();
            var allCount = allExistSchoolDocList.Count();
            int traningCount = (int)(allCount * 0.7);//评估的数据源个数
            var traningSchoolList = new List<School>();//训练数据
            var evaluateSchoolList = new List<School>();//评估数据
            foreach (var traningSchool in allExistSchoolDocList)
            {
                var curSchool = new School()
                {
                   // buildingArea = traningSchool.Float("buildingArea"),
                   // cityGuid = traningSchool.Text("cityGuid"),
                    classCount = traningSchool.Float("classCount_new"),
                    student = traningSchool.Float("student_new"),
                    teacher = traningSchool.Float("teacher"),
                    typeNameIndex = traningSchool.Float("typeNameIndex")
                };
                if (traningSchoolList.Count <= traningCount)
                {
                    traningSchoolList.Add(curSchool);
                }
                else
                {
                    evaluateSchoolList.Add(curSchool);
                }
            }

            PredictionModel<School, SchoolStudentPrediction> model= null;
            ///模型地址
            var modelFileName = $"{ModelDirectory}/{string.Join("_", features)}-{labelColumn}-{traningSchoolList.Count()}.zip";
            if (!alwayCreateModel&&File.Exists(modelFileName))
            {
                 model = await PredictionModel.ReadAsync<School, SchoolStudentPrediction>(modelFileName);
               // await在你的Main方法中添加一个方法意味着该Main方法必须具有async修饰符并返回a Task：
               // using System.Threading.Tasks;
               // 评估模型
            }
            if (model == null) {
                //PredictionModel<School, SchoolStudentPrediction> model = TrainAsync();
                //改变的返回类型Train方法意味着你必须一个补充await，以调用codde Train在Method下面的代码如下所示：
                model = await Train(traningSchoolList, modelFileName, labelColumn, oneHotColumns, features);
            }
            Evaluate(evaluateSchoolList,model);

            var prediction1 = model.Predict(TestTrips.Trip1);
            Console.WriteLine($"classCount:{TestTrips.Trip1.classCount}teacher:{TestTrips.Trip1.teacher}  Predicted fare: {(int)prediction1.student}  fare: {TestTrips.Trip1.student}");
            Console.WriteLine("确认模型后按回车开始");
            Console.ReadKey();
            // var query = Query.And(Query.Exists("student", true), Query.Exists("teacher", true), Query.Exists("classCount", false));
            
            var allSchoolList = dataop.FindByQuery(tableName, predicQuery).SetFields("student_new","teacher", "typeNameIndex", "classCount_new", "cityGuid", "predicColumns", "predicColumns", "buildingArea").ToList();


            foreach (var school in allSchoolList)
            {
                /// 比如没有班级字段通过模型的学生和老师个数来预测班级数
                var typeNameIndex = school.Int("typeNameIndex");
                var student= school.Int("student_new");
                var teacher = school.Int("teacher");
                var classCount = school.Int("classCount_new");
                var predicBaseColumns = school.Contains("predicBaseColumns") ? school["predicBaseColumns"] as BsonArray ?? new BsonArray(): new BsonArray();
                var predicColumns = school.Contains("predicColumns") ? school["predicColumns"] as BsonArray ?? new BsonArray() : new BsonArray();
                School Trip1 = new School
                {
                    typeNameIndex = typeNameIndex,
                    classCount = classCount,
                    teacher = teacher,
                    student = student,
                    //cityGuid = school.Text("cityGuid")
                };
                var prediction = model.Predict(Trip1);
                var doc = new BsonDocument();
                if (prediction.student != 0)
                {
                    doc.Add($"{labelColumn}_new", (int)prediction.student);
                    #region 操作日志字段
                    foreach (var feature in features)
                    {
                        //基于字段
                        if (!predicBaseColumns.Contains(feature))
                        {
                            predicBaseColumns.Add(feature);
                        }
                    }
                    ///预测的字段
                    if (!predicColumns.Contains(labelColumn))
                    {
                        predicColumns.Add(labelColumn);
                    }
                    doc.Add("isPredic", 1);//是否预测字段
                    doc.Add("predicColumns", predicColumns);
                    doc.Add("predicBaseColumns", predicBaseColumns);
                   
                    DBChangeQueue.Instance.EnQueue(new StorageData() { Name = tableName, Document = doc, Query = Query.EQ("_id", ObjectId.Parse(school.Text("_id"))), Type = StorageType.Update });
                    #endregion
                }
                Console.WriteLine($"student:{Trip1.student}teacher:{Trip1.teacher} classCount:{Trip1.classCount} Predicted fare: {(int)prediction.student}");
            }
            StartDBChangeProcessQuick(dataop);
            Console.WriteLine("操作结束");


        }


        /// <summary>
        /// 对需要更新的队列数据更新操作进行批量处理,可考虑异步执行
        /// </summary>
        public static void StartDBChangeProcessQuick(MongoRepositoryHelperA3 _mongoDBOp, bool isDefaultField = false)
        {
            if (_mongoDBOp == null)
            {
                throw new NullReferenceException("操作对象为null");
            }
            var result = new InvokeResult();
            List<StorageData> updateList = new List<StorageData>();
            while (DBChangeQueue.Instance.Count > 0)
            {

                var temp = DBChangeQueue.Instance.DeQueue();
                if (temp != null)
                {
                    var insertDoc = temp.Document;

                    switch (temp.Type)
                    {
                        case StorageType.Insert:
                            if (isDefaultField == true)
                            {
                                if (insertDoc.Contains("createDate") == false) insertDoc.Add("createDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));      //添加时,默认增加创建时间
                                if (insertDoc.Contains("createUserId") == false) insertDoc.Add("createUserId", "1");
                                //更新用户
                                if (insertDoc.Contains("underTable") == false) insertDoc.Add("underTable", temp.Name);
                                insertDoc.Set("updateDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));      //更新时间
                                insertDoc.Set("updateUserId", "1");
                            }
                            result = _mongoDBOp.Save(temp.Name, insertDoc); ;
                            break;
                        case StorageType.Update:

                            result = _mongoDBOp.Save(temp.Name, temp.Query, insertDoc);
                            break;
                        case StorageType.Delete:
                            result = _mongoDBOp.Delete(temp.Name, temp.Query);
                            break;
                    }
                    //logInfo1.Info("");
                    if (result.Status == Status.Failed)
                    {

                        //throw new Exception(result.Message);
                    }

                }

            }

            if (DBChangeQueue.Instance.Count > 0)
            {
                StartDBChangeProcessQuick(_mongoDBOp, isDefaultField);
            }
        }
        public bool CheckValue(int teacher, int student)
        {
            if (student - teacher <= 10)
            {
                return false;
            }
            return true;
        }

        public static string GetConnectionStr(string ip, string dbName)
        {
            return $"mongodb://MZsa:MZdba@{ip}:37088/{dbName}";
        }

    }
}
