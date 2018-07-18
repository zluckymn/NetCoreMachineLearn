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
using Microsoft.ML.Data;

namespace MLHelloWorld
{
    public class TaxiPriceML
    {
        const string DataPath = @".\Data\taxi-fare-train.csv";//训练数据
        const string TestDataPath = @".\Data\taxi-fare-test.csv";//测试数据
        const string ModelPath = @".\Models\Model.zip";//模型地址
        const string ModelDirectory = @".\Models";//模型目录

        public class TaxiTrip
        {
            /// <summary>
            /// 主键
            /// </summary>
            [Column(ordinal: "0")]
            public string vendor_id;
            /// <summary>
            /// 评分
            /// </summary>
            [Column(ordinal: "1")]
            public string rate_code;
            /// <summary>
            /// 乘客数量
            /// </summary>
            [Column(ordinal: "2")]
            public float passenger_count;
            /// <summary>
            /// 称作时间
            /// </summary>
            [Column(ordinal: "3")]
            public float trip_time_in_secs;
            /// <summary>
            /// 距离
            /// </summary>
            [Column(ordinal: "4")]
            public float trip_distance;
            /// <summary>
            /// 支付方式
            /// </summary>
            [Column(ordinal: "5")]
            public string payment_type;
            /// <summary>
            /// 运费
            /// </summary>
            [Column(ordinal: "6")]
            public float fare_amount;
        }
        /// <summary>
        ///预测类
        /// </summary>
        public class TaxiTripFarePrediction
        {
            [ColumnName("Score")]
            public float fare_amount;
        }

      
        /// <summary>
        /// 训练并生成模型
        /// </summary>
        /// <returns></returns>
        public static async Task<PredictionModel<TaxiTrip, TaxiTripFarePrediction>> Train()
        {
            //创建学习管道
            var pipeline = new LearningPipeline();
            
            //加载和转换您的数据
            pipeline.Add(new TextLoader(DataPath).CreateFrom<TaxiTrip>(useHeader: true, separator: ','));

            
            //使用该ColumnCopier()功能将“票价_帐户”列复制到名为“标签”的新列中。此列是标签。
            pipeline.Add(new ColumnCopier(("fare_amount", "Label")));
            //进行一些特征工程来转换数据，以便它可以有效地用于机器学习。该训练模型需要算法的数字功能，
            //您变换中的分类数据（vendor_id，rate_code，和payment_type）为数字。
            //该CategoricalOneHotVectorizer()
            //函数为每个列中的值分配一个数字键。通过添加以下代码来转换您的数据：
            pipeline.Add(new CategoricalOneHotVectorizer("vendor_id",
                                             "rate_code",
                                             "payment_type"));
            //数据准备的最后一步是使用该功能将所有功能组合到一个向量中ColumnConcatenator()。这一必要步骤
            //有助于算法轻松处理您的功能。按照您在最后一步中编写的内容添加以下代码：
            //请注意，“trip_time_in_secs”列不包括在内。你已经确定它不是一个有用的预测功能。
            pipeline.Add(new ColumnConcatenator("Features",
                                    "vendor_id",
                                    "rate_code",
                                    "passenger_count",
                                    "trip_distance",
                                    "payment_type"));
            //在将数据添加到流水线并将其转换为正确的输入格式之后，您可以选择一种学习算法（学习者）。学习算
            //法训练模型。你为这个问题选择了一个回归任务，所以你增加了一个学习者调用FastTreeRegressor()到
            //使用梯度提升的管道。
            //渐变增强是回归问题的机器学习技术。它以逐步的方式构建每个回归树。它使用预定义的损失函数来测
            //量每个步骤中的错误，并在下一步中对其进行修正。结果是预测模型实际上是较弱预测模型的集合。
            pipeline.Add(new FastTreeRegressor());

            //训练模型
            //最后一步是训练模型。在此之前，管道中没有任何东西被执行。该pipeline.Train<T_Input, T_Output>()
            //函数接受预定义的TaxiTrip类类型并输出一个TaxiTripFarePrediction类型。将这最后一段代码添加到Train()
            //函数中：
            PredictionModel<TaxiTrip, TaxiTripFarePrediction> model = pipeline.Train<TaxiTrip, TaxiTripFarePrediction>();
            //改性Train（）方法为异步方法public static async Task<PredictionModel<TaxiTrip, TaxiTripFarePrediction>> Train()
            await model.WriteAsync(ModelPath);
            
            return model;

        }
        /// <summary>
        /// 评估模型
        /// </summary>
        /// <param name="model"></param>
        public static void Evaluate(PredictionModel<TaxiTrip, TaxiTripFarePrediction> model)
        {
            var testData = new TextLoader(TestDataPath).CreateFrom<TaxiTrip>( useHeader: true, separator: ',');
            var evaluator = new RegressionEvaluator();
            RegressionMetrics metrics = evaluator.Evaluate(model, testData);
            // Rms should be around 2.795276
            //RMS是评估回归问题的一个指标。它越低，你的模型就越好。将以下代码添加到该Evaluate()函数中以打印模型的RMS。
            Console.WriteLine("Rms=" + metrics.Rms);
            //Squared是评估回归问题的另一个指标。RSquared将是介于0和1之间的值。越接近1，模型越好。将下面的代码添加到该Evaluate()函数中以打印模型的RSquared值。
            Console.WriteLine("RSquared = " + metrics.RSquared);
        }
        static class TestTrips
        {
            internal static readonly TaxiTrip Trip1 = new TaxiTrip
            {
                vendor_id = "VTS",
                rate_code = "1",
                passenger_count = 1,
                trip_distance = 10.33f,
                payment_type = "CSH",
                fare_amount = 0 // predict it. actual = 29.5
            };
        }
        public async Task start()
        {
            PredictionModel<TaxiTrip, TaxiTripFarePrediction> model= null;
            if (File.Exists(ModelPath))
            {
                model = await PredictionModel.ReadAsync<TaxiTrip, TaxiTripFarePrediction>(ModelPath);
            }
            if (model == null) {
                //PredictionModel<TaxiTrip, TaxiTripFarePrediction> model = TrainAsync();
                //改变的返回类型Train方法意味着你必须一个补充await，以调用codde Train在Method下面的代码如下所示：
                model = await Train();
            }
            //await在你的Main方法中添加一个方法意味着该Main方法必须具有async修饰符并返回a Task：
            //using System.Threading.Tasks;
            //评估模型
            Evaluate(model);
            //此行的实际票价为29.5，但使用0作为占位符。机器学习算法将预测票价。
            //在你的Main函数中添加下面的代码。它使用TestTrip数据测试你的模型：
            var prediction = model.Predict(TestTrips.Trip1);
            Console.WriteLine("Predicted fare: {0}, actual fare: 29.5", prediction.fare_amount);
           
        }
         
    }
}
