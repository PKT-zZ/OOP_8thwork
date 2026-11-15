/****************/
// MSSV: 202418996
// Họ tên: Nguyễn Khánh Toàn
/****************/

namespace DeviceManagement;

public class ProjectorRemote : TeachingDevice
{
    public ProjectorRemote(string id, string name, int batteryLevel)
        : base(id, name)
    {
        ValidateReading(batteryLevel);
        BatteryLevel = batteryLevel;
    }

    public int BatteryLevel { get; private set; }

    public void UpdateBatteryLevel(int newLevel)
    {
        ValidateReading(newLevel);
        AlertSeverity oldSeverity = GetBatterySeverity(BatteryLevel);
        AlertSeverity newSeverity = GetBatterySeverity(newLevel);
        BatteryLevel = newLevel;

        if (newSeverity != oldSeverity && newSeverity != AlertSeverity.Information)
            RaiseAlert($"Pin máy chiếu còn {newLevel}%.", newSeverity);
    }

    private void ValidateReading(int value)
    {
        if (value < 0 || value > 100)
            throw new InvalidDeviceReadingException(Id, value);
    }

    private static AlertSeverity GetBatterySeverity(int level) =>
        level == 0 ? AlertSeverity.Critical
        : level < 20 ? AlertSeverity.Warning
        : AlertSeverity.Information;
}

public class AirConditionerRemote : TeachingDevice
{
    public AirConditionerRemote(string id, string name, string assignedRoom)
        : base(id, name)
    {
        if (string.IsNullOrWhiteSpace(assignedRoom))
            throw new ArgumentException("Phòng được gán không được rỗng.", nameof(assignedRoom));

        AssignedRoom = assignedRoom.Trim();
        IsResponsive = true;
    }

    public string AssignedRoom { get; }
    public bool IsResponsive { get; private set; }

    public void UpdateResponseState(bool isResponsive)
    {
        bool wasResponsive = IsResponsive;
        IsResponsive = isResponsive;

        if (wasResponsive && !isResponsive)
            RaiseAlert($"Điều khiển điều hòa tại {AssignedRoom} không phản hồi.", AlertSeverity.Critical);
    }
}

public class WirelessMicrophone : TeachingDevice
{
    public WirelessMicrophone(string id, string name, int batteryLevel, int signalQuality)
        : base(id, name)
    {
        ValidateReading(batteryLevel);
        ValidateReading(signalQuality);
        BatteryLevel = batteryLevel;
        SignalQuality = signalQuality;
    }

    public int BatteryLevel { get; private set; }
    public int SignalQuality { get; private set; }

    public void UpdateBatteryLevel(int newLevel)
    {
        ValidateReading(newLevel);
        AlertSeverity oldSeverity = GetBatterySeverity(BatteryLevel);
        AlertSeverity newSeverity = GetBatterySeverity(newLevel);
        BatteryLevel = newLevel;

        if (newSeverity != oldSeverity && newSeverity != AlertSeverity.Information)
            RaiseAlert($"Pin micro còn {newLevel}%.", newSeverity);
    }

    public void UpdateSignalQuality(int newQuality)
    {
        ValidateReading(newQuality);
        AlertSeverity oldSeverity = GetSignalSeverity(SignalQuality);
        AlertSeverity newSeverity = GetSignalSeverity(newQuality);
        SignalQuality = newQuality;

        if (newSeverity != oldSeverity && newSeverity != AlertSeverity.Information)
            RaiseAlert($"Chất lượng tín hiệu micro còn {newQuality}%.", newSeverity);
    }

    private void ValidateReading(int value)
    {
        if (value < 0 || value > 100)
            throw new InvalidDeviceReadingException(Id, value);
    }

    private static AlertSeverity GetBatterySeverity(int level) =>
        level == 0 ? AlertSeverity.Critical
        : level < 30 ? AlertSeverity.Warning
        : AlertSeverity.Information;

    private static AlertSeverity GetSignalSeverity(int quality) =>
        quality == 0 ? AlertSeverity.Critical
        : quality < 40 ? AlertSeverity.Warning
        : AlertSeverity.Information;
}
