/****************/
// MSSV: 202418996
// Họ tên: Nguyễn Khánh Toàn
/****************/

using System.Text;

namespace DeviceManagement;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var projector = new ProjectorRemote("PJ01", "Remote Epson", 80);
        var airConditioner = new AirConditionerRemote("AC01", "Remote Daikin", "A203");
        var microphone = new WirelessMicrophone("MIC01", "Micro không dây", 80, 80);
        TeachingDevice[] devices = [projector, airConditioner, microphone];

        var notifications = new ConsoleNotificationService();
        var auditLogger = new DeviceAuditLogger();
        var maintenance = new MaintenanceService();
        using var coordinator = new DeviceMonitoringCoordinator(
            devices, notifications, auditLogger, maintenance);

        //1. Thay doi trang thai va kiem tra Subscribe() idempotent.
        PrintHeader("1. TRANG THAI VA NHAT KY");
        coordinator.Subscribe();
        coordinator.Subscribe();
        Console.WriteLine($"  Trang thai ban dau: {projector.Status}");
        bool statusUpdatedBeforeEvent = false;
        EventHandler<DeviceStatusChangedEventArgs> checkUpdatedStatus =
            (_, e) => statusUpdatedBeforeEvent = projector.Status == e.NewStatus;
        projector.StatusChanged += checkUpdatedStatus;
        projector.ChangeStatus(DeviceStatus.Borrowed);
        projector.StatusChanged -= checkUpdatedStatus;
        projector.ChangeStatus(DeviceStatus.Available);
        projector.ChangeStatus(DeviceStatus.Available);
        Console.WriteLine($"  Status da cap nhat truoc khi subscriber nhan event: {statusUpdatedBeforeEvent}");
        Console.WriteLine($"  So muc nhat ky (2 lan doi): {auditLogger.Entries.Count}");
        Console.WriteLine($"  Ban ghi cuoi: {auditLogger.Entries[^1]}");

        //2. Canh bao pin may chieu va chi ghi bao tri cho Critical.
        PrintHeader("2. CANH BAO MAY CHIEU");
        projector.UpdateBatteryLevel(19);
        projector.UpdateBatteryLevel(18);
        projector.UpdateBatteryLevel(0);
        projector.UpdateBatteryLevel(10);
        projector.UpdateBatteryLevel(30);
        Console.WriteLine($"  So yeu cau bao tri: {maintenance.Requests.Count}");

        //3. Dieu hoa chi canh bao khi vua mat kha nang phan hoi.
        PrintHeader("3. CANH BAO DIEU HOA");
        airConditioner.UpdateResponseState(false);
        airConditioner.UpdateResponseState(false);

        //4. Pin va chat luong tin hieu micro duoc danh gia rieng.
        PrintHeader("4. CANH BAO MICRO");
        int filteredCount = 0;
        var criticalOnly = new FilteredAlertSubscriber(
            alert => alert.Severity == AlertSeverity.Critical,
            alert =>
            {
                filteredCount++;
                Console.WriteLine($"  CRITICAL FILTER: {alert.DeviceId} - {alert.Message}");
            });
        microphone.AlertRaised += criticalOnly.HandleAlert;
        microphone.UpdateBatteryLevel(25);
        microphone.UpdateSignalQuality(35);
        Console.WriteLine($"  Warning bi bo qua boi delegate: {filteredCount == 0}");
        microphone.UpdateBatteryLevel(0);
        microphone.UpdateSignalQuality(0);
        Console.WriteLine($"  Critical duoc delegate chap nhan: {filteredCount}");
        microphone.AlertRaised -= criticalOnly.HandleAlert;
        Console.WriteLine($"  Tong yeu cau bao tri sau Critical: {maintenance.Requests.Count}");

        //5. Dau vao sai bi tu choi truoc khi property thay doi hay event phat ra.
        PrintHeader("5. KIEM TRA DAU VAO");
        int previousBattery = projector.BatteryLevel;
        int previousSignal = microphone.SignalQuality;
        int previousAlertCount = maintenance.Requests.Count;
        int invalidInputEvents = 0;
        EventHandler<DeviceAlertEventArgs> countInvalidInputEvents = (_, _) => invalidInputEvents++;
        projector.AlertRaised += countInvalidInputEvents;
        microphone.AlertRaised += countInvalidInputEvents;
        TryInvalidReading(() => projector.UpdateBatteryLevel(-1));
        TryInvalidReading(() => microphone.UpdateSignalQuality(101));
        projector.AlertRaised -= countInvalidInputEvents;
        microphone.AlertRaised -= countInvalidInputEvents;
        Console.WriteLine($"  Pin may chieu giu nguyen: {projector.BatteryLevel == previousBattery}");
        Console.WriteLine($"  Tin hieu micro giu nguyen: {microphone.SignalQuality == previousSignal}");
        Console.WriteLine($"  Khong phat event khi loi: {invalidInputEvents == 0}");
        Console.WriteLine($"  Khong tao them bao tri: {maintenance.Requests.Count == previousAlertCount}");

        //6. Huy dang ky nhieu lan an toan; log khong tang sau khi huy.
        PrintHeader("6. HUY DANG KY");
        int previousEntries = auditLogger.Entries.Count;
        coordinator.Unsubscribe();
        coordinator.Unsubscribe();
        projector.ChangeStatus(DeviceStatus.Borrowed);
        int previousRequests = maintenance.Requests.Count;
        projector.UpdateBatteryLevel(10);
        coordinator.Dispose();
        Console.WriteLine($"  Audit khong tang sau Unsubscribe: {auditLogger.Entries.Count == previousEntries}");
        Console.WriteLine($"  Subscriber canh bao da huy dang ky: {maintenance.Requests.Count == previousRequests}");
        Console.WriteLine("  Dispose sau Unsubscribe: thanh cong");
    }

    private static void TryInvalidReading(Action update)
    {
        try
        {
            update();
        }
        catch (InvalidDeviceReadingException ex)
        {
            Console.WriteLine($"  {ex.GetType().Name}: {ex.DeviceId}, gia tri {ex.InvalidValue}");
        }
    }

    private static void PrintHeader(string title)
    {
        Console.WriteLine();
        Console.WriteLine($"===== {title} =====");
    }
}


