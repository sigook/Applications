using Covenant.Common.Enums;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Accounting;
using Microsoft.Extensions.Logging;

namespace Covenant.Infrastructure.Accounting.Deductions;

public class ProvincialTaxTablesLoader(IDeductionsRepository repository, ILogger<ProvincialTaxTablesLoader> logger)
	: TaxTablesLoader(repository, logger, TaxType.Provincial);
