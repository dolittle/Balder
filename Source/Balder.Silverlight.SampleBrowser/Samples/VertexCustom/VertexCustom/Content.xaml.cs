using System;
using System.Windows.Threading;
using Balder.Tools;
using System.Windows.Controls;
using System.Windows;
using Balder.Materials;
using Balder.Math;
using Balder.Objects.Geometries;
using System.Collections.Generic;
using Balder.View;

namespace Balder.Silverlight.SampleBrowser.Samples.VertexCustom.VertexCustom
{
    public partial class Content
    {
        VertexCustom vtxCuston;
        TextBox txtVX1;
        TextBox txtVY1;
        TextBox txtVZ1;
        TextBox txtVX2;
        TextBox txtVY2;
        TextBox txtVZ2;

        public Content()
        {
            InitializeComponent();

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
           
            vtxCuston = new VertexCustom { Position = new Coordinate(0, 0, 0), Rotation = new Coordinate(-45, -45, 0), InteractionEnabled = true, Color = Colors.Red};
            Game.Scene.AddNode(vtxCuston);
            vtxCuston.GenerateVertices(126, 2, new Vertex(-5, 3, -2), new Vertex(2, 5, 4));
            vtxCuston.GenerateLines(63, 1);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            vtxCuston.Rotation.Y += 0.5f;
        }

        private static readonly Random rnd = new Random();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            lstBox.Items.Clear();
            lstBox.Items.Add(string.Format("Vertex: {0}", vtxCuston.FullDetailLevel.VertexCount.ToString()));
            lstBox.Items.Add(string.Format("Lines: {0}", vtxCuston.FullDetailLevel.LineCount.ToString()));
            lstBox.Items.Add(string.Format("Faces: {0}", vtxCuston.FullDetailLevel.FaceCount.ToString()));
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vtxCuston.GenerateVertices(126, 2, new Vertex(Convert.ToInt32(txtVX1.Text), Convert.ToInt32(txtVY1.Text), Convert.ToInt32(txtVZ1.Text)), new Vertex(Convert.ToInt32(txtVX2.Text), Convert.ToInt32(txtVY2.Text), Convert.ToInt32(txtVZ2.Text)));
                vtxCuston.GenerateLines(63, 1);
            }
            catch (Exception ex)
            {
                txtException.Text = ex.Message;
            }
            
        }

        private void spValueV1_Loaded(object sender, RoutedEventArgs e)
        {
            StackPanel stackPanel = (sender as StackPanel);

            TextBlock infoV1X = new TextBlock();
            infoV1X.Text = "V1-X:";
            TextBlock infoV1Y = new TextBlock();
            infoV1Y.Text = "V1-Y:";
            TextBlock infoV1Z = new TextBlock();
            infoV1Z.Text = "V1-Z:";
            txtVX1 = new TextBox();
            txtVX1.Text = "-5";
            txtVY1 = new TextBox();
            txtVY1.Text = "3";
            txtVZ1 = new TextBox();
            txtVZ1.Text = "-2";

            stackPanel.Children.Add(infoV1X);
            stackPanel.Children.Add(txtVX1);
            stackPanel.Children.Add(infoV1Y);
            stackPanel.Children.Add(txtVY1);
            stackPanel.Children.Add(infoV1Z);
            stackPanel.Children.Add(txtVZ1);
        }

        private void spValueV2_Loaded(object sender, RoutedEventArgs e)
        {
            StackPanel stackPanel = (sender as StackPanel);

            TextBlock infoV2X = new TextBlock();
            infoV2X.Text = "V2-X:";
            TextBlock infoV2Y = new TextBlock();
            infoV2Y.Text = "V2-Y:";
            TextBlock infoV2Z = new TextBlock();
            infoV2Z.Text = "V2-Z:";
            txtVX2 = new TextBox();
            txtVX2.Text = "2";
            txtVY2 = new TextBox();
            txtVY2.Text = "5";
            txtVZ2 = new TextBox();
            txtVZ2.Text = "4";

            stackPanel.Children.Add(infoV2X);
            stackPanel.Children.Add(txtVX2);
            stackPanel.Children.Add(infoV2Y);
            stackPanel.Children.Add(txtVY2);
            stackPanel.Children.Add(infoV2Z);
            stackPanel.Children.Add(txtVZ2);
        }
    }
}
