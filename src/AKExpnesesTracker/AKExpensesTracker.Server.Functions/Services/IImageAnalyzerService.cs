using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AKExpensesTracker.Server.Functions.Services
{
	public interface IImageAnalyzer
	{

		Task<IEnumerable<string>> ExtractImageCategoriesAsync(Stream imageStream);

	}
}
