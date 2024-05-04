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
        peripheral.WriteToDisplay(dto.swing1Rotation.ToString());
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> Display([FromForm] SyncDisplayDto dto)
    {
        peripheral.WriteToDisplay(dto.Text);
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> ClearDisplay()
    {
        peripheral.WriteToDisplay("");
        return Ok();
    }
}