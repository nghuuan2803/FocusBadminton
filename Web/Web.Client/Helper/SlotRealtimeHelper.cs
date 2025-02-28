using Microsoft.AspNetCore.SignalR.Client;

namespace Web.Client.Helper
{
    public class SlotRealtimeHelper : IAsyncDisposable
    {
        private readonly HubConnection _hubConnection;

        // Events để thông báo cho UI
        public event Action<object>? OnSlotHeld;
        public event Action<object>? OnSlotReleased;
        public event Action<int>? OnBookingCreated;

        public SlotRealtimeHelper(string hubUrl)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            // Đăng ký nhận tín hiệu từ SlotHub
            _hubConnection.On<object>("SlotHeld", payload =>
            {
                OnSlotHeld?.Invoke(payload);
            });

            _hubConnection.On<object>("SlotReleased", payload =>
            {
                OnSlotReleased?.Invoke(payload);
            });

            _hubConnection.On<int>("BookingCreated", bookingId =>
            {
                OnBookingCreated?.Invoke(bookingId);
            });

            // Xử lý các trạng thái kết nối
            _hubConnection.Closed += async (error) =>
            {
                await Task.CompletedTask;
            };

            _hubConnection.Reconnecting += async (error) =>
            {
                await Task.CompletedTask;
            };

            _hubConnection.Reconnected += async (connectionId) =>
            {
                await Task.CompletedTask;
            };
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await _hubConnection.StartAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (_hubConnection.State != HubConnectionState.Disconnected)
            {
                try
                {
                    await _hubConnection.StopAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            await StopAsync();
            await _hubConnection.DisposeAsync();
        }
    }
}
