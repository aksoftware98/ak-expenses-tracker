using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AKExpensesTracker.Shared.Responses;

public class ApiResponse
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public DateTime ResponseDate { get; set; }
    
}
