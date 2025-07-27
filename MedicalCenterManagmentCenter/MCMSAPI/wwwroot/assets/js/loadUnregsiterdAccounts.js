function loadAvailableEntities() {
  const roleId = $('#role').val();
  let url = "";

  switch (parseInt(roleId)) {
    case 2: url = "https://localhost:7119/api/Users/unregistered-doctors"; break;
    case 3: url = "https://localhost:7119/api/Users/unregistered-patients"; break;
    case 4: url = "https://localhost:7119/api/Users/unregistered-pharmacists"; break;
    case 5: url = "https://localhost:7119/api/Users/unregistered-staff"; break;
    default: return;
  }

 $.ajax({
  url: url,
  method: "GET",
  success: function(data) {
    const select = $('#personId');
    select.empty();
    select.append(`<option disabled selected>Choose from list</option>`);

    data.forEach(item => {
      const id = item.doctorId || item.patientId || item.pharmacistId || item.staffId;
      select.append(`<option value="${id}">${item.fullName}</option>`);
    });
  },
  error: function(xhr) {
     
    showErrorMessage("❌ Failed to load account list.");
  }
});

}








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
