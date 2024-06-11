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
    private readonly List<int> weights = new() { 30, 30, 30 };
    

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await WriteToDisplay(DisplayMessageTypeEnum.STATUSS_OK, "All systems are up and r!");
        await WriteToDisplay(DisplayMessageTypeEnum.MESSAGE, "Starting up!");
        // Play initialization sound
        while (!stoppingToken.IsCancellationRequested)
        {
            await OpenBarrierAsync(false);
            await robotService.MoveBelt(new MoveBeltContinuousMessage() { Running = true });
            await WriteToDisplay(DisplayMessageTypeEnum.MESSAGE, "Awaiting object!");
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
        await WriteToDisplay(DisplayMessageTypeEnum.MESSAGE, "Letting object pass!");
        await OpenBarrierAsync();
        await WriteToDisplay(DisplayMessageTypeEnum.MESSAGE, "Traversing to color sensor!");
        await MoveBeltAsync(_options.BarrierPassingDistance);
        await WaitMotorStopAsync();
        await OpenBarrierAsync(false);
        await MoveBeltAsync(_options.BarrierColorSensorDistance - _options.BarrierPassingDistance);
        await WaitMotorStopAsync();

        await WriteToDisplay(DisplayMessageTypeEnum.MESSAGE, "Classifying object!");
        await Task.Delay(_options.ColorSensor.WaitTimeMs);
        string objectName = await ClassifyObjectWithColorSensorAsync(); //get the color
        await HandleKnownObject(objectName);
    }

    private async Task HandleKnownObject(string objectName)
    {
        int minimalWeight = -1;
        int weight = 0;
        if (objectName == "white_disc")
        {
            weight = 10;
            minimalWeight = GetMinimalWeight(weights[0], weights[1], weights[2], 10);
        } else if (objectName == "black_disc")
        {
            weight = 20;
            minimalWeight = GetMinimalWeight(weights[0], weights[1], weights[2], 20);
        } 
        else if (objectName == "none")
        {
            await ThrowToTrash();
            return;
        } 
        else if (objectName == "empty")
        {
            await WriteToDisplay(DisplayMessageTypeEnum.MESSAGE, "Object possibly taken from belt!");
            return;
        }
        
        if (minimalWeight > 0)
        {
            await WriteToDisplay(DisplayMessageTypeEnum.MESSAGE, "Moving " + objectName + " to bin " + minimalWeight); 
            await MoveBeltAsync(_options.ColorSensorPusherDistance + (minimalWeight - 1) * _options.InterPusherDistance);
            await PushAsync(minimalWeight - 1);
            weights[minimalWeight - 1] += weight;
        }
        else
        {
            await WriteToDisplay(DisplayMessageTypeEnum.MESSAGE, "Bins full, please empty and press Enter!");
            Console.ReadLine(); // After this point we expect the bins to be empty.
            for (int i = 0; i < weights.Count; i++)
            {
                weights[i] = 0;
            }

            await 
                HandleKnownObject(objectName);
        }
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
                text = text.Insert(0, "s0");
                break;
            case DisplayMessageTypeEnum.STATUSS_WARNING:
                text = text.Insert(0, "s1");
                break;
            case DisplayMessageTypeEnum.STATUSS_ERROR:
                text = text.Insert(0, "s9");
                break;
            case DisplayMessageTypeEnum.MESSAGE:
                text = text.Insert(0, "me");
                break;
            case DisplayMessageTypeEnum.CURRENT_OP:
                text = text.Insert(0, "op");
                break;
        }

        var response = await robotService.WriteToDisplay(new WriteToDisplayMessage(text));
    }

    private async Task ThrowToTrash()
    {
        await WriteToDisplay(DisplayMessageTypeEnum.CURRENT_OP, "Moving foreign object to trash!");
        await MoveBeltAsync(_options.ColorSensorToTrashDistance);
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