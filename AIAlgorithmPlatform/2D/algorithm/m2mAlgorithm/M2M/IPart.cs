using System;
using System.Collections.Generic;
using System.Text;
using Position_Interface;
using Position_Connected_Interface;

namespace M2M
{
    public interface IPart : IPosition
    {
        IPart_Edit Create();

        int GetBottomLevelPointNum();

        int GetSubPositionNum();

        IPosition GetRandomOneFormDescendantPoint();

        IPosition GetRandomPointFormBottomLevel();

        //�õ��������ӷֿ鼯�ϣ���������ʽ�ϵģ�
        IPositionSet GetTrueChildPositionSet();
    }

    public interface IPart_Edit : IPart
    {
        void SetX(int x);
        void SetY(int y);

        void SubPointNumIncrease(int num);

        void SubPointNumDecrease(int num);

        void AddToSubPositionList(IPosition position);

        void RemoveFormSubPositionList(IPosition RemovePosition);

        void SetDeputyPoint(IPosition point);
    }
}