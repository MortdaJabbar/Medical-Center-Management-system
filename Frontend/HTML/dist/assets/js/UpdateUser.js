let currentUserId = "";

function showUpdateUserModal(userId) {
    currentUserId = userId;


    $.ajax({
        url: `https://localhost:7119/api/Users/${currentUserId}`,
        method: "GET",
        success: function (user) {
            $('#UserId').val(user.userId);
            $('#Email').val(user.email);
            $('#IsActive').prop('checked', user.isActive);
            $('#Is2FAEnabled').prop('checked', user.is2FAEnabled);

            $('#updateUserModal').modal('show');
        },
        error: function (xhr) {
            console.error("Failed to load user:", xhr.responseText);
            alert("Error loading user data.");
        }
    });
}

function updateUser() {
    const updatedUser = {
        
        email: $('#Email').val(),
        isActive: $('#IsActive').is(':checked'),
        is2FAEnable: $('#Is2FAEnabled').is(':checked')
    };

    $.ajax({
        url: `https://localhost:7119/api/Users/Update/${currentUserId}`,
        method: "PUT",
        contentType: "application/json",
        data: JSON.stringify(updatedUser),
        success: function () {
            $('#updateUserModal').modal('hide');
            loadUsers();
        },
        error: function (xhr) {
            
            alert("Error updating user.");
        }
    });
}
