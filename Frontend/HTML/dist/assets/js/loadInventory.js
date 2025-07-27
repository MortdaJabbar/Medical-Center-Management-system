function loadInventory() {
   $.ajax({
  url: "https://localhost:7119/api/Inventory/AllDetails",
  method: "GET",
  success: function(data) {
    const tbody = $("#inventory-body");
    tbody.empty();

    data.forEach(inv => {
      const row = `
        <tr>
          <td class="p-3">${inv.medicationName ?? inv.medicationID}</td>
          <td class="p-3">${inv.quantity}</td>
          <td class="p-3">${inv.unitOfMeasure}</td>
          <td class="p-3">${inv.supplier}</td>
          <td class="p-3">${inv.expiryDate}</td>
          <td class="p-3">${inv.recivedDate}</td>
          <td class="p-3 text-end">
            <button class="btn btn-sm btn-primary me-2 edit-inv-btn" data-id="${inv.inventoryID}">Edit</button>
            <button class="btn btn-sm btn-danger delete-inv-btn" data-id="${inv.inventoryID}">Delete</button>
          </td>
        </tr>`;
      tbody.append(row);
    });
    PagationDataTable("#myTable",[],10);
  },
  error: function(xhr) {
     tbody.empty();

    showErrorMessage("❌ Failed to load inventory data.");
  }
});

}

$(document).ready(function () {
    loadInventory();
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
