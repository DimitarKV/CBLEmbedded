using Microsoft.AspNetCore.Mvc;
using SimulationTransferServer.Services.Interfaces;

namespace SimulationTransferServer;

[ApiController]
[Route("/[controller]/[action]")]
public class SyncController(IPeripheralCommunication _peripheral) : ControllerBase
{
    private IPeripheralCommunication _peripheral = _peripheral;
    [HttpPost]
    public async Task<IActionResult> All([FromForm] SyncAllDTO dto)
    {
        _peripheral.WriteToDisplay(dto.swing1Rotation.ToString());
        return Ok();
    }
}