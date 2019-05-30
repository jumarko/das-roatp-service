﻿CREATE PROCEDURE [dbo].[RoATP_CSV_SUMMARY]
AS

SET NOCOUNT ON

select ukprn AS UKPRN, 
 LegalName + 
	 case isnull(tradingName,'') when '' then ''
	 else ' T/A ' + tradingName
	 END AS 'Organisation Name',
 pt.ProviderType AS 'Provider type',
 CASE Json_value(OrganisationData,'$.NonLevyContract')
	WHEN 'true' then 'Y' else 'N' end  AS 'Contracted to deliver to non-levied employers',
 case Json_value(OrganisationData,'$.ParentCompanyGuarantee')
	WHEN 'true' then 'Y' else 'N' end AS 'Parent company guarantee',
 case Json_value(OrganisationData,'$.FinancialTrackRecord')
	WHEN 'true' then 'N' else 'Y' end AS 'New Organisation without financial track record',
 convert(varchar(10),convert(date,Json_value(OrganisationData,'$.StartDate')), 111) AS  'Start Date',
 Case StatusId WHEN 0 then convert(varchar(10),StatusDate,111) else NULL END AS 'End Date',
 CASE StatusId WHEN 2 THEN convert(varchar(10),StatusDate,111) ELSE NULL END AS 'Provider not currently starting new apprentices',
 convert(varchar(10),convert(date,Json_value(OrganisationData,'$.RefreshDate')), 111) AS  'Refresh Date'
 from organisations o 
 left outer join providerTypes pt on o.ProviderTypeId = pt.Id
	 where o.StatusId in (0,1,2) -- exclude on-boarding
	  order by COALESCE(o.UpdatedAt, o.CreatedAt) desc