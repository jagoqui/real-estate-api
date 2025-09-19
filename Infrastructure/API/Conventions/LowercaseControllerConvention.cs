using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace RealEstate.Infrastructure.API.Conventions
{
    /// <summary>
    /// An <see cref="IControllerModelConvention"/> implementation that enforces lowercase controller names
    /// in ASP.NET Core MVC routing.
    /// </summary>
    /// <remarks>
    /// This convention modifies the <c>ControllerName</c> property of the <see cref="ControllerModel"/>
    /// to be lowercase, which can help ensure consistent and case-insensitive routing URLs.
    /// </remarks>
    public class LowercaseControllerConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            controller.ControllerName = controller.ControllerName.ToLowerInvariant();
        }
    }
}