using Application.Common.Mappings;
using Domain.Repositories;
using Shared.Enums;
using Shared.Vouchers;

namespace Application.Features.Vouchers.Commands
{
    public class UpdateVoucherTemplateCommand : IRequest<VoucherTemplateDTO>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DiscountType { get; set; } = "Percent";
        public double Value { get; set; }
        public double MaximumValue { get; set; }
        public int Duration { get; set; }
    }

    public class UpdateVoucherTemplateCommandHandler : IRequestHandler<UpdateVoucherTemplateCommand, VoucherTemplateDTO>
    {
        private readonly IRepository<VoucherTemplate> _voucherTemplateRepository;

        public UpdateVoucherTemplateCommandHandler(IRepository<VoucherTemplate> voucherTemplateRepository)
        {
            _voucherTemplateRepository = voucherTemplateRepository;
        }

        public async Task<VoucherTemplateDTO> Handle(UpdateVoucherTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = await _voucherTemplateRepository.FindAsync(request.Id, cancellationToken);
            if (template == null)
            {
                throw new Exception("Không tìm thấy mẫu voucher.");
            }

            template.Name = request.Name;
            template.Description = request.Description;
            template.DiscountType = Enum.Parse<DiscountType>(request.DiscountType);
            template.Value = request.Value;
            template.MaximumValue = request.MaximumValue;
            template.Duration = request.Duration;

            _voucherTemplateRepository.Update(template);
            await _voucherTemplateRepository.SaveAsync(cancellationToken);

            return VoucherMapping.ToVoucherTemplateDTO(template);
        }
    }
}
