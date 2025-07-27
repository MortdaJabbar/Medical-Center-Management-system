 function confirmDeleteStaffModal(staffId, fullName, imagePath) {
      $("#deleteStaffId").val(staffId);
      $("#deleteStaffName").text(fullName);
      $("#deleteStaffImage").attr("src", imagePath ?? '');

      const modal = new bootstrap.Modal(document.getElementById('deleteStaffModal'));
      modal.show();
  }

  // تنفيذ الحذف
  function deleteStaffConfirmed() {
      const staffId = $("#deleteStaffId").val();

      $.ajax({
          url: `https://localhost:7119/api/Staff/delete/${staffId}`,
          method: "DELETE",
          success: function () {
              $('#deleteStaffModal').modal('hide');
              showSuccessMessage("✅ Staff deleted successfully.");
              fetchStaff(); // إعادة تحميل الجدول
          },
          error: function () {
              showErrorMessage("❌ Failed to delete staff.");
          }
      });
  }

  // رسائل تنبيه
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