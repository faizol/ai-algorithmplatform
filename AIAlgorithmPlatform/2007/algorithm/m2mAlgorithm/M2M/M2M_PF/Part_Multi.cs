using System;
using System.Collections.Generic;
using System.Text;
using Position_Interface;
using Position_Implement;

namespace M2M
{
    public class Part_Multi : Part, IPart_Multi
    {
        public override IPart_Edit Create()
        {
            return new Part_Multi();
        }

        public IPart_Connected CreateSubPart()
        {
            return new Part_Connected();
        }

        List<IPart_Connected> SubPartList = new List<IPart_Connected>();

        public Part_Multi()
        {
            Part_Connected firstPart = new Part_Connected();
            firstPart.SetSubPositionList(subPositionList);
            SubPartList.Add(firstPart);
        }

        public IEnumerable<IPart_Connected> GetSubPartSet()
        {
            return SubPartList;
        }

        public void SetSubPartSet(List<IPart_Connected> SubPartList)
        {
            this.SubPartList = SubPartList;
        }

        /// <summary>
        /// �õ���ʵ����һ���У����ڸ÷ֿ�ķֿ鼯��
        /// </summary>
        /// <returns></returns>
        public override IPositionSet GetTrueChildPositionSet()
        {
            PositionSetSet positionSetSet = new PositionSetSet();
            foreach(IPart_Connected part_Connected in SubPartList)
            {
                positionSetSet.AddPositionSet(part_Connected.GetTrueChildPositionSet());
            }

            return positionSetSet;
        }
    }
}
