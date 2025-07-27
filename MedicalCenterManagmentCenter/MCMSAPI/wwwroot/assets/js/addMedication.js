$('#addMedicationForm').submit(function (e) {
    e.preventDefault();

    const newMedication = {
        name: $('#medName').val(),
        description: $('#medDescription').val(),
        strength: $('#medStrength').val(),
        dosageForm: $('#medDosageForm').val()
    };

    $.ajax({
        url: 'https://localhost:7119/api/Medications/add',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(newMedication),
        success: function () {
            $('#addMedicationModal').modal('hide');
            $('#addMedicationForm')[0].reset();
            showSuccessMessage("Medication Added Successffuly");
            loadMedications(); // تحديث الجدول
        },
        error: function () {
            showErrorMessage('Failed to add medication.');
        }
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
