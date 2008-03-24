using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Position_Interface;

namespace M2M
{
    public class M2MStructure_General : AM2MStructure, IM2MStructure
    {
        public M2MStructure_General(List<Level> LevelList)
        {
            this.LevelList = LevelList;
        }

        public List<Level> LevelList = null;

        override public ILevel GetLevel(int levelSequecne)
        {
            return LevelList[levelSequecne];
        }

        override public int GetLevelNum()
        {
            return LevelList.Count;
        }

        //�õ���ʽ�ϵ��ӷֿ鼯�ϣ�������ʽ�ϵ��ӷֿ��п��������ݽṹ�ϵ����²����ֿ鼯�ϣ�
        override public IPositionSet GetChildPositionSetByParentPart(int parentPartLevelSequence, IPart parentPart)
        {
            return parentPart.GetTrueChildPositionSet();
        }

        private int ThreadNum = 4;

        private int completeThreadNum;

        private IPositionSet positionSet;

        public void Preprocessing2(IPositionSet positionSet)
        {
            this.positionSet = positionSet;

            completeThreadNum = 0;

            ThreadPool.SetMaxThreads(4, 4);

            positionSet.InitToTraverseSet();
            for (int i = 0; i < ThreadNum; i++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadProc));
            }

            while (completeThreadNum < ThreadNum)
            {
                Thread.CurrentThread.Join(1);
            }
        }

        public void Preprocessing(IPositionSet positionSet)
        {
            positionSet.InitToTraverseSet();
            while (positionSet.NextPosition())
            {
                Insert(positionSet.GetPosition());
            }
        }

        private void ThreadProc(Object o)
        {
            IPosition tempPosition;
            while (true)
            {
                Monitor.Enter(positionSet);
                if (positionSet.NextPosition())
                {
                    tempPosition = positionSet.GetPosition();
                }
                else
                {
                    Monitor.Exit(positionSet);
                    break;
                }
                Monitor.Exit(positionSet);

                Insert(tempPosition);
            }
            Interlocked.Increment(ref completeThreadNum);
        }

        public bool CanInsert(IPosition point)
        {
            float relativeX = LevelList[0].ConvertRealValueToRelativeValueX(point.GetX());
            float relativeY = LevelList[0].ConvertRealValueToRelativeValueY(point.GetY());

            if (relativeX >= 0 && relativeX < LevelList[0].GetGridWidth() && relativeY >= 0 && relativeY < LevelList[0].GetGridHeight())
            {
                return true;
            }
            return false;
        }

        public void Insert(IPosition point)
        {
            IPosition NewPosition = point;

            for (int levelSequrence = LevelList.Count - 1; levelSequrence >= 0; levelSequrence--)
            {
                IPart_Edit currentPart = LevelList[levelSequrence].GOCPartRefByPoint(point);

                Monitor.Enter(currentPart);
                currentPart.AddToSubPositionList(NewPosition);

                //���²�ͬ������õ����ڵ�part�е����Ŀ
                currentPart.SubPointNumIncrease(1);
                Monitor.Exit(currentPart);

                if (currentPart.GetSubPositionNum() == 1)
                {
                    //���part����ֻ��һ��point,˵�����partҲ���½���,�������ĺ�۷ֿ�Ҳ�����������.
                    NewPosition = currentPart;
                }
                else
                {
                    break;
                }
            }
        }

        public void Remove_NotTestYet(IPosition point)
        {
            //���²�ͬ������õ����ڵ�part�е����Ŀ
            for (int levelSequrence = LevelList.Count - 2; levelSequrence >= 0; levelSequrence--)
            {
                ((IPart_Edit)LevelList[levelSequrence + 1].GetPartRefByPoint(point)).SubPointNumDecrease(1);
            }

            IPosition RemovePosition = point;

            for (int levelSequrence = LevelList.Count - 1; levelSequrence >= -1; levelSequrence--)
            {
                if (levelSequrence == -1)
                {
                    ((IPart_Edit)LevelList[0].GetPartRefByPoint(point)).RemoveFormSubPositionList(RemovePosition);
                    break;
                }

                IPart_Edit currentPart = (IPart_Edit)(LevelList[levelSequrence].GetPartRefByPoint(point));

                if (currentPart == null)
                {
                    throw new Exception("�����ڸ÷ֿ��ָ��!(ָ��ɾ���ĵ��Ƿ��Ѿ�����?)");
                }

                currentPart.RemoveFormSubPositionList(RemovePosition);

                if (currentPart.GetSubPositionNum() == 0)
                {
                    //���part����һ���㶼û��,Ӧ�ðѸ�part�ӱ����ɾ��.
                    LevelList[levelSequrence].RemovePartByPoint(point);
                    //ͬʱҲӦ�ôӰ����÷ֿ�ĺ�۷ֿ���ɾ���÷ֿ�.
                    RemovePosition = currentPart;
                }
                else
                {
                    IPosition newDeputyPoint = null;
                    int levelSequrence2 = levelSequrence;
                    IPosition deputyPoint = LevelList[levelSequrence2].GetPartRefByPoint(point).GetRandomOneFormDescendantPoint();
                    if (point == deputyPoint)
                    {
                        newDeputyPoint = currentPart.GetRandomPointFormBottomLevel();
                    }

                    //�����part���泬��һ��������Խ���ɾ������,������Ҫ�ж�һ�¸�part�Լ����������part�Ĵ�����Ƿ�����ɾ����,�����,��Ҫ����.
                    for (levelSequrence2 = levelSequrence; levelSequrence2 >= 0; levelSequrence2--)
                    {
                        deputyPoint = LevelList[levelSequrence2].GetPartRefByPoint(point).GetRandomOneFormDescendantPoint();
                        if (point == deputyPoint)
                        {
                            ((IPart_Edit)(LevelList[levelSequrence2].GetPartRefByPoint(point))).SetDeputyPoint(newDeputyPoint);
                        }
                        else
                        {
                            //�������ĳ��Ĵ�����Ѿ�������ɾ����,����Ҫ�������ϲ�ѯ.
                            break;
                        }
                    }

                    break;
                }
            }
        }
    }
}