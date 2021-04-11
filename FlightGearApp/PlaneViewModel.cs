using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FlightGearApp
{
	class PlaneViewModel : INotifyPropertyChanged
	{
		PlaneModel model = new PlaneModel();
		public PlaneViewModel(PlaneModel model)
		{
			this.model = model;
			model.PropertyChanged +=
				delegate (Object sender, PropertyChangedEventArgs e)
				{
					NotifyPropertyChanged("VM_" + e.PropertyName);
				};
		}
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string propName)
		{
			if (PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		public void VM_SliderTime(int slider_time)
		{
			model.TimeSlider(slider_time);
		}
		public void VM_MouseOnSlider()
		{
			model.MouseOnSlider();
		}
		public void VM_ChangeSpeed(double speed)
		{
			model.ChangeSpeed(speed);
		}

		public int VM_VideoDuration()
		{
			return model.VideoDuration();
		}

		public void VM_Pause()
		{
			model.Pause();
		}

		public void VM_Stop()
		{
			model.Stop();
		}

		public void VM_connect()
		{
			model.connect();
		}

		public void VM_start()
		{
			model.start();
		}

		public void VM_disconnect()
		{
			model.disconnect();
		}
		public TimeSpan VM_Time
		{
			get { return model.Time; }
		}

		public int VM_Point_Slider
		{
			get { return model.Point_Slider; }
		}
		public XmlNodeList VM_InitializeProperties()
		{
			return model.InitializeProperties();
		}
		public void VM_PropertyIndex(int index)
		{
			model.PropertyIndex(index);
		}

		public OxyPlot.PlotModel VM_Plot6
		{
			get { return model.Plot6; }
		}
		public PlotModel VM_Plot7
		{
			get { return model.Plot7; }
		}
		public PlotModel VM_Plot8
		{
			get { return model.Plot8; }
		}
		public string VM_BestCollerate
		{
			get { return model.BestCollerate; }
		}

		public XmlNodeList VM_Properties
		{
			get { return model.Properties; }
		}
		//sides
		public double VM_Aileron
		{
			//341 are the place of the joistick, *50 so we will see the joistyck move
			get { return (model.Aileron * 50) + 341; }
			//get { return (model.Aileron * 1000); }
		}
		//up and down
		public double VM_Elevator
		{
			get { return (model.Elevator * 50) + 213; }
			//get { return (model.Elevator * 1000); }
		}
		public double VM_Throttle1
		{
			get { return model.Throttle1; }
		}
		public double VM_Throttle2
		{
			get { return model.Throttle2; }
		}
		public double VM_Rudder
		{
			get { return model.Rudder; }
		}
		public double VM_Altitude
		{
			get { return model.Altitude; }
		}
		public string VM_AltitudeText
		{
			get { return model.AltitudeText; }
		}
		public double VM_Airspeed
		{
			get { return model.Airspeed - 120; }
		}
		public string VM_AirspeedText
		{
			get { return model.AirspeedText; }
		}
		public double VM_Heading
		{
			get { return model.Heading; }
		}
		public double VM_Roll
		{
			get { return model.Roll; }
		}
		public double VM_Yaw
		{
			get { return model.Yaw; }
		}
		public double VM_Pitch
		{
			get { return model.Pitch; }
		}
	}
}
