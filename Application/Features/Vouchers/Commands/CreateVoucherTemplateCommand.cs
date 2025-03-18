using Application.Common.Mappings;
using Domain.Repositories;
using Shared.Enums;
using Shared.Vouchers;


namespace Application.Features.Vouchers.Commands
{
    public class CreateVoucherTemplateCommand : IRequest<VoucherTemplateDTO>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DiscountType DiscountType { get; set; }
        public double Value { get; set; }
        public double MaximumValue { get; set; }
        public int Duration { get; set; }
    }
    public class CreateVoucherTemplateCommandHandler : IRequestHandler<CreateVoucherTemplateCommand, VoucherTemplateDTO>
    {
        private readonly IRepository<VoucherTemplate> _voucherTemplateRepository;

        public CreateVoucherTemplateCommandHandler(IRepository<VoucherTemplate> voucherTemplateRepository)
        {
            _voucherTemplateRepository = voucherTemplateRepository;
        }

        public async Task<VoucherTemplateDTO> Handle(CreateVoucherTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = new VoucherTemplate
            {
                Name = request.Name,
                Description = request.Description,
                DiscountType = request.DiscountType,
                Value = request.Value,
                MaximumValue = request.MaximumValue,
                Duration = request.Duration
            };

            await _voucherTemplateRepository.AddAsync(template, cancellationToken);
            await _voucherTemplateRepository.SaveAsync(cancellationToken);

            return VoucherMapping.ToVoucherTemplateDTO(template);
        }
    }
}
