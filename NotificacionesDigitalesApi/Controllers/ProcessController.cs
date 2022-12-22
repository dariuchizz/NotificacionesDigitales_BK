using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NotificacionesDigitalesApi.Model;
using Processor.HangfireProcess;

namespace NotificacionesDigitalesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProcessController : ControllerBase
    {
        private readonly ILogger<ProcessController> _logger;

        protected readonly IProcessManager _processManager;

        public ProcessController(ILogger<ProcessController> logger, IProcessManager processManager)
        {
            _logger = logger;
            _processManager = processManager;
        }

        [HttpPost]
        [Route("{indentificador}")]
        public async Task<ActionResult> ExecuteJobAsync(string indentificador, [FromBody] string parameters)
        {
            if (_processManager.TryGetProcess(indentificador, out ProcessDto process))
            {
                try
                {
                    var executor = process.ProcessFactory();
                    BackgroundJob.Enqueue(() =>
                        executor.ExecuteAsync(parameters, CancellationToken.None)
                    );
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                return Ok();
            }
            return NotFound();

        }

        /*
          Schedule, add or update a recurrent task 
         */
        [HttpPut]
        [Route("recurrentjobs/{identifier}/{recurrentJobId}")]
        public async Task<ActionResult> ScheduleRecurrentJob(string identifier, string recurrentJobId, [FromBody] CnonDto cnonDto)
        {
            try
            {
                if (_processManager.TryGetProcess(identifier, out ProcessDto process))
                {
                    var executor = process.ProcessFactory();
                    RecurringJob.AddOrUpdate(
                           recurrentJobId,
                           () => executor.ExecuteAsync(cnonDto.Parameters, CancellationToken.None),
                           cnonDto.CronExpression,
                           System.TimeZoneInfo.Local
                      );
                    return Ok();
                }
                return NotFound();
            }
            catch (Exception e) when (e.Message.Contains("CRON expression is invalid"))
            {
                return BadRequest(e.InnerException?.Message);
            }
        }
    }
}
