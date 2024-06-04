using System.Reflection;
using ServiceLayer.Services.Implementation;

namespace Orchestrator.Driver;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            while (RobotService.ReadDepthSensorMessage == "no object detected")
            {
                RobotService.MoveBelt();
            }
            RobotService.MoveBelt();//by 20mm
            discHandling();
            if (stopProcess == true)
            {
                break;
            }
        }
    }

    private const float SPEED_OF_BELT = 1;//change speed
    private bool stopProcess = false;
    
    int speedToSeconds(int distance)
    {
        return (int) (distance / SPEED_OF_BELT) * 1000;
    }
    
    void discHandling()
    {
        RobotService.SetServoPos(); //open barrier
        RobotService.MoveBelt();//by 53mm
        Task.Delay(speedToSeconds(15));
        RobotService.SetServoPos();//close barrier
        String colorCurrObj = RobotService.ReadColorSensorData();//get the color
        if (colorCurrObj == "White")
        {
            int minimalWeight = getMinimalWeight(weight1, weight2, weight3, 10);
            switch (minimalWeight)
            {
                case 0: RobotService.WriteToDisplay();//Write "Containers full. Empty!"
                    stopProcess = true;
                    break;
                case 1: RobotService.MoveBelt();//by 45mm
                    RobotService.SetServoPos();//move servo1 forward
                    RobotService.SetServoPos();//move servo1 backward
                    weight1 += 10;
                    if (weight1 == 40)
                    {
                        RobotService.SetLEDOn();//set led1 on
                    }
                    break;
                case 2: RobotService.MoveBelt();//by 120mm
                    RobotService.SetServoPos();//move servo2 forward
                    RobotService.SetServoPos();//move servo2 backward
                    weight2 += 10;
                    if (weight2 == 40)
                    {
                        RobotService.SetLEDOn();//set led2 on
                    }
                    break;
                case 3: RobotService.MoveBelt();//by 180mm
                    RobotService.SetServoPos();//move servo2 forward
                    RobotService.SetServoPos();//move servo2 backward
                    weight3 += 10;
                    if (weight3 == 40)
                    {
                        RobotService.SetLEDOn();//set led3 on
                    }
                    break;
            }
        } 
        else if (colorCurrObj == "Black")
        {
            int minimalWeight = getMinimalWeight(weight1, weight2, weight3, 10);
            switch (minimalWeight)
            {
                case 0: RobotService.WriteToDisplay();//Write "Containers full. Empty!"
                    stopProcess = true;
                    break;
                case 1: RobotService.MoveBelt();//by 30mm
                    RobotService.SetServoPos();//move servo1 forward
                    RobotService.SetServoPos();//move servo1 backward
                    weight1 += 20;
                    if (weight1 == 40)
                    {
                        RobotService.SetLEDOn();//set led1 on
                    }
                    break;
                case 2: RobotService.MoveBelt();//by 120mm
                    RobotService.SetServoPos();//move servo2 forward
                    RobotService.SetServoPos();//move servo2 backward
                    weight2 += 20;
                    if (weight2 == 40)
                    {
                        RobotService.SetLEDOn();//set led2 on
                    }
                    break;
                case 3: RobotService.MoveBelt();//by 180mm
                    RobotService.SetServoPos();//move servo2 forward
                    RobotService.SetServoPos();//move servo2 backward
                    weight3 += 20;
                    if (weight3 == 40)
                    {
                        RobotService.SetLEDOn();//set led3 on
                    }
                    break;
            }
        }
        else
        {
            RobotService.MoveBelt();//by 170mm
        }
        
        //Check for full containers
        if (weight1 == 40 && weight2 == 40 && weight3 == 40)
        {
            RobotService.WriteToDisplay();//Write "Containers full. Empty!"
            stopProcess = true;
        }
    }
    
    int getMinimalWeight(int weight1, int weight2, int weight3, int discWeight)
    {
        if (weight1 <= weight2 && weight1 <= weight3)
        {
            if (discWeight + weight1 > 40)
            {
                return 0;
            }
            return 1;
        } 
        else if (weight2 <= weight1 && weight2 <= weight3)
        {
            if (discWeight + weight2 > 40)
            {
                return 0;
            }
            return 2;
        } 
        if (discWeight + weight3 > 40)
        {
            return 0;
        }
        return 3;
    }
}