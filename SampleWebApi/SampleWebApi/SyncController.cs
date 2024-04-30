using Microsoft.AspNetCore.Mvc;
using SampleWebApi.Services.Interfaces;

namespace SampleWebApi;

[ApiController]
[Route("/[controller]/[action]")]
public class SyncController : ControllerBase
{
    public ISerialCommunication SerialCommunication { get; set; }
    
    public SyncController(ISerialCommunication serialCommunication)
    {
        SerialCommunication = serialCommunication;
    }

    
    [HttpPost]
    public async Task<IActionResult> All([FromForm] SyncAllDTO dto)
    {
        SerialCommunication.GetSerialPort().Write("WTD" + dto.swing1Rotation + "\r\n");
        return Ok();
    }
}