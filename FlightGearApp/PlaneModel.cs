using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Xml;
using OxyPlot;
using OxyPlot.Series;

namespace FlightGearApp { 
	interface IPlaneModelShow : INotifyPropertyChanged
{
	void connect();
	void disconnect();
	void start();
	TimeSpan Time { set; get; }
	int Point_Slider { set; get; }
	void ChangeSpeed(double speed);
	void TimeSlider(int seconds);
	void MouseOnSlider();
	void Pause();
	void Stop();
	int VideoDuration();
	void readFile();
	//List<string> readFile();
	XmlNodeList InitializeProperties();
	PlotModel Plot6 { set; get; }
	PlotModel Plot7 { set; get; }
	PlotModel Plot8 { set; get; }
	string BestCollerate { set; get; }
	XmlNodeList Properties { set; get; }
	double Aileron { set; get; }
	double Elevator { set; get; }
	double Throttle1 { set; get; }
	double Throttle2 { set; get; }
	double Rudder { set; get; }
	double Altitude { set; get; }
	string AltitudeText { set; get; }
	double Airspeed { set; get; }
	string AirspeedText { set; get; }
	double Heading { set; get; }
	double Roll { set; get; }
	double Yaw { set; get; }
	double Pitch { set; get; }
}

	public class PlaneModel : IPlaneModelShow
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private int sleep_time;
		private int seconds;
		volatile int file_line;
		private Socket soc;
		volatile Boolean stop;
		volatile Boolean pause_pressed;
		volatile Boolean stop_pressed;
		volatile Boolean play_already_pressed;
		volatile Boolean slider_pressed;
		private List<string> file_content = new List<string>();
		private int position;
		private LineSeries line6;
		private LineSeries line7;
		private LineSeries line8;
		volatile int property_index;
		volatile int corresponding_property_index;
		volatile Boolean property_changed;
		private double[,] correlations;
		private int num_of_rows;
		private int num_of_properties;
		List<string> properties_list = new List<string>();
		List<double[]> properties_values = new List<double[]>();

		//send notification to the view throgh the viewmodel that make the slider move according to the time
		private int point_slider;
		public int Point_Slider
		{
			get { return point_slider; }
			set
			{
				point_slider = value;
				NotifyPropertyChanged("Point_Slider");
			}
		}
		//send notification to the view throgh the viewmodel that make the time movie do forward or backward
		private TimeSpan time;
		public TimeSpan Time
		{
			get { return time; }
			set
			{
				time = value;
				NotifyPropertyChanged("Time");
			}
		}
		//send notification to the view throgh the viewmodel about the aileron property
		private double aileron;
		public double Aileron
		{
			get { return aileron; }
			set
			{
				aileron = value;
				NotifyPropertyChanged("Aileron");
			}
		}
		//send notification to the view throgh the viewmodel about the elevator property
		private double elevator;
		public double Elevator
		{
			get { return elevator; }
			set
			{
				elevator = value;
				NotifyPropertyChanged("Elevator");
			}
		}
		//send notification to the view throgh the viewmodel  about the first throttle property
		private double throttle1;
		public double Throttle1
		{
			get { return throttle1; }
			set
			{
				throttle1 = value;
				NotifyPropertyChanged("Throttle1");
			}
		}
		//send notification to the view throgh the viewmodel about the second throttle property
		private double throttle2;
		public double Throttle2
		{
			get { return throttle2; }
			set
			{
				throttle2 = value;
				NotifyPropertyChanged("Throttle2");
			}
		}
		//send notification to the view throgh the viewmodel about the rudder property
		private double rudder;
		public double Rudder
		{
			get { return rudder; }
			set
			{
				rudder = value;
				NotifyPropertyChanged("Rudder");
			}
		}
		//send notification to the view throgh the viewmodel about the altitude property
		private double altitude;
		public double Altitude
		{
			get { return altitude; }
			set
			{
				altitude = value;
				NotifyPropertyChanged("Altitude");
			}
		}
		//send notification to the view throgh the viewmodel about the rudder property in words
		private string altitude_text;
		public string AltitudeText
		{
			get { return altitude_text; }
			set
			{
				altitude_text = value;
				NotifyPropertyChanged("AltitudeText");
			}
		}
		//send notification to the view throgh the viewmodel about the airspeed property 
		private double airspeed;
		public double Airspeed
		{
			get { return airspeed; }
			set
			{
				airspeed = value;
				NotifyPropertyChanged("Airspeed");
			}
		}
		//send notification to the view throgh the viewmodel about the airspeed property in words
		private string airspeed_text;
		public string AirspeedText
		{
			get { return airspeed_text; }
			set
			{
				airspeed_text = value;
				NotifyPropertyChanged("AirspeedText");
			}
		}
		//send notification to the view throgh the viewmodel about the heading property
		private double heading;
		public double Heading
		{
			get { return heading; }
			set
			{
				heading = value;
				NotifyPropertyChanged("Heading");
			}
		}
		//send notification to the view throgh the viewmodel about the roll property
		private double roll;
		public double Roll
		{
			get { return roll; }
			set
			{
				roll = value;
				NotifyPropertyChanged("Roll");
			}
		}
		//send notification to the view throgh the viewmodel about the pitch property
		private double pitch;
		public double Pitch
		{
			get { return pitch; }
			set
			{
				pitch = value;
				NotifyPropertyChanged("Pitch");
			}
		}
		//send notification to the view throgh the viewmodel about the yaw property
		private double yaw;
		public double Yaw
		{
			get { return yaw; }
			set
			{
				yaw = value;
				NotifyPropertyChanged("Yaw");
			}
		}


		public PlaneModel()
		{
			sleep_time = 100;
			file_line = 1;
			stop = false;
			soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			pause_pressed = false;
			stop_pressed = false;
			play_already_pressed = false;
			slider_pressed = false;
			property_changed = false;
			property_index = 0;
			corresponding_property_index = 0;
			line6 = new OxyPlot.Series.LineSeries();
			line7 = new OxyPlot.Series.LineSeries();
			line8 = new OxyPlot.Series.LineSeries();
			seconds = 0;


		}
		//connect to the flightgear
		public void connect()
		{
			System.Net.IPAddress ipAdd = System.Net.IPAddress.Parse("127.0.0.1");
			System.Net.IPEndPoint remoteEP = new IPEndPoint(ipAdd, 5400);
			soc.Connect(remoteEP);
			Console.WriteLine("Socket connected to -> {0} ",
								  soc.RemoteEndPoint.ToString());
		}
		//disconnnect from the flight gear
		public void disconnect()
		{
			stop = true;
			soc.Shutdown(SocketShutdown.Both);
			soc.Close();
		}


		//when the player press the play button
		public void start()
		{
			pause_pressed = false; ;
			stop_pressed = false;
			if (!play_already_pressed)
			{
				play_already_pressed = true;
				new Thread(delegate ()
				{
					while (!stop)
					{
						for (position = 0; position < file_content.Count; position++)
						{
							if ((pause_pressed == true) || (stop_pressed == true))
							{
								while ((pause_pressed) || (stop_pressed)) { }
							}
							string line = file_content.ElementAt(position);
							string[] values = line.Split(',');
							byte[] byData = System.Text.Encoding.ASCII.GetBytes(line + "\n");
							soc.Send(byData);
							if (!slider_pressed)
							{
								Throttle1 = Double.Parse(values[6]);
								Throttle2 = Double.Parse(values[7]);
								double temp_height = Double.Parse(values[16]);
								if (temp_height < 0)
								{
									AltitudeText = "0";
									Altitude = 0;
								}
								else
								{
									Altitude = temp_height;
									AltitudeText = values[16];
								}
								Airspeed = Double.Parse(values[21]);
								AirspeedText = values[21];
								Heading = Double.Parse(values[19]);
								Roll = Double.Parse(values[17]);
								Pitch = Double.Parse(values[18]);
								Yaw = Double.Parse(values[7]);
								Rudder = Double.Parse(values[2]);
								Aileron = Double.Parse(values[0]);
								Elevator = Double.Parse(values[1]);
								if ((position / 10) % 1 == 0)
								{
									if (property_changed == true)
									{
										line6.Points.Clear();
										property_changed = false;
									}
									seconds = (position / 10);
									int minutes = (seconds / 60);
									int hours = minutes / 60;
									Time = new TimeSpan(hours, minutes % 60, seconds % 60);
									Point_Slider = seconds;
									UpdateLine6(seconds, values);
									UpdateLine7(seconds, values);
									UpdateLine8();
								}
							}
							System.Threading.Thread.Sleep(sleep_time);
						}
					}
				}).Start();
			}
		}

		//return to the view thrpugh the viewmodel the duration of the video so theslider will know his maximum value
		public int VideoDuration()
		{
			readFile();
			num_of_rows = file_content.Count();
			num_of_properties = file_content.ElementAt(0).Split(',').Length;
			correlations = CalculatePearson();
			int count = file_content.Count;
			int time = count / 10;
			return time;
		}
		//whenever the player remove the slider, we need to change the flight of the plane
		public void TimeSlider(int seconds)
		{
			slider_pressed = false;
			pause_pressed = false;
			position = seconds * 10;
			int minutes = (seconds / 60);
			int hours = minutes / 60;
			Time = new TimeSpan(hours, minutes % 60, seconds % 60);
			slider_pressed = false;
		}

		public void MouseOnSlider()
		{
			pause_pressed = true;
			slider_pressed = true;
		}
		public void Pause()
		{
			pause_pressed = true;
		}
		public void Stop()
		{
			Time = new TimeSpan(00, 00, 00);
			stop_pressed = true;
			//customEventStop(stop_pressed);
			position = 0;
			line6 = new OxyPlot.Series.LineSeries();
			line7 = new OxyPlot.Series.LineSeries();
			line8 = new OxyPlot.Series.LineSeries();
			plot6 = new OxyPlot.PlotModel();
			plot7 = new OxyPlot.PlotModel();
			plot8 = new OxyPlot.PlotModel();
		}
		//change the speed of the flight, from the view throguh the viewmodel
		public void ChangeSpeed(double speed)
		{
			switch (speed)
			{
				case 1:
					sleep_time = 100;
					break;
				case 1.5:
					sleep_time = 75;
					break;
				case 2:
					sleep_time = 50;
					break;
				case 0.5:
					sleep_time = 200;
					break;
			}
		}

		public void NotifyPropertyChanged(string propName)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		/// <summary>
		/// read the scv once i order to get all the data about the flight
		/// </summary>
		public void readFile()
		{
			StreamReader reader = new StreamReader("reg_flight.csv");
			while (!reader.EndOfStream)
			{
				var line = reader.ReadLine();
				file_content.Add(line);
			}
		}
		//change the porperty that correlate best because the change in the pick of the player
		public void PropertyIndex(int index)
		{
			property_changed = true;
			property_index = index;
			corresponding_property_index = CheckCorrespondingProperty();
		}

		//read the properties from the xml file in prder to let the user choose from them
		public XmlNodeList InitializeProperties()
		{
			XmlDocument xml = new XmlDocument();
			xml.Load("playback_small.xml");
			XmlNodeList xnList = xml.SelectNodes("/PropertyList/generic/output/chunk/name");
			foreach (XmlNode xn in xnList)
			{
				properties_list.Add(xn.InnerText);
			}
			Properties = xnList;
			return xnList;
		}
		private XmlNodeList properties;
		public XmlNodeList Properties
		{
			get { return properties; }
			set
			{
				properties = value;
				NotifyPropertyChanged("Properties");

			}
		}

		//responsible for drawing the plot on the selected roperty
		private PlotModel plot6 = new OxyPlot.PlotModel
		{
			Title = "Selected Property",
			TitleFontSize = 8
		};
		public PlotModel Plot6
		{
			get { return plot6; }
			set
			{
				plot6 = value;
				NotifyPropertyChanged("Plot6");
			}
		}
		//responsible for drawing the plot on the best collerate property
		private PlotModel plot7 = new OxyPlot.PlotModel
		{
			Title = "        Best Corellation Property",
			TitleFontSize = 8
		};
		public PlotModel Plot7
		{
			get { return plot7; }
			set
			{
				plot7 = value;
				NotifyPropertyChanged("Plot7");
			}
		}
		//responsible for drawing the plot of the linear regression
		private PlotModel plot8 = new OxyPlot.PlotModel { };
		public PlotModel Plot8
		{
			get { return plot8; }
			set
			{
				plot8 = value;
				NotifyPropertyChanged("Plot8");
			}
		}
		//calculate the pearson
		public double[,] CalculatePearson()
		{
			double[,] values = GetVluesFromFile(num_of_rows, num_of_properties);
			properties_values = GetListByProperties(num_of_rows,
				num_of_properties, values);
			//calculate pearson between each
			double[,] correlations = new double[num_of_properties, num_of_properties];
			for (int k = 0; k < num_of_properties; k++)
			{
				for (int j = 0; j < num_of_properties; j++)
				{
					if (k == j)
					{
						correlations[k, j] = -1;
					}
					else
					{
						double avg1 = properties_values[k].Average();
						double avg2 = properties_values[j].Average();

						double sum1 = properties_values[k].Zip(properties_values[j],
							(x1, y1) => (x1 - avg1) * (y1 - avg2)).Sum();

						double sumSqr1 = properties_values[k].Sum(x => Math.Pow((x - avg1), 2.0));
						double sumSqr2 = properties_values[j].Sum(y => Math.Pow((y - avg2), 2.0));

						double result = sum1 / Math.Sqrt(sumSqr1 * sumSqr2);
						correlations[k, j] = result;
					}
				}
			}
			return correlations;
		}

		double[,] GetVluesFromFile(int num_of_rows, int num_of_properties)
		{
			double[,] values = new double[num_of_rows, num_of_properties];
			// int i = 0;
			//while (!reader.EndOfStream)
			for (int posit = 0; posit < file_content.Count; posit++)
			{
				//var line = reader.ReadLine()
				string line = file_content.ElementAt(posit);
				string[] value = line.Split(',');
				for (int j = 0; j < num_of_properties; j++)
				{
					values[posit, j] = double.Parse(value[j]);
				}
				//    i++;
			}
			return values;
		}

		List<double[]> GetListByProperties(int num_of_rows, int num_of_properties, double[,] values)
		{
			List<double[]> properties_values = new List<double[]>();
			for (int k = 0; k < num_of_properties; k++)
			{
				double[] arr = new double[num_of_rows];
				for (int j = 0; j < num_of_rows; j++)
				{
					arr[j] = values[j, k];
				}
				properties_values.Add(arr);
			}
			//RestartReader();
			return properties_values;
		}
		void UpdateLine6(double time, string[] value)
		{
			line6.Points.Add(new DataPoint(time, Double.Parse(value[property_index])));
			plot6.Series.Remove(line6);
			plot6.Series.Add(line6);
			plot6.InvalidatePlot(true);
			Plot6 = plot6;
		}
		void UpdateLine7(double time, string[] value)
		{
			line7.Points.Add(new DataPoint(time, Double.Parse(value[corresponding_property_index])));
			plot7.Series.Remove(line7);
			plot7.Series.Add(line7);
			plot7.InvalidatePlot(true);
			Plot7 = plot7;
			BestCollerate = properties_list[corresponding_property_index];
		}

		//decide about the best property according to the current one
		int CheckCorrespondingProperty()
		{
			double max = -1;
			int index = 0;
			for (int i = 0; i < num_of_properties; i++)
			{
				double temp = correlations[property_index, i];
				if (temp > max)
				{
					max = temp;
					index = i;
				}
			}
			return index;
		}
		private string best_collerate;
		public string BestCollerate
		{
			get { return best_collerate; }
			set
			{
				best_collerate = value;
				NotifyPropertyChanged("BestCollerate");
			}
		}

		public void UpdateLine8()
		{
			(double min, double max) = GetMinMaxPropertyIndex();
			plot8 = new OxyPlot.PlotModel { };
			LineSeries[] lines = new LineSeries[num_of_rows];
			//draw the points
			double min_time = (seconds * 10) - 300;
			if (min_time < 0)
			{
				min_time = 0;
			}
			double max_time = seconds * 10;
			for (int i = 0; i < num_of_rows; i = i + 10)
			{
				if ((min_time <= i) && (i <= max_time))
				{
					lines[i] = new OxyPlot.Series.LineSeries()
					{
						//Color = OxyColors.Red,
						MarkerFill = OxyColors.Red,
						//MarkerStroke = OxyColors.Red,
						MarkerType = MarkerType.Circle,
						StrokeThickness = 0,
						MarkerSize = 2,
					};
				}
				else
				{
					lines[i] = new OxyPlot.Series.LineSeries()
					{
						//Color = OxyColors.Red,
						MarkerFill = OxyColors.Blue,
						//MarkerStroke = OxyColors.Red,
						MarkerType = MarkerType.Circle,
						StrokeThickness = 0,
						MarkerSize = 2,
					};
				}
				lines[i].Points.Add(new DataPoint(properties_values[property_index][i],
				   properties_values[corresponding_property_index][i]));
				plot8.Series.Add(lines[i]);
			}
			//draw the reggresion line

			LineSeries line8 = new OxyPlot.Series.LineSeries();
			(double m, double n) = LinearRegression();
			double x1 = min;
			double x2 = max;
			double y1 = (m * x1) + n;
			double y2 = (m * x2) + n;
			line8.Points.Add(new DataPoint(x1, y1));
			line8.Points.Add(new DataPoint(x2, y2));
			plot8.Series.Add(line8);
			plot8.InvalidatePlot(true);
			Plot8 = plot8;
		}
		//calculate the linear regression
		public (double, double) LinearRegression()
		{
			double sumOfX = 0;
			double sumOfY = 0;
			double sumOfXSq = 0;
			double sumOfYSq = 0;
			double sumCodeviates = 0;

			for (var i = 0; i < properties_values[property_index].Length; i++)
			{
				var x = properties_values[property_index][i];
				var y = properties_values[corresponding_property_index][i];
				sumCodeviates += x * y;
				sumOfX += x;
				sumOfY += y;
				sumOfXSq += x * x;
				sumOfYSq += y * y;
			}

			var count = properties_values[property_index].Length;
			var ssX = sumOfXSq - ((sumOfX * sumOfX) / count);

			var sCo = sumCodeviates - ((sumOfX * sumOfY) / count);

			var meanX = sumOfX / count;
			var meanY = sumOfY / count;
			double n = meanY - ((sCo / ssX) * meanX);
			double m = sCo / ssX;
			return (m, n);
		}

		public (double, double) GetMinMaxPropertyIndex()
		{
			double min = properties_values[property_index][0];
			double max = properties_values[property_index][0];
			for (int i = 1; i < num_of_rows; i++)
			{
				if (properties_values[property_index][i] < min)
				{
					min = properties_values[property_index][i];
				}
				if (properties_values[property_index][i] > max)
				{
					max = properties_values[property_index][i];
				}
			}
			return (min, max);
		}
	}
}


