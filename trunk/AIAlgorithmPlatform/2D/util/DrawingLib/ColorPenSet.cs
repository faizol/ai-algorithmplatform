using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace DrawingLib
{
    /**
     * ��ɫ�ʼ�
     * �趨��ɫ�����ֵmax֮�󣬴�1��maxΪ��Ȼ��������Ϻ콥��ɫ��
     * 
     */
    public class ColorPenSet
    {
        int max;
        int halfMax;
        int maxValue;//��ͼ�ĸ����ϵ�������ֵ
        ColorSet colorMap = null;
        Pen[] pens = null;
        float rate = 1;
        float width = 1;

        public ColorPenSet()
        {
            colorMap = new Rainbow();
            max = colorMap.getMax();
            halfMax = colorMap.getHalfMax();
            setMaxValue(max);
            pens = new Pen[max];
            for (int i = 0; i < max; i++)
            {
                pens[i] = new Pen(colorMap.getColor(i + 1));
            }
        }

        public Pen getPen(int index)
        {
            if (index > 0)
            {
                if (maxValue != max)
                    return pens[(int)((index - 1) * rate) % max];
                else
                    return pens[(index - 1) % max];
            }
            else
            {
                if (index == 0)
                    return Pens.White;
                else
                    return Pens.Black;
            }
        }

        public Pen getReversePen(int index)
        {
            if (index > 0)
            {
                if (maxValue != max)
                    return pens[((int)((index - 1) * rate) + halfMax) % max];
                else
                    return pens[(index - 1 + halfMax) % max];
            }
            else
            {
                if (index == 0)
                    return Pens.White;
                else
                    return Pens.Black;
            }
        }

        public void setMaxValue(int max)
        {
            maxValue = max;
            rate = this.max / (float)maxValue;
        }

        public int getMax()
        {
            return max;
        }

        public int getHalfMax()
        {
            return halfMax;
        }

        public void setEndCap(System.Drawing.Drawing2D.LineCap cap)
        {
            for (int i = 0; i < max; i++)
            {
                pens[i].EndCap = cap;
            }
        }

        public void setStartCap(System.Drawing.Drawing2D.LineCap cap)
        {
            for (int i = 0; i < max; i++)
            {
                pens[i].StartCap = cap;
            }
        }

        public void setWidth(float w)
        {
            width = w;   
            for (int i = 0; i < max; i++)
            {
                pens[i].Width = width;
            }
        }

        public float getWidth()
        {
            return width;
        }
    }
}
