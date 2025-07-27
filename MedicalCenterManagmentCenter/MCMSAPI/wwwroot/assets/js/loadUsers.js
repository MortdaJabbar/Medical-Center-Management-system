function loadUsers() {
    $.ajax({
        url: "https://localhost:7119/api/Users/detailed",
        method: "GET",
        success: function (users) {
            $("#Users-body").empty();

            users.forEach(user => {
                const row = `
                    <tr>
                        <td class="p-3 d-flex align-items-center">
                            <img src="${user.userImage}" alt="User" class="avatar avatar-md-sm rounded-circle me-2" width="40" height="40">
                            <span>${user.fullName}</span>
                        </td>
                        <td class="p-3">${user.email}</td>
                        <td class="p-3">${user.role}</td>
                        <td class="p-3">${user.isActive ? "✔️" : "❌"}</td>
                        <td class="p-3">${user.is2FAEnabled ? "✔️" : "❌"}</td>
                        <td class="p-3 text-end">
                            <button class="btn btn-sm btn-primary me-1" onclick="showUpdateUserModal('${user.userId}')">
                                <i class="uil uil-pen"></i>
                            </button>
                            <button 
                class="btn btn-sm btn-danger"
                onclick="showDeleteUserModal(this)"
                data-id="${user.userId}"
                data-name="${user.fullName}"
                data-img="${user.userImage}">
                <i class="uil uil-trash"></i>
            </button>
                        </td>
                    </tr>
                `;
                $("#Users-body").append(row);
            });
        },
        error: function (xhr) {
            
        }
    });
}

// Call on page load
$(document).ready(function () {
    loadUsers();
});
