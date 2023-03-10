using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Diagnostics.Eventing.Reader;
using Microsoft.VisualBasic.Logging;

namespace Utilities
{
    public class JointClass6
    {
        private float a;
        private float d;
        private int index = -1;
        public JointClass6()
        {
        }
        public JointClass6(float av, float ad, int i, float w)
        {
            a = av;
            d = ad;
            index = i;
            JWeight = w;
        }
        public float A
        { get { return a; } }

        public float D
        { get { return d; } }

        public double CurAngle //当前角度值
        {
            set;
            get;
        }
        public float CurPos //当前位置值
        {
            set;
            get;
        }
        public float Velocity //当前角速度值
        {
            set;
            get;
        }
        public float MaxAngle
        {
            set;
            get;
        }
        public float MinAngle
        {
            set;
            get;
        }
        public Matrix4x4 T //位置矩阵
        {
            get
            {
                float b = (float)CurAngle;
                float sb = (float)Math.Sin(b);
                float cb = (float)Math.Cos(b);
                Matrix4x4 ansMat = new Matrix4x4();
                switch (index)
                {
                    case 1:
                        ansMat = new Matrix4x4(cb, 0, -sb, a * cb, sb, 0, cb, a * sb, 0, -1, 0, d, 0, 0, 0, 1);
                        break;
                    case 2:
                        //  ansMat = new Matrix4x4(cb, -sb, 0, a * cb, sb, cb, 0, a * sb, 0, 0, 1, 0, 0, 0, 0, 1);
                        ansMat = new Matrix4x4(sb, cb, 0, a * sb, -cb, sb, 0, -a * cb, 0, 0, 1, 0, 0, 0, 0, 1);
                        break;
                    case 3:
                        ansMat = new Matrix4x4(cb, 0, -sb, a * cb, sb, 0, cb, a * sb, 0, -1, 0, 0, 0, 0, 0, 1);
                        break;
                    case 4:
                        ansMat = new Matrix4x4(cb, 0, sb, 0, sb, 0, -cb, 0, 0, 1, 0, d, 0, 0, 0, 1);
                        break;
                    case 5:
                        ansMat = new Matrix4x4(cb, 0, -sb, a * cb, sb, 0, cb, a * sb, 0, -1, 0, d, 0, 0, 0, 1);
                        break;
                    case 6:
                        ansMat = new Matrix4x4(cb, -sb, 0, a * cb, sb, cb, 0, a * sb, 0, 0, 1, d, 0, 0, 0, 1);
                        break;
                    case 10:
                        ansMat = new Matrix4x4(1, 0, 0, CurPos, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
                        break;
                    case 20:
                        ansMat = new Matrix4x4(1, 0, 0, 0, 0, 1, 0, CurPos, 0, 0, 1, 0, 0, 0, 0, 1);
                        break;
                    case 30:
                        ansMat = new Matrix4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, CurPos, 0, 0, 0, 1);
                        break;
                    case 200:
                        ansMat = new Matrix4x4(cb, 0, -sb, 0, 0, 1, 0, 0, sb, 0, cb, 0, 0, 0, 0, 1);
                        break;
                    case 11:
                        ansMat = new Matrix4x4(1, 0, 0, 0, 0, cb, -sb, a * cb, 0, sb, cb, a * sb, 0, 0, 0, 1);
                        break;
                    case 12:
                        ansMat = new Matrix4x4(cb, 0, sb, a * cb, 0, 1, 0, 0, -sb, 0, cb, -a * sb, 0, 0, 0, 1);
                        break;
                    case 13:
                        ansMat = new Matrix4x4(cb, -sb, 0, a * cb, sb, cb, 0, a * sb, 0, 0, 1, 0, 0, 0, 0, 1);
                        break;
                    default:
                        break;
                }
                return ansMat;
            }
        }
        public Matrix4x4 T1(float a) //一阶导数矩阵
        {
            return new Matrix4x4();
        }
        public float JWeight
        {
            set;
            get;
        }
        /// <summary>
        /// 关节末端的空间姿态-位置向量[px,py,pz]
        /// </summary>
        /// <param name="AD">每个关节的a,d值构成的数组列表，如GSK RB8：{{150,0},{560,0},{155,0},{0,630},{0,0},{0,155}}</param>
        /// <param name="Ang">每个关节的角度值</param>
        /// <returns>[px,py,pz]</returns>
        public static double[] R6P(double[][] AD, double[] Ang)
        {
            double A1 = AD[0][0];
            double D1 = AD[0][1];
            double A2 = AD[1][0];
            double A3 = AD[2][0];
            double D4 = AD[3][1];
            double A6 = AD[5][0];
            double D6 = AD[5][1];
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            double C2 = Math.Cos(Ang[1]);
            double S2 = Math.Sin(Ang[1]);
            double C3 = Math.Cos(Ang[2]);
            double S3 = Math.Sin(Ang[2]);
            double S23 = Math.Sin(Ang[1] + Ang[2]);
            double C23 = Math.Cos(Ang[1] + Ang[2]);
            double C4 = Math.Cos(Ang[3]);
            double S4 = Math.Sin(Ang[3]);
            double C5 = Math.Cos(Ang[4]);
            double S5 = Math.Sin(Ang[4]);
            double C6 = Math.Cos(Ang[5]);
            double S6 = Math.Sin(Ang[5]);

            double px = ((C1 * S23 * C4 + S1 * S4) * C5 + C1 * C23 * S5) * A6 * C6 - (C1 * S23 * S4 - S1 * C4) * A6 * S6 - ((C1 * S23 * C4 + S1 * S4) * S5 - C1 * C23 * C5) * D6 + C1 * C23 * D4 + C1 * S23 * A3 + C1 * A2 * S2 + A1 * C1;
            double py = ((S1 * S23 * C4 - C1 * S4) * C5 + S1 * C23 * S5) * A6 * C6 - (S1 * S23 * S4 + C1 * C4) * A6 * S6 + ((-S1 * S23 * C4 + C1 * S4) * S5 + S1 * C23 * C5) * D6 + S1 * C23 * D4 + S1 * S23 * A3 + S1 * A2 * S2 + A1 * S1;
            double pz = (C23 * C4 * C5 - S23 * S5) * A6 * C6 - C23 * S4 * A6 * S6 - (C23 * C4 * S5 + S23 * C5) * D6 - S23 * D4 + C23 * A3 + A2 * C2 + D1;
            return new double[] { px, py, pz };
        }
        /// <summary>
        /// 关节末端的空间姿态-位置向量[px,py,pz] A6=0 J6=0时
        /// </summary>
        /// <param name="AD">每个关节的a,d值构成的数组列表，如GSK RB8：{{150,0},{560,0},{155,0},{0,630},{0,0},{0,155}}</param>
        /// <param name="Ang">每个关节的角度值</param>
        /// <returns>[px,py,pz]</returns>
        public static double[] R6P2(double[][] AD, double[] Ang)
        {
            double A1 = AD[0][0];
            double D1 = AD[0][1];
            double A2 = AD[1][0];
            double A3 = AD[2][0];
            double D4 = AD[3][1];
            double A6 = 0;
            double D6 = AD[5][1];
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            double C2 = Math.Cos(Ang[1]);
            double S2 = Math.Sin(Ang[1]);
            double C3 = Math.Cos(Ang[2]);
            double S3 = Math.Sin(Ang[2]);
            double S23 = Math.Sin(Ang[1] + Ang[2]);
            double C23 = Math.Cos(Ang[1] + Ang[2]);
            double C4 = Math.Cos(Ang[3]);
            double S4 = Math.Sin(Ang[3]);
            double C5 = Math.Cos(Ang[4]);
            double S5 = Math.Sin(Ang[4]);
            double C6 = 1;
            double S6 = 0;

            double px = -((C1 * S23 * C4 + S1 * S4) * S5 - C1 * C23 * C5) * D6 + C1 * C23 * D4 + C1 * S23 * A3 + C1 * A2 * S2 + A1 * C1;
            double py = ((-S1 * S23 * C4 + C1 * S4) * S5 + S1 * C23 * C5) * D6 + S1 * C23 * D4 + S1 * S23 * A3 + S1 * A2 * S2 + A1 * S1;
            double pz = -(C23 * C4 * S5 + S23 * C5) * D6 - S23 * D4 + C23 * A3 + A2 * C2 + D1;
            return new double[] { px, py, pz };
        }
        /// <summary>
        /// 关节末端的空间姿态-n向量[nx,ny,nz]
        /// </summary>
        /// <param name="Ang">每个关节的角度值</param>
        /// <returns>[nx,ny,nz]</returns>
        public static double[] R6N(double[] Ang)
        {
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            //   double C2 = Math.Cos(Ang[1]);
            //   double S2 = Math.Sin(Ang[1]);
            double C3 = Math.Cos(Ang[2]);
            double S3 = Math.Sin(Ang[2]);
            double S23 = Math.Sin(Ang[1] + Ang[2]);
            double C23 = Math.Cos(Ang[1] + Ang[2]);
            double C4 = Math.Cos(Ang[3]);
            double S4 = Math.Sin(Ang[3]);
            double C5 = Math.Cos(Ang[4]);
            double S5 = Math.Sin(Ang[4]);
            double C6 = Math.Cos(Ang[5]);
            double S6 = Math.Sin(Ang[5]);
            double nx = ((C1 * S23 * C4 + S1 * S4) * C5 + C1 * C23 * S5) * C6 - (C1 * S23 * S4 - S1 * C4) * S6;
            double ny = ((S1 * S23 * C4 - C1 * S4) * C5 + S1 * C23 * S5) * C6 - (S1 * S23 * S4 + C1 * C4) * S6;
            double nz = (C23 * C4 * C5 - S23 * S5) * C6 - C23 * S4 * S6;

            /*
            double nx = (((((C1) * (C2)) * (C3)+((C1) * (-S2)) * (S3)) * (C4)+(((-S1)) * (-1)) * (S4)) * (C5)+((((C1) * (C2)) * (-S3)+((C1) * (-S2)) * (C3))) * (S5)) * (C6)+(((((C1) * (C2)) * (C3)+((C1) * (-S2)) * (S3)) * (S4)+(((-S1)) * (-1)) * (-C4)) * (-1)) * (S6) ;
            double ny = (((((S1) * (C2)) * (C3)+((S1) * (-S2)) * (S3)) * (C4)+(((C1)) * (-1)) * (S4)) * (C5)+((((S1) * (C2)) * (-S3)+((S1) * (-S2)) * (C3))) * (S5)) * (C6)+(((((S1) * (C2)) * (C3)+((S1) * (-S2)) * (S3)) * (S4)+(((C1)) * (-1)) * (-C4)) * (-1)) * (S6) ;
            double nz = (((((-1) * (S2)) * (C3) + ((-1) * (C2)) * (S3)) * (C4)) * (C5) + ((((-1) * (S2)) * (-S3) + ((-1) * (C2)) * (C3))) * (S5)) * (C6) + (((((-1) * (S2)) * (C3) + ((-1) * (C2)) * (S3)) * (S4)) * (-1)) * (S6);
             */
            return new double[] { nx, ny, nz };
        }
        /// <summary>
        /// 关节末端的空间姿态-o向量[ox,oy,oz]
        /// </summary>
        /// <param name="Ang">每个关节的角度值</param>
        /// <returns>[ox,oy,oz]</returns>
        public static double[] R6O(double[] Ang)
        {
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            //   double C2 = Math.Cos(Ang[1]);
            //   double S2 = Math.Sin(Ang[1]);
            double C3 = Math.Cos(Ang[2]);
            double S3 = Math.Sin(Ang[2]);
            double S23 = Math.Sin(Ang[1] + Ang[2]);
            double C23 = Math.Cos(Ang[1] + Ang[2]);
            double C4 = Math.Cos(Ang[3]);
            double S4 = Math.Sin(Ang[3]);
            double C5 = Math.Cos(Ang[4]);
            double S5 = Math.Sin(Ang[4]);
            double C6 = Math.Cos(Ang[5]);
            double S6 = Math.Sin(Ang[5]);
            double ox = ((C1 * S23 * C4 + S1 * S4) * C5 + C1 * C23 * S5) * (-S6) - (C1 * S23 * S4 - S1 * C4) * C6;
            double oy = ((S1 * S23 * C4 - C1 * S4) * C5 + S1 * C23 * S5) * (-S6) - (S1 * S23 * S4 + C1 * C4) * C6;
            double oz = (C23 * C4 * C5 - S23 * S5) * (-S6) - C23 * S4 * C6;
            /*
            double ox =  (((((C1) * (C2)) * (C3)+((C1) * (-S2)) * (S3)) * (C4)+(((-S1)) * (-1)) * (S4)) * (C5)+((((C1) * (C2)) * (-S3)+((C1) * (-S2)) * (C3))) * (S5)) * (-S6)+(((((C1) * (C2)) * (C3)+((C1) * (-S2)) * (S3)) * (S4)+(((-S1)) * (-1)) * (-C4)) * (-1)) * (C6);
            double oy = (((((S1) * (C2)) * (C3)+((S1) * (-S2)) * (S3)) * (C4)+(((C1)) * (-1)) * (S4)) * (C5)+((((S1) * (C2)) * (-S3)+((S1) * (-S2)) * (C3))) * (S5)) * (-S6)+(((((S1) * (C2)) * (C3)+((S1) * (-S2)) * (S3)) * (S4)+(((C1)) * (-1)) * (-C4)) * (-1)) * (C6);
            double oz = (((((-1) * (S2)) * (C3) + ((-1) * (C2)) * (S3)) * (C4)) * (C5) + ((((-1) * (S2)) * (-S3) + ((-1) * (C2)) * (C3))) * (S5)) * (-S6) + (((((-1) * (S2)) * (C3) + ((-1) * (C2)) * (S3)) * (S4)) * (-1)) * (C6);
             */
            return new double[] { ox, oy, oz };
        }
        /// <summary>
        /// 关节末端的空间姿态-a向量[ax,ay,az]
        /// </summary>
        /// <param name="Ang">每个关节的角度值</param>
        /// <returns>[ax,ay,az]</returns>
        public static double[] R6A(double[] Ang)
        {
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            // double C2 = Math.Cos(Ang[1]);
            // double S2 = Math.Sin(Ang[1]);
            double C3 = Math.Cos(Ang[2]);
            double S3 = Math.Sin(Ang[2]);
            double S23 = Math.Sin(Ang[1] + Ang[2]);
            double C23 = Math.Cos(Ang[1] + Ang[2]);
            double C4 = Math.Cos(Ang[3]);
            double S4 = Math.Sin(Ang[3]);
            double C5 = Math.Cos(Ang[4]);
            double S5 = Math.Sin(Ang[4]);
            double C6 = Math.Cos(Ang[5]);
            double S6 = Math.Sin(Ang[5]);
            double ax = (C1 * S23 * C4 + S1 * S4) * (-S5) + C1 * C23 * C5;
            double ay = (S1 * S23 * C4 - C1 * S4) * (-S5) + S1 * C23 * C5;
            double az = C23 * C4 * (-S5) - S23 * C5;
            /*
            double ax = (((((C1) * (C2)) * (C3)+((C1) * (-S2)) * (S3)) * (C4)+(((-S1)) * (-1)) * (S4)) * (-S5)+((((C1) * (C2)) * (-S3)+((C1) * (-S2)) * (C3))) * (C5));
            double ay = (((((S1) * (C2)) * (C3)+((S1) * (-S2)) * (S3)) * (C4)+(((C1)) * (-1)) * (S4)) * (-S5)+((((S1) * (C2)) * (-S3)+((S1) * (-S2)) * (C3))) * (C5));
            double az = (((((-1) * (S2)) * (C3)+((-1) * (C2)) * (S3)) * (C4)) * (-S5)+((((-1) * (S2)) * (-S3)+((-1) * (C2)) * (C3))) * (C5));
             */
            return new double[] { ax, ay, az };
        }
        public static double[] R5P(double[][] AD, double[] Ang)
        {
            double A1 = AD[0][0];
            double D1 = AD[0][1];
            double A2 = AD[1][0];
            double A3 = AD[2][0];
            double D4 = AD[3][1];
            double A6 = AD[5][0];
            double D6 = AD[5][1];
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            double C2 = Math.Cos(Ang[1]);
            double S2 = Math.Sin(Ang[1]);
            double C3 = Math.Cos(Ang[2]);
            double S3 = Math.Sin(Ang[2]);
            double S23 = Math.Sin(Ang[1] + Ang[2]);
            double C23 = Math.Cos(Ang[1] + Ang[2]);
            double C4 = Math.Cos(Ang[3]);
            double S4 = Math.Sin(Ang[3]);
            double C5 = Math.Cos(Ang[4]);
            double S5 = Math.Sin(Ang[4]);
            double C6 = Math.Cos(Ang[5]);
            double S6 = Math.Sin(Ang[5]);

            double px = ((-C1 * (-C23)) * (D4) + (C1 * S23 * A3 + C1 * A2 * S2 + A1 * C1));
            double py = ((-S1 * (-C23)) * (D4) + (S1 * S23 * A3 + S1 * A2 * S2 + A1 * S1));
            double pz = ((-S23) * (D4) + (C23 * A3 + A2 * C2 + D1));
            return new double[] { px, py, pz };
        }
        public static double[] R5N(double[] Ang)
        {
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            //      double C2 = Math.Cos(Ang[1]);
            //      double S2 = Math.Sin(Ang[1]);
            double C3 = Math.Cos(Ang[2]);
            double S3 = Math.Sin(Ang[2]);
            double S23 = Math.Sin(Ang[1] + Ang[2]);
            double C23 = Math.Cos(Ang[1] + Ang[2]);
            double C4 = Math.Cos(Ang[3]);
            double S4 = Math.Sin(Ang[3]);
            double C5 = Math.Cos(Ang[4]);
            double S5 = Math.Sin(Ang[4]);
            double C6 = Math.Cos(Ang[5]);
            double S6 = Math.Sin(Ang[5]);
            double nx = ((C1 * S23) * (C4) + (S1) * (S4)) * (C5) + ((-C1 * (-C23))) * (S5);
            double ny = ((S1 * S23) * (C4) + (-C1) * (S4)) * (C5) + ((-S1 * (-C23))) * (S5);
            double nz = (C23 * (C4)) * (C5) + ((-S23)) * (S5);

            /*
            double nx = (((((C1) * (C2)) * (C3)+((C1) * (-S2)) * (S3)) * (C4)+(((-S1)) * (-1)) * (S4)) * (C5)+((((C1) * (C2)) * (-S3)+((C1) * (-S2)) * (C3))) * (S5)) * (C6)+(((((C1) * (C2)) * (C3)+((C1) * (-S2)) * (S3)) * (S4)+(((-S1)) * (-1)) * (-C4)) * (-1)) * (S6) ;
            double ny = (((((S1) * (C2)) * (C3)+((S1) * (-S2)) * (S3)) * (C4)+(((C1)) * (-1)) * (S4)) * (C5)+((((S1) * (C2)) * (-S3)+((S1) * (-S2)) * (C3))) * (S5)) * (C6)+(((((S1) * (C2)) * (C3)+((S1) * (-S2)) * (S3)) * (S4)+(((C1)) * (-1)) * (-C4)) * (-1)) * (S6) ;
            double nz = (((((-1) * (S2)) * (C3) + ((-1) * (C2)) * (S3)) * (C4)) * (C5) + ((((-1) * (S2)) * (-S3) + ((-1) * (C2)) * (C3))) * (S5)) * (C6) + (((((-1) * (S2)) * (C3) + ((-1) * (C2)) * (S3)) * (S4)) * (-1)) * (S6);
             */
            return new double[] { nx, ny, nz };
        }
        public static double[] R5O(double[] Ang)
        {
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            //     double C2 = Math.Cos(Ang[1]);
            //     double S2 = Math.Sin(Ang[1]);
            double C3 = Math.Cos(Ang[2]);
            double S3 = Math.Sin(Ang[2]);
            double S23 = Math.Sin(Ang[1] + Ang[2]);
            double C23 = Math.Cos(Ang[1] + Ang[2]);
            double C4 = Math.Cos(Ang[3]);
            double S4 = Math.Sin(Ang[3]);
            double C5 = Math.Cos(Ang[4]);
            double S5 = Math.Sin(Ang[4]);
            double C6 = Math.Cos(Ang[5]);
            double S6 = Math.Sin(Ang[5]);
            double ox = ((C1 * S23) * (S4) + (S1) * (-C4)) * (-1);
            double oy = ((S1 * S23) * (S4) + (-C1) * (-C4)) * (-1);
            double oz = (C23 * (S4)) * (-1);
            /*
            double ox =  (((((C1) * (C2)) * (C3)+((C1) * (-S2)) * (S3)) * (C4)+(((-S1)) * (-1)) * (S4)) * (C5)+((((C1) * (C2)) * (-S3)+((C1) * (-S2)) * (C3))) * (S5)) * (-S6)+(((((C1) * (C2)) * (C3)+((C1) * (-S2)) * (S3)) * (S4)+(((-S1)) * (-1)) * (-C4)) * (-1)) * (C6);
            double oy = (((((S1) * (C2)) * (C3)+((S1) * (-S2)) * (S3)) * (C4)+(((C1)) * (-1)) * (S4)) * (C5)+((((S1) * (C2)) * (-S3)+((S1) * (-S2)) * (C3))) * (S5)) * (-S6)+(((((S1) * (C2)) * (C3)+((S1) * (-S2)) * (S3)) * (S4)+(((C1)) * (-1)) * (-C4)) * (-1)) * (C6);
            double oz = (((((-1) * (S2)) * (C3) + ((-1) * (C2)) * (S3)) * (C4)) * (C5) + ((((-1) * (S2)) * (-S3) + ((-1) * (C2)) * (C3))) * (S5)) * (-S6) + (((((-1) * (S2)) * (C3) + ((-1) * (C2)) * (S3)) * (S4)) * (-1)) * (C6);
             */
            return new double[] { ox, oy, oz };
        }
        public static double[] R5A(double[] Ang)
        {
            List<int> ff = new List<int>() { 2, 3, 4 };
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            //      double C2 = Math.Cos(Ang[1]);
            //      double S2 = Math.Sin(Ang[1]);
            double C3 = Math.Cos(Ang[2]);
            double S3 = Math.Sin(Ang[2]);
            double S23 = Math.Sin(Ang[1] + Ang[2]);
            double C23 = Math.Cos(Ang[1] + Ang[2]);
            double C4 = Math.Cos(Ang[3]);
            double S4 = Math.Sin(Ang[3]);
            double C5 = Math.Cos(Ang[4]);
            double S5 = Math.Sin(Ang[4]);
            double C6 = Math.Cos(Ang[5]);
            double S6 = Math.Sin(Ang[5]);
            double ax = -(C1 * S23 * C4 + S1 * S4) * S5 + C1 * C23 * C5;
            double ay = -(S1 * S23 * C4 - C1 * S4) * S5 + S1 * C23 * C5;
            double az = (-C23) * C4 * S5 - S23 * C5;
            /*
            double ax = (((((C1) * (C2)) * (C3)+((C1) * (-S2)) * (S3)) * (C4)+(((-S1)) * (-1)) * (S4)) * (-S5)+((((C1) * (C2)) * (-S3)+((C1) * (-S2)) * (C3))) * (C5));
            double ay = (((((S1) * (C2)) * (C3)+((S1) * (-S2)) * (S3)) * (C4)+(((C1)) * (-1)) * (S4)) * (-S5)+((((S1) * (C2)) * (-S3)+((S1) * (-S2)) * (C3))) * (C5));
            double az = (((((-1) * (S2)) * (C3)+((-1) * (C2)) * (S3)) * (C4)) * (-S5)+((((-1) * (S2)) * (-S3)+((-1) * (C2)) * (C3))) * (C5));
             */
            return new double[] { ax, ay, az };
        }
        public static double[] R4P(double[][] AD, double[] Ang)
        {
            double A1 = AD[0][0];
            double D1 = AD[0][1];
            double A2 = AD[1][0];
            double A3 = AD[2][0];
            double D4 = AD[3][1];
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            double C2 = Math.Cos(Ang[1]);
            double S2 = Math.Sin(Ang[1]);
            double C3 = Math.Cos(Ang[2]);
            double S3 = Math.Sin(Ang[2]);
            double S23 = Math.Sin(Ang[1] + Ang[2]);
            double C23 = Math.Cos(Ang[1] + Ang[2]);
            double C4 = Math.Cos(Ang[3]);
            double S4 = Math.Sin(Ang[3]);
            /*
            double px = (((((C1) * (C2)) * (C3)+((C1) * (-S2)) * (S3)) * (C4)+(((-S1)) * (-1)) * (S4)) * (C5)+((((C1) * (C2)) * (-S3)+((C1) * (-S2)) * (C3))) * (S5)) * (A6 * C6)+(((((C1) * (C2)) * (C3)+((C1) * (-S2)) * (S3)) * (S4)+(((-S1)) * (-1)) * (-C4)) * (-1)) * (A6 * S6)+(((((C1) * (C2)) * (C3)+((C1) * (-S2)) * (S3)) * (C4)+(((-S1)) * (-1)) * (S4)) * (-S5)+((((C1) * (C2)) * (-S3)+((C1) * (-S2)) * (C3))) * (C5)) * (D6)+(((((C1) * (C2)) * (-S3)+((C1) * (-S2)) * (C3)) * (D4)+(((C1) * (C2)) * (A3 * C3)+((C1) * (-S2)) * (A3 * S3)+((C1) * (A2 * C2)+(A1 * C1)))));
            double py = (((((S1) * (C2)) * (C3)+((S1) * (-S2)) * (S3)) * (C4)+(((C1)) * (-1)) * (S4)) * (C5)+((((S1) * (C2)) * (-S3)+((S1) * (-S2)) * (C3))) * (S5)) * (A6 * C6)+(((((S1) * (C2)) * (C3)+((S1) * (-S2)) * (S3)) * (S4)+(((C1)) * (-1)) * (-C4)) * (-1)) * (A6 * S6)+(((((S1) * (C2)) * (C3)+((S1) * (-S2)) * (S3)) * (C4)+(((C1)) * (-1)) * (S4)) * (-S5)+((((S1) * (C2)) * (-S3)+((S1) * (-S2)) * (C3))) * (C5)) * (D6)+(((((S1) * (C2)) * (-S3)+((S1) * (-S2)) * (C3)) * (D4)+(((S1) * (C2)) * (A3 * C3)+((S1) * (-S2)) * (A3 * S3)+((S1) * (A2 * C2)+(A1 * S1)))));
            double pz = (((((-1) * (S2)) * (C3)+((-1) * (C2)) * (S3)) * (C4)) * (C5)+((((-1) * (S2)) * (-S3)+((-1) * (C2)) * (C3))) * (S5)) * (A6 * C6)+(((((-1) * (S2)) * (C3)+((-1) * (C2)) * (S3)) * (S4)) * (-1)) * (A6 * S6)+(((((-1) * (S2)) * (C3)+((-1) * (C2)) * (S3)) * (C4)) * (-S5)+((((-1) * (S2)) * (-S3)+((-1) * (C2)) * (C3))) * (C5)) * (D6)+(((((-1) * (S2)) * (-S3)+((-1) * (C2)) * (C3)) * (D4)+(((-1) * (S2)) * (A3 * C3)+((-1) * (C2)) * (A3 * S3)+((-1) * (A2 * S2)))));
             */
            double px = (-C1 * (-C23)) * (D4) + (C1 * S23 * A3 + C1 * A2 * S2 + A1 * C1);
            double py = (-S1 * (-C23)) * (D4) + (S1 * S23 * A3 + S1 * A2 * S2 + A1 * S1);
            double pz = (-S23) * (D4) + (C23 * A3 + A2 * C2 + D1);
            return new double[] { px, py, pz };
        }
        public static double[] R4N(double[] Ang)
        {
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            //     double C2 = Math.Cos(Ang[1]);
            //     double S2 = Math.Sin(Ang[1]);
            double C3 = Math.Cos(Ang[2]);
            double S3 = Math.Sin(Ang[2]);
            double S23 = Math.Sin(Ang[1] + Ang[2]);
            double C23 = Math.Cos(Ang[1] + Ang[2]);
            double C4 = Math.Cos(Ang[3]);
            double S4 = Math.Sin(Ang[3]);
            double nx = (C1 * S23) * (C4) + (S1) * (S4);
            double ny = (S1 * S23) * (C4) + (-C1) * (S4);
            double nz = C23 * (C4);
            return new double[] { nx, ny, nz };
        }
        public static double[] R4O(double[] Ang)
        {
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            //    double C2 = Math.Cos(Ang[1]);
            //    double S2 = Math.Sin(Ang[1]);
            double C3 = Math.Cos(Ang[2]);
            double S3 = Math.Sin(Ang[2]);
            double S23 = Math.Sin(Ang[1] + Ang[2]);
            double C23 = Math.Cos(Ang[1] + Ang[2]);
            double C4 = Math.Cos(Ang[3]);
            double S4 = Math.Sin(Ang[3]);
            double ox = (-C1 * (-C23));
            double oy = (-S1 * (-C23));
            double oz = (-S23);
            return new double[] { ox, oy, oz };
        }
        public static double[] R4A(double[] Ang)
        {
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            //      double C2 = Math.Cos(Ang[1]);
            //      double S2 = Math.Sin(Ang[1]);
            double C3 = Math.Cos(Ang[2]);
            double S3 = Math.Sin(Ang[2]);
            double S23 = Math.Sin(Ang[1] + Ang[2]);
            double C23 = Math.Cos(Ang[1] + Ang[2]);
            double C4 = Math.Cos(Ang[3]);
            double S4 = Math.Sin(Ang[3]);
            double ax = (C1) * (S23) * (S4) - (S1) * (C4);
            double ay = (S1) * (S23) * (S4) + (C1) * (C4);
            double az = C23 * (S4);
            /*
            double ax = (((C1) * (C2)) * (C3) + ((C1) * (-S2)) * (S3)) * (S4) + (((-S1)) * (-1)) * (-C4);
            double ay = (((S1) * (C2)) * (C3) + ((S1) * (-S2)) * (S3)) * (S4) + (((C1)) * (-1)) * (-C4);
            double az = (((-1) * (S2)) * (C3) + ((-1) * (C2)) * (S3)) * (S4);
            */
            return new double[] { ax, ay, az };
        }
        public static double[] R3P(double[][] AD, double[] Ang)
        {
            double A1 = AD[0][0];
            double D1 = AD[0][1];
            double A2 = AD[1][0];
            double A3 = AD[2][0];
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            double C2 = Math.Cos(Ang[1]);
            double S2 = Math.Sin(Ang[1]);
            double C3 = Math.Cos(Ang[2]);
            double S3 = Math.Sin(Ang[2]);
            double S23 = Math.Sin(Ang[1] + Ang[2]);
            double C23 = Math.Cos(Ang[1] + Ang[2]);
            double px = (C1) * (S23) * (A3) + (C1 * A2 * S2) + (A1 * C1);
            double py = (S1) * (S23) * (A3) + (S1 * A2 * S2) + (A1 * S1);
            double pz = (C23 * A3 + A2 * C2) + (D1);
            return new double[] { px, py, pz };
        }
        public static double[] R3N(double[] Ang)
        {
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            //     double C2 = Math.Cos(Ang[1]);
            //     double S2 = Math.Sin(Ang[1]);
            double C3 = Math.Cos(Ang[2]);
            double S3 = Math.Sin(Ang[2]);
            double S23 = Math.Sin(Ang[1] + Ang[2]);
            double C23 = Math.Cos(Ang[1] + Ang[2]);
            double nx = (C1) * (S23);
            double ny = (S1) * (S23);
            double nz = C23;
            return new double[] { nx, ny, nz };
        }
        public static double[] R3O(double[] Ang)
        {
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            //    double C2 = Math.Cos(Ang[1]);
            //    double S2 = Math.Sin(Ang[1]);
            double C3 = Math.Cos(Ang[2]);
            double S3 = Math.Sin(Ang[2]);
            double S23 = Math.Sin(Ang[1] + Ang[2]);
            double C23 = Math.Cos(Ang[1] + Ang[2]);
            double ox = S1;
            double oy = -C1;
            double oz = 0;
            return new double[] { ox, oy, oz };
        }
        public static double[] R3A(double[] Ang)
        {
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            //    double C2 = Math.Cos(Ang[1]);
            //    double S2 = Math.Sin(Ang[1]);
            double C3 = Math.Cos(Ang[2]);
            double S3 = Math.Sin(Ang[2]);
            double S23 = Math.Sin(Ang[1] + Ang[2]);
            double C23 = Math.Cos(Ang[1] + Ang[2]);
            double ax = (C1) * C23;
            double ay = (S1) * C23;
            double az = (-S23);
            return new double[] { ax, ay, az };
        }

        public static double[] R2P(double[][] AD, double[] Ang)
        {
            double A1 = AD[0][0];
            double D1 = AD[0][1];
            double A2 = AD[1][0];
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            double C2 = Math.Cos(Ang[1]);
            double S2 = Math.Sin(Ang[1]);

            double px = (C1 * A2 * S2) + (A1 * C1);
            double py = (S1 * A2 * S2) + (A1 * S1);
            double pz = (A2 * C2) + (D1);
            return new double[] { px, py, pz };
        }
        public static double[] R2N(double[] Ang)
        {
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            double C2 = Math.Cos(Ang[1]);
            double S2 = Math.Sin(Ang[1]);
            double nx = (C1) * (S2);
            double ny = (S1) * (S2);
            double nz = C2;
            return new double[] { nx, ny, nz };
        }
        public static double[] R2O(double[] Ang)
        {
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            double C2 = Math.Cos(Ang[1]);
            double S2 = Math.Sin(Ang[1]);
            double ox = C1 * C2;
            double oy = S1 * C2;
            double oz = -S2;
            return new double[] { ox, oy, oz };
        }
        public static double[] R2A(double[] Ang)
        {
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            double C2 = Math.Cos(Ang[1]);
            double S2 = Math.Sin(Ang[1]);
            double ax = -S1;
            double ay = C1;
            double az = 0;
            return new double[] { ax, ay, az };
        }
        public static double[] R1P(double[][] AD, double[] Ang)
        {
            double A1 = AD[0][0];
            double D1 = AD[0][1];
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            double px = (A1 * C1);
            double py = (A1 * S1);
            double pz = (D1);
            return new double[] { px, py, pz };
        }
        public static double[] R1N(double[] Ang)
        {
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            double nx = C1;
            double ny = S1;
            double nz = 0;
            return new double[] { nx, ny, nz };
        }
        public static double[] R1O(double[] Ang)
        {
            double ox = 0;
            double oy = 0;
            double oz = -1;
            return new double[] { ox, oy, oz };
        }
        public static double[] R1A(double[] Ang)
        {
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            double ax = -(S1);
            double ay =  (C1);
            double az = 0;
            return new double[] { ax, ay, az };
        }


        static double eps = 1e-4;
        /// <summary>
        /// 六关节机器人逆运算
        /// </summary>
        /// <param name="Mat6">末端姿态及位置</param>
        /// <param name="AD">每个关节的a,d值构成的数组列表，如GSK RB8：{{150,0},{560,0},{155,0},{0,630},{0,0},{0,155}}</param>
        /// <param name="A5minus">第5轴是否为负值</param>
        /// <returns>六关节角度数组</returns>
        public static double[] InverseCal(double[,] Mat6, double[][] AD, double[] lAngs)
        {
            double A1 = AD[0][0];
            double D1 = AD[0][1];
            double A2 = AD[1][0];
            double A3 = AD[2][0];
            double D4 = AD[3][1];
            double A6 = AD[5][0];
            double D6 = AD[5][1];

            double p5x = Mat6[0, 3] - A6 * Mat6[0, 0];
            double p5y = Mat6[1, 3] - A6 * Mat6[1, 0];
            double p5z = Mat6[2, 3] - A6 * Mat6[2, 0];

            double p4x = p5x - D6 * Mat6[0, 2];
            double p4y = p5y - D6 * Mat6[1, 2];
            double p4z = p5z - D6 * Mat6[2, 2];

            double Ө1 = 0, Ө11 = 0, Ө2 = 0, Ө3 = 0, Ө4 = 0, Ө5 = 0, Ө44 = 0, Ө6 = 0;

            Ө1 = Math.Atan2(p4y, p4x);
            //if (Ө1 < 0)
            //    Ө1 += Math.PI;
            Ө11 = Ө1;
            if (p4y > 0)
            {
                if (Ө1 < 0)
                    Ө1 += Math.PI;

            }
            else
            {
                if (Math.Abs(p4y) < eps)
                {
                    if (p4x > 0)
                    {
                        Ө1 = 0;
                    }
                    else
                    {
                        Ө1 = Math.PI;
                    }
                }
                else
                {
                    if (Ө1 < 0)
                        Ө1 += 2 * Math.PI;
                    else
                        Ө1 += Math.PI;
                }
            }
            if (Ө1 > 0)
                Ө11 -= Math.PI;
            else
            {
                Ө11 += Math.PI;
            }

            if (lAngs != null)
                if (Math.Abs(Ө1 * 57.29578 - lAngs[0]) > Math.Abs(Ө11 * 57.29578 - lAngs[0]))
                    Ө1 = Ө11;

            if(Ө1>Math.PI)
            {
                Ө1 -= 2 * Math.PI;
            }
            else if(Ө1<-Math.PI)
            {
                Ө1 += 2 * Math.PI;
            }
            double p2x = (A1 * Math.Cos(Ө1));//2关节节点
            double p2y = (A1 * Math.Sin(Ө1));
            double p2z = D1;

            double l42xy = (p4x - p2x) * (p4x - p2x) + (p4y - p2y) * (p4y - p2y);
            double l422 = l42xy + (p4z - p2z) * (p4z - p2z);
            double l42 = Math.Sqrt(l422);//2-5间直线距离
            double l3 = Math.Sqrt(A3 * A3 + D4 * D4);//3-5间直线距离
            double t22 = Math.Acos((A2 * A2 + l42 * l42 - l3 * l3) / (2 * A2 * l42));//余弦定理
            double t23 = Math.Acos((A2 * A2 - l42 * l42 + l3 * l3) / (2 * A2 * l3));//余弦定理
            double t21 = Math.Asin((p4z - D1) / l42); ///l42与xy平面夹角
            if (Math.Abs(l42xy) > eps && (Math.Sign(p4x - p2x) != Math.Sign(p2x) || Math.Sign(p4y - p2y) != Math.Sign(p2y)))
            {
                t21 = Math.PI - t21;
            }

            //double l42_1 = (l422 - l3 * l3 + A2 * A2) / (2 * l42);//J2 - J3到L42垂线点距离
            //double t22 = Math.Acos(l42_1 / A2);
            //double t23 = Math.Acos((l42 - l42_1) / l3);

            Ө2 = (-(t21 + t22) + Math.PI / 2);

            double t3a = Math.Atan2(D4, A3);
            Ө3 = Math.PI - t23 - t3a;
            double S1 = Math.Sin(Ө1);
            double C1 = Math.Cos(Ө1);
            double S2 = Math.Sin(Ө2);
            double C2 = Math.Cos(Ө2);
            double S3 = Math.Sin(Ө3);
            double C3 = Math.Cos(Ө3);
            double S23 = Math.Sin(Ө2 + Ө3);
            double C23 = Math.Cos(Ө2 + Ө3);

            double n3x = C1 * S23;
            double n3y = S1 * S23;
            double n3z = C23;
            double o3x = S1;
            double o3y = -C1;
            double o3z = 0;
            double a3x = C1 * C23;
            double a3y = S1 * C23;
            double a3z = -S23;

            //            Matrix4x43d T2 = new Matrix4x43d(n3x, n3y, n3z, o3x, o3y, o3z, a3x, a3y, a3z);

            //            Matrix4x43d T3 = T2 * T1;


            double T02 = n3x * Mat6[0, 2] + n3y * Mat6[1, 2] + n3z * Mat6[2, 2];//n*a,n-Z Axis
            double T12 = o3x * Mat6[0, 2] + o3y * Mat6[1, 2] + o3z * Mat6[2, 2];//o*a,o-Y Axis
            double T22 = a3x * Mat6[0, 2] + a3y * Mat6[1, 2] + a3z * Mat6[2, 2];//a*a,a-X Axis
            double T20 = a3x * Mat6[0, 0] + a3y * Mat6[1, 0] + a3z * Mat6[2, 0];//a*n
            double T21 = a3x * Mat6[0, 1] + a3y * Mat6[1, 1] + a3z * Mat6[2, 1];//a*o
            if (Math.Abs(T22 - 1) < eps)
            {
                Ө5 = 0;
                double Tnn = n3x * Mat6[0, 0] + n3y * Mat6[1, 0] + n3z * Mat6[2, 0];
                double Ton = o3x * Mat6[0, 0] + o3y * Mat6[1, 0] + o3z * Mat6[2, 0];
                if (Math.Abs(Tnn) < eps)
                    Ө6 = 0;
                else
                {
                    Ө6 = Math.Atan2(Ton, Tnn);
                }
                /*
                if (Math.Abs(Tnn) < eps)
                    Ө4 = 0;
                else
                    Ө4 = Math.Atan2(Tnn);
                */
            }
            else
            {
                Ө4 = Math.Atan2(T12, T02);
                if (Math.Abs(T12) < eps)
                    Ө4 = 0;
                if (Ө4 < 0)
                {
                    Ө44 = Math.PI + Ө4;
                }
                else
                {
                    Ө44 = Ө4 - Math.PI;
                }
                if (Math.Abs(Ө4 * 57.29578 - lAngs[3]) > Math.Abs(Ө44 * 57.29578 - lAngs[3]))
                    Ө4 = Ө44;
                double C4 = Math.Cos(Ө4);
                double S4 = Math.Sin(Ө4);

                Ө5 = -Math.Atan2(T02 * C4 + T12 * S4, T22);
                Ө6 = -Math.Atan2(T21, T20);

                if (Ө5 < 0)
                {
                    if (Ө6 < 0)
                        Ө6 = Math.PI + Ө6;
                    else
                        Ө6 = -Math.PI + Ө6;
                }
                if (Math.Abs(T21) < eps)
                    Ө6 = 0;
            }
            return new double[] { Ө1 * 57.29578f, Ө2 * 57.29578f, Ө3 * 57.29578f, Ө4 * 57.29578f, Ө5 * 57.29578f, Ө6 * 57.29578f };
        }
        public static double[] InverseCal(double[] r6n, double[] r6o, double[] r6a, double[] r6p, double[][] AD, double[] lAngs)
        {
            if (lAngs == null)
                lAngs = new double[] { 0, 0, 0, 0, 0, 0 };
            double A1 = AD[0][0];
            double D1 = AD[0][1];
            double A2 = AD[1][0];
            double A3 = AD[2][0];
            double D4 = AD[3][1];
            double A6 = AD[5][0];
            double D6 = AD[5][1];
            double Ө1 = 0, Ө11 = 0, Ө2 = 0, Ө3 = 0, Ө4 = 0, Ө5 = 0, Ө44 = 0, Ө6 = 0;
            double p5x = r6p[0] - A6 * r6n[0];//6关节位置
            double p5y = r6p[1] - A6 * r6n[1];
            double p5z = r6p[2] - A6 * r6n[2];

            double p4x = p5x - D6 * r6a[0];//5关节节点
            double p4y = p5y - D6 * r6a[1];
            double p4z = p5z - D6 * r6a[2];

            Ө1 = Math.Atan2(p4y, p4x);
            if (Ө1 < 0)
                Ө1 += Math.PI;
            Ө11 = Ө1;
            if (p4y > 0)
            {
                if (Ө1 < 0)
                    Ө1 += Math.PI;
            }
            else
            {
                if (Math.Abs(p4y) < eps)
                {
                    if (p4x > 0)
                    {
                        Ө1 = 0;
                    }
                    else
                    {
                        Ө1 = Math.PI;
                    }
                }
                else
                {
                    if (Ө1 < 0)
                        Ө1 += 2 * Math.PI;
                    else
                        Ө1 += Math.PI;
                }
            }
            if (Ө1 > 0)
                Ө11 -= Math.PI;
            else
            {
                Ө11 += Math.PI;
            }
            if (lAngs != null)
                if (Math.Abs(Ө1 * 57.29578 - lAngs[0]) > Math.Abs(Ө11 * 57.29578 - lAngs[0]))
                    Ө1 = Ө11;

            if (Ө1 > Math.PI)
            {
                Ө1 -= 2 * Math.PI;
            }
            //else if (Ө1 < -Math.PI)
            //{
            //    Ө1 += 2 * Math.PI;
            //}

            double p2x = (A1 * Math.Cos(Ө1));//2关节节点
            double p2y = (A1 * Math.Sin(Ө1));
            double p2z = D1;

            double l42xy = (p4x - p2x) * (p4x - p2x) + (p4y - p2y) * (p4y - p2y);
            double l422 = l42xy + (p4z - p2z) * (p4z - p2z);
            double l42 = Math.Sqrt(l422);//2-5间直线距离
            double l3 = Math.Sqrt(A3 * A3 + D4 * D4);//3-5间直线距离
            double t22 = Math.Acos((A2 * A2 + l42 * l42 - l3 * l3) / (2 * A2 * l42));//余弦定理
            double t23 = Math.Acos((A2 * A2 - l42 * l42 + l3 * l3) / (2 * A2 * l3));//余弦定理
                                                                                    //double t21 = Math.Asin((p4z - D1) / l42); ///l42与xy平面夹角
            double l40xy = Math.Sqrt(p4x * p4x + p4y * p4y)-A1;

            double t21 = Math.Atan2((p4z - D1), l40xy); ///l42与xy平面夹角
            //if (Math.Abs(l42xy) > eps && (Math.Sign(p4x - p2x) != Math.Sign(p2x) || Math.Sign(p4y - p2y) != Math.Sign(p2y)))
            //{
            //    t21 = Math.PI - t21;
            //}


            /*
                double l42xy = (p4x - p2x) * (p4x - p2x) + (p4y - p2y) * (p4y - p2y);
            double l422 = l42xy + (p4z - p2z) * (p4z - p2z);
            double l4xy = p4x * p4x + p4y * p4y;
            double l2xy = p2x * p2x + p2y * p2y;
            double dl42xy = l4xy - l2xy;
            double l42 = Math.Sqrt(l422);//2-5间直线距离
            double t21 = Math.Atan2((p4z - D1), (Math.Sqrt(l42xy)-A1)); ///l42与xy平面夹角
            double l3 = Math.Sqrt(A3 * A3 + D4 * D4);//3-5间直线距离
            if (Math.Abs(l42 - (A2 + Math.Sqrt(A3 * A3 + D4 * D4))-A1) < eps)
            {
                Ө2 = (-(t21) + Math.PI / 2);
                Ө3 = Math.Atan2(D4, A3);
            }
            else
            {
                double t22 = Math.Acos((A2 * A2 + l42 * l42 - l3 * l3) / (2 * A2 * l42));//余弦定理
                double t23 = Math.Acos((A2 * A2 - l42 * l42 + l3 * l3) / (2 * A2 * l3));//余弦定理
                //if (Math.Abs(l42xy) > eps && (Math.Sign(p4x - p2x) != Math.Sign(p2x) || Math.Sign(p4y - p2y) != Math.Sign(p2y)))
                //{
                //    t21 = Math.PI - t21;
                //}
                //double l42_1 = (l422 - l3 * l3 + A2 * A2) / (2 * l42);//J2 - J3到L42垂线点距离
                //double t22 = Math.Acos(l42_1 / A2);
                //double t23 = Math.Acos((l42 - l42_1) / l3);

                Ө2 = (-(t21 + t22) + Math.PI / 2);

                double t3a = Math.Atan2(D4, A3);
                Ө3 = Math.PI - t23 - t3a;
            }*/


            //double l42_1 = (l422 - l3 * l3 + A2 * A2) / (2 * l42);//J2 - J3到L42垂线点距离
            //double t22 = Math.Acos(l42_1 / A2);
            //double t23 = Math.Acos((l42 - l42_1) / l3);

            Ө2 = (-(t21 + t22) + Math.PI / 2);

            double t3a = Math.Atan2(D4, A3);
            Ө3 = Math.PI - t23 - t3a;
            double S1 = Math.Sin(Ө1);
            double C1 = Math.Cos(Ө1);
            //double S2 = Math.Sin(Ө2);
            //double C2 = Math.Cos(Ө2);
            //double S3 = Math.Sin(Ө3);
            //double C3 = Math.Cos(Ө3);
            double S23 = Math.Sin(Ө2 + Ө3);
            double C23 = Math.Cos(Ө2 + Ө3);

            double n3x = C1 * S23;
            double n3y = S1 * S23;
            double n3z = C23;
            double o3x = S1;
            double o3y = -C1;
            double o3z = 0;
            double a3x = C1 * C23;
            double a3y = S1 * C23;
            double a3z = -S23;

            double T22 = a3x * r6a[0] + a3y * r6a[1] + a3z * r6a[2];
            double T02 = n3x * r6a[0] + n3y * r6a[1] + n3z * r6a[2];
            double T12 = o3x * r6a[0] + o3y * r6a[1] + o3z * r6a[2];
            double T20 = a3x * r6n[0] + a3y * r6n[1] + a3z * r6n[2];
            double T21 = a3x * r6o[0] + a3y * r6o[1] + a3z * r6o[2];

            if (Math.Abs(T22 - 1) < eps)
            {
                Ө5 = 0;
                double Tnn = n3x * r6n[0] + n3y * r6n[1] + n3z * r6n[2];
                double Ton = o3x * r6n[0] + o3y * r6n[1] + o3z * r6n[2];
                if (Math.Abs(Tnn) < eps)
                    Ө6 = 0;
                else
                {
                    Ө6 = Math.Atan2(Ton, Tnn);
                }

                //if (Math.Abs(Tnn) < eps)
                //    Ө4 = 0;
                //else
                //    Ө4 = Math.Atan2(Tnn);

            }
            else
            {
                Ө4 = Math.Atan2(T12, T02);
                if (Math.Abs(T12) < eps)
                    Ө4 = 0;
                if (Ө4 < 0)
                {
                    Ө44 = Math.PI + Ө4;
                }
                else if (Ө4 != 0)
                {
                    Ө44 = Ө4 - Math.PI;
                }
                if (lAngs != null)
                    if (Math.Abs(Ө4 * 57.29578 - lAngs[3]) > Math.Abs(Ө44 * 57.29578 - lAngs[3]))
                        Ө4 = Ө44;
                double C4 = Math.Cos(Ө4);
                double S4 = Math.Sin(Ө4);

                Ө5 = -Math.Atan2(T02 * C4 + T12 * S4, T22);

                Ө6 = -Math.Atan2(T21, T20);

                if (Ө5 < 0)
                {
                    if (Ө6 < 0)
                        Ө6 = Math.PI + Ө6;
                    else
                        Ө6 = -Math.PI + Ө6;
                }
                if (Math.Abs(T21) < eps)
                    Ө6 = 0;
            }
            return new double[] { Ө1 * 57.29578f, Ө2 * 57.29578f, Ө3 * 57.29578f, Ө4 * 57.29578f, Ө5 * 57.29578f, Ө6 * 57.29578f };
        }
        /// <summary>
        /// 六关节机器人前五关节雅可比矩阵
        /// </summary>
        /// <param name="AD">每个关节的a,d值构成的数组列表，如GSK RB8：{{150,0},{560,0},{155,0},{0,630},{0,0},{0,155}}</param>
        /// <param name="Ang">每个关节的角度值数组</param>
        /// <returns>6X6矩阵</returns>
        public static double[,] Jacobian5(double[][] AD, double[] Ang)
        {
            double A1 = AD[0][0];
            double D1 = AD[0][1];
            double A2 = AD[1][0];
            double A3 = AD[2][0];
            double D4 = AD[3][1];
            double A6 = 0;
            double D6 = AD[5][1];
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            double C2 = Math.Cos(Ang[1]);
            double S2 = Math.Sin(Ang[1]);
            double C3 = Math.Cos(Ang[2]);
            double S3 = Math.Sin(Ang[2]);
            double S23 = Math.Sin(Ang[1] + Ang[2]);
            double C23 = Math.Cos(Ang[1] + Ang[2]);
            double C4 = Math.Cos(Ang[3]);
            double S4 = Math.Sin(Ang[3]);
            double C5 = Math.Cos(Ang[4]);
            double S5 = Math.Sin(Ang[4]);
            double C6 = 1;
            double S6 = 0;

            double J11 = -((-S1 * S23 * C4 + C1 * S4) * S5 + S1 * C23 * C5) * D6 - S1 * C23 * D4 - S1 * S23 * A3 - S1 * A2 * S2 - A1 * S1;
            double J12 = -(C1 * C23 * C4 * S5 + C1 * S23 * C5) * D6 - C1 * S23 * D4 + C1 * C23 * A3 + C1 * A2 * C2;
            double J13 = -(C1 * C23 * C4 * S5 + C1 * S23 * C5) * D6 - C1 * S23 * D4 + C1 * C23 * A3;
            double J14 = ((C1 * S23 * S4 - S1 * C4) * S5) * D6;
            double J15 = -((C1 * S23 * C4 + S1 * S4) * C5 + C1 * C23 * S5) * D6;

            double J21 = ((-C1 * S23 * C4 - S1 * S4) * S5 + C1 * C23 * C5) * D6 + C1 * C23 * D4 + C1 * S23 * A3 + C1 * A2 * S2 + A1 * C1;
            double J22 = (-S1 * C23 * C4 * S5 - S1 * S23 * C5) * D6 - S1 * S23 * D4 + S1 * C23 * A3 + S1 * A2 * C2;
            double J23 = (-S1 * C23 * C4 * S5 - S1 * S23 * C5) * D6 - S1 * S23 * D4 + S1 * C23 * A3;
            double J24 = (S1 * S23 * S4 + C1 * C4) * S5 * D6;
            double J25 = ((-S1 * S23 * C4 + C1 * S4) * C5 - S1 * C23 * S5) * D6;

            double J31 = 0;
            double J32 = (S23 * C4 * S5 - C23 * C5) * D6 - C23 * D4 - S23 * A3 - A2 * S2;
            double J33 = (S23 * C4 * S5 - C23 * C5) * D6 - C23 * D4 - S23 * A3;
            double J34 = C23 * S4 * S5 * D6;
            double J35 = -(C23 * C4 * C5 - S23 * S5) * D6;

            double J41 = 0;
            double J42 = -S1;
            double J43 = -S1;
            double J44 = C1 * C23;
            double J45 = C1 * S23 * S4 - S1 * C4;
            double J46 = (C1 * S23 * C4 + S1 * S4) * (-S5) + C1 * C23 * C5;

            double J51 = 0;
            double J52 = C1;
            double J53 = C1;
            double J54 = S1 * C23;
            double J55 = S1 * S23 * S4 + C1 * C4;
            double J56 = (S1 * S23 * C4 - C1 * S4) * (-S5) + S1 * C23 * C5;

            //double J61 = 1;
            //double J62 = 0;
            //double J63 = 0;
            //double J64 = -S23;
            //double J65 = C23 * S4;
            //double J66 = -(C23 * C4 * S5 + S23 * C5);
            double[,] Mat = new double[,] { { J11, J12, J13, J14, J15 }, { J21, J22, J23, J24, J25 }, { J31, J32, J33, J34, J35 }, { J41, J42, J43, J44, J45 }, { J51, J52, J53, J54, J55 } };
            return Mat;
        }
        /// <summary>
        ///         /// <summary>
        /// 六关节机器人雅可比矩阵
        /// </summary>
        /// <param name="AD">每个关节的a,d值构成的数组列表，如GSK RB8：{{150,0},{560,0},{155,0},{0,630},{0,0},{0,155}}</param>
        /// <param name="Ang">每个关节的角度值数组</param>
        /// <returns>6X6矩阵</returns>
        public static double[,] Jacobian(double[][] AD, double[] Ang)
        {
            double A1 = AD[0][0];
            double D1 = AD[0][1];
            double A2 = AD[1][0];
            double A3 = AD[2][0];
            double D4 = AD[3][1];
            double A6 = AD[5][0];
            double D6 = AD[5][1];
            double C1 = Math.Cos(Ang[0]);
            double S1 = Math.Sin(Ang[0]);
            double C2 = Math.Cos(Ang[1]);
            double S2 = Math.Sin(Ang[1]);
            double C3 = Math.Cos(Ang[2]);
            double S3 = Math.Sin(Ang[2]);
            double S23 = Math.Sin(Ang[1] + Ang[2]);
            double C23 = Math.Cos(Ang[1] + Ang[2]);
            double C4 = Math.Cos(Ang[3]);
            double S4 = Math.Sin(Ang[3]);
            double C5 = Math.Cos(Ang[4]);
            double S5 = Math.Sin(Ang[4]);
            double C6 = Math.Cos(Ang[5]);
            double S6 = Math.Sin(Ang[5]);

            double J11 = ((-S1 * S23 * C4 + C1 * S4) * C5 - S1 * C23 * S5) * A6 * C6 + (S1 * S23 * S4 + C1 * C4) * A6 * S6 - ((-S1 * S23 * C4 + C1 * S4) * S5 + S1 * C23 * C5) * D6 - S1 * C23 * D4 - S1 * S23 * A3 - S1 * A2 * S2 - A1 * S1;
            double J12 = (C1 * C23 * C4 * C5 - C1 * S23 * S5) * A6 * C6 - C1 * C23 * S4 * A6 * S6 - (C1 * C23 * C4 * S5 + C1 * S23 * C5) * D6 - C1 * S23 * D4 + C1 * C23 * A3 + C1 * A2 * C2;
            double J13 = (C1 * C23 * C4 * C5 - C1 * S23 * S5) * A6 * C6 - C1 * C23 * S4 * A6 * S6 - (C1 * C23 * C4 * S5 + C1 * S23 * C5) * D6 - C1 * S23 * D4 + C1 * C23 * A3;
            double J14 = (-C1 * S23 * S4 + S1 * C4) * C5 * A6 * C6 - (C1 * S23 * C4 + S1 * S4) * A6 * S6 + ((C1 * S23 * S4 - S1 * C4) * S5) * D6;
            double J15 = ((C1 * S23 * C4 + S1 * S4) * (-S5) + C1 * C23 * C5) * A6 * C6 - ((C1 * S23 * C4 + S1 * S4) * C5 + C1 * C23 * S5) * D6;
            double J16 = ((C1 * S23 * C4 + S1 * S4) * C5 + C1 * C23 * S5) * A6 * (-S6) - (C1 * S23 * S4 - S1 * C4) * A6 * C6;

            double J21 = ((C1 * S23 * C4 + S1 * S4) * C5 + C1 * C23 * S5) * A6 * C6 - (C1 * S23 * S4 - S1 * C4) * A6 * S6 + ((-C1 * S23 * C4 - S1 * S4) * S5 + C1 * C23 * C5) * D6 + C1 * C23 * D4 + C1 * S23 * A3 + C1 * A2 * S2 + A1 * C1;
            double J22 = (S1 * C23 * C4 * C5 - S1 * S23 * S5) * A6 * C6 - S1 * C23 * S4 * A6 * S6 + (-S1 * C23 * C4 * S5 - S1 * S23 * C5) * D6 - S1 * S23 * D4 + S1 * C23 * A3 + S1 * A2 * C2;
            double J23 = (S1 * C23 * C4 * C5 - S1 * S23 * S5) * A6 * C6 - S1 * C23 * S4 * A6 * S6 + (-S1 * C23 * C4 * S5 - S1 * S23 * C5) * D6 - S1 * S23 * D4 + S1 * C23 * A3;
            double J24 = (-S1 * S23 * S4 - C1 * C4) * C5 * A6 * C6 - (S1 * S23 * C4 - C1 * S4) * A6 * S6 + (S1 * S23 * S4 + C1 * C4) * S5 * D6;
            double J25 = ((S1 * S23 * C4 - C1 * S4) * (-S5) + S1 * C23 * C5) * A6 * C6 + ((-S1 * S23 * C4 + C1 * S4) * C5 - S1 * C23 * S5) * D6;
            double J26 = ((S1 * S23 * C4 - C1 * S4) * C5 + S1 * C23 * S5) * A6 * (-S6) - (S1 * S23 * S4 + C1 * C4) * A6 * C6;

            double J31 = 0;
            double J32 = -(S23 * C4 * C5 + C23 * S5) * A6 * C6 + S23 * S4 * A6 * S6 + (S23 * C4 * S5 - C23 * C5) * D6 - C23 * D4 - S23 * A3 - A2 * S2;
            double J33 = -(S23 * C4 * C5 + C23 * S5) * A6 * C6 + S23 * S4 * A6 * S6 + (S23 * C4 * S5 - C23 * C5) * D6 - C23 * D4 - S23 * A3;
            double J34 = -(C23 * S4 * C5) * A6 * C6 - C23 * C4 * A6 * S6 + C23 * S4 * S5 * D6;
            double J35 = -(C23 * C4 * S5 + S23 * C5) * A6 * C6 - (C23 * C4 * C5 - S23 * S5) * D6;
            double J36 = (-C23 * C4 * C5 + S23 * S5) * A6 * S6 - C23 * S4 * A6 * C6;

            double J41 = 0;
            double J42 = -S1;
            double J43 = -S1;
            double J44 = C1 * C23;
            double J45 = C1 * S23 * S4 - S1 * C4;
            double J46 = (C1 * S23 * C4 + S1 * S4) * (-S5) + C1 * C23 * C5;

            double J51 = 0;
            double J52 = C1;
            double J53 = C1;
            double J54 = S1 * C23;
            double J55 = S1 * S23 * S4 + C1 * C4;
            double J56 = (S1 * S23 * C4 - C1 * S4) * (-S5) + S1 * C23 * C5;

            double J61 = 1;
            double J62 = 0;
            double J63 = 0;
            double J64 = -S23;
            double J65 = C23 * S4;
            double J66 = -(C23 * C4 * S5 + S23 * C5);
            double[,] Mat = new double[,] { { J11, J12, J13, J14, J15, J16 }, { J21, J22, J23, J24, J25, J26 }, { J31, J32, J33, J34, J35, J36 }, { J41, J42, J43, J44, J45, J46 }, { J51, J52, J53, J54, J55, J56 }, { J61, J62, J63, J64, J65, J66 } };
            return Mat;
        }
        /// <summary>
        /// 六关节机器人关节微分运算
        /// </summary>
        /// <param name="AD">每个关节的a,d值构成的数组列表，如GSK RB8：{{150,0},{560,0},{155,0},{0,630},{0,0},{0,155}}</param>
        /// <param name="Ang">每个关节的角度值</param>
        /// <param name="diffMat">末端空间位置姿态微分数组[dpx,dpy,dpz,δx,δy,δz]</param>
        /// <returns>六关节角度微分数组[dӨ1,dӨ2,dӨ3,dӨ4,dӨ5,dӨ6]</returns>
        public static double[] Differential(double[][] AD, double[] Ang, double[] diffMat)
        {
            double[] A = diffMat;
            double[,] JacobianMat = Jacobian(AD, Ang);
            for (int i = 0; i < 6; i++)
            {
                double m = JacobianMat[i, i];
                for (int j = i; j < 6; j++)
                {
                    JacobianMat[i, j] /= m;
                }
                A[i] /= m;
                for (int k = 0; k < 6; k++)
                {
                    if (k != i)
                    {
                        m = JacobianMat[k, i];
                        for (int l = i; l < 6; l++)
                        {
                            JacobianMat[k, l] -= m * JacobianMat[i, l];
                        }
                        A[k] -= m * A[i];
                    }
                }
            }
            return A;
        }

        /// <summary>
        /// 六关节机器人关节微分运算
        /// </summary>
        /// <param name="AD">每个关节的a,d值构成的数组列表，如GSK RB8：{{150,0},{560,0},{155,0},{0,630},{0,0},{0,155}}</param>
        /// <param name="JacobianMat">雅可比矩阵</param>
        /// <param name="diffMat">末端空间位置姿态微分数组[dpx,dpy,dpz,δx,δy,δz]</param>
        /// <returns>六关节角度微分数组[dӨ1,dӨ2,dӨ3,dӨ4,dӨ5,dӨ6]</returns>
        public static double[] Differential(double[,] JacobianMat, double[] diffMat)
        {
            double[] A = diffMat;
            for (int i = 0; i < 6; i++)
            {
                double m = JacobianMat[i, i];
                for (int j = i; j < 6; j++)
                {
                    JacobianMat[i, j] /= m;
                }
                A[i] /= m;
                for (int k = 0; k < 6; k++)
                {
                    if (k != i)
                    {
                        m = JacobianMat[k, i];
                        for (int l = i; l < 6; l++)
                        {
                            JacobianMat[k, l] -= m * JacobianMat[i, l];
                        }
                        A[k] -= m * A[i];
                    }
                }
            }
            return A;
        }

        /// <summary>
        /// 位姿向量组转换为欧拉角RotZ-RotY'-RotX'
        /// </summary>
        /// <param name="n">n 向量</param>
        /// <param name="o">o 向量</param>
        /// <param name="a">a 向量</param>
        /// <param name="Result">欧拉角度数组[δx-α ,δy-β,δz-γ]</param>
        public static double[] VectorToEuler(double[] n, double[] o, double[] a)
        {
            double sx = Math.Atan2(n[1], n[0]);
            sx = sx < 0 ? sx + Math.PI : sx;
            double sy = Math.Atan2((-n[2]), (n[0] * Math.Cos(sx) + n[1] * Math.Sin(sx)));
            double sz = Math.Atan2((a[0] * Math.Sin(sx) - a[1] * Math.Cos(sx)), (-o[0] * Math.Sin(sx) + o[1] * Math.Cos(sx)));
            sz = sz < 0 ? sz + Math.PI : sz;

            return new double[] { sx * 57.29578, sy * 57.29578, sz * 57.29578 };
        }
        /// <summary>
        /// 位姿向量组转换为欧拉角RotZ-RotY'-RotX'
        /// </summary>
        /// <param name="euler">欧拉角[δx-α ,δy-β,δz-γ]</param>
        /// <param name="Result">向量矩阵[n,o,a]</param>
        public static double[,] EulerToVector(double[] euler)
        {
            double α = euler[0] / 57.29578;
            double β = euler[1] / 57.29578;
            double γ = euler[2] / 57.29578;
            double cx = Math.Cos(-γ);
            double sx = Math.Sin(-γ);
            double cy = Math.Cos(-β);
            double sy = Math.Sin(-β);
            double cz = Math.Cos(-α);
            double sz = Math.Sin(-α);
            return new double[,] { { cy * cz, sx * sy * cz + cx * sz,  -cx * sy * cz + sx * sz}, {-cy * sz,  -sx * sy * sz + cx * cz, cx * sy * sz + sx * cz }, {sy , -sx * cy, cx * cy } };
        }

        /// <summary>
        /// 位姿向量组转换为框架角RotZ-RotY'-RotZ'
        /// </summary>
        /// <param name="n">n 向量</param>
        /// <param name="o">o 向量</param>
        /// <param name="a">a 向量</param>
        /// <param name="Result">框架角度数组[δx-roll,δy-pitch,δz-yaw]</param>
        public static double[] VectorToEuler2(double[] n, double[] o, double[] a)
        {
            double sx = Math.Atan2(a[1], a[0]);
            sx = sx < 0 ? sx + Math.PI : sx;
            double sy = Math.Atan2((a[0] * Math.Cos(sx) + a[1] * Math.Sin(sx)), a[2]);
            double sz = Math.Atan2((-n[0] * Math.Sin(sx) + n[1] * Math.Cos(sx)), (-o[0] * Math.Sin(sx) + o[1] * Math.Cos(sx)));
            return new double[] { sx * 57.29578, sy * 57.29578, sz * 57.29578 };
        }
        /// <summary>
        /// 位姿向量组转换为欧拉角RotZ-RotY'-RotZ'
        /// </summary>
        /// <param name="euler">欧拉角[δx-α ,δy-β,δz-γ]</param>
        /// <param name="Result">向量矩阵[n,o,a]</param>
        public static double[,] EulerToVector2(double[] euler)
        {
            double α = euler[0] / 57.29578;
            double β = euler[1] / 57.29578;
            double γ = euler[2] / 57.29578;
            double cx = Math.Cos(-γ);
            double sx = Math.Sin(-γ);
            double cy = Math.Cos(-β);
            double sy = Math.Sin(-β);
            double cz = Math.Cos(-α);
            double sz = Math.Sin(-α);
            return new double[,] { { cx * cy * cz - sx * sz, sx * cy * cz + cx * sz,-sy * cz  }, {-cx * cy * sz - sx * cz,  -sx * cy * sz + cx * cz,  sy * sz }, {cx * sy, sx * sy, cy } };
        }
        /// <summary>
        /// 求矩阵行列式值Determinant
        /// </summary>
        /// <param name="Mat">矩阵</param>
        /// <param name="Result">行列式值</param>
        public static double Det(double[,] Mat)
        {
            int S, k, k1, i, j;
            double[,] DArray;
            double save, ArrayK, tmpDet;
            int Rows = Mat.GetUpperBound(0), Cols = Mat.GetUpperBound(0);
            DArray = (double[,])Mat.Clone();

            if (Rows != Cols) { };
            S = Rows;
            tmpDet = 1;

            for (k = 0; k <= S; k++)
            {
                if (DArray[k, k] == 0)
                {
                    j = k;
                    while ((j < S) && (DArray[k, j] == 0)) j = j + 1;
                    if (DArray[k, j] == 0) return 0;
                    else
                    {
                        for (i = k; i <= S; i++)
                        {
                            save = DArray[i, j];
                            DArray[i, j] = DArray[i, k];
                            DArray[i, k] = save;
                        }
                    }
                    tmpDet = -tmpDet;
                }
                ArrayK = DArray[k, k];
                tmpDet = tmpDet * ArrayK;
                if (k < S)
                {
                    k1 = k + 1;
                    for (i = k1; i <= S; i++)
                    {
                        for (j = k1; j <= S; j++)
                            DArray[i, j] = DArray[i, j] - DArray[i, k] * (DArray[k, j] / ArrayK);
                    }
                }
            }
            return tmpDet;
        }

        public void rotate(double[,] mf)
        {
            double rotateX = 0, rotateY = 0, rotateZ = 0;
            if (mf[0,1] > 0.998)
            { // singularity at north pole

                rotateY = (float)(Math.Atan2(mf[2,0], mf[2,2]) * 180 / 3.1415926f);

                rotateZ = (float)(Math.PI / 2 * 180 / 3.1415926f);

                rotateX = 0;

                return;

            }

            if (mf[0,1] < -0.998)
            { // singularity at south pole

                rotateY = (float)(Math.Atan2(mf[2,0], mf[2,2]) * 180 / 3.1415926f);

                rotateZ = (float)(-Math.PI / 2 * 180 / 3.1415926f);

                rotateX = 0;

                return;

            }

            rotateY = (float)(Math.Atan2(-mf[0,2], mf[0,0]) * 180 / 3.1415926f);

            rotateX = (float)(Math.Atan2(-mf[2,1], mf[1,1]) * 180 / 3.1415926f);

            rotateZ = (float)(Math.Asin(mf[0,1]) * 180 / 3.1415926f);

        }



        public double toAxisAngle(double[,] m)
        {

            double angle = 0, x, y, z; // variables for result

            double epsilon = 0.01; // margin to allow for rounding errors

            double epsilon2 = 0.1; // margin to distinguish between 0 and 180 degrees

            double rotate = 0;

            if ((Math.Abs(m[0,2] - m[0,1]) < epsilon)

              && (Math.Abs(m[2,0] - m[0,2]) < epsilon)

              && (Math.Abs(m[2,1] - m[1,2]) < epsilon))
            {

                // singularity found

                // first check for identity matrix which must have +1 for all terms

                //  in leading diagonaland zero in other terms

                if ((Math.Abs(m[1,0] + m[0,1]) < epsilon2)

                  && (Math.Abs(m[2,0] + m[0,2]) < epsilon2)

                  && (Math.Abs(m[2,1] + m[1,2]) < epsilon2)

                  && (Math.Abs(m[0,0] + m[1,1] + m[2,2] - 3) < epsilon2))
                {

                    // this singularity is identity matrix so angle = 0

                    rotate = (float)(angle * 180 / 3.1415926f);

                    //Log.e("webview", "angle" + angle);

                    return rotate;//new axisAngle(0,1,0,0); // zero angle, arbitrary axis

                }

                // otherwise this singularity is angle = 180

                angle = Math.PI;

                double xx = (m[0,0] + 1) / 2;

                double yy = (m[1,1] + 1) / 2;

                double zz = (m[2,2] + 1) / 2;

                double xy = (m[1,0] + m[0,1]) / 4;

                double xz = (m[2,0] + m[0,2]) / 4;

                double yz = (m[2,1] + m[1,2]) / 4;

                if ((xx > yy) && (xx > zz))
                { // m[0,0][0] is the largest diagonal term

                    if (xx < epsilon)
                    {

                        x = 0;

                        y = 0.7071;

                        z = 0.7071;

                    }
                    else
                    {

                        x = Math.Sqrt(xx);

                        y = xy / x;

                        z = xz / x;

                    }

                }
                else if (yy > zz)
                { // m[0,1][1] is the largest diagonal term

                    if (yy < epsilon)
                    {

                        x = 0.7071;

                        y = 0;

                        z = 0.7071;

                    }
                    else
                    {

                        y = Math.Sqrt(yy);

                        x = xy / y;

                        z = yz / y;

                    }

                }
                else
                { // m[0,2][2] is the largest diagonal term so base result on this

                    if (zz < epsilon)
                    {

                        x = 0.7071;

                        y = 0.7071;

                        z = 0;

                    }
                    else
                    {

                        z = Math.Sqrt(zz);

                        x = xz / z;

                        y = yz / z;

                    }

                }

               rotate = (float)(angle * 180 / 3.1415926f);

                //Log.e("webview", "angle" + angle);

                return rotate;//new axisAngle(angle,x,y,z); // return 180 deg rotation

            }

            // as we have reached here there are no singularities so we can handle normally

            double s = Math.Sqrt((m[1,2] - m[2,1]) * (m[1,2] - m[2,1])

            + (m[2,0] - m[0,2]) * (m[2,0] - m[0,2])

            + (m[0,1] - m[1,0]) * (m[0,1] - m[1,0])); // used to normalise

            if (Math.Abs(s) < 0.001) s = 1;

            // prevent divide by zero, should not happen if matrix is orthogonal and should be

            // caught by singularity test above, but I've left it in just in case

            angle = Math.Acos((m[0,0] + m[1,1] + m[2,2] - 1) / 2);

            x = (m[1,2] - m[2,1]) / s;

            y = (m[2,0] - m[0,2]) / s;

            z = (m[0,1] - m[1,0]) / s;

            rotate = (float)(angle * 180 / 3.1415926f);

            //Log.e("webview", "angle" + angle);

            return rotate;//new axisAngle(angle,x,y,z);

        }
        //四元数转旋转矩阵 Hamilton表达右手系
        double[] getRotation(double[] Quaternion)
        {
            double[] rt_mat = new double[9];
            rt_mat[0] = 1 - 2 * (Quaternion[2] * Quaternion[2]) - 2 * (Quaternion[3] * Quaternion[3]);
            rt_mat[1] = 2 * Quaternion[1] * Quaternion[2] - 2 * Quaternion[0] * Quaternion[3];
            rt_mat[2] = 2 * Quaternion[1] * Quaternion[3] + 2 * Quaternion[0] * Quaternion[2];
            rt_mat[3] = 2 * Quaternion[1] * Quaternion[2] + 2 * Quaternion[0] * Quaternion[3];
            rt_mat[4] = 1 - 2 * (Quaternion[1] * Quaternion[1]) - 2 * (Quaternion[3] * Quaternion[3]);
            rt_mat[5] = 2 * Quaternion[2] * Quaternion[3] - 2 * Quaternion[0] * Quaternion[1];
            rt_mat[6] = 2 * Quaternion[1] * Quaternion[3] - 2 * Quaternion[0] * Quaternion[2];
            rt_mat[7] = 2 * Quaternion[2] * Quaternion[3] + 2 * Quaternion[0] * Quaternion[1];
            rt_mat[8] = 1 - 2 * (Quaternion[1] * Quaternion[1]) - 2 * (Quaternion[2] * Quaternion[2]);
            return rt_mat;
        }
        //旋转矩阵转四元数
        double[] getQuaternion(double[,] R)
        {
            double[] Q = new double[9];
            double trace = R[0, 0] + R[1, 1] + R[2, 2];

            if (trace > 0.0)
            {
                double s = Math.Sqrt(trace + 1.0);
                Q[3] = (s * 0.5);
                s = 0.5 / s;
                Q[0] = (R[2, 1] - R[1, 2]) * s;
                Q[1] = (R[0, 2] - R[2, 0]) * s;
                Q[2] = (R[1, 0] - R[0, 1]) * s;
            }

            else
            {
                int i = R[0, 0] < R[1, 1] ? (R[1, 1] < R[2, 2] ? 2 : 1) : (R[0, 0] < R[2, 2] ? 2 : 0);
                int j = (i + 1) % 3;
                int k = (i + 2) % 3;

                double s = Math.Sqrt(R[i, i] - R[j, j] - R[k, k] + 1.0);
                Q[i] = s * 0.5;
                s = 0.5 / s;

                Q[3] = (R[k, j] - R[j, k]) * s;
                Q[j] = (R[j, i] + R[i, j]) * s;
                Q[k] = (R[k, i] + R[i, k]) * s;
            }
            return Q;
        }
        //旋转矩阵转四元数 JPL表达左手系
        double[] getQuaternion2(double[,] R)//JPL
        {
            double[] Q = new double[9];
            double m11 = R[0, 0], m12 = R[0, 1], m13 = R[0, 2];
            double m21 = R[1, 0], m22 = R[1, 1], m23 = R[1, 2];
            double m31 = R[2, 0], m32 = R[2, 1], m33 = R[2, 2];

            double w=0, x=0, y=0, z=0;

            //探测四元数中最大的项 
            double fourWSquaredMinusl = m11 + m22 + m33;
            double fourXSquaredMinusl = m11 - m22 - m33;
            double fourYSquaredMinusl = m22 - m11 - m33;
            double fourZSquaredMinusl = m33 - m11 - m22;

            int biggestIndex = 0;
            double fourBiggestSqureMinus1 = fourWSquaredMinusl;
            if (fourXSquaredMinusl > fourBiggestSqureMinus1)
            {
                fourBiggestSqureMinus1 = fourXSquaredMinusl;
                biggestIndex = 1;
            }
            if (fourYSquaredMinusl > fourBiggestSqureMinus1)
            {
                fourBiggestSqureMinus1 = fourYSquaredMinusl;
                biggestIndex = 2;
            }
            if (fourZSquaredMinusl > fourBiggestSqureMinus1)
            {
                fourBiggestSqureMinus1 = fourZSquaredMinusl;
                biggestIndex = 3;
            }

            //计算平方根和除法 
            double biggestVal = Math.Sqrt(fourBiggestSqureMinus1 + 1.0f) * 0.5f;
            double mult = 0.25f / biggestVal;
            //计算四元数的值
            switch (biggestIndex)
            {
                case 0:
                    w = biggestVal;
                    x = (m23 - m32) * mult;
                    y = (m31 - m13) * mult;
                    z = (m12 - m21) * mult;
                    break;
                case 1:
                    x = biggestVal;
                    w = (m23 - m32) * mult;
                    y = (m12 + m21) * mult;
                    z = (m31 + m13) * mult;
                    break;
                case 2:
                    y = biggestVal;
                    w = (m31 - m13) * mult;
                    x = (m12 + m21) * mult;
                    z = (m23 + m32) * mult;
                    break;
                case 3:
                    z = biggestVal;
                    w = (m12 - m21) * mult;
                    x = (m31 + m13) * mult;
                    y = (m23 + m32) * mult;
                    break;
            }
            Q[0] = x;
            Q[1] = y;
            Q[2] = z;
            Q[3] = w;
            return Q;
        }
        Matrix4x4 Eular2Rot(double EX, double EY, double EZ, double[] TransVector)
        {
            // EX, EY, EZ, TransVector是机器手坐标系（或工具坐标系）在基坐标系（世界坐标系或者工件坐标系也可）里面的位置和姿态变换参数，需要从机器人控制器或示教盒中读取
            //  EX--机器手坐标系绕基坐标系的X轴旋转欧拉角 
            //   EY--绕Y轴旋转欧拉角 
            //   EZ--绕Z轴旋转欧拉角
            //   TransVector--机器手坐标系原点相对基坐标系的平移向量:3x1
            double a = EX / 180 * Math.PI;
            double b = EY / 180 * Math.PI;
            double r = EZ / 180 * Math.PI;
            float ca = (float)Math.Cos(a);
            float sa = (float)Math.Sin(a);
            float cb = (float)Math.Cos(b);
            float sb = (float)Math.Sin(b);
            float cr = (float)Math.Cos(r);
            float sr = (float)Math.Sin(r);
            Matrix4x4 Rx = new Matrix4x4(1, 0, 0, 0, 0, ca, -sa, 0, 0, sa, ca, 0, 0, 0, 0, 1);
            Matrix4x4 Ry = new Matrix4x4(cb, 0, sb, 0, 0, 1, 0, 0, -sb, 0, cb, 0, 0, 0, 0, 1);
            Matrix4x4 Rz = new Matrix4x4(cr, -sr, 0, 0, sr, cr, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
            Matrix4x4 R = Rz * Ry * Rx;
            return R;
        }


        //注：Hamilton为右手系
        //   JPL     为左手系

        double[,] quat_to_pos_matrix_hm(double p_x, double p_y, double p_z, double x, double y, double z, double w)
        {
            double[,] T = new double[4, 4];
            T[0, 3] = p_x;
            T[1, 3] = p_y;
            T[2, 3] = p_z;
            T[3, 3] = 1;
            T[0, 0] = 1 - 2 * Math.Pow(y, 2) - 2 * Math.Pow(z, 2);
            T[0, 1] = 2 * (x * y - w * z);
            T[0, 2] = 2 * (x * z + w * y);
            T[1, 0] = 2 * (x * y + w * z);
            T[1, 1] = 1 - 2 * Math.Pow(x, 2) - 2 * Math.Pow(z, 2);
            T[1, 2] = 2 * (y * z - w * x);

            T[2, 0] = 2 * (x * z - w * y);
            T[2, 1] = 2 * (y * z + w * x);
            T[2, 2] = 1 - 2 * Math.Pow(x, 2) - 2 * Math.Pow(y, 2);
            return T;
        }


        double[,] quat_to_pos_matrix_JPL(double p_x, double p_y, double p_z, double x, double y, double z, double w)
        {
            double[,] T = new double[4, 4];
            T[0, 3] = p_x;
            T[1, 3] = p_y;
            T[2, 3] = p_z;
            T[3, 3] = 1;
            T[0, 0] = 1 - 2 * Math.Pow(y, 2) - 2 * Math.Pow(z, 2);
            T[0, 1] = 2 * (x * y + w * z);
            T[0, 2] = 2 * (x * z - w * y);

            T[1, 0] = 2 * (x * y - w * z);
            T[1, 1] = 1 - 2 * Math.Pow(x, 2) - 2 * Math.Pow(z, 2);
            T[1, 2] = 2 * (y * z + w * x);

            T[2, 0] = 2 * (x * z + w * y);
            T[2, 1] = 2 * (y * z - w * x);
            T[2, 2] = 1 - 2 * Math.Pow(x, 2) - 2 * Math.Pow(y, 2);
            return T;
        }


        double[] pos_matrix_to_quat_hm(double[,] T)
        {
            double r11 = T[0, 0];
            double r12 = T[0, 1];
            double r13 = T[0, 2];
            double r21 = T[1, 0];
            double r22 = T[1, 1];
            double r23 = T[1, 2];
            double r31 = T[2, 0];
            double r32 = T[2, 1];
            double r33 = T[2, 2];
            double w = (1 / 2) * Math.Sqrt(1 + r11 + r22 + r33);
            double x = (r32 - r23) / (4 * w);
            double y = (r13 - r31) / (4 * w);
            double z = (r21 - r12) / (4 * w);
            return new double[] { x, y, z, w };
        }


        double[] pos_matrix_to_quat_JPL(double[,] T)
        {
            double r11 = T[0, 0];
            double r12 = T[0, 1];
            double r13 = T[0, 2];
            double r21 = T[1, 0];
            double r22 = T[1, 1];
            double r23 = T[1, 2];
            double r31 = T[2, 0];
            double r32 = T[2, 1];
            double r33 = T[2, 2];
            double w = (1 / 2) * Math.Sqrt(1 + r11 + r22 + r33);
            double x = (r23 - r32) / (4 * w);
            double y = (r31 - r13) / (4 * w);
            double z = (r12 - r21) / (4 * w);
            return new double[] { x, y, z, w };
        }
    }
}
