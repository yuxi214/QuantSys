﻿using QuantSys.Analytics.StatisticalModeling;
using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.Analytics.Timeseries.Indicators.Averages;
using QuantSys.DataStructures;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Channels
{
    public class KirshenbaumBands : AbstractChannel
    {

        private EMA EMA;
        private MovingQueue<double> LRValues;
        private double STDEV;

        public KirshenbaumBands(int n = 20, int l = 30, double dev = 1.75)
            : base(n)
        {
            EMA = new EMA(n);
            LRValues= new MovingQueue<double>(l);
            STDEV = dev;
        }

        public override void HandleNextTick(Tick t)
        {
            double emaVal = EMA.HandleNextTick(t);
            LRValues.Enqueue(emaVal);

            double[] Y = LRValues.ToArray();

            LinearRegression l = new LinearRegression();
            l.Model(Y);
            
            HighData.Enqueue(EMA[0]+STDEV * l.STDERR);
            MiddleData.Enqueue(EMA[0]);
            LowData.Enqueue(EMA[0] - STDEV * l.STDERR);
             
        }

        public override string ToString()
        {
            return "Kirshenbaum Bands" + Period;
        }
        
    }
}
