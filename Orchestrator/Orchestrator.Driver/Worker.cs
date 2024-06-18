using System.Diagnostics;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Modbus.Types;
using Modbus.Types.Interfaces;
using Orchestrator.Driver.Config;
using Orchestrator.Driver.Config.ColorSensor;
using Orchestrator.Driver.Exceptions;
using ServiceLayer.Helpers;
using ServiceLayer.Services;
using ServiceLayer.Types;

namespace Orchestrator.Driver;

/// <summary>
/// Implements the whole logic of the robot using commands in the service layer.
/// </summary>
public class Worker(ILogger<Worker> logger, IRobotService robotService, IConfiguration configuration, IRobotSoundService robotSoundService) : BackgroundService
{
    private readonly RobotVariablesOptions _options =
        configuration.GetSection(RobotVariablesOptions.RobotVariables).Get<RobotVariablesOptions>()!;
    private readonly ColorSensorInterpreter _colorSensorInterpreter = new();

    private readonly List<int> weights = new() { 0, 0, 0 };
    public bool AllGood { get; set; } = true;
    public string StatusMessage { get; set; } = "";
    public string SimpleMessage { get; set; } = "";
    public string CurrentOperation { get; set; } = "";
    public DateTime ObjectAwaitStartTime { get; set; }
    public bool BeltMoveErrorSound { get; set; } = false;
    public Task? SoundTask { get; set; } = null;
    
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        // await Task.Delay(500);
        await LightAllLights(false);
        await WriteToDisplay(DisplayMessageTypeEnum.MESSAGE, "Starting up!");
        await PlaySound(_options.Audio["StartingSound"]);
        await Task.Delay(1500);
        await DisplayBinWeights();
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await OpenBarrierAsync(false);
                await robotService.MoveBelt(new MoveBeltContinuousMessage() { Running = true });
                await WriteToDisplay(DisplayMessageTypeEnum.MESSAGE, "Awaiting object!");
                ObjectAwaitStartTime = DateTime.Now;
                BeltMoveErrorSound = false;
                while (!await ObjectAtBarrierAsync())
                {
                    if (DateTime.Now.Subtract(ObjectAwaitStartTime).Seconds >= 15 && !BeltMoveErrorSound)
                    {
                        await WriteToDisplay(DisplayMessageTypeEnum.MESSAGE, "Belt probably not moving!");
                        await PlaySound(_options.Audio["ErrorSound"]);
                        BeltMoveErrorSound = true;
                    }
                    await Task.Delay(250, stoppingToken);
                }
                await WriteToDisplay(DisplayMessageTypeEnum.MESSAGE, "");

                await robotService.MoveBelt(new MoveBeltContinuousMessage() { Running = false });
                await robotService.MoveBelt(new MoveBeltMessage()
                    { Distance = _options.BarrierAdjustmentDistance }); //by 20mm
                await WaitMotorStopAsync();
                await HandleObject();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                continue;
            }
            
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        robotService.MoveBelt(new MoveBeltContinuousMessage() { Running = false });
        OpenBarrierAsync();
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

            await PlaySound(_options.Audio["ErrorSound"]);
            await Task.Delay(1500);
            return;
        }
        
        if (minimalWeight > 0)
        {
            await WriteToDisplay(DisplayMessageTypeEnum.MESSAGE, "Moving " + objectName + " to bin " + minimalWeight); 
            await MoveBeltAsync(_options.ColorSensorPusherDistance + (minimalWeight - 1) * _options.InterPusherDistance);
            await PushAsync(minimalWeight - 1);
            weights[minimalWeight - 1] += weight;
            await DisplayBinsOnLeds();
            await DisplayBinWeights();
        }
        else
        {
            await LightAllLights();
            await WriteToDisplay(DisplayMessageTypeEnum.MESSAGE, "Bins full, please empty and press Enter!");
            await PlaySound(_options.Audio["EmptySound"]);
            
            Console.ReadLine(); // After this point we expect the bins to be empty.
            await LightAllLights(false);
            
            for (int i = 0; i < weights.Count; i++)
            {
                weights[i] = 0;
            }

            await 
                HandleKnownObject(objectName);
        }
    }

    private async Task DisplayBinsOnLeds()
    {
        SetLedStatesMessage message = new SetLedStatesMessage() { States = new []{weights[0] >= 40, weights[1] >= 40, weights[2] >= 40, false, false, false, false, false}};
        await robotService.SetLedStatesAsync(message);
    }
    
    private async Task LightAllLights(bool light = true)
    {
        SetLedStatesMessage message = new SetLedStatesMessage() { States = new []{light, light, light, false, false, false, false, false}};
        await robotService.SetLedStatesAsync(message);
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
                if(text == StatusMessage)
                    return;
                StatusMessage = text;
                break;
            case DisplayMessageTypeEnum.STATUSS_WARNING:
                text = text.Insert(0, "s1");
                if(text == StatusMessage)
                    return;
                StatusMessage = text;
                break;
            case DisplayMessageTypeEnum.STATUSS_ERROR:
                text = text.Insert(0, "s9");
                if(text == StatusMessage)
                    return;
                StatusMessage = text;
                break;
            case DisplayMessageTypeEnum.MESSAGE:
                text = text.Insert(0, "me");
                if(text == SimpleMessage)
                    return;
                SimpleMessage = text;
                break;
            case DisplayMessageTypeEnum.CURRENT_OP:
                text = text.Insert(0, "op");
                if(text == CurrentOperation)
                    return;
                CurrentOperation = text;
                break;
        }

        var response = await robotService.WriteToDisplay(new WriteToDisplayMessage(text));
        // await ThrowIfInvalidResponse(response);
    }

    private async Task ThrowToTrash()
    {
        await WriteToDisplay(DisplayMessageTypeEnum.CURRENT_OP, "Moving foreign object to trash!");
        await PlaySound(_options.Audio["ErrorSound"]);
        await MoveBeltAsync(_options.ColorSensorToTrashDistance);
    }
    
    private async Task<bool> ObjectAtBarrierAsync()
    {
        var depthSensorResponse = await robotService.ReadDepthSensorMessage();
        await ThrowIfInvalidResponse(depthSensorResponse);
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
        var response = await robotService
            .SetServoProgressions(
                new SetServoProgressionsMessage()
                {
                    Progressions = new List<ServoProgressionDto>()
                    {
                        new ServoProgressionDto()
                            { ServoId = 0, Progression = open ? (byte)0 : (byte)255 }
                    }
                });
        await ThrowIfInvalidResponse(response);
        await Task.Delay(_options.InterOperationDelayMs);
    }

    private async Task PushAsync(int pusherNumber)
    {
        var response = await robotService
            .SetServoProgressions(
                new SetServoProgressionsMessage()
                {
                    Progressions = new List<ServoProgressionDto>()
                    {
                        new ServoProgressionDto()
                            { ServoId = (byte)(pusherNumber + 1), Progression = 255 }
                    }
                });
        await ThrowIfInvalidResponse(response);
        await Task.Delay(_options.PusherMoveDelayMs);
        response = await robotService
            .SetServoProgressions(
                new SetServoProgressionsMessage()
                {
                    Progressions = new List<ServoProgressionDto>()
                    {
                        new ServoProgressionDto()
                            { ServoId = (byte)(pusherNumber + 1), Progression = 0 }
                    }
                });
        await ThrowIfInvalidResponse(response);
        await Task.Delay(_options.PusherMoveDelayMs);
    }

    private async Task MoveBeltAsync(int distance)
    {
        await ThrowIfInvalidResponse(
        await robotService.MoveBelt(new MoveBeltMessage() { Distance = distance })
        );
        await WaitMotorStopAsync();
    }

    private async Task<bool> MotorStillMovingAsync()
    {
        var motorStillMovingResponse = await robotService.IsMotorMoving();
        await ThrowIfInvalidResponse(motorStillMovingResponse);
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

    private bool ValidResponse<T>(ModbusResponse<T> response) where T : IModbusDeserializable<T>
    {
        return response.Success && response.DeviceStatus == 0 && response.Data is not null;
    }
    private async Task<string> ClassifyObjectWithColorSensorAsync()
    { 
        var response = (await robotService.ReadColorSensorData());
        await ThrowIfInvalidResponse(response);
        if (!ValidResponse(response) || response.Data!.Lux > _options.ColorSensor.LuxThreshold)
        {
            await WriteToDisplay(DisplayMessageTypeEnum.STATUSS_ERROR, "Color sensor error!");
            if (ValidResponse(response) && response.Data!.Lux > _options.ColorSensor.LuxThreshold)
            {
                await WriteToDisplay(DisplayMessageTypeEnum.CURRENT_OP, "Reduce light to proceed!");
            }

            while (!ValidResponse(response) || response.Data!.Lux > _options.ColorSensor.LuxThreshold)
            {
                response = await robotService.ReadColorSensorData();
                await ThrowIfInvalidResponse(response);
            }
        }
        // logger.LogInformation(response.Data!.Lux.ToString());
        var objectName = _colorSensorInterpreter.ColorFind(new Point(response.Data!.Red, response.Data.Green,
            response.Data.Blue));
        
        

        

        // Append to the file.
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            // Don't write the header again.
            HasHeaderRecord = false,
        };
        using (var stream = File.Open("./CollectedData/bulk.csv", FileMode.Append))
        using (var writer = new StreamWriter(stream))
        using (var csv = new CsvWriter(writer, config))
        {
            await csv.WriteRecordsAsync(new List<ReadColorSensorMessage>() {response.Data});
        }
        
        config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            // Don't write the header again.
            HasHeaderRecord = false,
        };
        using (var stream = File.Open("./CollectedData/classified.csv", FileMode.Append))
        using (var writer = new StreamWriter(stream))
        using (var csv = new CsvWriter(writer, config))
        {
            await csv.WriteRecordsAsync(new List<char>() {objectName[0]});
        }
        
        return objectName;
    }

    private async Task DisplayBinWeights()
    {
        await WriteToDisplay(DisplayMessageTypeEnum.CURRENT_OP,
            "B3:" + weights[2] + " " + "B2:" + weights[1] + " " + "B1:" + weights[0]);
    }

    private async Task ThrowIfInvalidResponse(ModbusResponse response)
    {
        if (response.DeviceStatus != 0)
        {
            await PlaySound(_options.Audio["ErrorSound"]);
            if(response.DeviceStatus == 1)
                await WriteToDisplay(DisplayMessageTypeEnum.STATUSS_ERROR, "Color sensor fail!");
            else if (response.DeviceStatus == 2)
                await WriteToDisplay(DisplayMessageTypeEnum.STATUSS_ERROR,"Depth sensor fail!");
            else if (response.DeviceStatus == 3)
                await WriteToDisplay(DisplayMessageTypeEnum.STATUSS_ERROR,"Color and Depth sensor fail!");
            throw new DeviceErrorException();
        }
        await WriteToDisplay(DisplayMessageTypeEnum.STATUSS_OK, "All systems are up!");
    }

    private async Task PlaySound(string sound)
    {
        if (SoundTask is not null)
            await SoundTask;
        SoundTask = robotSoundService.PlaySound(sound);
    }
}