using Booking.Domain.Errors;
using Booking.Domain.Models;
using FluentValidation;

namespace Booking.API.Middlewares
{
    public class FluentValidationMiddleware
    {
        private readonly RequestDelegate _next;
        public FluentValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                var errors = new ValidationFailureResponse
                {
                    Errors = ex.Errors.Select(x => new ValidationResponse
                    {
                        PropertyName = x.PropertyName,
                        ErrorMessage = x.ErrorMessage,
                    })
                };
                await context.Response.WriteAsJsonAsync(errors);
            }
        }
    }
}
