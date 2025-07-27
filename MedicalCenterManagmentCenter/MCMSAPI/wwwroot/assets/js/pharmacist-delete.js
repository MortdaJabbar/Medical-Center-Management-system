// Variables to store selected pharmacist's info
let deletePharmacistId = "";
let pharmacistFullName = "";
let pharmacistImageUrl = "";

// Function to open the delete confirmation modal (uses data directly from row)
function confirmDeletePharmacistModal(pharmacistId, fullName, imageUrl) {
    deletePharmacistId = pharmacistId;
     
    pharmacistFullName = fullName;
    pharmacistImageUrl = imageUrl;

    $('#deletePharmacistName').text(pharmacistFullName);
    $('#deletePharmacistImage').attr('src', pharmacistImageUrl);
    $('#confirmDeletePharmacistModal').modal('show');
    
}



// Confirm delete click handler
$('#confirmDeletePharmacistButton').on('click', function () {
    $.ajax({
        url: `https://localhost:7119/api/Pharmacists/Delete/${deletePharmacistId}`,
        method: 'DELETE',
        success: function () {
            $('#confirmDeletePharmacistModal').modal('hide');
            showSuccessMessage("Pharmacist deleted successfully!");
            fetchPharmacists(); // Reload pharmacist list (you must have this function)
        },
        error: function (err) {
            $('#confirmDeletePharmacistModal').modal('hide');
            switch (err.status) {
                case 404:
                    showErrorMessage("The requested pharmacist could not be found.");
                    break;
                case 500:
                    showErrorMessage("Something went wrong on the server. Please try again later.");
                    break;
                case 0:
                    showErrorMessage("Cannot connect to the server. Check your internet or server status.");
                    break;
                default:
                    showErrorMessage("Something went wrong. Try again later.");
                    break;
            }
        }
    });
});
