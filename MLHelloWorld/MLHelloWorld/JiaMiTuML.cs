using System;
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
using MZ.Common;

namespace MLHelloWorld
{
    /// <summary>
    /// 学校人数预测,根据班级，学校类型占地面积预测学生个数据
    /// </summary>
    public class JiaMiTuML
    {
        const string DataPath = @".\Data\buildingArea_JiaMiTu-train.csv";//训练数据
        const string TestDataPath = @".\Data\buildingArea_JiaMiTu-test.csv";//测试数据
        const string ModelPath = @".\Models\JiaMiTu_predic_Model_buildingArea_ClasCount.zip";//模型地址,
        const string ModelDirectory = @".\Models";//模型目录
        public enum RareDegree
        {
            [EnumDescription("普通")]
            normal =1,
            [EnumDescription("稀有")]
            uncommon =2,
            [EnumDescription("稀罕")]
            rare =3,
            [EnumDescription("史诗")]
            epic =4,
            [EnumDescription("传奇")]
            legend =5
        }
        public class JiaMiTu
        {

           
            /// <summary>
            /// 嘴巴
            /// </summary>
            [Column(ordinal: "0")]
            public float MOUTH;
            /// <summary>
            /// 花纹
            /// </summary>
            [Column(ordinal: "1")]
            public float PATTERN_COLOR;
            /// <summary>
            /// 眼睛颜色
            /// </summary>
            [Column(ordinal: "2")]
            public float EYE_COLOR;

            /// <summary>
            /// 眼睛
            /// </summary>
            [Column(ordinal: "3")]
            public float EYE;
            /// <summary>
            /// 花纹颜色
            /// </summary>
            [Column(ordinal: "4")]
            public float PATTERN;

            /// <summary>
            /// 手指
            /// </summary>
            [Column(ordinal: "5")]
            public float FIGURE;
            /// <summary>
            /// 身体颜色
            /// </summary>
            [Column(ordinal: "6")]
            public float BODY_COLOR;

            /// <summary>
            /// 汇总得分值
            /// </summary>
            [Column(ordinal: "7")]
            public float TotalPoint;

            /// <summary>
            /// 汇总得分值
            /// </summary>
            [Column(ordinal: "8")]
            public float FatherRareDegree;
            /// <summary>
            /// 汇总得分值
            /// </summary>
            [Column(ordinal: "9")]
            public float MotherRareDegree;


            /// <summary>
            /// 学生个数
            /// </summary>
            [Column(ordinal: "10")]
            public float RareDegree;
      
        }
        /// <summary>
        ///预测类
        /// </summary>
        public class JiaMiTuPrediction
        {
            [ColumnName("Score")]
            public float RareDegree;
           
        }


        /// <summary>
        /// 训练并生成模型
        /// </summary>
        /// <returns></returns>
        public static async Task<PredictionModel<JiaMiTu, JiaMiTuPrediction>> Train(IEnumerable<JiaMiTu> trainData, string modelFileName, string labelColumn, string[] oneHotColumns, string[] features, string[] drops)
        { 
            //创建学习管道
            var pipeline = new LearningPipeline();
            //加载和转换您的数据
            //var textLoader = new TextLoader<JiaMiTu>(DataPath, useHeader: true, separator: ",");

            //pipeline.Add(textLoader);
             pipeline.Add(CollectionDataSource.Create(trainData));
            //使用该ColumnCopier()功能将“票价_帐户”列复制到名为“标签”的新列中。此列是标签。
             pipeline.Add(new ColumnCopier((labelColumn, "Label")));
            //一个对象叫ColumnDropper，可以用来在训练开始前舍弃掉不需要的字段，比如id，对结果没有任何影响，因此可以去掉
            if (drops.Count() > 0)
            {
                pipeline.Add(new ColumnDropper() { Column = drops });
            }
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
            //函数接受预定义的JiaMiTu类类型并输出一个JiaMiTuPrediction类型。将这最后一段代码添加到Train()
            //函数中：
            PredictionModel<JiaMiTu, JiaMiTuPrediction> model = pipeline.Train<JiaMiTu, JiaMiTuPrediction>();
            //改性Train（）方法为异步方法public static async Task<PredictionModel<JiaMiTu, JiaMiTuPrediction>> Train()
            ///通过生么预测什么
            if(!string.IsNullOrEmpty(modelFileName))
            await model.WriteAsync(modelFileName);
            
            return model;

        }
        /// <summary>
        /// 评估模型
        /// </summary>
        /// <param name="model"></param>
        public static void Evaluate(IEnumerable<JiaMiTu> testData,PredictionModel<JiaMiTu, JiaMiTuPrediction> model)
        {
            //var testData = new TextLoader<JiaMiTu>(TestDataPath, useHeader: true, separator: ",");
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
            internal static readonly JiaMiTu Trip1 = new JiaMiTu
            {
               BODY_COLOR=1, EYE=1, EYE_COLOR=0, FIGURE=1,  MOUTH=1, PATTERN=1, PATTERN_COLOR=1, RareDegree=1
            };
        }
        string[] specialFields = new string[] { "BODY_COLOR", "EYE", "EYE_COLOR", "FIGURE", "MOUTH", "PATTERN", "PATTERN_COLOR" };
        public JiaMiTu InitialJiaMiTu(BsonDocument doc)
        {
            var miTuObj = new JiaMiTu() {
                //BODY_COLOR = doc["BODY_COLOR.rare"].ToString()=="true"?1:0,
                //EYE = doc["EYE.rare"].ToString() == "true" ? 1 : 0,
                //EYE_COLOR = doc["EYE_COLOR.rare"].ToString() == "true" ? 1 : 0,
                //FIGURE = doc["FIGURE.rare"].ToString() == "true" ? 1 : 0,
                //MOUTH = doc["MOUTH.rare"].ToString() == "true" ? 1 : 0,
                //PATTERN = doc["PATTERN.rare"].ToString() == "true" ? 1 : 0,
                //PATTERN_COLOR = doc["PATTERN_COLOR.rare"].ToString() == "true" ? 1 : 0,
            };
             var type = miTuObj.GetType();
            try
            {
                //赋值遍历
                foreach (var elem in doc.Elements)
                {
                    if (specialFields.Contains(elem.Name))
                    {
                        var curValue = doc[elem.Name] as BsonDocument;
                        if (curValue.Text("rare") == "true")
                        {
                            var getProperty = type.GetField(elem.Name);
                            if (getProperty != null)
                            {
                                getProperty.SetValue(miTuObj, 1);
                            }
                        }
                    }
                }

                miTuObj.RareDegree = (int)Enum.Parse(typeof(RareDegree), doc.Text("rareDegreeKey"));
            }
            catch (Exception ex)
            {

            }
            return miTuObj;
        }

        public JiaMiTu InitialJiaMiTuPoint(JiaMiTu curMiTu,JiaMiTu father, JiaMiTu mother)
        {
            float totalPoint = 0;
            var curMiTuType = curMiTu.GetType();
            var fatherMiTuType = curMiTu.GetType();
            var motherMiTuType = curMiTu.GetType();
           
            foreach (var field in specialFields)
            {
                var fatherValue = (float)fatherMiTuType.GetField(field).GetValue(father);
                var motherValue = (float)motherMiTuType.GetField(field).GetValue(mother);
                var summary = fatherValue + motherValue;
                curMiTuType.GetField(field).SetValue(curMiTu, summary);
                totalPoint += summary;
            }
        
 
            //curMiTu.BODY_COLOR = father.BODY_COLOR + mother.BODY_COLOR;
            //curMiTu.EYE = father.EYE + mother.EYE;
            //curMiTu.EYE_COLOR = father.EYE_COLOR + mother.EYE_COLOR;
            //curMiTu.FIGURE = father.FIGURE + mother.FIGURE;
            //curMiTu.MOUTH = father.MOUTH + mother.MOUTH;
            //curMiTu.PATTERN = father.PATTERN + mother.PATTERN;
            //curMiTu.PATTERN_COLOR = father.PATTERN_COLOR + mother.PATTERN_COLOR;
            ////汇总得分
            //totalPoint = curMiTu.BODY_COLOR + curMiTu.EYE + curMiTu.EYE_COLOR + curMiTu.FIGURE + curMiTu.MOUTH + curMiTu.PATTERN + curMiTu.PATTERN_COLOR;
            curMiTu.TotalPoint = totalPoint;
            curMiTu.FatherRareDegree = father.RareDegree;
            curMiTu.MotherRareDegree= mother.RareDegree;
            return curMiTu;
        }

        public async Task start()
        {
            bool alwayCreateModel = true;//是否建立模型文档
            string labelColumn = "RareDegree";//需要计算的字段 student
            string[] oneHotColumns = new string[] {  };//oneHot编码字段
            string[] features = new string[] { "FatherRareDegree", "MotherRareDegree", "TotalPoint" };//特征列, "classCount" "student"
            var curTypeNameIndex =4;//幼儿园 1小学 4中学 5大学 6驾校 7教育其他
            var tableName = "MiTuProfile";
            var tableNameEgg = "MiTu";
            //条件筛选项
           
            var conStr = GetConnectionStr("192.168.1.124", "CrawlerDataBase");
            var dataop = new MongoRepositoryHelperA3(conStr);
            //训练评估查询语句
            var query = Query.And(Query.EQ("isUpdate", 1));
            //预测语句
            var fields = new string[] { "id","BODY_COLOR", "EYE", "EYE_COLOR", "FIGURE", "MOUTH", "PATTERN", "PATTERN_COLOR", "rareDegreeKey", "parents" };
         
            var traningJiaMiTuList = new List<JiaMiTu>();//训练数据
            var evaluateJiaMiTuList = new List<JiaMiTu>();//评估数据
          

            PredictionModel<JiaMiTu, JiaMiTuPrediction> model= null;
            ///模型地址
            var modelFileName = $"{ModelDirectory}/{string.Join("_", features)}-{labelColumn}.zip";
            if (File.Exists(modelFileName))
            {
                model = await PredictionModel.ReadAsync<JiaMiTu, JiaMiTuPrediction>(modelFileName);
                // await在你的Main方法中添加一个方法意味着该Main方法必须具有async修饰符并返回a Task：
                // using System.Threading.Tasks;
                // 评估模型
            }
            else
            {
                if (model == null)
                {
                    var allExistJiaMiTuDocList = dataop.FindByQuery(tableName, query).SetFields(fields).ToList();
                    var allCount = allExistJiaMiTuDocList.Count();
                    int traningCount = (int)(allCount * 0.7);//评估的数据源个数
                    ///加载模型需要数据
                    foreach (var traningJiaMiTu in allExistJiaMiTuDocList)
                    {
                        if (!traningJiaMiTu.Contains("parents")) continue;
                        var parents = traningJiaMiTu["parents"] as BsonArray;
                        if (parents.Count <= 1) continue;
                        var father = parents[0] as BsonDocument;
                        var mother = parents[1] as BsonDocument;

                        var curJiaMiTu = InitialJiaMiTu(traningJiaMiTu);//赋值字段
                                                                        //var fatherDoc = allExistJiaMiTuDocList.Where(c => c.Text("id") == father.Text("petId")).FirstOrDefault();
                                                                        //var motherDoc = allExistJiaMiTuDocList.Where(c => c.Text("id") == mother.Text("petId")).FirstOrDefault();
                        var fatherDoc = dataop.FindOne(tableName, Query.EQ("id", father.Text("petId")));
                        var motherDoc = dataop.FindOne(tableName, Query.EQ("id", mother.Text("petId")));

                        if (fatherDoc == null || motherDoc == null) continue;
                        //父亲
                        var fatherMiTu = InitialJiaMiTu(fatherDoc);
                        //母亲
                        var motherMiTu = InitialJiaMiTu(motherDoc);
                        //当前加密兔

                        curJiaMiTu = InitialJiaMiTuPoint(curJiaMiTu, fatherMiTu, motherMiTu);//计算得分
                        if (curJiaMiTu.TotalPoint <= 0)
                        {
                            continue;
                        }
                        if (traningJiaMiTuList.Count <= traningCount)
                        {
                            traningJiaMiTuList.Add(curJiaMiTu);
                        }
                        else
                        {
                            evaluateJiaMiTuList.Add(curJiaMiTu);
                        }
                    }
                    //PredictionModel<JiaMiTu, JiaMiTuPrediction> model = TrainAsync();
                    //改变的返回类型Train方法意味着你必须一个补充await，以调用codde Train在Method下面的代码如下所示：
                    model = await Train(traningJiaMiTuList, modelFileName, labelColumn, oneHotColumns, features, specialFields);

                }
                Evaluate(evaluateJiaMiTuList, model);
            }
            var prediction1 = model.Predict(TestTrips.Trip1);
           
            Console.WriteLine("确认模型后按回车开始");
            
            // var query = Query.And(Query.Exists("student", true), Query.Exists("teacher", true), Query.Exists("classCount", false));
             
            var allJiaMiTuList = dataop.FindByQuery(tableNameEgg, Query.And(Query.Exists("pIds",true),Query.NE("isPredic", 1),Query.NE("isPass",1))).ToList();
            var ExecNum = Guid.NewGuid().ToString();
            var index = 1;
            foreach (var JiaMiTu in allJiaMiTuList)
            {
                 
                /// 比如没有班级字段通过模型的学生和老师个数来预测班级数
                var id = JiaMiTu.Text("id");
                var pIds = JiaMiTu["pIds"] as BsonArray;
                var fatherDoc = dataop.FindOne(tableName, Query.EQ("id", pIds[0].ToString()));
                var motherDoc = dataop.FindOne(tableName, Query.EQ("id", pIds[1].ToString()));
                if (fatherDoc == null || motherDoc == null) continue;
                var doc = new BsonDocument();
                doc.Set("ExecNum", ExecNum);
                //父亲
                var fatherMiTu = InitialJiaMiTu(fatherDoc);
                //母亲
                var motherMiTu = InitialJiaMiTu(motherDoc);
                //当前加密兔
                var Trip1 = InitialJiaMiTu(JiaMiTu);//赋值字段
                Trip1 = InitialJiaMiTuPoint(Trip1, fatherMiTu, motherMiTu);//计算得分
                if (motherMiTu.RareDegree >= (int)RareDegree.rare && fatherMiTu.RareDegree <= (int)RareDegree.rare || fatherMiTu.RareDegree >= (int)RareDegree.rare && motherMiTu.RareDegree <= (int)RareDegree.rare)
                {


                    var prediction = model.Predict(Trip1);
                  
                    if (prediction.RareDegree != 0)
                    {
                        string txt = EnumDescription.GetFieldText((RareDegree)prediction.RareDegree);
                        doc.Add($"{labelColumn}Predic", prediction.RareDegree);
                        doc.Add($"{labelColumn}PredicTxt", txt);//总得分
                        if (txt != doc.Text("rareDegree"))
                        {

                        }
                        #region 操作日志字段
                        doc.Add("isPredic", 1);//是否预测字段
                        doc.Add("point", Trip1.TotalPoint);//总得分


                        DBChangeQueue.Instance.EnQueue(new StorageData() { Name = tableNameEgg, Document = doc, Query = Query.EQ("_id", ObjectId.Parse(JiaMiTu.Text("_id"))), Type = StorageType.Update });
                        #endregion
                    }
                    Console.WriteLine($"fatherRareDegree:{fatherMiTu.RareDegree}motherRareDegree{motherMiTu.RareDegree} predic:{prediction.RareDegree} id:{id}");
                }
                else
                {
                   
                    doc.Add("isPass", 1);//是否预测字段
                    DBChangeQueue.Instance.EnQueue(new StorageData() { Name = tableNameEgg, Document = doc, Query = Query.EQ("_id", ObjectId.Parse(JiaMiTu.Text("_id"))), Type = StorageType.Update });
                }
                index++;
                if (index % 100 == 0)
                {
                   
                    Console.WriteLine("剩余处理{0}{1}", allJiaMiTuList.Count-index, ExecNum);
                }
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
