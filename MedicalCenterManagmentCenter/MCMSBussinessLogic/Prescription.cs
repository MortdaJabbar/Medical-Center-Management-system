using MCMSDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCMSBussinessLogic
{
    public class Prescription : IPrescription
    {
        private static readonly PrescriptionData _prescriptionData = new();

        public PrescriptionDto DTO => new()
        {
            PrescriptionID = PrescriptionID,
            PatientID = PatientID,
            DoctorID = DoctorID,
            MedicationID = MedicationID,
            PrescriptionDate = PrescriptionDate,
            Refills = Refills,
            Instructions = Instructions
        };
        public int PrescriptionID { get; set; }
        public Guid PatientID { get; set; }
        public Guid DoctorID { get; set; }
        public int MedicationID { get; set; }
        public DateOnly PrescriptionDate { get; set; }
        public int? Refills { get; set; }
        public string? Instructions { get; set; }

        public Prescription() { }

        public Prescription(PrescriptionDto dto)
        {
            PrescriptionID = dto.PrescriptionID;
            PatientID = dto.PatientID;
            DoctorID = dto.DoctorID;
            MedicationID = dto.MedicationID;
            PrescriptionDate = dto.PrescriptionDate;
            Refills = dto.Refills;
            Instructions = dto.Instructions;
        }

        public async Task<bool> AddNewPrescriptionAsync()
        {
            int id = await _prescriptionData.CreatePrescriptionAsync(DTO);
            this.PrescriptionID = id;
            return id > 0;
        }

        public async Task<bool> UpdatePrescriptionAsync()
        {
            return await _prescriptionData.UpdatePrescriptionAsync(DTO);
        }

        public static async Task<Prescription?> FindByIdAsync(int id)
        {
            var dto = await _prescriptionData.GetPrescriptionByIdAsync(id);
            return dto != null ? new Prescription(dto) : null;
        }

        public static async Task<List<Prescription>> GetAllAsync()
        {
            var list = await _prescriptionData.GetAllPrescriptionsAsync();
            return list.Select(dto => new Prescription(dto)).ToList();
        }

        public static async Task<List<Prescription>> GetPagedAsync(int page, int size)
        {
            var list = await _prescriptionData.GetPagedPrescriptionsAsync(page, size);
            return list.Select(dto => new Prescription(dto)).ToList();
        }
        public static  async Task<List<PrescriptionDetailsDto>> GetAllPrescriptionsWithNamesAsync()
        {
            return await _prescriptionData.GetAllWithNamesAsync();
        }
        public static async Task<bool> DeleteByIdAsync(int id)
        {
            return await _prescriptionData.DeletePrescriptionAsync(id);
        }

        public static async Task<List<PrescriptionByDoctorDto>> GetPrescriptionsByDoctorIdAsync(Guid doctorId)
        {
            return await _prescriptionData.GetPrescriptionsByDoctorIdAsync(doctorId);
        }
    }

}
