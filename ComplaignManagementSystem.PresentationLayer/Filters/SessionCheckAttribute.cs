using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ComplaignManagementSystem.Presentation.Filters
{
    public class SessionCheckAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerName = context.ActionDescriptor.RouteValues["controller"];
            var actionName = context.ActionDescriptor.RouteValues["action"];

            // ✅ Skip session check for login and reset actions
            if (controllerName != null &&
                controllerName.Equals("User", StringComparison.OrdinalIgnoreCase) &&
                (actionName.Equals("Login", StringComparison.OrdinalIgnoreCase) ||
                 actionName.Equals("Reset", StringComparison.OrdinalIgnoreCase) ||
                 actionName.Equals("Register", StringComparison.OrdinalIgnoreCase)))
            {
                base.OnActionExecuting(context);
                return;
            }

            var session = context.HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(session))
            {
                context.Result = new RedirectToActionResult("Login", "User", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
