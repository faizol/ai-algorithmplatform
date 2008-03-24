using System;
using System.Collections.Generic;
using System.Text;

using ConvexHullEngine;
using GrahamScanAlgorithm;
using QuickHullAlgorithm;
using JarvisMatchAlgorithm;
using Position;
using PositionSetViewer;

namespace TestConvexHull
{
    class LanchConvexHull
    {
        private static PositionSet_ImplementByList testData()
        {
            double[] data =
            {
                100, 100,
                100, 150,
                100, 200,
                200, 200,
                200, 100
            };
            int n = data.Length / 2;
            List<IPosition> pl = new List<IPosition>();
            for (int i = 0; i < n; i++)
            {
                pl.Add(new SimplePosition((float)(data[2 * i]), (float)(data[2 * i + 1])));
            }
            PositionSet_ImplementByList ps = new PositionSet_ImplementByList(pl);
            return ps;
        }

        public static void printPositionSet(IPositionSet ps)
        {
            ps.InitToTraverseSet();
            while (ps.NextPosition())
            {
                IPosition p = ps.GetPosition();
                System.Console.Out.WriteLine(p.GetX() + "," + p.GetY() + ",");
            }
        }

        public static void lanch(IConvexHullEngine chEngine, int pointCount, float min, float max)
        {
            //���͹������ͼ
            //IPositionSet ps = testData();
            IPositionSet ps = new RandomPositionSet(pointCount, min, max);
            
            //System.Console.Out.WriteLine("position set:");
            //printPositionSet(ps);
            IPositionSet cps = chEngine.ConvexHull(ps);
            //System.Console.Out.WriteLine("Jarvis Match:");
            //printPositionSet(cps);
            PainterDialog.Clear();
            PainterDialog.DrawPositionSet(ps);
            PainterDialog.DrawConvexHull(cps);
            PainterDialog.Show();


            AnalyzeReport report = new AnalyzeReport();
            //���Ե㼯����
            cps.InitToTraverseSet();
            ps.InitToTraverseSet();
            bool correct = true;
            while (correct && cps.NextPosition())
            {
                IPosition cp = cps.GetPosition();
                bool find = false;
                ps.InitToTraverseSet();
                while (!find && ps.NextPosition())
                {
                    IPosition p = ps.GetPosition();
                    if (cp == p)
                        find = true;
                }
                if (!find)
                    correct = false;
            }
            report.content += "���ò��ԣ�" + (correct ? "��ȷ" : "����") + "\n" ;

            //����͹��
            if (correct)
            {
                report.content += "͹����ȷ�Բ��ԣ�\n";
                IPositionSet cps_ref = (new QuickHull()).ConvexHull(ps);
            //    System.Console.Out.WriteLine("Quick Hull:");
            //    printPositionSet(cps_ref);
                IPosition[] cpa = (IPosition[]) (cps.ToArray());
                IPosition[] cpa_ref = (IPosition[]) (cps_ref.ToArray());
                if (cpa.Length != cpa_ref.Length)
                {
                    report.content += "��Ŀ����";
                    correct = false;
                }
                if (correct)
                {
                    int n = cpa.Length;
                    int m = 0;
                    int p = 0;
                    for (; p < n; p++)
                    {
                        if (cpa[0] == cpa_ref[p])
                            break;
                    }
                    if (p == n)
                    {
                        correct = false;
                        report.content += "��������";
                    }
                    if (correct)
                    {
                        for (int i = 0; i < n; i++)
                        {
                            if (cpa[i] == cpa_ref[(p + i + n ) % n])
                                m++;
                        }
                        int tm = 0;
                        for (int i = 0; i < n; i++)
                        {
                            if (cpa[i] == cpa_ref[(p - i + n) % n])
                                tm++;
                        }
                        if (tm > m)
                            m = tm;
                        report.content += "��ȷ�ʣ�" + m.ToString() + "/" + n.ToString();
                        if (m == n)
                            report.content += "��ȷ��";
                        else
                            report.content += "����ȷ��";
                    }
                }
            }
            report.Show();
            
        }
    }
}
