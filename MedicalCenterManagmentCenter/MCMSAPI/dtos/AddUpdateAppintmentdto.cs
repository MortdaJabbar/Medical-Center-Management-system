namespace MCMSAPI.dtos
{
    public class AddUpdateAppintmentdto
    {
        public Guid PatientID { get; set; }
        public Guid DoctorID { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public  string AppointmentTime { get; set; }
        public bool Paid { get; set; }
        public string Reason { get; set; } = string.Empty;
        public int Status { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateAppintmentdto
    {
        
        public DateOnly AppointmentDate { get; set; }
        public string AppointmentTime { get; set; }
        public bool Paid { get; set; }
        public string Reason { get; set; } = string.Empty;
        public int Status { get; set; }
        public string? Notes { get; set; }
    }
}
