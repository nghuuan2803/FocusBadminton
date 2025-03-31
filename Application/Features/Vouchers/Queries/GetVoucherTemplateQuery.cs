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
        public string? Name { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
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
            var templates = await _voucherTemplateRepository.GetAllAsync(cancellationToken: cancellationToken);

            // Lọc theo tên nếu có
            if (!string.IsNullOrEmpty(request.Name))
            {
                templates = templates.Where(t => t.Name.Contains(request.Name, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Phân trang
            return templates
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();
        }
    }
}
