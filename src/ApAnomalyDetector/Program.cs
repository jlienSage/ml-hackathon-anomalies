using System;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;

namespace Sage.CRE.ApAnomalyDetector
{
    public class Program
    {
        public const string Usage = @"Usage:
Train:
    --train|-t path/to/csv/data path/to/output/model
Predict:
    --predict|-p path/to/model path/to/csv/data numberOfPredictions";

        public static void Main(string[] args)
        {
            switch(args.Length < 1 ? string.Empty : args[0])
            {
                case "--train":
                case "-t":
                    if (args.Length < 3)
                    {
                        Console.WriteLine("Train requires 2 arguments.");
                        return;
                    }
                    Trainer.TrainModel(args[1], args[2]);
                    break;
                case "--predict":
                case "-p":
                    if (args.Length < 4)
                    {
                        Console.WriteLine("Predict requires 3 argument.");
                        return;
                    }
                    Predictor.Predict(args[1], args[2], int.Parse(args[3]));
                    break;
                case "--help":
                case "-h":
                default:
                    Console.WriteLine(Usage);
                    return;
            }
        }
    }
}
