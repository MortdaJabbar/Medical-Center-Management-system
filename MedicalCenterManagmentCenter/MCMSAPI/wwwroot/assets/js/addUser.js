$('#addUserAccountForm').submit(function (e) {
  e.preventDefault();

  const password = $('#password').val();
  const confirmPassword = $('#confirmPassword').val();
 

  const data = {
    personId: $('#personId').val(),
    email: $('#email').val(),
    password: password,
    roleId: parseInt($('#role').val())
  };

  $.ajax({
    url: "https://localhost:7119/api/Auth/register",
    method: "POST",
    contentType: "application/json",
    data: JSON.stringify(data),
    success: function () {
       $('#addUserAccountForm')[0].reset();

   
        $('#successModal').modal('show');
   
      
    },
    error: function (xhr) {
      alert("Account creation failed: " + xhr.responseText);
    }
  });
});
