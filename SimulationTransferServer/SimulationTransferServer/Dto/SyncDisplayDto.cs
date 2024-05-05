using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SimulationTransferServer.Dto;

public class SyncDisplayDto
{
    [ValidateNever]
    public string Text { get; set; }
}