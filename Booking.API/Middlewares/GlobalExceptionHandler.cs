using Booking.Application.Errors;
using Booking.Domain.Errors;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading;

namespace Booking.API.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService)
        {
            _logger = logger;
            _problemDetailsService = problemDetailsService;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var requestId = Activity.Current?.Id ?? httpContext.TraceIdentifier; 
            var (statusCode, title) = MapException(exception);
            var exceptionErrors = GetExceptionErrors(exception);
            httpContext.Response.StatusCode = statusCode;

            var problemDetailsContext = new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Title = title,
                    Status = statusCode,
                    Type = exception.GetType().Name,
                }
            };
            problemDetailsContext.ProblemDetails.Extensions["errors"] = exceptionErrors;
            return await _problemDetailsService.TryWriteAsync(problemDetailsContext);
        }

        private object GetExceptionErrors(Exception exception)
        {
            if (exception is ValidationException validationException)
            {
                return validationException.Errors.Select(x => new ValidationResponse
                {
                    PropertyName = x.PropertyName,
                    ErrorMessage = x.ErrorMessage,
                });
            }
            if (exception is NotFoundException notFoundException)
            {
                return new { notFoundException.ErrorMessage };
            }
            if (exception is UnauthorizedException unauthorizedException)
            {
                return new { unauthorizedException.ErrorMessage };
            }
            if (exception is ForbiddenException forbiddenException)
            {
                return new { forbiddenException.ErrorMessage };
            }
            return new { exception.Message };
        }

        private (int statusCode, string title) MapException(Exception exception) {
            return exception switch
            {
                ValidationException => (StatusCodes.Status400BadRequest, "Validation Failed"),
                NotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found"),
                UnauthorizedException => (StatusCodes.Status401Unauthorized, "Not Authorized"),
                ForbiddenException => (StatusCodes.Status403Forbidden, "Forbidden request"),
                _ => (StatusCodes.Status500InternalServerError, "Something is not working"),
            };
        }
    }
}
