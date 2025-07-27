let updateId = 0;

$(document).on('click', '.edit-invoice-btn', function () {
    updateId = $(this).data('id');
    const total = $(this).data('total');
    const status = $(this).data('status');
    const notes = $(this).data('notes');

    $('#editInvoiceId').val(updateId);
    $('#editTotalAmount').val(total);
    $('#editPaymentStatus').val(status);
    $('#editNotes').val(notes);

    $('#editInvoiceModal').modal('show');
});

$('#editInvoiceForm').submit(function (e) {
    e.preventDefault();

    updateId = $('#editInvoiceId').val();
    const updatedInvoice = {
        totalAmount: parseFloat($('#editTotalAmount').val()),
        paymentStatus: $('#editPaymentStatus').val(),
        notes: $('#editNotes').val()
    };

    $.ajax({
        url: `https://localhost:7119/api/Invoices/${updateId}`,
        method: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify(updatedInvoice),
        success: function () {
            $('#editInvoiceModal').modal('hide');
            loadAllInvoices();
            alert("Invoice updated successfully!");
        },
        error: function () {
            alert("Failed to update invoice.");
        }
    });
});
