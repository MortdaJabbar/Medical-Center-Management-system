using System.ComponentModel.DataAnnotations;

namespace MCMSAPI.dtos
{
    public class AddUpdatePrescriptionDto
    { 
            [Required]
            public Guid PatientID { get; set; }

            [Required]
            public Guid DoctorID { get; set; }

            [Required]
            public int MedicationID { get; set; }

            [Required]
            public DateOnly PrescriptionDate { get; set; }

            public int? Refills { get; set; }

            public string? Instructions { get; set; }
        }

    }

