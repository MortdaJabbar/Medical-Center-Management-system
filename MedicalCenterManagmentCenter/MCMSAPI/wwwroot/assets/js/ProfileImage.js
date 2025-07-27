
let entityId = localStorage.getItem("entityId");

$.get(`https://localhost:7119/api/Auth/profile/${entityId}`, function (data) {
    $('#CurrentName').text(data.fullName);
     $('#CurrentRole').text(data.currentRole);
    $('#profileImage1').attr('src', data.imageLocation);
    $('#profileImage2').attr('src', data.imageLocation);
});
