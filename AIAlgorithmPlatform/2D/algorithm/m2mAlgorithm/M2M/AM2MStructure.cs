using System;
using System.Collections.Generic;
using System.Text;
using Position_Interface;
using Position_Implement;

namespace M2M
{
    /// <summary>
    /// ʹ�����������ʵ�ֵĽ�����ʽ����ʵ��ͬ��M2M���ݽṹ
    /// </summary>
    public abstract class AM2MStructure
    {
        //�õ���ʽ�ϵ�Level
        public abstract ILevel GetLevel(int levelSequecne);

        //�õ���ʽ�ϵ�Level��Ŀ
        public abstract int GetLevelNum();

        //�õ���ʽ�ϵ��ӷֿ鼯�ϣ�������ʽ�ϵ��ӷֿ��п��������ݽṹ�ϵ����²����ֿ鼯�ϣ�
        public abstract IPositionSet GetChildPositionSetByParentPart(int parentPartLevelSequence, IPart parentPart);

        public IPart GetPartRefByDescendantPart(int AncestorLevelSequence, IPart DescendantPart, int DescendantLevelSequence)
        {
            ILevel AncestorLevel = GetLevel(AncestorLevelSequence);
            int x = ((int)DescendantPart.GetX()) / (GetLevel(DescendantLevelSequence).GetUnitNumInWidth() / AncestorLevel.GetUnitNumInWidth());
            int y = ((int)DescendantPart.GetY()) / (GetLevel(DescendantLevelSequence).GetUnitNumInHeight() / AncestorLevel.GetUnitNumInHeight());
            return AncestorLevel.GetPartRefByPartIndex(x, y);
        }

        //�õ���ʽ�ϵĸ��ֿ�ָ��
        public IPart GetPartRefByChildPart(IPart childPart, int chlidPartSequence)
        {
            if (chlidPartSequence <= 0)
            {
                throw new Exception("chlidPartSequence <= 0");
            }

            return GetPartRefByDescendantPart(chlidPartSequence - 1, childPart, chlidPartSequence);
        }

        /// <summary>
        /// �õ�����ĳһ�ֿ���²���ָ����ķֿ鼯��
        /// </summary>
        /// <param name="DescendantLevelSequence"></param>
        /// <param name="AncestorPart"></param>
        /// <param name="AncestorLevelSequence"></param>
        /// <returns></returns>
        public IPositionSet GetDescendentPositionSetByAncestorPart(int DescendantLevelSequence, IPart AncestorPart, int AncestorLevelSequence)
        {
            return new PositionSet_DescendentPosition(DescendantLevelSequence, AncestorPart, AncestorLevelSequence, this);
        }

        public IPositionSet GetBottonLevelPositionSetByAncestorPart(IPart AncestorPart, int AncestorLevelSequence)
        {
            return GetDescendentPositionSetByAncestorPart(GetLevelNum(), AncestorPart, AncestorLevelSequence);
        } 

        class PositionSet_DescendentPosition : APositionSet<IPosition>, IPositionSet
        {
            int AncestorLevelSequence;
            int DescendantLevelSequence;

            AM2MStructure M2MS;

            IPositionSet childPositionSet = null;

            IPart AncestorPart;

            IPositionSet DescendentPositionSetOfChildPart = null;

            IPart childPart = null;

            private IPosition returnPosition = null;

            enum State { None, CurrentPartSubPointNumIsOne, ChildPartSubPointNumIsOne };

            State state = State.None;

            public PositionSet_DescendentPosition(int DescendantLevelSequence, IPart AncestorPart, int AncestorLevelSequence, AM2MStructure M2MS)
            {
                if (AncestorPart == null || M2MS == null)
                {
                    throw new Exception("null reference");
                }
                this.DescendantLevelSequence = DescendantLevelSequence;
                this.AncestorLevelSequence = AncestorLevelSequence;
                this.AncestorPart = AncestorPart;
                this.M2MS = M2MS;
            }

            override public void InitToTraverseSet()
            {
                if (true)
                {
                    if (AncestorLevelSequence == DescendantLevelSequence - 1)
                    {
                        DescendentPositionSetOfChildPart =
                            M2MS.GetChildPositionSetByParentPart(AncestorLevelSequence, AncestorPart);

                        DescendentPositionSetOfChildPart.InitToTraverseSet();
                    }
                    else if (AncestorLevelSequence > DescendantLevelSequence - 1)
                    {
                        throw new Exception("AncestorLevelSequence > DescendantLevelSequence - 1");
                    }
                    else
                    {
                        childPositionSet = M2MS.GetChildPositionSetByParentPart(AncestorLevelSequence, AncestorPart);

                        childPositionSet.InitToTraverseSet();

                        if (childPositionSet.NextPosition())
                        {
                            childPart = (IPart)childPositionSet.GetPosition();

                            if (true)
                            {
                                DescendentPositionSetOfChildPart =
                                    new PositionSet_DescendentPosition(DescendantLevelSequence,
                                                                       (IPart) childPositionSet.GetPosition(),
                                                                       AncestorLevelSequence + 1, M2MS);

                                DescendentPositionSetOfChildPart.InitToTraverseSet();
                            }
                            else
                            {
                                state = State.ChildPartSubPointNumIsOne;
                                state = State.None;

                            }
                        }
                    }
                }
            }

            override public bool NextPosition()
            {
                //if (state == State.ChildPartSubPointNumIsOne)
                //{
                //    returnPosition = childPart.GetRandomOneFormDescendantPoint();

                //    if (childPositionSet.NextPosition())
                //    {
                //        childPart = (IPart)childPositionSet.GetPosition();
                //        if (childPart.GetBottomLevelPointNum() != 1)
                //        {
                //            DescendentPositionSetOfChildPart =
                //                new PositionSet_DescendentPosition(DescendantLevelSequence,
                //                                                   childPart,
                //                                                   AncestorLevelSequence + 1, M2MS);

                //            DescendentPositionSetOfChildPart.InitToTraverseSet();

                //            state = State.None;
                //        }
                //        else
                //        {
                //            state = State.ChildPartSubPointNumIsOne;
                //            state = State.None;

                //        }
                //    }
                //    else
                //    {
                //        DescendentPositionSetOfChildPart = null;
                //        state = State.None;
                //    }

                //    return true;
                //}

                if(DescendentPositionSetOfChildPart == null)
                {
                    //if (state == State.CurrentPartSubPointNumIsOne)
                    //{
                    //    state = State.None;
                    //    returnPosition = AncestorPart.GetRandomOneFormDescendantPoint();
                    //    return true;
                    //}
                    return false;
                }

                if (DescendentPositionSetOfChildPart.NextPosition())
                {
                    returnPosition = DescendentPositionSetOfChildPart.GetPosition();
                    return true;
                }
                else
                {
                    if (childPositionSet == null)
                    {
                        return false;
                    }

                    if (childPositionSet.NextPosition())
                    {
                        childPart = (IPart)childPositionSet.GetPosition();
                        if (true)
                        {
                            if (AncestorLevelSequence >= DescendantLevelSequence - 2)
                            {
                                DescendentPositionSetOfChildPart =
                                    M2MS.GetChildPositionSetByParentPart(AncestorLevelSequence,
                                                                    childPart);

                                DescendentPositionSetOfChildPart.InitToTraverseSet();

                                if (DescendentPositionSetOfChildPart.NextPosition())
                                {
                                    returnPosition = DescendentPositionSetOfChildPart.GetPosition();
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                DescendentPositionSetOfChildPart =
                                    new PositionSet_DescendentPosition(DescendantLevelSequence,
                                                                       childPart,
                                                                       AncestorLevelSequence + 1, M2MS);

                                DescendentPositionSetOfChildPart.InitToTraverseSet();

                                if (DescendentPositionSetOfChildPart.NextPosition())
                                {
                                    returnPosition = DescendentPositionSetOfChildPart.GetPosition();
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            override public IPosition GetPosition()
            {
                return returnPosition;
            }
        }
    }
}