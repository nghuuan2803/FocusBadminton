using Application.Common.Mappings;
using Application.Features.Vouchers.Commands;
using Application.Features.Vouchers.Queries;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Vouchers;

namespace Web.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class VouchersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly AppDbContext _dbContext;
        public VouchersController(IMediator mediator, AppDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }

        [HttpGet("get-voucher")]
        public async Task<ActionResult<VoucherDTO>> GetVoucher()
        {
            var data = await _dbContext.Vouchers.ToListAsync();
            return Ok(data);
        }
      
        // POST: api/vouchers
        [HttpPost]
        public async Task<ActionResult<VoucherDTO>> CreateVoucher([FromBody] CreateVoucherCommand command)
        {
            var voucher = await _mediator.Send(command);
            var voucherDTO = VoucherMapping.ToVoucherDTO(voucher);
            return CreatedAtAction(nameof(GetVoucherTemplates), new { id = voucherDTO.Id }, voucherDTO);
        }

        // GET: api/vouchers/templates
        [HttpGet("templates")]
        public async Task<ActionResult<IEnumerable<VoucherTemplateDTO>>> GetVoucherTemplates()
        {
            var query = new GetVoucherTemplateQuery();
            var templates = await _mediator.Send(query);
            var templateDTOs = templates.Select(VoucherMapping.ToVoucherTemplateDTO).ToList();
            return Ok(templateDTOs);
        }

        // POST: api/vouchers/templates
        [HttpPost("templates")]
        public async Task<ActionResult<VoucherTemplateDTO>> CreateVoucherTemplate([FromBody] CreateVoucherTemplateCommand command)
        {
            var template = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetVoucherTemplates), new { id = template.Id }, template);
        }
    }
}
