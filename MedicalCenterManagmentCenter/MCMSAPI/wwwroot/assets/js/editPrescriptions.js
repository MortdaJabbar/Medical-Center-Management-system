
function editPrescription(button) {
    const id = $(button).data('id');

    // ابحث عن البيانات داخل الصف (من جدول موجود سابقًا)
    const row = $(button).closest('tr');

    const patientName = row.find('td:eq(0)').text().trim();
    const doctorName = row.find('td:eq(1)').text().trim();
    const medicationId = row.find('td:eq(2)').text().trim();
    const prescriptionDate = row.find('td:eq(3)').text().trim();
    const refills = row.find('td:eq(4)').text().trim();
    const instructions = row.find('td:eq(5)').text().trim();

    // تعبئة المودال
    $('#editPrescriptionId').val(id);
    $('#editPrescriptionPatientId').empty().append(`<option selected>${patientName}</option>`);
    $('#editPrescriptionDoctorId').empty().append(`<option selected>${doctorName}</option>`);

    // تحميل قائمة الأدوية
    $.get("https://localhost:7119/api/Medications/all", function (meds) {
        const medSelect = $("#editMedicationSelect");
        medSelect.empty().append('<option disabled>Select Medication</option>');
        meds.forEach(med => {
            const selected = med.medicationID == medicationId ? 'selected' : '';
            medSelect.append(`<option value="${med.medicationID}" ${selected}>${med.name}</option>`);
        });
    });

    $('#editPrescriptionDate').val(new Date(prescriptionDate).toISOString().split('T')[0]);
    $('#editPrescriptionRefills').val(refills !== "-" ? refills : "");
    $('#editPrescriptionInstructions').val(instructions);

    $('#editPrescriptionModal').modal('show');
}
let id =0;
// submit form
$(document).ready(function () {
    $('#editPrescriptionForm').submit(function (e) {
        e.preventDefault();

         id = $('#editPrescriptionId').val();

        const prescription = {
            
            medicationId: $('#editMedicationSelect').val(),
            prescriptionDate: $('#editPrescriptionDate').val(),
            refills: $('#editPrescriptionRefills').val() || null,
            instructions: $('#editPrescriptionInstructions').val()
        };

        $.ajax({
            url: `https://localhost:7119/api/Prescriptions/update/${id}`,
            method: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(prescription),
            success: function () {
                $('#editPrescriptionModal').modal('hide');
                loadPrescriptions();
                showSuccessMessage('✅ Prescription updated successfully.');
            },
            error: function () {
                showErrorMessage('Failed to update prescription.');
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
