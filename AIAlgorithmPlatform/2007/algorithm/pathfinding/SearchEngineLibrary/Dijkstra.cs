using System;
using System.Collections.Generic;
using System.Text;
using DataStructure;
using DataStructure.PriorityQueue;
using Position_Connected_Interface;

namespace SearchEngineLibrary
{
    //Dijkstra�㷨
    public class Dijkstra : ISearchPathEngine
    {
        IComparer<IPosition_Connected> com;
        IPositionSet_Connected positionSet;
        ushort time_stamp;//ÿ��Ѱ��֮ǰ������һ���µ�ʱ����������ж�ĳ���ڵ����Ϣ����Ч���ǹ��ڵ�

        float pathLength = 0;

        public float GetPathLength()
        {
            return pathLength;
        }

        public Dijkstra()
        {
            com = new DijkstraTagComparer();
            time_stamp = 0;
        }

        //��ʼ����ͼ���ڵ�ͼ������֮�����
        public void InitEngineForMap(IPositionSet_Connected map)
        {
            positionSet = map;
            IPosition_Connected p;
            
            //��ʼ��
            positionSet.InitToTraverseSet();
            while (positionSet.NextPosition())
            {
                p = positionSet.GetPosition_Connected();
                p.SetAttachment(new Tag());
            }

            int num = (int)(Math.Sqrt((double)positionSet.GetNum()));

            path = new List<IPosition_Connected>(num * 2);

            if (num > 0)
                open = new PriorityQueue<IPosition_Connected>(num * 4, com);
            else
                open = new PriorityQueue<IPosition_Connected>(com);
        }

        //ÿ��Ѱ��֮ǰ������㡢�յ���Ϣ����ʼ��
        bool Init(IPosition_Connected end, IPosition_Connected start)
        {
            //�ж������յ��Ƿ��ڵ�ͼ�ϣ�����ʼ�����ı�ǩ
            Tag tag = (Tag)end.GetAttachment();

            //�������Ѱ��ʹ�õ�time stamp
            time_stamp = TimeStamp.getNextTimeStamp(time_stamp);

            if (tag != null)
            {
                tag.g = 0;
                tag.parent = null;
                tag.closed = false;
                tag.timeStamp = time_stamp;
            }
            else
                return false;
            if (start.GetAttachment() == null)
                return false;

            return true;
        }

        List<IPosition_Connected> path;
        IPriorityQueue<IPosition_Connected> open;

        //û��·���򷵻�null
        public List<IPosition_Connected> SearchPath(IPosition_Connected start, IPosition_Connected end)
        {
            path.Clear();
            
            if (end == start)
            {
                path.Add(end);
                return path;
            }

            //��ʼ��
            if (!Init(end, start))
                return null;

            IPosition_Connected p = null, p_adj;
            IPositionSet_Connected_Adjacency adj_set;
            Tag p_tag, p_adj_tag;
            float newG;

            //int count = 0;
            //if (debug)
            //    Console.WriteLine("SearchPath:");

            open.clear();

            //��������open��
            open.add(end);

            //��open��ǿ�ʱ
            while (open.getSize() > 0)
            {
                //�����һ������ĵ�
                p = open.removeFirst();
                //�����յ������
                if (p == start)
                {
                    pathLength = ((Tag)start.GetAttachment()).g;
                    break;
                }
                p_tag = (Tag)p.GetAttachment();
                p_tag.closed = true;
                adj_set = p.GetAdjacencyPositionSet();
                adj_set.InitToTraverseSet();

                //if (debug)
                //{
                //    count++;
                //    Console.Write(p.ToString() + "\t");
                //}

                while (adj_set.NextPosition())
                {
                    p_adj = adj_set.GetPosition_Connected();
                    p_adj_tag = (Tag)p_adj.GetAttachment();

                    //���δ������
                    if (p_adj_tag.timeStamp != time_stamp)
                    {
                        newG = p_tag.g + adj_set.GetDistanceToAdjacency();

                        p_adj_tag.parent = p;
                        p_adj_tag.g = newG;
                        p_adj_tag.closed = false;
                        p_adj_tag.timeStamp = time_stamp;
                        open.add(p_adj);
                    }
                    else
                    {
                        if (!p_adj_tag.closed)
                        {
                            newG = p_tag.g + adj_set.GetDistanceToAdjacency();
                            if (newG < p_adj_tag.g)
                            {
                                p_adj_tag.parent = p;
                                p_adj_tag.g = newG;
                                open.update(p_adj);
                            }
                        }
                    }

                    //if (debug)
                    //    Console.Write(p_adj.ToString() + ":" + p_adj.GetAttachment().ToString() + "\t");
                }
                //if (debug)
                //    Console.WriteLine();
            }

            //if (debug)
            //{
            //    Console.Write("position count:");
            //    Console.WriteLine(count);
            //}

            //���յ���ݱ�ǩ�м�¼�ĸ��ڵ��ҵ���㣬����·��       
            if (p == start)
            {
                path.Add(start);
                p_tag = (Tag)start.GetAttachment();
                while (p_tag.parent != null)
                {
                    path.Add(p_tag.parent);
                    p_tag = (Tag)p_tag.parent.GetAttachment();
                }
                //path.Reverse();
                return path;
            }
            else
                return null;
        }
    }
}
