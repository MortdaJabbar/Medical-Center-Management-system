
$('.login-form').on('submit', function (e) {
    e.preventDefault();

    const email = $('#email').val().trim();
    const password = $('#password').val();
    const rememberMe = $('#remember-check').is(':checked');

    $.ajax({
        url: 'https://localhost:7119/api/auth/login',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ email, password }),
        xhrFields: { withCredentials: true },

       success: function (data) {

    // 🔐 2FA required
    if (data.message && data.message.includes("2FA")) {

        localStorage.setItem('tempUserId', data.userId);

        Swal.fire({
            icon: 'info',
            title: 'Two-Factor Authentication',
            text: 'Code has been sent to your email please enter it to log in',
            confirmButtonText: 'Proceed'
        }).then(() => {
            window.location.href = "TwoFactorAuthentication.html";
        });

        return;
    }

    // ✅ Successful login
    if (data.roleId) {

        localStorage.setItem('roleId', parseInt(data.roleId));
        localStorage.setItem('userId', data.userId);
        localStorage.setItem('entityId', data.entityId);
        localStorage.setItem('role', data.role);

        FrowardToDashboardPage(parseInt(data.roleId));
        return;
    }

    // ❌ Fallback
    Swal.fire({
        icon: 'error',
        title: 'Login Failed',
        text: 'Incorrect email or password.'
    });
},

        // ⚠️ Server error
        error: function (xhr) {
            Swal.fire({
                icon: 'error',
                title: 'Server Error',
                text: xhr.responseText
            });
            
             
        }
    });
});



function FrowardToDashboardPage(roleId)
{

  switch (roleId) {
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
                        Swal.fire({
                            icon: 'error',
                            title: 'Unknown Role',
                            text: 'Your role is not recognized. Please contact support.'
                        });
                        break;
                }

}