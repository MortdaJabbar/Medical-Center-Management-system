// Function to show the patient's details in the modal
function viewPatient(patientId) {
    // Show a loading spinner or placeholder text while waiting for the data
    $('#viewProfileModal').modal('show'); // Open the modal immediately
    $('#patientFullName').text("Loading..."); // Set initial text
    $('#patientDOB').text("Loading...");
    $('#patientGender').text("Loading...");
    $('#patientEmail').text("Loading...");
    $('#patientPhone').text("Loading...");
    $('#patientWeight').text("Loading...");
    $('#patientHeight').text("Loading...");
    $('#patientImage').attr("src", ""); // Clear the image

    // Make the API request to fetch patient details
    $.ajax({
        url: `https://localhost:7119/api/Patients/${patientId}`, // Use the patientId in the URL
        method: 'GET',
        success: function(patient) {
            // Populate the modal with the patient data
            $('#patientFullName').text(`${patient.person.firstName} ${patient.person.secondName} ${patient.person.thirdName ?? ''}`);
            $('#patientDOB').text(patient.person.dateOfBirth);
            $('#patientGender').text(patient.person.gender ? "Male" : "Female");
            $('#patientEmail').text(patient.person.email);
            $('#patientPhone').text(patient.person.phone);
            $('#patientWeight').text(`${patient.weight} kg`);
            $('#patientHeight').text(`${patient.height} cm`);
            $('#patientImage').attr("src", patient.person.imageLocation); // Set the image source
        },
        error: function(err) {
            
            $('#viewProfileModal').modal('hide'); 
            switch (err.status) {
                case 404:
                    showErrorMessage("The requested data could not be found.");
                    break;
                case 500:
                    showErrorMessage("Something went wrong on our server. Please try again later.");
                case 0 : showErrorMessage("Cannot connect to server. Please check your internet connection or if the server is running.");
                    break;
                default:
                    showErrorMessage("Something went wrong.  Please Check Your internet connection or try again later.");
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


