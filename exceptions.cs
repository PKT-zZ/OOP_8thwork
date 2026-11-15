/****************/
// MSSV: 202418996
// Họ tên: Nguyễn Khánh Toàn
/****************/

namespace DeviceManagement;

public class InvalidDeviceReadingException : Exception
{
    public InvalidDeviceReadingException(string deviceId, int invalidValue)
        : base($"Giá trị {invalidValue} của thiết bị {deviceId} phải nằm trong khoảng 0–100.")
    {
        DeviceId = deviceId;
        InvalidValue = invalidValue;
    }

    public string DeviceId { get; }
    public int InvalidValue { get; }
}
