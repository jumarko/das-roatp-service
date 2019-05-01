using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tests
{
    [TestFixture]
    public class ControllerAuthorizeTests
    {
        private readonly List<string> _controllersThatDoNotRequireAuthorize = new List<string>()
        {
            "PingController"
        };

        private const string RoatpRoleName = "RoATPServiceInternalAPI";

        [Test]
        public void ControllersShouldHaveAuthorizeAttribute()
        {
            var webAssembly = typeof(SearchController).GetTypeInfo().Assembly;

            var controllers = webAssembly.DefinedTypes.Where(c => c.BaseType == typeof(Controller)).ToList();

            foreach (var controller in controllers.Where(c => !_controllersThatDoNotRequireAuthorize.Contains(c.Name)))
            {
                controller.Should().BeDecoratedWith<AuthorizeAttribute>(attr => attr.Roles.Contains(RoatpRoleName));
            }
        }
    }
}