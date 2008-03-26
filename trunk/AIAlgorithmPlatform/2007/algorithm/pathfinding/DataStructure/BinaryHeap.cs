using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructure
{
    /// <summary>
    /// BinaryHeap ��ժҪ˵����-------�����(���������ʵ��)
    /// </summary>
    public class BinaryHeap : IPriorityQueue
    {
        protected ArrayList array;
        //����һ���������_length������Ŀն����
        public BinaryHeap(uint _length)
        {
            //
            // TODO: �ڴ˴���ӹ��캯���߼�
            //
            array = new ArrayList((int)_length);
            array.Capacity = (int)_length;
        }

        //���ж������
        public virtual int Count
        {
            get { return this.array.Count; }
        }

        //����Ա��������1Ϊ����������ʽ
        public virtual object Item(int _i)
        {
            if (_i >= this.array.Capacity)
                throw new Exception("My:out of index");//���ܳ���
            return this.array[_i - 1];
        }

        #region IPriorityQueue ��Ա

        //�Ƚ��ն������������һ��λ���ϣ�Ҳ����i(ע��������1),Ȼ���[i/2]λ���ϵ����Ƚϣ����С���򽫿ն����Ƶ�[i/2]λ�ã���ԭ��[i/2]λ���ϵĶ������Ƶ�[i]�ϣ�����ͽ��ն���Ϊ_obj----��˵ݹ�
        public void Enqueue(Object _obj)
        {
            // TODO: ��� BinaryHeap.Enqueue ʵ��
            if (this.array.Count == this.array.Capacity)
                throw new Exception("My:priority queue is full");//������ȶ������������׳��쳣
            this.array.Add(new object());
            int i = this.array.Count;
            while (i > 1 && Comparer.Default.Compare(this.array[i / 2 - 1], _obj) > 0)
            {
                //this.Item(i)=this.Item(i/2);
                this.array[i - 1] = this.array[i / 2 - 1];
                i /= 2;
            }
            this.array[i - 1] = _obj;
        }

        public object FindMin()
        {
            // TODO: ��� BinaryHeap.FindMin ʵ��
            if (this.array.Count == 0)
                throw new Exception("My:priority queue is empty");//��������ǿյģ����׳��쳣
            return this.array[0];
        }

        public object DequeueMin()
        {
            // TODO: ��� BinaryHeap.DequeueMin ʵ��
            object tmpObj = this.FindMin();
            int i = 1;
            while ((2 * i + 1) <= this.array.Count)
            {
                if (Comparer.Default.Compare(this.array[2 * i - 1], this.array[2 * i]) <= 0)
                {
                    this.array[i - 1] = this.array[2 * i - 1];
                    this.array[2 * i - 1] = tmpObj;
                    i = 2 * i;
                }
                else
                {
                    this.array[i - 1] = this.array[2 * i];
                    this.array[2 * i] = tmpObj;
                    i = 2 * i + 1;
                }
            }

            object delObj = this.array[i - 1];//��ʱ����Ҫɾȥ��Ԫ��

            if (i != this.array.Count)//����������Ķ��������������һ��������ʲô����Ҫ��
            {
                this.array[i - 1] = this.array[this.array.Count - 1];//���ն�
            }
            this.array.RemoveAt(this.array.Count - 1);//�����һ������ɾ��
            return delObj;
        }

        #endregion
    }
}
