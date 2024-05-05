using Microsoft.AspNetCore.Mvc;
using SimulationTransferServer.Dto;
using SimulationTransferServer.Services;
using SimulationTransferServer.Types;

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

    public async Task<IActionResult> Test()
    {
        var message = await robotService.ReadDummySensor(new ReadDummySensorMessage());
        return Ok(message);
    }
}