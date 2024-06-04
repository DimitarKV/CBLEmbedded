using Orchestrator.Driver.Config.ColorSensor;
using Orchestrator.Driver.Config.DepthSensor;
using Orchestrator.Driver.Config.Motor;
using Orchestrator.Driver.Config.Pushers;

namespace Orchestrator.Driver.Config;

public class RobotVariablesOptions
{
    public const string RobotVariables = "RobotVariables";
    public MotorConfig Motor { get; set; }
    public DepthSensorConfig DepthSensor { get; set; }
    public ColorSensorConfig ColorSensor { get; set; }
    public List<ServoConfig> Pushers { get; set; }
    public int BarrierPassingDistance { get; set; }
    public int BarrierColorSensorDistance { get; set; }
    public int InterOperationDelayMs { get; set; }
}