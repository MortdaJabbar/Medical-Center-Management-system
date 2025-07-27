// Function to fetch and populate the modal with the doctor's current details
function showUpdateDoctorModal(doctorId) {
    $.ajax({
        url: `https://localhost:7119/api/Doctors/${doctorId}`,
        method: "GET",
        success: function(data) {
            $('#doctorId').val(data.doctorId);
            $('#firstName').val(data.person.firstName);
            $('#secondName').val(data.person.secondName);
            $('#thirdName').val(data.person.thirdName);
            $('#email').val(data.person.email);
            $('#phone').val(data.person.phone);
            $('#dateOfBirth').val(data.person.dateOfBirth);
            $('#gender').val(data.person.gender.toString());
            $('#specialization').val(data.specialization);
            $('#experience').val(data.experienceyears);
            $('#available').val(data.available.toString());
            $('#UpdatedimageLocation').val(data.person.imageLocation);
            $('#currentImage').attr("src", data.person.imageLocation);

            $('#personId').val(data.person.personId); // Required for PUT body

            $('#updateDoctorModal').modal('show');
        },
        error: function(err) {
            handleError(err);
        }
    });
}

// Change the image on file select
$('#imageToUpload').on('change', function(event) {
    const file = event.target.files[0].name;
    const Fullpath = "C:/Users/User/Desktop/Doctris_v1.4.0/HTML/dist/assets/images/client/" + file;
    $('#currentImage').attr("src", Fullpath);
    $('#UpdatedimageLocation').val(Fullpath);
});

// Handle the update doctor form submission
$('#updateDoctorForm').on('submit', function(event) {
    event.preventDefault();

    const updatedDoctorData = {
        doctorId: $('#doctorId').val(),
        specialization: $('#specialization').val(),
        experienceyears: $('#experience').val(),
        available: $('#available').val() === "true",
        person: {
            personId: $('#personId').val(),
            firstName: $('#firstName').val(),
            secondName: $('#secondName').val(),
            thirdName: $('#thirdName').val(),
            dateOfBirth: $('#dateOfBirth').val(),
            gender: $('#gender').val() === "true",
            phone: $('#phone').val(),
            email: $('#email').val(),
            imageLocation: $('#UpdatedimageLocation').val()
        }
    };

    $.ajax({
        url: `https://localhost:7119/api/Doctors/update/${updatedDoctorData.doctorId}`,
    method: "PUT",
    contentType: "application/json",
    data: JSON.stringify(updatedDoctorData),
        success: function(response) {
            fetchDoctors(); // refresh doctor table
            $('#updateDoctorModal').modal('hide');
            showSuccessMessage("Doctor updated successfully.");
        },
        error: function(err) {
            $('#updateDoctorModal').modal('hide');
            switch (err.status) {
                case 404:
                    showErrorMessage("Doctor not found.");
                    break;
                case 500:
                    showErrorMessage("Server error. Please try again later.");
                    break;
                case 0:
                    showErrorMessage("Cannot connect to server.");
                    break;
                default:
                    showErrorMessage("Unexpected error occurred.");
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
