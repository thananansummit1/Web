using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Web.Util
{
    public class ActionableAttribute : Attribute
    {
        protected bool _requireFilter = true;

        public bool requireFilter
        {
            get
            {
                return this._requireFilter;
            }
        }

        virtual public void OnActionExecuting(ActionExecutingContext context)
        {
            // Validate
        }

        virtual public void OnActionExecuted(ActionExecutedContext context)
        {
            // Extend
        }
    }
}
