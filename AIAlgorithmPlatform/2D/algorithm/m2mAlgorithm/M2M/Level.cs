using System;
using System.Runtime.CompilerServices;
using Position_Interface;
using System.Threading;

namespace M2M
{
    public class Level : ILevel
    {
        int unitNumInWidth;
        public void SetUnitNumInWidth(int unitNumInWidth)
        {
            this.unitNumInWidth = unitNumInWidth;
        }
        public int GetUnitNumInWidth()
        {
            return unitNumInWidth;
        }

        int unitNumInHeight;
        public void SetUnitNumInHeight(int unitNumInHeight)
        {
            this.unitNumInHeight = unitNumInHeight;
        }
        public int GetUnitNumInHeight()
        {
            return unitNumInHeight;
        }

        float gridWidth = 0;
        public void SetGridWidth(float gridWidth)
        {
            this.gridWidth = gridWidth;
        }
        public float GetGridWidth()
        {
            return gridWidth;
        }

        float gridHeight = 0;
        public void SetGridHeight(float gridHeight)
        {
            this.gridHeight = gridHeight;
        }
        public float GetGridHeight()
        {
            return gridHeight;
        }

        public void AllocateMemory()
        {
            //�����ڴ�
            queryTable.InitTable(unitNumInWidth, unitNumInHeight);
        }

        Position_Point relativelyPoint = null;

        public void SetRelativelyPoint(Position_Point relativelyPoint)
        {
            this.relativelyPoint = relativelyPoint;
        }

        public float ConvertRealValueToRelativeValueX(float realityValue)
        {
            return realityValue - relativelyPoint.GetX();
        }

        public float ConvertRealValueToRelativeValueY(float realityValue)
        {
            return realityValue - relativelyPoint.GetY();
        }

        public int ConvertRelativeValueToPartSequenceX(float relativelValue)
        {
            return (int)(relativelValue / gridWidth);
        }

        public int ConvertRelativeValueToPartSequenceY(float relativelValue)
        {
            return (int)(relativelValue / gridHeight);
        }

        public int ConvertRealValueToPartSequenceX(float realityValue)
        {
            return ConvertRelativeValueToPartSequenceX(ConvertRealValueToRelativeValueX(realityValue));
        }

        public int ConvertRealValueToPartSequenceY(float realityValue)
        {
            return ConvertRelativeValueToPartSequenceY(ConvertRealValueToRelativeValueY(realityValue));
        }

        public float ConvertPartSequenceXToRealValue(float x)
        {
            //return (float)Math.Floor(x * gridWidth + relativelyPoint.GetX());
            return x * gridWidth + relativelyPoint.GetX();
        }

        public float ConvertPartSequenceYToRealValue(float y)
        {
            //return (float)Math.Floor(y * gridHeight + relativelyPoint.GetY());
            return y * gridHeight + relativelyPoint.GetY();
        }

        QueryTable queryTable = new QueryTableByArray(typeof(Part));
        internal QueryTable QueryTable
        {
            set { queryTable = value; }
        }

        public IPart GetPartRefByPartIndex(int x,int y)
        {
            return queryTable.GetPartRef(x, y);
        }

        /// <summary>
        /// ����õ����ڵķֿ��Ѿ������򷵻ظ÷ֿ��ָ��,���򷵻�null
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public IPart GetPartRefByPoint(IPosition point)
        {
            int x, y;
            x = ConvertRealValueToPartSequenceX(point.GetX());
            y = ConvertRealValueToPartSequenceY(point.GetY());

            return queryTable.GetPartRef(x,y);
        }

        /// <summary>
        /// ����õ����ڵķֿ��Ѿ������򷵻ظ÷ֿ��ָ��,���򴴽��÷ֿ鲢���ظ÷ֿ��ָ��
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        //[MethodImplAttribute(MethodImplOptions.Synchronized)]
        public IPart_Edit GOCPartRefByPoint(IPosition point)
        {
            //lock (this)
            {
                int x, y;
                x = ConvertRealValueToPartSequenceX(point.GetX());
                y = ConvertRealValueToPartSequenceY(point.GetY());

                IPart_Edit queryPart = queryTable.GetPartRef(x, y);

                //lock (queryTable)
                {
                    if (queryPart == null)
                    {
                        queryPart = queryTable.CreateAndGetPartRef(x, y);
                        queryPart.SetX(x);
                        queryPart.SetY(y);

                        //������part��������һ���趨Ϊ��part�Ĵ����.
                        queryPart.SetDeputyPoint(point);
                    }
                }

                return queryPart;
            }
        }

        public IPart RemovePartByPoint(IPosition point)
        {
            int x, y;
            x = ConvertRealValueToPartSequenceX(point.GetX());
            y = ConvertRealValueToPartSequenceY(point.GetY());

            IPosition queryPosition = queryTable.RemovePart(x, y);

            if (queryPosition == null)
            {
                throw new Exception("�ֿ���������û�д�ɾ���ֿ��ָ��!");
            }

            return (IPart)queryPosition;
        }

        public bool IndexIsExceedPartRange(int x, int y)
        {
            if ((x < 0) || (x > unitNumInWidth - 1) || (y < 0) || (y > unitNumInHeight - 1))
            {
                return true;
            }
            return false;
        }
    }
}