$('#addTestForm').submit(function (e) {
    e.preventDefault();

    const patientId = $('#addPatientId').val();
    const doctorId = $('#addDoctorId').val();
    const testTypeId = $('#addTestTypeId').val();
    const cost = $('#addCost').val();
    const status = $('#addStatus').val();
    const notes = $('#addNotes').val();
    const createdAt = new Date().toISOString().split('T')[0]; // yyyy-MM-dd

    const fileInput = $('#addTestResult')[0];
    let testResult = null;

    if (fileInput.files.length > 0) {
        const fileName = fileInput.files[0].name;
        testResult = `C:\\Files\\${fileName}`;
    }

    const newTest = {
        patientID: patientId,
        doctorID: doctorId,
        testTypeID: testTypeId,
        cost: parseFloat(cost),
        status: parseInt(status),
        notes: notes,
        testResult: testResult,
        createdAt: createdAt
    };

    $.ajax({
        url: 'https://localhost:7119/api/Tests/add',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(newTest),
        success: function () {
            const modal = bootstrap.Modal.getInstance(document.getElementById('addTestModal'));
            modal.hide();
            $('#addTestForm')[0].reset();
            showSuccessMessage("Test Added Successfuly")
            loadTests();
        },
        error: function () {
              modal.hide();
            $('#addTestForm')[0].reset();
            showErrorMessage('Failed to add new test.');
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
