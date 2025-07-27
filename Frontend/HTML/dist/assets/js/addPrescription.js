
$(document).ready(function () {
    $('#addPrescriptionForm').submit(function (e) {
        e.preventDefault(); // يمنع إعادة تحميل الصفحة

        const prescription = {
            patientId: $('#addPrescriptionPatientId').val(),
            doctorId: $('#addPrescriptionDoctorId').val(),
            medicationId: $('#addMedicationSelect').val(),
            prescriptionDate: $('#addPrescriptionDate').val(),
            refills: $('#addPrescriptionRefills').val() || null,
            instructions: $('#addPrescriptionInstructions').val()
        };

        $.ajax({
            url: 'https://localhost:7119/api/Prescriptions/add',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(prescription),
            success: function () {
                $('#addPrescriptionModal').modal('hide');
                $('#addPrescriptionForm')[0].reset();
                loadPrescriptions(); // إعادة تحميل الجدول
                showSuccessMessage('Prescription added successfully.');
            },
            error: function () {
                showErrorMessage('❌ Failed to add prescription.');
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
