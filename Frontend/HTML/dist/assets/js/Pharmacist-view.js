function viewPharmacist(pharmacistId) {
    
    $.ajax({
        url: `https://localhost:7119/api/Pharmacists/${pharmacistId}`,
        method: "GET",
        success: function (pharmacist) {
            const p = pharmacist;
           
            $('#PharmacistFullName').text(`${p.person.firstName} ${p.person.secondName} ${p.person.thirdName ?? ''}`);
            $('#PharmacistDOB').text(p.person.dateOfBirth);
            $('#PharmacistGender').text(p.person.gender ? "Male" : "Female");
            $('#PharmacistEmail').text(p.person.email);
            $('#PharmacistPhone').text(p.person.phone);
            $('#PharmacistLicense').text(p.licenseNumber);
            $('#PharmacistHireDate').text(p.hireDate);
            $('#PharmacistExperience').text(p.expereinceYears ?? "N/A");
            $('#PharmacistImage').attr("src", p.person.imageLocation);

            $('#viewPharmacistModal').modal('show');
        },
        error: function() 
        {
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


