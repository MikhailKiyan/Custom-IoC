using System;
using System.Collections.Generic;
using System.Text;

namespace CustomIoC.Core
{
	public class InvoiceService
	{
		public InvoiceService(IRepository<Customer> repository, ILogger logger)
		{
		}
	}
}
