using Booking.Application.Errors;
using Booking.Domain.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                //Add logging
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.ContentType = "application/problem+json";

                var problem = new ProblemDetails
                {
                    Type = "/errors/not-found", //can be more specific
                    Title = "Resource Not Found",
                    Detail = ex.ErrorMessage, // service exception property
                    Status = ex.StatusCode, // service exception property
                    Instance = context.Request.Path
                };

                await context.Response.WriteAsJsonAsync(problem);
            }
            catch (ForbiddenException ex)
            {
                //Add logging
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/problem+json";

                var problem = new ProblemDetails
                {
                    Type = "/errors/forbidden", //can be more specific
                    Title = "Unauthorized Action",
                    Detail = ex.ErrorMessage, // service exception property
                    Status = ex.StatusCode, // service exception property
                    Instance = context.Request.Path
                };

                await context.Response.WriteAsJsonAsync(problem);
            }
            /*
            catch (Exception ex)
            {
                //Add logging
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/problem+json";

                var problem = new ProblemDetails
                {
                    Type = "/errors/internal-server-error", //can be more specific
                    Title = "An unexpected error occurred.",
                    Detail = ex.Message,
                    Status = StatusCodes.Status500InternalServerError,
                    Instance = context.Request.Path
                };

                await context.Response.WriteAsJsonAsync(problem);
            }
            */
        }
    }
}
