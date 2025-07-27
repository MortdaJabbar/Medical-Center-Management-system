$(document).ready(function () {
    $(document).on('click', '.delete-inv-btn', function () {
        const id = $(this).data('id');
        $('#deleteInventoryId').val(id);
        $('#deleteInventoryModal').modal('show');
    });

    $('#confirmDeleteInventoryBtn').click(function () {
        const id = $('#deleteInventoryId').val();

        $.ajax({
            url: `https://localhost:7119/api/Inventory/${id}`,
            method: 'DELETE',
            success: function () {
                $('#deleteInventoryModal').modal('hide');
                showSuccessMessage('  inventory deleted Successfully.');
                loadInventory();
            },
            error: function () {
                showErrorMessage('Failed to delete inventory.');
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
