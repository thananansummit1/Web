using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Web.Util
{
    public class AuthorizationAttribute : ActionableAttribute
    {
        [Flags]
        public enum AuthorizationGroupType
        {
            None = 0,
            Admin = 1,
            UserWeb = 2,
            AmindOrUserWeb = Admin | UserWeb
        };

        public AuthorizationGroupType authorizationGroupType = AuthorizationGroupType.None;

        public AuthorizationAttribute(AuthorizationGroupType authorizationType)
        {
            this.authorizationGroupType = authorizationType;
        }

        override public void OnActionExecuting(ActionExecutingContext context)
        {
            // Validate

            if (this.authorizationGroupType != AuthorizationGroupType.None)
            {
                try
                {
                    JWT.ValidateToken(context.HttpContext, authorizationGroupType);
                }
                catch (Exception e)
                {

                    throw new AttributeException(e.Message, HttpStatusCode.Unauthorized)
                    {
                        actionResult = new UnauthorizedResult()
                    };
                }
            }
        }

        override public void OnActionExecuted(ActionExecutedContext context)
        {
            // Extend
            //if (this.authorizationType != AuthorizationType.None && context.HttpContext.Items.ContainsKey(HttpContextItemKey.UserInfoKey))
            //{
            //    JWT.ExtendToken(context.HttpContext, (User)context.HttpContext.Items[HttpContextItemKey.UserInfoKey]);
            //}
        }
    }
}
