using System;
using System.Collections.Generic;
using System.Text;

namespace M2M
{
    /*
    public class M2M_NN1 : INearestNeighbor
    {
        const float bias = 0.01f;

        float MapWidth;
        float MapHeight;

        float MaxGridLength;
        int UnitNumInGridLength;
        int levelNum;

        List<Level> LevelList = new List<Level>();

        Level GetLevel(int num)
        {
            return LevelList[num];
        }

        int GetLevelNum()
        {
            return LevelList.Count;
        }

        List<IPosition> root = new List<IPosition>();

        public void Init(float MapWidth, float MapHeight, float MaxGridLength, int UnitNumInLength, int levelNum)
        {
            this.MapWidth = MapWidth;
            this.MapHeight = MapHeight;

            this.MaxGridLength = MaxGridLength;
            this.UnitNumInGridLength = UnitNumInLength;
            this.levelNum = levelNum;
        }

        private void InitMemory()
        {
            Level level0 = new Level();

            level0.SetUnitNumInWidth((int)Math.Ceiling(MapWidth / MaxGridLength));
            level0.SetUnitNumInHeight((int)Math.Ceiling(MapHeight / MaxGridLength));

            level0.SetGridWidth(MaxGridLength);
            level0.SetGridHeight(MaxGridLength);

            level0.AllocateMemory();

            LevelList.Clear();
            LevelList.Add(level0);

            for (int levelSequence = 1; levelSequence < levelNum; levelSequence++)
            {
                Level level = new Level();

                level.SetUnitNumInWidth(UnitNumInGridLength * LevelList[levelSequence - 1].GetUnitNumInWidth());
                level.SetUnitNumInHeight(UnitNumInGridLength * LevelList[levelSequence - 1].GetUnitNumInHeight());

                level.SetGridWidth(LevelList[levelSequence - 1].GetGridWidth() / UnitNumInGridLength);
                level.SetGridHeight(LevelList[levelSequence - 1].GetGridHeight() / UnitNumInGridLength);

                level.AllocateMemory();

                LevelList.Add(level);
            }

            ////////////Ϊ������Ա����ռ�/////////////
            SearchEverPartList.Capacity = 25;
            WaitToSearchPartList.Capacity = 25;
        }

        public void PreProcess(List<IPosition> pointList)
        {
            int PointNum = pointList.Count;

            float MaxLargeX = float.MinValue;
            float MaxLargeY = float.MinValue;

            foreach (IPosition point in pointList)
            {
                if (point.GetX() > MaxLargeX)
                {
                    MaxLargeX = point.GetX();
                }

                if (point.GetY() > MaxLargeY)
                {
                    MaxLargeY = point.GetY();
                }
            }

            //float mapWidth = MaxLargeX + 0.0001f;
            //float mapHeight = MaxLargeY + 0.0001f;

            float mapWidth = MaxLargeX;
            float mapHeight = MaxLargeY;

            UnitNumInGridLength = 5;
            int MicPartNumInMacPart = UnitNumInGridLength * UnitNumInGridLength;

            MaxGridLength = mapWidth / UnitNumInGridLength;

            if (MaxGridLength > mapHeight / UnitNumInGridLength)
            {
                MaxGridLength = mapHeight / UnitNumInGridLength;
            }

            MaxGridLength += bias;
            
            levelNum = (int)Math.Ceiling(Math.Log(PointNum * 1.7, MicPartNumInMacPart));
            //levelNum = 4;

            MapWidth = mapWidth;
            MapHeight = mapHeight;

            InitMemory();

            foreach (IPosition point in pointList)
            {
                Insert(point);
            }
        }

        public void Insert(IPosition point)
        {
            IPosition NewPosition = point;

            for (int levelSequrence = LevelList.Count - 1; levelSequrence >= -1; levelSequrence--)
            {
                if (levelSequrence == -1)
                {
                    root.Add(NewPosition);
                    break;
                }

                Part currentPart = LevelList[levelSequrence].GOCPartRefByPoint(point);

                currentPart.AddToSubPositionList(NewPosition);
                
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

            //���²�ͬ������õ����ڵ�part�е����Ŀ
            for (int levelSequrence = LevelList.Count - 1; levelSequrence >= 0; levelSequrence--)
            {
                ((Part)LevelList[levelSequrence].GetPartRefByPoint(point)).SubPointNumIncrease(1);
            }
        }

        public void Remove(IPosition point)
        {
            //���²�ͬ������õ����ڵ�part�е����Ŀ
            for (int levelSequrence = LevelList.Count - 1; levelSequrence >= 0; levelSequrence--)
            {
                ((Part)LevelList[levelSequrence].GetPartRefByPoint(point)).SubPointNumDecrease(1);
            }

            IPosition RemovePosition = point;

            for (int levelSequrence = LevelList.Count; levelSequrence >= -1; levelSequrence--)
            {
                if (levelSequrence == -1)
                {
                    root.Remove(RemovePosition);
                    break;
                }

                Part currentPart = (Part)LevelList[levelSequrence].GetPartRefByPoint(point);

                if(currentPart == null)
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
                    //�����part���泬��һ��������Խ���ɾ������,������Ҫ�ж�һ�¸�part�Լ����������part�Ĵ�����Ƿ�����ɾ����,�����,��Ҫ����.
                    for (int levelSequrence2 = levelSequrence; levelSequrence2 >= 0; levelSequrence2++)
                    {
                        IPosition deputyPoint = LevelList[levelSequrence2].GetPartRefByPoint(point).GetRandomOneFormDescendantPoint();
                        if (point == deputyPoint)
                        {
                            deputyPoint = currentPart.GetRandomPositionFormSubPositionList();
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

        private Level SearchTheTargetPart(IPosition targetPoint, ref Part targetPart)
        {
            int levelSequence = 0;

            if (LevelList[0].GetPartRefByPoint(targetPoint) == null)
            {
                //���������û�д���
                //throw new Exception("���������û�д���!");
                
                targetPart = LevelList[0].GetPartRefByPoint(targetPoint);

                levelSequence = 0;
            }
            else
            {
                for (levelSequence = 1; levelSequence < LevelList.Count; levelSequence++)
                {
                    //����÷ֿ鲻����,˵���÷ֿ�����û��������.
                    if (LevelList[levelSequence].GetPartRefByPoint(targetPoint) == null)
                    {
                        targetPart = LevelList[levelSequence - 1].GetPartRefByPoint(targetPoint);

                        break;
                    }
                }

                levelSequence--;

                if (targetPart == null)
                {
                    targetPart = LevelList[LevelList.Count - 1].GetPartRefByPoint(targetPoint);

                    levelSequence = LevelList.Count - 1;
                }
            }

            return LevelList[levelSequence];
        }

        public IPosition ApproximateNearestNeighbor(IPosition targetPoint)
        {
            Part targetPart = null;

            SearchTheTargetPart(targetPoint, ref targetPart);

            TravelThePointInPart travelThePointInPart = new TravelThePointInPart();

            travelThePointInPart.InitToGetTheNearestPointInPart(targetPart,targetPoint);

            return travelThePointInPart.GetTheNearestPointInPart();
        }

        //��ȷ���������ÿ�ζ����õ�.
        List<Part> SearchEverPartList = new List<Part>();
        List<Part> WaitToSearchPartList = new List<Part>();

        public IPosition NearestNeighbor2(IPosition point)
        {
            //��ʼ������
            SearchEverPartList.Clear();
            IPosition CurrentNearestPoint;
            
            float CurrentNearestDistanceSquare = float.MaxValue;
            TravelThePointInPart travelThePointInPart = new TravelThePointInPart();


            Part targetPart = null;
            Level targetPartLevel = SearchTheTargetPart(point, ref targetPart);
            
            travelThePointInPart.InitToGetTheNearestPointInPart(targetPart, point);
            CurrentNearestPoint = travelThePointInPart.GetTheNearestPointInPart();
            CurrentNearestDistanceSquare = travelThePointInPart.GetNearestDistanceSquare();
            SearchEverPartList.Add(targetPart);

            while (true)
            {
                DetermineWaitToSearchPartList(point, CurrentNearestDistanceSquare, targetPartLevel);

                //(δʵ��)
                //��WaitToSearchPartListÿ���ֿ鰴�÷ֿ�Ĵ���㵽�����ľ�����һ��ð��,����ԽС��Խ�类ѡ��.
                //�����С����ȵ�ǰ��С��������С,Ӧ�ø�����С��������,��������С�����,�����»�������������,���¿�ʼ����.

                foreach (Part currentCheckedPart in WaitToSearchPartList)
                {
                    travelThePointInPart.InitToGetTheNearestPointInPart(currentCheckedPart, point);

                    IPosition CurrentPartNearestPoint = travelThePointInPart.GetTheNearestPointInPart();
                    float CurrentPartNearestDistanceSquare = travelThePointInPart.GetNearestDistanceSquare();
                    SearchEverPartList.Add(currentCheckedPart);

                    //����ڸ÷ֿ������ҵ��ĵ�ľ����ԭ������̾�������С,�������С��������,������С�����,�����»�������������,���¿�ʼ����.
                    if (CurrentPartNearestDistanceSquare < CurrentNearestDistanceSquare)
                    {
                        CurrentNearestDistanceSquare = CurrentPartNearestDistanceSquare;
                        CurrentNearestPoint = CurrentPartNearestPoint;
                        continue;
                    }
                }

                break;
            }

            return CurrentNearestPoint;
        }

        private Level SearchTheTargetPart2(IPosition targetPoint, ref Part targetPart)
        {
            int maxCheckPointNum = 18;

            int levelSequence = 0;

            for (levelSequence = 0; levelSequence < LevelList.Count; levelSequence++)
            {
                Part currentPart = LevelList[levelSequence].GetPartRefByPoint(targetPoint);

                if ((levelSequence == 0) && (currentPart == null))
                {
                    targetPart = null;

                    return LevelList[0];
                }

                if ((currentPart == null) || (currentPart.GetSubPointNum() <= maxCheckPointNum))
                {
                    targetPart = currentPart;

                    return LevelList[levelSequence];
                }
            }

            if (targetPart == null)
            {
                targetPart = LevelList[LevelList.Count - 1].GetPartRefByPoint(targetPoint);

                return LevelList[LevelList.Count - 1];
            }

            throw new Exception("��Ӧ�õ�������!");

            return LevelList[0];
        }

        enum HorizontalSearchPriority { left, right }
        enum VerticalSearchPriority { top, bottom }

        int leftSearchLine, rightSearchLine, bottomSearchLine, topSearchLine;

        //�㼯�����߽�
        int upperBound, lowerBound, leftBound, rightBound;

        public IPosition NearestNeighbor(IPosition targetPoint)
        {
            //��ʼ������
            IPosition CurrentNearestPoint = null;

            float CurrentNearestDistanceSquare = float.MaxValue;
            float CurrentNearestDistance = float.MaxValue;

            TravelThePointInPart travelThePointInPart = new TravelThePointInPart();

            Part targetPart = null;
            Level targetPartLevel = SearchTheTargetPart2(targetPoint, ref targetPart);

            HorizontalSearchPriority horizontalSearchPriority;
            VerticalSearchPriority verticalSearchPriority;

            int horizontalForewardSearchValue;
            int horizontalAfterwardSearchValue;
            int verticalForewardSearchValue;
            int verticalAfterwardSearchValue;

            int targetPartX = targetPartLevel.ConvertRealityValueToPartSequenceX(targetPoint.GetX());
            int targetPartY = targetPartLevel.ConvertRealityValueToPartSequenceY(targetPoint.GetY());

            //����������

            //������ڷֿ����벿��
            if (targetPoint.GetX() <= targetPartLevel.ConvertPartSequenceXToRealityValue(Convert.ToInt32(targetPartX)) + targetPartLevel.GetGridWidth() / 2)
            {
                leftSearchLine = targetPartX - 1;
                rightSearchLine = targetPartX;
                horizontalSearchPriority = HorizontalSearchPriority.right;
            }
            else
            {
                leftSearchLine = targetPartX;
                rightSearchLine = targetPartX + 1;
                horizontalSearchPriority = HorizontalSearchPriority.left;
            }

            //������ڷֿ���°벿��
            if (targetPoint.GetY() <= targetPartLevel.ConvertPartSequenceYToRealityValue(Convert.ToInt32(targetPartY)) + targetPartLevel.GetGridHeight() / 2)
            {
                bottomSearchLine = targetPartY - 1;
                topSearchLine = targetPartY;
                verticalSearchPriority = VerticalSearchPriority.top;
            }
            else
            {
                bottomSearchLine = targetPartY;
                topSearchLine = targetPartY + 1;
                verticalSearchPriority = VerticalSearchPriority.bottom;
            }

            //�ѵ㼯�߽���Ϊ�����߽�
            upperBound = targetPartLevel.GetUnitNumInHeight() - 1;
            lowerBound = 0;
            leftBound = 0;
            rightBound = targetPartLevel.GetUnitNumInWidth() - 1;
                              
            while (true)
            {
                //�ж��������Ƿ��ڵ�ͼ�߽�����.
                //ISTheSearchLineInsideMapAndUpdateTheSearchLine(targetPartLevel);

                if (verticalSearchPriority == VerticalSearchPriority.top)
                {
                    verticalForewardSearchValue = topSearchLine;
                    verticalAfterwardSearchValue = bottomSearchLine;
                }
                else
                {
                    verticalForewardSearchValue = bottomSearchLine;
                    verticalAfterwardSearchValue = topSearchLine;
                }

                if (horizontalSearchPriority == HorizontalSearchPriority.right)
                {
                    horizontalForewardSearchValue = rightSearchLine;
                    horizontalAfterwardSearchValue = leftSearchLine;
                }
                else
                {
                    horizontalForewardSearchValue = leftSearchLine;
                    horizontalAfterwardSearchValue = rightSearchLine;
                }

                //if ((verticalForewardSearchValue < point.GetY() + CurrentNearestDistance) && (verticalForewardSearchValue > point.GetY() - CurrentNearestDistance))

                bool isNeedToSearch = true;

                ///////////////////////��ֱ����////////////////////////
                isNeedToSearch = true;

                if (verticalSearchPriority == VerticalSearchPriority.top)
                {
                    if (verticalForewardSearchValue > upperBound)
                    {
                        isNeedToSearch = false;
                    }
                }
                else
                {
                    if (verticalForewardSearchValue < lowerBound)
                    {
                        isNeedToSearch = false;
                    }
                }

                if (isNeedToSearch)
                {
                    for (int j = leftSearchLine + 1; j <= rightSearchLine - 1; j++)
                    {
                        if (ISTheCoordinateInsideSearchBound(j, verticalForewardSearchValue))
                        {
                            Part currentCheckedPart = targetPartLevel.GetPartRefByPartIndex(j, verticalForewardSearchValue);
                            if (currentCheckedPart != null)
                            {
                                SearchNearestPointInOnePart(targetPoint, ref CurrentNearestPoint, ref CurrentNearestDistanceSquare, ref CurrentNearestDistance, travelThePointInPart, targetPartLevel, currentCheckedPart);
                            }
                        }
                    }
                }

                ///////////////////////ˮƽ����////////////////////////
                isNeedToSearch = true;

                if (horizontalSearchPriority == HorizontalSearchPriority.right)
                {
                    if (horizontalForewardSearchValue > rightBound)
                    {
                        isNeedToSearch = false;
                    }
                }
                else
                {
                    if (horizontalForewardSearchValue < leftBound)
                    {
                        isNeedToSearch = false;
                    }
                }

                if (isNeedToSearch)
                {
                    for (int i = bottomSearchLine + 1; i <= topSearchLine - 1; i++)
                    {
                        if (ISTheCoordinateInsideSearchBound(horizontalForewardSearchValue, i))
                        {
                            Part currentCheckedPart = targetPartLevel.GetPartRefByPartIndex(horizontalForewardSearchValue, i);
                            if (currentCheckedPart != null)
                            {
                                SearchNearestPointInOnePart(targetPoint, ref CurrentNearestPoint, ref CurrentNearestDistanceSquare, ref CurrentNearestDistance, travelThePointInPart, targetPartLevel, currentCheckedPart);
                            }
                        }
                    }
                }

                ///////////////////////��ֱ�ͺ�////////////////////////
                isNeedToSearch = true;

                if (verticalSearchPriority == VerticalSearchPriority.top)
                {
                    if (verticalAfterwardSearchValue < lowerBound)
                    {
                        isNeedToSearch = false;
                    }
                }
                else
                {
                    if (verticalAfterwardSearchValue > upperBound)
                    {
                        isNeedToSearch = false;
                    }
                }

                if (isNeedToSearch)
                {
                    for (int j = leftSearchLine + 1; j <= rightSearchLine - 1; j++)
                    {
                        if (ISTheCoordinateInsideSearchBound(j, verticalAfterwardSearchValue))
                        {
                            Part currentCheckedPart = targetPartLevel.GetPartRefByPartIndex(j, verticalAfterwardSearchValue);
                            if (currentCheckedPart != null)
                            {
                                SearchNearestPointInOnePart(targetPoint, ref CurrentNearestPoint, ref CurrentNearestDistanceSquare, ref CurrentNearestDistance, travelThePointInPart, targetPartLevel, currentCheckedPart);
                            }
                        }
                    }
                }

                ///////////////////////ˮƽ�ͺ�////////////////////////
                isNeedToSearch = true;

                if (horizontalSearchPriority == HorizontalSearchPriority.right)
                {
                    if (horizontalAfterwardSearchValue < leftBound)
                    {
                        isNeedToSearch = false;
                    }
                }
                else
                {
                    if (horizontalAfterwardSearchValue > rightBound)
                    {
                        isNeedToSearch = false;
                    }
                }

                if (isNeedToSearch)
                {
                    for (int i = bottomSearchLine + 1; i <= topSearchLine - 1; i++)
                    {
                        if (ISTheCoordinateInsideSearchBound(horizontalAfterwardSearchValue, i))
                        {
                            Part currentCheckedPart = targetPartLevel.GetPartRefByPartIndex(horizontalAfterwardSearchValue, i);
                            if (currentCheckedPart != null)
                            {
                                SearchNearestPointInOnePart(targetPoint, ref CurrentNearestPoint, ref CurrentNearestDistanceSquare, ref CurrentNearestDistance, travelThePointInPart, targetPartLevel, currentCheckedPart);
                            }
                        }
                    }
                }

                ///////////////////////���Ƚ���////////////////////////
                isNeedToSearch = true;

                if (verticalSearchPriority == VerticalSearchPriority.top)
                {
                    if (verticalForewardSearchValue > upperBound)
                    {
                        isNeedToSearch = false;
                    }
                }
                else
                {
                    if (verticalForewardSearchValue < lowerBound)
                    {
                        isNeedToSearch = false;
                    }
                }

                if (horizontalSearchPriority == HorizontalSearchPriority.right)
                {
                    if (horizontalForewardSearchValue > rightBound)
                    {
                        isNeedToSearch = false;
                    }
                }
                else
                {
                    if (horizontalForewardSearchValue < leftBound)
                    {
                        isNeedToSearch = false;
                    }
                }

                if (isNeedToSearch)
                {
                    Part currentCheckedPart = targetPartLevel.GetPartRefByPartIndex(horizontalForewardSearchValue, verticalForewardSearchValue);
                    if (currentCheckedPart != null)
                    {
                        SearchNearestPointInOnePart(targetPoint, ref CurrentNearestPoint, ref CurrentNearestDistanceSquare, ref CurrentNearestDistance, travelThePointInPart, targetPartLevel, currentCheckedPart);
                    }
                }

                ///////////////////////��ֱ����ˮƽ�ͺ����////////////////////////
                isNeedToSearch = true;

                if (verticalSearchPriority == VerticalSearchPriority.top)
                {
                    if (verticalForewardSearchValue > upperBound)
                    {
                        isNeedToSearch = false;
                    }
                }
                else
                {
                    if (verticalForewardSearchValue < lowerBound)
                    {
                        isNeedToSearch = false;
                    }
                }

                if (horizontalSearchPriority == HorizontalSearchPriority.right)
                {
                    if (horizontalAfterwardSearchValue < leftBound)
                    {
                        isNeedToSearch = false;
                    }
                }
                else
                {
                    if (horizontalAfterwardSearchValue > rightBound)
                    {
                        isNeedToSearch = false;
                    }
                }

                if (isNeedToSearch)
                {
                    Part currentCheckedPart = targetPartLevel.GetPartRefByPartIndex(horizontalAfterwardSearchValue, verticalForewardSearchValue);
                    if (currentCheckedPart != null)
                    {
                        SearchNearestPointInOnePart(targetPoint, ref CurrentNearestPoint, ref CurrentNearestDistanceSquare, ref CurrentNearestDistance, travelThePointInPart, targetPartLevel, currentCheckedPart);                    }
                }

                ///////////////////////��ֱ�ͺ�ˮƽ���Ƚ���////////////////////////
                isNeedToSearch = true;

                if (verticalSearchPriority == VerticalSearchPriority.top)
                {
                    if (verticalAfterwardSearchValue < lowerBound)
                    {
                        isNeedToSearch = false;
                    }
                }
                else
                {
                    if (verticalAfterwardSearchValue > upperBound)
                    {
                        isNeedToSearch = false;
                    }
                }

                if (horizontalSearchPriority == HorizontalSearchPriority.right)
                {
                    if (horizontalForewardSearchValue > rightBound)
                    {
                        isNeedToSearch = false;
                    }
                }
                else
                {
                    if (horizontalForewardSearchValue < leftBound)
                    {
                        isNeedToSearch = false;
                    }
                }

                if (isNeedToSearch)
                {
                    Part currentCheckedPart = targetPartLevel.GetPartRefByPartIndex(horizontalForewardSearchValue, verticalAfterwardSearchValue);
                    if (currentCheckedPart != null)
                    {
                        SearchNearestPointInOnePart(targetPoint, ref CurrentNearestPoint, ref CurrentNearestDistanceSquare, ref CurrentNearestDistance, travelThePointInPart, targetPartLevel, currentCheckedPart);                    
                    }
                }

                ///////////////////////��ֱ�ͺ�ˮƽ�ͺ����////////////////////////

                if (verticalSearchPriority == VerticalSearchPriority.top)
                {
                    if (verticalAfterwardSearchValue < lowerBound)
                    {
                        isNeedToSearch = false;
                    }
                }
                else
                {
                    if (verticalAfterwardSearchValue > upperBound)
                    {
                        isNeedToSearch = false;
                    }
                }

                if (horizontalSearchPriority == HorizontalSearchPriority.right)
                {
                    if (horizontalAfterwardSearchValue < leftBound)
                    {
                        isNeedToSearch = false;
                    }
                }
                else
                {
                    if (horizontalAfterwardSearchValue > rightBound)
                    {
                        isNeedToSearch = false;
                    }
                }

                if (isNeedToSearch)
                {
                    Part currentCheckedPart = targetPartLevel.GetPartRefByPartIndex(horizontalAfterwardSearchValue, verticalAfterwardSearchValue);
                    if (currentCheckedPart != null)
                    {
                        SearchNearestPointInOnePart(targetPoint, ref CurrentNearestPoint, ref CurrentNearestDistanceSquare, ref CurrentNearestDistance, travelThePointInPart, targetPartLevel, currentCheckedPart);
                    }
                }

                //���������߽�
                leftSearchLine -= 1;
                rightSearchLine += 1;
                bottomSearchLine -= 1;
                topSearchLine += 1;

                if ((leftSearchLine < leftBound) && (rightSearchLine > rightBound) && (topSearchLine > upperBound) && (bottomSearchLine < lowerBound))
                {
                    break;
                }
            }

            return CurrentNearestPoint;
        }

        private bool ISTheCoordinateInsideSearchBound(int x, int y)
        {
            return (x <= rightBound && x >= leftBound && y <= upperBound && y >= lowerBound);
        }

        private bool ISThePartInsideSearchBound(Part currentCheckedPart)
        {
            return ((currentCheckedPart.GetX()) <= rightBound) && ((currentCheckedPart.GetX()) >= leftBound)
                                        && ((currentCheckedPart.GetY()) <= upperBound) && ((currentCheckedPart.GetX()) >= lowerBound);
        }

        //private void ISTheSearchLineInsideMapAndUpdateTheSearchLine(Level targetPartLevel)
        //{
        //    //�ж��������Ƿ��ڵ�ͼ�߽�����.
        //    if (topSearchLine > targetPartLevel.GetUnitNumInHeight() - 1)
        //    {
        //        topSearchLine = targetPartLevel.GetUnitNumInHeight() - 1;
        //    }

        //    if (bottomSearchLine < 0)
        //    {
        //        bottomSearchLine = 0;
        //    }

        //    if (rightSearchLine > targetPartLevel.GetUnitNumInWidth() - 1)
        //    {
        //        rightSearchLine = targetPartLevel.GetUnitNumInWidth() - 1;
        //    }

        //    if (leftSearchLine < 0)
        //    {
        //        leftSearchLine = 0;
        //    }
        //}

        private void SearchNearestPointInOnePart(IPosition targetPoint, ref IPosition CurrentNearestPoint, ref float CurrentNearestDistanceSquare, ref float CurrentNearestDistance, TravelThePointInPart travelThePointInPart, Level targetPartLevel, Part currentCheckedPart)
        {
            travelThePointInPart.InitToGetTheNearestPointInPart(currentCheckedPart, targetPoint);

            IPosition CurrentPartNearestPoint = travelThePointInPart.GetTheNearestPointInPart();
            float CurrentPartNearestDistanceSquare = travelThePointInPart.GetNearestDistanceSquare();

            //����ڸ÷ֿ������ҵ��ĵ�ľ����ԭ������̾�������С,�������С��������,������С�����,��������������߽�
            if (CurrentPartNearestDistanceSquare < CurrentNearestDistanceSquare)
            {
                CurrentNearestDistanceSquare = CurrentPartNearestDistanceSquare;
                CurrentNearestPoint = CurrentPartNearestPoint;

                CurrentNearestDistance = (float)Math.Sqrt((double)CurrentNearestDistanceSquare);

                //���������߽�
                upperBound = targetPartLevel.ConvertRealityValueToPartSequenceY(targetPoint.GetY() + CurrentNearestDistance);
                lowerBound = targetPartLevel.ConvertRealityValueToPartSequenceY(targetPoint.GetY() - CurrentNearestDistance);
                leftBound = targetPartLevel.ConvertRealityValueToPartSequenceX(targetPoint.GetX() - CurrentNearestDistance);
                rightBound = targetPartLevel.ConvertRealityValueToPartSequenceX(targetPoint.GetX() + CurrentNearestDistance);

                //�ж������߽��Ƿ��ڵ�ͼ�߽�����
                if (upperBound > targetPartLevel.GetUnitNumInHeight() - 1)
                {
                    upperBound = targetPartLevel.GetUnitNumInHeight() - 1;
                }

                if (lowerBound < 0)
                {
                    lowerBound = 0;
                }

                if (rightBound > targetPartLevel.GetUnitNumInWidth() - 1)
                {
                    rightBound = targetPartLevel.GetUnitNumInWidth() - 1;
                }

                if (leftBound < 0)
                {
                    leftBound = 0;
                }
            }
        }

        private static void SearchNearestPointOnOneSide(IPosition targetPoint, ref IPosition CurrentNearestPoint, ref float CurrentNearestDistanceSquare, ref float CurrentNearestDistance, TravelThePointInPart travelThePointInPart, Level targetPartLevel, int leftSearchLine, int rightSearchLine, ref int upperBound, ref int lowerBound, ref int leftBound, ref int rightBound)
        {
            //for (int j = leftSearchLine + 1; j <= rightSearchLine - 1; j++)
            //{
            //    Part currentCheckedPart = targetPartLevel.GetPartRefByPartIndex(j, verticalForewardSearchValue);
            //    if (currentCheckedPart != null)
            //    {
            //        travelThePointInPart.InitToGetTheNearestPointInPart(currentCheckedPart, targetPoint);

            //        Position CurrentPartNearestPoint = travelThePointInPart.GetTheNearestPointInPart();
            //        float CurrentPartNearestDistanceSquare = travelThePointInPart.GetNearestDistanceSquare();

            //        //����ڸ÷ֿ������ҵ��ĵ�ľ����ԭ������̾�������С,�������С��������,������С�����,��������������߽�
            //        if (CurrentPartNearestDistanceSquare < CurrentNearestDistanceSquare)
            //        {
            //            CurrentNearestDistanceSquare = CurrentPartNearestDistanceSquare;
            //            CurrentNearestPoint = CurrentPartNearestPoint;

            //            CurrentNearestDistance = (float)Math.Sqrt((double)CurrentNearestDistanceSquare);

            //            //���������߽�
            //            upperBound = targetPartLevel.ConvertRealityValueToPartSequenceY(targetPoint.GetY() + CurrentNearestDistance);
            //            lowerBound = targetPartLevel.ConvertRealityValueToPartSequenceY(targetPoint.GetY() - CurrentNearestDistance);
            //            leftBound = targetPartLevel.ConvertRealityValueToPartSequenceX(targetPoint.GetX() - CurrentNearestDistance);
            //            rightBound = targetPartLevel.ConvertRealityValueToPartSequenceX(targetPoint.GetX() + CurrentNearestDistance);
            //        }
            //    }
            //}         
        }

        private void DetermineWaitToSearchPartList(IPosition point, float CurrentNearestDistanceSquare, Level targetPartLevel)
        {
            float CurrentNearestDistance = (float)Math.Sqrt((double)CurrentNearestDistanceSquare);

            int upperBound = targetPartLevel.ConvertRealityValueToPartSequenceY(point.GetY() + CurrentNearestDistance);
            int lowerBound = targetPartLevel.ConvertRealityValueToPartSequenceY(point.GetY() - CurrentNearestDistance);
            int leftBound = targetPartLevel.ConvertRealityValueToPartSequenceX(point.GetX() - CurrentNearestDistance);
            int rightBound = targetPartLevel.ConvertRealityValueToPartSequenceX(point.GetX() + CurrentNearestDistance);

            WaitToSearchPartList.Clear();

            for (int i = lowerBound; i <= upperBound; i++)
            {
                for (int j = leftBound; j <= rightBound; j++)
                {
                    if (!targetPartLevel.IndexIsExceedPartRange(j, i))
                    {
                        Part currentCheckedPart = targetPartLevel.GetPartRefByPartIndex(j, i);
                        if ((currentCheckedPart != null) && (!SearchEverPartList.Contains(currentCheckedPart)))
                        {
                            WaitToSearchPartList.Add(currentCheckedPart);
                        }
                    }
                }
            }
        }
        
        class TravelThePointInPart
        {
            Part currentPart;
            IPosition comparePoint;
            IPosition nearestPointToComparePoint;
            float nearestDistanceSquare;

            public float GetNearestDistanceSquare()
            {
                return nearestDistanceSquare;
            }

            float CalculateTheDistanceFromComparePoint(IPosition point)
            {
                float dx = point.GetX() - comparePoint.GetX();
                float dy = point.GetY() - comparePoint.GetY();

                return dx * dx + dy * dy;
            }

            public void InitToGetTheNearestPointInPart(Part part,IPosition point)
            {
                currentPart = part;
                comparePoint = point;
                nearestDistanceSquare = float.MaxValue;
            }

            void CompareToThePoint(IPosition point)
            {
                float currentDistance = CalculateTheDistanceFromComparePoint(point);
                if (nearestDistanceSquare > currentDistance)
                {
                    nearestPointToComparePoint = point;
                    nearestDistanceSquare = currentDistance;
                }
            }

            public IPosition GetTheNearestPointInPart()
            {
                //currentPart.OnGetOnePoint = this.CompareToThePoint;

                currentPart.OnGetOnePoint += CompareToThePoint;

                currentPart.TravelAllPointInPart();

                currentPart.OnGetOnePoint -= CompareToThePoint;

                return nearestPointToComparePoint;
            }
        }        
    }
    */
}