
let deletedid = 0;
$(document).on('click', '.delete-invoice-btn', function () {
    deletedid = $(this).data('id');
    $('#deleteInvoiceId').val(deletedid);
    $('#deleteInvoiceModal').modal('show');
  alert(deletedid);
    
});

$('#confirmDeleteBtn').click(function () {
     deletedid = $('#deleteInvoiceId').val();

    $.ajax({
        url: `https://localhost:7119/api/Invoices/${deletedid}`,
        method: 'DELETE',
        success: function () {
            $('#deleteInvoiceModal').modal('hide');
            alert("Invoice deleted successfully!");
            loadAllInvoices();
            
        },
        error: function () {
            alert("Failed to delete invoice.");
        }
    });
});
