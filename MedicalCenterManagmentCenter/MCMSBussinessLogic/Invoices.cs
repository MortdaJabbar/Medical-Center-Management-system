using MCMSDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCMSBussinessLogic
{
    public class Invoice
    {
       
            // جلب جميع الفواتير بالتفاصيل (مع اسم المريض + وصف الخدمة)
            public async static Task<List<InvoiceDetailsDto>> GetAllAsync()
            {
                return await InvoiceData.GetAllAsync();
            }

            // إضافة فاتورة جديدة
            public async static Task<bool> InsertAsync(AddInvoiceDto dto)
            {
                return await InvoiceData.InsertAsync(dto);
            }

            // تعديل فاتورة (TotalAmount + PaymentStatus + Notes)
            public async static Task<bool> UpdateAsync(int invoiceId, UpdateInvoiceDto dto)
            {
                return await InvoiceData.UpdateAsync(invoiceId, dto);
            }

            // حذف فاتورة
            public async static Task<bool> DeleteAsync(int invoiceId)
            {
                return await InvoiceData.DeleteAsync(invoiceId);
            }

            // جلب فواتير مريض معيّن (بدون اسم أو صورة)
            public static async Task<List<PatientInvoiceDto>> GetByPatientIdAsync(Guid patientId)
            {
                return await InvoiceData.GetInvoicesForPatientAsync(patientId);
            }

        public  async static Task<List<UnpaidServiceDto>> GetUnpaidTestsAsync() => await InvoiceData.GetUnpaidTestsAsync();
        public async static Task<List<UnpaidServiceDto>> GetUnpaidAppointmentsAsync() => await InvoiceData.GetUnpaidAppointmentsAsync();
        public  async static Task<List<UnpaidServiceDto>> GetUnpaidPrescriptionsAsync() => await InvoiceData.GetUnpaidPrescriptionsAsync();




    }

    }

