using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.BassWasapi;
using ArduinoAudioPlayer;
using System.Configuration;

using Color = System.Windows.Media.Color;

namespace Upper_computer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private GUI gui = new GUI();
        private Tool tool=new Tool();
        private bool sigm=false;
        private byte[] ReDatas = new byte[1];
        private int sigcom=-1;
        private int[] fft = new int[65];
        private double[] y_fft = new double[65];



        public MainWindow()
        {
            InitializeComponent();
            tool.setup();
            gui.GUI_Init(Root);
           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var timer = new System.Windows.Threading.DispatcherTimer();
            var redata = new System.Windows.Threading.DispatcherTimer();
            for (int i = 0; i < tool.GetCOM().Length; i++)
            {
                ListCOM.Items.Add(tool.GetCOM()[i]);
            }

            timer.Tick += new EventHandler(OnTimedEvent);
            timer.Interval = TimeSpan.FromSeconds(1.0 / 500);
            timer.Start();


        }


        private void OnTimedEvent(object sender, EventArgs e)
        {
            Root.Children.Clear();
            datahand();
            Moation_Spectrum_rect(null);


            if (sigcom >= 0)
            {

                switch (sigcom)
                {
                    case 0:

                        break;
                    case 1:
                        Array.Clear(fft, 0, fft.Length);
                        tool.Write("l");
                        break;
                    case 2:
                        Array.Clear(fft, 0, fft.Length);
                        tool.Write("r");
                        break;
                    case 3:
                        tool.Write("n");
                        break;
                    case 4:
                        tool.Write("m");
                        break;
                    default:
                        break;
                }
                
                tool.Write(string.Join(" ", fft));
            }
          
        }



        private void datahand()
        {
            double value = 3;
           
            for (int i = 0; i < fft.Length; i++)
            {

                double FFTbuffer = tool.GetLong(i, 6, 128, 5000);
               

                if (y_fft[i] > FFTbuffer)
                {
                    y_fft[i] -= 1.5;
                }
                else if (y_fft[i] < FFTbuffer && y_fft[i] < 128)
                {
                    y_fft[i] += value;
                }
                else if (y_fft[i] >= 128)
                {
                    y_fft[i] = 128;
                }

                
                if (sigm == true)
                {
                    fft[i] = Convert.ToInt32(FFTbuffer);
                }

            }
        }
        private void Moation_Spectrum_rect(Brush theme)
        {
            int x = 0;
            for (int i = 0; i < fft.Length; i++)
            {
                if (y_fft[i] >= 0)
                {
                    gui.Fill_Rect(x, 256 - y_fft[i], 4, y_fft[i], new SolidColorBrush(Color.FromRgb(255, 255, 255)));
                }
                x += 5;
            }
        }
       

        private void ListCOM_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            try
            {
                
                tool.Connect(ListCOM.SelectedValue.ToString());
                sigcom = 1;
                sigm = false; ;

            }
            catch (Exception)
            {
                tool.arduinoPort.Close();
            }
        }

        private void R_Click(object sender, RoutedEventArgs e)
        {

            sigcom = 2;
            sigm = false;


            Console.WriteLine(sigcom.ToString());
            
        }

        private void L_Click(object sender, RoutedEventArgs e)
        {

            sigcom =1;
            sigm = false;

            Console.WriteLine(sigcom.ToString());
        }

        private void M_Click(object sender, RoutedEventArgs e)
        {


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {

                this.DragMove();
            }
        }

        private void List_Label1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            sigcom = 3;
            sigm = true;
        }

        private void List_Label2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            sigcom = 4;
            sigm = true;
        }
    }
}
