function showUpdateStaffModal(staffId) {
    $.ajax({
        url: `https://localhost:7119/api/Staff/${staffId}`,
        method: "GET",
        success: function (staff) {
            const p = staff.person;

            $("#updateStaffId").val(staff.staffId);

            $("#updateFirstName").val(p.firstName);
            $("#updateSecondName").val(p.secondName);
            $("#updateThirdName").val(p.thirdName ?? '');
            $("#updateDob").val(p.dateOfBirth);
            $("#updateGender").val(p.gender.toString());
            $("#updatePhone").val(p.phone);
            $("#updateEmail").val(p.email);
            $("#updateHireDate").val(staff.hireDate);
            $("#updateIsAdmin").prop("checked", staff.isAdmin);

            // Set current image
            const imagePath = p.imageLocation ?? '';
            $("#UpdatedimageLocation").val(imagePath);
            $("#currentImage").attr("src", imagePath);

            const modal = new bootstrap.Modal(document.getElementById('updateStaffModal'));
            modal.show();
        },
        error: function () {
            showErrorMessage("❌ Failed to load staff data.");
        }
    });
}

// Handle image selection + update preview + store path
document.getElementById('imageUpload').addEventListener('change', function () {
    const file = this.files[0];
    if(file)
        {
    const fileName = file.name;
    const fullPath = `C:/images/${fileName}`;
    }
    
    document.getElementById('UpdatedimageLocation').value = fullPath;
});

function submitStaffUpdate() {
    const staffId = $("#updateStaffId").val();

    const updatedStaff = {
        person: {
            firstName: $("#updateFirstName").val(),
            secondName: $("#updateSecondName").val(),
            thirdName: $("#updateThirdName").val(),
            dateOfBirth: $("#updateDob").val(),
            gender: $("#updateGender").val() === "true",
            phone: $("#updatePhone").val(),
            email: $("#updateEmail").val(),
            imageLocation: $("#UpdatedimageLocation").val()
        },
        isAdmin: $("#updateIsAdmin").is(":checked"),
        hireDate: $("#updateHireDate").val()
    };

    $.ajax({
        url: `https://localhost:7119/api/Staff/update/${staffId}`,
        method: "PUT",
        contentType: "application/json",
        data: JSON.stringify(updatedStaff),
        success: function () {
            $('#updateStaffModal').modal('hide');
            showSuccessMessage("✅ Staff updated successfully.");
            fetchStaff(); // reload table
        },
        error: function () {
            showErrorMessage("❌ Failed to update staff.");
        }
    });
}

// Optional: global alert functions
function showErrorMessage(message) {
    $('#errorAlert').remove();
    $('body').append(`
        <div id="errorAlert" class="alert alert-danger alert-dismissible fade show position-fixed top-0 end-0 m-4" role="alert" style="z-index: 9999; min-width: 300px;">
            <i class="uil uil-exclamation-circle"></i> <strong>Error:</strong> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `);
    setTimeout(() => { $('#errorAlert').alert('close'); }, 5000);
}

function showSuccessMessage(message) {
    $('#successAlert').remove();
    $('body').append(`
        <div id="successAlert" class="alert alert-success alert-dismissible fade show position-fixed top-0 end-0 m-4" role="alert" style="z-index: 9999; min-width: 600px;">
            <i class="uil uil-check-circle"></i> <strong>Success:</strong> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `);
    setTimeout(() => { $('#successAlert').alert('close'); }, 5000);
}
