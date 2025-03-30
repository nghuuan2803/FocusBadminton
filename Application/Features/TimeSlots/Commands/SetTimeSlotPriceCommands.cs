using Domain.Repositories;

namespace Application.Features.TimeSlots.Commands
{
    public record SetTimeSlotPriceCommands(int id, double price) : IRequest<bool>
    {
    }

    public class SetTimeSlotPriceCommandsHandler : IRequestHandler<SetTimeSlotPriceCommands, bool>
    {
        private readonly IRepository<TimeSlot> _repository;

        public SetTimeSlotPriceCommandsHandler(IRepository<TimeSlot> repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(SetTimeSlotPriceCommands request, CancellationToken cancellationToken)
        {
            var timeSlot = await _repository.FindAsync(request.id);
            if (timeSlot == null)
                return false;
            timeSlot.Price = request.price;
            timeSlot.UpdatedAt = DateTimeOffset.Now;
            timeSlot.UpdatedBy = "Admin";
            _repository.Update(timeSlot);
            await _repository.SaveAsync();
            return true;
        }
    }
}
