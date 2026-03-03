


    // check






    const inputs = document.querySelectorAll('.otp-input');
    const countdownElement = document.getElementById('countdown');

    let timeLeft;
    let timerId;
    let codeSubmitted = false;

    // Auto-focus behavior
    inputs.forEach((input, index) => {
        input.addEventListener('keyup', (e) => {
            if (e.key >= 0 && e.key <= 9) {
                if (index < inputs.length - 1) inputs[index + 1].focus();
            } else if (e.key === "Backspace") {
                if (index > 0 && !input.value) inputs[index - 1].focus();
            }
        });
    });

    function startCountdown() {
        timeLeft = 5 * 60; // 5 minutes
        countdownElement.classList.remove('d-none');
        updateCountdown();
    }

    function updateCountdown() {
        const minutes = String(Math.floor(timeLeft / 60)).padStart(2, '0');
        const seconds = String(timeLeft % 60).padStart(2, '0');
        countdownElement.textContent = `Resend available in ${minutes}:${seconds}`;
        if (timeLeft > 0) {
            timeLeft--;
            timerId = setTimeout(updateCountdown, 1000);
        } else {
            if (!codeSubmitted) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Time Expired',
                    text: 'Verification time has expired. Please login again.',
                    confirmButtonText: 'OK'
                }).then(() => {
                    localStorage.removeItem('tempUserId');
                    window.location.href = "login.html";
                });
            }
        }
    }

    // Auto-start countdown on page load
    window.onload = function () {
        if (countdownElement) {
            startCountdown();
        }
    };

    // 2FA Submit Handler
    document.querySelector('form').addEventListener('submit', function (e) {
        e.preventDefault();

        const code = Array.from(document.querySelectorAll('.otp-input'))
            .map(input => input.value)
            .join('');

        if (code.length !== 6) {
            Swal.fire({
                icon: 'info',
                title: 'Incomplete Code',
                text: 'Please enter all 6 digits.'
            });
            return;
        }

        const userId = localStorage.getItem('tempUserId');
        if (!userId) {
            Swal.fire({
                icon: 'error',
                title: 'Session Expired',
                text: 'Your session has expired. Please login again.'
            }).then(() => {
                window.location.href = "login.html";
            });
            return;
        }

        codeSubmitted = true; // stop timer-based redirection

        fetch(`${AuthClient.apiBase}/api/Auth/confirm-2fa?userId=${userId}&code=${code}`, {
            method: "POST",
            credentials: 'include'
        })
        .then(async res => {
            if (!res.ok) {
                const txt = await res.text();
                throw new Error(txt || 'Verification failed');
            }
            return res.json();
        })
        .then(data => {
            // Server sets HttpOnly cookies; server also returns user claims in body.
            if (data && data.roleId) {
                localStorage.removeItem('tempUserId');
                localStorage.setItem('roleId', data.roleId);
                localStorage.setItem('userId', data.userId);
                localStorage.setItem('entityId', data.entityId);
                localStorage.setItem('role', data.role);

                Swal.fire({
                    icon: 'success',
                    title: 'Verified',
                    text: '2FA code verified successfully!'
                }).then(() => {
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
                            Swal.fire({
                                icon: 'error',
                                title: 'Unknown Role',
                                text: 'Your role is not recognized. Please login again.'
                            }).then(() => {
                                window.location.href = "login.html";
                            });
                            break;
                    }
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Invalid Code',
                    text: data.message || 'The code is incorrect. Please try again.'
                });

                document.querySelectorAll('.otp-input').forEach(input => input.value = '');
                inputs[0].focus();
                codeSubmitted = false; // allow timeout again
            }
        })
        .catch(err => {
            console.error(err);
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: err.message || 'Something went wrong. Please try again.'
            }).then(() => {
                window.location.href = "login.html";
            });
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
                    
                }

}



