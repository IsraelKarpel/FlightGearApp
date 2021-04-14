# Flightgear simulator desktop application
## Discription
Creating a windows app for investigating flights in FlightGear Simulator (hereinafter FG). The user can use our app to tracking flight's data: 
Watch the flight at FG and analize the data at our dashboard
 
## Prerequisites
* Download FlightGear Simulator [here](http://home.flightgear.org/)
* Place the file [playback_small.xml](https://github.com/IsraelKarpel/FlightGearApp/blob/master/FlightGearApp/playback_small.xml) at `{FG installation folder}\data\protocol\`
* Run FG
* Go to Setting and add the following lines:

```--generic=socket,in,10,127.0.0.1,5400,tcp,playback_small```

```--fdm=null```
![settin image](https://github.com/IsraelKarpel/FlightGearApp/blob/master/flightgear%20setting.jpg)

* Place your flight Data file at `{project}\bin\debug\` and name it **reg_flight** (In the future you will be able to choose file from your file system)
* Run the app

## about the application
The app will send your flight data to FG.

App dashboard:
![app dashboard](https://github.com/IsraelKarpel/FlightGearApp/blob/master/flightgear%20dashboard2.jpg)

You can control and analize the flight with:
1. Play button (start sending data to FG and dashboard)
2. Pause button (pause at the current time)
3. Stop button (stop and set time back to start)
4. Choose the play speed (x0.5 x1 x1.5 x2)
5. Choose property from the list and you will see:
  I. Graph of the selected property over time
  II. Graph of the selected property with the most correlative property (Pearson correlation)
  III. graph of the linear regresion of the selected property with the most correlative property
5. The Dashboard will always show you:
  - km/h
  - Throttle1
  - Throttle2
  - Rudder
  - Altitude
  - Yaw
  - Roll
  - Pitch
  - Heading
  - Joystick status
6. Jump to any point in time with the time bar:
![time bar](https://github.com/IsraelKarpel/FlightGearApp/blob/master/flightgeat%20time%20bar.jpg)
7. {future: anomaly detection from dll selected at runtime}

## software architecture
There are 4 main files:
1. Main Window.xaml - View (visuality)- contain all the visual components and containers
2. MainWindow.xaml.css - View (functionallity) - contain all the logic and code of the visual components
3. PlaneModel - Model - data, data analtics and data manipulation of the flight's data
4. PlaneViewModel - View-Model - connect the view and the model

## basic uml
![uml](https://github.com/IsraelKarpel/FlightGearApp/blob/master/flightgear%20uml.jpg)

## Tutorial
you can watch [here](https://youtu.be/HhcgV7TdUY0) example of basic use of this app
