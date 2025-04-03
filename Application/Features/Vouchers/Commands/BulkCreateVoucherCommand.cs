using MediatR;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Repositories;

namespace Application.Features.Vouchers.Commands
{
    public class BulkCreateVoucherCommand : IRequest<List<Voucher>>
    {
        public int VoucherTemplateId { get; set; }
        public List<string> AccountIds { get; set; }
        public DateTimeOffset? Expiry { get; set; }
    }

    public class BulkCreateVoucherCommandHandler : IRequestHandler<BulkCreateVoucherCommand, List<Voucher>>
    {
        private readonly IRepository<VoucherTemplate> _voucherTemplateRepository;
        private readonly IRepository<Voucher> _voucherRepository;

        public BulkCreateVoucherCommandHandler(
            IRepository<VoucherTemplate> voucherTemplateRepository,
            IRepository<Voucher> voucherRepository)
        {
            _voucherTemplateRepository = voucherTemplateRepository;
            _voucherRepository = voucherRepository;
        }

        public async Task<List<Voucher>> Handle(BulkCreateVoucherCommand request, CancellationToken cancellationToken)
        {
            var template = await _voucherTemplateRepository.FindAsync(request.VoucherTemplateId, cancellationToken);
            if (template == null)
            {
                throw new Exception("Không tìm thấy mẫu voucher.");
            }

            var vouchers = request.AccountIds.Select(accountId => template.Clone(accountId, request.Expiry)).ToList();

            await _voucherRepository.AddRangeAsync(vouchers, cancellationToken);
            await _voucherRepository.SaveAsync(cancellationToken);

            return vouchers;
        }
    }
}
