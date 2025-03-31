using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Vouchers.Commands
{
    public class DeleteVoucherTemplateCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class DeleteVoucherTemplateCommandHandler : IRequestHandler<DeleteVoucherTemplateCommand, bool>
    {
        private readonly IRepository<VoucherTemplate> _voucherTemplateRepository;

        public DeleteVoucherTemplateCommandHandler(IRepository<VoucherTemplate> voucherTemplateRepository)
        {
            _voucherTemplateRepository = voucherTemplateRepository;
        }

        public async Task<bool> Handle(DeleteVoucherTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = await _voucherTemplateRepository.FindAsync(request.Id, cancellationToken);
            if (template == null)
            {
                return false;
            }

            _voucherTemplateRepository.Remove(template);
            await _voucherTemplateRepository.SaveAsync(cancellationToken);
            return true;
        }
    }
}
