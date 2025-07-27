var  fullName         =""; 
var  deletePatientId  =""; 
var  deletePersonId   ="";
var  ImageUrl         = "";
    


// Call this function when Delete button clicked
function openDeleteModal(patientId) 
{
    $.ajax({
        url: `https://localhost:7119/api/Patients/${patientId}`, // Use the patientId in the URL
        method: 'GET',
        success: function(patient) {
        
              fullName        = `${patient.person.firstName} ${patient.person.secondName} ${patient.person.thirdName ?? ''}`;
             
              ImageUrl        = patient.person.imageLocation;
             
              deletePatientId = patientId;
             
              deletePersonId  = patient.person.personId;


              $('#deletePatientName').text(fullName);
              $('#deletePatientImage').attr('src', ImageUrl);
              $('#confirmDeleteModal').modal('show');

        },
        error: function (err) {
            $('#confirmDeleteModal').modal('hide');
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

// Handle confirm delete button
$('#confirmDeleteButton').on('click', function () {
    $.ajax({
        url: `https://localhost:7119/api/Patients/Delete/${deletePatientId}/person/${deletePersonId}`,
        method: "DELETE",
        success: function (response) {
            $('#confirmDeleteModal').modal('hide');
            showSuccessMessage("Patient deleted successfully!");
            fetchPatients(); // Refresh patient list
        },
        error: function (err) {
            $('#confirmDeleteModal').modal('hide');
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
