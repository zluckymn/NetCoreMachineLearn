using Microsoft.ML;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using System;
using System.Threading.Tasks;

namespace MLHelloWorld
{
    class Program
    {
        //https://www.cnblogs.com/BeanHsiang/p/9094371.html
        //教程务必照着写
        static void Main(string[] args)
        {
            var ml = new SchoolML();
            var awaiter= ml.start();
            Task.WaitAll(awaiter);
            Console.ReadKey();
        }
    }
  
}
