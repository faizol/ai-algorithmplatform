using System;
using System.Collections.Generic;
using System.Text;
using Position_Interface;
using Position_Implement;
using Position_Connected_Interface;

namespace M2M
{
    class PositionAndPositon
    {
        public PositionAndPositon(IPosition_Connected positionInExistPart, IPosition_Connected position)
        {
            this.positionInExistPart = positionInExistPart;
            this.position = position;
        }
        public IPosition_Connected positionInExistPart;
        public IPosition_Connected position;
    }

    public class Tag_M2M_Position
    {
        public float g;
        public bool isClose;
        public ushort timeStamp;
        public IPart_Connected parentPart = null;
        public IPosition_Connected parent = null;
    };

    public class Tag_M2M_Part
    {
        public float gs, ge;
        public bool isClose;
        public ushort timeStamp;
        public bool isNeedToSearch;
    };

    public class BuildPartSetConnectionForM2MStructure
    {
        #region code for algorithm demo
        //Preprocess
        public event dGetPositionSetInSpecificLevel GetPartSetInSpecificLevel;
        //public event dGetM2MStructure GetM2MStructure;

        //Query Process
        //public event dGetPosition GetQueryPosition;
        //public event dGetPartInSpecificLevel GetQueryPart;
        //public event dSearchOnePartBeginAndGetSearchPartSequence SearchOnePartBeginAndGetSearchPartSequence;
        //public event dGetPartInSpecificLevel GetSearchPart;
        //public event dGetPosition GetComparedPoint;
        //public event dGetPosition CurrentNearestPointChanged;
        //public event dGetRectangle SearchBoundChanged;
        #endregion

        public BuildPartSetConnectionForM2MStructure(){}

        public BuildPartSetConnectionForM2MStructure(IM2MStructure m2mStructure)
        {
            TraversalEveryLevelAndBuild(m2mStructure);
        }

        public void TraversalEveryLevelAndBuild(IM2MStructure m2mStructure)
        {
            IPart rootPart = m2mStructure.GetLevel(0).GetPartRefByPartIndex(0, 0);

            IPositionSet bottonLevelPositionSet = m2mStructure.GetBottonLevelPositionSetByAncestorPart(rootPart,0);

            bottonLevelPositionSet.InitToTraverseSet();

            while(bottonLevelPositionSet.NextPosition())
            {
                ((IPosition_Connected)(bottonLevelPositionSet.GetPosition())).SetAttachment(new Tag_M2M_Position());
            }

            //����ÿһ��
            for (int levelSequence = m2mStructure.GetLevelNum() - 1; levelSequence >= 0; levelSequence--)
            {
                Queue<PositionAndPositon> partAndPositonQueue = new Queue<PositionAndPositon>();

                bool isPart = true;

                ILevel currentLevel = m2mStructure.GetLevel(levelSequence);
                IPositionSet currrentLevelPositionSet = null;

                //���������ײ�
                if (levelSequence == m2mStructure.GetLevelNum() - 1)
                {
                    isPart = false;
                }

                if (levelSequence == 0)
                {
                    IPositionSetEdit temp = new Position_Implement.PositionSetEdit_ImplementByICollectionTemplate();
                    temp.AddPosition(rootPart);
                    currrentLevelPositionSet = temp;
                }
                else
                {                    
                    currrentLevelPositionSet = m2mStructure.GetDescendentPositionSetByAncestorPart(levelSequence, rootPart, 0);
                }

                currrentLevelPositionSet.InitToTraverseSet();
                
                //����ÿһ���ֿ�
                while (currrentLevelPositionSet.NextPosition())
                {
                    List<IPart_Connected> SubPartList = new List<IPart_Connected>();

                    IPart_Multi currentMultiPart = (IPart_Multi)(currrentLevelPositionSet.GetPosition());

                    //����part�ı߽�
                    float minSequenceX;
                    float maxSequenceX;
                    float minSequenceY;
                    float maxSequenceY;
                    if (isPart)
                    {
                        float unitNumInLength = (int)(m2mStructure.GetLevel(levelSequence + 1).GetUnitNumInHeight() / currentLevel.GetUnitNumInHeight());
                        minSequenceX = currentMultiPart.GetX() * unitNumInLength;
                        maxSequenceX = minSequenceX + unitNumInLength;
                        minSequenceY = currentMultiPart.GetY() * unitNumInLength;
                        maxSequenceY = minSequenceY + unitNumInLength;
                    }
                    else
                    {
                        minSequenceX = currentLevel.ConvertPartSequenceXToRealValue(currentMultiPart.GetX());
                        maxSequenceX = minSequenceX + currentLevel.GetGridWidth();
                        minSequenceY = currentLevel.ConvertPartSequenceYToRealValue(currentMultiPart.GetY());
                        maxSequenceY = minSequenceY + currentLevel.GetGridHeight();
                    }

                    IPositionSet childPositionSetOfCurrentPart = m2mStructure.GetChildPositionSetByParentPart(levelSequence, currentMultiPart);
                    childPositionSetOfCurrentPart.InitToTraverseSet();

                    //����ÿһ���ӷֿ���
                    while (childPositionSetOfCurrentPart.NextPosition())
                    {
                        if(isPart)
                        {
                            IPart_Multi childPartMulti = (IPart_Multi)childPositionSetOfCurrentPart.GetPosition();
                            foreach(IPosition_Connected currentPosition_Connected in childPartMulti.GetSubPartSet())
                            {
                                BFSInOnePoint(partAndPositonQueue, isPart, SubPartList, currentMultiPart, minSequenceX, maxSequenceX, minSequenceY, maxSequenceY, currentPosition_Connected);
                            }
                        }
                        else
                        {
                            IPosition_Connected currentPosition_Connected = (IPosition_Connected)childPositionSetOfCurrentPart.GetPosition();
                            BFSInOnePoint(partAndPositonQueue, isPart, SubPartList, currentMultiPart, minSequenceX, maxSequenceX, minSequenceY, maxSequenceY, currentPosition_Connected);
                        }
                    }
                    //����ÿһ���ӷֿ����

                    currentMultiPart.SetSubPartSet(SubPartList);
                }
                //������ĳ���ÿ���ֿ�

                //����û�������ӵĵ�ͷֿ��
                foreach (PositionAndPositon pap in partAndPositonQueue)
                {
                    IPosition_Connected position_Connected = pap.position;
                    IPart_Connected currentParentPart = GetParentPart(position_Connected, isPart);
                    IPart_Connected currentParentPart2 = GetParentPart(pap.positionInExistPart, isPart);

                    float distance = CalculateTheDistanceBetweenTwoPosition(currentParentPart2, currentParentPart);

                    IPositionSet_Connected_AdjacencyEdit positionSet_Connected_Adjacency = currentParentPart2.GetAdjacencyPositionSetEdit();

                    bool isContain = false;

                    positionSet_Connected_Adjacency.InitToTraverseSet();
                    while (positionSet_Connected_Adjacency.NextPosition())
                    {
                        if(positionSet_Connected_Adjacency.GetPosition() == currentParentPart)
                        {
                            isContain = true;
                            break;
                        }
                    }

                    if (isContain)
                    {
                        continue;
                    }
                    else
                    {
                        positionSet_Connected_Adjacency.AddAdjacency(currentParentPart, distance);
                    }

                    positionSet_Connected_Adjacency = currentParentPart.GetAdjacencyPositionSetEdit();

                    isContain = false;

                    positionSet_Connected_Adjacency.InitToTraverseSet();
                    while (positionSet_Connected_Adjacency.NextPosition())
                    {
                        if (positionSet_Connected_Adjacency.GetPosition() == currentParentPart2)
                        {
                            isContain = true;
                            break;
                        }
                    }

                    if (isContain)
                    {
                        continue;
                    }
                    else
                    {
                        positionSet_Connected_Adjacency.AddAdjacency(currentParentPart2, distance);
                    }
                }

                #region code for algorithm demo
                if (GetPartSetInSpecificLevel != null)
                {
                    GetPartSetInSpecificLevel(m2mStructure.GetLevel(levelSequence), levelSequence, m2mStructure.GetDescendentPositionSetByAncestorPart(levelSequence, rootPart, 0));
                }
                #endregion
            }
            //����ÿһ�����
        }

        private void BFSInOnePoint(Queue<PositionAndPositon> PositionAndPositonQueue, bool isPart, List<IPart_Connected> SubPartList, IPart_Multi currentMultiPart, float minSequenceX, float maxSequenceX, float minSequenceY, float maxSequenceY, IPosition_Connected currentPosition_Connected)
        {
            //���û�б�����
            if (GetParentPart(currentPosition_Connected, isPart) == null)
            {
                IPart_Connected currentPart_Connected = currentMultiPart.CreateSubPart();
                currentPart_Connected.SetAttachment(new Tag_M2M_Part());
                currentPart_Connected.SetX(currentMultiPart.GetX());
                currentPart_Connected.SetY(currentMultiPart.GetY());
                bool isNeedToAddToSubPartList = true;
                SubPartList.Add(currentPart_Connected);

                currentPart_Connected.AddToSubPositionList(currentPosition_Connected);
                SetParentPart(currentPosition_Connected, currentPart_Connected, isPart);

                //�Ե�ǰ��Ϊ��ʼ����ī������
                Queue<IPosition_Connected> OpenTable = new Queue<IPosition_Connected>();

                OpenTable.Enqueue(currentPosition_Connected);

                //���Open����Ԫ�������ȡ��
                while (OpenTable.Count > 0)
                {
                    IPosition_Connected currentCenterPosition_Connected = OpenTable.Dequeue();
                    IPositionSet_Connected_Adjacency currentPositionAdjacencyPositionSet = currentCenterPosition_Connected.GetAdjacencyPositionSet();

                    currentPositionAdjacencyPositionSet.InitToTraverseSet();

                    while (currentPositionAdjacencyPositionSet.NextPosition())
                    {
                        IPosition_Connected adjPosition_Connected = currentPositionAdjacencyPositionSet.GetPosition_Connected();

                        float pX = adjPosition_Connected.GetX();
                        float pY = adjPosition_Connected.GetY();
                        //����ֿ��ڵ�ǰ�ֿ�֮��
                        if (pX >= minSequenceX && pX < maxSequenceX && pY >= minSequenceY && pY < maxSequenceY)
                        {
                            IPart_Connected parentPart = GetParentPart(adjPosition_Connected, isPart);
                            //����÷ֿ黹û������
                            if (parentPart == null)
                            {
                                currentPart_Connected.AddToSubPositionList(adjPosition_Connected);
                                SetParentPart(adjPosition_Connected, currentPart_Connected, isPart);

                                //�Ž�Open��
                                OpenTable.Enqueue(adjPosition_Connected);
                            }
                            //������ֿ鲻Ϊnull��������÷ֿ�֮ǰ�Ѿ��������������������ķֿ���ԭ�ֿ���ͨ
                            else if (parentPart != currentPart_Connected)
                            {
                                isNeedToAddToSubPartList = false;
                                //��ԭ���ĸ��ֿ���Ϊ�µĸ��ֿ�

                                IPositionSet tempPositionSet = parentPart.GetTrueChildPositionSet();
                                tempPositionSet.InitToTraverseSet();
                                while(tempPositionSet.NextPosition())
                                {
                                    IPosition_Connected temp = (IPosition_Connected)tempPositionSet.GetPosition();

                                    SetParentPart(temp, currentPart_Connected, isPart);
                                    currentPart_Connected.AddToSubPositionList(temp);
                                }

                                SubPartList.Remove(parentPart);
                            }
                        }
                        //������ڵ�ǰ�ֿ�����
                        else
                        {
                            //����ڵ�ǰ�ֿ���ĵ㻹û������������û���ƶ��丸�ֿ飩�����Ž�partAndPositonQueue
                            IPart_Connected currentParentPart = GetParentPart(adjPosition_Connected, isPart);

                            PositionAndPositonQueue.Enqueue(new PositionAndPositon(currentCenterPosition_Connected, adjPosition_Connected));

                            //if (currentParentPart == null)
                            //{
                            //    PositionAndPositonQueue.Enqueue(new PositionAndPositon(currentCenterPosition_Connected, adjPosition_Connected));
                            //}
                            ////����ڵ�ǰ�ֿ���ĵ��Ѿ����丸�ֿ飬���������ֿ������ӹ�ϵ��
                            //else
                            //{
                            //    currentPart_Connected.GetAdjacencyPositionSetEdit().AddAdjacency(currentParentPart, CalculateTheDistanceBetweenTwoPosition(currentPart_Connected, currentParentPart));
                            //}
                        }
                    }
                }
            }
        }

        IPart_Connected GetParentPart(IPosition_Connected position_Connected, bool isPart)
        {
            if(isPart)
            {
                return ((IPart_Connected)position_Connected).GetParentPart();
            }
            else
            {
                return ((Tag_M2M_Position)position_Connected.GetAttachment()).parentPart;
            }
        }

        int total = 0;

        void SetParentPart(IPosition_Connected position_Connected, IPart_Connected parentPart, bool isPart)
        {
            if(isPart)
            {
                ((IPart_Connected)position_Connected).SetParentPart(parentPart);
            }
            else
            {
                ((Tag_M2M_Position)position_Connected.GetAttachment()).parentPart = parentPart;
                total++;
            }
        }

        float CalculateTheDistanceBetweenTwoPosition(IPosition position1, IPosition position2)
        {
            return (float)Math.Sqrt(Math.Pow((position1.GetX() - position2.GetX()), 2) + Math.Pow((position1.GetY() - position2.GetY()), 2));
        }
    }
}
