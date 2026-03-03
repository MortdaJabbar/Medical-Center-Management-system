using MCMSBLL;
using MCMSBussinessLogic;
using MCMSBussinessLogic.Configuration;
using MCMSBussinessLogic.Interfaces;
using MCMSBussinessLogic.Services;
using MCMSDAL;
using MCMSDAL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MCMSAPI.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services)
        {
            services.AddScoped<IAppointmentData, AppointmentData>();
            services.AddScoped<IDoctorData, DoctorData>();
            services.AddScoped<IPersonData, PersonData>();
            services.AddScoped<IPatientData, PatientData>();
            services.AddScoped<IPharmacistData, PharmacistData>();
            services.AddScoped<IStaffData, StaffData>();
            services.AddScoped<IInventoryData, InventoryData>();
            services.AddScoped<IMedicationData, MedicationData>();
            services.AddScoped<IPrescriptionData, PrescriptionData>();
            services.AddScoped<ITestData, TestData>();
            services.AddScoped<ITestTypeData, TestTypeData>();
            services.AddScoped<IInvoiceData, InvoiceData>();
            services.AddScoped<IUserAccountData, UserAccountData>();
            services.AddScoped<IServicePaymentData, ServicePaymentData>();
            services.AddScoped<ITwoFactorCodeData, TwoFactorCodeData>();
            services.AddScoped<IRefreshTokenData, RefreshTokenData>();

            return services;
        }

        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IPharmacistService, PharmacistService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IMedicationService, MedicationService>();
            services.AddScoped<IPrescriptionService, PrescriptionService>();
            services.AddScoped<ITestService, TestService>();
            services.AddScoped<ITestTypeService, TestTypeService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IUserAccountService, UserAccountService>();

            return services;
        }

        public static IServiceCollection AddCrossCuttingServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration.GetSection("Stripe").Get<StripeSettings>() ?? new StripeSettings());
            services.AddSingleton(configuration.GetSection("Email:Smtp").Get<SmtpSettings>() ?? new SmtpSettings());

            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IEmailSenderService, EmailSenderService>();

            // Existing BLL interfaces
            services.AddScoped<IServicePayment, ServicePayment>();

            // Stripe client is stateless and can be singleton.
            services.AddSingleton<IStripeService, StripeService>();

            return services;
        }
    }
}
