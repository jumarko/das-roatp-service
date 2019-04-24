﻿namespace SFA.DAS.RoATPService.Application.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using Domain;
    using Commands;

    public interface IUpdateOrganisationRepository
    {
    

        Task<Guid?> CreateOrganisation(CreateOrganisationCommand command);

      
        Task<bool> UpdateLegalName(Guid organisationId, string legalName, string updatedBy);

        Task<bool> UpdateFinancialTrackRecord(Guid organisationId, bool financialTrackRecord, string updatedBy);

        Task<bool> UpdateUkprn(Guid organisationId, long ukprn, string updatedBy);

        Task<bool> UpdateParentCompanyGuarantee(Guid organisationId, bool parentCompanyGuarantee, string updatedBy);

        Task<bool> UpdateTradingName(Guid organisationId, string tradingName, string updatedBy);

        Task<bool> UpdateOrganisationStatus(Guid organisationId, int organisationStatusId, string updatedBy);
        Task<RemovedReason> UpdateStatusWithRemovedReason(Guid organisationId, int organisationStatusId, int removedReasonId, string updatedBy);

        Task<bool> UpdateOrganisationType(Guid organisationId, int organisationTypeId, string updatedBy);

        Task<bool> UpdateStartDate(Guid organisationId, DateTime startDate);
    
        Task<bool> UpdateProviderTypeAndOrganisationType(Guid organisationId, int providerTypeId, int organisationTypeId, string updatedBy);

        Task<bool> WriteFieldChangesToAuditLog(AuditData auditFieldChanges);
    }
}
