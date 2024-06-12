# CBLEmbedded

## Description

A robot that simulates an unloading baggage-handling system in airports. In particular, it is intended to load a vehicle with luggage from an aircraft. Our baggage cart has three compartments, which will be filled by keeping their weights as equal as possible. The robot works on the principle that a new luggage is put in the most light-weighted compartment. There are two types of baggage – 10kg and 20kg. Luggage that was not intended for this flight will be placed in a separate container and moved to the lost and found. The capacity of each compartment is 40kg. When the current baggage cannot be stored in any of the compartments without exceeding the weight limit, the vehicle is ready to go. 

## How to use the project

- In the *Orchestrator* folder there are:
  - the Modbus Connector which creates another layer of abstraction over the Serial protocol used by both ends - ESP-32 and RaspberryPI;
  - the Driver which implements the whole logic of the robot using commands in the service layer;
  - the Portal which collects and preserves the measurements in csv files for later usage in our data model.
  - the Robot SeviceLayer which creates application specific implementation over Modbus;
  - the Simulation transfer server
- In the *Peripherals* folder there is implementation of functionality of the hardware devices, used in the robot:
  -  the Color sensor determines the behavior of the color sensor. Receives data and converts it in the needed form for the project;
  -  the Modbus connector handles Modbus communication over the serial connection. It includes methods for reading, decoding, processing, and responding to Modbus commands;
  -  the Depth sensor determines the behavior of the depth sensor. Receives data and converts it in the needed form for the project;
  -  the Motor driver controls a motor using four input pins. It provides methods to initialize the motor, move it by steps or length, check its status, and handle continuous movement;
  -  the Display determines the behavior of the display. Writes different type of messages depending on the state of the robot.
  -  the Servo controller manages multiple servos using the PCA9685 PWM driver. It provides methods to initialize the controller, add and configure servos, set angles immediately, and control servo progression;
  -  the Error prone device defines the basic functionality of the devices. Makes sure that the peripheral devices work properly;
  -  the Main manages various devices connected to the ESP-32.
- In the *Printing* folder there are 3D files of different holders for the hardware devices
- In the *Simulation* folder there is an exe file which contains the built simulation of the robot and all work files for the simulation created in Unity.
