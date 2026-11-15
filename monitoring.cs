/****************/
// MSSV: 202418996
// Họ tên: Nguyễn Khánh Toàn
/****************/

namespace DeviceManagement;

public delegate bool AlertFilter(DeviceAlertEventArgs alert);

public class FilteredAlertSubscriber
{
    private readonly AlertFilter _filter;
    private readonly Action<DeviceAlertEventArgs> _action;

    public FilteredAlertSubscriber(
        AlertFilter filter,
        Action<DeviceAlertEventArgs> action)
    {
        _filter = filter ?? throw new ArgumentNullException(nameof(filter));
        _action = action ?? throw new ArgumentNullException(nameof(action));
    }

    public void HandleAlert(object? sender, DeviceAlertEventArgs e)
    {
        if (_filter(e))
            _action(e);
    }
}

public class DeviceMonitoringCoordinator : IDisposable
{
    private readonly List<TeachingDevice> _devices;
    private readonly ConsoleNotificationService _notificationService;
    private readonly DeviceAuditLogger _auditLogger;
    private readonly MaintenanceService _maintenanceService;
    private bool _isSubscribed;

    public DeviceMonitoringCoordinator(
        IEnumerable<TeachingDevice> devices,
        ConsoleNotificationService notificationService,
        DeviceAuditLogger auditLogger,
        MaintenanceService maintenanceService)
    {
        _devices = devices?.Distinct().ToList()
            ?? throw new ArgumentNullException(nameof(devices));
        _notificationService = notificationService
            ?? throw new ArgumentNullException(nameof(notificationService));
        _auditLogger = auditLogger
            ?? throw new ArgumentNullException(nameof(auditLogger));
        _maintenanceService = maintenanceService
            ?? throw new ArgumentNullException(nameof(maintenanceService));
    }

    public void Subscribe()
    {
        if (_isSubscribed)
            return;

        foreach (TeachingDevice device in _devices)
        {
            device.StatusChanged += _auditLogger.HandleStatusChanged;
            device.AlertRaised += _notificationService.HandleAlert;
            device.AlertRaised += _maintenanceService.HandleCriticalAlert;
        }

        _isSubscribed = true;
    }

    public void Unsubscribe()
    {
        if (!_isSubscribed)
            return;

        foreach (TeachingDevice device in _devices)
        {
            device.StatusChanged -= _auditLogger.HandleStatusChanged;
            device.AlertRaised -= _notificationService.HandleAlert;
            device.AlertRaised -= _maintenanceService.HandleCriticalAlert;
        }

        _isSubscribed = false;
    }

    public void Dispose() => Unsubscribe();
}
