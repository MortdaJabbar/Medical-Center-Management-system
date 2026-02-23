using MCMSDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCMSBussinessLogic
{
    public class Invoice : IInvoice
    {
            public async Task<List<InvoiceDetailsDto>> GetAllInvoicesAsync()
            {
                return await GetAllAsync();
            }

            public async Task<bool> InsertInvoiceAsync(AddInvoiceDto dto)
            {
                return await InsertAsync(dto);
            }

            public async Task<bool> UpdateInvoiceAsync(int invoiceId, UpdateInvoiceDto dto)
            {
                return await UpdateAsync(invoiceId, dto);
            }

            public async Task<bool> DeleteInvoiceAsync(int invoiceId)
            {
                return await DeleteAsync(invoiceId);
            }

            public async Task<List<PatientInvoiceDto>> GetInvoicesByPatientIdAsync(Guid patientId)
            {
                return await GetByPatientIdAsync(patientId);
            }

            Task<List<UnpaidServiceDto>> IInvoice.GetUnpaidTestsAsync() => Invoice.GetUnpaidTestsAsync();
            Task<List<UnpaidServiceDto>> IInvoice.GetUnpaidAppointmentsAsync() => Invoice.GetUnpaidAppointmentsAsync();
            Task<List<UnpaidServiceDto>> IInvoice.GetUnpaidPrescriptionsAsync() => Invoice.GetUnpaidPrescriptionsAsync();

       
            // جلب جميع الفواتير بالتفاصيل (مع اسم المريض + وصف الخدمة)
            public async static Task<List<InvoiceDetailsDto>> GetAllAsync()
            {
                var invoiceData = new InvoiceData();
                return await invoiceData.GetAllAsync();
            }

            // إضافة فاتورة جديدة
            public async static Task<bool> InsertAsync(AddInvoiceDto dto)
            {
                var invoiceData = new InvoiceData();
                return await invoiceData.InsertAsync(dto);
            }

            // تعديل فاتورة (TotalAmount + PaymentStatus + Notes)
            public async static Task<bool> UpdateAsync(int invoiceId, UpdateInvoiceDto dto)
            {
                var invoiceData = new InvoiceData();
                return await invoiceData.UpdateAsync(invoiceId, dto);
            }

            // حذف فاتورة
            public async static Task<bool> DeleteAsync(int invoiceId)
            {
                var invoiceData = new InvoiceData();
                return await invoiceData.DeleteAsync(invoiceId);
            }

            // جلب فواتير مريض معيّن (بدون اسم أو صورة)
            public static async Task<List<PatientInvoiceDto>> GetByPatientIdAsync(Guid patientId)
            {
                var invoiceData = new InvoiceData();
                return await invoiceData.GetInvoicesForPatientAsync(patientId);
            }

        public  async static Task<List<UnpaidServiceDto>> GetUnpaidTestsAsync()
        {
            var invoiceData = new InvoiceData();
            return await invoiceData.GetUnpaidTestsAsync();
        }

        public async static Task<List<UnpaidServiceDto>> GetUnpaidAppointmentsAsync()
        {
            var invoiceData = new InvoiceData();
            return await invoiceData.GetUnpaidAppointmentsAsync();
        }

        public  async static Task<List<UnpaidServiceDto>> GetUnpaidPrescriptionsAsync()
        {
            var invoiceData = new InvoiceData();
            return await invoiceData.GetUnpaidPrescriptionsAsync();
        }




    }

    }

