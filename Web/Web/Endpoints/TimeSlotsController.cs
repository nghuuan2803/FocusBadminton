﻿using Application.Features.Statictis;
using Application.Features.TimeSlots.Commands;
using Application.Features.TimeSlots.Queries;
using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSlotsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TimeSlotsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellation)
        {
            var result = await _mediator.Send(new GetTimeSlotsQuery(), cancellation);
            return Ok(result);
        }

        [HttpPost("statictis")]
        public async Task<IActionResult> GetStatictis(TimeSlotStatictisQuery request, CancellationToken cancellation)
        {
            var result = await _mediator.Send(request, cancellation);
            return Ok(result);
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update(SetTimeSlotPriceCommands request, CancellationToken cancellation)
        {
            var result = await _mediator.Send(request, cancellation);
            return Ok(result);
        }
    }
}
