namespace SFA.DAS.RoATPService.Application.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Domain;
    using Exceptions;
    using FluentAssertions;
    using Handlers;
    using Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using Validators;

    [TestFixture]
    public class OrganisationSearchHandlerTests
    {
        private Mock<IOrganisationSearchRepository> _repository;
        private Mock<ILogger<OrganisationSearchHandler>> _logger;
        private Mock<IOrganisationSearchValidator> _validator;
        private OrganisationSearchHandler _organisationSearchHandler;

        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<OrganisationSearchHandler>>();
            _repository = new Mock<IOrganisationSearchRepository>();
            _validator = new Mock<IOrganisationSearchValidator>();
            _validator.Setup(x => x.IsValidSearchTerm(It.IsAny<string>())).Returns(true);
            _organisationSearchHandler =
                new OrganisationSearchHandler(_repository.Object, _logger.Object, _validator.Object);
        }

        [Test]
        public void Organisation_search_by_UKPRN()
        {
            _validator.Setup(x => x.IsValidUKPRN(It.IsAny<string>())).Returns(true);
            var organisations = new List<Organisation>
            {
                new Organisation {UKPRN = 10001234}
            };
            _repository.Setup(x => x.OrganisationSearchByUkPrn(It.IsAny<string>())).ReturnsAsync(organisations);
            _repository.Setup(x => x.OrganisationSearchByName(It.IsAny<string>()));
            var organisationSearchRequest = new OrganisationSearchRequest {SearchTerm = "10001234"};
            Task<IEnumerable<Organisation>> searchResults =
                _organisationSearchHandler.Handle(organisationSearchRequest, new CancellationToken());

            searchResults.Result.Count().Should().Be(1);

            _repository.Verify(x => x.OrganisationSearchByUkPrn(It.IsAny<string>()), Times.Once);
            _repository.Verify(x => x.OrganisationSearchByName(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void Organisation_search_by_company_name()
        {
            _validator.Setup(x => x.IsValidUKPRN(It.IsAny<string>())).Returns(false);
            var organisations = new List<Organisation>
            {
                new Organisation {UKPRN = 10001234, LegalName = "TEST PROVIDER"}
            };
            _repository.Setup(x => x.OrganisationSearchByName(It.IsAny<string>())).ReturnsAsync(organisations);
            _repository.Setup(x => x.OrganisationSearchByUkPrn(It.IsAny<string>()));

            var organisationSearchRequest = new OrganisationSearchRequest {SearchTerm = "TEST"};
            Task<IEnumerable<Organisation>> searchResults =
                _organisationSearchHandler.Handle(organisationSearchRequest, new CancellationToken());

            searchResults.Result.Count().Should().Be(1);

            _repository.Verify(x => x.OrganisationSearchByName(It.IsAny<string>()), Times.Once);
            _repository.Verify(x => x.OrganisationSearchByUkPrn(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void Organisation_search_with_invalid_search_term()
        {
            _validator.Setup(x => x.IsValidSearchTerm(It.IsAny<string>())).Returns(false);

            var organisationSearchRequest = new OrganisationSearchRequest {SearchTerm = "10001234"};
            Func<Task> result = async () => await
                _organisationSearchHandler.Handle(organisationSearchRequest, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [Test]
        public void Organisation_search_by_company_name_with_no_results_found()
        {
            _validator.Setup(x => x.IsValidUKPRN(It.IsAny<string>())).Returns(false);
            var organisations = new List<Organisation>();
            _repository.Setup(x => x.OrganisationSearchByName(It.IsAny<string>())).ReturnsAsync(organisations);
            _repository.Setup(x => x.OrganisationSearchByUkPrn(It.IsAny<string>()));

            var organisationSearchRequest = new OrganisationSearchRequest { SearchTerm = "10001234" };
            Task<IEnumerable<Organisation>> searchResults =
                _organisationSearchHandler.Handle(organisationSearchRequest, new CancellationToken());

            searchResults.Result.Should().BeEmpty();

            _repository.Verify(x => x.OrganisationSearchByName(It.IsAny<string>()), Times.Once);
            _repository.Verify(x => x.OrganisationSearchByUkPrn(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void Organisation_search_by_company_name_with_multiple_results_found()
        {
            _validator.Setup(x => x.IsValidUKPRN(It.IsAny<string>())).Returns(false);
            var organisations = new List<Organisation>
            {
                new Organisation {UKPRN = 10001234, LegalName = "TEST PROVIDER"},
                new Organisation {UKPRN = 10002222, LegalName = "TESTING SERVICES"}
            };
            _repository.Setup(x => x.OrganisationSearchByName(It.IsAny<string>())).ReturnsAsync(organisations);
            _repository.Setup(x => x.OrganisationSearchByUkPrn(It.IsAny<string>()));

            var organisationSearchRequest = new OrganisationSearchRequest { SearchTerm = "TEST" };
            Task<IEnumerable<Organisation>> searchResults =
                _organisationSearchHandler.Handle(organisationSearchRequest, new CancellationToken());

            searchResults.Result.Count().Should().Be(2);

            _repository.Verify(x => x.OrganisationSearchByName(It.IsAny<string>()), Times.Once);
            _repository.Verify(x => x.OrganisationSearchByUkPrn(It.IsAny<string>()), Times.Never);
        }
    }
}

