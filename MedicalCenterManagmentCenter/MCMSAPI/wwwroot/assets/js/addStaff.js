document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("addStaffForm"); // اسم الفورم بقى نفسه
    const imagePath = "C:/Images/";

    // ✅ عرض الصورة عند اختيارها
    document.getElementById("imageUpload").addEventListener("change", function () {
        const file = this.files[0].name;
        document.getElementById("currentImage").src = imagePath + file;
    });

    // ✅ عند إرسال النموذج
    form.addEventListener("submit", function (e) {
        e.preventDefault();

        const staff = {
            person: {
                firstName: document.getElementById("firstName").value.trim(),
                secondName: document.getElementById("secondName").value.trim(),
                thirdName: document.getElementById("thirdName").value.trim(),
                email: document.getElementById("email").value.trim(),
                phone: document.getElementById("phone").value.trim(),
                dateOfBirth: document.getElementById("dateOfBirth").value,
                gender: document.getElementById("gender").value === "true",
                imageLocation: document.getElementById("currentImage").src
            },
            isAdmin: document.getElementById("isAdmin").checked,
            hireDate: document.getElementById("hireDate").value
        };

        $.ajax({
  url: "https://localhost:7119/api/Staff/add",
  method: "POST",
  contentType: "application/json",
  data: JSON.stringify(staff), 
  success: function(response) {
    showSuccessMessage("✅ Staff added successfully");
            form.reset();
            document.getElementById("currentImage").src = imagePath + "00.jpg"; // إعادة الصورة الأصلية
  },
  error: function(xhr) {
    form.reset();
            showErrorMessage("❌ Failed to add staff");
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
