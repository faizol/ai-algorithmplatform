using System;
using System.Collections.Generic;
using System.Text;
using Configuration;

namespace CurvePlotDrawer
{
    //--------------------------------------
    // �ࣺSequenceMaker
    // ������������
    //--------------------------------------
    public class SequenceMaker
    {
        //����һ��ָ��������X����
        public static double[] GenerateData_Exp()
        {
            SequenceGenerOfExp gener = new SequenceGenerOfExp();
            new ConfiguratedByForm(gener);
            return gener.CreateSequence();
        }
        public static double[] GenerateData_Exp(double baseNum, int startExp, int endExp)
        {
            SequenceGenerOfExp gener = new SequenceGenerOfExp();
            gener.BaseNum = baseNum;
            gener.StartExp = startExp;
            gener.EndExp = endExp;
            return gener.CreateSequence();
        }

        //����һ����start��end������������X����
        public static double[] GenerateData_Linear()
        {
            SequenceGenerOfLinear gener = new SequenceGenerOfLinear();
            new ConfiguratedByForm(gener);
            return gener.CreateSequence();
        }
        public static double[] GenerateData_Linear(double startNum, double endNum, double interval)
        {
            SequenceGenerOfLinear gener = new SequenceGenerOfLinear();
            gener.StartNum = startNum;
            gener.EndNum = endNum;
            gener.Interval = interval;
            return gener.CreateSequence();
        }

        //����һ����start��end�ı���������X����
        public static double[] GenerateData_Multiple()
        {
            SequenceGenerOfMultiple gener = new SequenceGenerOfMultiple();          
            new ConfiguratedByForm(gener);

            return gener.CreateSequence();
        }
        public static double[] GenerateData_Multiple(double startNum, double endNum, double multiple)
        {
            SequenceGenerOfMultiple gener = new SequenceGenerOfMultiple();
            gener.StartNum = startNum;
            gener.EndNum = endNum;
            gener.Multiple = multiple;
            return gener.CreateSequence();
        }
    }

    /// <summary>
    /// �ض���������������
    /// </summary>
    public interface SequenceGener
    {
        double[] CreateSequence();
    }

    /// <summary>
    /// ָ����������
    /// </summary>
    class SequenceGenerOfExp : SequenceGener
    {
        private double baseNum;
        private int startExp, endExp;

        public int StartExp
        {
            get { return startExp; }
            set { startExp = value; }
        }

        public int EndExp
        {
            get { return endExp; }
            set { endExp = value; }
        }

        public double BaseNum
        {
            get { return baseNum; }
            set { baseNum = value; }
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <returns></returns>
        public double[] CreateSequence()
        {
            double[] sequenceData = new double[endExp - startExp + 1];
            sequenceData[0] = 1;
            for (int i = 0; i < startExp; i++)
            {
                sequenceData[0] *= baseNum;
            }
            for (int i = 0; i < endExp - startExp; i++)
            {
                sequenceData[i + 1] = sequenceData[i] * baseNum;
            }
            return sequenceData;
        }
    }

    /// <summary>
    /// ������������
    /// </summary>
    class SequenceGenerOfLinear : SequenceGener
    {
        private double startNum, endNum, interval;

        public double Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        public double StartNum
        {
            get { return startNum; }
            set { startNum = value; }
        }

        public double EndNum
        {
            get { return endNum; }
            set { endNum = value; }
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <returns></returns>
        public double[] CreateSequence()
        {
            int num = (int)((endNum - startNum) / interval) + 1;
            double[] xData = new double[num];

            double curX = startNum;
            for (int i = 0; i < num; i ++, curX += interval)
                xData[i] = curX;

            return xData;
        }
    }

    class SequenceGenerOfMultiple : SequenceGener
    {
        private double startNum = 100; 
        private double endNum = 1000000;
        private double multiple = 1.1;

        public double Multiple
        {
            get { return multiple; }
            set { multiple = value; }
        }

        public double EndNum
        {
            get { return endNum; }
            set { endNum = value; }
        }

        public double StartNum
        {
            get { return startNum; }
            set { startNum = value; }
        }

        public double[] CreateSequence()
        {
            int num = (int)(Math.Log(endNum / startNum) / Math.Log(multiple)) + 1;
            double[] xData = new double[num];

            double curX = startNum;
            for (int i = 0; i < num; i ++, curX *= multiple)
            {
                xData[i] = curX;
            }
            return xData;
        }
    }
}
