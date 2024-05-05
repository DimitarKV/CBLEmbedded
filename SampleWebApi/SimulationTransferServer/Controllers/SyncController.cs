using Microsoft.AspNetCore.Mvc;
using SimulationTransferServer.Dto;
using SimulationTransferServer.Services;

namespace SimulationTransferServer.Controllers;

[ApiController]
[Route("/[controller]/[action]")]
public class SyncController(IRobotService robotService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> All([FromForm] SyncAllDto dto)
    {
        await robotService.WriteToDisplay(dto.swing1Rotation.ToString());
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> Display([FromForm] SyncDisplayDto dto)
    {
        await robotService.WriteToDisplay(dto.Text);
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> ClearDisplay()
    {
        await robotService.WriteToDisplay("");
        return Ok();
    }
}