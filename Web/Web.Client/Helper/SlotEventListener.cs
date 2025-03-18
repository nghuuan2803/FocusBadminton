using Microsoft.AspNetCore.SignalR.Client;

namespace Web.Client.Helper
{
    public class SlotEventHelper : IAsyncDisposable, ISlotEventHelper
    {
        private readonly HubConnection _hubConnection;

        // Events để thông báo cho UI
        public event Action<object>? OnSlotHeld;
        public event Action<object>? OnSlotReleased;
        public event Action<object>? OnBookingCreated;
        public event Action<object>? OnBookingCanceled;
        public event Action<object>? OnBookingApproved;
        public event Action<object>? OnBookingRejected;
        public event Action<object>? OnBookingCompleted;
        public event Action<object>? OnBookingPaused;
        public event Action<object>? OnBookingResumed;

        public SlotEventHelper(string hubUrl)
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

            _hubConnection.On<object>("BookingCreated", payload =>
            {
                OnBookingCreated?.Invoke(payload);
            });
            _hubConnection.On<object>("BookingCanceled", payload =>
            {
                OnBookingCanceled?.Invoke(payload);
            });
            _hubConnection.On<object>("BookingApproved", payload =>
            {
                OnBookingApproved?.Invoke(payload);
            });
            _hubConnection.On<object>("BookingRejected", payload =>
            {
                OnBookingRejected?.Invoke(payload);
            });
            _hubConnection.On<object>("BookingCompeted", payload =>
            {
                OnBookingCompleted?.Invoke(payload);
            });
            _hubConnection.On<object>("BookingPaused", payload =>
            {
                OnBookingPaused?.Invoke(payload);
            });
            _hubConnection.On<object>("BookingResumed", payload =>
            {
                OnBookingResumed?.Invoke(payload);
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
                    Console.WriteLine("hub connect success");
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
