// Function to show the doctor's details in the modal
function viewDoctor(doctorId) {
    // Open the modal immediately
    $('#viewDoctorModal').modal('show');
    window.scrollTo({ top: 0, behavior: 'smooth' });
    // Show placeholders while loading
    $('#doctorFullName').text("Loading...");
    $('#doctorDOB').text("Loading...");
    $('#doctorGender').text("Loading...");
    $('#doctorEmail').text("Loading...");
    $('#doctorPhone').text("Loading...");
    $('#doctorSpecialization').text("Loading...");
    $('#doctorExperience').text("Loading...");
    $('#doctorAvailable').text("Loading...");
    $('#doctorImage').attr("src", ""); // Clear the image

    // Make the API request to fetch doctor details
    $.ajax({
        url: `https://localhost:7119/api/Doctors/${doctorId}`, // Adjust the URL for doctors
        method: 'GET',
        success: function(doctor) {
            // Populate the modal with doctor data
            $('#doctorFullName').text(`${doctor.person.firstName} ${doctor.person.secondName} ${doctor.person.thirdName ?? ''}`);
            $('#doctorDOB').text(doctor.person.dateOfBirth);
            $('#doctorGender').text(doctor.person.gender ? "Male" : "Female");
            $('#doctorEmail').text(doctor.person.email);
            $('#doctorPhone').text(doctor.person.phone);
            $('#doctorSpecialization').text(doctor.specialization);
            $('#doctorExperience').text(`${doctor.experienceyears} years`);
            $('#doctorAvailable').text(doctor.available ? "Available" : "Not Available");
            $('#doctorImage').attr("src", doctor.person.imageLocation); // Set doctor image
        },
        error: function(err) {
            $('#viewDoctorModal').modal('hide');
            switch (err.status) {
                case 404:
                    showErrorMessage("The requested doctor could not be found.");
                    break;
                case 500:
                    showErrorMessage("Something went wrong on our server. Please try again later.");
                    break;
                case 0:
                    showErrorMessage("Cannot connect to server. Please check your internet connection or if the server is running.");
                    break;
                default:
                    showErrorMessage("Something went wrong. Please check your internet connection or try again later.");
                    break;
            }
        }
    });
}

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
    

    // Auto close after 3 seconds
    setTimeout(() => { 
        $('#successAlert').alert('close'); 
    }, 5000);
}

