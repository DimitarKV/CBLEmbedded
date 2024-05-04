using Microsoft.AspNetCore.Mvc;
using SimulationTransferServer.Dto;
using SimulationTransferServer.Services.Interfaces;

namespace SimulationTransferServer.Controllers;

[ApiController]
[Route("/[controller]/[action]")]
public class SyncController(IPeripheralCommunication peripheral) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> All([FromForm] SyncAllDto dto)
    {
        await peripheral.WriteToDisplay(dto.swing1Rotation.ToString());
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> Display([FromForm] SyncDisplayDto dto)
    {
        await peripheral.WriteToDisplay(dto.Text);
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> ClearDisplay()
    {
        await peripheral.WriteToDisplay("");
        return Ok();
    }
}