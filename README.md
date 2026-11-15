# OOP W10 - Smart Home Mini

## Chạy chương trình

```powershell
dotnet run --project .\smartHomeMini.csproj
```

## Các pattern được dùng

- **Factory** (`DeviceFactory`): chọn và tạo `SmartLight`, `SmartFan` hoặc `SmartDoor`, trả về `IDevice`. Client không gọi constructor cụ thể. Lợi ích là gom quy tắc tạo đối tượng; hạn chế là khi thêm loại thiết bị phải cập nhật factory.
- **Singleton** (`HomeConfiguration`): `Instance` dùng chung cấu hình ngôi nhà và mức sáng ban đêm. Lợi ích là mọi nơi đọc cùng một cấu hình; hạn chế là trạng thái dùng chung có thể gây khó khăn khi kiểm thử và thay đổi cấu hình toàn cục.
- **Facade** (`SmartHomeFacade`): cung cấp `GoToWork()` và `GoToSleep()` để gọi chuỗi thao tác trên thiết bị. Lợi ích là client không phải tự nhớ thứ tự bật/tắt/khóa; hạn chế là facade chỉ bao phủ các chế độ đã định nghĩa, còn thao tác tùy ý vẫn cần gọi thiết bị riêng.

## Kiểm tra minh họa

Chạy chương trình sẽ hiển thị kết quả cho các tình huống T1–T5: tạo ba loại thiết bị, từ chối loại `camera`, so sánh hai lần lấy `HomeConfiguration.Instance`, chạy chế độ đi làm và đi ngủ. Mỗi tình huống in `PASS` nếu kết quả đúng.

## Câu hỏi

1. **Factory đang che giấu quyết định thiết kế nào?** Client không cần biết constructor cụ thể nào cần gọi cho từng loại; client yêu cầu loại qua chuỗi và nhận về `IDevice`.
2. **Singleton bảo đảm điều gì, nhưng tạo rủi ro nào?** Nó bảo đảm các lần truy cập `Instance` cùng trả về một đối tượng. Trạng thái toàn cục dùng chung có thể làm các phần phụ thuộc nhau và khiến kiểm thử khó cô lập.
3. **Facade khác Factory ở mục đích chính như thế nào?** Factory tạo đúng loại đối tượng theo yêu cầu; Facade gom nhiều thao tác đối tượng thành một lời gọi mức cao.
4. **Nếu bỏ `SmartHomeFacade`, client phải biết thêm gì?** Client phải tự gọi từng thiết bị và biết thứ tự tắt đèn, tắt quạt, khóa cửa hoặc bật đèn ngủ, cũng như đọc cấu hình mức sáng.

## Sơ đồ lớp

Xem [uml.md](uml.md).
