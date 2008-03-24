using System;
using System.Collections.Generic;
using System.Text;
using Position_Interface;

namespace TestConvexHull
{
    class ConvexHullCompare
    {
        IPositionSet ps, cps, cps_ref;
        string report;
        bool correct;

        public ConvexHullCompare(IPositionSet ps, IPositionSet cps, IPositionSet cps_ref)
        {
            this.ps = ps;
            this.cps = cps;
            this.cps_ref = cps_ref;
        }

        public string GetReport()
        {
            return report;
        }

        public bool Compare()
        {
            report = "";
            correct = true;

            TestReference();
            TestEquivalence();
            report += "\r\n";
            return correct;
        }

        private bool ComparePosition(IPosition p1, IPosition p2)
        {
            return p1.GetX() == p2.GetX() && p1.GetY() == p2.GetY();
        }

        //���Ե㼯����
        private void TestReference()
        {
            cps.InitToTraverseSet();
            ps.InitToTraverseSet();
            correct = true;
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
            report += "�����ò��ԡ�\r\n" + (correct ? "ͨ����" : "δͨ����") + "\r\n";
        }

        //����͹���ȼ���
        private void TestEquivalence()
        {
            IPosition[] cpa = (IPosition[])(cps.ToArray());
            IPosition[] cpa_ref = (IPosition[])(cps_ref.ToArray());

            report += "��͹����ȷ�Բ��ԡ�\r\n";
            if (cpa.Length != cpa_ref.Length)
            {
                report += "��Ŀ����";
                correct = false;
                return;
            }

            int n = cpa.Length;
            int m = 0;
            for (int dir1 = -1; dir1 < 2; dir1 += 2)
            {
                for (int dir2 = -1; dir2 < 2; dir2 += 2)
                {
                    int p1 = 0, p2 = 0;
                    while (ComparePosition(cpa[(p1 + n - dir1) % n], cpa[p1]))
                        p1 = (p1 + n - dir1) % n;
                    for (; p2 < n; p2++)
                    {
                        if (ComparePosition(cpa[p1], cpa_ref[p2]))
                        {
                            while (ComparePosition(cpa_ref[(p2 + n - dir2) % n], cpa_ref[p2]))
                                p2 = (p2 + n - dir2) % n;
                            break;
                        }
                    }
                    if (p2 == n)
                    {
                        correct = false;
                        report += "��������";
                    }

                    int tm = 0;
                    if (correct)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            if (ComparePosition(cpa[(p1 + j * dir1 + n) % n], cpa_ref[(p2 + j * dir2 + n) % n]))
                                tm++;
                        }
                    }
                    if (tm > m)
                        m = tm;

                }
            }
            report += "��ȷ�ʣ�" + m.ToString() + "/" + n.ToString();
            report += "\r\n";
            if (m == n)
                report += "ͨ����";
            else
                report += "δͨ����";
            report += "\r\n";
        }

    }
}
