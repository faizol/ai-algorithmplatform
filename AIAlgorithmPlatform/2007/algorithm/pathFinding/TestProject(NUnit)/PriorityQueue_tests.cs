using System;
using System.Collections.Generic;
using System.Text;

using DataStructure;

using NUnit.Framework;

namespace TestProject
{
    [TestFixture]
    public class PriorityQueue_tests
    {
        TestComparer peopleComparator = new TestComparer();
        IPriorityQueue<People> q1, q2;
        int count, range;

        [SetUp]
        public void SetUp()
        {
            count = 10000;//�������ݵ�����
            range = (int)Math.Sqrt(count);//�������ݵ�ȡֵ��Χ0~range-1
        }

        [Test]
        public void testPriorityList()
        {
            Console.WriteLine("PriorityList");
            q1 = new PriorityList<People>(peopleComparator);
            List<People> list = add_remove_removeFirst_test();
            q2 = new PriorityList<People>(q1);
            constructorTest(list);

            //����removeAll
            Assert.Greater(q2.getSize(), 0);
            while (q2.getSize() > 0)
            {
                q2.removeAll(q2.getFirst());
            }
            Assert.AreEqual(q2.getSize(), 0);
        }

        [Test]
        public void testPriorityQueue()
        {
            Console.WriteLine("PriorityQueue");
            q1 = new PriorityQueue<People>(peopleComparator);
            List<People> list = add_remove_removeFirst_test();
            q2 = new PriorityQueue<People>(q1);
            constructorTest(list);

            //����removeAll
            Assert.Greater(q2.getSize(), 0);
            while (q2.getSize() > 0)
            {
                q2.removeAll(q2.getFirst());
            }
            Assert.AreEqual(q2.getSize(), 0);
        }

        protected List<People> add_remove_removeFirst_test()
        {
            Console.WriteLine("count:" + count.ToString() + "\trange:" + range.ToString());
            Console.WriteLine("add_remove_removeFirst_test:");
            Random r = new Random();
            List<People> all = new List<People>();
            List<People> elementInQueue = new List<People>();
            List<People> elementInQueue_shadow;
            People p;
            for (int i = 0; i < count; i++)
            {
                p = new People("People" + i.ToString(), r.Next(range));
                all.Add(p);
                q1.add(p);
            }
            elementInQueue_shadow = new List<People>(all);
            for (int i = 0; i < count / 2; i++)
            {
                p = all[r.Next(all.Count)];
                q1.remove(p);
                elementInQueue_shadow.Remove(p);
            }
            while (q1.getSize() > 0)
            {
                elementInQueue.Add(q1.removeFirst());
            }
            elementInQueue_shadow.Sort(peopleComparator);
            Console.WriteLine("elementInQueue.Count:" + elementInQueue.Count);
            Console.WriteLine("elementInQueue_shadow.Count:" + elementInQueue_shadow.Count);
            //�����������е�Ԫ�ظ����Ƿ����
            Assert.AreEqual(elementInQueue.Count, elementInQueue_shadow.Count);
            for (int i = 1; i < elementInQueue.Count; i++)
            {
                //���Զ�����õ��������Ƿ�����
                Assert.LessOrEqual(elementInQueue[i - 1].getAge(), elementInQueue[i].getAge());
            }
            for (int i = 0; i < elementInQueue.Count; i++)
            {
                //���Զ������List.Sort֮��õ����������Ӧλ�õ�Ԫ��ֵ�Ƿ���ȡ�
                Assert.AreEqual(peopleComparator.Compare(elementInQueue[i], elementInQueue_shadow[i]), 0);
                q1.add(elementInQueue[i]);
            }
            return elementInQueue;
        }

        protected void constructorTest(List<People> list)
        {
            Console.WriteLine("constructorTest:");
            //���������ѵ�Ԫ�ظ����Ƿ���ͬ
            int len1 = q1.getSize(), len2 = q2.getSize();
            Console.WriteLine("q1.getSize():" + len1.ToString() + "\tq2.getSize():" + len2.ToString());
            Assert.AreEqual(len1, len2);
            object[] a1 = q1.getQueue(), a2 = q2.getQueue();
            for (int i = 0; i < len1; i++)
                //���������ѵ������Ƿ�һ��
                Assert.AreEqual(a1[i], a2[i]);
        }

        [Test]
        public void value_class_test()
        {
            //����ֵ��ȵĻ������Ͷ����Hash Code���
            double i = 0, j = 1, k = 2, l = 1, m = 2, n = 3;
            Console.WriteLine(i.GetHashCode());
            Console.WriteLine(j.GetHashCode());
            Console.WriteLine(k.GetHashCode());
            Console.WriteLine(l.GetHashCode());
            Console.WriteLine(m.GetHashCode());
            Console.WriteLine(n.GetHashCode());
            Assert.AreEqual(j.GetHashCode(), l.GetHashCode());
            Assert.AreEqual(k.GetHashCode(), m.GetHashCode());
        }
    }
}
