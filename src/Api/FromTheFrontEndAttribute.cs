using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api;

// The session cookie is SameSite=Lax, so any other developkerr.com subdomain counts as the same
// site and can make a browser send it on a POST. Only the front end's own origin may.
[AttributeUsage(AttributeTargets.Method)]
public class FromTheFrontEndAttribute : Attribute, IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var frontEnd = context.HttpContext.RequestServices.GetRequiredService<FrontEnd>();
        if (context.HttpContext.Request.Headers.Origin != frontEnd.Origin)
        {
            context.Result = new ObjectResult(new ErrorResponse("Use Pre/Post's own site to do that.")) { StatusCode = StatusCodes.Status403Forbidden };
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
