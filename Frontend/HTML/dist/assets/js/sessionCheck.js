const  roleId = localStorage.getItem('roleId');

 console.log(roleId);
   if(roleId>0 && !isNaN(roleId)&&roleId!=undefined&&roleId!=null)
    {
            FrowardToDashboardPage(parseInt(roleId));
        }
 
 else 
 {
window.location.href = "login.html";


 }
 
 
 
 
 
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
                    default : window.location.href = "login.html"; break;
                    
                }

}


  