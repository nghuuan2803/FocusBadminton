using MediatR;
using Domain.Entities;
using System;
using Domain.Repositories;

namespace Application.Features.Vouchers.Commands
{
    public class CreateVoucherCommand : IRequest<Voucher>
    {
        public int VoucherTemplateId { get; set; }
        public string AccountId { get; set; }
        public DateTimeOffset? Expiry { get; set; } 
    }
    public class CreateVoucherCommandHandler : IRequestHandler<CreateVoucherCommand, Voucher>
    {
        private readonly IRepository<VoucherTemplate> _voucherTemplateRepository;
        private readonly IRepository<Voucher> _voucherRepository;

        public CreateVoucherCommandHandler(
            IRepository<VoucherTemplate> voucherTemplateRepository,
            IRepository<Voucher> voucherRepository)
        {
            _voucherTemplateRepository = voucherTemplateRepository;
            _voucherRepository = voucherRepository;
        }

        public async Task<Voucher> Handle(CreateVoucherCommand request, CancellationToken cancellationToken)
        {
            // Lấy VoucherTemplate theo ID
            var template = await _voucherTemplateRepository.FindAsync(request.VoucherTemplateId, cancellationToken);
            if (template == null)
            {
                throw new Exception("Không tìm thấy mẫu voucher.");
            }

            // Tạo Voucher mới dựa trên template (Prototype Pattern)
            var voucher = template.Clone(request.AccountId, request.Expiry);

            // Thêm vào repository và lưu
            await _voucherRepository.AddAsync(voucher, cancellationToken);
            await _voucherRepository.SaveAsync(cancellationToken);

            return voucher;
        }
    }
}
