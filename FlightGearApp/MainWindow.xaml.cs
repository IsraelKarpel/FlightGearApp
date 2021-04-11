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
using System.Xml;

namespace FlightGearApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PlaneViewModel vm;
        XmlNodeList xnList;
        int index;
        List<string> properties = new List<string>();
        public MainWindow()
        {
            vm = new PlaneViewModel(new PlaneModel());
            DataContext = vm;
            InitializeComponent();
            GetVideoTime();
            vm.VM_VideoDuration();
            xnList = vm.VM_InitializeProperties();
            InitializeProperties();
            vm.VM_connect();
        }
        //get the maximum time of the flight
        private void GetVideoTime()
        {
            Duration.Maximum = vm.VM_VideoDuration();
        }
        private void Movie_Speed(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem comboBoxItem = (ComboBoxItem)e.AddedItems[0];
            double speed = Double.Parse(comboBoxItem.Content.ToString());
            vm.VM_ChangeSpeed(speed);
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            vm.VM_start();
        }
        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            vm.VM_Pause();
        }
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            vm.VM_Stop();
        }

        private void Duration_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            object a = ((Slider)sender).Value;
            int d = (int)double.Parse(a.ToString());
            vm.VM_SliderTime(d);
        }

        private void Duration_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            vm.VM_MouseOnSlider();
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listbox = sender as ListBox;
            index = listbox.SelectedIndex;
            string selected_properties = properties[index];
            vm.VM_PropertyIndex(index);
        }

        //get from the view the list of the propreties according to the xml file and display yhem to user to coose from
        private void InitializeProperties()
        {
            //foreach (XmlNode xn in xnList)
            for (int i = 0; i < 42; i++)
            {
                properties.Add(xnList.Item(i).InnerText);
            }
            //{
            // properties.Add(xn.InnerText);
            // }
        }
    }
}
