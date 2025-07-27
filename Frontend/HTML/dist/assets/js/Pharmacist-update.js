 let Pharmacistid  = "";
function showUpdatePharmacistModal(id) {
    Pharmacistid=id;
    $.ajax({
        url: `https://localhost:7119/api/Pharmacists/${Pharmacistid}`,
        method: "GET",
        success: function (p) {
            $('#PharmacistId').val(p.pharmacistId);
            $('#firstName').val(p.person.firstName);
            $('#secondName').val(p.person.secondName);
            $('#thirdName').val(p.person.thirdName ?? '');
            $('#dateOfBirth').val(p.person.dateOfBirth);
            $('#email').val(p.person.email);
            $('#phone').val(p.person.phone);
            $('#gender').val(p.person.gender);
            $('#licenseNumber').val(p.licenseNumber);
            $('#hireDate').val(p.hireDate); // only date
            $('#experienceYears').val(p.expereinceYears ?? '');
            $('#currentImage').attr('src', p.person.imageLocation);
            $('#UpdatedimageLocation').val(p.person.imageLocation);

            $('#updatePharmacistModal').modal('show');
        },
        error: () => showErrorMessage("Unable to load pharmacist data for update.")
    });

    
}

document.getElementById("imageUpload").addEventListener("change", function () {
    const file = this.files[0]
   
        // تحديث صورة المعاينة
        if(file){
        document.getElementById("currentImage").src = "C:/Images/" + file.name;

        // تحديث قيمة الحقل المخفي حتى تنرسل الصورة الجديدة
        document.getElementById("UpdatedimageLocation").value = "C:/Images/" + file.name;
        }
});

$('#updatePharmacistForm').submit(function (e) {
    e.preventDefault();

  
    const updatedPharmacist = {
        person: {
            firstName: $('#firstName').val().trim(),
            secondName: $('#secondName').val().trim(),
            thirdName: $('#thirdName').val().trim(),
            email: $('#email').val().trim(),
            phone: $('#phone').val().trim(),
            dateOfBirth: $('#dateOfBirth').val(),
            gender: $('#Gender').val() === "true",
            imageLocation: $('#UpdatedimageLocation').val()
        },
        licenseNumber: $('#licenseNumber').val().trim(),
        hireDate: $('#hireDate').val(),
        expereinceYears: parseInt($('#experienceYears').val()) || null
    };

    

    $.ajax({
        url: `https://localhost:7119/api/Pharmacists/update/${Pharmacistid}`,
        method: "PUT",
        contentType: "application/json",
        data: JSON.stringify(updatedPharmacist),
        success: () => {
            $('#updatePharmacistModal').modal('hide');
            showSuccessMessage("Pharmacist updated successfully!");
            fetchPharmacists();
        },
          error: function(err) 
        {
          //  $('#updatePharmacistForm').reset();
            $('#updatePharmacistModal').modal('hide');
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
});



