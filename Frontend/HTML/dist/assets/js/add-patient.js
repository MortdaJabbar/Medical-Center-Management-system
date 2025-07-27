document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("addPatientForm");
   const imagepath = "C:/Images/";
   const imageName = "00.jpg";



const imageUploadInput = document.getElementById("imageUpload");
const currentImage = document.getElementById("currentImage");
let  fullimagePath = "";
imageUploadInput.addEventListener("change", function () {
    const file = imageUploadInput.files[0];
    if (file) {
       
            currentImage.src = imagepath+file?.name; // يعرض الصورة فورًا
        
    }

   fullimagePath = currentImage.src; 
   

});





    form.addEventListener("submit", function (e) {
        e.preventDefault();
 
       
        const patient = {
            person: {
                firstName: document.getElementById("firstName").value.trim(),
                secondName: document.getElementById("secondName").value.trim(),
                thirdName: document.getElementById("thirdName").value.trim(),
                email: document.getElementById("email").value.trim(),
                phone: document.getElementById("phone").value.trim(),
                dateOfBirth: document.getElementById("dateOfBirth").value,
                gender: document.getElementById("gender").value === "true",
                imageLocation: fullimagePath
            },
            weight: parseFloat(document.getElementById("weight").value),
            height: parseFloat(document.getElementById("height").value)
        };

        $.ajax({
  url: "https://localhost:7119/api/Patients/add",
  method: "POST",
  contentType: "application/json",
  data: JSON.stringify(patient), 
  success: function(response) {
      showSuccessMessage("Patient Add Successfuly");
                form.reset();
                currentImage.src="C:/Images/00.jpg";
  },
  error: function(xhr) {
     form.reset();
                currentImage.src="C:/Images/00.jpg";
    switch (xhr.status) {
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
