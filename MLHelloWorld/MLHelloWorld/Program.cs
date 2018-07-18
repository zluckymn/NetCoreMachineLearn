using Microsoft.ML;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MLHelloWorld
{
    class Program
    {
        //https://www.cnblogs.com/BeanHsiang/p/9094371.html
        //教程务必照着写
        static void Main(string[] args)
        {
            //Test();
            var ml = new JiaMiTuML();
            var awaiter= ml.start();
            Task.WaitAll(awaiter);
            Console.ReadKey();
        }
        static void Test()
        {
            var btyeArray_K = new byte[16];
            //btyeArray_K[0] = 35;
            //btyeArray_K[1] = 35;
            //btyeArray_K[2] = 43;
            //btyeArray_K[3] = 39;
            //btyeArray_K[4] = 33;
            //btyeArray_K[5] = 30;
            //btyeArray_K[6] = 44;
            //btyeArray_K[7] = 38;
            //btyeArray_K[8] = 32;
            //btyeArray_K[9] = 37;
            //btyeArray_K[10] = 42;
            //btyeArray_K[11] = 44;
            //btyeArray_K[12] = 41;
            //btyeArray_K[13] = 42;
            //btyeArray_K[14] = 46;
            //btyeArray_K[15] = 44;
            var btyeArray_V = new byte[16];
            //btyeArray_V[0] = 44;
            //btyeArray_V[1] = 37;
            //btyeArray_V[2] = 43;
            //btyeArray_V[3] = 36;
            //btyeArray_V[4] = 46;
            //btyeArray_V[5] = 37;
            //btyeArray_V[6] = 31;
            //btyeArray_V[7] = 41;
            //btyeArray_V[8] = 31;
            //btyeArray_V[9] = 32;
            //btyeArray_V[10] = 31;
            //btyeArray_V[11] = 35;
            //btyeArray_V[12] = 33;
            //btyeArray_V[13] = 45;
            //btyeArray_V[14] = 45;
            //btyeArray_V[15] = 35;
            var btyeArray = new byte[16];
            btyeArray[0] = 92;
            btyeArray[1] = 115;
            btyeArray[2] = 116;
            btyeArray[3] = 117;
            btyeArray[4] = 112;
            btyeArray[5] = 113;
            btyeArray[6] = 6;
            btyeArray[7] = 112;
            btyeArray[8] = 112;
            btyeArray[9] = 3;
            btyeArray[10] = 3;
            btyeArray[11] = 4;
            btyeArray[12] = 6;
            btyeArray[13] = 118;
            btyeArray[14] = 0;
            btyeArray[15] = 112;

            int j = 0;
            for (var m = 24; j < btyeArray.Length;)
            {
                var temp = (byte)(m ^ btyeArray[j] & 0xFF);
                btyeArray[j] = temp;
                j += 1;
                m = temp;
            }
            var result3 = Encoding.UTF8.GetString(btyeArray);
           
            Console.WriteLine(result3);
        }
    }
  
}
