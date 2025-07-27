let is2FAEnabled= false;
$(document).ready(function () {
    const userId = localStorage.getItem("userId");

    // Load user 2FA status
    $.get(`https://localhost:7119/api/Users/${userId}`, function (user) {
         is2FAEnabled = user.is2FAEnabled;

        const statusText = is2FAEnabled ? 'enabled' : 'disabled';
        const btnClass = is2FAEnabled ? 'btn-outline-danger' : 'btn-outline-success';
        const btnText = is2FAEnabled ? 'Disable 2FA' : 'Enable 2FA';

        $("#2fa-status").html(`2FA is <strong>${statusText}</strong>.`);
        $("#toggle2faBtn")
            .removeClass('btn-outline-success btn-outline-danger')
            .addClass(btnClass)
            .text(btnText)
            .data("enabled", is2FAEnabled); // store current status
    });

    // Handle change password
    $("#changePasswordForm").submit(function (e) {
        e.preventDefault();

        const currentPassword = $("#currentPassword").val();
        const newPassword = $("#newPassword").val();

        $.ajax({
            url: `https://localhost:7119/api/Users/change-password/${userId}`,
            method: "PUT",
            contentType: "application/json",
            data: JSON.stringify({
                oldPassword: currentPassword,
                newPassword: newPassword
            }),
            success: function () {
                showSuccessMessage("Password updated successfully.");
                $("#changePasswordForm")[0].reset();
            },
            error: function (xhr) {
                showErrorMessage("Failed to update password");
                 console.log(xhr.responseText);
            }
        });
    });

    // Handle toggle 2FA
    $("#toggle2faBtn").click(function () {
         
        const newStatus = !is2FAEnabled;

        $.ajax({
            url: `https://localhost:7119/api/Users/2FA/${userId}`,
            method: "PUT",
            contentType: "application/json",
            data: JSON.stringify(newStatus),
            success: function () {
                const statusText = newStatus ? 'enabled' : 'disabled';
                const btnClass = newStatus ? 'btn-outline-danger' : 'btn-outline-success';
                const btnText = newStatus ? 'Disable 2FA' : 'Enable 2FA';

                $("#2fa-status").html(`2FA is <strong>${statusText}</strong>.`);
                $("#toggle2faBtn")
                    .removeClass('btn-outline-success btn-outline-danger')
                    .addClass(btnClass)
                    .text(btnText)
                    .data("enabled", newStatus);

                showSuccessMessage(`Two-Factor Authentication ${newStatus ? 'enabled' : 'disabled'} successfully.`);
               location.reload();
            },
            error: function (xhr) {
                showErrorMessage("Failed to update 2FA: " );
                console.log(xhr.responseText);
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
