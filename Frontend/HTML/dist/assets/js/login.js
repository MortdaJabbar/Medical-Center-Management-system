
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
            // ✅ Successful login with token
            if (data.token) {
                const storage = localStorage;

                storage.setItem('token', data.token);
                storage.setItem('roleId', parseInt(data.roleId));
                storage.setItem('userId', data.userId);
                storage.setItem('entityId', data.entityId);
                storage.setItem('role', data.role);

                FrowardToDashboardPage(parseInt(data.roleId));
            }

            // 🔐 2FA required
            else if (data.message && data.message.includes("2FA")) {
                localStorage.setItem('tempUserId', data.userId);

                Swal.fire({
                    icon: 'info',
                    title: 'Two-Factor Authentication ',
                    text: 'code has been sent to your email please enter it to log in ',
                    confirmButtonText: 'Proceed'
                }).then(() => {
                    window.location.href = "TwoFactorAuthentication.html";
                });
            }

            // ❌ Login failed
            else {
                Swal.fire({
                    icon: 'error',
                    title: 'Login Failed',
                    text: 'Incorrect email or password.'
                });
            }
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