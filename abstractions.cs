/****************/
// MSSV: 202418996
// Họ tên: Nguyễn Khánh Toàn
/****************/

namespace DeviceManagement;

public enum DeviceStatus
{
    Available,
    Borrowed,
    UnderMaintenance
}

public enum AlertSeverity
{
    Information,
    Warning,
    Critical
}

public class DeviceStatusChangedEventArgs : EventArgs
{
    public DeviceStatusChangedEventArgs(
        string deviceId,
        DeviceStatus oldStatus,
        DeviceStatus newStatus,
        DateTime changedAt)
    {
        DeviceId = deviceId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        ChangedAt = changedAt;
    }

    public string DeviceId { get; }
    public DeviceStatus OldStatus { get; }
    public DeviceStatus NewStatus { get; }
    public DateTime ChangedAt { get; }
}

public class DeviceAlertEventArgs : EventArgs
{
    public DeviceAlertEventArgs(
        string deviceId,
        string message,
        AlertSeverity severity,
        DateTime occurredAt)
    {
        DeviceId = deviceId;
        Message = message;
        Severity = severity;
        OccurredAt = occurredAt;
    }

    public string DeviceId { get; }
    public string Message { get; }
    public AlertSeverity Severity { get; }
    public DateTime OccurredAt { get; }
}

public abstract class TeachingDevice
{
    protected TeachingDevice(string id, string name)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Mã thiết bị không được rỗng.", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên thiết bị không được rỗng.", nameof(name));

        Id = id.Trim();
        Name = name.Trim();
        Status = DeviceStatus.Available;
    }

    public string Id { get; }
    public string Name { get; }
    public DeviceStatus Status { get; private set; }

    //Publisher cong bo su kien, khong biet subscriber nao se nhan.
    public event EventHandler<DeviceStatusChangedEventArgs>? StatusChanged;
    public event EventHandler<DeviceAlertEventArgs>? AlertRaised;

    public void ChangeStatus(DeviceStatus newStatus)
    {
        if (newStatus == Status)
            return;

        DeviceStatus oldStatus = Status;
        Status = newStatus;
        OnStatusChanged(new DeviceStatusChangedEventArgs(
            Id, oldStatus, newStatus, DateTime.Now));
    }

    protected void RaiseAlert(string message, AlertSeverity severity)
    {
        OnAlertRaised(new DeviceAlertEventArgs(
            Id, message, severity, DateTime.Now));
    }

    //Cac lop con co the mo rong cach phat, nhung event van chi duoc goi tu day.
    protected virtual void OnStatusChanged(DeviceStatusChangedEventArgs e) =>
        StatusChanged?.Invoke(this, e);

    protected virtual void OnAlertRaised(DeviceAlertEventArgs e) =>
        AlertRaised?.Invoke(this, e);
}
