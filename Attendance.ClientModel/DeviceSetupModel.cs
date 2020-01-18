namespace Attendance.ClientModel
{
    using System;
    using System.Collections.Generic;

    public partial class DeviceSetupModel
    {
        public int Id { get; set; }

        public int? SchoolId { get; set; }
        public int DivisionId { get; set; }
        public int DistrictId { get; set; }
        public int UpazilaId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? TypeId { get; set; }

        public string Location { get; set; }

        public string Gate { get; set; }

        public string IPAddress { get; set; }

        public string Port { get; set; }

        public bool? IsActive { get; set; }

        public string DeviceTypeName { get; set; }

        public string SchoolName { get; set; }
        public string GeoName { get; set; }
    }
}
