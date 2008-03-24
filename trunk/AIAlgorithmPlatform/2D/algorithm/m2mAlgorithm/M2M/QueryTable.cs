using System;
using System.Collections.Generic;
using System.Text;
using Position_Interface;

namespace M2M
{
    interface QueryTable
    {
        /// <summary>
        /// ��ʼ�����,���������ڴ�
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        void InitTable(int width,int height);

        /// <summary>
        /// ����õ����ڵķֿ��Ѿ������򷵻ظ÷ֿ��ָ��,���򷵻�null
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        IPart_Edit GetPartRef(int x,int y);

        /// <summary>
        /// �����÷ֿ鲢���ظ÷ֿ��ָ��
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        IPart_Edit CreateAndGetPartRef(int x, int y);

        /// <summary>
        /// �ӱ����ɾ��һ�����зֿ�ָ��,��������ָ��.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        IPart_Edit RemovePart(int x, int y);
    }
}