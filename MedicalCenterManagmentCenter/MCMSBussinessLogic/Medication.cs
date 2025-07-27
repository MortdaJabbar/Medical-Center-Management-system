using MCMSDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MCMSBussinessLogic
{
    public class Medication
    {
        public int MedicationID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Strength { get; set; }
        public string DosageForm { get; set; }

        [JsonIgnore]
        public MedicationDto DTO => new MedicationDto
        {
            MedicationID = MedicationID,
            Name = Name,
            Description = Description,
            Strength = Strength,
            DosageForm = DosageForm
        };

        public Medication() { }

        public Medication(MedicationDto dto)
        {
            MedicationID = dto.MedicationID;
            Name = dto.Name;
            Description = dto.Description;
            Strength = dto.Strength;
            DosageForm = dto.DosageForm;
        }

        public async Task<bool> AddNewMedicationAsync()
        {
            return await MedicationData.InsertMedicationAsync(this.DTO);
        }

        public async Task<bool> UpdateMedicationAsync()
        {
            return await MedicationData.UpdateMedicationAsync(this.DTO);
        }

        public static async Task<bool> DeleteMedicationAsync(int id)
        {
            return await MedicationData.DeleteMedicationAsync(id);
        }

        public static async Task<Medication?> FindByIdAsync(int id)
        {
            var dto = await MedicationData.GetMedicationByIdAsync(id);
            return dto != null ? new Medication(dto) : null;
        }

        public static async Task<List<Medication>> GetAllAsync()
        {
            var dtos = await MedicationData.GetAllMedicationsAsync( );
            return dtos.Select(dto => new Medication(dto)).ToList();
        }
    }

}
