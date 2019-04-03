﻿namespace SFA.DAS.RoATPService.Application.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using Domain;

    public interface IUpdateOrganisationRepository
    {
        Task<string> GetLegalName(Guid organisationId);
        Task<bool> UpdateLegalName(Guid organisationId, string legalName, string updatedBy);
        Task<bool> GetParentCompanyGuarantee(Guid organisationId);
        Task<bool> UpdateParentCompanyGuarantee(Guid organisationId, bool parentCompanyGuarantee, string updatedBy);
        Task<string> GetTradingName(Guid organisationId);
        Task<bool> UpdateTradingName(Guid organisationId, string tradingName, string updatedBy);
        Task<int> GetStatus(Guid organisationId);
        Task<bool> UpdateStatus(Guid organisationId, int organisationStatusId, string updatedBy);
        Task<RemovedReason> GetRemovedReason(Guid organisationId);
        Task<RemovedReason> UpdateStatusWithRemovedReason(Guid organisationId, int organisationStatusId, int removedReasonId, string updatedBy);
        Task<bool> UpdateStartDate(Guid organisationId, DateTime startDate);
    }
}