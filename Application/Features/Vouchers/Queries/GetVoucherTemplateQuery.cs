using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Vouchers.Queries
{
    public class GetVoucherTemplateQuery : IRequest<IEnumerable<VoucherTemplate>>
    {
    }
    public class GetVoucherTemplateQueryHandler : IRequestHandler<GetVoucherTemplateQuery, IEnumerable<VoucherTemplate>>
    {
        private readonly IRepository<VoucherTemplate> _voucherTemplateRepository;

        public GetVoucherTemplateQueryHandler(IRepository<VoucherTemplate> voucherTemplateRepository)
        {
            _voucherTemplateRepository = voucherTemplateRepository;
        }

        public async Task<IEnumerable<VoucherTemplate>> Handle(GetVoucherTemplateQuery request, CancellationToken cancellationToken)
        {
            // Lấy tất cả voucher templates
            return await _voucherTemplateRepository.GetAllAsync(cancellationToken: cancellationToken);
        }
    }
}
