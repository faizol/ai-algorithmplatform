using System;
using System.Collections.Generic;
using System.Text;
//using DataStructure;
using Position_Connected_Interface;

namespace SearchEngineLibrary
{
    public interface ISearchPathEngine
    {
        void InitEngineForMap(IPositionSet_Connected map);
        //û��·���򷵻�null
        List<IPosition_Connected> SearchPath(IPosition_Connected start, IPosition_Connected end);
    }
}
