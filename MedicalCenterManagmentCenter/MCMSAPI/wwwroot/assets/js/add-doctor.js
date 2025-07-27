document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("addDoctorForm"); // same form ID used
    const imagePath = "C:/Images/";

    form.addEventListener("submit", function (e) {
        e.preventDefault();

        const doctor = {
            person: {
                firstName: document.getElementById("firstName").value.trim(),
                secondName: document.getElementById("secondName").value.trim(),
                thirdName: document.getElementById("thirdName").value.trim(),
                email: document.getElementById("email").value.trim(),
                phone: document.getElementById("phone").value.trim(),
                dateOfBirth: document.getElementById("dateOfBirth").value,
                gender: document.getElementById("gender").value === "true",
                imageLocation: imagePath + document.getElementById("imageUpload").files[0]?.name
            },
            specialization: document.getElementById("specialization").value.trim(),
            experienceyears: parseInt(document.getElementById("experienceyears").value),
            available: document.getElementById("available").value === "true" 
        };

        $.ajax({
  url: "https://localhost:7119/api/Doctors/add",
  method: "POST",
  contentType: "application/json",
  data: JSON.stringify(doctor), 
  success: function(response) {
     showSuccessMessage("Doctor added successfully!");
            form.reset();
  },
  error: function(xhr) {
    switch (xhr.status) {
                case 404:
                    showErrorMessage("The requested data could not be found.");
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
              form.reset();
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
