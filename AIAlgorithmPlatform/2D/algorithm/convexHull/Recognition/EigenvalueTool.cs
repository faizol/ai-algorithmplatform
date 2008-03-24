using System;
using System.Collections.Generic;
using System.Text;

namespace Recognition
{
    public class EigenvalueTool
    {
         double TOL ;
        //�������   
        int NMAX;
        //�������   
        int M ;
        //����������   
        public EigenvalueTool(int N)
        {
            NMAX = N;
            TOL = 0.001;
            M = 20000;
        }
        public EigenvalueTool(int N,double err)
        {
            NMAX =N;
            TOL = err;
            M = 20000;
        }

        public EigenvalueTool(int N, double err, int iters)
        {
            NMAX = N;
            TOL = err;
            M = iters;
        }

       double Max(double[] u)
        {
            double j = u[0];
            for (int i = 1; i < NMAX; i++)
            {
                if (j < u[i])
                    j = u[i];
            }
            return j;
        }

         void Evaluate(double[] u, double[] u0)
        {
            for (int i = 0; i < NMAX; i++)
                u[i] = u0[i];
        }

        void Multi(double[] v, double[][] a, double[] u)
        {
            for (int i = 0; i < NMAX; i++)
            {
                v[i] = 0;
                for (int j = 0; j < NMAX; j++)
                {
                    v[i] += a[i][j] * u[j];
                }
            }
        }

        public double MaxEigenValue(double[][] a)
        {
        

            double[] u0 = new double[NMAX];//�����ʼ��������ȥ��ʼ��  
            for (int l = 0; l < NMAX; l++)
            {
                u0[l] = 1;
            }
            
            double b;//����������ֵ�Ľ���ֵ   
            double[] u = new double[NMAX];//������������   

            double k, ERR;
            int i;
            double[] w, v;
            w = new double[NMAX];
            v = new double[NMAX];
            Evaluate(u, u0);

            b = Max(u);

            for (i = 0; i < NMAX; i++)
            {
                u[i] = u[i] / b;
            }

            for (k = 1; k < M; k++)
            {
                Multi(v, a, u);
                b = Max(v);

                if (0 == b)
                    return 0;//   0������ֵ����ѡ��ֵu0���¼���   

                for (i = 0; i < NMAX; i++)
                {
                    w[i] = v[i] / b;
                    u[i] = Math.Abs(u[i] - w[i]);
                }
                ERR = Max(u);
                Evaluate(u, w);

                if (ERR < TOL)
                    return b;   //   b������ֵ��u����������   
            }

            return 0;   //�����������   
        }

        public double[] MaxEigenVector(double[][] a)
        {


            double[] u0 = new double[NMAX];//�����ʼ��������ȥ��ʼ��  
            for (int l = 0; l < NMAX; l++)
            {
                u0[l] = 1;
            }

            double b;//����������ֵ�Ľ���ֵ   
            double[] u = new double[NMAX];//������������   

            double k, ERR;
            int i;
            double[] w, v;
            w = new double[NMAX];
            v = new double[NMAX];
            Evaluate(u, u0);

            b = Max(u);

            for (i = 0; i < NMAX; i++)
            {
                u[i] = u[i] / b;
            }

            for (k = 1; k < M; k++)
            {
                Multi(v, a, u);
                b = Max(v);

                if (0 == b)
                    return null;//   0������ֵ����ѡ��ֵu0���¼���   

                for (i = 0; i < NMAX; i++)
                {
                    w[i] = v[i] / b;
                    u[i] = Math.Abs(u[i] - w[i]);
                }
                ERR = Max(u);
                Evaluate(u, w);

                if (ERR < TOL)
                    return u;   //   b������ֵ��u����������   
            }

            return null;   //�����������   
        } 

    }
}
