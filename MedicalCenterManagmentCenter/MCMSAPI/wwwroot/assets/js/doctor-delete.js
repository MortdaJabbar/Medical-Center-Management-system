// Variables to store selected doctor's info
let deleteDoctorId = "";
let deletePersonId = "";
let doctorFullName = "";
let doctorImageUrl = "";

// Function to open the delete confirmation modal (uses data directly from row)
function confirmDeleteDoctorModal(doctorId, personId, fullName, imageUrl) {
    deleteDoctorId = doctorId;
    deletePersonId = personId;
    doctorFullName = fullName;
    doctorImageUrl = imageUrl; 
  
    $('#deleteDoctorName').text(fullName);
    $('#deleteDoctorImage').attr('src', doctorImageUrl);
    $('#confirmDeleteDoctorModal').modal('show');
}

// Confirm delete click handler
$('#confirmDeleteDoctorButton').on('click', function () {

    $.ajax({
        url: `https://localhost:7119/api/Doctors/Delete/${deleteDoctorId}/person/${deletePersonId}`,
        method: 'DELETE',
        success: function () {
            $('#confirmDeleteDoctorModal').modal('hide');
            showSuccessMessage("Doctor deleted successfully!");
            fetchDoctors(); // Reload doctor list
        },
        error: function (err) {
            $('#confirmDeleteDoctorModal').modal('hide');
            switch (err.status) {
                case 404:
                    showErrorMessage("The requested doctor could not be found.");
                    break;
                case 500:
                    showErrorMessage("Something went wrong on the server. Please try again later.");
                    break;
                case 0:
                    showErrorMessage("Cannot connect to the server. Check your internet or server status.");
                    break;
                default:
                    showErrorMessage("Something went wrong. Try again later.");
                    break;
            }
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
