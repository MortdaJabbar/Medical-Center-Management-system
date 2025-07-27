

// Function to fetch and populate the modal with the patient's current details
function showUpdatePatientModal(patientId) {
    // Fetch patient data from the API using the patientId
    $.ajax({
        url: `https://localhost:7119/api/Patients/${patientId}`,
        method: "GET",
        success: function(data) {
            // Populate the modal fields with patient data
            $('#patientId').val(data.patientId);
            $('#firstName').val(data.person.firstName);
            $('#secondName').val(data.person.secondName);
            $('#thirdName').val(data.person.thirdName);
            $('#email').val(data.person.email);
            $('#phone').val(data.person.phone);
            $('#dateOfBirth').val(data.person.dateOfBirth);
            $('#weight').val(data.weight);
            $('#height').val(data.height);
            $('#imageLocation').val(data.person.imageLocation);
            $('#gender').val(data.person.gender.toString());
            $('#currentImage').attr("src", data.person.imageLocation);

         
            // Show the modal            
            $('#updatePatientModal').modal('show');
        },
        error: function(err) {
            handleError(err);
        }
    });
}

// change the image on select 
$('#imageToUpload').on('change', function(event) {

    const file = event.target.files[0].name;
    const Fullpath = "C:/Users/User/Desktop/Doctris_v1.4.0/HTML/dist/assets/images/client/"+file;
    $('#currentImage').attr("src", Fullpath); 
    $('#UpdatedimageLocation').val(Fullpath);
     
   }
);

// Handle the update form submission
$('#updatePatientForm').on('submit', function(event) {
    event.preventDefault(); // Prevent the form from submitting normally

    // Prepare the data to be sent in the PUT request
    const updatedPatientData = {
        patientId: $('#patientId').val(),
        weight: $('#weight').val(),
        height: $('#height').val(),
        person: {
            personId: $('#personId').val(),
            firstName: $('#firstName').val(),
            secondName: $('#secondName').val(),
            thirdName:$('#thirdName').val(), 
            dateOfBirth: $('#dateOfBirth').val(), 
            gender: $('#gender').val() === "true",
            phone: $('#phone').val(),
            email: $('#email').val(),
            imageLocation: $('#UpdatedimageLocation').val()  // Use the updated image URL
        }
       
    };
   
    // Send the update request to the API
    $.ajax(
        {
        url: `https://localhost:7119/api/Patients/update?PatientId=${updatedPatientData.patientId}`,
        method: "PUT",
        contentType: "application/json",
        data: JSON.stringify(updatedPatientData),
        success: function(response) 
        {
            // Show success message and refresh the table
                
            fetchPatients(); 
            $('#updatePatientModal').modal('hide');
          
        },
        error: function(err) 
        {
          
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
        }
    );
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
