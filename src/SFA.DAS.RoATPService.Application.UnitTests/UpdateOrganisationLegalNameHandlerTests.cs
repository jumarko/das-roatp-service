﻿namespace SFA.DAS.RoATPService.Application.UnitTests
{
    using Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using SFA.DAS.RoATPService.Application.Handlers;
    using SFA.DAS.RoATPService.Domain;
    using System;
    using System.Threading;
    using Api.Types.Models;
    using FluentAssertions;
    using Validators;
    using System.Threading.Tasks;
    using SFA.DAS.RoATPService.Application.Exceptions;

    [TestFixture]
    public class UpdateOrganisationLegalNameHandlerTests
    {
        private Mock<ILogger<UpdateOrganisationLegalNameHandler>> _logger;
        private Mock<IOrganisationValidator> _validator;
        private Mock<IUpdateOrganisationRepository> _repository;
        private Mock<IAuditLogRepository> _auditRepository;
        private UpdateOrganisationLegalNameHandler _handler;

        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<UpdateOrganisationLegalNameHandler>>();
            _validator = new Mock<IOrganisationValidator>();
            _validator.Setup(x => x.IsValidLegalName(It.IsAny<string>())).Returns(true);
            _repository = new Mock<IUpdateOrganisationRepository>();
            _repository.Setup(x => x.GetLegalName(It.IsAny<Guid>())).ReturnsAsync("existing legal name").Verifiable();
            _repository.Setup(x => x.UpdateLegalName(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true).Verifiable();
            _auditRepository = new Mock<IAuditLogRepository>();
            _auditRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>())).ReturnsAsync(true).Verifiable();

            _handler = new UpdateOrganisationLegalNameHandler(_logger.Object, _validator.Object, _repository.Object, _auditRepository.Object);
        }

        [Test]
        public void Handler_does_not_update_database_if_legal_name_invalid()
        {
            _validator.Setup(x => x.IsValidLegalName(It.IsAny<string>())).Returns(false);

            var request = new UpdateOrganisationLegalNameRequest
            {
                LegalName = "legal name %%%%", OrganisationId = Guid.NewGuid(), UpdatedBy = "unit test"
            };

            Func<Task> result = async () => await
                _handler.Handle(request, new CancellationToken());
            result.Should().Throw<BadRequestException>();

            _repository.Verify(x => x.GetLegalName(It.IsAny<Guid>()), Times.Never);
            _repository.Verify(x => x.UpdateLegalName(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _auditRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
        }

        [Test]
        public void Handler_does_not_update_database_if_legal_name_unchanged()
        {
            var request = new UpdateOrganisationLegalNameRequest
            {
                LegalName = "existing legal name",
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };

            var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
            result.Should().BeFalse(); 

            _repository.Verify(x => x.GetLegalName(It.IsAny<Guid>()), Times.Once);
            _repository.Verify(x => x.UpdateLegalName(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _auditRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
        }

        [Test]
        public void Handler_does_not_write_audit_log_entry_if_save_operation_fails()
        {
            _repository.Setup(x => x.UpdateLegalName(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false).Verifiable();
            
            var request = new UpdateOrganisationLegalNameRequest
            {
                LegalName = "new legal name",
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };

            var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
            result.Should().BeFalse();

            _repository.Verify(x => x.GetLegalName(It.IsAny<Guid>()), Times.Once);
            _repository.Verify(x => x.UpdateLegalName(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _auditRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
        }
        
        [Test]
        public void Handler_writes_updated_legal_name_and_audit_log_entry_to_database()
        {
            var request = new UpdateOrganisationLegalNameRequest
            {
                LegalName = "new legal name",
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };

            var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
            result.Should().BeTrue();

            _repository.Verify(x => x.GetLegalName(It.IsAny<Guid>()), Times.Once);
            _repository.Verify(x => x.UpdateLegalName(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _auditRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Once);
        }
    }
}