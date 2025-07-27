
$('.login-form').on('submit', function (e) {
    e.preventDefault();

    const email = $('#email').val().trim();
    const password = $('#password').val();
    const rememberMe = $('#rememberMe').is(':checked');

    $.ajax({
        url: 'https://localhost:7119/api/auth/login',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ email, password }),
        success: function (data) {
              
            // إذا رجع توكن → سجل دخول
            if (data.token) {
                const storage =   localStorage ;

                storage.setItem('token', data.token);
                storage.setItem('roleId', data.roleId);
                storage.setItem('userId', data.userId);
                storage.setItem('entityId', data.entityId);
                storage.setItem('role',data.role)
                    
               
                switch (parseInt(data.roleId)) {
                    case 1:
                        window.location.href = "admin-dashboard.html";
                        break;
                    case 2:
                        window.location.href = "doctor-dashboard.html";
                        break;
                    case 3:
                        window.location.href = "patient-dashboard.html";
                        break;
                    case 4:
                        window.location.href = "pharmacist-dashboard.html";
                        break;
                    case 5:
                        window.location.href = "staff-dashboard.html";
                        break;
                    default:
                        alert("Unknown role. Contact support.");
                        break;
                }
            }

            // إذا 2FA مفعّل
            else if (data.message && data.message.includes("2FA")) {
                sessionStorage.setItem('tempUserId', data.userId);
                
                window.location.href = "TwoFactorAuthentication.html";
            }

            // إذا فشل
            else {
                console.log("failed");
            }
        },
        error: function (xhr) {
            console.error("Login error:", xhr);
            
        }
    });
});

