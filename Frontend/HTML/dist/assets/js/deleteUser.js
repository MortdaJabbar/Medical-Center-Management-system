function showDeleteUserModal(button) {
    const userId = $(button).data("id");
    const userName = $(button).data("name");
    const userImg = $(button).data("img");

    $('#DeleteUserId').val(userId);
    $('#deleteUserName').text(userName);
    $('#deleteUserImage').attr('src', userImg);
    $('#deleteUserModal').modal('show');
}

function deleteUser() {
    const userId = $('#DeleteUserId').val();

    $.ajax({
        url: `https://localhost:7119/api/Users/${userId}`,
        method: "DELETE",
        success: function () {
            $('#deleteUserModal').modal('hide');
            loadUsers(); // Reload table after deletion
        },
        error: function (xhr) {
            console.error("Failed to delete user:", xhr.responseText);
            alert("Failed to delete user.");
        }
    });
}
