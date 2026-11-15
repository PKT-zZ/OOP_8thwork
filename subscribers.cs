/****************/
// MSSV: 202418996
// Họ tên: Nguyễn Khánh Toàn
/****************/

namespace DeviceManagement;

public class ConsoleNotificationService
{
    public void HandleAlert(object? sender, DeviceAlertEventArgs e) =>
        Console.WriteLine($"  [{e.Severity}] {e.DeviceId}: {e.Message}");
}

public class DeviceAuditLogger
{
    private readonly List<string> _entries = new();

    public IReadOnlyList<string> Entries => _entries.AsReadOnly();

    public void HandleStatusChanged(object? sender, DeviceStatusChangedEventArgs e) =>
        _entries.Add($"{e.ChangedAt:HH:mm:ss} {e.DeviceId}: {e.OldStatus} -> {e.NewStatus}");
}

public class MaintenanceService
{
    private readonly List<string> _requests = new();

    public IReadOnlyList<string> Requests => _requests.AsReadOnly();

    public void HandleCriticalAlert(object? sender, DeviceAlertEventArgs e)
    {
        if (e.Severity != AlertSeverity.Critical)
            return;

        _requests.Add($"Maintenance request for {e.DeviceId}: {e.Message}");
    }
}
