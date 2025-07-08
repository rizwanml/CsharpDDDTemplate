using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Serilog;
using SmallService.API.Options;
using SmallService.Domain.ErrorResponses;

namespace SmallService.API.Filters;

internal sealed class CircuitBreakerResponseFilter : IAsyncActionFilter
{
    private readonly AsyncCircuitBreakerPolicy<ObjectResult> _circuitBreakerPolicy;
    private readonly IOptions<CircuitBreakerResponseOptions> _options;

    public CircuitBreakerResponseFilter(IOptions<CircuitBreakerResponseOptions> options)
    {
        _options = options;

        //Configure the circuit breaker policy
        //This policy specifically monitors for HTTP 500 responses from the API
        //See: https://github.com/App-vNext/Polly/wiki/Advanced-Circuit-Breaker
        _circuitBreakerPolicy = Policy.HandleResult<ObjectResult>(r => r != null && r.StatusCode == StatusCodes.Status500InternalServerError)
            .AdvancedCircuitBreakerAsync<ObjectResult>(
                failureThreshold: _options.Value.FailureThreshold, // Break if the failure rate is above %. A double between 0 and 1.
                samplingDuration: TimeSpan.FromSeconds(_options.Value.SamplingDuration), // Duration to measure the failure rate
                minimumThroughput: _options.Value.MinimumThroughput, // Minimum number of requests within the sampling duration
                durationOfBreak: TimeSpan.FromSeconds(_options.Value.DurationOfBreak), // Duration for the circuit breaker to stay open, before checking half open state
                onBreak: OnCircuitBreakerOpen, // Action to perform when circuit breaker opens
                onReset: OnCircuitBreakerReset, // Action to perform when circuit breaker resets
                onHalfOpen: OnCircuitBreakerHalfOpen // Action to perform when the circuit breaker transitions to half-open
            );
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext actionExecutingContext, ActionExecutionDelegate next)
    {
        var result = await next();

        try
        {
            await _circuitBreakerPolicy.ExecuteAsync(async sc => await GetObjectResult(result), CancellationToken.None);
        }
        catch (BrokenCircuitException ex)
        {
            // Handle circuit breaker open state
            if (result.Result is ObjectResult objectResult && objectResult != null)
            {
                objectResult.StatusCode = StatusCodes.Status503ServiceUnavailable;

                if(objectResult.Value is SystemErrorResponse systemErrorResponse && systemErrorResponse.ErrorMessage != null)
                {
                    systemErrorResponse.ErrorMessage = "The API response monitor has exceeded the failure threshold";
                }
            }
        }
    }

    private async Task<ObjectResult> GetObjectResult(ActionExecutedContext actionExecutedContext)
    {
        return (ObjectResult)actionExecutedContext.Result;
    }

    private void OnCircuitBreakerOpen(DelegateResult<ObjectResult> result, TimeSpan timeSpan)
    {
        // Perform any actions when the circuit breaker opens (e.g., logging, notifications)
        Log.ForContext("ClassType", nameof(CircuitBreakerResponseFilter))
            .ForContext("Message", "The API response monitor has exceeded the failure threshold for HTTP 500 errors")
            .Fatal("Circuit breaker open");
    }

    private void OnCircuitBreakerReset()
    {
        // Perform any actions when the circuit breaker resets (e.g., logging, notifications)
        Log.ForContext("ClassType", nameof(CircuitBreakerResponseFilter))
            .ForContext("Message", "The API response monitor has opened for all requests")
            .Information("Circuit breaker reset");
    }

    private void OnCircuitBreakerHalfOpen()
    {
        // Perform any actions when the circuit breaker transitions to half-open (e.g., logging, notifications)
        Log.ForContext("ClassType", nameof(CircuitBreakerResponseFilter))
            .ForContext("Message", "The API response monitor has opened for a limited number of requests")
            .Information("Circuit breaker half-open");
    }
}