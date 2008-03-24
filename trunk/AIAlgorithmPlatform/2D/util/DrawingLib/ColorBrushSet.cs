using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace DrawingLib
{
    /**
     * ��ɫ��ˢ��
     * �趨��ɫ�����ֵmax֮�󣬴�1��maxΪ��Ȼ��������Ϻ콥��ɫ��
     * 
     */
    //----------------------------------------------------------------
    public interface ColorBrushSet
    {
        Brush getBrush(int index);
        Brush getReverseBrush(int index);
        void setMaxValue(int max);
        int getMax();
        int getHalfMax();
    }
    //----------------------------------------------------------------

    //----------------------------------------------------------------
    public class SolidColorBrushSet : ColorBrushSet
    {
        int max;
        int halfMax;
        int maxValue;//��ͼ�ĸ����ϵ�������ֵ
        ColorSet colorMap = null;
        Brush[] brushMap = null;
        float rate = 1;

        public SolidColorBrushSet()
        {
            colorMap = new Rainbow();
            max = colorMap.getMax();
            halfMax = colorMap.getHalfMax();
            setMaxValue(max);
            brushMap = new SolidBrush[max];
            for (int i = 0; i < max; i++)
            {
                brushMap[i] = new SolidBrush(colorMap.getColor(i + 1));
            }
        }

        public void setMaxValue(int max)
        {
            maxValue = max;
            rate = this.max / (float)maxValue;
        }

        //�������Ϊindex����ɫ�Ļ�ˢ
        public Brush getBrush(int index)
        {
            if (index > 0)
            {
                if (maxValue != max)
                    return brushMap[(int)((index - 1) * rate) % max];
                else
                    return brushMap[(index - 1) % max];
            }
            else
            {
                if (index == 0)
                    return Brushes.White;
                else
                    return Brushes.Black;
            }
        }

        //�������Ϊindex����ɫ�ķ�ɫ��ˢ
        public Brush getReverseBrush(int index)
        {
            if (index > 0)
            {
                if (maxValue != max)
                    return brushMap[((int)((index - 1) * rate) + halfMax) % max];
                else
                    return brushMap[(index - 1 + halfMax) % max];
            }
            else
            {
                if (index == 0)
                    return Brushes.White;
                else
                    return Brushes.Black;
            }
        }

        public int getMax()
        {
            return max;
        }

        public int getHalfMax()
        {
            return halfMax;
        }
    }
}
