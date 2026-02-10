using HMS.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.ServiceAbstraction
{
    public interface IPaymentService
    {
        Task<GenericResponse<string>> CreatePaymentUrlAsync(Guid bookingId);
    }
}
