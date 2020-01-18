using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance.ClientModel
{
    public class GenericViewModel<T>
    {
        public T DataModel { get; set; }
        public List<BreadcrumbModel> Breadcrumbs { get; set; }
    }
}
