
$(document).ready(function () {
    // فتح المودال وملء البيانات
    $(document).on('click', '.add-invoice-btn', function () {
        const patientId = $(this).data('patient-id');
        const serviceId = $(this).data('service-id');
        const amount = $(this).data('amount');

        $('#addPatientId').val(patientId);
        $('#addServiceId').val(serviceId);
        $('#addTotalAmount').val(amount);

        
        $('#addInvoiceModal').modal('show');

        

    });

    // إرسال البيانات عند الإضافة
    $('#addInvoiceForm').submit(function (e) {
        e.preventDefault();

        const invoiceData = {
            patientId: $('#addPatientId').val(),
            serviceType: 'Test',
            serviceId: $('#addServiceId').val(),
            totalAmount: parseFloat($('#addTotalAmount').val()),
            paymentMethod: $('#addPaymentMethod').val(),
            paymentStatus: $('#addPaymentStatus').val(),
            notes: $('#addNotes').val(),
            cardSessionID: null,
            cardPaymentIntentID: null
        };
         
        $.ajax({
            url: 'https://localhost:7119/api/Invoices',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(invoiceData),
            success: function () {
                
                
        $('#addInvoiceForm')[0].reset();
                $('#addInvoiceModal').modal('hide');
                  alert( 'success .');
                // أعد تحميل الفحوصات غير المدفوعة
                loadUnpaidTests(); // ← تأكد أن هذه الدالة موجودة عندك
            },
            error: function () {
                alert( 'Failed to add invoice.');
            }
        });
    });
});

