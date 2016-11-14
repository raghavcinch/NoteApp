using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using WpfApplication1;

namespace StickyNotes
{
    public class ViewPort
    {
        public double Top;
        public double Left;
        public double Right;
        public double Bottom;
        public int Offset;
    }
    public class Window2 : Window
    {
        public Window2(ViewPort viewPort)
        {

            this.Name = "Window2";
            
            var lgb = ChangeBackgroundColor(System.Windows.Media.Colors.Beige);
            this.Background = lgb;

            this.WindowStyle = WindowStyle.ToolWindow;
            this.Height = this.Width = 200;
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            var margin = viewPort.Offset * 20;
            //if(margin + viewPort.Top + Height > (viewPort.Bottom)
            //    || (margin + viewPort.Left + Width) > viewPort.Right)
            //    margin += 20;
            this.Left = margin + viewPort.Left;
            this.Top = margin + viewPort.Top;
            this.Content = InitializeStickyNote();

            //this.Closing += new System.ComponentModel.CancelEventHandler(Window2_Closing);
            this.SizeChanged += new SizeChangedEventHandler(Window2_SizeChanged);
            this.ShowInTaskbar = false;
            rtb.Focus();
        }


        public static LinearGradientBrush ChangeBackgroundColor( System.Windows.Media.Color c)
        {
            GradientStop gs1 = new GradientStop(System.Windows.Media.Colors.White, 0);
            GradientStop gs2 = new GradientStop(c, 1);
            GradientStopCollection gsc = new GradientStopCollection();
            gsc.Add(gs1);
            gsc.Add(gs2);
            LinearGradientBrush lgb = new LinearGradientBrush();
            lgb.StartPoint = new System.Windows.Point(0, 0);
            lgb.EndPoint = new System.Windows.Point(0, 1);
            lgb.GradientStops = gsc;
            return lgb;
        }

        void Window2_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            rtb.Width = ((this.ActualWidth - 10) > 0) ? (this.ActualWidth - 10) : rtb.Width;
            rtb.Height = ((this.ActualHeight - 50) > 0) ? (this.ActualHeight - 50) : rtb.Height; 
        }

        private StackPanel InitializeStickyNote()
        {
            StackPanel sp = new StackPanel();
            WrapPanel wp = new WrapPanel();

            rtb = new RichTextBox();
            rtb.Name ="NoteBox";
            rtb.Background = System.Windows.Media.Brushes.Transparent;
            rtb.BorderThickness = new Thickness(0,1,0,0);
            rtb.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            rtb.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            rtb.Height = this.Height -50;
            rtb.Width = this.Width - 10;
            sp.Children.Add(rtb);

            return sp;
        }

      public int BackgroundColorIndex
        {
            get
            {
                return _windowBackground;
            }
            set
            {
                _windowBackground = value;
            }
        }

        public int ForegroundColorIndex
        {
            get
            {
                return _windowForeground;
            }
            set
            {
                _windowForeground = value;
            }
        }

        
        public bool TopmostWindow
        {
            get
            {
                return OnTop;
            }
            set
            {
                OnTop = value;
            }
        }

        public string RepetitionNumber
        {
            get
            {
                return repetitionNumber;
            }
            set
            {
                repetitionNumber = value;
            }
        }

        private RichTextBox rtb = null;
        private double _windowHeight = 0;
        private double _windowWidth= 0;
        private int _windowBackground = -1;
        private int _windowForeground = -1;
        private bool OnTop = false;
        private bool alarmDismissed = false;
        private string repetitionNumber = "null";
        private System.Windows.Media.Brush backGround = null;
        private System.Windows.Media.Brush foreGround = null;
    }
}