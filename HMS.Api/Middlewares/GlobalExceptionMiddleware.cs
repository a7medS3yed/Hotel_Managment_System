using Microsoft.AspNetCore.Mvc;

namespace HMS.Api.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);

                if (context.Response.StatusCode == StatusCodes.Status404NotFound
                    && !context.Response.HasStarted)
                {
                    var problem = new ProblemDetails()
                    {
                        Title = "Error while processing http request - Endpoint not found",
                        Status = StatusCodes.Status404NotFound,
                        Detail = $"Endpoint {context.Request.Path} not found",
                        Instance = context.Request.Path
                    };

                    await context.Response.WriteAsJsonAsync(problem);
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Something went wrong. Please try again later.");


                var problem = new ProblemDetails()
                {
                    Title = "An unexpected error occured",
                    Detail = ex.Message,
                    Instance = context.Request.Path,
                    Status = StatusCodes.Status500InternalServerError
                };

                context.Response.StatusCode = problem.Status.Value;

                await context.Response.WriteAsJsonAsync(problem);
            }
        }
    }

}
