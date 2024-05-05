using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SimulationTransferServer.Dto;
using SimulationTransferServer.Services;
using SimulationTransferServer.Types;

namespace SimulationTransferServer.Controllers;

[ApiController]
[Route("/[controller]/[action]")]
public class SyncController(IRobotService robotService) : ControllerBase
{
    private static float averageResponseTime = 0;
    private static int nMeasurements = 0;

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

    //Average response time: 4381.6523us (40000 measurements) over 27116ms
    public async Task<IActionResult> Test()
    {
        Stopwatch sw1 = new Stopwatch();
        Stopwatch sw2 = new Stopwatch();
        sw1.Start();
        for (int i = 0; i < 20000; i++)
        {
            sw2.Reset();
            sw2.Start();
            robotService.ReadDummySensor(new ReadDummySensorMessage()).Wait();
            sw2.Stop();
            averageResponseTime = (averageResponseTime * nMeasurements++ + (sw2.Elapsed.Milliseconds * 1000 + sw2.Elapsed.Microseconds)) / nMeasurements;
        }
        sw1.Stop();
        
        Console.WriteLine("Average response time: {0}us ({1} measurements) over {2}ms", averageResponseTime, nMeasurements, sw1.Elapsed.Seconds * 1000 + sw1.Elapsed.Milliseconds);
        var message = await robotService.ReadDummySensor(new ReadDummySensorMessage());
        return Ok(message);
    }
}