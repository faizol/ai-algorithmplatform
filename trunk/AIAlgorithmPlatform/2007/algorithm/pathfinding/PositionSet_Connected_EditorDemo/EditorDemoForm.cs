using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using M2M;
using NearestNeighbor;
using Configuration;
using PositionSetViewer;
using Position_Interface;
using DrawingLib;
using PositionSetEditer;
using AlgorithmDemo;
using Position_Implement;
using Position_Connected_Interface;
using Position_Connected_Implement;

namespace PositionSet_Connected_EditorDemo
{
    public partial class EditorDemoForm : Form
    {
        delegate void dDemoProcess();

        public EditorDemoForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //��������㼯��
            RandomPositionSet_Connected_Config config = new RandomPositionSet_Connected_Config();
            new ConfiguratedByForm(config);
            IPositionSet_ConnectedEdit set = config.Produce();
            runDemo(set);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //��������㼯��
            RandomPositionSet_InFixedDistribution randomPositionSet_InFixedDistribution = new RandomPositionSet_InFixedDistribution();
            randomPositionSet_InFixedDistribution.PointNum = 10000;
            randomPositionSet_InFixedDistribution.DistributionStyle = distributionStyle.ClusterGaussianDistribution;
            new ConfiguratedByForm(randomPositionSet_InFixedDistribution);
            randomPositionSet_InFixedDistribution.Produce();
            IPositionSetEdit set = randomPositionSet_InFixedDistribution;
            runDemo(set);
        }

        protected void runDemo(IPositionSet set)
        {
            //���´��������UI�߳��е��ã��������������߳��е���
            LayersExOptDlg layers = new LayersExOptDlg();
            LayersPainterForm layersPainterForm = new LayersPainterForm(layers);
            LayersPaintedControl layersPaintedControl = layersPainterForm.LayersPaintedControl;
            LayersEditedControl layersEditedControl = new LayersEditedControl();
            layersEditedControl.Dock = DockStyle.Top;
            layersEditedControl.LayersPaintedControl = layersPaintedControl;
            layersPainterForm.Controls.Add(layersEditedControl);
            FlowControlerForm flowControlerForm = new FlowControlerForm();
            layersPainterForm.Show();
            flowControlerForm.Show(layersPainterForm);

            //�½����ⲿ�������ʾ����
            M2M_NN m2m_NN = new M2M_NN();
            M2M_CH m2m_CH = new M2M_CH();

            //��һ��Worker�߳��������㷨���̵���ʾ�����������UI�߳���������ʾ���ܽ��У�
            IAsyncResult result = new dDemoProcess(delegate
            {
                ////��������㼯��
                //RandomPositionSet_InFixedDistribution randomPositionSet_InFixedDistribution = new RandomPositionSet_InFixedDistribution();
                //randomPositionSet_InFixedDistribution.PointNum = 100000;
                //randomPositionSet_InFixedDistribution.DistributionStyle = distributionStyle.ClusterGaussianDistribution;
                //new ConfiguratedByForm(randomPositionSet_InFixedDistribution);
                //randomPositionSet_InFixedDistribution.Produce();

                //�༭�㼯
                Layer_PositionSet layer;
                if (set is IPositionSet_Connected)
                {
                    layer = new Layer_PositionSet_Connected((IPositionSet_Connected)set);
                    ((Layer_PositionSet_Connected)layer).Point.PointColor = Color.Yellow;
                    ((Layer_PositionSet_Connected)layer).Point.PointRadius = 2;
                    ((Layer_PositionSet_Connected)layer).Point.IsDrawPointBorder = true;
                }
                else
                {
                    layer = new Layer_PositionSet_Point(set);
                    ((Layer_PositionSet_Point)layer).Point.PointColor = Color.Yellow;
                    ((Layer_PositionSet_Point)layer).Point.PointRadius = 2;
                    ((Layer_PositionSet_Point)layer).Point.IsDrawPointBorder = true;
                }   
                layer.EditAble = true;
                layers.Add(layer);
                layersPainterForm.Invalidate();
                flowControlerForm.SuspendAndRecordWorkerThread();
                //layers.Remove(layer);

                ///////////////////////////////////////
                //GetRandomPositionFromPositionSetRectangle getRandomPositionFromPositionSetRectangle
                //= new GetRandomPositionFromPositionSetRectangle(randomPositionSet_InFixedDistribution);

                ////��m2m_NN�㷨������ʾ��
                ////Ϊ������¼�����¼���Ӧ���Խ����㷨��ʾ
                //AlgorithmDemo_M2M_NN algorithmDemo_M2M_NN = new AlgorithmDemo_M2M_NN(m2m_NN, layers, flowControlerForm, layersPaintedControl.Invalidate);

                ////���´��������ʾ״̬һ����
                //m2m_NN.PreProcess(randomPositionSet_InFixedDistribution);
                //m2m_NN.NearestNeighbor(layersPaintedControl.GetMouseDoubleChickedRealPosition());
                //while (true)
                //{
                //    for (int i = layers.Count - 1; i >= 0; i--)
                //    {
                //        if ((layers[i].Name != "M2MStructure") && (layers[i].Name != "PositionSetOfComparedPoint"))
                //        {
                //            layers.Remove(layers[i]);
                //        }
                //    }

                //    m2m_NN.NearestNeighbor(getRandomPositionFromPositionSetRectangle.Get());
                //}

                ////������ʾ������¼���Ӧ�İ󶨣�
                //algorithmDemo_M2M_NN.EndDemo();
                ///////////////////////////////////////

                //��m2m_CH�㷨������ʾ��
                //Ϊ������¼�����¼���Ӧ���Խ����㷨��ʾ
                //AlgorithmDemo_M2M_CH algorithmDemo_M2M_CH = new AlgorithmDemo_M2M_CH(m2m_CH, layers, flowControlerForm, layersPaintedControl.Invalidate);
                //while (true)
                //{
                //    layers.Clear();
                //    //���´��������ʾ״̬һ����
                //    m2m_CH.ConvexHull(randomPositionSet_InFixedDistribution.Produce());
                //}

                ////������ʾ������¼���Ӧ�İ󶨣�
                //algorithmDemo_M2M_CH.EndDemo();

            }).BeginInvoke(null, null);
        }
    }
}