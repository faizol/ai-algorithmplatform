using System;
using System.Collections.Generic;
using System.Text;
using Position_Interface;

namespace ConvexHullEngine
{
    //͹���㷨�ӿ�
    public interface IConvexHullEngine
    {
        IPositionSet ConvexHull(IPositionSet ps);
    }
}
