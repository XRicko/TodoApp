using System.Diagnostics;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ToDoList.Diagnostic
{
    public class ProccesTimeActionFilterAttribute : ActionFilterAttribute
    {
        private readonly ProccessTimeCounterSource proccessTimeCounterSource;
        private readonly Stopwatch stopwatch = new();

        public ProccesTimeActionFilterAttribute(ProccessTimeCounterSource timeCounterSource)
        {
            proccessTimeCounterSource = timeCounterSource;
        }

        public override void OnActionExecuting(ActionExecutingContext context) => stopwatch.Start();
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            stopwatch.Stop();
            proccessTimeCounterSource.Request(context.HttpContext.Request.GetDisplayUrl(), stopwatch.ElapsedMilliseconds);
        }
    }
}
