using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Services;
using ServiceLayer.Types;
using SimulationTransferServer.Dto;

namespace SimulationTransferServer.Controllers;

[ApiController]
[Route("/[controller]/[action]")]
public class SyncController(IRobotService robotService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> All([FromForm] SyncAllDto dto)
    {
        await robotService.WriteToDisplay(new WriteToDisplayMessage(dto.swing1Rotation.ToString()));
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Display([FromForm] SyncDisplayDto dto)
    {
        await robotService.WriteToDisplay(new WriteToDisplayMessage(dto.Text));
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> ClearDisplay()
    {
        await robotService.WriteToDisplay(new WriteToDisplayMessage(""));
        return Ok();
    }

    public async Task<IActionResult> ReadColorSensor()
    {
        var message = await robotService.ReadColorSensorData();
        return Ok(message);
    }

    [HttpPost]
    public async Task<IActionResult> SetServoPos([FromBody] List<ServoPosDto> dto)
    {
        await robotService.SetServoPos(new SetServoPositionsMessage() {ServoParameters = dto});
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> SetServoProgressions([FromBody] List<ServoProgressionDto> dto)
    {
        await robotService.SetServoProgressions(new SetServoProgressionsMessage() {Progressions = dto});
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> MoveBelt([FromBody] MoveBeltMessage message)
    {
        await robotService.MoveBelt(message);
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> MoveBeltContinuous([FromBody] MoveBeltContinuousMessage message)
    {
        await robotService.MoveBelt(message);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> MoveBeltSteps([FromBody] MoveBeltStepsMessage message)
    {
        await robotService.MoveBeltSteps(message);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> IsMotorRunning()
    {
        return Ok(await robotService.IsMotorMoving());
    }
    
    [HttpGet]
    public async Task<IActionResult> ReadDepthSensor()
    {
        var result = await robotService.ReadDepthSensorMessage();
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> ReportTimes()
    {
        await robotService.ToggleReportTimes();
        return Ok();
    }

    private static CancellationTokenSource _cancellationTokenSource = new();
    private async Task Worker(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("Here");
            await Task.Delay(1000, cancellationToken);
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> StartTask()
    {
        Task.Run(() => Worker(_cancellationTokenSource.Token));
        return Ok();
    }
    [HttpGet]
    public async Task<IActionResult> StopTask()
    {
        await _cancellationTokenSource.CancelAsync();
        return Ok();
    }
}