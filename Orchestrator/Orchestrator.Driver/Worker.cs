using Orchestrator.Driver.Config;
using Orchestrator.Driver.Config.ColorSensor;
using ServiceLayer.Helpers;
using ServiceLayer.Services;
using ServiceLayer.Types;

namespace Orchestrator.Driver;

public class Worker(ILogger<Worker> logger, IRobotService robotService, IConfiguration configuration) : BackgroundService
{
    private readonly RobotVariablesOptions _options =
        configuration.GetSection(RobotVariablesOptions.RobotVariables).Get<RobotVariablesOptions>()!;

    private readonly ColorSensorInterpreter _colorSensorInterpreter = new();
    private readonly List<int> weights = new() { 0, 0, 0 };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Play initialization sound
        while (!stoppingToken.IsCancellationRequested)
        {
            await OpenBarrierAsync(false);
            await robotService.MoveBelt(new MoveBeltContinuousMessage() { Running = true });
            while (!await ObjectAtBarrierAsync())
            {
                await Task.Delay(100, stoppingToken);
            }

            await robotService.MoveBelt(new MoveBeltContinuousMessage() { Running = false });
            await robotService.MoveBelt(new MoveBeltMessage()
                { Distance = _options.BarrierAdjustmentDistance }); //by 20mm
            await WaitMotorStopAsync();
            await HandleObject();
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await robotService.MoveBelt(new MoveBeltContinuousMessage() { Running = false });
        await OpenBarrierAsync();
    }

    // private static int counter = 0;

    private async Task HandleObject()
    {
        await OpenBarrierAsync();
        await MoveBeltAsync(_options.BarrierPassingDistance);
        await WaitMotorStopAsync();
        await OpenBarrierAsync(false);
        await MoveBeltAsync(_options.BarrierColorSensorDistance - _options.BarrierPassingDistance);
        await WaitMotorStopAsync();

        await Task.Delay(_options.ColorSensor.WaitTimeMs);
        string objectName = await ClassifyObjectWithColorSensorAsync(); //get the color
        
        int minimalWeight = -1;
        if (objectName == "white_disc")
        {
            minimalWeight = GetMinimalWeight(weights[0], weights[1], weights[2], 10);
        } else if (objectName == "black_disc")
        {
            minimalWeight = GetMinimalWeight(weights[0], weights[1], weights[2], 20);
        } else if (objectName == "empty")
        {
            
        }
        
        await robotService.WriteToDisplay(new WriteToDisplayMessage("op" + objectName));

        await MoveBeltAsync(_options.ColorSensorPusherDistance);
        
        

        // else
        // {
        //     RobotService.MoveBelt();//by 170mm
        // }
        //
        // //Check for full containers
        // if (weight1 == 40 && weight2 == 40 && weight3 == 40)
        // {
        //     RobotService.WriteToDisplay();//Write "Containers full. Empty!"
        //     stopProcess = true;
        // }
    }
    //
    int GetMinimalWeight(int weight1, int weight2, int weight3, int discWeight)
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

    enum DisplayMessageTypeEnum
    {
        STATUSS_OK,
        STATUSS_WARNING,
        STATUSS_ERROR,
        MESSAGE,
        CURRENT_OP
    }

    private async Task WriteToDisplay(DisplayMessageTypeEnum messageType, string text)
    {
        switch (messageType)
        {
            case DisplayMessageTypeEnum.STATUSS_OK:
                text.Insert(0, "s0");
                break;
            case DisplayMessageTypeEnum.STATUSS_WARNING:
                text.Insert(0, "s1");
                break;
            case DisplayMessageTypeEnum.STATUSS_ERROR:
                text.Insert(0, "s9");
                break;
            case DisplayMessageTypeEnum.MESSAGE:
                break;
            case DisplayMessageTypeEnum.CURRENT_OP:
                text.Insert(0, "op");
                break;
        }

        var response = await robotService.WriteToDisplay(new WriteToDisplayMessage(text));
    }
    
    private async Task<bool> ObjectAtBarrierAsync()
    {
        var depthSensorResponse = await robotService.ReadDepthSensorMessage();
        if (depthSensorResponse.Success && depthSensorResponse.Data is not null)
        {
            int depthSensorRange = depthSensorResponse.Data.Range;
            if (depthSensorRange <= _options.DepthSensor.ObjectUpperBound)
            {
                return true;
            }

            return false;
        }

        return false;
    }

    private async Task OpenBarrierAsync(bool open = true)
    {
        await robotService
            .SetServoProgressions(
                new SetServoProgressionsMessage()
                {
                    Progressions = new List<ServoProgressionDto>()
                    {
                        new ServoProgressionDto()
                            { ServoId = 0, Progression = open ? (byte)0 : (byte)255 }
                    }
                });
        await Task.Delay(_options.InterOperationDelayMs);
    }

    private async Task PushAsync(int pusherNumber)
    {
        await robotService
            .SetServoProgressions(
                new SetServoProgressionsMessage()
                {
                    Progressions = new List<ServoProgressionDto>()
                    {
                        new ServoProgressionDto()
                            { ServoId = (byte)(pusherNumber + 1), Progression = 255 }
                    }
                });
        await Task.Delay(_options.PusherMoveDelayMs);
        await robotService
            .SetServoProgressions(
                new SetServoProgressionsMessage()
                {
                    Progressions = new List<ServoProgressionDto>()
                    {
                        new ServoProgressionDto()
                            { ServoId = (byte)(pusherNumber + 1), Progression = 0 }
                    }
                });
        await Task.Delay(_options.PusherMoveDelayMs);
    }

    private async Task MoveBeltAsync(int distance)
    {
        await robotService.MoveBelt(new MoveBeltMessage() { Distance = distance });
        await WaitMotorStopAsync();
    }

    private async Task<bool> MotorStillMovingAsync()
    {
        var motorStillMovingResponse = await robotService.IsMotorMoving();
        if (motorStillMovingResponse.Success && motorStillMovingResponse.Data is not null)
            return motorStillMovingResponse.Data.isMoving;
        return true;
    }

    private async Task WaitMotorStopAsync()
    {
        while ((await MotorStillMovingAsync()))
        {
            await Task.Delay(100);
        }
    }

    private bool InDeviation(ReadColorSensorMessage message, IdentifiableObject identifiableObject)
    {
        return Math.Abs(message.Red - identifiableObject.Red) <= identifiableObject.Deviation &&
               Math.Abs(message.Green - identifiableObject.Green) <= identifiableObject.Deviation &&
               Math.Abs(message.Blue - identifiableObject.Blue) <= identifiableObject.Deviation;
    }

    private async Task<string> ClassifyObjectWithColorSensorAsync()
    {
        ReadColorSensorMessage colorSensorMessage = (await robotService.ReadColorSensorData()).Data!;
        logger.LogInformation(colorSensorMessage.Lux.ToString());
        var objectName = _colorSensorInterpreter.ColorFind(new Point(colorSensorMessage.Red, colorSensorMessage.Green,
            colorSensorMessage.Blue));
        return objectName;
    }
}