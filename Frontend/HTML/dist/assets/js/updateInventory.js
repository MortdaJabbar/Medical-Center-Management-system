let myid = 0;
$(document).ready(function () {
    $(document).on('click', '.edit-inv-btn', function () {
        const id = $(this).data('id');
        myid=id;

        $.get(`https://localhost:7119/api/Inventory/${id}`, function (inv) {
            $('#editInventoryId').val(inv.inventoryID);
            $('#editMedicationSelect').val(inv.medicationID);
            $('#editQuantity').val(inv.quantity);
            $('#editUnit').val(inv.unitOfMeasure);
            $('#editSupplier').val(inv.supplier);
            $('#editExpiry').val(inv.expiryDate);
            $('#editReceived').val(inv.recivedDate);
            $('#editInventoryModal').modal('show');
        });
    });

    $('#editInventoryForm').submit(function (e) {
        e.preventDefault();

        const updatedInventory = {
            medicationID: $('#editMedicationSelect').val(),
            quantity: $('#editQuantity').val(),
            unitOfMeasure: $('#editUnit').val(),
            supplier: $('#editSupplier').val(),
            expiryDate: $('#editExpiry').val(),
            recivedDate: $('#editReceived').val()
        };

        $.ajax({
            url: `https://localhost:7119/api/Inventory/update/${myid}`,
            method: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(updatedInventory),
            success: function () {
                $('#editInventoryModal').modal('hide');
                showErrorMessage('  update inventory Done Successfully.');
                loadInventory();
            },
            error: function () {
                showErrorMessage('Failed to update inventory.');
            }
        });
    });
});




function showErrorMessage(message) {
    // Remove any existing alert first
    $('#errorAlert').remove();

    // Create and append the alert
    $('body').append(`
        <div id="errorAlert" class="alert alert-danger alert-dismissible fade show position-fixed top-0 end-0 m-4" role="alert" style="z-index: 9999; min-width: 300px;">
            <i class="uil uil-exclamation-circle"></i> <strong>Error:</strong> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `);

    window.scrollTo({ top: 0, behavior: 'smooth' });

    // Auto close after 5 seconds
    setTimeout(() => { 
        $('#errorAlert').alert('close'); 
    }, 5000);
}
function showSuccessMessage(message) {
    // Remove any existing alert first
    $('#successAlert').remove();

    // Create and append the alert
    $('body').append(`
        <div id="successAlert" class="alert alert-success alert-dismissible fade show position-fixed top-0 end-0 m-4" role="alert" style="z-index: 9999; min-width: 600px;">
            <i class="uil uil-check-circle"></i> <strong>Success:</strong> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `);
    window.scrollTo({ top: 0, behavior: 'smooth' });


    // Auto close after 3 seconds
    setTimeout(() => { 
        $('#successAlert').alert('close'); 
    }, 5000);
}
